#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Text;
using Quire.Core;
using Quire.Wpf;

namespace Quire;

public partial class FindReplaceRichTextBox
{
    private const int MaxCharactersLimit = 100;

    public FindReplaceRichTextBox()
    {
        AddCommandsHandler();
        PreventTextWrapping();
    }

    private void AddCommandsHandler()
    {
        WpfUtils.AddPreemptiveCommandBinding(this, RichTextBase.ToggleSuperscriptCommand, (object sender, ExecutedRoutedEventArgs e) =>
        {
            e.Handled = true;
        }, null, false);
        WpfUtils.AddPreemptiveCommandBinding(this, ApplicationCommands.Paste, (object sender, ExecutedRoutedEventArgs e) =>
        {
            PrePasting();
            HandlePaste();
            e.Handled = true;
        }, null, false);
    }

    private void PreventTextWrapping()
    {
        Document.PageWidth = 1000.0;
    }

    private void HandlePaste()
    {
        try
        {
            RichTextEmbeddedItemsInfo? richTextEmbeddedItemsInfo = Clipboard.GetDataObject()?.GetTextWithEmbeddedItemsInfo();
            if (richTextEmbeddedItemsInfo.HasValue && richTextEmbeddedItemsInfo.Value != RichTextEmbeddedItemsInfo.None)
            {
                return;
            }
            using (DeclareChangeBlock())
            {
                IClipboard current = WpfClipboard.Current;
                string formattedText = current.GetFormattedText(out var dataFormat, out var isPlainText);
                TextPointer insertPosition = Selection.IsEmpty ? CaretPosition : Selection.Start;
                if (current.HasSExpression())
                {
                    ConvertSExpressionAndPasteText(formattedText, insertPosition);
                }
                else if (isPlainText)
                {
                    InsertPlainText(formattedText, insertPosition);
                }
                else
                {
                    PasteFormattedContent(formattedText, dataFormat);
                }
                RemoveAdditionalParagraphs();
                TruncateTextIfNeeded();
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }
    }

    private void TruncateTextIfNeeded()
    {
        if ((Text?.Trim(new char[2] { '\r', '\n' }) ?? string.Empty).Length > 100)
        {
            TextPointer positionAtTextOffset = RichTextBase.GetPositionAtTextOffset(TextRange.Start, 100);
            if (positionAtTextOffset != null)
            {
                new System.Windows.Documents.TextRange(positionAtTextOffset, TextRange.End).Text = string.Empty;
                CaretPosition = TextRange.End;
            }
        }
    }

    private void InsertPlainText(string formattedText, TextPointer insertPosition)
    {
        Selection.Text = string.Empty;
        Run run = new Run(formattedText, insertPosition);
        CaretPosition = run.ContentEnd;
    }

    private void ConvertSExpressionAndPasteText(string formattedText, TextPointer insertPosition)
    {
        List<(bool, string)> list = ParseFormattedTextToTextSegments(formattedText);
        Selection.Text = string.Empty;
        Run run = null;
        foreach (var item in list)
        {
            if (item.Item1)
            {
                run = new Run(item.Item2, insertPosition);
            }
            else
            {
                run = new SubscriptRun(item.Item2, insertPosition);
            }
            insertPosition = run.ContentEnd;
        }
        CaretPosition = insertPosition;
    }

    private List<(bool, string)> ParseFormattedTextToTextSegments(string formattedText)
    {
        List<(bool, string)> list = new List<(bool, string)>();
        string text = string.Empty;
        // Pre-declaring varValue/subValue places them at slots [2]/[3] before the foreach enumerator at [4].
        string varValue;
        string subValue;
        foreach (Match item in ExtractSubscriptMatchesAndText(formattedText, out text))
        {
            varValue = item.Groups["varValue"].Value;
            subValue = item.Groups["subValue"].Value;
            list.Add((true, varValue));
            list.Add((false, subValue));
        }
        list.Add((true, text));
        return list;
    }

    // Regex fields declared after ParseFormattedTextToTextSegments to preserve closure ordinals 2_x, 15_x, 17_x.
    private Regex _constantRegex = new Regex("^\\(@LABEL CONSTANT\\s+(?<content>.*)\\)");

    private Regex _unitRegex = new Regex("^\\(@LABEL UNIT\\s+(?<content>.*)\\)");

    private Regex _variableRegex = new Regex("^\\(@LABEL VARIABLE\\s+(?<content>.*)\\)");

    private Regex _variableSuperscriptRegex = new Regex("^\\(\\^\\s+(?<varValue>[^\\s\\)]+)\\s+(?<superValue>.*?)\\)$");

    private Regex _variableIDRegex = new Regex("\\(@ID\\s+(?<content>.*?)\\)$");

    private Regex _variableSubscriptRegex = new Regex("\\s*(?<varValue>.*?)\\s*\\(@SUB (?<subValue>.*?)\\)");

    private MatchCollection ExtractSubscriptMatchesAndText(string formattedText, out string text)
    {
        formattedText = _constantRegex.Replace(formattedText, (Match m) => m.Groups["content"].Value);
        formattedText = _unitRegex.Replace(formattedText, (Match m) => m.Groups["content"].Value);
        formattedText = _variableRegex.Replace(formattedText, (Match m) => m.Groups["content"].Value);
        formattedText = _variableSuperscriptRegex.Replace(formattedText, (Match m) => m.Groups["varValue"].Value + m.Groups["superValue"].Value);
        formattedText = _variableIDRegex.Replace(formattedText, (Match m) => m.Groups["content"].Value);
        MatchCollection matchCollection = _variableSubscriptRegex.Matches(formattedText);
        formattedText = _variableSubscriptRegex.Replace(formattedText, string.Empty);
        if (matchCollection.Count > 0 && formattedText.StartsWith(" "))
        {
            formattedText = formattedText.Substring(1);
        }
        text = formattedText;
        return matchCollection;
    }

    private void PasteFormattedContent(string formattedText, string dataFormat)
    {
        if (dataFormat == DataFormats.Xaml || dataFormat == DataFormats.Rtf)
        {
            SetFormattedContent(formattedText, dataFormat, CurrentProofingLanguage, null);
            foreach (SuperscriptRun run in TextRange.EnumerateRunsInRange().OfType<SuperscriptRun>())
            {
                CustomScriptRunBase.ToggleRangeText(new System.Windows.Documents.TextRange(run.ContentStart, run.ContentEnd), ScriptState.None, double.NaN);
            }
        }
    }

    public override void SetFormattedContent(string content, string dataFormat, XmlLanguage proofingLanguage, TextFormat textFormat = null)
    {
        TextPointer position = Selection.IsEmpty ? CaretPosition : Selection.Start;
        Selection.Text = string.Empty;
        System.Windows.Documents.TextRange textRange = new System.Windows.Documents.TextRange(position, CaretPosition);
        _textFormatInitialization = textFormat;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.Write(content ?? string.Empty);
                streamWriter.Flush();
                memoryStream.Seek(0L, SeekOrigin.Begin);
                textRange.Load(memoryStream, dataFormat);
            }
        }
        textRange.FixAndCleanImportedHyperlinksData(FixHyperlinkEmptyRuns);
        if (DebugTools.AssertNotNull(proofingLanguage, null).Passed)
        {
            Document.Language = proofingLanguage;
        }
        UpdateCurrentProofingLanguage(Selection);
        UpdateCurrentRuns(Selection, true);
        RemoveHyperlinksIfNeeded(textRange);
        Document.Blocks.Each((Block block) =>
        {
            block.ClearTextFormatPropsRecursively((TextElement x) => true, TextFormat.ClearFormatPropsList);
        });
        CustomScriptRunBase.RecoverSubSuperscriptRunsInRange(textRange);
        CaretPosition = textRange.End;
    }

    private void RemoveHyperlinksIfNeeded(System.Windows.Documents.TextRange range)
    {
        foreach (Hyperlink item in range.EnumerateHyperlinksInRange().ToList())
        {
            System.Windows.Documents.Paragraph paragraph = item.Parent as System.Windows.Documents.Paragraph;
            foreach (Inline item2 in item.Inlines.ToList())
            {
                item.Inlines.Remove(item2);
                paragraph?.Inlines.InsertBefore(item, item2);
            }
            paragraph?.Inlines.Remove(item);
        }
    }

    protected override void PrePasting()
    {
        ClearInsertionPointsCache();
        CurrentInsertionScriptState = null;
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        string text = Text?.Trim(new char[2] { '\r', '\n' }) ?? string.Empty;
        if (!string.IsNullOrEmpty(e.Text) && text.Length + e.Text.Length > 100)
        {
            e.Handled = true;
        }
        else
        {
            base.OnPreviewTextInput(e);
        }
    }

    protected override void HandleTextChanged(object sender, TextChangedEventArgs args)
    {
        _cachedBaseline = null;
        ClearInsertionPointsCache();
        RaiseEvent(new RoutedEventArgs(RichTextBase.ContentChangedEvent));
        if (IsLoaded)
        {
            UpdateCurrentRuns(Selection, args.UndoAction == UndoAction.Create && args.Changes?.Count > 0, args.Changes?.FirstOrDefault()?.RemovedLength > 0);
            CurrentTextFormat = SelectionTextFormat;
            if (CurrentInsertionScriptState.HasValue)
            {
                _changeLevel++;
                ToggleRunElementAccordingToScriptState(args);
                _changeLevel--;
            }
        }
    }

    protected override void ExecuteToggleScriptUndoCommand(System.Windows.Documents.TextRange toggleRange, ScriptState toggleToState)
    {
        ToggleScriptUndoCommand.CreateToggleScriptUndoCommand(this, toggleRange, toggleToState)?.Do();
    }

    protected override void HandleReplace(object sender, ExecutedRoutedEventArgs e)
    {
        DebugTools.DevAssert(!Selection.IsEmpty, null);
        ((FindReplaceQuery)e.Parameter).Status = FindReplaceStatus.ExecuteReplaceFailed;
    }

    public void GetSubscriptPositions(List<Pair<int, int>> subscriptPositions)
    {
        subscriptPositions.Clear();
        BuildSubscriptPositions(TextRange, subscriptPositions);
    }

    private void RemoveAdditionalParagraphs()
    {
        if (Document == null)
        {
            return;
        }
        if (Document.Blocks.Count > 1)
        {
            if (!(Document.Blocks.FirstBlock is System.Windows.Documents.Paragraph))
            {
                return;
            }
            foreach (System.Windows.Documents.Paragraph paragraph in Document.Blocks.Skip(1).OfType<System.Windows.Documents.Paragraph>().ToList())
            {
                Document.Blocks.Remove(paragraph);
            }
        }
    }
}
