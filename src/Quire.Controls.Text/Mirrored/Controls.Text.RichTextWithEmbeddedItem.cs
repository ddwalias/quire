#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DocumentTextRange = System.Windows.Documents.TextRange;
using System.Windows.Input;
using System.Windows.Navigation;
using Quire.Controls.Core;
using Quire.Wpf;
using Quire.Undo;
using Quire.Calculation.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.Whiteboard;
using Quire.IO;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Services;
using Quire.FunctionalitiesLimitation;

namespace Quire.Controls.Text;

public partial class RichTextWithEmbeddedItem
{
    // Compile-order layout follows Mathcad method order; ordinal anchors are in the lane checkpoint.

    private void BindHyperlinkCommands()
    {
        CommandBindings.Add(new CommandBinding(HyperlinkCommands.ModifyHyperlink, ModifyHyperlink, CanModifyHyperlink));
        CommandBindings.Add(new CommandBinding(HyperlinkCommands.RemoveHyperlinks, RemoveHyperlinks, CanRemoveHyperlinks));
        CommandBindings.Add(new CommandBinding(HyperlinkCommands.OpenHyperlink, OpenHyperlink, CanOpenOrEditOrCopySingleHyperlink));
        CommandBindings.Add(new CommandBinding(HyperlinkCommands.CopyHyperlink, CopyHyperlink, CanOpenOrEditOrCopySingleHyperlink));
    }

    private void CanOpenOrEditOrCopySingleHyperlink(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanOpenOrEditOrCopySingleHyperlink();
    }

    private bool CanOpenOrEditOrCopySingleHyperlink()
    {
        return GetContextLinks()?.Count == 1;
    }

