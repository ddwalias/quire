#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core.Serialization;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentTextRange = System.Windows.Documents.TextRange;
using System.Windows.Documents;
using System.IO;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Core;
using Quire.Undo;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class RichTextBase
{

    public virtual string GetHelpContext()
    {
        return RichTextBaseConstants.RichTextHelpContext;
    }

    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }

    internal virtual void CopyImageToTempBitmap(Image image)
    {
        ImageUtils.CopyImageToTempBitmap(image);
    }

    public static string GetTextInRange(DocumentTextRange range)
    {
        return range.Text;
    }

    private void IsSelectedChangedHandler()
    {
        UpdateCurrentFormats();
    }

    protected void ClearInsertionPointsCache()
    {
        _cachedInsertionPoints = null;
    }

    private void ClearIsInSerialization()
    {
        ClearValue(IsInSerializationProperty);
    }

    public void ClearRevertCoerceAfterSerializationProcess()
    {
        ClearValue(RevertCoerceAfterSerializationProcessProperty);
    }

    private void ClearTextFormatCache()
    {
        _textFormat = null;
    }

    public void DisableSpellCheck()
    {
        ApplySpellCheckValue(false);
    }

    protected virtual void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        HandleIsInPrintingChanged(args);
    }

    protected virtual void OnIsInSavingCLRPropertyChanged(bool newValue)
    {
        WrapUndoAtSerialization(newValue);
    }

    protected static void RemoveBaselineOffsetFromImage(Image imageToClean)
    {
        imageToClean.ClearValue(TextBlock.BaselineOffsetProperty);
    }

    public virtual void SimulateMouseClick(Point location, int numberOfClicks)
    {
        Activate(true);
    }

    private void CacheScriptStateInRange(DocumentTextRange range)
    {
        _cacheSelectedScriptElementsOffsets = TextRangeExtension.GetRangeItemsOffsets<CustomScriptRunBase>(range);
    }

    protected void FireGotFocusEvent()
    {
        OnGotFocusEvent.Invoke(this);
    }

    public bool GetIsInSerialization()
    {
        return (bool)GetValue(IsInSerializationProperty);
    }

    internal TextPointer GetPointerToInsertionPoint(int insertionPoint)
    {
        return InsertionPoints.ElementAtOrDefault(insertionPoint);
    }

    protected virtual void HandleIsOnVisiblePageChanged()
    {
        StartOrStopSpellCheckActivationTimer(CanEnableSpellCheck);
    }

    protected virtual void HandleIsVisibleChanged()
    {
        StartOrStopSpellCheckActivationTimer(CanEnableSpellCheck);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        HandleSelectionChanged(null, null);
    }

    private void SetIsInSerialization()
    {
        SetValue(IsInSerializationProperty, true);
    }

    public void SetRevertCoerceAfterSerializationProcess()
    {
        SetValue(RevertCoerceAfterSerializationProcessProperty, true);
    }

    protected virtual void OnIsInCopyPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        WrapUndoAtSerialization((bool)args.NewValue);
    }

    private void WrapUndoAtSerialization(bool bStartCommandScope)
    {
        if (bStartCommandScope)
        {
            StartCommandScope();
            return;
        }
        EndCommandScope();
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == SpellCheck.IsEnabledProperty)
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        OnGotFocusEvent.Invoke(this);
    }

    private void CacheTextFormat()
    {
        _textFormat = TextFormat.GetTextFormatForWholeDocument(Document);
    }

    public virtual void SetSelection(int fromOffset, int toOffset)
    {
        Selection.Select(GetPointerToInsertionPoint(fromOffset), GetPointerToInsertionPoint(toOffset));
    }

    protected override void OnDrop(System.Windows.DragEventArgs dragEventArgs)
    {
        IsOnDrop = true;
        base.OnDrop(dragEventArgs);
        IsOnDrop = false;
    }

    protected virtual void ExecuteToggleScriptUndoCommand(DocumentTextRange toggleRange, ScriptState toggleToState)
    {
        CommandBuffer.Execute(ToggleScriptUndoCommand.CreateToggleScriptUndoCommand(this, toggleRange, toggleToState, true), null);
    }

    protected virtual void PastingHandler(object sender, DataObjectPastingEventArgs e)
    {
        _pasteDataObject = e.DataObject;
        PrePasting();
    }

    private void AttemptActivatingSpellCheck()
    {
        StartOrStopSpellCheckActivationTimer(false);
        if (CanEnableSpellCheck)
        {
            ApplySpellCheckValue(true);
        }
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        if (EnumerableExtensions.IsNotEmpty(e.Text))
        {
            CacheSubscriptStateOnTypeOver(true);
        }
        base.OnPreviewTextInput(e);
    }

    public void Copy()
    {
        DocumentAttachedProperties.SetIsInCopy(this, true);
        try
        {
            base.Copy();
        }
        finally
        {
            DocumentAttachedProperties.ClearIsInCopy(this);
        }
    }

    protected void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected virtual textType DoGetRegionPersistentDataInternal(IS11NProvider s11NProvider)
    {
        return new textType
        {
            Document = Document,
            S11NProvider = s11NProvider,
        };
    }

    private static void OnProhibitedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(false);
    }

    protected void UpdateCurrentProofingLanguage(DocumentTextRange range)
    {
        CurrentProofingLanguage = ComputeProofingLanguage(range);
    }

    protected static void OnInheritedOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(d.ReadLocalValue(e.Property) == DependencyProperty.UnsetValue);
    }

    private void OnPreviewEnter(object sender, ExecutedRoutedEventArgs e)
    {
        if (object.Equals(sender, this))
        {
            CacheSubscriptStateOnTypeOver(false);
        }
    }

    private void CanExecutePasteIntoTextRegion(object sender, CanExecuteRoutedEventArgs e)
    {
        bool canPaste = WpfClipboard.CanPasteIntoTextRegion();
        e.CanExecute = canPaste;
        e.Handled = !canPaste;
    }

    private void LoadXamlContent(string pasteXaml)
    {
        try
        {
            TextRangeExtensionMethods.LoadElementFromClipboard(Selection, pasteXaml);
        }
        catch (Exception ex)
        {
            DebugTools.LogError(ex.ToString());
        }
    }

    protected virtual void UpdateCurrentFormats()
    {
        UpdateCurrentProofingLanguage(WhiteboardDependencyProperties.GetIsSelected(this) ? (DocumentTextRange)TextRange : Selection);
    }

    public SpellingError GetSpellingErrorInPosition(ref TextPointer position)
    {
        SpellingError spellingError;
        GetSpellingErrorAtPosition(ref position, out spellingError);
        return spellingError;
    }

    private void ClearSplitFontSizeDataQueue()
    {
        if (_splitData.Count > 0)
        {
            DebugTools.FailAssertion();
        }
        _splitData.Clear();
    }

    public void SetCaretOffsetAtContentEnd()
    {
        CaretOffset = TextRange.Start.GetTextInRun(LogicalDirection.Forward).Length;
    }

    protected void SetCaretPositionAndClearSuggestedX(TextPointer caretPosition)
    {
        EditingCommands.MoveToDocumentStart.Execute(null, this);
        CaretPosition = caretPosition;
    }

    protected virtual bool ApplyTextStyleCanExecute(object parameter)
    {
        if (IsActive || IsSelected)
        {
            return parameter is TextStyleData;
        }
        return false;
    }

    private bool ClearTextFormatCanExecute(object parameter)
    {
        if (!IsActive)
        {
            if (IsSelected)
            {
                return parameter is TextFormat;
            }
            return false;
        }
        return true;
    }

    public bool CanAddToDictionary()
    {
        string word = GetWordToAddToDictionary();
        if (word != null)
        {
            return word.Length <= 128;
        }
        return false;
    }

    public double? DequeueSplitFontSizeData()
    {
        if (_splitData.Count != 0)
        {
            return _splitData.Dequeue();
        }
        return null;
    }

    private void EnqueueSplitFontSizeData(double newValue)
    {
        _splitData.Enqueue(newValue);
        if (_splitData.Count > 2)
        {
            DebugTools.FailAssertion();
        }
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        IsInSaving = true;
        textType result;
        try
        {
            result = DoGetRegionPersistentDataInternal(s11NProvider);
        }
        finally
        {
            IsInSaving = false;
        }
        return result;
    }

    private void InitializePasteCommand()
    {
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPasteExecuted, OnPasteCanExecute));
    }

    private void CanSetCurrentProofingLanguage(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = WhiteboardDependencyProperties.GetIsSelected(this) || !XmlLanguage.Empty.Equals(CurrentProofingLanguage);
    }

    private ScriptState GetScriptType(Run run)
    {
        ScriptState scriptState = ScriptState.None;
        if (run is SubscriptRun)
        {
            scriptState = ScriptState.Subscript;
        }
        if (run is SuperscriptRun)
        {
            scriptState = ScriptState.Superscript;
        }
        return scriptState;
    }

    protected bool CaretShouldLeaveTextRegionInDirection(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection direction)
    {
        if (ShouldLeaveTextRegionOnCaretMove(direction))
        {
            RaiseEvent(new CommonRoutedEvents.CaretLeavesTextRegionEventArgs(CaretLeavesTextAreaEvent) { LeavingDirection = direction });
            return true;
        }
        return false;
    }

    protected virtual void OnStyleChanged(ICollection<TextStyleData> textStyles)
    {
        if (!IsVisible)
        {
            Canvas canvas = VisualParent as Canvas;
            if (canvas == null || canvas.IsVisible)
            {
                return;
            }
        }
        SetStyleFromTextStyles(textStyles);
    }

    private string ExtractXamlFromClipboard()
    {
        IDataObject dataObject = Clipboard.GetDataObject();
        if (dataObject == null || !dataObject.GetDataPresent(DataFormats.Xaml))
        {
            return null;
        }
        RaiseEvent(dataObject);
        return dataObject.GetData(DataFormats.Xaml) as string;
    }

    private void RestoreSelection(string text, int selectionStart)
    {
        ClearInsertionPointsCache();
        TextPointer start = GetPointerToInsertionPoint(selectionStart);
        TextPointer end = GetPositionAtTextOffset(start, text.Length);
        Selection.Select(start, end);
    }

    protected void CloneContent(RichTextBase original)
    {
        TextFormat.CopyTextFormat(original.Document, Document);
        WpfUtils.TextRangeCopy(original.TextRange, TextRange, DataFormats.XamlPackage);
        if (WhiteboardDependencyProperties.GetIsRegionBorderVisible(original))
        {
            WhiteboardDependencyProperties.SetIsRegionBorderVisible(this, true);
        }
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        DebugTools.DevAssert(!_contextMenuOpening);
        _contextMenuOpening = true;
        try
        {
            base.OnContextMenuOpening(e);
        }
        finally
        {
            _contextMenuOpening = false;
        }
    }

    private void RaiseEvent(IDataObject data)
    {
        DataObjectPastingEventArgs args = new DataObjectPastingEventArgs(data, true, DataFormats.Xaml) { RoutedEvent = DataObject.PastingEvent };
        RaiseEvent(args);
    }

    protected virtual void StartCommandScope()
    {
        int count;
        if (!ScopeRefCounts.TryGetValue(this, out count))
        {
            if (ActiveScopes.Count == 0)
            {
                _isInCopyUndesiredCommandScope = new CommandScope("drop undo side effects");
            }
            ScopeRefCounts[this] = 1;
            ActiveScopes.Add(this);
            SetIsInSerialization();
            return;
        }
        ScopeRefCounts[this] = count + 1;
    }

    protected virtual bool ToggleSubSuperscriptCommandCanExecute(object argsParameter)
    {
        if (SupportsSubscriptSuperscript && IsActive)
        {
            if (!Selection.IsEmpty)
            {
                return Selection.Text.Length > 0;
            }
            return true;
        }
        return false;
    }

    protected virtual bool ShouldLeaveTextRegionOnCaretMove(CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection direction)
    {
        if (direction != CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Up && direction != CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Down)
        {
            return false;
        }
        int lines;
        CaretPosition.GetLineStartPosition(Convert.ToInt32(direction), out lines);
        return lines == 0;
    }

    private void CanReplace(object sender, CanExecuteRoutedEventArgs e)
    {
        bool hasValue = (e.Parameter as FindReplaceQuery)?.ValueToReplaceWith != null;
        e.CanExecute = hasValue && !Selection.IsEmpty;
    }

    private void OnPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        IDataObject data = Clipboard.GetDataObject();
        e.CanExecute = data.GetDataPresent(DataFormats.Text) || data.GetDataPresent(DataFormats.Dib) || data.GetDataPresent(DataFormats.Xaml) || data.GetDataPresent(DataFormats.XamlPackage) || data.GetDataPresent(DataFormats.Rtf) || data.GetDataPresent(DataFormats.Bitmap);
    }

    private void StartOrStopSpellCheckActivationTimer(bool start)
    {
        if (start)
        {
            RoutedEventArgs args = new RoutedEventArgs(SpellCheckTimerStartEvent);
            RaiseEvent(args);
            if (args.Handled)
            {
                _spellCheckActivationTimer.Start();
                _pendingSpellCheckTimers.Add(_spellCheckActivationTimer);
            }
        }
        else
        {
            _spellCheckActivationTimer.Stop();
            _pendingSpellCheckTimers.Remove(_spellCheckActivationTimer);
        }
    }

    public void RefreshDictionary()
    {
        if (!SpellCheck.GetIsEnabled(this))
        {
            return;
        }
        if (DebugTools.DevAssert(File.Exists(DictionaryHelper.WpfSpellerCustomDictionaryUri.LocalPath)).Passed)
        {
            try
            {
                SpellCheck.CustomDictionaries.Add(DictionaryHelper.WpfSpellerCustomDictionaryUri);
                if (DictionaryHelper.IsWindowsSpellerCustomDictionaryValid)
                {
                    SpellCheck.CustomDictionaries.Remove(DictionaryHelper.WpfSpellerCustomDictionaryUri);
                }
            }
            catch (Exception ex)
            {
                DebugTools.FailAssertion(ex);
            }
        }
    }

    public static string SerializeRangeContentToRtfString(DocumentTextRange range)
    {
        string result;
        using (MemoryStream stream = new MemoryStream())
        {
            range.Save(stream, DataFormats.Rtf, true);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
        }
        return result;
    }


    private void ApplySpellCheckValue(bool value)
    {
        DebugTools.LogData("RichTextBase.ApplySpellCheckValue(" + value.ToString() + "): Enter");
        if (SpellCheck.GetIsEnabled(this) == value)
        {
            return;
        }
        try
        {
            if (!value)
            {
                SpellCheck.CustomDictionaries.Clear();
            }
            else
            {
                if (File.Exists(DictionaryHelper.WpfSpellerCustomDictionaryUri.LocalPath) && !DictionaryHelper.IsWindowsSpellerCustomDictionaryValid)
                {
                    SpellCheck.CustomDictionaries.Add(DictionaryHelper.WpfSpellerCustomDictionaryUri);
                }
            }
            SpellCheck.SetIsEnabled(this, value);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
        }
    }

    private void HandleIsInPrintingChanged(DependencyPropertyChangedEventArgs args)
    {
        if ((bool)args.NewValue)
        {
            SetIsInSerialization();
            _textChangeCounter = 0;
            return;
        }
        if (_textChangeCounter > 0)
        {
            using (CommandScope scope = new CommandScope(new CompoundCommand()))
            {
                SetRevertCoerceAfterSerializationProcess();
                while (_textChangeCounter > 0)
                {
                    base.Undo();
                    _textChangeCounter--;
                }
            }
            ClearRevertCoerceAfterSerializationProcess();
        }
        ClearIsInSerialization();
    }

    private void OnPreviewCopy(object sender, ExecutedRoutedEventArgs e)
    {
        if (!e.OriginalSource.Equals(this) || !IsActive)
        {
            return;
        }
        DocumentAttachedProperties.SetIsInCopy(this, true);
    }

    public void SaveWholeTextToRtf(Stream rtfStream)
    {
        try
        {
            StartCommandScope();
            TextRange.Save(rtfStream, DataFormats.Rtf, true, false);
        }
        finally
        {
            EndCommandScope();
        }
    }
}
