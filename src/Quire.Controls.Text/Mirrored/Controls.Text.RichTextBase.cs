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
    // Compile-order layout follows Mathcad member ordinals; anchor offsets are recorded in the lane checkpoint.
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Text.RichTextBase+PropertyNames, Ptc.Controls.Text")]
    public static partial class PropertyNames
    {
        public const string CurrentTextFormat = "CurrentTextFormat";
    }

    public double[] CustomScriptsSizesForXamlSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool IsInPrinting => (bool)GetValue(IsInPrintingProperty);

    public bool IsInCopy => (bool)GetValue(IsInCopyProperty);

    public bool IsInSaving
    {
        get => _isInSaving;
        private set
        {
            if (_isInSaving == value)
            {
                return;
            }

            OnIsInSavingCLRPropertyChanged(value);
            _isInSaving = value;
        }
    }

    public bool WpfRecreateScripts
    {
        get => (bool)GetValue(WpfRecreateScriptsProperty);
        internal set
        {
            if (value && WpfRecreateScripts)
            {
                DebugTools.FailAssertion("check re-entrance");
            }

            SetValue(WpfRecreateScriptsProperty, value);
        }
    }

    private void SetIsInSerialization()
    {
        SetValue(IsInSerializationProperty, true);
    }

    private void ClearIsInSerialization()
    {
        ClearValue(IsInSerializationProperty);
    }

    public bool GetIsInSerialization()
    {
        return (bool)GetValue(IsInSerializationProperty);
    }

    public bool RevertCoerceAfterSerializationProcess => (bool)GetValue(RevertCoerceAfterSerializationProcessProperty);

    public void SetRevertCoerceAfterSerializationProcess()
    {
        SetValue(RevertCoerceAfterSerializationProcessProperty, true);
    }

    public void ClearRevertCoerceAfterSerializationProcess()
    {
        ClearValue(RevertCoerceAfterSerializationProcessProperty);
    }

    public ScriptState TextScriptState
    {
        get => (ScriptState)GetValue(TextScriptStateProperty);
        private set
        {
            SetValue(TextScriptStatePropertyKey, value);
            FirePropertyChanged(new PropertyChangedEventArgs("IsSubscript"));
            FirePropertyChanged(new PropertyChangedEventArgs("IsSuperscript"));
        }
    }

    public bool IsSubscript => TextScriptState == ScriptState.Subscript;

    public bool IsSuperscript => TextScriptState == ScriptState.Superscript;

    public bool IsOnDrop { get; set; }

    protected virtual bool SupportsLists => true;

    protected virtual bool SupportsIndentation => true;

    protected virtual bool SupportsSubscriptSuperscript => true;

    protected int PasteStartPositionOffset { get; private set; }

    protected BlockedSelectionDirection BlockedSelectDirection { get; private set; }

    internal static bool SpellCheckReady
    {
        get
        {
            foreach (global::System.Windows.Threading.DispatcherTimer timer in _pendingSpellCheckTimers.ToList())
            {
                if (DebugTools.DevAssert(timer.IsEnabled, null).Failed)
                {
                    _pendingSpellCheckTimers.Remove(timer);
                }
            }

            return _pendingSpellCheckTimers.IsNullOrEmpty();
        }
    }

    protected override void OnDrop(System.Windows.DragEventArgs dragEventArgs)
    {
        IsOnDrop = true;
        base.OnDrop(dragEventArgs);
        IsOnDrop = false;
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

    private static void ContextMenuOpened(object sender, global::System.Windows.RoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Text.RichTextBase+BlockedSelectionDirection, Ptc.Controls.Text")]
    [global::System.Flags]
    protected enum BlockedSelectionDirection
    {
        None = 0,
        Down = 1,
        Up = 2,
        Left = 4,
        Right = 8,
        Start = 16,
        End = 32,
        Backward = 22,
        Forward = 41,
        All = 63,
    }

    private static readonly global::System.Type _typeOfThis = typeof(RichTextBase);

    public static readonly global::System.Windows.RoutedEvent SpellCheckTimerStartEvent = EventManager.RegisterRoutedEvent("SpellCheckTimerStart", RoutingStrategy.Bubble, typeof(RoutedEventHandler), _typeOfThis);

    public static readonly global::System.Windows.Input.RoutedUICommand AddToDictionaryCommand = new RoutedUICommandWithFunctionalityKey("Add To Dictionary", "AddToDictionary", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)777);

    public static readonly global::System.Windows.Input.RoutedUICommand SetCurrentProofingLanguageCommand = new RoutedUICommandWithFunctionalityKey("Set Current Proofing Language", "SetCurrentProofingLanguage", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)778);

    public static readonly global::System.Windows.Input.RoutedUICommand ClearTextFormatCommand = new RoutedUICommandWithFunctionalityKey("Clear Text Format", "ClearTextFormat", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)785);

    public static readonly global::System.Windows.Input.RoutedUICommand ApplyTextStyleCommand = new RoutedUICommandWithFunctionalityKey("Apply Text Style", "ApplyTextStyle", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)786);

    public static readonly global::System.Windows.Input.RoutedUICommand TextStyleCreateFromCommand = new RoutedUICommandWithFunctionalityKey("Create Text Style from", "TextStyleCreateFrom", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)790);

    public static readonly global::System.Windows.Input.RoutedUICommand ToggleSubscriptCommand = new RoutedUICommandWithFunctionalityKey("Toggle Subscript", "ToggleSubscript", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)791);

    public static readonly global::System.Windows.Input.RoutedUICommand ToggleSuperscriptCommand = new RoutedUICommandWithFunctionalityKey("Toggle Superscript", "ToggleSuperscript", _typeOfThis, (Quire.FunctionalitiesLimitation.EnumFunctionalityKey)792);

    internal static readonly global::System.Windows.DependencyPropertyKey TextScriptStatePropertyKey = DependencyProperty.RegisterReadOnly("TextScriptState", typeof(ScriptState), _typeOfThis, new PropertyMetadata((object)ScriptState.None));

    public static readonly global::System.Windows.DependencyProperty TextScriptStateProperty = TextScriptStatePropertyKey.DependencyProperty;

    private static readonly global::System.Collections.Generic.HashSet<global::System.Windows.Threading.DispatcherTimer> _pendingSpellCheckTimers = new System.Collections.Generic.HashSet<System.Windows.Threading.DispatcherTimer>();

    private static readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Text.RichTextBase, int> ScopeRefCounts = new System.Collections.Generic.Dictionary<RichTextBase, int>();

    private static readonly global::System.Collections.Generic.List<global::Quire.Controls.Text.RichTextBase> ActiveScopes = new System.Collections.Generic.List<RichTextBase>();

    private static readonly global::System.Collections.Generic.Queue<global::Quire.Controls.Text.RichTextBase> SerializedObjectsToReleaseFromScope = new System.Collections.Generic.Queue<RichTextBase>();

    public static global::System.Func<global::System.Windows.Documents.Run, global::System.Windows.Documents.Run, bool> compareRuns = (Run run1, Run run2) => run1.FontFamily == run2.FontFamily && run1.FontSize == run2.FontSize && run1.FontWeight == run2.FontWeight && run1.Foreground == run2.Foreground && run1.Background == run2.Background && run1.Tag == run2.Tag && run1.TextDecorations?.Count == run2.TextDecorations?.Count;

    private const double FontSizeResetToCreateRun = 14.67d;

    public static readonly global::System.Windows.RoutedEvent ContentChangedEvent;

    public static readonly global::System.Windows.RoutedEvent CaretLeavesTextAreaEvent;

    public static readonly global::System.Windows.DependencyProperty IsInPrintingProperty;

    public static readonly global::System.Windows.DependencyProperty IsInCopyProperty;

    public static readonly global::System.Windows.DependencyProperty WpfRecreateScriptsProperty;

    public static readonly global::System.Windows.DependencyProperty IsInSerializationProperty;

    public static readonly global::System.Windows.DependencyProperty RevertCoerceAfterSerializationProcessProperty;

    private global::Quire.Core.TextFormat _textFormat;

    protected global::Quire.Core.TextFormat _textFormatInitialization;

    private bool _isEndOfLine;

    private global::System.Windows.IDataObject _pasteDataObject;

    // Compile-order device: placed after the placeholders that precede it in Mathcad so the closures are <.cctor>b__81_*.
    static RichTextBase()
    {
        HeightProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback(OnProhibitedPropertyChanged)));
        FontFamilyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback(OnInheritedOnlyPropertyChanged)));
        FontSizeProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback(OnInheritedOnlyPropertyChanged)));
        ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
        CaretLeavesTextAreaEvent = CommonRoutedEvents.CaretLeavesTextAreaEvent.AddOwner(_typeOfThis);
        WhiteboardDependencyProperties.IsSelectedProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback((sender, args) => ((RichTextBase)sender).IsSelectedChangedHandler())));
        IsInPrintingProperty = DocumentAttachedProperties.IsInPrintingProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback((obj, args) => ((RichTextBase)obj).OnIsInPrintingPropertyChanged(args))));
        IsInCopyProperty = DocumentAttachedProperties.IsInCopyProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback((obj, args) => ((RichTextBase)obj).OnIsInCopyPropertyChanged(args))));
        RevertCoerceAfterSerializationProcessProperty = CustomScriptRunBase.RevertCoerceAfterSerializationProcessProperty.AddOwner(_typeOfThis);
        IsInSerializationProperty = CustomScriptRunBase.IsInSerializationProperty.AddOwner(_typeOfThis);
        WpfRecreateScriptsProperty = CustomScriptRunBase.WpfRecreateScriptsProperty.AddOwner(_typeOfThis);
        AttachedProperties.IsOnVisiblePageProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(new PropertyChangedCallback((sender, args) => ((RichTextBase)sender).HandleIsOnVisiblePageChanged())));
        WorksheetPropertiesManager.RegisterToWorksheetPropertyNotifications(_typeOfThis);
        EventManager.RegisterClassHandler(typeof(ContextMenu), ContextMenu.OpenedEvent, new RoutedEventHandler(ContextMenuOpened));
        CommandManager.RegisterClassInputBinding(_typeOfThis, new InputBinding(new Quire.FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper(ToggleSubscriptCommand), Gestures.CtrlMinus));
        CommandManager.RegisterClassInputBinding(_typeOfThis, new InputBinding(new Quire.FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper(ToggleSubscriptCommand), Gestures.CtrlNumpadMinus));
        CommandManager.RegisterClassInputBinding(_typeOfThis, new InputBinding(new Quire.FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper(ToggleSuperscriptCommand), Gestures.CtrlPlus));
        CommandManager.RegisterClassInputBinding(_typeOfThis, new InputBinding(new Quire.FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper(ToggleSuperscriptCommand), Gestures.CtrlNumpadPlus));
    }

    protected virtual void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        HandleIsInPrintingChanged(args);
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

    protected virtual void OnIsInSavingCLRPropertyChanged(bool newValue)
    {
        WrapUndoAtSerialization(newValue);
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

    protected virtual void EndCommandScope()
    {
        int count;
        if (!ScopeRefCounts.TryGetValue(this, out count) || count <= 0)
        {
            DebugTools.FailAssertion((string)null);
            return;
        }
        count--;
        if (count > 0)
        {
            ScopeRefCounts[this] = count;
            return;
        }
        ScopeRefCounts.Remove(this);
        SerializedObjectsToReleaseFromScope.Enqueue(this);
        ClearIsInSerialization();
        ActiveScopes.Remove(this);
        if (ActiveScopes.Count != 0 || _isInCopyUndesiredCommandScope == null)
        {
            return;
        }
        foreach (RichTextBase richTextBase in SerializedObjectsToReleaseFromScope)
        {
            richTextBase.SetRevertCoerceAfterSerializationProcess();
        }
        try
        {
            while (CommandBuffer.CanUndo)
            {
                CommandBuffer.Undo();
            }
        }
        finally
        {
            foreach (RichTextBase richTextBase2 in SerializedObjectsToReleaseFromScope)
            {
                richTextBase2.ClearRevertCoerceAfterSerializationProcess();
            }
            SerializedObjectsToReleaseFromScope.Clear();
        }
        _isInCopyUndesiredCommandScope.Dispose();
        _isInCopyUndesiredCommandScope = null;
    }

    private global::System.Windows.Documents.TextPointer _lastCaretPosition;

    private global::System.Windows.Media.Brush _cachedDocumentHighlightColor;

    private bool _isInSaving;

    private readonly global::System.Windows.Threading.DispatcherTimer _spellCheckActivationTimer;

    public RichTextBase() { }

    private void InitializePasteCommand()
    {
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPasteExecuted, OnPasteCanExecute));
    }

    private void InitializeFeatureFlagCommandBindings()
    {
        if (!SupportsLists)
        {
            ExecutedRoutedEventHandler emptyHandler = (sender, e) => { };
            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleBullets, emptyHandler, (sender, e) =>
            {
                e.CanExecute = false;
                e.Handled = true;
            }));
            CommandBindings.Add(new CommandBinding(EditingCommands.ToggleNumbering, emptyHandler, (sender, e) =>
            {
                e.CanExecute = false;
                e.Handled = true;
            }));
        }
        if (!SupportsIndentation)
        {
            ExecutedRoutedEventHandler emptyHandler = (sender, e) => { };
            CommandBindings.Add(new CommandBinding(EditingCommands.IncreaseIndentation, emptyHandler, (sender, e) =>
            {
                e.CanExecute = false;
                e.Handled = true;
            }));
            CommandBindings.Add(new CommandBinding(EditingCommands.DecreaseIndentation, emptyHandler, (sender, e) =>
            {
                e.CanExecute = false;
                e.Handled = true;
            }));
        }
    }

    private void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        try
        {
            string xaml = ExtractXamlFromClipboard();
            if (string.IsNullOrEmpty(xaml))
            {
                Paste();
            }
            else
            {
                LoadXamlContent(xaml);
            }
            e.Handled = true;
        }
        catch (Exception ex)
        {
            DebugTools.LogError(ex.ToString());
        }
    }

    private void OnPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        IDataObject data = Clipboard.GetDataObject();
        e.CanExecute = data.GetDataPresent(DataFormats.Text) || data.GetDataPresent(DataFormats.Dib) || data.GetDataPresent(DataFormats.Xaml) || data.GetDataPresent(DataFormats.XamlPackage) || data.GetDataPresent(DataFormats.Rtf) || data.GetDataPresent(DataFormats.Bitmap);
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

    private void RaiseEvent(IDataObject data)
    {
        DataObjectPastingEventArgs args = new DataObjectPastingEventArgs(data, true, DataFormats.Xaml) { RoutedEvent = DataObject.PastingEvent };
        RaiseEvent(args);
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

    private void OnCorrectSpellingError(object sender, ExecutedRoutedEventArgs e)
    {
        string correctedWord = e.Parameter as string;
        if (!(e.OriginalSource.Equals(this) && IsActive && !correctedWord.IsNullOrEmpty()))
        {
            return;
        }
        DebugTools.DevAssert(!_spellingErrorRange.HasValue, null);
        _spellingErrorRange = null;
        DocumentTextRange wordRange = GetWordRangeAtCaretPosition(out string partialWord, (TextFromRunsStops)7, true);
        if (wordRange.IsNotEmpty() && wordRange.Start.Parent is CustomScriptRunBase)
        {
            int offset = Document.ContentStart.GetOffsetToPosition(wordRange.Start);
            _spellingErrorRange = (offset, correctedWord.Length);
        }
    }

    private void SpellingErrorCorrected()
    {
        if (!_spellingErrorRange.HasValue)
        {
            return;
        }
        var (start, end) = _spellingErrorRange.Value;
        _spellingErrorRange = null;
        TextPointer startPointer = Document.ContentStart.GetPositionAtOffset(start);
        TextPointer endPointer = startPointer?.GetPositionAtOffset(end);
        if (endPointer != null)
        {
            CustomScriptRunBase.RecoverSubSuperscriptRunsInRange(new DocumentTextRange(startPointer, endPointer));
            return;
        }
        DebugTools.FailAssertion((string)null);
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

    private static bool _contextMenuOpening;

    protected void SetStyleFromTextStyles(ICollection<TextStyleData> textStyles) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    protected virtual bool ApplyTextStyleCanExecute(object parameter)
    {
        if (IsActive || IsSelected)
        {
            return parameter is TextStyleData;
        }
        return false;
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

    private void HandleApplyTextStyle(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoApplyTextStyle_internal(global::Quire.TextStyleData styleData) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleClearTextFormat(object sender, ExecutedRoutedEventArgs e)
    {
        if (!ClearTextFormatCanExecute(e.Parameter))
        {
            return;
        }
        TextFormat textFormat = (TextFormat)e.Parameter;
        if (DebugTools.AssertNotNull(textFormat, null, Array.Empty<object>()).Failed)
        {
            return;
        }
        DocumentTextRange range;
        if (Selection.IsEmpty)
        {
            Hyperlink hyperlink = Selection.Start.GetTextElementFromTextPointer<Hyperlink>() ?? Selection.Start.GetAdjacentHyperlink(LogicalDirection.Forward) ?? Selection.Start.GetAdjacentHyperlink(LogicalDirection.Backward);
            range = hyperlink != null ? new DocumentTextRange(hyperlink.ContentStart, hyperlink.ContentEnd) : GetWordRangeAtCaretPosition(out string wordText, TextFromRunsStops.FindWordStrict, true);
        }
        else
        {
            range = Selection;
        }
        if (range != null)
        {
            CompoundCommand clearAllFormatProperties = new CompoundCommand();
            DelegateFunction0<bool> doMethod = () =>
            {
                using (new CommandScope(clearAllFormatProperties))
                {
                    if (IsSelected)
                    {
                        ClearSelectedRegionFormatProperties(textFormat);
                    }
                    else
                    {
                        ClearFormatPropertiesInRange(textFormat, range);
                    }
                }
                return true;
            };
            CommandBuffer.Execute(new UndoableCommand("Clear TextFormat", doMethod, () => clearAllFormatProperties.Undo()), null);
        }
    }

    private void ClearFormatPropertiesInRange(global::Quire.Core.TextFormat textFormat, global::System.Windows.Documents.TextRange range) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ClearSelectedRegionFormatProperties(TextFormat textFormat)
    {
        CommandBuffer.Execute(ToggleScriptUndoCommand.CreateToggleScriptUndoCommand(this, TextRange, ScriptState.None, false), null);
        Document.ApplyTextFormatValues(textFormat);
        using (DeclareChangeBlock())
        {
            Document.Blocks.Each(block => block.ClearTextFormatPropsRecursively((TextElement x) => true, TextFormat.ClearFormatPropsList));
        }
    }

    protected (ScriptState, double)? CurrentInsertionScriptState { get; set; }

    private void HandleToggleSubscriptCommand(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleToggleSuperscriptCommand(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleToggleSubSuperScriptCommand(global::System.Func<global::Quire.Controls.Text.ScriptState, global::Quire.Controls.Text.ScriptState> computeToggleToState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void ExecuteToggleScriptUndoCommand(DocumentTextRange toggleRange, ScriptState toggleToState)
    {
        CommandBuffer.Execute(ToggleScriptUndoCommand.CreateToggleScriptUndoCommand(this, toggleRange, toggleToState, true), null);
    }

    protected void ToggleRunElementAccordingToScriptState(global::System.Windows.Controls.TextChangedEventArgs argsChanges) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void RecalculateTextScriptState()
    {
        bool allRunsOfSameType = false;
        Run firstRun = Selection.EnumerateRunsInRange().FirstOrDefault();
        if (firstRun != null)
        {
            Type typeFirst = firstRun.GetType();
            allRunsOfSameType = Selection.EnumerateRunsInRange().All(run => run.GetType() == typeFirst);
        }
        ScriptState scriptState = CurrentInsertionScriptState?.Item1 ?? (allRunsOfSameType ? CustomScriptServiceFunctions.RunTypeToScriptState(firstRun) : default);
        if (TextScriptState != scriptState)
        {
            TextScriptState = scriptState;
        }
    }

    private void IsSelectedChangedHandler()
    {
        UpdateCurrentFormats();
    }

    protected virtual void UpdateCurrentFormats()
    {
        UpdateCurrentProofingLanguage(WhiteboardDependencyProperties.GetIsSelected(this) ? (DocumentTextRange)TextRange : Selection);
    }

    private void CanSetCurrentProofingLanguage(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = WhiteboardDependencyProperties.GetIsSelected(this) || !XmlLanguage.Empty.Equals(CurrentProofingLanguage);
    }

    // Compile-order device: moved behind ClearSelectedRegionFormatProperties so its closures keep Release ordinals 113-114.
    private void HandleSetCurrentProofingLanguage(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal TextRangeExtended GetWordRangeAtCaretPosition(out string wordText, TextFromRunsStops stopFlags, bool allowCursorAtWordBoundary)
    {
        wordText = null;
        TextPointer start = GetTextBoundary(LogicalDirection.Backward, out string partialStart, stopFlags);
        if (!allowCursorAtWordBoundary && partialStart.IsNullOrEmpty())
        {
            return null;
        }
        TextPointer end = GetTextBoundary(LogicalDirection.Forward, out string partialEnd, stopFlags);
        if (!allowCursorAtWordBoundary && partialEnd.IsNullOrEmpty())
        {
            return null;
        }
        wordText = partialStart + partialEnd;
        return new TextRangeExtended(start, end);
    }

    protected virtual void HandleIsVisibleChanged()
    {
        StartOrStopSpellCheckActivationTimer(CanEnableSpellCheck);
    }

    protected virtual void HandleIsOnVisiblePageChanged()
    {
        StartOrStopSpellCheckActivationTimer(CanEnableSpellCheck);
    }

    private void AttemptActivatingSpellCheck()
    {
        StartOrStopSpellCheckActivationTimer(false);
        if (CanEnableSpellCheck)
        {
            ApplySpellCheckValue(true);
        }
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

    public void DisableSpellCheck()
    {
        ApplySpellCheckValue(false);
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

    protected virtual bool CanEnableSpellCheck
    {
        get
        {
            if (IsVisible && AttachedProperties.GetIsOnVisiblePage(this))
            {
                return WorksheetPropertiesManager.GetWorksheetProperties(this).SpellCheckEnabled;
            }

            return false;
        }
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == SpellCheck.IsEnabledProperty)
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    public SpellingError GetSpellingErrorInPosition(ref TextPointer position)
    {
        SpellingError spellingError;
        GetSpellingErrorAtPosition(position, out spellingError);
        return spellingError;
    }

    private TextPointer GetSpellingErrorAtPosition(in TextPointer textPointer, out SpellingError spellingError)
    {
        spellingError = null;
        if (DebugTools.AssertNotNull(textPointer).Failed)
        {
            return null;
        }
        TextPointer position = null;
        try
        {
            spellingError = GetSpellingError(position = textPointer) ?? GetSpellingError(position = textPointer.GetReversedTextPointer());
        }
        catch
        {
            DebugTools.DevAssert(false);
        }
        if (spellingError != null)
        {
            return position;
        }
        return null;
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

    public bool CanAddToDictionary()
    {
        string word = GetWordToAddToDictionary();
        if (word != null)
        {
            return word.Length <= 128;
        }
        return false;
    }

    public string GetWordToAddToDictionary()
    {
        TextPointer pointer = GetPointerToAddDictionary();
        if (pointer == null)
        {
            return null;
        }

        string word = null;
        DocumentTextRange range = GetSpellingErrorRange(pointer);
        if (DebugTools.AssertNotNull(range, null).Passed)
        {
            GetTextBoundary(LogicalDirection.Forward, out word, DictionaryHelper.DictionaryWordStopFlags, range);
        }

        return word;
    }

    private TextPointer GetPointerToAddDictionary()
    {
        TextPointer pointer = null;
        if (SpellCheck.GetIsEnabled(this) && IsNotEmpty && Selection.IsEmpty)
        {
            SpellingError spellingError;
            pointer = GetSpellingErrorAtPosition(CaretPosition, out spellingError);
        }

        return pointer;
    }

    private void ShouldNoOpOnSelection(object sender, CanExecuteRoutedEventArgs args)
    {
        RoutedCommand command = args.Command as RoutedCommand;
        if (DebugTools.AssertNotNull(command).Failed)
        {
            return;
        }
        if (args.Command == EditingCommands.SelectDownByLine)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Down) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectDownByPage)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Down) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectDownByParagraph)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Down) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectRightByCharacter)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Right) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectRightByWord)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Right) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectToDocumentEnd)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.End) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectToLineEnd)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.End) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectUpByLine)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Up) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectUpByPage)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Up) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectUpByParagraph)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Up) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectLeftByCharacter)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Left) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectLeftByWord)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Left) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectToDocumentStart)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Start) != BlockedSelectionDirection.None;
            return;
        }
        if (args.Command == EditingCommands.SelectToLineStart)
        {
            args.CanExecute = (BlockedSelectDirection & BlockedSelectionDirection.Start) != BlockedSelectionDirection.None;
            return;
        }
        DebugTools.FailAssertion("Unexpected command: {0}", command.Name);
    }

    protected override void OnMouseMove(global::System.Windows.Input.MouseEventArgs args) => base.OnMouseMove(args);

    private void OnPreviewCopy(object sender, ExecutedRoutedEventArgs e)
    {
        if (!e.OriginalSource.Equals(this) || !IsActive)
        {
            return;
        }
        DocumentAttachedProperties.SetIsInCopy(this, true);
    }

    protected virtual void CopyingHandler(object sender, DataObjectCopyingEventArgs e)
    {
        if (!e.OriginalSource.Equals(this))
        {
            return;
        }
        if (Selection.IsNotEmpty())
        {
            e.DataObject.GetData(DataFormats.Rtf);
            SerializeRangeContentToRtfString(new DocumentTextRange(Selection.Start.GetPositionAtOffset(0), Selection.End.GetPositionAtOffset(0)));
            SerializeRangeContentToRtfString(TextRange);
        }
        DocumentAttachedProperties.ClearIsInCopy(this);
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

    protected virtual void PastingHandler(object sender, DataObjectPastingEventArgs e)
    {
        _pasteDataObject = e.DataObject;
        PrePasting();
    }

    public double? DequeueSplitFontSizeData()
    {
        if (_splitData.Count != 0)
        {
            return _splitData.Dequeue();
        }
        return null;
    }

    private void ClearSplitFontSizeDataQueue()
    {
        if (_splitData.Count > 0)
        {
            DebugTools.FailAssertion();
        }
        _splitData.Clear();
    }

    private void EnqueueSplitFontSizeData(double newValue)
    {
        _splitData.Enqueue(newValue);
        if (_splitData.Count > 2)
        {
            DebugTools.FailAssertion();
        }
    }

    protected virtual void PrePasting()
    {
        ClearInsertionPointsCache();
        PasteStartPositionOffset = Selection.IsEmpty ? CaretOffset : SelectionStart;
        ClearSplitFontSizeDataQueue();
        if (Selection.Start.Parent.Equals(Selection.End.Parent))
        {
            ICustomFontSizeOwner owner = Selection.Start.Parent as ICustomFontSizeOwner;
            if (owner != null)
            {
                Inline inline = Selection.Start.Parent as Inline;
                if (inline != null && Selection.Start.CompareTo(inline.ContentStart) != 0 && Selection.End.CompareTo(inline.ContentEnd) != 0)
                {
                    EnqueueSplitFontSizeData(owner.FontSizeForReport);
                }
            }
        }
        CurrentInsertionScriptState = null;
    }

    private int _textChangeCounter;

    private global::System.Nullable<global::System.ValueTuple<int, int>> _spellingErrorRange;

    protected global::Quire.Undo.CommandScope _pastingCommandScope;

    protected virtual void PostPasting()
    {
        TextRangeExtension.FixAndCleanImportedHyperlinksData(TextRange, DataObjectExtensions.HasTextWithEmbeddedItemsInfo(_pasteDataObject) ? false : FixHyperlinkEmptyRuns);
        global::System.Windows.Documents.TextRange range = new global::System.Windows.Documents.TextRange(GetPointerToInsertionPoint(PasteStartPositionOffset), CaretPosition);
        EnumerateEmbeddedObjectsInRange(range).OfType<Image>().Each(image =>
        {
            RemoveBaselineOffsetFromImage(image);
            CopyImageToTempBitmap(image);
        });
        CustomScriptRunBase.RecoverSubSuperscriptRunsInRange(range);
    }

    internal virtual void CopyImageToTempBitmap(Image image)
    {
        ImageUtils.CopyImageToTempBitmap(image);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        HandleSelectionChanged(null, null);
    }

    protected virtual void HandleSelectionChanged(object sender, EventArgs args)
    {
        ClearSplitFontSizeDataQueue();
        if (_pasteDataObject != null)
        {
            try
            {
                PostPasting();
            }
            finally
            {
                _pasteDataObject = null;
            }
        }
        UpdateCurrentRuns(TextRange, false, false);
        CacheScriptStateInRange(Selection);
        CurrentTextFormat = SelectionTextFormat;
        if (Selection.IsEmpty)
        {
            _lastCaretPosition = CaretPosition;
        }
        UpdateCurrentProofingLanguage(Selection);
        BlockedSelectDirection = BlockedSelectionDirection.None;
        AddBlockedSelectDirectionForNextElement(LogicalDirection.Backward);
        AddBlockedSelectDirectionForNextElement(LogicalDirection.Forward);
        CurrentInsertionScriptState = null;
        if (_reviveScriptAfterBackspaceData.HasValue)
        {
            CurrentInsertionScriptState = _reviveScriptAfterBackspaceData;
            TextPointer pointer = Selection.End.GetNextInsertionPosition(LogicalDirection.Forward);
            if (pointer == null || pointer.IsAtLineStartPosition)
            {
                Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline);
                Selection.ApplyPropertyValue(Control.FontSizeProperty, CurrentInsertionScriptState.Value.Item2);
            }
            _reviveScriptAfterBackspaceData = null;
        }
        RecalculateTextScriptState();
    }

    private void CacheScriptStateInRange(DocumentTextRange range)
    {
        _cacheSelectedScriptElementsOffsets = TextRangeExtension.GetRangeItemsOffsets<CustomScriptRunBase>(range);
    }

    private static global::Quire.Undo.CommandScope _isInCopyUndesiredCommandScope;

    private bool WasScriptElementDeletedOnChangeEvent(global::System.Windows.Controls.TextChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private readonly global::System.Collections.Generic.Queue<double> _splitData;

    protected void UpdateCurrentRuns(global::System.Windows.Documents.TextRange range, bool isNewRun = false, bool textRemoved = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void UpdateCurrentProofingLanguage(DocumentTextRange range)
    {
        CurrentProofingLanguage = ComputeProofingLanguage(range);
    }

    internal XmlLanguage ComputeProofingLanguage(global::System.Windows.Documents.TextRange range)
    {
        IEnumerable<Run> runs = range.EnumerateRunsInRange();
        if (runs.IsNotEmpty())
        {
            XmlLanguage language;
            if (!runs.Select(run => run.Language).HasCommonValue(out language))
            {
                return null;
            }
            return language;
        }
        if (WhiteboardDependencyProperties.GetIsSelected(this) || IsEmpty)
        {
            return Document.Language;
        }
        return XmlLanguage.Empty;
    }

    private void AddBlockedSelectDirectionForNextElement(LogicalDirection direction)
    {
        TextPointerContext context = CaretPosition.GetPointerContext(direction);
        if (context == TextPointerContext.EmbeddedElement)
        {
            if (DebugTools.DevAssert(CaretPosition.Parent is BlockUIContainer, null).Passed)
            {
                bool isBlocked;
                TextPointer leading;
                TextPointer trailing;
                BlockedSelectionDirection side;
                if (direction == LogicalDirection.Backward)
                {
                    leading = Selection.End;
                    trailing = Selection.Start;
                    side = (BlockedSelectionDirection)4;
                }
                else
                {
                    leading = Selection.Start;
                    trailing = Selection.End;
                    side = (BlockedSelectionDirection)8;
                }
                if (Selection.IsEmpty || CaretPosition.CompareTo(leading) != 0)
                {
                    isBlocked = true;
                }
                else
                {
                    TextPointer next = leading.GetNextContextPosition(direction);
                    isBlocked = next != null && next.CompareTo(trailing) == 0;
                }
                if (isBlocked)
                {
                    BlockedSelectDirection |= (BlockedSelectionDirection)63 & ~side;
                    return;
                }
                BlockedSelectDirection |= (BlockedSelectionDirection)63;
                return;
            }
        }
        else
        {
            TextPointer position = CaretPosition;
            while (position != null && (context == TextPointerContext.ElementStart || context == TextPointerContext.ElementEnd))
            {
                position = position.GetNextContextPosition(direction);
                context = position != null ? position.GetPointerContext(direction) : TextPointerContext.None;
            }
            if (position != null && position.Parent is BlockUIContainer && context == TextPointerContext.EmbeddedElement)
            {
                BlockedSelectDirection |= direction == LogicalDirection.Backward ? (BlockedSelectionDirection)22 : (BlockedSelectionDirection)41;
            }
        }
    }

    protected static void OnInheritedOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(d.ReadLocalValue(e.Property) == DependencyProperty.UnsetValue);
    }

    private static void OnProhibitedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(false);
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

    protected virtual global::Quire.Undo.IUndoableCommand CreateTextChangedCommand(global::System.Windows.Controls.TextChangedEventArgs args)
    {
        bool wasScriptElementDeleted = WasScriptElementDeletedOnChangeEvent(args);
        IUndoableCommand command = new SyncTextChangedCommand(this, _oldTextFormatStyleKey, _undoRunsTextStyleKey, _cachedDocumentHighlightColor, _redoTextFormatStyleKey, wasScriptElementDeleted);
        _undoRunsTextStyleKey = null;
        return command;
    }

    private global::System.Collections.Generic.List<global::System.ValueTuple<int, int>> _cacheSelectedScriptElementsOffsets;

    private void ExecuteTextChangedAndReactivateCommand(global::System.Windows.Controls.TextChangedEventArgs args)
    {
        bool isActiveItem = IsActive;
        IUndoableCommand textChangedCmd = CreateTextChangedCommand(args);
        if (GetIsInSerialization())
        {
            _textChangeCounter++;
        }
        CommandBuffer.Execute(new UndoableCommandWithSideEffects(
            "text changed and reactivate if needed",
            () =>
            {
                ClearTextFormatCache();
                return true;
            },
            () =>
            {
                textChangedCmd.Undo();
                Activate(isActiveItem);
                ClearTextFormatCache();
            },
            () =>
            {
                textChangedCmd.Redo();
                Activate(isActiveItem);
                ClearTextFormatCache();
            }), null);
    }

    private void ClearTextFormatCache()
    {
        _textFormat = null;
    }

    protected void ClearInsertionPointsCache()
    {
        _cachedInsertionPoints = null;
    }

    protected virtual void HandleTextChanged(object sender, TextChangedEventArgs args)
    {
        _cachedBaseline = null;
        ClearInsertionPointsCache();
        using (CommandScope commandScope = new CommandScope("RichTextBase.OnTextChanged"))
        {
            RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
            if (args.UndoAction == UndoAction.Create && _changeLevel == 0)
            {
                ExecuteTextChangedAndReactivateCommand(args);
            }
        }
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
            if (_spellingErrorRange.HasValue)
            {
                _changeLevel++;
                SpellingErrorCorrected();
                _changeLevel--;
            }
        }
    }

    private void OnPreviewBackspace(object sender, ExecutedRoutedEventArgs e)
    {
        Run run = null;
        Run run2 = Selection.Start.Parent as Run;
        int? num = run2?.ContentStart.GetOffsetToPosition(Selection.Start);
        if (Selection.IsEmpty && num == 1 || !Selection.IsEmpty && num == 0)
        {
            run = run2;
        }
        else if (Selection.IsEmpty && num == 0 && Selection.Start.GetInsertionPosition(LogicalDirection.Backward).Parent is Run run3 && new DocumentTextRange(run3.ContentStart, Selection.Start).Text.Trim(new char[] { '\r', '\n' }).Length == 1)
        {
            run = run3;
        }
        if (run != null)
        {
            double item = run is ICustomFontSizeOwner customFontSizeOwner ? customFontSizeOwner.FontSizeForReport : run.FontSize;
            _reviveScriptAfterBackspaceData = (CustomScriptServiceFunctions.RunTypeToScriptState(run), item);
        }
    }

    private void OnPreviewEnter(object sender, ExecutedRoutedEventArgs e)
    {
        if (object.Equals(sender, this))
        {
            CacheSubscriptStateOnTypeOver(false);
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

    private void CacheSubscriptStateOnTypeOver(bool immediateAction)
    {
        if (!Selection.IsEmpty)
        {
            CustomScriptRunBase run = Selection.Start.Parent as CustomScriptRunBase;
            if (run != null && run.ContentStart.CompareTo(Selection.Start) == 0)
            {
                ScriptState state = CustomScriptServiceFunctions.RunTypeToScriptState(run);
                if (immediateAction)
                {
                    CurrentInsertionScriptState = (state, CurrentTextFormat.FontSize);
                }
                else
                {
                    _reviveScriptAfterBackspaceData = (state, CurrentTextFormat.FontSize);
                }
            }
        }
    }

    protected void FireGotFocusEvent()
    {
        OnGotFocusEvent.Invoke(this);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        OnGotFocusEvent.Invoke(this);
    }

    public TextRangeExtended TextRange => new TextRangeExtended(Document.ContentStart, Document.ContentEnd);

    public string Text
    {
        get => GetTextInRange(TextRange);
        set => TextRange.Text = value;
    }

    public static string GetTextInRange(DocumentTextRange range)
    {
        return range.Text;
    }

    private void CacheTextFormat()
    {
        _textFormat = TextFormat.GetTextFormatForWholeDocument(Document);
    }

    protected int _changeLevel;

    private global::System.Nullable<global::System.ValueTuple<global::Quire.Controls.Text.ScriptState, double>> _reviveScriptAfterBackspaceData;

    public global::Quire.Core.TextFormat GetTextFormatForWholeDocument() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual int CaretOffset { get => GetInsertionPointAtPointer(CaretPosition); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::System.Collections.Generic.List<global::System.Windows.Documents.TextPointer> _cachedInsertionPoints;

    protected IEnumerable<TextPointer> InsertionPoints
    {
        get
        {
            if (_cachedInsertionPoints != null)
            {
                return _cachedInsertionPoints;
            }

            _cachedInsertionPoints = new List<TextPointer>();
            TextPointer position = Document.ContentStart;
            if (!position.IsAtInsertionPosition)
            {
                position = position.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            while (position != null)
            {
                _cachedInsertionPoints.Add(position.GetInsertionPosition(LogicalDirection.Forward));
                position = position.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            VerifyInsertionPositions();
            return _cachedInsertionPoints;
        }
    }

    protected virtual bool FixHyperlinkEmptyRuns => true;

    private void VerifyInsertionPositions()
    {
        if (!DebugTools.DebugVerificationsAllowed)
        {
            return;
        }
        TextPointer last = Document.ContentStart;
        foreach (TextPointer insertionPoint in _cachedInsertionPoints.Concat(Document.ContentEnd.Yield()))
        {
            TextPointer previous = last;
            TextPointer current = insertionPoint;
            if (last.GetOffsetToPosition(insertionPoint) > 1)
            {
                TextPointer position = previous.GetPositionAtOffset(1);
                while (position != null && position.CompareTo(current) < 0)
                {
                    if (position.IsAtInsertionPosition && position.GetInsertionPosition(LogicalDirection.Forward).CompareTo(current) < 0)
                    {
                        DebugTools.FailAssertion($"Unreachable Insertion Position at offset: {Document.ContentStart.GetOffsetToPosition(position)}.");
                        break;
                    }
                    position = position.GetPositionAtOffset(1);
                }
            }
            last = insertionPoint;
        }
    }

    public void SetCaretOffsetAtContentEnd()
    {
        CaretOffset = TextRange.Start.GetTextInRun(LogicalDirection.Forward).Length;
    }

    protected internal int GetInsertionPointAtPointer(in global::System.Windows.Documents.TextPointer pointer)
    {
        if (pointer == null)
        {
            return -1;
        }
        global::System.Windows.Documents.TextPointer insertionPosition = pointer.GetInsertionPosition(global::System.Windows.Documents.LogicalDirection.Forward);
        global::System.Collections.Generic.Comparer<global::System.Windows.Documents.TextPointer> comparer = global::System.Collections.Generic.Comparer<global::System.Windows.Documents.TextPointer>.Create((global::System.Windows.Documents.TextPointer ptr1, global::System.Windows.Documents.TextPointer ptr2) => ptr1.CompareTo(ptr2));
        return ((InsertionPoints as global::System.Collections.Generic.List<global::System.Windows.Documents.TextPointer>) ?? InsertionPoints.ToList()).BinarySearch(insertionPosition, comparer);
    }

    internal TextPointer GetPointerToInsertionPoint(int insertionPoint)
    {
        return InsertionPoints.ElementAtOrDefault(insertionPoint);
    }

    public int SelectionStart { get => GetInsertionPointAtPointer(Selection.Start); }

    public int SelectionEnd { get => GetInsertionPointAtPointer(Selection.End); }

    public virtual void SetSelection(int fromOffset, int toOffset)
    {
        Selection.Select(GetPointerToInsertionPoint(fromOffset), GetPointerToInsertionPoint(toOffset));
    }

    public virtual void SelectTo(int toOffset)
    {
        TextPointer anchor = Selection.IsEmpty ? CaretPosition : (CaretPosition.CompareTo(Selection.End) == 0 ? Selection.Start : Selection.End);
        Selection.Select(anchor, GetPointerToInsertionPoint(toOffset));
    }

    public IEnumerable<object> SelectedEmbeddedObjects => EnumerateEmbeddedObjectsInRange(Selection);

    public virtual global::System.Collections.Generic.IEnumerable<object> EnumerateEmbeddedObjectsInRange(global::System.Windows.Documents.TextRange range)
    {
        TextPointer curPos = range.Start;
        while (curPos is not null && curPos.CompareTo(range.End) < 0)
        {
            if (curPos.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement)
            {
                yield return curPos.GetAdjacentElement(LogicalDirection.Forward);
            }

            curPos = curPos.GetPositionAtOffset(1, LogicalDirection.Forward);
        }

        // Preserve Mathcad's iterator completion cleanup in Release IL.
        curPos = null;
    }

    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }

    public virtual bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        if ((options & FindOptions.FindInText) == 0)
        {
            return false;
        }
        TextPointer start = Document.ContentStart;
        TextPointer end = Document.ContentEnd;
        if (startFromCaret)
        {
            if (LogicalDirection.Forward == direction)
            {
                start = CaretPosition;
            }
            else
            {
                end = CaretPosition;
            }
        }
        return FindInRange(start, end, text, direction, options, subscriptPositions);
    }

    protected bool FindInRange(TextPointer start, TextPointer end, string text, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        DebugTools.DevAssert(options.HasFlag(FindOptions.FindInText));
        DebugTools.DevAssert(!(start.CompareTo(end) > 0));
        StringComparison comparisonType = options.HasFlag(FindOptions.MatchCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        DocumentTextRange range = new DocumentTextRange(start, end);
        string rangeText = GetTextInRange(range);
        DocumentTextRange match;
        bool isWholeSubscript = subscriptPositions?.Count == 1 && subscriptPositions[0].First == 0 && subscriptPositions[0].Second == text.Length - 1;
        if (options.HasFlag(FindOptions.WholeWordOnly) && (subscriptPositions.IsNullOrEmpty() | isWholeSubscript))
        {
            match = SearchWholeWordRuns(range, text, direction, comparisonType, options, subscriptPositions);
        }
        else if (direction == LogicalDirection.Forward)
        {
            match = SearchForward(range, rangeText, text, comparisonType, options, subscriptPositions);
        }
        else
        {
            match = SearchBackward(range, rangeText, text, comparisonType, options, subscriptPositions);
        }
        if (match != null)
        {
            SelectMatch(match.Start, match.End, direction);
            return true;
        }
        return false;
    }

    private void SelectMatch(TextPointer matchStart, TextPointer matchEnd, LogicalDirection direction)
    {
        Focus();
        if (LogicalDirection.Forward == direction)
        {
            Selection.Select(matchStart, matchEnd);
        }
        else
        {
            Selection.Select(matchEnd, matchStart);
        }
    }

    private DocumentTextRange SearchForward(DocumentTextRange originalRange, string rangeText, string text, StringComparison comparisonType, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        int searchStart = 0;
        while (searchStart < rangeText.Length)
        {
            int? index = FindUtilities.GetIndexInText(rangeText.Substring(searchStart), text, LogicalDirection.Forward, options);
            if (!index.HasValue)
            {
                break;
            }
            int start = searchStart + index.Value;
            DocumentTextRange match = GetRangeIfMatch(originalRange, text, start, comparisonType, subscriptPositions);
            if (match != null)
            {
                return match;
            }
            searchStart = start + 1;
        }
        return null;
    }

    private DocumentTextRange SearchBackward(DocumentTextRange originalRange, string rangeText, string text, StringComparison comparisonType, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        int length = rangeText.Length;
        while (length > 0)
        {
            int? index = FindUtilities.GetIndexInText(rangeText.Substring(0, length), text, LogicalDirection.Backward, options);
            if (!index.HasValue)
            {
                break;
            }
            DocumentTextRange match = GetRangeIfMatch(originalRange, text, index.Value, comparisonType, subscriptPositions);
            if (match != null)
            {
                return match;
            }
            length = index.Value;
        }
        return null;
    }

    private global::System.Windows.Documents.TextRange GetRangeIfMatch(global::System.Windows.Documents.TextRange originalRange, string text, int index, global::System.StringComparison comparisonType, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Windows.Documents.TextRange SearchWholeWordRuns(global::System.Windows.Documents.TextRange originalRange, string text, global::System.Windows.Documents.LogicalDirection direction, global::System.StringComparison comparisonType, global::Quire.Controls.Core.FindOptions options, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private List<(TextPointer, TextPointer)> GetSameTypeRunRanges(IReadOnlyList<Run> runs, DocumentTextRange originalRange)
    {
        var result = new List<(TextPointer, TextPointer)>();
        TextPointer runStart = originalRange.Start;
        for (int i = 1; i < runs.Count; i++)
        {
            if (runs[i].GetType() != runs[i - 1].GetType())
            {
                result.Add((runStart, runs[i - 1].ContentEnd));
                runStart = runs[i].ContentStart;
            }
        }
        result.Add((runStart, originalRange.End));
        return result;
    }

    private DocumentTextRange TryFindWholeWord(TextPointer start, TextPointer end, int originalRangeStartPoint, DocumentTextRange originalRange, string text, LogicalDirection direction, StringComparison comparisonType, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        int? index = FindUtilities.GetIndexInText(GetTextInRange(new DocumentTextRange(start, end)), text, direction, options);
        if (index.HasValue)
        {
            int offset = GetInsertionPointAtPointer(start) - originalRangeStartPoint;
            return GetRangeIfMatch(originalRange, text, offset + index.Value, comparisonType, subscriptPositions);
        }
        return null;
    }

    private bool SubscriptsMatch(global::System.Windows.Documents.TextRange foundRange, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> findTextSubscriptsList) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal static TextPointer GetPositionAtTextOffset(TextPointer from, int offset)
    {
        if (from == null || offset < 0)
        {
            DebugTools.DevAssert(false);
            return null;
        }
        TextPointer current = from;
        while (current != null && offset > 0)
        {
            if (TextPointerContext.Text == current.GetPointerContext(LogicalDirection.Forward))
            {
                int runLength = current.GetTextRunLength(LogicalDirection.Forward);
                int step = offset > runLength ? runLength : offset;
                current = current.GetPositionAtOffset(step);
                offset -= step;
            }
            else
            {
                TextPointer next = current;
                while (next != null && TextPointerContext.Text != next.GetPointerContext(LogicalDirection.Forward))
                {
                    next = next.GetNextContextPosition(LogicalDirection.Forward);
                }
                if (next != null)
                {
                    DocumentTextRange range = new DocumentTextRange(current, next);
                    offset -= range.Text.Length;
                }
                current = next;
            }
        }
        return current ?? from.DocumentEnd;
    }

    private void CanReplace(object sender, CanExecuteRoutedEventArgs e)
    {
        bool hasValue = (e.Parameter as FindReplaceQuery)?.ValueToReplaceWith != null;
        e.CanExecute = hasValue && !Selection.IsEmpty;
    }

    protected virtual void HandleReplace(object sender, ExecutedRoutedEventArgs e)
    {
        DebugTools.DevAssert(!Selection.IsEmpty, null);
        FindReplaceQuery query = e.Parameter as FindReplaceQuery;
        string valueToReplaceWith = query.ValueToReplaceWith;
        DebugTools.DevAssert(valueToReplaceWith != null, null);
        Focus();
        List<Run> runs = TextRangeExtension.EnumerateRunsInRange(Selection).ToList();
        DebugTools.DevAssert(runs.Count != 0, null);
        if (runs.Count == 0)
        {
            return;
        }
        TextFormat textFormat = new TextFormat(Selection, true);
        int selectionStart = SelectionStart;
        using (DeclareChangeBlock())
        {
            bool sameFindText = query.ValueToFind != null && query.ValueToFind.Equals(Selection.Text);
            if (sameFindText && IsInvalidOperation(query, runs))
            {
                return;
            }
            bool preserveScriptState = !query.IsFindContainSubscript && !query.IsReplaceContainSubscript;
            ScriptState scriptState = GetScriptType(runs[0]);
            bool shouldRestoreStyle = true;
            Hyperlink hyperlink = runs[0].GetTextElementParentOrSelf<Hyperlink>();
            if (hyperlink != null)
            {
                ReplaceTextInHyperlink(valueToReplaceWith, hyperlink, preserveScriptState, scriptState, query.ReplaceSubscriptPositions, selectionStart, out shouldRestoreStyle);
            }
            else
            {
                ReplaceText(valueToReplaceWith, preserveScriptState, scriptState, query.ReplaceSubscriptPositions, selectionStart);
            }
            RestoreSelection(valueToReplaceWith, selectionStart);
            if (shouldRestoreStyle)
            {
                textFormat.ApplyTextFormat(Selection);
            }
        }
    }

    private void ReplaceText(string text, bool preserveScriptState, ScriptState scriptState, List<Pair<int, int>> subscriptPositions, int selectionStartOffset)
    {
        if (preserveScriptState)
        {
            Selection.Text = text;
            CustomScriptRunBase.ToggleRangeText(Selection, scriptState, double.NaN);
            return;
        }
        Selection.Text = string.Empty;
        new Run(text, Selection.Start);
        foreach (Pair<int, int> pair in subscriptPositions)
        {
            ClearInsertionPointsCache();
            TextPointer start = GetPointerToInsertionPoint(selectionStartOffset);
            ToggleRangeToState(start, pair.First, pair.Second + 1, ScriptState.Subscript);
        }
    }

    private void ReplaceTextInHyperlink(string text, Hyperlink hyperlink, bool preserveScriptState, ScriptState scriptState, List<Pair<int, int>> subscriptPositions, int selectionStartOffset, out bool shouldRestoreStyle)
    {
        int originalLength = Selection.Text.Length;
        int? hyperlinkLength = hyperlink != null ? (int?)hyperlink.GetText().Length : null;
        ReplaceText(text, preserveScriptState, scriptState, subscriptPositions, selectionStartOffset);
        shouldRestoreStyle = (hyperlink != null ? (int?)hyperlink.GetText().Length : null) == hyperlinkLength - originalLength + text.Length && !(originalLength == hyperlinkLength);
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

    private bool IsInvalidOperation(global::Quire.Controls.Core.FindReplaceQuery query, global::System.Collections.Generic.List<global::System.Windows.Documents.Run> selectionRuns) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ToggleRangeToState(global::System.Windows.Documents.TextPointer startPointer, int startOffset, int endOffset, global::Quire.Controls.Text.ScriptState state) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void RestoreSelection(string text, int selectionStart)
    {
        ClearInsertionPointsCache();
        TextPointer start = GetPointerToInsertionPoint(selectionStart);
        TextPointer end = GetPositionAtTextOffset(start, text.Length);
        Selection.Select(start, end);
    }

    public virtual void SetFormattedContent(string content, string dataFormat, XmlLanguage proofingLanguage, TextFormat textFormat = default)
    {
        TextRangeExtended range = TextRange;
        _textFormatInitialization = textFormat;
        using (MemoryStream stream = new MemoryStream())
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(content ?? string.Empty);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                range.Load(stream, dataFormat);
                EnumerateEmbeddedObjectsInRange(range).OfType<global::System.Windows.Controls.Image>().Each(RemoveBaselineOffsetFromImage);
            }
        }

        range.FixAndCleanImportedHyperlinksData(FixHyperlinkEmptyRuns);
        if (DebugTools.AssertNotNull(proofingLanguage, null).Passed)
        {
            Document.Language = proofingLanguage;
        }

        UpdateCurrentProofingLanguage(Selection);
        UpdateCurrentRuns(Selection, true, false);
        CustomScriptRunBase.RecoverSubSuperscriptRunsInRange(TextRange);
    }

    protected static void RemoveBaselineOffsetFromImage(Image imageToClean)
    {
        imageToClean.ClearValue(TextBlock.BaselineOffsetProperty);
    }

    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    protected void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem> OnGotFocusEvent;

    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem, global::Quire.Direction> RequestRemove;

    private double Baseline
    {
        get
        {
            if (!_cachedBaseline.HasValue)
            {
                _cachedBaseline = CalculateBaseline();
            }

            return _cachedBaseline.Value;
        }
    }

    private double CalculateBaseline() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual double VerticalAlignmentOffset { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool IsNotEmpty => !IsEmpty;

    public bool IsEmpty
    {
        get
        {
            if (Document.Blocks.Count == 0)
            {
                return true;
            }

            if (Document.Blocks.Count > 1)
            {
                return false;
            }

            var paragraph = Document.Blocks.FirstBlock as System.Windows.Documents.Paragraph;
            if (paragraph == null)
            {
                return false;
            }

            if (paragraph.Inlines.Count == 0)
            {
                return true;
            }

            if (paragraph.Inlines.Count > 1)
            {
                return false;
            }

            var inline = paragraph.Inlines.FirstInline;
            return new DocumentTextRange(inline.ContentStart, inline.ContentEnd).IsEmpty;
        }
    }

    protected global::System.Nullable<double> _cachedBaseline;

    private double _cachedVerticalAlignmentOffset;

    public virtual void Activate(bool isActive)
    {
        if (isActive)
        {
            global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Controls.Core.IFrameworkElementFacade>().ExecuteOnLoad(this, () =>
            {
                Focus();
            });
        }
    }

    public Vector? NavigateIntoItem(Direction direction)
    {
        Activate(true);
        switch (direction)
        {
            case Direction.Left:
            case Direction.Up:
                SetCaretPositionAndClearSuggestedX(Document.ContentEnd);
                break;
            case Direction.Right:
            case Direction.Down:
                SetCaretPositionAndClearSuggestedX(Document.ContentStart);
                break;
        }
        return null;
    }

    public virtual void SimulateMouseClick(Point location, int numberOfClicks)
    {
        Activate(true);
    }

    public virtual TextFormat CurrentTextFormat
    {
        get => _currentTextFormat;
        set
        {
            // The IL branches to one shared return when the format is unchanged; an early return emits its own ret.
            if (_currentTextFormat != value)
            {
                if (value != null)
                {
                    _oldTextFormatStyleKey = value.StyleKey;
                    value.FormatChanged += HandleTextFormattingChanged;
                }

                if (_currentTextFormat != null)
                {
                    _oldTextFormatStyleKey = _currentTextFormat.StyleKey;
                    _currentTextFormat.FormatChanged -= HandleTextFormattingChanged;
                }

                _currentTextFormat = value;
                FirePropertyChanged(new PropertyChangedEventArgs("CurrentTextFormat"));
            }
        }
    }

    public bool IsActive => WhiteboardDependencyProperties.GetIsActive(this);

    public bool IsSelected => WhiteboardDependencyProperties.GetIsSelected(this);

    private void HandleTextFormattingChanged(DependencyPropertyChangedEventArgs arg)
    {
        DocumentTextRange range = Selection;
        if (Selection.IsEmpty && arg.Property == Control.FontSizeProperty)
        {
            range = GetWordRangeAtCaretPosition(out string _, TextFromRunsStops.FindWordStrict, true);
            if (range.IsEmpty)
            {
                range = Selection;
            }
        }
        CreateSplitFontSizeDataBeforeApplyPropOnRange(range);
        TextFormat.ApplyPropertyValueFormat(arg.Property, arg.NewValue, range);
        if (IsActive)
        {
            Focus();
        }
    }

    private void CreateSplitFontSizeDataBeforeApplyPropOnRange(DocumentTextRange range)
    {
        if (range.IsEmpty)
        {
            return;
        }
        ClearSplitFontSizeDataQueue();
        TextPointer start = range.Start;
        ICustomFontSizeOwner startOwner = start.Parent as ICustomFontSizeOwner;
        Inline startInline;
        if (startOwner != null)
        {
            startInline = start.Parent as Inline;
            if (startInline != null && startInline.ContentStart.CompareTo(start) < 0)
            {
                if (DependencyPropertyHelper.GetValueSource(startInline, Control.FontSizeProperty).BaseValueSource == BaseValueSource.Local)
                {
                    EnqueueSplitFontSizeData(startOwner.FontSizeForReport);
                }
            }
        }
        TextPointer end = range.End;
        ICustomFontSizeOwner endOwner = end.Parent as ICustomFontSizeOwner;
        Inline endInline;
        if (endOwner != null)
        {
            endInline = end.Parent as Inline;
            if (endInline != null && endInline.ContentEnd.CompareTo(end) > 0)
            {
                if (DependencyPropertyHelper.GetValueSource(endInline, Control.FontSizeProperty).BaseValueSource == BaseValueSource.Local)
                {
                    EnqueueSplitFontSizeData(endOwner.FontSizeForReport);
                }
            }
        }
    }

    protected TextFormat SelectionTextFormat => new TextFormat(CurrentTextFormat, Selection, false);

    public virtual IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        textType text = (textType)data;
        text.DeS11NProvider = d11NProvider;
        FlowDocument document = text.Document;
        if (document != null)
        {
            Document = document;
            TextRange.FixAndCleanImportedHyperlinksData(FixHyperlinkEmptyRuns);
        }
        return null;
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

    protected virtual textType DoGetRegionPersistentDataInternal(IS11NProvider s11NProvider)
    {
        return new textType
        {
            Document = Document,
            S11NProvider = s11NProvider,
        };
    }

    protected void SetCaretPositionAndClearSuggestedX(TextPointer caretPosition)
    {
        EditingCommands.MoveToDocumentStart.Execute(null, this);
        CaretPosition = caretPosition;
    }

    public void ApplyTextFormatPropertyValueToControl(DependencyProperty property, object value, Block block = null)
    {
        global::System.Windows.Documents.Paragraph paragraph = (block as List)?.ListItems.FirstListItem.Blocks.FirstBlock as global::System.Windows.Documents.Paragraph;
        TextFormat.SetTextFormat(property, paragraph?.Inlines.FirstInline != null ? paragraph.Inlines.FirstInline.GetValue(property) : value, Document);
    }

    private global::Quire.Core.TextFormat _currentTextFormat;

    private string _oldTextFormatStyleKey;

    private string _redoTextFormatStyleKey;

    private global::System.Collections.Generic.IList<string> _undoRunsTextStyleKey;

    public void SetTextFormatPropertyValue(global::System.Windows.DependencyProperty property, object value, bool isUndoRedo)
    {
        if (isUndoRedo)
        {
            return;
        }

        using (new CommandScope("SetTextFormatPropertyValue"))
        {
            TextFormat.ApplyPropertyValueFormat(property, value, TextFormat.TextRange(this));
            object oldValue = ((DependencyObject)Document).GetValue(property);
            object newValue = value;
            CommandBuffer.Execute(new UndoableCommandWithSideEffects("Update the property of the FlowDocument of this control", () =>
            {
                TextFormat.SetTextFormat(property, newValue, Document);
                ClearTextFormatCache();
                return true;
            }, () =>
            {
                TextFormat.SetTextFormat(property, oldValue, Document);
                ClearTextFormatCache();
            }, () =>
            {
                TextFormat.SetTextFormat(property, newValue, Document);
                ClearTextFormatCache();
            }));
        }
    }

    public virtual string GetHelpContext()
    {
        return RichTextBaseConstants.RichTextHelpContext;
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

    public virtual void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        if (((ReadOnlySpan<string>)propertyNames).Contains("SpellCheckEnabled"))
        {
            if (!newProperties.SpellCheckEnabled)
            {
                DisableSpellCheck();
                return;
            }

            if (CanEnableSpellCheck && !_spellCheckActivationTimer.IsEnabled)
            {
                StartOrStopSpellCheckActivationTimer(true);
            }
        }
    }

    public virtual void Dispose() { }

    public XmlLanguage CurrentProofingLanguage
    {
        get => _currentProofingLanguage ??= ComputeProofingLanguage(TextRange);
        private set
        {
            if (!Equals(value, _currentProofingLanguage))
            {
                _currentProofingLanguage = value;
                FirePropertyChanged(new PropertyChangedEventArgs(nameof(CurrentProofingLanguage)));
            }
        }
    }

    internal static global::System.Windows.Documents.TextPointer GetTextBoundary(global::System.Windows.Documents.LogicalDirection direction, out string partialText, global::Quire.Controls.Text.TextFromRunsStops stopFlags, global::System.Windows.Documents.TextRange range) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal TextPointer GetTextBoundary(LogicalDirection direction, out string partialText, TextFromRunsStops stopFlags = TextFromRunsStops.FindWord)
    {
        DocumentTextRange range = direction == LogicalDirection.Forward
            ? new DocumentTextRange(CaretPosition, Document.ContentEnd)
            : new DocumentTextRange(Document.ContentStart, CaretPosition);
        return GetTextBoundary(direction, out partialText, stopFlags, range);
    }

    public static string GetTextOnlyFromRuns(DocumentTextRange range)
    {
        string text;
        GetTextBoundary(LogicalDirection.Forward, out text, TextFromRunsStops.None, range);
        return text;
    }

    private void CanExecutePasteIntoTextRegion(object sender, CanExecuteRoutedEventArgs e)
    {
        bool canPaste = WpfClipboard.CanPasteIntoTextRegion();
        e.CanExecute = canPaste;
        e.Handled = !canPaste;
    }

    private global::System.Windows.Markup.XmlLanguage _currentProofingLanguage;

    public bool IsFullySubscript(global::System.Windows.Documents.TextRange range)
    {
        return range.EnumerateRunsInRange().All(run => run.GetType() == typeof(SubscriptRun));
    }

    protected void BuildSubscriptPositions(global::System.Windows.Documents.TextRange range, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions)
    {
        int startOffset = GetInsertionPointAtPointer(range.Start);
        int endOffset = GetInsertionPointAtPointer(range.End);
        foreach (SubscriptRun subscriptRun in range.EnumerateRunsInRange().OfType<SubscriptRun>())
        {
            int runStart = Math.Max(startOffset, GetInsertionPointAtPointer(subscriptRun.ContentStart)) - startOffset;
            int runEnd = Math.Min(endOffset, GetInsertionPointAtPointer(subscriptRun.ContentEnd)) - 1 - startOffset;
            int lastIndex = subscriptPositions.Count - 1;
            if (subscriptPositions.Count > 0 && runStart == subscriptPositions[lastIndex].Second + 1)
            {
                runStart = subscriptPositions[lastIndex].First;
                subscriptPositions.RemoveAt(lastIndex);
            }

            if (runStart <= runEnd)
            {
                subscriptPositions.Add(new global::Quire.Pair<int, int>(runStart, runEnd));
            }
        }
    }

}