    private void CopyHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        Hyperlink hyperlink = CheckAndGetExactlyOneHyperlink();
        if (hyperlink != null && DebugTools.AssertNotNull(hyperlink.NavigateUri, null).Passed)
        {
            try
            {
                _copyLinkText = hyperlink.NavigateUri.ToString();
                TextPointer caretPosition = null;
                DocumentTextRange selectionRange = null;
                if (Selection.IsEmpty)
                {
                    caretPosition = CaretPosition;
                }
                else
                {
                    selectionRange = new DocumentTextRange(Selection.Start, Selection.End);
                }
                Selection.Select(hyperlink.ElementStart, hyperlink.ElementEnd);
                Copy();
                if (selectionRange != null)
                {
                    Selection.Select(selectionRange.Start, selectionRange.End);
                }
                else
                {
                    CaretPosition = caretPosition;
                }
            }
            finally
            {
                _copyLinkText = null;
            }
        }
    }

    private void OpenHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        Hyperlink link = CheckAndGetExactlyOneHyperlink();
        if (link != null)
        {
            link.RaiseEvent(new RequestNavigateEventArgs(link.NavigateUri, string.Empty));
        }
    }

    private Hyperlink CheckAndGetExactlyOneHyperlink()
    {
        List<Hyperlink> contextLinks = GetContextLinks();
        return DebugTools.DevAssert(contextLinks != null && contextLinks.Count == 1).Failed ? null : contextLinks.FirstOrDefault();
    }

    private void CanModifyHyperlink(object sender, CanExecuteRoutedEventArgs e)
    {
        if (!CanModifyHyperlink())
        {
            return;
        }
        e.CanExecute = true;
        ModifyHyperlinkCommandParameter parameter = e.Parameter as ModifyHyperlinkCommandParameter;
        if (parameter != null)
        {
            parameter.ResultIsSingleLinkEditOperation = CanOpenOrEditOrCopySingleHyperlink();
        }
    }

    private void ModifyHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.DevAssert(CanModifyHyperlink()).Failed)
        {
            return;
        }
        ModifyHyperlink(e.Parameter as ModifyHyperlinkCommandParameter ?? new ModifyHyperlinkCommandParameter(true, null, null));
    }

    internal void ModifyHyperlink(ModifyHyperlinkCommandParameter param)
    {
        HyperlinkContextValues context = new HyperlinkContextValues(this);
        bool restoreSelectionBrush = false;
        if (context.ConvertToLinkRange != null)
        {
            Selection.Select(context.ConvertToLinkRange.Start, context.ConvertToLinkRange.End);
            if (!WpfUtils.HasLocalValue(this, System.Windows.Controls.Primitives.TextBoxBase.SelectionBrushProperty))
            {
                // Mathcad copies the inherited brush into the local value before the highlight is restored.
                SelectionBrush = SelectionBrush;
                restoreSelectionBrush = true;
            }
            IsInactiveSelectionHighlightEnabled = true;
        }

        HyperlinkDialogViewModel viewModel = new HyperlinkDialogViewModel(context);
        if (param.UserInputText != null)
        {
            viewModel.UserInputText = param.UserInputText;
        }
        if (param.UserInputAddress != null)
        {
            viewModel.UserInputAddress = param.UserInputAddress;
        }

        HyperlinkEditState state = default;
        if (!param.ShowDialog || HyperlinkDialog.ShowHyperlinkDialog(viewModel, Application.Current.MainWindow))
        {
            state = viewModel.GetState();
            DebugTools.DevAssert(state.PerformedLinkEditOperation != LinkEditOperation.None, "Either Link operation context must contain link, or address field shouldn't be empty");
        }

        if (IsInactiveSelectionHighlightEnabled)
        {
            IsInactiveSelectionHighlightEnabled = false;
        }
        if (restoreSelectionBrush)
        {
            ClearValue(System.Windows.Controls.Primitives.TextBoxBase.SelectionBrushProperty);
        }

        LinkEditOperation operation = state.PerformedLinkEditOperation;
        switch (operation)
        {
            case LinkEditOperation.Delete:
                RemoveHyperlinkUndoableCommand(context.ContextLink);
                break;
            case LinkEditOperation.Insert:
            case LinkEditOperation.Edit:
                param.ResultIsSingleLinkEditOperation = state.PerformedLinkEditOperation == LinkEditOperation.Edit;
                param.ResultHyperlink = EditLinkUndoableCommand(state, context.ConvertToLinkRange, false);
                break;
            default:
                DebugTools.FailAssertion("Unknown link operation.");
                break;
            case LinkEditOperation.None:
                break;
        }

        if (state.PerformedLinkEditOperation != LinkEditOperation.None && context.ContextLink != null)
        {
            InlineCollection parentInlines = context.ContextLink.GetParentInlines();
            if (DebugTools.DevAssert(parentInlines == null, null).Failed)
            {
                parentInlines.Remove(context.ContextLink);
            }
        }
    }

    private Hyperlink EditLinkUndoableCommand(HyperlinkEditState hyperlinkState, DocumentTextRange convertToLinkRange, bool skipOneInsertionPosition = false)
    {
        Hyperlink hyperlink = null;
        using (new CommandScope("Edit\\Insert hyperlink item"))
        {
            Quire.Core.TextFormat textFormat = null;
            bool flag = false;
            TextPointer insertionPosition = CaretPosition;
            using (DeclareChangeBlock())
            {
                Dictionary<int, System.Windows.Media.Brush> dictionary = new Dictionary<int, System.Windows.Media.Brush>();
                foreach (Inline item in Selection.EnumerateInlinesInRange<Inline>())
                {
                    dictionary[((object)item).GetHashCode()] = item.Foreground;
                }
                if (convertToLinkRange.IsNotEmpty())
                {
                    if (hyperlinkState.UserChangedText)
                    {
                        textFormat = new Quire.Core.TextFormat(convertToLinkRange, onlyFirstRun: true);
                        convertToLinkRange.Text = string.Empty;
                        insertionPosition = convertToLinkRange.Start;
                    }
                    else
                    {
                        try
                        {
                            TextPointer textPointer = convertToLinkRange.Start;
                            TextPointer textPointer2 = convertToLinkRange.End;
                            List<Hyperlink> hyperlinksInRange = GetHyperlinksInRange(convertToLinkRange);
                            if (hyperlinksInRange.IsNotEmpty())
                            {
                                DebugTools.DevAssert(hyperlinkState.PerformedLinkEditOperation == LinkEditOperation.Edit && hyperlinksInRange.Count == 1, null);
                                int offsetToPosition = Document.ContentStart.GetOffsetToPosition(textPointer);
                                int offsetToPosition2 = Document.ContentEnd.GetOffsetToPosition(textPointer2);
                                RemoveHyperlinkUndoableCommand(hyperlinksInRange);
                                textPointer = Document.ContentStart.GetPositionAtOffset(offsetToPosition) ?? Document.ContentStart;
                                textPointer2 = Document.ContentEnd.GetPositionAtOffset(offsetToPosition2) ?? Document.ContentEnd;
                            }
                            if (DebugTools.DevAssert(textPointer.CompareTo(textPointer2) < 0 && new DocumentTextRange(textPointer, textPointer2).Text.Length > 0, null).Failed)
                            {
                                flag = true;
                            }
                            else if (textPointer.Parent == textPointer2.Parent && textPointer2.Parent is BlockUIContainer blockUIContainer)
                            {
                                BlockCollection parentBlocks = blockUIContainer.GetParentBlocks();
                                if (DebugTools.AssertNotNull(parentBlocks, null).Passed)
                                {
                                    UIElement child = blockUIContainer.Child;
                                    blockUIContainer.Child = null;
                                    InlineUIContainer childInline = new InlineUIContainer(child);
                                    hyperlink = Quire.Wpf.HyperlinkExtensions.CreateLink(hyperlinkState.UserInputUri, childInline);
                                    System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph();
                                    paragraph.Inlines.Add(hyperlink);
                                    parentBlocks.InsertBefore(blockUIContainer, paragraph);
                                    parentBlocks.Remove(blockUIContainer);
                                }
                            }
                            else
                            {
                                hyperlink = Quire.Wpf.HyperlinkExtensions.CreateLink(hyperlinkState.UserInputUri, textPointer, textPointer2);
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugTools.FailAssertion(ex);
                            flag = true;
                        }
                    }
                }
                else
                {
                    flag = DebugTools.DevAssert(hyperlinkState.PerformedLinkEditOperation == LinkEditOperation.Insert, null).Failed;
                }
                if (!flag)
                {
                    if (hyperlink == null)
                    {
                        hyperlink = Quire.Wpf.HyperlinkExtensions.CreateLink(hyperlinkState.UserInputUri, new Run(hyperlinkState.UserInputText), insertionPosition);
                        textFormat?.ApplyTextFormat(new DocumentTextRange(hyperlink.ContentStart, hyperlink.ContentEnd));
                    }
                    _hyperlinkDictionary.Add(((object)hyperlink).GetHashCode(), dictionary);
                    hyperlink.ClearForegroundInDepth();
                    TextPointer insertionPosition2 = hyperlink.ElementEnd.GetInsertionPosition(LogicalDirection.Forward);
                    TextPointer textPointer3 = skipOneInsertionPosition ? insertionPosition2.GetNextInsertionPosition(LogicalDirection.Forward) : null;
                    SetCaretPositionAndClearSuggestedX(textPointer3 ?? insertionPosition2);
                }
            }
            if (flag)
            {
                while (CommandBuffer.CanUndo)
                {
                    CommandBuffer.Undo();
                }
            }
        }
        return hyperlink;
    }

    private void CanRemoveHyperlinks(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = IsActive && ActiveItem == null && EnumerableExtensions.IsNotEmpty(GetContextLinks());
    }

    private List<Hyperlink> GetContextLinks()
    {
        if (!Selection.IsEmpty)
        {
            return GetHyperlinksInRange(Selection);
        }
        return GetHyperlinkAtCaretPositionIncludingEdges()?.Yield().ToList();
    }

    private void RemoveHyperlinks(object sender, ExecutedRoutedEventArgs e)
    {
        RemoveHyperlinkUndoableCommand(GetContextLinks());
    }

    private void RemoveHyperlinkUndoableCommand(Hyperlink link)
    {
        RemoveHyperlinkUndoableCommand(link != null ? EnumerableExtensions.Yield(link).ToList() : null);
    }

    private void RemoveHyperlinkUndoableCommand(List<Hyperlink> links)
    {
        if (DebugTools.DevAssert(links.IsNotEmpty()).Failed)
        {
            return;
        }
        bool undoAll = false;
        using (CommandScope commandScope = new CommandScope("Remove hyperlink from text"))
        {
            using (DeclareChangeBlock())
            {
                foreach (Hyperlink link in links)
                {
                    InlineCollection parentInlines = link.GetParentInlines();
                    if (DebugTools.DevAssert(parentInlines != null && parentInlines.Contains(link)).Failed)
                    {
                        undoAll = true;
                        break;
                    }
                    link.SetTextFormatForLinkRemoval(out List<Inline> inlines);
                    link.Inlines.RemoveAll();
                    foreach (Inline inline in inlines)
                    {
                        parentInlines.InsertBefore(link, inline);
                    }
                    ReproduceRemovedHyperlinkColors(inlines, link);
                    parentInlines.Remove(link);
                }
            }
            if (undoAll)
            {
                while (CommandBuffer.CanUndo)
                {
                    CommandBuffer.Undo();
                }
            }
        }
    }

    private void ReproduceRemovedHyperlinkColors(global::System.Collections.Generic.List<global::System.Windows.Documents.Inline> inlines, global::System.Windows.Documents.Hyperlink link) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal DocumentTextRange GetWordRange()
    {
        return GetWordRangeAtCaretPosition(out string _, TextFromRunsStops.FindWordStrict, false);
    }

    internal const string InternalLinkPrefix = "#";

    public static partial class PropertyNames
    {
        public const string ActiveItem = "ActiveItem";
    }

    private partial struct PreviewKeyDownData
    {
        public Key PressedKey { get; }

        public int ChangeInsertionPoint { get; }

        internal PreviewKeyDownData(Key key, int point)
        {
            PressedKey = key;
            ChangeInsertionPoint = point;
        }
    }

    private bool CanModifyHyperlink()
    {
        if (!IsActive || ActiveItem != null)
        {
            return false;
        }

        bool canModify;
        if (Selection.IsEmpty)
        {
            canModify = true;
        }
        else
        {
            List<object> embeddedObjects = EnumerateEmbeddedObjectsInRange(Selection).ToList();
            bool canModifyEmbeddedObjects = embeddedObjects.IsNullOrEmpty() || embeddedObjects.All(item => item is Image);
            bool hasFewerThanTwoHyperlinks = (GetHyperlinksInRange(Selection)?.Count ?? 0) < 2;
            // Mathcad combines these checks with bitwise AND before the final inline-range check.
            canModify = (canModifyEmbeddedObjects & hasFewerThanTwoHyperlinks) && RangeContainsOnlyInlineElements(Selection);
        }

        return canModify;
    }

    private bool RangeContainsOnlyInlineElements(DocumentTextRange range, bool prohibitSelectionInBlockUiContainer = false)
    {
        bool result = true;
        TextPointer start = range.Start.GetInsertionPosition(LogicalDirection.Forward);
        TextPointer end = range.End.GetInsertionPosition(LogicalDirection.Backward);
        while ((start != null && start.CompareTo(end) < 0) & result)
        {
            TextPointerContext context = start.GetPointerContext(LogicalDirection.Forward);
            if (context == TextPointerContext.ElementStart)
            {
                result = start.GetAdjacentElement(LogicalDirection.Forward) is Inline;
            }
            else if (prohibitSelectionInBlockUiContainer && context == TextPointerContext.EmbeddedElement)
            {
                result = start.Parent is Inline;
            }
            start = start.GetNextContextPosition(LogicalDirection.Forward);
        }
        return result;
    }

    internal bool ContainsSimpleText(DocumentTextRange textRange)
    {
        if (EnumerateEmbeddedObjectsInRange(textRange).IsNullOrEmpty())
        {
            return textRange.EnumerateInlinesInRange<LineBreak>().IsNullOrEmpty();
        }
        return false;
    }

private static void RequestNavigateHandler(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
{
    RichTextWithEmbeddedItem richText = sender as RichTextWithEmbeddedItem;
    Hyperlink hyperlink = e.Source as Hyperlink;
    bool openedFile = false;
    string fileName = string.Empty;
    DebugTools.DevAssert(hyperlink != null && hyperlink.NavigateUri == e.Uri);
    if (DebugTools.AssertNotNull(richText).Passed)
    {
        bool handled = false;
        if (hyperlink.NavigateUri.ToString().StartsWith("#"))
        {
            if (FunctionalityService.Instance.IsFunctionalityEnabled((EnumFunctionalityKey)14))
            {
                handled = true;
                e.Handled = false;
            }
        }
        else
        {
            string target = e.Uri.OriginalString;
            if (!e.Uri.IsAbsoluteUri)
            {
                target = string.Concat((hyperlink.DataContext as RichTextWithEmbeddedItem)?.BaseRelativeDirectory, "\\", target);
            }

            string filePathWithoutTag;
            string tagName;
            if (UrlUtils.HasTag(target, out filePathWithoutTag, out tagName) && FunctionalityService.Instance.IsFunctionalityEnabled((EnumFunctionalityKey)14))
            {
                openedFile = true;
                OpenFileCommand.Execute(target, null);
                e.Handled = true;
            }

            fileName = filePathWithoutTag;
        }

        if (!openedFile && !handled)
        {
            if (fileName.IsNotEmpty())
            {
                hyperlink.NavigateUri = new Uri(fileName);
            }

            richText.NavigateToHyperLink(hyperlink);
            e.Handled = true;
        }

        Hyperlink originalHyperlink = e.OriginalSource as Hyperlink;
        if (originalHyperlink != null)
        {
            HyperlinkProperties.SetLinkVisited(originalHyperlink, true);
        }
    }
}

    private void NavigateToHyperLink(Hyperlink link)
    {
        if (DebugTools.AssertNotNull(link).Failed)
        {
            return;
        }
        TextPointer position = null;
        try
        {
            position = GetPositionFromPoint(Mouse.GetPosition(this), true);
        }
        catch (Exception)
        {
        }
        if (DebugTools.DevAssert(position != null && link.ContentStart.CompareTo(position) <= 0 && link.ContentEnd.CompareTo(position) >= 0).Failed)
        {
            position = link.ElementStart;
        }
        if (!IsActive)
        {
            Activate(true);
        }
        else
        {
            Focus();
        }
        CaretPosition = position;
        DisplayHyperlink(link);
    }

    private void DisplayHyperlink(Hyperlink link, bool showAboutBox = true)
    {
        HyperlinkProperties.SetLinkVisited(link, true);
        if (FunctionalityService.Instance.IsFunctionalityEnabled((EnumFunctionalityKey)14))
        {
            OpenHyperlinkUrl(link.NavigateUri);
            return;
        }
        if (DebugTools.DevAssert(showAboutBox).Passed)
        {
            DisplayFreemiumAboutBoxCommandParameter parameter = new DisplayFreemiumAboutBoxCommandParameter(new HyperlinkToScreenProvider(this, CaretPosition), (EnumFunctionalityKey)14);
            FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox.Execute(parameter, null);
        }
    }

    private static IMessageBoxManager MessageBoxManager
    {
        get
        {
            if (_messageBoxMngr == null || !_messageBoxMngr.IsConnected)
            {
                return _messageBoxMngr = AppServiceRegistry.GetService<IMessageBoxManager>();
            }

            return _messageBoxMngr;
        }
    }

    private bool ShowQuestionWarningDialog(NavigateUrlErrorWarningKeys messageKey, string path)
    {
        return MessageBoxManager.ShowQuestion(Application.Current.MainWindow, string.Format((TryFindResource(messageKey) as string) ?? "{0}", path), WpfUtils.ApplicationFullName, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
    }

    private void ShowWarningDialog(NavigateUrlErrorWarningKeys messageKey, string param)
    {
        MessageBoxManager.ShowWarning(Application.Current.MainWindow, string.Format((TryFindResource(messageKey) as string) ?? "{0}", param), WpfUtils.ApplicationFullName);
    }

    private void ShowWarningDialog(NavigateUrlErrorWarningKeys messageKey)
    {
        MessageBoxManager.ShowWarning(Application.Current.MainWindow, (TryFindResource(messageKey) as string) ?? string.Empty, WpfUtils.ApplicationFullName);
    }

    private void OpenHyperlinkUrl(global::System.Uri path) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static NavigateUrlErrorWarningKeys? ValidateURL(System.Uri absolutePath)
    {
        NavigateUrlErrorWarningKeys? result = null;
        if (absolutePath == null)
        {
            result = NavigateUrlErrorWarningKeys.UnsupportedUrlFormat;
        }
        else if (!_supportedURLSchemes.Contains(absolutePath.Scheme))
        {
            result = NavigateUrlErrorWarningKeys.UnsupportedUrlProtocol;
        }
        else if (absolutePath.IsFile)
        {
            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(absolutePath.LocalPath);
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(absolutePath.LocalPath);
                if (!fileInfo.Exists && !directoryInfo.Exists)
                {
                    // Assigns instead of returning from inside try: Release IL keeps one result local, with no separate return temp.
                    result = NavigateUrlErrorWarningKeys.FileNotExist;
                }
            }
            catch (System.Security.SecurityException)
            {
                result = NavigateUrlErrorWarningKeys.AccessSecurityIssue;
            }
            catch (System.Exception)
            {
                result = NavigateUrlErrorWarningKeys.UnsupportedUrlFormat;
            }
        }
        return result;
    }

    internal Uri ResolveUrl(Uri path)
    {
        if (path == null)
        {
            return null;
        }
        if (path.IsAbsoluteUri)
        {
            return path;
        }
        if (!UrlUtils.TryCreateAbsoluteUri(BaseRelativeDirectory, path, out Uri absoluteUri))
        {
            return null;
        }
        return absoluteUri;
    }

    private string BaseRelativeDirectory
    {
        get
        {
            IWorksheetProperties worksheetProperties = WorksheetPropertiesManager.GetWorksheetProperties(this);
            if (!string.IsNullOrEmpty(worksheetProperties.FilePath))
            {
                return System.IO.Path.GetDirectoryName(worksheetProperties.FilePath);
            }

            return worksheetProperties.DocCalculationVariables.CurrentWorkingDirectory;
        }
    }

    public bool HasHyperlinkInMouseContext(object context)
    {
        TextElement element = context as TextElement;
        Hyperlink link = element != null
            ? element.GetTextElementParentOrSelf<Hyperlink>()
            : GetHyperlinkFromTextPointer(GetPositionFromPoint(Mouse.GetPosition(this), false));
        return link != null;
    }

    // Compile-order device: placed before the iterator so the iterator and AddItems/RemoveItems/MoveItems lambda ordinals match Release.
    private Hyperlink GetHyperlinkFromTextPointer(TextPointer ptr)
    {
        if (ptr == null)
        {
            return null;
        }
        return TextPointerExtensions.GetTextElementFromTextPointer<Hyperlink>(ptr);
    }

    private bool SelectionFullyInsideHyperlink
    {
        get
        {
            Hyperlink hyperlink = GetHyperlinkFromTextPointer(Selection.Start);
            if (hyperlink != null)
            {
                if (hyperlink.ElementStart.CompareTo(Selection.Start) == -1)
                {
                    return hyperlink.ElementEnd.CompareTo(Selection.End) == 1;
                }

                return false;
            }

            return false;
        }
    }

    internal Hyperlink GetHyperlinkAtCaretPositionIncludingEdges()
    {
        return GetHyperlinkAtCaretPosition()
            ?? TextPointerExtensions.GetAdjacentHyperlink(CaretPosition, CaretPosition.LogicalDirection)
            ?? TextPointerExtensions.GetAdjacentHyperlink(CaretPosition, CaretPosition.LogicalDirection == LogicalDirection.Forward ? LogicalDirection.Backward : LogicalDirection.Forward);
    }

    internal Hyperlink GetHyperlinkAtCaretPosition()
    {
        if (Selection.IsEmpty)
        {
            return GetHyperlinkFromTextPointer(CaretPosition);
        }
        return null;
    }

    internal List<Hyperlink> GetHyperlinksFromWholeDocument()
    {
        return GetHyperlinksInRange(new DocumentTextRange(Document.ContentStart, Document.ContentEnd), false);
    }

    public List<Hyperlink> GetHyperlinksInRange(DocumentTextRange range, bool allowEmpty = false)
    {
        return TextRangeExtension.EnumerateHyperlinksInRange(range, allowEmpty).ToList();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (HandleLinkOpsOnPreviewKeyDown(e.Key))
        {
            e.Handled = true;
            return;
        }
        base.OnPreviewKeyDown(e);
    }

    private bool HandleLinkOpsOnPreviewKeyDown(Key key)
    {
        _keyDownData = null;
        if (ActiveItem != null || Selection.IsNotEmpty())
        {
            return false;
        }
        bool handled = false;
        if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0 && (Keyboard.Modifiers & ModifierKeys.Control) == 0)
        {
            // Return is listed first: Release IL lays out the switch sections in source order.
            switch (key)
            {
                case Key.Return:
                {
                    Hyperlink hyperlinkAtCaretPosition = GetHyperlinkAtCaretPosition();
                    if (hyperlinkAtCaretPosition != null)
                    {
                        DisplayHyperlink(hyperlinkAtCaretPosition);
                        handled = true;
                    }
                    break;
                }
                case Key.Back:
                {
                    if (FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.RemoveHyperlinkCommand))
                    {
                        Hyperlink adjacentHyperlink = CaretPosition.GetAdjacentHyperlink(LogicalDirection.Backward);
                        if (adjacentHyperlink != null)
                        {
                            RemoveHyperlinkUndoableCommand(adjacentHyperlink);
                            handled = true;
                        }
                    }
                    break;
                }
                case Key.Delete:
                {
                    if (FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.RemoveHyperlinkCommand))
                    {
                        Hyperlink adjacentHyperlink = CaretPosition.GetAdjacentHyperlink(LogicalDirection.Forward);
                        if (adjacentHyperlink != null)
                        {
                            Selection.Select(adjacentHyperlink.ElementStart, adjacentHyperlink.ElementEnd);
                            handled = true;
                        }
                    }
                    break;
                }
            }
        }
        if (!handled)
        {
            CollectKeyDataForPostChangeHyperlinkInsertion(key, Keyboard.Modifiers);
        }
        return handled;
    }

    internal void CollectKeyDataForPostChangeHyperlinkInsertion(Key key, ModifierKeys keyModifiers)
    {
        if (key == Key.Tab || key == Key.Return || key == Key.Space)
        {
            int caretOffset = CaretOffset;
            _keyDownData = DebugTools.DevAssert(caretOffset >= 0).Passed ? new PreviewKeyDownData(key, caretOffset) : (PreviewKeyDownData?)null;
        }
    }

    // Compile-order device: these fields precede the ordinal 52 closure.
    private string _copyLinkText;

    private ITextStyleChangedHandler _textStyleChangedHandler;

    private readonly global::System.Collections.Generic.IDictionary<int, global::System.Collections.Generic.IDictionary<int, global::System.Windows.Media.Brush>> _hyperlinkDictionary;

    public static readonly global::System.Windows.Input.RoutedUICommand OpenFileCommand;

    private static global::Quire.Wpf.IMessageBoxManager _messageBoxMngr;

    private void TryPostChangeHyperlinkInsertion(global::System.Collections.Generic.ICollection<global::System.Windows.Controls.TextChange> changes)
    {
        // Nested guards preserve the shared Release IL exit used by the captured action.
        if (!_keyDownData.HasValue)
        {
            return;
        }

        PreviewKeyDownData? keyDownData = _keyDownData;
        _keyDownData = null;
        if (!FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.ModifyHyperlinkCommand) || DebugTools.DevAssert(Selection.IsEmpty, null).Failed)
        {
            return;
        }

        TextPointer linkSearchPos = GetPotentialLinkEndPosition(changes, keyDownData.Value.PressedKey, keyDownData.Value.ChangeInsertionPoint);
        if (linkSearchPos != null && GetHyperlinkFromTextPointer(linkSearchPos) == null)
        {
            TextPointer pointerWordStart = RichTextBase.GetTextBoundary(LogicalDirection.Backward, out string partialText, TextFromRunsStops.FindWord, new DocumentTextRange(Document.ContentStart, linkSearchPos));
            if (partialText.IsNotEmpty() && UrlUtils.TryCreateKnownUri(partialText, out Uri newUri))
            {
                Action action = () =>
                {
                    EditLinkUndoableCommand(new HyperlinkEditState(LinkEditOperation.Insert, partialText, userChangedText: false, newUri), new DocumentTextRange(pointerWordStart, linkSearchPos), skipOneInsertionPosition: true);
                };
                Dispatcher.BeginInvoke((Delegate)action, Array.Empty<object>());
            }
        }
    }

    private global::System.Windows.Documents.TextPointer GetPotentialLinkEndPosition(global::System.Collections.Generic.ICollection<global::System.Windows.Controls.TextChange> changes, global::System.Windows.Input.Key key, int insertionPoint) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void UNITTEST_ExecuteAddItemToListCommand(IWhiteboardItemInText item)
    {
        ExecuteAddItemToListCommand(item);
    }

    public void UNITTEST_ExecuteRemoveItemFromListCommand(IWhiteboardItemInText item)
    {
        ExecuteRemoveItemFromListCommand(item);
    }

    public bool HyperlinkCanHaveToolTip
    {
        get => (bool)GetValue(HyperlinkCanHaveToolTipProperty);
        set => SetValue(HyperlinkCanHaveToolTipProperty, value);
    }

    public IBookmarksManager BookmarksManager { get; set; }

    static RichTextWithEmbeddedItem()
    {
        OpenFileCommand = new RoutedUICommand();
        _internetURLSchemes = new string[3] { System.Uri.UriSchemeHttps, System.Uri.UriSchemeHttp, System.Uri.UriSchemeFtp };
        _supportedURLSchemes = _internetURLSchemes.Concat(new string[2] { System.Uri.UriSchemeFile, System.Uri.UriSchemeMailto }).ToArray();
        _typeOfThis = typeof(RichTextWithEmbeddedItem);
        EmbeddedMathRootPadding = new Thickness(2, 0, 2, 0);
        TabOutOfTextEvent = EventManager.RegisterRoutedEvent("TabOutOfText", RoutingStrategy.Bubble, typeof(CustomRoutedEventHandler<FocusNavigationDirection>), _typeOfThis);
        HyperlinkCanHaveToolTipProperty = DependencyProperty.Register("HyperlinkCanHaveToolTip", typeof(bool), _typeOfThis, new PropertyMetadata((object)true));
        EventManager.RegisterClassHandler(_typeOfThis, Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(RequestNavigateHandler));
    }

    protected override void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        base.OnIsInPrintingPropertyChanged(args);
        UpdateHyperlinkCanHaveToolTip();
    }

    protected override void OnIsInSavingCLRPropertyChanged(bool newValue)
    {
        base.OnIsInSavingCLRPropertyChanged(newValue);
        UpdateHyperlinkCanHaveToolTip();
    }

    protected override void OnIsInCopyPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        base.OnIsInCopyPropertyChanged(args);
        UpdateHyperlinkCanHaveToolTip();
    }

    private void UpdateHyperlinkCanHaveToolTip()
    {
        HyperlinkCanHaveToolTip = !(IsInPrinting || IsInCopy) && !IsInSaving;
    }

    private static readonly string[] _internetURLSchemes;

    private static readonly string[] _supportedURLSchemes;

    private global::System.Nullable<global::Quire.Controls.Text.RichTextWithEmbeddedItem.PreviewKeyDownData> _keyDownData;

    private static readonly global::System.Type _typeOfThis;

    public static readonly global::System.Windows.Thickness EmbeddedMathRootPadding;

    public static readonly global::System.Windows.RoutedEvent TabOutOfTextEvent;

    public static readonly global::System.Windows.DependencyProperty HyperlinkCanHaveToolTipProperty;

    private global::Quire.Controls.Core.WhiteboardItemsCollection _whiteboardItems;

    private global::Quire.Calculation.Core.Support.CalculatableChildHider _childManager;

    private global::Quire.Controls.Core.IWhiteboardItem _activeItem;

    private global::Quire.Controls.Text.RichTextSerializationStrategy _serializationStrategy;

    private global::Quire.Controls.Text.RichTextDeserializationStrategy _deserializationStrategy;

    private global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItemInText, int> _itemOffsetMap;

    private global::Quire.Controls.Core.NumberFormat _currentNumberFormat;

    private global::Quire.Controls.Core.MatrixFormat _currentMatrixFormat;

    private global::Quire.Controls.Core.MathFormat _currentMathFormat;

    private bool _hideFromCalculationButtonEnabled;

    private bool _hideFromCalculationButtonChecked;

    internal RichTextWithEmbeddedItem() { }

    private void OnPreviewMoveUpByPage(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem != null)
        {
            IWhiteboardItemInText item = ActiveItem as IWhiteboardItemInText;
            if (!item.CanNavigate(Direction.Up))
            {
                ActiveItem = null;
                Activate(true);
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    SetCaretPositionAndClearSuggestedX(container.ElementStart.GetInsertionPosition(LogicalDirection.Backward));
                }

                EditingCommands.MoveUpByPage.Execute(null, this);
            }
        }
        else
        {
            CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Up);
        }
    }

    public void OnPreviewMoveDownByPage(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem != null)
        {
            IWhiteboardItemInText item = ActiveItem as IWhiteboardItemInText;
            if (!item.CanNavigate(Direction.Down))
            {
                ActiveItem = null;
                Activate(true);
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    SetCaretPositionAndClearSuggestedX(container.ElementStart.GetInsertionPosition(LogicalDirection.Backward));
                }

                EditingCommands.MoveDownByPage.Execute(null, this);
            }
        }
        else
        {
            CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Down);
        }
    }

    private IWhiteboardItem GetDescendantItem(DependencyObject descendantObject)
    {
        IWhiteboardItem item = null;
        if (ActiveItem == null)
        {
            return item;
        }
        item = WpfUtils.TryFindAncestorIncludingSelf<IWhiteboardItem>(descendantObject, false);
        if (item == null)
        {
            return null;
        }
        if (item != this)
        {
            return null;
        }
        if (WpfUtils.TryFindVisualAncestor<IWhiteboardItemContainer>(item as DependencyObject, false) == null)
        {
            return null;
        }
        return item;
    }

    private void OnPreviewTabForward(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (ActiveItem == null)
        {
            return;
        }

        if (((IWhiteboardItemInText)ActiveItem).CanTraverse((FocusNavigationDirection)0))
        {
            return;
        }

        e.Handled = true;
        IWhiteboardItem whiteboardItem = WhiteboardItems.SkipWhile((IWhiteboardItem item) => item != ActiveItem).Skip(1).FirstOrDefault(item => ContainerItemManager.IsTabNavigatable(item));
        if (whiteboardItem != null)
        {
            ContainerItemManager.TabInto(whiteboardItem, (FocusNavigationDirection)0);
        }
        else
        {
            RaiseEvent(new CustomRoutedEventArgs<FocusNavigationDirection>(TabOutOfTextEvent, (FocusNavigationDirection)0));
        }
    }

    private void OnPreviewTabBackward(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem == null)
        {
            return;
        }

        if (((IWhiteboardItemInText)ActiveItem).CanTraverse(FocusNavigationDirection.Previous))
        {
            return;
        }

        e.Handled = true;
        IWhiteboardItem candidate = null;
        foreach (IWhiteboardItem item in WhiteboardItems)
        {
            if (item == ActiveItem)
            {
                break;
            }

            if (ContainerItemManager.IsTabNavigatable(item))
            {
                candidate = item;
            }
        }

        if (candidate != null)
        {
            ContainerItemManager.TabInto(candidate, FocusNavigationDirection.Previous);
            return;
        }

        RaiseEvent(new CustomRoutedEventArgs<FocusNavigationDirection>(TabOutOfTextEvent, FocusNavigationDirection.Previous));
    }

    private void CanExecuteMathFormatCommands(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CurrentMathFormat != null;
    }

    private void ExecuteClearMathFormat(object sender, ExecutedRoutedEventArgs e)
    {
        ExecuteCommandKeepingSelection(CurrentMathFormat.ClearFormat, "ClearMathFormat");
    }

    private void ExecuteIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
    {
        CurrentMathFormat.IncreaseFontSize();
    }

    private void ExecuteDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
    {
        CurrentMathFormat.DecreaseFontSize();
    }

    private void ExecuteChangeFontColor(object sender, ExecutedRoutedEventArgs e)
    {
        ExecuteCommandKeepingSelection(() => CurrentMathFormat.ChangeFontColor((System.Windows.Media.Color)e.Parameter), "ChangeFontColor");
    }

    private void ExecuteChangeHighlightColor(object sender, ExecutedRoutedEventArgs e)
    {
        ExecuteCommandKeepingSelection(() => CurrentMathFormat.ChangeHighlightColor((System.Windows.Media.Color?)e.Parameter), "ChangeHighlightColor");
    }

    private void ExecuteCommandKeepingSelection(Func<bool> internalCommandDo, string name)
    {
        CommandBuffer.Execute(new EmbededItemsCommandWithKeepSelection(this, name, internalCommandDo), null);
    }

    protected override void CopyingHandler(object sender, global::System.Windows.DataObjectCopyingEventArgs e) => base.CopyingHandler(sender, e);

    public void CopyWholeRangeXamlToDataObject(IDataObject dataObject)
    {
        string xaml = TextRange.SerializeRangeContentToXamlString();
        List<IWhiteboardItem> items = GetWhiteboardItemsInRange(TextRange).ToList();
        string xamlWithItems = InlayXamlWithEmbeddedItems(TextRange, xaml, out RichTextEmbeddedItemsInfo info, items);
        dataObject.SetData(DataFormats.Xaml, xamlWithItems ?? xaml);
        DataObjectExtensions.SetTextWithEmbeddedItemsInfo(dataObject, info);
    }

    private string InlayXamlWithEmbeddedItems(DocumentTextRange rangeToSerialize, string xaml, out RichTextEmbeddedItemsInfo richTextEmbeddedItemsInfoData, List<IWhiteboardItem> embeddedEquationItems)
    {
        richTextEmbeddedItemsInfoData = RichTextEmbeddedItemsInfo.None;
        if (DebugTools.DevAssert(rangeToSerialize.IsNotEmpty() && xaml != null, null).Failed)
        {
            return null;
        }
        IList<string> list = new List<string>();
        TextPointer textPointer = rangeToSerialize.Start;
        while (textPointer.CompareTo(rangeToSerialize.End) < 0)
        {
            // An if/else-if on a local, not a switch: Release IL tests Text first and falls through into its body.
            TextPointerContext pointerContext = textPointer.GetPointerContext(LogicalDirection.Forward);
            if (pointerContext == TextPointerContext.Text)
            {
                if ((textPointer.CompareTo(rangeToSerialize.Start) == 0 || textPointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart) && textPointer.GetTextInRun(LogicalDirection.Forward) == " ")
                {
                    list.Add(string.Empty);
                }
            }
            else if (pointerContext == TextPointerContext.EmbeddedElement)
            {
                // Declared first: Release IL numbers textElement before the container locals; adjacentElement gets no local.
                TextElement textElement;
                DependencyObject adjacentElement = textPointer.GetAdjacentElement(LogicalDirection.Backward);
                InlineUIContainer inlineUIContainer = adjacentElement as InlineUIContainer;
                BlockUIContainer blockUIContainer = adjacentElement as BlockUIContainer;
                if (inlineUIContainer != null)
                {
                    textElement = inlineUIContainer;
                    IWhiteboardItemInTextWrapper wrapper = GetWrapper(inlineUIContainer.Child);
                    if (wrapper != null)
                    {
                        richTextEmbeddedItemsInfoData |= RichTextEmbeddedItemsInfo.HasEmbeddedMath;
                        if (embeddedEquationItems.Contains(wrapper.WrappedItem))
                        {
                            embeddedEquationItems.Remove(wrapper.WrappedItem);
                        }
                        else
                        {
                            DebugTools.FailAssertion("Found embedded element, that wasn't listed by SelectedItems() method.");
                        }
                    }
                    else
                    {
                        richTextEmbeddedItemsInfoData |= RichTextEmbeddedItemsInfo.HasImages;
                    }
                }
                else
                {
                    textElement = blockUIContainer;
                    richTextEmbeddedItemsInfoData |= RichTextEmbeddedItemsInfo.HasImages;
                }
                if (textElement != null)
                {
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    using (Quire.MindTouch.IO.ChunkedMemoryStream chunkedMemoryStream = new Quire.MindTouch.IO.ChunkedMemoryStream())
                    {
                        using (System.IO.StreamWriter streamWriter = Quire.Xml.XmlHelper.CreateStreamWriterForXamlCopy(chunkedMemoryStream, leaveOpen: true))
                        {
                            DocumentAttachedProperties.SetIsInCopy(this, value: true);
                            System.Windows.Markup.XamlWriter.Save(textElement, streamWriter);
                            streamWriter.Flush();
                            DocumentAttachedProperties.ClearIsInCopy(this);
                        }
                        chunkedMemoryStream.Position = 0L;
                        xmlDocument.Load(chunkedMemoryStream);
                    }
                    list.Add(xmlDocument.InnerXml);
                }
                else
                {
                    list.Add(string.Empty);
                    DebugTools.FailAssertion("Found embedded element, that isn't 'InlineUIContainer' nor 'BlockUIContainer'.");
                }
            }
            textPointer = textPointer.GetPositionAtOffset(1);
        }
        DebugTools.DevAssert(embeddedEquationItems.IsNullOrEmpty(), "Some embedded equations weren't found during previous loop. The XAML we will create will lack 'non-found' equations.");
        return GetCombinedDocumentXaml(xaml, list);
    }

    private global::Quire.Controls.Text.IRichTextWithEmbededItemCloneHelper _richTextWithEmbededItemCloneHelper;

    private static string GetCombinedDocumentXaml(string xaml, IList<string> itemXamls)
    {
        if (DebugTools.AssertNotNull(xaml, "Copying rich text w/ embedded item, did not find expected Xaml data format").Failed)
        {
            return null;
        }
        if (itemXamls == null)
        {
            return null;
        }
        int num = itemXamls.Sum((string itemXaml) => itemXaml.Length);
        if (num == 0)
        {
            return null;
        }
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(xaml.Length + num);
        int num2 = 0;
        foreach (string itemXaml in itemXamls)
        {
            bool flag = itemXaml.StartsWith("<BlockUIContainer");
            string text = (flag ? "> </Paragraph>" : "> </Run>");
            string value = (flag ? "<Paragraph" : "<Run");
            int num3 = xaml.IndexOf(text, num2);
            if (string.IsNullOrEmpty(itemXaml))
            {
                stringBuilder.Append(xaml, num2, num3 + text.Length - num2);
            }
            else
            {
                int num4 = xaml.LastIndexOf(value, num3);
                stringBuilder.Append(xaml, num2, num4 - num2).Append(itemXaml);
            }
            num2 = num3 + text.Length;
        }
        stringBuilder.Append(xaml, num2, xaml.Length - num2);
        return stringBuilder.ToString();
    }

    protected override void PastingHandler(object sender, global::System.Windows.DataObjectPastingEventArgs e) => base.PastingHandler(sender, e);

    protected override void PrePasting() => base.PrePasting();

    protected override void PostPasting() => base.PostPasting();

    public IContainerItemManager ContainerItemManager => WhiteboardItemContainerExtensions.ItemManager;

    IUIElementArrangeInfoProvider IHasChildArrangeInfoProvider.ChildArrangeInfoProvider => ContainerItemManager;

    public override int CaretOffset { get => base.CaretOffset; set => base.CaretOffset = value; }

    public override IEnumerable<object> EnumerateEmbeddedObjectsInRange(DocumentTextRange range)
    {
        for (TextPointer curPos = range.Start; curPos != null && curPos.CompareTo(range.End) < 0; curPos = curPos.GetPositionAtOffset(1, LogicalDirection.Forward))
        {
            if (curPos.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement)
            {
                object obj = curPos.GetAdjacentElement(LogicalDirection.Forward);
                IWhiteboardItemInTextWrapper wrapper = GetWrapper(obj);
                if (wrapper != null)
                {
                    obj = wrapper.WrappedItem;
                }
                yield return obj;
            }
        }
    }

    private void AddItemToList(IWhiteboardItemInText item)
    {
        if (WhiteboardItems.Contains(item))
        {
            return;
        }

        item.OnGotFocusEvent += HandleChildItemGotFocus;
        item.RequestRemove += HandleChildItemRequestRemove;
        InlineUIContainer container = GetContainer(item);
        if (DebugTools.AssertNotNull(container, "Adding an embedded item that is not hosted in the document.").Failed)
        {
            return;
        }

        int index = 0;
        while (index < WhiteboardItems.Count)
        {
            InlineUIContainer other = GetContainer(WhiteboardItems[index] as IWhiteboardItemInText);
            if (other != null && container.ElementStart.CompareTo(other.ElementStart) < 0)
            {
                WhiteboardItems.Insert(index, item);
                return;
            }

            index++;
        }

        SetItemContextMenuHandling(item);
        WhiteboardItems.Add(item);
    }

    private void RemoveItemFromList(IWhiteboardItemInText item)
    {
        if (!WhiteboardItems.Contains(item))
        {
            return;
        }
        WhiteboardItems.Remove(item);
        if (item == ActiveItem)
        {
            ActiveItem = null;
        }
        item.OnGotFocusEvent -= HandleChildItemGotFocus;
        item.RequestRemove -= HandleChildItemRequestRemove;
    }

    private void ExecuteAddItemToListCommand(global::Quire.Controls.Core.IWhiteboardItemInText item)
    {
        if (WhiteboardItems.Contains(item))
        {
            return;
        }

        InlineUIContainer container = GetContainer(item);
        if (DebugTools.AssertNotNull(container, "Adding an embedded item that is not hosted in the document.").Failed)
        {
            return;
        }

        int offset = textType.GetOffsetToPosition(Document, container.ElementStart);
        bool isItemActive = ContainerItemManager.IsItemActivated(item);
        DelegateFunction0<bool> doMethod = () =>
        {
            AddItemToList(item);
            if (isItemActive)
            {
                item.Activate(isActive: true);
            }

            return true;
        };
        Delegate0 undoMethod = () => RemoveItemFromList(item);
        Delegate0 redoMethod = () => RestoreTextBoxContent(item, offset, isItemActive, isRedo: true);
        CommandBuffer.Execute(new UndoableCommand("Add embedded item to list", doMethod, undoMethod, redoMethod));
    }

    private void ExecuteRemoveItemFromListCommand(global::Quire.Controls.Core.IWhiteboardItemInText item)
    {
        int offset;
        bool isItemActive;
        if (!WhiteboardItems.Contains(item))
        {
            return;
        }

        offset = _itemOffsetMap[item];
        isItemActive = ContainerItemManager.IsItemActivated(item);
        DelegateFunction0<bool> doMethod = () =>
        {
            RemoveItemFromList(item);
            return true;
        };
        Delegate0 undoMethod = () => RestoreTextBoxContent(item, offset, isItemActive, isRedo: false);
        CommandBuffer.Execute(new UndoableCommand("Remove embedded item from list", doMethod, undoMethod));
    }

    private void RestoreTextBoxContent(global::Quire.Controls.Core.IWhiteboardItemInText item, int offset, bool isItemActive, bool isRedo)
    {
        if (textType.GetPositionAtOffset(Document, offset).GetAdjacentElement(LogicalDirection.Forward) is InlineUIContainer container)
        {
            SetItem(container, item);
            AddItemToList(item);
            CommitItem(item);
            if (isItemActive)
            {
                item.Activate(isActive: true);
            }
            else if (isRedo)
            {
                ContainerItemManager.ExecuteInBackground(() => Focus(), asynch: true);
            }
        }
    }

    private void UpdateItemOffsetMap()
    {
        foreach (IWhiteboardItem item in WhiteboardItems)
        {
            IWhiteboardItemInText embeddedItem = item as IWhiteboardItemInText;
            if (IsItemRemovedFromDocument(embeddedItem))
            {
                _itemOffsetMap.Remove(embeddedItem);
            }
            else
            {
                _itemOffsetMap[embeddedItem] = textType.GetOffsetToPosition(Document, GetContainer(embeddedItem).ElementStart);
            }
        }
    }

    private bool IsItemRemovedFromDocument(IWhiteboardItemInText item)
    {
        InlineUIContainer container = GetContainer(item);
        if (container != null)
        {
            return !Document.ContentStart.IsInSameDocument(container.ElementStart);
        }
        return true;
    }

    protected override void OnGotFocus(global::System.Windows.RoutedEventArgs e) => base.OnGotFocus(e);

    protected override void OnLostFocus(global::System.Windows.RoutedEventArgs e) => base.OnLostFocus(e);

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        if (!IsFocused)
        {
            return;
        }
        base.OnContextMenuOpening(e);
    }

    private void HandleChildItemGotFocus(IWhiteboardItem item)
    {
        if (item != ActiveItem && WhiteboardItems.Contains(item))
        {
            ActiveItem = item as IWhiteboardItemInText;
        }
    }

    private void HandleChildItemRequestRemove(IWhiteboardItem item, Direction dir)
    {
        RemoveItem(item);
        Activate(true);
    }

    private void HandleItemsListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                IEnumerable<IWhiteboardItemInText> addedItems = e.NewItems.Cast<IWhiteboardItemInText>();
                AddItems(addedItems);
                return;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                IEnumerable<IWhiteboardItemInText> removedItems = e.OldItems.Cast<IWhiteboardItemInText>();
                RemoveItems(removedItems);
                return;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                IEnumerable<IWhiteboardItemInText> movedItems = e.NewItems.Cast<IWhiteboardItemInText>();
                MoveItems(movedItems);
                return;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                return;
        }
    }

    // Compile-order device: lambda ordinals 128-130 follow the members above in Release IL.
    protected virtual void AddItems(IEnumerable<IWhiteboardItemInText> items)
    {
        items.Each((IWhiteboardItemInText item) =>
        {
            _childManager.ChildAdded(item);
        });
    }

    protected virtual void RemoveItems(IEnumerable<IWhiteboardItemInText> items)
    {
        items.Each((IWhiteboardItemInText item) =>
        {
            _childManager.ChildRemoved(item);
        });
    }

    protected virtual void MoveItems(IEnumerable<IWhiteboardItemInText> items)
    {
        items.Each((IWhiteboardItemInText item) =>
        {
            _childManager.ChildMoved(item);
        });
    }

    private void OnPreviewEnter(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem == null)
        {
            return;
        }
        IWhiteboardItemInText activeItem = (IWhiteboardItemInText)ActiveItem;
        if (activeItem.CanReturn())
        {
            return;
        }
        ActiveItem = null;
        Activate(true);
        InlineUIContainer container = GetContainer(activeItem);
        if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
        {
            SetCaretPositionAndClearSuggestedX(container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward));
        }
        e.Handled = true;
    }

    private void ResetCaretPositionWhenNecessary(TextPointer referenceLineStartPosition)
    {
        if (referenceLineStartPosition.GetPointerContext(LogicalDirection.Backward) != TextPointerContext.EmbeddedElement)
        {
            return;
        }
        TextPointer leftOfElement = referenceLineStartPosition.GetNextInsertionPosition(LogicalDirection.Backward);
        ContainerItemManager.ExecuteInBackground(() =>
        {
            if (CaretPosition.CompareTo(referenceLineStartPosition) >= 0)
            {
                CaretPosition = leftOfElement;
            }
        }, asynch: true);
    }

    private void OnPreviewMoveUpByLine(object sender, ExecutedRoutedEventArgs e)
    {
        TextPointer lineStart;
        if (ActiveItem != null)
        {
            IWhiteboardItemInText item = ActiveItem as IWhiteboardItemInText;
            if (!item.CanNavigate(Direction.Up))
            {
                e.Handled = true;
                ActiveItem = null;
                Activate(true);
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    SetCaretPositionAndClearSuggestedX(container.ElementStart.GetInsertionPosition(LogicalDirection.Backward));
                }

                EditingCommands.MoveUpByLine.Execute(null, this);
            }

            return;
        }
        else
        {
            if (CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Up))
            {
                return;
            }

            if (GetLineStartPosition(CaretPosition, -1) == null)
            {
                return;
            }

            lineStart = GetLineStartPosition(CaretPosition, 0);
            ResetCaretPositionWhenNecessary(lineStart);
        }
    }

    private void OnPreviewMoveDownByLine(object sender, ExecutedRoutedEventArgs e)
    {
        TextPointer lineStart;
        if (ActiveItem != null)
        {
            IWhiteboardItemInText item = ActiveItem as IWhiteboardItemInText;
            if (!item.CanNavigate(Direction.Down))
            {
                ActiveItem = null;
                Activate(true);
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    SetCaretPositionAndClearSuggestedX(container.ElementStart.GetInsertionPosition(LogicalDirection.Backward));
                }

                EditingCommands.MoveDownByLine.Execute(null, this);
            }

            return;
        }
        else
        {
            if (CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Down))
            {
                return;
            }

            lineStart = GetLineStartPosition(CaretPosition, 2);
            if (lineStart == null)
            {
                return;
            }

            ResetCaretPositionWhenNecessary(lineStart);
        }
    }

    private void OnPreviewMoveLeftByCharacter(object sender, ExecutedRoutedEventArgs e)
    {
        TextPointer caret;
        TextPointer next;
        if (ActiveItem != null)
        {
            IWhiteboardItemInText item = ActiveItem as IWhiteboardItemInText;
            if (!item.CanNavigate(Direction.Left))
            {
                ActiveItem = null;
                Activate(true);
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    SetCaretPositionAndClearSuggestedX(container.ElementStart.GetInsertionPosition(LogicalDirection.Backward));
                }
            }

            return;
        }

        caret = CaretPosition;
        next = CaretPosition.GetNextInsertionPosition(LogicalDirection.Backward);
        if (next != null)
        {
            while (caret.CompareTo(next) > 0)
            {
                if (caret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.EmbeddedElement)
                {
                    InlineUIContainer embeddedContainer = caret.Parent as InlineUIContainer;
                    if (embeddedContainer != null)
                    {
                        IWhiteboardItemInText embeddedItem = GetItem(embeddedContainer);
                        if (embeddedItem != null)
                        {
                            embeddedItem.NavigateIntoItem(Direction.Left);
                            e.Handled = true;
                            return;
                        }
                    }
                }

                caret = caret.GetPositionAtOffset(-1);
            }
        }

        CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Left);
    }

    private void OnPreviewMoveRightByCharacter(object sender, ExecutedRoutedEventArgs e)
    {
        TextPointer caret;
        TextPointer next;
        if (ActiveItem != null)
        {
            IWhiteboardItemInText item = ActiveItem as IWhiteboardItemInText;
            if (!item.CanNavigate(Direction.Right))
            {
                ActiveItem = null;
                Activate(true);
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    SetCaretPositionAndClearSuggestedX(container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward));
                }
            }

            return;
        }

        caret = CaretPosition;
        next = CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
        if (next != null)
        {
            while (caret.CompareTo(next) < 0)
            {
                if (caret.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement)
                {
                    InlineUIContainer embeddedContainer = caret.Parent as InlineUIContainer;
                    if (embeddedContainer != null)
                    {
                        IWhiteboardItemInText embeddedItem = GetItem(embeddedContainer);
                        if (embeddedItem != null)
                        {
                            embeddedItem.NavigateIntoItem(Direction.Right);
                            e.Handled = true;
                            return;
                        }
                    }
                }

                caret = caret.GetPositionAtOffset(1);
            }
        }

        CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Right);
    }

    protected override void HandleTextChanged(object sender, global::System.Windows.Controls.TextChangedEventArgs args) => base.HandleTextChanged(sender, args);

    protected override global::Quire.Undo.IUndoableCommand CreateTextChangedCommand(global::System.Windows.Controls.TextChangedEventArgs args) => base.CreateTextChangedCommand(args);

    public WhiteboardItemsCollection WhiteboardItems => _whiteboardItems;

    public IWhiteboardItem ActiveItem
    {
        get => _activeItem;
        set
        {
            if (_activeItem == value)
            {
                return;
            }

            if (value != null && !WhiteboardItems.Contains(value))
            {
                throw new ArgumentException("Only a child can be set as an ActiveItem");
            }

            if (_activeItem != null)
            {
                DeactivateActiveItem();
            }

            _activeItem = value;
            if (_activeItem != null)
            {
                ClearSavedState();
                ActivateActiveItem();
                Activate(true);
            }

            UpdateCurrentFormats();
            UpdateHideFromCalculationButton();
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(ActiveItem)));
        }
    }

    public bool CanAddItems
    {
        get
        {
            if (ActiveItem == null)
            {
                return !SelectionFullyInsideHyperlink;
            }

            return false;
        }
    }

    public void AddItem(IWhiteboardItem item)
    {
        AddEmbeddedItem(item as IWhiteboardItemInText, true);
    }

    // Compile-order device: these three members come before the font-colour and iterator closures so their Release ordinals match.
    public void RemoveItem(IWhiteboardItem item)
    {
        RemoveEmbeddedItem(item as IWhiteboardItemInText);
    }

    public bool IsTabNavigatable => WhiteboardItems.Any((IWhiteboardItem item) => ContainerItemManager.GetIsTabNavigatable(item));

    public void TabInto(FocusNavigationDirection dir)
    {
        ContainerItemManager.TabInto(dir == FocusNavigationDirection.Next ? WhiteboardItems.First((IWhiteboardItem item) => ContainerItemManager.IsTabNavigatable(item)) : WhiteboardItems.Last((IWhiteboardItem item) => ContainerItemManager.IsTabNavigatable(item)), dir);
    }

    private void SaveState()
    {
        _savedActiveItem = ActiveItem;
    }

    private void ClearSavedState()
    {
        _savedActiveItem = null;
    }

    private void RestoreSavedState()
    {
        if (_savedActiveItem != null && WhiteboardItems.Contains(_savedActiveItem))
        {
            ActiveItem = _savedActiveItem;
        }
    }

    private void ClearState()
    {
        ActiveItem = null;
    }

    public void AddEmbeddedItem(IWhiteboardItemInText item, bool activate)
    {
        using (CommandScope commandScope = new CommandScope("Add embedded item"))
        {
            if (activate)
            {
                ContainerItemManager.ActivateItem(item);
            }
            using (DeclareChangeBlock())
            {
                if (!Selection.IsEmpty)
                {
                    Selection.Text = "";
                }
                AddEmbeddedItem(item, null);
            }
        }
    }

    public void AddEmbeddedItem(IWhiteboardItemInText item, TextPointer textPointer = default)
    {
        FrameworkElement element = item as FrameworkElement;
        if (element != null)
        {
            global::Quire.Controls.Core.IORegions.IOTag.SetIOTag(element, global::Quire.Controls.Core.IORegions.IOTagType.None);
        }

        InlineUIContainer container = _richTextWithEmbededItemCloneHelper.Insert(item, textPointer ?? CaretPosition);
        if (item.BookmarksManager == null)
        {
            item.BookmarksManager = BookmarksManager;
        }

        SetCaretPositionAndClearSuggestedX(container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward));
    }

    public void AddEmbeddedItem(IWhiteboardItemInText equation, int textCaretPosition)
    {
        AddEmbeddedItem(equation, textType.GetPositionAtOffset(Document, textCaretPosition));
    }

    public void RemoveEmbeddedItem(IWhiteboardItemInText item)
    {
        InlineUIContainer container = GetContainer(item);
        if (DebugTools.AssertNotNull(container, "Removing an embedded item that is not hosted in the document.").Failed)
        {
            return;
        }
        SetCaretPositionAndClearSuggestedX(container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward));
        if (item.BookmarksManager != null)
        {
            item.BookmarksManager = null;
        }
        container.ElementStart.Paragraph.Inlines.Remove(container);
    }

    public int GetEmbeddedItemSymbolOffset(IWhiteboardItemInText item)
    {
        return textType.GetOffsetToPosition(Document, GetContainer(item).ElementStart);
    }

    public void PasteItem(IWhiteboardItemInText item)
    {
        if (DebugTools.DevAssert(CanAddItems).Failed)
        {
            return;
        }
        AddEmbeddedItem(item, false);
        CommitItem(item);
    }

    protected override void OnDragOver(System.Windows.DragEventArgs e)
    {
        base.OnDragOver(e);
        if (!CanDrop(e))
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private static bool DataObjectContainsAnyEmbeddedItems(IDataObject dataObject, RichTextEmbeddedItemsInfo filter = RichTextEmbeddedItemsInfo.HasEmbeddedImagesAndMath)
    {
        if (dataObject.GetTextWithEmbeddedItemsInfo() is RichTextEmbeddedItemsInfo info)
        {
            return (info & filter) != 0;
        }
        return false;
    }

    protected override void OnPreviewDrop(global::System.Windows.DragEventArgs e) => base.OnPreviewDrop(e);

    private bool CanDrop(System.Windows.DragEventArgs args)
    {
        TextPointer position = GetPositionFromPoint(args.GetPosition(this), false);
        if (DataObjectContainsAnyEmbeddedItems(args.Data, RichTextEmbeddedItemsInfo.HasEmbeddedImagesAndMath))
        {
            return GetHyperlinkFromTextPointer(position) == null;
        }
        return true;
    }

    public void ReplacePlaceholderWithEmbeddedItem(IWhiteboardItemInText item, int symbolOffset)
    {
        TextPointer position = textType.GetPositionAtOffset(Document, symbolOffset);
        if (position != null)
        {
            Run run = position.GetAdjacentElement(LogicalDirection.Forward) as Run;
            if (run != null && run.Text == " ")
            {
                var paragraph = position.Paragraph;
                if (DebugTools.AssertNotNull(paragraph).Passed)
                {
                    DebugTools.DevAssert(RemoveInlineRecursively(paragraph.Inlines, run), "Failed to remove object placeholder, see if RemoveInlineRecursively needs to be updated");
                }
            }
            else
            {
                DebugTools.FailAssertion("Expected object placeholder (empty run) was not found, and therefore the object was inserted w/o replacing anything.");
            }
            AddEmbeddedItem(item, position);
        }
    }

    private static bool RemoveInlineRecursively(global::System.Windows.Documents.InlineCollection inlines, global::System.Windows.Documents.Inline inlineToRemove)
    {
        if (!inlines.Remove(inlineToRemove))
        {
            return inlines.OfType<Span>().Any((Span span) => RemoveInlineRecursively(span.Inlines, inlineToRemove));
        }

        return true;
    }

    private void LogDocumentContents(string suffix = default) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public IEnumerable<object> EmbeddedObjects
    {
        get
        {
            IList<object> embeddedObjects = new List<object>();
            TextPointer pointer = Document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement)
                {
                    object element = pointer.GetAdjacentElement(LogicalDirection.Forward);
                    IWhiteboardItemInTextWrapper wrapper = GetWrapper(element);
                    if (wrapper != null)
                    {
                        element = wrapper.WrappedItem;
                    }

                    embeddedObjects.Add(element);
                }

                pointer = pointer.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return embeddedObjects;
        }
    }

    public int GetEmbeddedObjectCaretOffset(object embeddedObject) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanBookmarkTag(object sender, CanExecuteRoutedEventArgs e)
    {
        bool hasActiveItem = ActiveItem != null;
        e.CanExecute = hasActiveItem;
        e.Handled = hasActiveItem;
    }

    private void HandleBookmarkTag(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.AssertNotNull(ActiveItem).Failed)
        {
            return;
        }
        string tag = BookmarkTag.GetBookmarkTag(ActiveItem as DependencyObject);
        BookmarkTagViewModel viewModel = new BookmarkTagViewModel(tag);
        if (DebugTools.AssertNotNull(BookmarksManager).Passed && BookmarkTagDialog.ShowBookmarkTagsDialog(viewModel, Application.Current.MainWindow))
        {
            BookmarksManager.SetBookmarkTagCommand(ActiveItem, tag, viewModel.Tag);
        }
    }

    private void CanRemoveBookmarkTag(object sender, CanExecuteRoutedEventArgs e)
    {
        bool hasActiveItem = ActiveItem != null;
        e.CanExecute = hasActiveItem && BookmarkTag.HasBookmarkTag(ActiveItem as DependencyObject);
        e.Handled = hasActiveItem;
    }

    private void HandleRemoveBookmarkTag(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.AssertNotNull(ActiveItem).Failed || DebugTools.AssertNotNull(BookmarksManager).Failed)
        {
            return;
        }
        string bookmarkTag = BookmarkTag.GetBookmarkTag(ActiveItem as DependencyObject);
        BookmarksManager.RemoveBookmarkTagCommand(ActiveItem, bookmarkTag);
    }

    public void CommitItem(IWhiteboardItem item)
    {
        ICalculatable calculatable = item as ICalculatable;
        if (calculatable != null)
        {
            calculatable.Commit();
        }
    }

    public void ActivateActiveItem()
    {
        if (ActiveItem == null)
        {
            return;
        }
        ActiveItem.Activate(true);
        ContainerItemManager.ActivateItem(ActiveItem);
        SetItemContextMenuHandling(ActiveItem);
    }

    protected virtual void DeactivateActiveItem()
    {
        if (ActiveItem == null)
        {
            return;
        }
        ActiveItem.Activate(false);
        ContainerItemManager.DeactivateItem(ActiveItem);
        ICalculatable calculatable = ActiveItem as ICalculatable;
        if (calculatable != null)
        {
            ICalculatableInfo calculatableInfo = calculatable as ICalculatableInfo;
            if (calculatableInfo == null || !calculatableInfo.IsCommitted)
            {
                calculatable.Commit();
            }
        }
        SetItemContextMenuHandling(ActiveItem);
    }

    private void SetItemContextMenuHandling(IWhiteboardItem item)
    {
        if (ContainerItemManager.IsItemActivated(item))
        {
            ContainerItemManager.ClearItemHandlesContextMenus(item);
            return;
        }
        ContainerItemManager.SetItemHandlesContextMenus(item, ContainerItemManager.GetItemHandlesContextMenus(this));
    }

    public override void Activate(bool isActive) => base.Activate(isActive);

    public IRegionFactory RegionFactory { get; set; }

    public ITextStyleChangedHandler TextStyleChangedHandler
    {
        get { return _textStyleChangedHandler; }
        set
        {
            _textStyleChangedHandler = value;
            if (_textStyleChangedHandler != null)
            {
                _textStyleChangedHandler.StyleChanged += OnStyleChanged;
            }
        }
    }

    protected override void OnStyleChanged(ICollection<TextStyleData> TextStyles)
    {
        base.OnStyleChanged(TextStyles);
    }

    protected override global::Quire.PersistentDataObjects.textType DoGetRegionPersistentDataInternal(global::Quire.Serialization.IS11NProvider s11NProvider) => base.DoGetRegionPersistentDataInternal(s11NProvider);

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        IList<IRegionPersistentData> regions = null;
        base.SetRegionPersistentData(d11NProvider, data);
        textType text = (textType)data;
        if (text.Regions != null && text.Regions.Any())
        {
            regions = _deserializationStrategy.SetRegions(text.Regions, d11NProvider);
            CaretPosition = Document.ContentStart;
        }
        return regions;
    }

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;

    public IEnumerable<ICalculatable> CalculatableChildren => _childManager.CalculatableChildren;

    protected virtual void FireCalculatableAdded(ICalculatable calc)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(this, calc);
        }
    }

    protected virtual void FireCalculatableRemoved(ICalculatable calc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calc);
        }
    }

    protected virtual void FireCalculatableMoved(ICalculatable calc)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(calc);
        }
    }

    protected global::Quire.Calculation.Core.Support.CalculatableChildHider InitializeChildManager() => new global::Quire.Calculation.Core.Support.CalculatableChildHider(WhiteboardItems, FireCalculatableAdded, FireCalculatableRemoved, FireCalculatableMoved, true);

    private InlineUIContainer GetContainer(IWhiteboardItemInText item)
    {
        IWhiteboardItemInTextWrapper wrapper = ContainerItemManager.GetParent(item) as IWhiteboardItemInTextWrapper;
        if (wrapper == null)
        {
            return null;
        }
        object parent = ContainerItemManager.GetParent(wrapper);
        Grid grid = parent as Grid;
        if (grid != null)
        {
            parent = ContainerItemManager.GetParent(grid);
        }
        return parent as InlineUIContainer;
    }

    private IWhiteboardItemInText GetItem(InlineUIContainer container)
    {
        return GetWrapper(container.Child)?.WrappedItem;
    }

    public override void EndInit()
    {
        base.EndInit();
        RepairWrapperHosts();
    }

    private void RepairWrapperHosts()
    {
        List<KeyValuePair<InlineUIContainer, UIElement>> list = new List<KeyValuePair<InlineUIContainer, UIElement>>();
        for (TextPointer textPointer = Document.ContentStart; textPointer != null; textPointer = textPointer.GetNextContextPosition(LogicalDirection.Forward))
        {
            if (textPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement && textPointer.GetAdjacentElement(LogicalDirection.Forward) is Grid { Parent: InlineUIContainer parent } grid && GetWrapper(grid) is UIElement value)
            {
                list.Add(new KeyValuePair<InlineUIContainer, UIElement>(parent, value));
            }
        }
        if (list.Count == 0)
        {
            return;
        }
        using (new CommandScope(new CompoundCommand()))
        {
            foreach (KeyValuePair<InlineUIContainer, UIElement> item in list)
            {
                ((Grid)item.Key.Child).Children.Remove(item.Value);
                item.Key.Child = item.Value;
            }
        }
    }

    private static IWhiteboardItemInTextWrapper GetWrapper(object embeddedElement)
    {
        IWhiteboardItemInTextWrapper wrapper = embeddedElement as IWhiteboardItemInTextWrapper;
        if (wrapper != null)
        {
            return wrapper;
        }
        Grid grid = embeddedElement as Grid;
        if (grid == null)
        {
            return null;
        }
        IWhiteboardItemInTextWrapper result = null;
        foreach (UIElement element in grid.Children)
        {
            IWhiteboardItemInTextWrapper candidate = element as IWhiteboardItemInTextWrapper;
            if (candidate != null)
            {
                if (DebugTools.DevAssert(result == null, "More than one wrapper hosted in the Grid of an embedded item.").Failed)
                {
                    break;
                }
                result = candidate;
            }
        }
        return result;
    }

    private void SetItem(InlineUIContainer container, IWhiteboardItemInText item)
    {
        IWhiteboardItemInTextWrapper oldWrapper = ContainerItemManager.GetParent(item) as IWhiteboardItemInTextWrapper;
        if (oldWrapper != null)
        {
            oldWrapper.WrappedItem = null;
        }
        IWhiteboardItemInTextWrapper wrapper = GetWrapper(container.Child);
        if (wrapper != null)
        {
            wrapper.WrappedItem = item;
            return;
        }
        Grid grid = container.Child as Grid;
        if (grid != null)
        {
            grid.Children.Clear();
            grid.Children.Add(new WhiteboardItemInTextWrapper(item));
            return;
        }
        DebugTools.FailAssertion("Unexpected InlineUIContainer.Child: " + container.Child?.GetType().FullName);
    }

    public virtual object Clone()
    {
        return CloneRichText(null);
    }

    public IRichTextWithEmbeddedItem CloneRichText(IEngineeringDocument engineeringDocument)
    {
        RichTextWithEmbeddedItem clone = new RichTextWithEmbeddedItem();
        clone.CloneContent(this, engineeringDocument, false);
        return clone;
    }

    protected void CloneContent(RichTextWithEmbeddedItem original, IEngineeringDocument engDoc, bool export = false)
    {
        if (engDoc != null)
        {
            _richTextWithEmbededItemCloneHelper = new ItemToImagesRichTextWithEmbededItemCloneHelper(engDoc);
        }
        using (new CommandScope(new CompoundCommand()))
        {
            CloneContent(original);
            foreach (IWhiteboardItem item2 in original.WhiteboardItems.Reverse())
            {
                IWhiteboardItemInText whiteboardItemInText = item2 as IWhiteboardItemInText;
                if (DebugTools.AssertNotNull(whiteboardItemInText, null).Failed)
                {
                    return;
                }
                int embeddedItemSymbolOffset = original.GetEmbeddedItemSymbolOffset(whiteboardItemInText);
                Rect? boundsConstraint = null;
                if (DebugTools.DevAssert(whiteboardItemInText is FrameworkElement, null).Passed && WhiteboardDependencyProperties.GetShowResizeIndicator(whiteboardItemInText as FrameworkElement))
                {
                    boundsConstraint = new Rect(ContainerItemManager.GetRenderSize(whiteboardItemInText));
                }
                using CopyWidgetBase popup = _richTextWithEmbededItemCloneHelper.CreatePopup();
                IWhiteboardItemInText item = _richTextWithEmbededItemCloneHelper.Clone(whiteboardItemInText, boundsConstraint, popup);
                ReplacePlaceholderWithEmbeddedItem(item, embeddedItemSymbolOffset);
            }
        }
        _richTextWithEmbededItemCloneHelper = _defaultCloneHelper;
    }

    // Compile-order device: these fields precede the ordinal 219/220 closure anchors.
    private readonly global::Quire.Controls.Text.IRichTextWithEmbededItemCloneHelper _defaultCloneHelper;

    private global::Quire.Core.ISnapToPixelHelper _snappingHelper;

    public bool CanHideFromCalculationContext => !WhiteboardItems.OfType<ICalculationHideableChild>().IsNullOrEmpty();

    public bool HideFromCalculationContext
    {
        get => !WhiteboardItems.OfType<ICalculationHideableChild>().Any((ICalculationHideableChild hideable) => !hideable.HideFromCalculationContext);
        set
        {
            IEnumerable<ICalculationHideableChild> hideableChangeList = from hideable in WhiteboardItems.OfType<ICalculationHideableChild>()
                where hideable.HideFromCalculationContext != value
                select hideable;
            if (!hideableChangeList.Any())
            {
                return;
            }

            CompoundCommand nestedCommand = new CompoundCommand();
            DelegateFunction0<bool> doMethod = () =>
            {
                using (new CommandScope(nestedCommand))
                {
                    hideableChangeList.Each((ICalculationHideableChild hideable) => hideable.HideFromCalculationContext = value);
                }

                FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("HideFromCalculationContext"));
                return true;
            };
            Delegate0 undoMethod = () =>
            {
                nestedCommand.Undo();
                FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("HideFromCalculationContext"));
            };
            Delegate0 redoMethod = () =>
            {
                nestedCommand.Redo();
                FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("HideFromCalculationContext"));
            };
            CommandBuffer.Execute(new UndoableCommand("Change HideFromCalculationContext on RichText with embedded items", doMethod, undoMethod, redoMethod));
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Text.RichTextWithEmbeddedItem+PreviewKeyDownData, Ptc.Controls.Text")]
    private partial struct PreviewKeyDownData
    {
    }

    private void ToggleHideFromCalculationContext()
    {
        IEnumerable<IWhiteboardItem> source = SelectionContainsEmbeddedItemsOnly ? SelectedWhiteboardItems : WhiteboardItems;
        IEnumerable<ICalculationHideableChild> visibleItems = from hideable in source.OfType<ICalculationHideableChild>()
            where !hideable.HideFromCalculationContext
            select hideable;
        using (new CommandScope("Changing HideFromCalculationContext"))
        {
            if (visibleItems.Any())
            {
                visibleItems.Each((ICalculationHideableChild hideable) => hideable.HideFromCalculationContext = true);
            }
            else
            {
                source.OfType<ICalculationHideableChild>().Each((ICalculationHideableChild hideable) => hideable.HideFromCalculationContext = false);
            }
        }

        UpdateHideFromCalculationButton();
    }

    private void OnToggleHideFromCalculationContext(object sender, ExecutedRoutedEventArgs e)
    {
        ToggleHideFromCalculationContext();
    }

    private void CanToggleHideFromCalculationContext(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = HideFromCalculationButtonEnabled;
    }

    public bool HideFromCalculationButtonEnabled
    {
        get => _hideFromCalculationButtonEnabled;
        set
        {
            _hideFromCalculationButtonEnabled = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(HideFromCalculationButtonEnabled)));
        }
    }

    public bool HideFromCalculationButtonChecked
    {
        get => _hideFromCalculationButtonChecked;
        set
        {
            _hideFromCalculationButtonChecked = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(HideFromCalculationButtonChecked)));
        }
    }

    private void UpdateHideFromCalculationButton()
    {
        List<ICalculationHideableChild> items = new List<ICalculationHideableChild>();
        if (ActiveItem != null)
        {
            if (ActiveItem is ICalculationHideableChild item)
            {
                items.Add(item);
            }
        }
        else if (SelectionContainsEmbeddedItemsOnly)
        {
            items.AddRange(SelectedWhiteboardItems.OfType<ICalculationHideableChild>());
        }
        else
        {
            items.AddRange(WhiteboardItems.OfType<ICalculationHideableChild>());
        }

        bool hasHideableItems = items.Any();
        bool hideFromCalculationButtonChecked = true;
        if (!hasHideableItems || items.Any((ICalculationHideableChild item) => !item.HideFromCalculationContext))
        {
            hideFromCalculationButtonChecked = false;
        }

        HideFromCalculationButtonEnabled = hasHideableItems;
        HideFromCalculationButtonChecked = hideFromCalculationButtonChecked;
    }

    public bool ContainsError(EngineError error)
    {
        foreach (ICalculatable child in CalculatableChildren)
        {
            ICalculationErrorOwner owner = child as ICalculationErrorOwner;
            if (owner != null && owner.ContainsError(error))
            {
                return true;
            }
        }
        return false;
    }

    public void ShowError(EngineError error)
    {
        foreach (ICalculatable child in CalculatableChildren)
        {
            ICalculationErrorOwner owner = child as ICalculationErrorOwner;
            if (owner != null && owner.ContainsError(error))
            {
                owner.ShowError(error);
                return;
            }
        }
    }

    public EngineError CurrentError
    {
        get
        {
            ICalculationErrorOwner errorOwner = ActiveItem as ICalculationErrorOwner;
            if (errorOwner != null)
            {
                return errorOwner.CurrentError;
            }

            return null;
        }
    }

    public override bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        bool findInText = (options & FindOptions.FindInText) == FindOptions.FindInText;
        if (!findInText && WhiteboardItems.Count == 0)
        {
            return false;
        }
        TextPointer textPointer = direction == LogicalDirection.Forward ? Document.ContentStart : Document.ContentEnd;
        if (startFromCaret)
        {
            if (ActiveItem == null)
            {
                textPointer = CaretPosition;
            }
            else
            {
                IWhiteboardItemInText item = (IWhiteboardItemInText)ActiveItem;
                if (FindInEmbeddedItem(item, text, startFromCaret, direction, options, subscriptPositions))
                {
                    return true;
                }
                InlineUIContainer container = GetContainer(item);
                if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
                {
                    // Each arm assigns textPointer: Release IL stores in both branches, not after a ternary.
                    if (direction == LogicalDirection.Forward)
                    {
                        textPointer = container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward);
                    }
                    else
                    {
                        textPointer = container.ElementStart.GetInsertionPosition(LogicalDirection.Backward);
                    }
                }
            }
        }
        if (direction == LogicalDirection.Forward)
        {
            foreach (IWhiteboardItemInText item2 in WhiteboardItems.Cast<IWhiteboardItemInText>())
            {
                InlineUIContainer container2 = GetContainer(item2);
                if (container2 == null)
                {
                    continue;
                }
                TextPointer insertionPosition = container2.ElementStart.GetInsertionPosition(LogicalDirection.Backward);
                if (insertionPosition.CompareTo(textPointer) >= 0)
                {
                    if (findInText && FindInRange(textPointer, insertionPosition, text, direction, options, subscriptPositions))
                    {
                        return true;
                    }
                    if (FindInEmbeddedItem(item2, text, startFromCaret: false, direction, options, subscriptPositions))
                    {
                        ActiveItem = item2;
                        return true;
                    }
                    textPointer = container2.ElementEnd.GetInsertionPosition(LogicalDirection.Forward);
                }
            }
            if (findInText)
            {
                return FindInRange(textPointer, Document.ContentEnd, text, direction, options, subscriptPositions);
            }
            return false;
        }
        foreach (IWhiteboardItemInText item3 in WhiteboardItems.Reverse().Cast<IWhiteboardItemInText>())
        {
            InlineUIContainer container3 = GetContainer(item3);
            if (container3 == null)
            {
                continue;
            }
            TextPointer insertionPosition2 = container3.ElementEnd.GetInsertionPosition(LogicalDirection.Forward);
            if (insertionPosition2.CompareTo(textPointer) <= 0)
            {
                if (findInText && FindInRange(insertionPosition2, textPointer, text, direction, options, subscriptPositions))
                {
                    return true;
                }
                if (FindInEmbeddedItem(item3, text, startFromCaret: false, direction, options, subscriptPositions))
                {
                    ActiveItem = item3;
                    return true;
                }
                textPointer = container3.ElementStart.GetInsertionPosition(LogicalDirection.Backward);
            }
        }
        if (findInText)
        {
            return FindInRange(Document.ContentStart, textPointer, text, direction, options, subscriptPositions);
        }
        return false;
    }

    private static bool FindInEmbeddedItem(IWhiteboardItemInText item, string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        WhiteboardItemSearchRestrictor restrictor = WhiteboardItemSearchRestrictor.TryCreateFindRestrictor(item);
        if (restrictor == null)
        {
            return false;
        }
        return restrictor.FindOnce(text, startFromCaret, direction, options, subscriptPositions);
    }

    public NumberFormat CurrentNumberFormat
    {
        get => _currentNumberFormat;
        set
        {
            _currentNumberFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(CurrentNumberFormat)));
        }
    }

    public MatrixFormat CurrentMatrixFormat
    {
        get => _currentMatrixFormat;
        set
        {
            _currentMatrixFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(CurrentMatrixFormat)));
        }
    }

    public MathFormat CurrentMathFormat
    {
        get => _currentMathFormat;
        set
        {
            _currentMathFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(CurrentMathFormat)));
        }
    }

    public override global::Quire.Core.TextFormat CurrentTextFormat { get => base.CurrentTextFormat; set => base.CurrentTextFormat = value; }

    // Compile-order device: keeps the earlier closure ordinals (the TabInto stub removal lowers them by one).
    public override void SetSelection(int fromOffset, int toOffset)
    {
        ActiveItem = null;
        Activate(true);
        base.SetSelection(fromOffset, toOffset);
    }

    public override void SelectTo(int toOffset) => base.SelectTo(toOffset);

    public bool HasRegionsWithConverterAnnotations => this.HasRegionsWithConverterAnnotations();

    public IEnumerable<UIElement> RegionsWithConverterAnnotations => WhiteboardItemContainerExtensions.RegionsWithConverterAnnotations(this);

    private IEnumerable<IWhiteboardItem> SelectedWhiteboardItems => GetWhiteboardItemsInRange(Selection);

    private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GetWhiteboardItemsInRange(global::System.Windows.Documents.TextRange range) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void HandleSelectionChanged(object sender, global::System.EventArgs args) => base.HandleSelectionChanged(sender, args);

    protected override void UpdateCurrentFormats() => base.UpdateCurrentFormats();

    private void CanSetNumberFormat(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CurrentNumberFormat != null || CurrentMatrixFormat != null;
    }

    private void OnSetNumberFormat(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source != this)
        {
            return;
        }
        using (CommandScope commandScope = new CommandScope(e.Command.ToString()))
        {
            DispatchCommandToItems(e.Command as RoutedUICommand, e.Parameter, e.Command == EquationCommands.ClearResultFormat ? PropertiesForUpdate.CurrentNumberFormatClear : PropertiesForUpdate.CurrentNumberFormat);
        }
    }

    private void OnSetMatrixFormat(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source != this)
        {
            return;
        }
        using (CommandScope commandScope = new CommandScope(e.Command.ToString()))
        {
            DispatchCommandToItems(e.Command as RoutedUICommand, e.Parameter, PropertiesForUpdate.CurrentMatrixFormat);
        }
    }

    private void DispatchCommandToItems(RoutedCommand cmd, object parameter, PropertiesForUpdate propertyForUpdate)
    {
        IEnumerable<IWhiteboardItem> items = SelectedWhiteboardItems.Any() ? SelectedWhiteboardItems : WhiteboardItems;
        IRegionCollectionWithProperty regions = global::Quire.Services.AppServiceRegistry.GetService<IRegionCollectionWithPropertyCreator>().CreateRegionCollection(items);
        regions.DispatchCommand(cmd, parameter, propertyForUpdate);
        if (propertyForUpdate == PropertiesForUpdate.CurrentNumberFormat || propertyForUpdate == PropertiesForUpdate.CurrentNumberFormatClear)
        {
            CurrentNumberFormat = regions.CurrentNumberFormat;
            return;
        }
        if (propertyForUpdate == PropertiesForUpdate.CurrentMatrixFormat)
        {
            CurrentMatrixFormat = regions.CurrentMatrixFormat;
        }
    }

    private void DeleteActiveOrSelectedItems(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem != null)
        {
            RemoveItem(ActiveItem);
        }
        else
        {
            EditingCommands.Delete.Execute(null, this);
        }
        Activate(true);
    }

    private void CanDeleteActiveOrSelectedItems(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ActiveItem != null || SelectionContainsEmbeddedItemsOnly;
    }

    protected bool SelectionContainsEmbeddedItemsOnly
    {
        get
        {
            if (Selection.IsEmpty)
            {
                return false;
            }

            TextPointer pointer = Selection.Start;
            while (Selection.End.CompareTo(pointer) > 0)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    return false;
                }

                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement && GetWrapper(pointer.GetAdjacentElement(LogicalDirection.Forward)) == null)
                {
                    return false;
                }

                pointer = pointer.GetPositionAtOffset(1);
            }

            return true;
        }
    }

    private void ProhibitMouseClickOnItem(DependencyObject obj, bool prohibit)
    {
        if (obj == null)
        {
            return;
        }
        obj.SetValue(AttachedProperties.ProhibitMouseClickProperty, prohibit);
    }

    public IMathFormat DeserializeMathFormat
    {
        set { _deserializationStrategy.DeserializeMathFormat = value; }
    }

    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat
    {
        set { _deserializationStrategy.DeserializeLabeledIdFormat = value; }
    }

    public StaticMathFormat TargetMathFormat => new StaticMathFormat(new MathFormatDobjController(this));

    public IDictionary<IdLabels, ExtendedFont> TargetLabeledIdFormat => (IDictionary<IdLabels, ExtendedFont>)GetValue(AttachedProperties.LabeledIdFormatProperty);

    protected static TextPointer GetLineStartPosition(TextPointer current, int position)
    {
        return current.GetLineStartPosition(position);
    }

    protected static TextPointer GetFirstLine(FlowDocument doc)
    {
        return GetLineStartPosition(doc.ContentStart, 0);
    }

    protected static TextPointer GetLastLine(FlowDocument doc)
    {
        return GetLineStartPosition(doc.ContentEnd, 0);
    }

    protected static TextPointer GetNextLine(TextPointer line)
    {
        if (line != null)
        {
            return GetLineStartPosition(line, 1);
        }
        return null;
    }

    public Rect FlowDocumentRect
    {
        get
        {
            global::System.Windows.Media.Visual contentHost = null;
            FrameworkElement element = Template.FindName("PART_ContentHost", this) as FrameworkElement;
            if (element is ScrollViewer)
            {
                contentHost = (element as ScrollViewer).Template.FindName("PART_ScrollContentPresenter", element) as global::System.Windows.Media.Visual;
            }
            else if (element is AdornerDecorator)
            {
                contentHost = (element as AdornerDecorator).Child;
            }

            if (DebugTools.AssertNotNull(contentHost, null).Failed)
            {
                return new Rect(0.0, 0.0, 0.0, 0.0);
            }

            Thickness pagePadding = Document.PagePadding;
            return new Rect(contentHost.TransformToAncestor(this).Transform(new Point(pagePadding.Left, pagePadding.Top)), new Size(ViewportWidth - pagePadding.Right - pagePadding.Left, ViewportHeight - pagePadding.Top - pagePadding.Bottom));
        }
    }

    public IEnumerable<TextPointer> Lines
    {
        get
        {
            for (TextPointer textPointer = GetFirstLine(Document); textPointer != null; textPointer = GetNextLine(textPointer))
            {
                yield return textPointer;
            }
        }
    }

    // Keep after the RichTextWithEmbeddedItem closure anchors; this field shifts later ordinals.
    private global::Quire.Controls.Core.IWhiteboardItem _savedActiveItem;

    public void Accept(ISpecialCopyVisitor visitor)
    {
        visitor.Visit(this);
    }

    internal static void InsertBaselineOffsetTag(double value, TextPointer insertionPoint)
    {
        Run run = new Run("baselineOffsetTag" + value.ToString(), insertionPoint);
        DocumentTextRange previousRange = new DocumentTextRange(insertionPoint.GetNextContextPosition(LogicalDirection.Backward), insertionPoint);
        FontWeight fontWeight = (FontWeight)previousRange.GetPropertyValue(TextElement.FontWeightProperty);
        DocumentTextRange runRange = new DocumentTextRange(run.ContentStart, run.ContentEnd);
        runRange.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight != FontWeights.Bold ? FontWeights.Bold : FontWeights.DemiBold);
    }

    public override string GetHelpContext() => base.GetHelpContext();

    public global::Quire.Core.ISnapToPixelHelper SnappingHelper => _snappingHelper ??= SnapToPixelWhiteboardHelper.Create(this);

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Text.RichTextWithEmbeddedItem+PropertyNames, Ptc.Controls.Text")]
    public static partial class PropertyNames
    {
    }
}
