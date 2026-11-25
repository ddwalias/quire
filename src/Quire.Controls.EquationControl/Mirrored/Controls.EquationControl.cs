using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.EquationEditor.Equation;
using Quire.Undo;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Internal;
using Quire.Core;
using Quire.EquationEditor;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Transform;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls;

public partial class EquationControl
{
    private string _rl = string.Empty;
    private global::Quire.Calculation.Core.SynchronizationStatus _syncStatus;
    private readonly List<EquationControlChildCalculatable> _calculatableChildren = new List<EquationControlChildCalculatable>();
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> RequestEvaluation;
    public event global::System.Func<global::Quire.Calculation.Core.ICalculatable, string, global::Quire.Calculation.Core.IResultNode> GetValueOfIdentifierBefore;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> TreatAsUpToDate;
    private global::Quire.Calculation.Core.IResultNode _evaluationResult;

    public string Rl
    {
        get
        {
            if (AllowExpressionsOnly || !_isForEngineCalculation)
            {
                return MathNodeToRlTransformer.Default.SetRlidAndTransform(_model.EquationRoot);
            }

            return _rl;
        }
    }

    public void Commit() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetStatusSynchronized() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetSynchronizationStatus(global::Quire.Calculation.Core.SynchronizationStatus syncStatus) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetCalculatingStatus(global::Quire.Calculation.Core.CalculatingStatus calculatingStatus) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetResult(global::Quire.Calculation.Core.IResultNode result) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetScriptingEngineError(string error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool OverrideIdLabelByResolvedForDefinitionLhs
    {
        get => _model.OverrideIdLabelByResolvedForDefinitionLhs;
        set => _model.OverrideIdLabelByResolvedForDefinitionLhs = value;
    }

    public void SetIdentifierLabels(global::System.Collections.Generic.IDictionary<global::Quire.Calculation.Core.ResolvedId, string[]> idLabelMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetAutoSelectAlgorithm(global::System.Collections.Generic.IDictionary<int, string> autoAlgorithmMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetRedefinitionWarnings(global::System.Collections.Generic.IList<global::Quire.Calculation.Core.EngineRedefInfo> warnings) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;
    public IEnumerable<ICalculatable> CalculatableChildren => _calculatableChildren;
    private void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireCalculatableMoved(global::Quire.Calculation.Core.ICalculatable calc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public SynchronizationStatus SynchronizationStatus => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);
    public CalculatingStatus CalculatingStatus => (CalculatingStatus)GetValue(CalculatingStatusProperty);
    public bool IsCommitted => !_model.IsExpressionValueModified;
    public EngineError Error => _model.CalculationError;
    public IResultNode Result => null;
    public IList<EngineRedefInfo> EngineWarnings => _model.EngineWarnings;

    public int ErrorCount
    {
        get
        {
            if (Error == null)
            {
                return _model.ScriptingError?.ResourceString != null ? 1 : 0;
            }

            return 1;
        }
    }

    public string ErrorCode
    {
        get
        {
            if (_model.CalculationError != null && !string.IsNullOrEmpty(_model.CalculationError.ErrorCode))
            {
                return _model.CalculationError.ErrorCode;
            }

            return "";
        }
    }

    public string ErrorText
    {
        get
        {
            if (GetErrorDisplayText != null)
            {
                return string.Concat(GetErrorDisplayText(_model.CalculationError), _model.ScriptingError?.ResourceString);
            }

            return ErrorCode;
        }
    }

    public string ErrorSExpression
    {
        get
        {
            if (_model.ErrorNode != null)
            {
                return _model.ErrorNode.SExpression;
            }

            return "";
        }
    }

    public bool CanHideFromCalculationContext => true;
    public bool HideFromCalculationContext { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private void UpdateSynchronizationStatus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetMatrixOrigin(global::Quire.Calculation.Core.RealSimpleResultNode result = default) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateCalculatingStatus(global::Quire.Calculation.Core.CalculatingStatus calculatingStatus) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void UpdateEngineResult(SynchronizationStatus syncStatus)
    {
        var engineResult = EngineResult;
        EngineResult = new EngineResult(engineResult, syncStatus);
    }

    private void UpdateEngineResult(IResultNode result)
    {
        if (result is IEmptyResultNode && EngineResult.Errors != null && EngineResult.Errors.Count > 0)
        {
            return;
        }

        var engineResult = EngineResult;
        EngineResult = new EngineResult(engineResult, result);
    }

    private void UpdateEngineResult(EngineError error)
    {
        EngineError clone = null;
        if (error != null)
        {
            clone = error.Clone();
        }

        var engineResult = EngineResult;
        EngineResult = new EngineResult(engineResult, clone);
    }

    private void UpdateEngineResult(IEnumerable<EngineRedefInfo> warnings)
    {
        var engineResult = EngineResult;
        EngineResult = new EngineResult(engineResult, warnings);
    }

    private global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> UpdateAggregateFromModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public IDataExchangeModel DataExchangeModel => _model.DataExchangeModel;
    public int? ComponentHandle { get; set; }
    public int ComponentCookieMultiThreaded { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal readonly global::Quire.EquationEditor.IMathNodeToVisualNodeMap _map;
    internal readonly global::Quire.EquationEditor.IEquationModel _model;
    private const string FirstPropertyForXamlSerializationPropertyName = "FirstPropertyForXamlSerialization";
    internal readonly global::Quire.Controls.Internal.VisualMathRegionNode _view;
    public int IncrementComponentCookieMultiThreaded(bool isAutoCalc = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::Quire.Delegate1<global::Quire.Controls.Core.IEquationControl> ChangeToTextBox;
    private bool _isSymbolicEvalKeywordsShown = true;
    public const double RecommendedSymbolicResultsWidthInInches = 5d;
    public static readonly double RecommendedSymbolicResultsWidth;
    private readonly global::Quire.DelegateFunction1<string, global::Quire.Calculation.Core.EngineError> GetErrorDisplayText;

    public static bool WasSuspendedRegenerate
    {
        get => EquationPresenter.WasSuspendedRegenerate;
        set => EquationPresenter.WasSuspendedRegenerate = value;
    }

    public bool FirstPropertyForXamlSerialization
    {
        get
        {
            DebugTools.DevAssert(_isForEngineCalculation);
            return true;
        }
        set
        {
            InCustomInitialization = value;
            ReadDeserializationFormatsForPaste();
        }
    }

    public void HandleSizeChanged(global::System.Windows.Vector offset, bool changeWidth, bool changeHeight, bool resizeInProgress) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ReadDeserializationFormatsForPaste()
    {
        if (DebugTools.DevAssert(MathFormatForPaste.Instance != null && MathFormatForPaste.Instance.DocumentMathFormat != null && MathFormatForPaste.Instance.DocumentLabeledIdFormat != null).Failed)
        {
            return;
        }

        DeserializeMathFormat = MathFormatForPaste.Instance.DocumentMathFormat;
        DeserializeLabeledIdFormat = MathFormatForPaste.Instance.DocumentLabeledIdFormat;
    }

    protected override void OnTemplateChanged(global::System.Windows.Controls.ControlTemplate oldTemplate, global::System.Windows.Controls.ControlTemplate newTemplate) => base.OnTemplateChanged(oldTemplate, newTemplate);

    public string Algorithm
    {
        get => CurrentNode?.Algorithm;
        set => CurrentNode.Algorithm = value;
    }

    public AlgorithmOptions AlgorithmOptions => CurrentNode?.AlgorithmOptions;
    public void FireAlgorithmChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool IsSelected => WhiteboardDependencyProperties.GetIsSelected(this);

    public bool InCustomInitialization
    {
        get => _model.InCustomInitialization;
        set => _model.InCustomInitialization = value;
    }

    public bool SubstituteAndCommitInputValue(string sExpr, string valueSExpr, string scalingSExpr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool IsSuitableForInputTaggingPrivate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool IsSuitableForOutputTaggingPrivate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ActivateAndScopeIdentifierPrivate(string idSExpr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string GetDefinitionSExpression(string idSExpr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.Undo.UndoableCommand CreateToggleInputCommand(global::System.Action<global::Quire.Controls.Core.IORegions.IInputRegion, bool> toggleInputTag, bool setTag) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleInput(bool setTag, string sExpr, global::System.Action<global::Quire.Controls.Core.IORegions.IInputRegion, bool> toggleInputTag) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public double ResizeIndicatorWidth
    {
        get
        {
            var indicatorColumn = Template.FindName("PART_ResizeIndicatorColumn", this) as ColumnDefinition;
            if (indicatorColumn != null)
            {
                return indicatorColumn.ActualWidth;
            }

            return 0.0;
        }
    }

    public event Action<IWhiteboardItem, Direction> RequestRemove = delegate { };
    public event global::System.Action RootNodeChanged;
    public event global::System.Action MeasureOverideCompleted;
    public event Action<IWhiteboardItem> OnGotFocusEvent = delegate { };
    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem> OnLostFocusEvent;
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    public event global::System.Action<object, global::System.Windows.RoutedEventArgs> OnAdornedElementReady;
    public event global::System.Windows.RoutedEventHandler Changed;

    internal EquationControl(IEquationModel model)
    {
        ImeHandler.SetIsInputMethodSuspended(this);
        Focusable = true;
        if (CommandBuffer.CurrentInstance == null)
        {
            CommandBuffer.CurrentInstance = new CommandBuffer();
        }
        _model = model ?? new EquationModel();
        _model.Settings = MathcadSettings.Instance;
        GetErrorDisplayText = EngineErrorMessages.GetDisplayMessage;
        MathFormat = new MathFormat(this);
        VisualMathRegionNode visualMathRegionNode = new VisualMathRegionNode(new GestureManager(), GetErrorDisplayText);
        visualMathRegionNode.MathFormat.SetPropertiesFrom(MathFormat);
        visualMathRegionNode.SetValue(AttachedProperties.LabeledIdFormatProperty, GetValue(AttachedProperties.LabeledIdFormatProperty));
        _view = visualMathRegionNode;
        _view.MeasureOverideCompleted += FireMeasureOverideCompleted;
        _view.RequestChangeToTextBox += HandleRequestChangeToTextBox;
        _view.RootNodeChanged += OnRootNodeChanged;
        _view.RequestToAllowContextMenu += HandleRequestToAllowContextMenu;
        _view.VisibleWarningMessageChanged += OnVisibleWarningMessageChanged;
        _map = new MathNodeToVisualNodeMap(_view.VisualFactory, MathcadSettings.Instance);
        new EquationPresenter(_model, _view, _map);
        HookupToEvents();
        Content = _view;
        Cursor = Cursors.Arrow;
        BindCommands();
        _view.AddHandler(VisualComboBoxControlNode.RequestNavigateOutOfInputControlNodeEvent, new VisualComboBoxControlNode.RequestNavigateOutOfInputControlNodeHandler(HandleRequestNavigateOutOfInputControlNode));
        AddHandler(VisualComboBoxControlNode.RequestActivateEquationForComboBoxNodeEvent, new RoutedEventHandler(ActivateEquationForComboBoxNode));
        AddHandler(VisualInputScriptableControlNodeBase.RequestActivateEquationForComboBoxNodeEvent, new RoutedEventHandler(ActivateEquationForComboBoxNode));
        TabIndex = -1;
        ExpressionValueChanged += HandleExpressionValueChanged;
        AddHandler(CommonRoutedEvents.TraversedFocusEvent, new CustomRoutedEventHandler<FocusNavigationDirection>(OnTraversedFocusEvent), handledEventsToo: true);
        _view.Activated += () =>
        {
            OnViewOnActivated();
        };
        _view.Deactivated += OnViewDeActivated;
        SizeChanged += HandleSizeChanged;
        UpdateLhsVisibleOnView();
        ReApplyBackground();
        SetupInputGestures();
    }

    public static readonly global::System.Windows.RoutedEvent ContentChangedEvent;
    public static global::System.Windows.RoutedEvent ChangedEvent;
    private void SetupInputGestures() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnContextMenuClosing(global::System.Windows.Controls.ContextMenuEventArgs e) => base.OnContextMenuClosing(e);
    private void SetContextMenu(string contextMenuKey) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnContextMenuOpening(global::System.Windows.Controls.ContextMenuEventArgs e) => base.OnContextMenuOpening(e);
    private void HandleRequestChangeToTextBox() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool IsChangeToTextBoxReady() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool IsWithinSolveBlock { get; set; }
    private global::Quire.Controls.Core.NumberFormat _currentNumberFormat;
    private global::Quire.Controls.Core.MatrixFormat _currentMatrixFormat;
    private void ActivateEquationForComboBoxNode(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnViewDeActivated() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnViewOnActivated(bool skipAnimation = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleRequestToAllowContextMenu(bool allow) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleRequestNavigateOutOfInputControlNode(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.RoutedUICommand> args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireMeasureOverideCompleted() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnRootNodeChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireRootNodeChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    object global::System.ICloneable.Clone() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.Controls.EquationControl Clone() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnTraversedFocusEvent(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.FocusNavigationDirection> args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleExpressionValueChanged(global::Quire.Controls.Core.IEquationControl equation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public EquationControl() : this((Quire.EquationEditor.IEquationModel)null) { }
    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("Constructor body not mirrored yet; the this() chain keeps the field initializers out of this constructor.")]
    public EquationControl(string sExpression) : this() { }
    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("Constructor body not mirrored yet; the this() chain keeps the field initializers out of this constructor.")]
    public EquationControl(string sExpression, bool isForEngineCalculation) : this(sExpression) { }
    public static global::Quire.Controls.EquationControl CreateWithTextInput(string s) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public double TextBaselineY => _view.TextBaselineY;
    public bool AtLeftEdge => _model.AtLeftEdge();

    public double ActiveElementOpacity
    {
        get => (double)GetValue(ActiveElementOpacityProperty);
        set => SetValue(ActiveElementOpacityProperty, value);
    }

    private void OnViewVisualParentChanged(Quire.EquationEditor.View.IVisualMathRegionNode sender, DependencyObject oldParent, DependencyObject newParent)
    {
        if (newParent == null)
        {
            return;
        }

        if (OnAdornedElementReady != null)
        {
            OnAdornedElementReady(this, null);
        }
    }

    public bool IsAdornedElementReady
    {
        get
        {
            UIElement view = _view;
            if (view != null)
            {
                return WpfUtils.GetVisualParent(view) != null;
            }

            return false;
        }
    }

    public NumberFormat CurrentNumberFormat
    {
        get => _currentNumberFormat;
        set
        {
            _currentNumberFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentNumberFormat"));
        }
    }

    public MatrixFormat CurrentMatrixFormat
    {
        get => _currentMatrixFormat;
        set
        {
            _currentMatrixFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentMatrixFormat"));
        }
    }

    private bool IsResizingMatrix { get; set; }
    private global::System.Nullable<global::Quire.Controls.Core.IdLabels> _currentIdLabel;
    private global::System.Nullable<global::Quire.Controls.Core.IdLabels> _currentDisplayIdLabel;
    public void StartResizingMatrix(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void StopResizingMatrix() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public IdLabels? CurrentIdLabel
    {
        get => _currentIdLabel;
        set
        {
            _currentIdLabel = value;
            if (_currentIdLabel.GetValueOrDefault() == IdLabels.None)
            {
                _currentIdLabel = null;
            }

            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentIdLabel"));
        }
    }

    public IdLabels? CurrentDisplayIdLabel
    {
        get => _currentDisplayIdLabel;
        set
        {
            _currentDisplayIdLabel = value;
            if (_currentDisplayIdLabel.GetValueOrDefault() == IdLabels.None)
            {
                _currentDisplayIdLabel = null;
            }

            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CurrentDisplayIdLabel"));
        }
    }

    public double VerticalAlignmentOffset => -TextBaselineOffset;
    public bool IsEmpty => _model.IsEmpty;
    private event global::System.Action<global::Quire.Controls.Core.FocusChangeLevel> _postActivated;
    public void Activate(bool isActive) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SimulateMouseClick(global::System.Windows.Point location, int numberOfClicks) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool CanNavigate(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool CanReturn() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool CanTraverse(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsEmbedded
    {
        get => (bool)GetValue(IsEmbeddedProperty);
        set => SetValue(IsEmbeddedProperty, value);
    }

    public bool IsScriptableControl => _model.IsScriptableControl;
    protected override void OnGotFocus(global::System.Windows.RoutedEventArgs e) => base.OnGotFocus(e);
    private void FocusViewAndUpdateNavigatorStateIfNeeded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ViewOnIsVisibleChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnGotKeyboardFocus(global::System.Windows.Input.KeyboardFocusChangedEventArgs e) => base.OnGotKeyboardFocus(e);
    protected override void OnKeyDown(global::System.Windows.Input.KeyEventArgs e) => base.OnKeyDown(e);
    public global::System.Windows.Point GetImeWindowPosition() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnTextInput(global::System.Windows.Input.TextCompositionEventArgs e) => base.OnTextInput(e);
    protected void AnimateShow() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void AnimateHide() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal MathNode CurrentNode => _model.NavigatorState.Current;
    public EquationEditModeEnum Mode => _model.Mode;
    public override void EndInit() => base.EndInit();
    public EngineResult EngineResult { get; set; }
    public bool InterceptAndUpdateResult => false;
    public bool UseICalculatableToRestoreResult => false;
    public void RestoreResult() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::System.Action<string> SExpressionChanging;
    public event global::System.Action<global::Quire.Controls.Core.IEquationControl> ExpressionValueChanged;
    private readonly bool _isForEngineCalculation = true;
    private global::Quire.Controls.ControlAdorners.MatrixResizeAdorner _matrixResizeAdorner;
    private bool _temporaryResultMatrixFormatShouldClear;
    private global::System.Collections.Generic.IList<global::Quire.TableCoordinate> _nestedMatrixPositionWhichFormatShouldClear;
    public EquationTypes EquationType => _model.EquationType;
    public global::System.Windows.Media.ScaleTransform GetTransformBasedOnNumericNodeHeight(double desiredHeight) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireExpressionValueChanged()
    {
        if (ExpressionValueChanged != null)
        {
            ExpressionValueChanged(this);
        }
    }

    public void FireContentChangedEvents()
    {
        FireContentAndRegionChangedEvents(IORegionChangeType.Content);
    }

    private void FireContentAndRegionChangedEvents(IORegionChangeType type)
    {
        RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
        if (IORegionChanged != null)
        {
            IORegionChanged.Invoke(this, type);
        }
    }

    public void OnSExpressionChanging(string SExpression)
    {
        if (SExpressionChanging != null)
        {
            SExpressionChanging(SExpression);
        }
    }

    public string ResultScalingSExpression
    {
        get => _model.ResultScalingSExpression;
        set => _model.ResultScalingSExpression = value;
    }

    public string EquationSExpression { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public string EquationSExpressionWithResolvedLabels => _model.EquationSExpressionWithResolvedLabels;
    public string RegionType => "equation";

    public System.Xml.XmlNode EquationXmlNode
    {
        get => _model.EquationXmlNode;
        private set => _model.EquationXmlNode = value;
    }

    private void SetRegionResultFormat(global::System.Xml.XmlNode xmlElement, string ns) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CreateFakeEngineResults() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsMathFormatHandledLocally
    {
        get => _model.IsMathFormatHandledLocally;
        set => _model.IsMathFormatHandledLocally = value;
    }

    public bool IsExceedsWidthConstrain
    {
        get
        {
            double resizeIndicatorWidth = IsEmbedded ? ResizeIndicatorWidth : 0.0;
            return ViewFullWidth > ActualWidth && ViewClippedWidth < ViewFullWidth - resizeIndicatorWidth;
        }
    }

    public bool IsResizable
    {
        get
        {
            if (ContainsSymbolicEvaluation && SymbolicResultsWidth > RecommendedSymbolicResultsWidth)
            {
                return !IsEmbedded;
            }

            return false;
        }
    }

    public bool IsResizableInputControl
    {
        get => (bool)GetValue(IsResizableInputControlProperty);
        set => SetValue(IsResizableInputControlProperty, value);
    }

    public bool IsShrunk => IsResizable && IsExceedsWidthConstrain;

    public double SymbolicResultsWidth
    {
        get
        {
            if (!ContainsSymbolicEvaluation)
            {
                return 0.0;
            }

            var node = _model.EvaluationRoot;
            while (node[0].DescriptorName == "SYMBOLIC_EVALUATION")
            {
                node = node[0];
            }

            var symbolicNode = node[2];
            var visualNode = _map.AssociateNode(symbolicNode) as VisualMathNode;
            var origin = visualNode.TranslatePoint(new Point(0, 0), _view);
            return ViewFullWidth - origin.X;
        }
    }

    public bool ContainsSymbolicEvaluation
    {
        get
        {
            var root = _model.EvaluationRoot;
            if (root != null)
            {
                return root.DescriptorName == "SYMBOLIC_EVALUATION" || root[0].DescriptorName == "SYMBOLIC_EVALUATION";
            }

            return false;
        }
    }

    public void ExecuteOnRootLoaded(global::System.Action action) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteOnRootSizeChanged(global::System.Action action) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetRegionXmlNode(global::System.Xml.XmlNode value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Xml.XmlNode GetRegionXmlNode() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, object data) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string TypeoverSExpression => _model.TypeoverSExpression;
    public string[] History { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public int GetRegionHistoryCount() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void Restore(int historyIndex) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string CurrentNodeLabel
    {
        get
        {
            if (_model.Mode == EquationEditModeEnum.Normal && _model.NavigatorState.Current != null)
            {
                return _model.NavigatorState.Current.Label;
            }

            return null;
        }
    }

    public bool ErrorVisible
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public double TextBaselineOffset
    {
        get
        {
            if (FindCommonVisualAncestor(_view) == null)
            {
                return _view.TextBaselineY;
            }

            // Release IL builds the point in a local before the call; a plain new Point(...) argument does not match.
            Point point;
            return _view.TransformToVisual(this).Transform(point = new Point(0, _view.TextBaselineY)).Y;
        }
    }

    public void EditNode(string label) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void EditNode(string label, int caretPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void EditNodeOnCaretSide(string label, global::Quire.Direction caretSide) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void EditNodeOnNodeSide(string label, global::Quire.Direction nodeSide) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ScopeTo(string label, global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ScopeLhs() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void Cut() => _view.FireCut();
    public void Copy() => _view.FireCopy();
    public void Paste() => _view.FirePaste();
    public void Backspace() => _view.FireDeleteLeft();
    public void Delete() => _view.FireDeleteRight();
    public void ContractScope() => _view.FireContractScope();
    public void ExpandScope() => _view.FireExpandScope();
    public void ExpandScopeLeft() => _view.FireScopeLeft();
    public void ExpandScopeRight() => _view.FireScopeRight();
    public void ExitGroupOnRight() => _view.FireExitGroupRight();
    public void FullyExpandScope() => _view.FireFullyExpandScope();
    public void FullyContractScope() => _view.FireFullyContractScope();
    public void TransformCharacter() => _view.FireGreek();
    public void NavigateLeft() => _view.FireMoveLeft();
    public void NavigateLeftByTerm() => _view.FireMoveLeftByTerm();
    public void NavigateRight() => _view.FireMoveRight();
    public void NavigateRightByTerm() => _view.FireMoveRightByTerm();
    public void NavigateUp() => _view.FireMoveUp();
    public void NavigateDown() => _view.FireMoveDown();
    public void NavigateRegionEnd() => _view.FireMoveToRegionEnd();
    public void NavigateRegionStart() => _view.FireMoveToRegionStart();
    public void Pi() => _view.AutomateKeypress('π');
    public void Keypress(string s) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool AllowExpressionsOnly
    {
        get => _model.AllowExpressionsOnly;
        set => _model.AllowExpressionsOnly = value;
    }

    public bool ProhibitComboBox
    {
        get => _model.ProhibitComboBox;
        set => _model.ProhibitComboBox = value;
    }

    public bool ProhibitInputControl
    {
        get => _model.ProhibitInputControl;
        set => _model.ProhibitInputControl = value;
    }

    public void ScopeHome() => _view.FireMoveToStart();
    public void ScopeEnd() => _view.FireMoveToEnd();
    public int CurrentNodeCaretPosition => _model.CurrentModeNavigatorState.CaretPosition.GetValueOrDefault(-1);
    public CaretInsertionTypes CurrentNodeCaretInsertionType => _model.CurrentModeNavigatorState.CaretInsertionType;
    public string[] HighlightedOperands { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public bool CurrentNodeIsIdentifier => _model.CurrentModeNavigatorState.Current.DescriptorName == "IDENTIFIER";
    public bool CurrentNodeIsNumeric => _model.CurrentModeNavigatorState.Current.DescriptorName == "NUMERIC";
    public bool IsReadonly => _model.IsReadonly;
    public bool IsScoped => _model.Mode == EquationEditModeEnum.Scope;
    public bool IsTypeover => _model.Mode == EquationEditModeEnum.Typeover;
    public global::Quire.Direction CurrentNodeNodeSide { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public int CurrentNodeOperatorPartIndex { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.Direction CurrentNodeCaretSide { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public Direction ScopeDirection => _model.ScopeSide;

    public string SelectedOperator
    {
        get
        {
            var current = _model.CurrentModeNavigatorState.Current;
            if (current.NoLogicOnlyVisual)
            {
                current = current.Parent;
            }

            return current.OperatorSExpName;
        }
    }

    public string CurrentNodeSExpression { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.TableCoordinate ActiveMatrixCell { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public int MatrixRows { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public int MatrixColumns { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public void GetResultMatrixHeadingIndices(out string[] rowIndices, out string[] columnIndices) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetResultMatrixSize(int rows, int columns) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetResultMatrixOffset(int rows, int columns) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool IsResultMatrixFormatIndexVisible() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Windows.Rect GetLabeledScreenInkBounds(string nodeLabel) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HookupToEvents()
    {
        _model.RequestEvaluation += OnModelRequestEvaluation;
        _model.RequestRemoveEquation += FireRequestRemove;
        _model.RequestFocus += FocusWhenAble;
        _model.EquationChanged += OnModelEquationChanged;
        _model.RestrictedReadonlyChanged += HandleModelIsRestrictedReadonlyChanged;
        _model.NavigatorStateChanged += HandleNavigatorStateChanged;
        _model.ScopeChanged += HandleScopeChanged;
        _model.TypeoverChanged += HandleTypeoverChanged;
        _model.NumericResultChanged += HandleNumericResultChanged;
        _model.ExpressionValueChanged += FireExpressionValueChanged;
        _model.CalculationErrorChanged += HandleCalculationErrorChanged;
        _model.WarningsOccurred += HandleWarningOccured;
        _model.ContentChanged += FireContentChangedEvents;
        _view.VisualParentChanged += OnViewVisualParentChanged;
        _model.VisibleLhsOfDefinitionChanged += HandleVisibleLhsOfDefinitionChanged;
    }

    private void UnhookFromModelEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void DetachFromModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnModelEquationChanged(bool shouldRegenerate)
    {
        _model_EquationChanged();
    }

    private void OnModelRequestEvaluation(IEquationModel obj)
    {
        FireRequestEvaluation(true);
    }

    private void HandleCalculationErrorChanged(Quire.Wpf.PropertyChangedEventArgs<EngineError> eventArgs)
    {
        UpdateEngineResult(_model.CalculationError);
        FirePropertyChanged(ErrorChangedEventArgs.GetArgs());
        UpdateLhsVisibleOnView();
    }

    private void HandleWarningOccured() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnVisibleWarningMessageChanged() => UpdateLhsVisibleOnView();

    private void HandleTypeoverChanged(TypeoverChangeEventArgs arg)
    {
        CurrentIdLabel = null;
        CurrentDisplayIdLabel = null;
        if (!IsResizingMatrix)
        {
            if (CurrentNestedResultMatrixPosition != null)
            {
                CurrentNestedResultMatrixPosition = null;
            }

            MathNode commonAncestor = MathNode.Utilities.FindCommonAncestor(arg.State.LeftNode, arg.State.RightNode);
            CurrentMatrixFormat = GetFormatForMatrix(commonAncestor);
        }

        BringCurrentNodeIntoView(arg.State.NavState.Current);
    }

    private void HandleScopeChanged(global::Quire.EquationEditor.Equation.ScopeStateChangeEventArgs arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleNavigatorStateChanged(NavigatorStateChangeEventArgs arg)
    {
        NavigatorState navigatorState = arg.NavigatorState;
        if (navigatorState.Equals(NavigatorState.Empty))
        {
            CurrentIdLabel = null;
            CurrentDisplayIdLabel = null;
        }
        else
        {
            MathNode currentNode = arg.NavigatorState.Current;
            if (_model.ResolvedLabelOverridesIdLabel(currentNode))
            {
                CurrentIdLabel = IdLabels.None;
                CurrentDisplayIdLabel = ((IdentifierNodeInfo)currentNode.NodeInfo).ResolvedIdLabel;
            }
            else
            {
                CurrentIdLabel = MathNode.Utilities.IdLabelOfNode(currentNode);
                CurrentDisplayIdLabel = MathNode.Utilities.DisplayIdLabelOfNode(currentNode);
            }
        }

        if (!IsResizingMatrix)
        {
            IList<TableCoordinate> nestedPosition = GetNestedPositionForMatrix(arg.NavigatorState.Current);
            if (!AreTableCoordinateListsEqual(CurrentNestedResultMatrixPosition, nestedPosition))
            {
                CurrentNestedResultMatrixPosition = nestedPosition;
            }

            CurrentMatrixFormat = GetFormatForMatrix(arg.NavigatorState.Current);
        }

        BringCurrentNodeIntoView(arg.NavigatorState.Current);
        CurrentCaretInsertionType = arg.NavigatorState.CaretInsertionType;
        IncreaseEquationWidthIfNessessary(arg.NavigatorState);
    }

    private void HandleSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size constraint) => base.MeasureOverride(constraint);
    private void FixCaretToRightMostPositionAfterClipping() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void NavigateLeftIfNeccessary() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void IncreaseEquationWidthIfNessessary(global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private double XFromNavigatorState(global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Nullable<double> MidYFromCurrentMathNode(global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateWidth(bool widthNotSet) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal double ViewFullWidth => _view.ActualWidth;
    internal double ViewClippedWidth => _view.DesiredSize.Width;
    private void BringCurrentNodeIntoView(global::Quire.EquationEditor.Tree.MathNode currentNode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnRootLoaded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void _model_EquationChanged() => RaiseEvent(new RoutedEventArgs(ChangedEvent));
    private void RemoveMatrixResizeAdorner() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void RemoveMatrixResizeAdornerIfNotNeeded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::System.Action RequestFocus;
    private global::System.Windows.Input.CommandBindingCollection _nonEditableCommandBindings;
    private global::System.Windows.Input.CommandBindingCollection _editableCommandBindings;
    private void FocusWhenAble(global::Quire.EquationEditor.IEquationModel notUsed) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FirePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    internal void FireRequestEvaluation(bool force = true) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void FireRequestEvaluationControlOnly() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireRequestRemove(Direction direction)
    {
        RequestRemove.Invoke(this, direction);
    }

    public void InsertUnitMonomial(System.Collections.Generic.IEnumerable<Quire.Calculation.Core.Unit> unitMonomial) => _view.FireInsertUnitMonomial(unitMonomial);
    public override bool ShouldSerializeContent() => base.ShouldSerializeContent();
    protected override bool ShouldSerializeProperty(global::System.Windows.DependencyProperty dp) => base.ShouldSerializeProperty(dp);
    internal void SetNavigatorStateFromLabel(string label, global::Quire.Direction caretSide, global::Quire.Direction nodeSide) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void SetNavigatorStateFromLabel(string label, int caretPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void BindCommands()
    {
        CommandBindings.AddRange(NonEditableCommandBindings);
        if (!_model.IsRestrictedReadOnly)
        {
            BindEditingCommands();
        }
    }

    private CommandBindingCollection NonEditableCommandBindings
    {
        get
        {
            return _nonEditableCommandBindings ??= new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.Undo, UndoRedoHelper.HandleUndoCommand, UndoRedoHelper.CanExecuteUndoCommand),
                new CommandBinding(ApplicationCommands.Redo, UndoRedoHelper.HandleRedoCommand, UndoRedoHelper.CanExecuteRedoCommand),
                new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCopy),
                new CommandBinding(EquationCommands.NavigateLeft, OnNavigateLeft, CanNavigateLeft),
                new CommandBinding(EquationCommands.NavigateRight, OnNavigateRight, CanNavigateRight),
                new CommandBinding(EquationCommands.NavigateUp, OnNavigateUp, CanNavigateUp),
                new CommandBinding(EquationCommands.NavigateDown, OnNavigateDown, CanNavigateDown),
                new CommandBinding(EquationCommands.NavigateLeftByTerm, OnNavigateLeftByTerm, CanNavigateLeft),
                new CommandBinding(EquationCommands.NavigateRightByTerm, OnNavigateRightByTerm, CanNavigateRight),
                new CommandBinding(EquationCommands.NavigateToEquationStart, OnNavigateToEquationStart),
                new CommandBinding(EquationCommands.NavigateToEquationEnd, OnNavigateToEquationEnd),
                new CommandBinding(EquationCommands.NavigateToStartOfScope, OnNavigateToStartOfScope),
                new CommandBinding(EquationCommands.NavigateToEndOfScope, OnNavigateToEndOfScope),
                new CommandBinding(EquationCommands.NavigateToOppositeOfScope, OnNavigateToOppositeOfScope, CanNavigateToOppositeOfScope),
                new CommandBinding(EquationCommands.ScopeLeft, OnScopeLeft),
                new CommandBinding(EquationCommands.ScopeRight, OnScopeRight),
                new CommandBinding(EquationCommands.ExpandScope, OnExpandScope),
                new CommandBinding(EquationCommands.ContractScope, OnContractScope),
                new CommandBinding(EquationCommands.FullyExpandScope, OnFullyExpandScope),
                new CommandBinding(EquationCommands.FullyContractScope, OnFullyContractScope),
                new CommandBinding(EquationCommands.RegionHistory, OnRegionHistory),
                new CommandBinding(EquationCommands.Commit, OnCommit),
                new CommandBinding(EquationCommands.ToggleHideFromCalculationContext, OnToggleHideFromCalculationContext),
                new CommandBinding(EquationCommands.Traverse, OnTraverseForward, CanTraverseForward),
                new CommandBinding(EquationCommands.BackwardsTraverse, OnTraverseBackward, CanTraverseBackward),
                new CommandBinding(EquationCommands.ClearResultFormat, OnClearResultFormat, CanClearResultFormat),
                new CommandBinding(EquationCommands.SetNumberFormatFieldCommand, OnSetNumberFormatField),
                new CommandBinding(EquationCommands.SetMatrixFormatFieldCommand, OnSetMatrixFormatField),
                new CommandBinding(MathFormat.ClearMathFormatCommand, ExecuteClearMathFormat, CanClearMathFormat),
                new CommandBinding(MathFormat.IncreaseFontSizeCommand, ExecuteIncreaseFontSize),
                new CommandBinding(MathFormat.DecreaseFontSizeCommand, ExecuteDecreaseFontSize),
                new CommandBinding(MathFormat.ChangeFontColorCommand, ExecuteChangeFontColor),
                new CommandBinding(MathFormat.ChangeHighlightColorCommand, ExecuteChangeHighlightColor)
            };
        }
    }

    private CommandBindingCollection EditableCommandBindings
    {
        get
        {
            return _editableCommandBindings ??= new CommandBindingCollection
            {
            new CommandBinding(ApplicationCommands.Cut, OnCut, CanCut),
            new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste),
            new CommandBinding(EquationCommands.Backspace, OnBackspace),
            new CommandBinding(EquationCommands.Delete, OnDelete),
            new CommandBinding(EquationCommands.TransformCharacter, OnTransformCharacter),
            new CommandBinding(EquationCommands.ToggleSubscript, OnToggleSubscript),
            new CommandBinding(EquationCommands.ToggleVisibleLhsOfDefinition, OnToggleVisibleLhsOfDefinition, CanToggleVisibleLhsOfDefinition),
            new CommandBinding(EquationCommands.ConvertToOperator, OnConvertToOperator),
            new CommandBinding(EquationCommands.InsertOperator, OnInsertOperator),
            new CommandBinding(EquationCommands.SwitchLineBreak, OnSwitchToLineBreakOperator, CanSwitchToLineBreakOperator),
            new CommandBinding(EquationCommands.InsertEvaluation, OnInsertEvaluation),
            new CommandBinding(EquationCommands.InsertSymbolicEvaluation, OnInsertSymbolicEvaluation),
            new CommandBinding(EquationCommands.InsertValue, OnInsertValue),
            new CommandBinding(EquationCommands.InsertId, OnInsertId),
            new CommandBinding(EquationCommands.InsertString, OnInsertString),
            new CommandBinding(EquationCommands.InsertInputControl, OnInsertInputControl, CanInsertInputControl),
            new CommandBinding(EquationCommands.InsertMatrix, OnInsertMatrix),
            new CommandBinding(EquationCommands.InsertFunction, OnInsertFunction, CanInsertFunction),
            new CommandBinding(EquationCommands.InsertKeyword, OnInsertKeyword, CanInsertKeywordOrModifier),
            new CommandBinding(EquationCommands.InsertModifier, OnInsertModifier, CanInsertKeywordOrModifier),
            new CommandBinding(EquationCommands.InsertRow, OnInsertRow, CanPerformRowOperations),
            new CommandBinding(EquationCommands.InsertColumn, OnInsertMatrixColumn, CanPerformColumnOperations),
            new CommandBinding(EquationCommands.DeleteRow, OnDeleteRow, CanPerformRowOperations),
            new CommandBinding(EquationCommands.DeleteColumn, OnDeleteMatrixColumn, CanPerformColumnOperations),
            new CommandBinding(EquationCommands.ClearCells, OnClearCells, CanPerformCearCellsOperation),
            new CommandBinding(EquationCommands.SplitLinearNaryLine, OnSplitLinearNaryLine, CanPerformLinearNaryOperations),
            new CommandBinding(EquationCommands.InsertPi, OnInsertPi),
            new CommandBinding(EquationCommands.InsertInfinity, OnInsertInfinity),
            new CommandBinding(EquationCommands.SetIdLabel, OnSetIdLabel, CanPerformIdLabelOperations),
            new CommandBinding(EquationCommands.UnsetIdLabel, OnUnsetIdLabel, CanPerformIdLabelOperations),
            new CommandBinding(EquationCommands.ToggleIdLabel, OnToggleIdLabel, CanPerformIdLabelOperations),
            new CommandBinding(EquationCommands.ToggleUnitIdLabel, OnToggleUnitIdLabel, CanPerformIdLabelOperations),
            new CommandBinding(RegionCommands.Replace, OnReplace, CanReplace),
            new CommandBinding(EquationCommands.ChangeAlgorithm, ChangeAlgorithm),
            new CommandBinding(EquationCommands.ToggleAlgorithmOptionMultiStart, ToggleAlgorithmOption),
            new CommandBinding(EquationCommands.ToggleAlgorithmOptionOptimize, ToggleAlgorithmOption),
            new CommandBinding(EquationCommands.ToggleSymbolicEvalKeywordsVisibilityState, ToggleSymbolicKeywordsVisibility),
            };
        }
    }

    private void CanToggleVisibleLhsOfDefinition(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void BindEditingCommands() => CommandBindings.AddRange(EditableCommandBindings);

    private void UnBindEditingCommands()
    {
        foreach (System.Windows.Input.CommandBinding commandBinding in EditableCommandBindings)
        {
            CommandBindings.Remove(commandBinding);
        }
    }

    private void ChangeAlgorithm(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool IsAlgorithmSupportOptions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ResetAlgorithmOptionIfNeeded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleAlgorithmOption(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SymbolicEvalKeywordsStateUndoRedoSupport() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleSymbolicKeywordsVisibility(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateSymbolicEvalKeywordsVisibility(bool showHideKeywords) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SymbolicEvalKeywordsRegionActivatedEventHandler() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SymbolicEvalKeywordsRegionDeactivatedEventHandler() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleSymCalculationErrorChanged(global::Quire.Wpf.PropertyChangedEventArgs<global::Quire.Calculation.Core.EngineError> eventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool SymbolicEvalKeywordsVisibilityState { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private void ExecuteClearMathFormat(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteIncreaseFontSize(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteDecreaseFontSize(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteChangeFontColor(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ExecuteChangeHighlightColor(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanNavigateUp(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanNavigateDown(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanNavigateRight(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanNavigateLeft(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanClearResultFormat(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanClearMathFormat(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnSetNumberFormatField(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnSetMatrixFormatField(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnToggleUnitIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnToggleIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnUnsetIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnSetIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanPerformIdLabelOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformIdLabelOperations(sender, e);
    private void OnToggleHideFromCalculationContext(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertPi(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertInfinity(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnCopy(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnCut(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnPaste(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanCopy(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanCopy(sender, e);
    private void CanCut(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanCut(sender, e);
    private void CanPaste(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPaste(sender, e);
    private void OnBackspace(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnContractScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnDelete(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void PostDeleteNodeAction() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanReplace(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnReplace(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnExpandScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnFullyContractScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnFullyExpandScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertOperator(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnSwitchToLineBreakOperator(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs executedRoutedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanSwitchToLineBreakOperator(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertEvaluation(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertSymbolicEvaluation(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertValue(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertId(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertString(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OpenComboBoxEditingMode(global::Quire.EquationEditor.Tree.MathNode mathNode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void InputControlOnEquationActivated(object param) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertInputControl(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanInsertInputControl(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertMatrix(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertFunction(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanInsertFunction(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs eArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertKeyword(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanInsertKeywordOrModifier(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertModifier(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanPerformColumnOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformColumnOperations(sender, e);
    private void CanPerformCearCellsOperation(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertRow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertMatrixColumn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnDeleteRow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnDeleteMatrixColumn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnClearCells(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanPerformLinearNaryOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformLinearNaryOperations(sender, e);
    private void CanPerformRowOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformRowOperations(sender, e);
    private void OnSplitLinearNaryLine(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateUp(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateDown(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateLeft(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateLeftByTerm(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateRight(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateRightByTerm(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateToEndOfScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanNavigateToOppositeOfScope(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanNavigateToOppositeOfScope(sender, e);
    private void OnNavigateToOppositeOfScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateToEquationEnd(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateToEquationStart(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnNavigateToStartOfScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnRegionHistory(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnScopeLeft(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnScopeRight(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnTransformCharacter(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnToggleSubscript(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnToggleVisibleLhsOfDefinition(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool ShowLhsOfDefinitionForXamlSerialization
    {
        get => VisibleLhsOfDefinition == Visibility.Visible;
        set
        {
            if (!value)
            {
                _model.ToggleVisibleLhsOfDefinition(Visibility.Hidden);
            }
        }
    }

    public Visibility VisibleLhsOfDefinition => _model.VisibleLhsOfDefinition;

    public Visibility VisibleLhsOfDefinitionForXamlSerialization
    {
        get => VisibleLhsOfDefinition;
        set
        {
            if (VisibleLhsOfDefinition != value)
            {
                _model.ToggleVisibleLhsOfDefinition(value);
            }
        }
    }

    private void HandleVisibleLhsOfDefinitionChanged(Visibility visibility)
    {
        FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("VisibleLhsOfDefinition"));
        FireContentChangedEvents();
        UpdateLhsVisibleOnView();
    }

    private void UpdateLhsVisibleOnView() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnConvertToOperator(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnCommit(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanTraverseForward(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanTraverseBackward(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnTraverseForward(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) => OnTraverse(e, Direction.Right);
    private void OnTraverseBackward(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) => OnTraverse(e, Direction.Left);
    private void OnTraverse(System.Windows.Input.ExecutedRoutedEventArgs e, Direction direction) => _view.FireTraversalRequest(e, direction);
    protected void OnClearResultFormat(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.TypeCode GetTypeCode() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool ToBoolean(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public char ToChar(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public sbyte ToSByte(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public byte ToByte(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public short ToInt16(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public ushort ToUInt16(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public int ToInt32(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public uint ToUInt32(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public long ToInt64(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public ulong ToUInt64(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public float ToSingle(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public double ToDouble(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public decimal ToDecimal(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.DateTime ToDateTime(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string ToString(global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public object ToType(global::System.Type conversionType, global::System.IFormatProvider provider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ClearKeyStrokesCache() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ClearResultFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool HighlightAll(string text, global::Quire.Controls.Core.FindOptions options) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool FindOncePrivate(string text, bool startFromCaret, global::System.Windows.Documents.LogicalDirection direction, global::Quire.Controls.Core.FindOptions options, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual string GetHelpContext() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string GetCurrentNodeIdentifier() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IVisualComboBoxControlNode> GetAllComboBoxControlsFromEquation() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public int WarningsCount => _model.Warnings?.Count ?? 0;
    internal global::System.Collections.Generic.List<global::System.ValueTuple<global::Quire.EquationEditor.RedefinitionWarning, global::Quire.EquationEditor.Tree.MathNode, global::Quire.EquationEditor.View.IVisualMathNode>> Warnings { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal global::Quire.EquationEditor.View.IVisualMathNode CurrentVisualMathNode() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool ContainsError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ShowError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public EngineError CurrentError => Error;
    public bool IsMatrixResized { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public int[] VisibleMatrixCellRange { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private void HandleModelIsRestrictedReadonlyChanged(bool isRestrictedReadonly)
    {
        if (isRestrictedReadonly)
        {
            UnBindEditingCommands();
        }
        else
        {
            BindEditingCommands();
        }
    }

    public global::System.Collections.Generic.IList<string> GetDefinedIdentifierSExpressions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string GetEvalIdentifierSExpression() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::System.Action<global::Quire.Calculation.Core.IResultNode> ResultChanged;

    private void HandleNumericResultChanged(IResultNode resultValue)
    {
        UpdateEngineResult(resultValue);
        ResultChanged?.Invoke(resultValue);
    }

    public double GetRealResult() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public double GetRealUnitedResult(string unitsOfResult) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ActivateAndScopeResultPrivate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string GetEvaluationSExp(bool getDefineForDefineEval) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::System.Action<global::System.Windows.DependencyObject, global::Quire.Controls.Core.IORegions.IORegionChangeType> IORegionChanged;
    public global::System.Windows.FrameworkElement GetAdornedElement() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private MatrixFormat GetFormatForMatrix(MathNode node)
    {
        if (node != null && !node.IsInNumericPartOfEvaluationResult)
        {
            MathNode matrixNode = null;
            if (node.DescriptorName == "MATRIX" || node.DescriptorName == "ScriptableControl1")
            {
                matrixNode = node;
            }
            else
            {
                MathNode ancestor = node.GetNearestAncestor("MATRIX") ?? node.GetNearestAncestor("ScriptableControl1");
                if (ancestor != null)
                {
                    matrixNode = ancestor;
                }
            }

            if (matrixNode != null)
            {
                return ((MatrixNodeInfo)matrixNode.NodeInfo).MatrixFormat;
            }
        }

        MatrixFormat format = ResultMatrixFormat;
        IList<TableCoordinate> nestedPosition = GetNestedPositionForMatrix(node);
        if (nestedPosition == null)
        {
            return format;
        }

        foreach (TableCoordinate position in nestedPosition)
        {
            MatrixFormat parentFormat = format;
            format = parentFormat.NestedMatrixFormats[position];
            if (format == parentFormat.NestedMatrixFormats.EmptyPosition)
            {
                return parentFormat;
            }
        }

        return format;
    }

    internal global::Quire.Controls.Core.MatrixFormat GetFormatForVisualMatrix(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void SetMatrixForResizeAdorner(global::Quire.EquationEditor.View.IVisualMatrixNode matrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void UpdateMatrixForResizeAdorner(global::Quire.EquationEditor.View.IVisualMatrixNode matrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        private static bool AreTableCoordinateListsEqual(IList<TableCoordinate> oneNestedMatrixPosition, IList<TableCoordinate> theOtherNestedMatrixPosition)
    {
        bool areEqual = true;
        if (oneNestedMatrixPosition == null && theOtherNestedMatrixPosition == null)
        {
            return true;
        }

        if (oneNestedMatrixPosition == null || theOtherNestedMatrixPosition == null)
        {
            return false;
        }

        if (oneNestedMatrixPosition.Count != theOtherNestedMatrixPosition.Count)
        {
            areEqual = false;
        }
        else
        {
            for (int i = 0; i < theOtherNestedMatrixPosition.Count; i++)
            {
                if (!theOtherNestedMatrixPosition[i].Equals(oneNestedMatrixPosition[i]))
                {
                    areEqual = false;
                    break;
                }
            }
        }

        return areEqual;
    }

    internal global::System.Collections.Generic.IList<int> GetMatrixPathFromRoot(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private IList<TableCoordinate> GetNestedPositionForMatrix(MathNode node)
    {
        List<TableCoordinate> nestedPosition = null;
        if (node == null)
        {
            return nestedPosition;
        }

        if (_model.EvaluationRoot == null
            || _model.EvaluationRoot.DescriptorName != "EVALUATION"
            || _model.EvaluationRoot[1].DescriptorName != "RSCALE"
            || _model.EvaluationRoot[1][0].DescriptorName != "MATRIX")
        {
            return nestedPosition;
        }

        MathNode matrix = node.DescriptorName == "MATRIX" ? node : node.GetNearestAncestor("MATRIX");
        if (matrix == null)
        {
            return nestedPosition;
        }

        MathNode entry = _model.EvaluationRoot[1][0];
        if (matrix == entry)
        {
            return nestedPosition;
        }

        if (!matrix.IsDescendentOf(entry))
        {
            return nestedPosition;
        }

        nestedPosition = new List<TableCoordinate>();
        MathNode current = entry;
        do
        {
            MatrixNodeInfo info = current.NodeInfo as MatrixNodeInfo;
            int cellIndex = matrix.GetMatrixCellIndex(current);
            int row = info.ViewportOffset.Row + cellIndex / info.ViewportSize.Columns;
            int column = info.ViewportOffset.Column + cellIndex % info.ViewportSize.Columns;
            nestedPosition.Add(new TableCoordinate(row, column));
            current = current[cellIndex];
            if (current.DescriptorName == "SCALE" && current[0].DescriptorName == "MATRIX")
            {
                current = current[0];
            }
        }
        while (current != matrix);

        if (nestedPosition.Count != 0)
        {
            return nestedPosition;
        }

        return null;
    }

    internal global::System.Collections.Generic.IList<global::Quire.TableCoordinate> GetNestedPositionForVisualMatrix(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CreateOrClearTemporaryResultMatrixFormat(bool create) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ClearTemporaryResultMatrixFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CreateTemporaryResultMatrixFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetVisibleMatrixCellRange(int startRow, int startCol, int endRow, int endCol) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.Calculation.Core.EngineResult EngineResultForSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private static bool IsUnitedMatrixUniform(global::Quire.Calculation.Core.IUnitedValueResultNode unitedElement) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::Quire.Calculation.Core.IResultNode ConvertAnyFormattedMatrixToDisplayMatrix(global::Quire.Calculation.Core.IResultNode resultNode, global::Quire.Controls.Core.MatrixFormat matrixFormat, bool useEagerMatrixForSmallMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::Quire.Calculation.Core.IMatrixResultNode ConvertToDisplayMatrix(global::Quire.Calculation.Core.IMatrixResultNode originalMatrix, global::Quire.Controls.Core.MatrixFormat matrixFormat, bool useEagerMatrixForSmallMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::Quire.Calculation.Core.IResultNode GetDisplayMatrixCell(global::Quire.Calculation.Core.IMatrixResultNode matrix, global::Quire.Controls.Core.MatrixFormat matrixFormat, int r, int c, bool useEagerMatrixForSmallMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::Quire.Calculation.Core.IMatrixResultNode ConvertToEagerMatrix(global::Quire.Calculation.Core.IMatrixResultNode originalMatrix, global::Quire.Controls.Core.MatrixFormat matrixFormat, bool useEagerMatrixForSmallMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void HandleMathFormatChanged(global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ReApplyBackground() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public MathFormat MathFormat { get; private set; }

    public StaticMathFormat LocalMathFormatForXamlSerialization
    {
        get => MathFormat.ToStaticMathFormat();
        set => MathFormat.FromStaticMathFormat(value);
    }

    public MathFormat CurrentMathFormat
    {
        get => MathFormat;
        set => throw new NotImplementedException();
    }

    public IMathFormat DeserializeMathFormat
    {
        set
        {
            if (_view == null || value == null)
            {
                return;
            }

            _view.MathFormat.SetPropertiesFrom(value, true);
        }
    }

    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat
    {
        set
        {
            DependencyObject view = _view;
            if (view == null || value == null)
            {
                return;
            }

            view.SetValue(AttachedProperties.LabeledIdFormatProperty, value);
        }
    }

    private void ClearExpressionValueModified()
    {
        _model.IsExpressionValueModified = false;
        FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsCommitted"));
    }

    public bool AddToDataObject(global::System.Windows.IDataObject dataObject) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void Accept(global::Quire.ISpecialCopyVisitor visitor) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public string GetEquationSizesAsString_ForTestsOnly(int numDecimalPlaces) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Collections.Generic.List<global::System.Windows.Size> GetEquationSizes_ForTestsOnly() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static string CollectSizes_ForTestsOnly(global::Quire.Controls.Internal.VisualMathNode node, global::System.Collections.Generic.List<global::System.Windows.Size> output) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public IBookmarksManager BookmarksManager { get; set; }
    private static readonly global::System.Type _typeOfThis;
    public static readonly global::System.Windows.DependencyProperty SynchronizationStatusProperty;
    public static readonly global::System.Windows.DependencyProperty CalculatingStatusProperty;
    public static readonly global::System.Windows.DependencyProperty ActiveElementOpacityProperty;
    public static readonly global::System.Windows.DependencyProperty ActiveNonNavigatableElementOpacityProperty;
    public static readonly global::System.Windows.DependencyProperty IsEmbeddedProperty;
    public static readonly global::System.Windows.DependencyProperty IsResizableInputControlProperty;
    public static readonly global::System.Windows.DependencyProperty CalcIsDisabledProperty;
    public static readonly global::System.Windows.DependencyProperty ResultMatrixFormatProperty;
    public static readonly global::System.Windows.DependencyProperty DisplayNumberFormatProperty;
    public static readonly global::System.Windows.DependencyProperty CurrentCaretInsertionTypeProperty;
    private static readonly global::System.Windows.DependencyProperty CurrentNestedResultMatrixPositionProperty;
    public static readonly global::System.Windows.DependencyProperty RootPaddingProperty;
    private static string _unitLabel;
    private static void HandleCalcIsDisabledChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public MatrixFormat ResultMatrixFormat
    {
        get => (MatrixFormat)GetValue(ResultMatrixFormatProperty);
        set => SetValue(ResultMatrixFormatProperty, value);
    }

    public NumberFormat DisplayNumberFormat
    {
        get => (NumberFormat)GetValue(DisplayNumberFormatProperty);
        set => SetValue(DisplayNumberFormatProperty, value);
    }

    public CaretInsertionTypes CurrentCaretInsertionType
    {
        get => (CaretInsertionTypes)GetValue(CurrentCaretInsertionTypeProperty);
        set => SetValue(CurrentCaretInsertionTypeProperty, value);
    }

    private IList<TableCoordinate> CurrentNestedResultMatrixPosition
    {
        get => (IList<TableCoordinate>)GetValue(CurrentNestedResultMatrixPositionProperty);
        set => SetValue(CurrentNestedResultMatrixPositionProperty, value);
    }

    public static bool GetOverrideLabelColor(global::System.Windows.DependencyObject obj) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void SetOverrideLabelColor(global::System.Windows.DependencyObject obj, bool val) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void ClearOverrideLabelColor(global::System.Windows.DependencyObject obj) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static global::System.Windows.Thickness GetRootPadding(global::System.Windows.DependencyObject obj) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void SetRootPadding(global::System.Windows.DependencyObject obj, object val) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    static EquationControl()
    {
        RecommendedSymbolicResultsWidth = WpfUtils.InchesToDips(5.0);
        ChangedEvent = EventManager.RegisterRoutedEvent("Changed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EquationControl));
        _typeOfThis = typeof(EquationControl);
        // Release IL uses the original enum values, which differ from the legacy enum declarations.
        SynchronizationStatusProperty = DependencyProperty.Register("SynchronizationStatus", typeof(SynchronizationStatus), _typeOfThis,
            new FrameworkPropertyMetadata((SynchronizationStatus)3, FrameworkPropertyMetadataOptions.Inherits, HandleSynchronizationStatusChanged));
        CalculatingStatusProperty = DependencyProperty.Register("CalculatingStatus", typeof(CalculatingStatus), _typeOfThis,
            new FrameworkPropertyMetadata((CalculatingStatus)2, FrameworkPropertyMetadataOptions.Inherits, HandleCalculatingStatusChanged));
        ActiveElementOpacityProperty = DependencyProperty.Register("ActiveElementOpacity", typeof(double), typeof(FrameworkElement),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        ActiveNonNavigatableElementOpacityProperty = DependencyProperty.Register("ActiveNonNavigatableElementOpacity", typeof(double), typeof(FrameworkElement),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        IsEmbeddedProperty = DependencyProperty.Register("IsEmbedded", typeof(bool), _typeOfThis);
        IsResizableInputControlProperty = DependencyProperty.Register("IsResizableInputControl", typeof(bool), _typeOfThis,
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        CurrentCaretInsertionTypeProperty = DependencyProperty.Register("CurrentCaretInsertionType", typeof(CaretInsertionTypes), _typeOfThis,
            new FrameworkPropertyMetadata(CaretInsertionTypes.Normal));
        CurrentNestedResultMatrixPositionProperty = DependencyProperty.Register("CurrentNestedResultMatrixPosition", typeof(IList<TableCoordinate>), _typeOfThis,
            new FrameworkPropertyMetadata(HandleCurrentNestedResultMatrixPositionChanged));
        RootPaddingProperty = DependencyProperty.RegisterAttached("RootPadding", typeof(Thickness), _typeOfThis,
            new FrameworkPropertyMetadata(new Thickness(4.0), FrameworkPropertyMetadataOptions.Inherits));
        _unitLabel = new IdLabelToStringConverter().Convert(IdLabels.Unit, typeof(string), null, null).ToString().ToLower();
        DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        AppServiceRegistry.AddService<IFontManager>(new FontManager());
        WhiteboardDependencyProperties.IsActiveProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsActiveChanged));
        WhiteboardDependencyProperties.IsSelectedProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsSelectedChanged));
        EngineErrorMessages.MergeDictionary(ResourceProvider<EquationControlErrorMessages>.Instance);
        AttachedProperties.CanActivateProperty.OverrideMetadata(typeof(EquationControl), new FrameworkPropertyMetadata(HandleCanActivateChanged));
        CalcIsDisabledProperty = AttachedProperties.CalcIsDisabledProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleCalcIsDisabledChanged));
        ResultMatrixFormatProperty = AttachedProperties.ResultMatrixFormatProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleResultMatrixFormatChanged));
        DisplayNumberFormatProperty = AttachedProperties.DisplayNumberFormatProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleDisplayNumberFormatChanged));
        AttachedProperties.LabeledIdFormatProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleLabeledIdFormatChanged));
        IOTag.IOTagProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleIOTagPropertyChanged));
        ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(typeof(EquationControl));
    }

    private static void HandleCanActivateChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleSynchronizationStatusChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleCalculatingStatusChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleDisplayNumberFormatChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleResultFormattingChanged(string propertyName, object newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleResultMatrixFormattingChanged(string propertyName, object newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleResultMatrixFormatChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleLabeledIdFormatChanged(global::System.Windows.DependencyObject depObj, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleIOTagPropertyChanged(global::System.Windows.DependencyObject depObj, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleCurrentNestedResultMatrixPositionChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void IsSelectedChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void IsActiveChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void IsSelectedChanged(bool isSelected) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void IsActiveChanged(bool isActive) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal partial class TestHelper
    {
        public static void SetEquationXmlNode(EquationControl eq, global::System.Xml.XmlNode xmlNode)
        {
            eq.EquationXmlNode = xmlNode;
        }

        public TestHelper()
        {
        }
    }
}
