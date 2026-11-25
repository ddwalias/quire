using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire.Calculation.Core;
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
    public EquationControl() : this((Quire.EquationEditor.IEquationModel)null) { }

    public bool IsEmpty => _model.IsEmpty;

    public IResultNode Result => null;

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

    public bool IsCommitted => !_model.IsExpressionValueModified;

    public bool IsSelected => WhiteboardDependencyProperties.GetIsSelected(this);

    public bool IsReadonly => _model.IsReadonly;

    public bool IsScoped => _model.Mode == EquationEditModeEnum.Scope;

    public bool IsTypeover => _model.Mode == EquationEditModeEnum.Typeover;

    public bool IsScriptableControl => _model.IsScriptableControl;

    public bool AtLeftEdge => _model.AtLeftEdge();

    public Direction ScopeDirection => _model.ScopeSide;

    public bool CanHideFromCalculationContext => true;

    public bool InterceptAndUpdateResult => false;

    public bool UseICalculatableToRestoreResult => false;

    public IDataExchangeModel DataExchangeModel => _model.DataExchangeModel;

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

    public string TypeoverSExpression => _model.TypeoverSExpression;

    public bool CurrentNodeIsNumeric => _model.CurrentModeNavigatorState.Current.DescriptorName == "NUMERIC";

    internal double ViewFullWidth => _view.ActualWidth;

    internal double ViewClippedWidth => _view.DesiredSize.Width;

    public EquationTypes EquationType => _model.EquationType;

    public string RegionType => "equation";

    public int CurrentNodeCaretPosition => _model.CurrentModeNavigatorState.CaretPosition.GetValueOrDefault(-1);

    public bool CurrentNodeIsIdentifier => _model.CurrentModeNavigatorState.Current.DescriptorName == "IDENTIFIER";

    public bool IsEmbedded
    {
        get => (bool)GetValue(IsEmbeddedProperty);
        set => SetValue(IsEmbeddedProperty, value);
    }

    public double TextBaselineY => _view.TextBaselineY;

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

    public bool IsShrunk => IsResizable && IsExceedsWidthConstrain;

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

    public bool IsExceedsWidthConstrain
    {
        get
        {
            double resizeIndicatorWidth = IsEmbedded ? ResizeIndicatorWidth : 0.0;
            return ViewFullWidth > ActualWidth && ViewClippedWidth < ViewFullWidth - resizeIndicatorWidth;
        }
    }

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

    public bool ErrorVisible
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public EngineError Error => _model.CalculationError;

    public IList<EngineRedefInfo> EngineWarnings => _model.EngineWarnings;

    public EquationEditModeEnum Mode => _model.Mode;

    internal MathNode CurrentNode => _model.NavigatorState.Current;

    public double VerticalAlignmentOffset => -TextBaselineOffset;

    public CalculatingStatus CalculatingStatus => (CalculatingStatus)GetValue(CalculatingStatusProperty);

    public SynchronizationStatus SynchronizationStatus => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);

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

    public double ActiveElementOpacity
    {
        get => (double)GetValue(ActiveElementOpacityProperty);
        set => SetValue(ActiveElementOpacityProperty, value);
    }

    public bool IsResizableInputControl
    {
        get => (bool)GetValue(IsResizableInputControlProperty);
        set => SetValue(IsResizableInputControlProperty, value);
    }

    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public string ResultScalingSExpression
    {
        get => _model.ResultScalingSExpression;
        set => _model.ResultScalingSExpression = value;
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

    public bool AllowExpressionsOnly
    {
        get => _model.AllowExpressionsOnly;
        set => _model.AllowExpressionsOnly = value;
    }

    public bool IsMathFormatHandledLocally
    {
        get => _model.IsMathFormatHandledLocally;
        set => _model.IsMathFormatHandledLocally = value;
    }

    public bool OverrideIdLabelByResolvedForDefinitionLhs
    {
        get => _model.OverrideIdLabelByResolvedForDefinitionLhs;
        set => _model.OverrideIdLabelByResolvedForDefinitionLhs = value;
    }

    public EngineError CurrentError => Error;

    public string EquationSExpressionWithResolvedLabels => _model.EquationSExpressionWithResolvedLabels;

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

    public CaretInsertionTypes CurrentCaretInsertionType
    {
        get => (CaretInsertionTypes)GetValue(CurrentCaretInsertionTypeProperty);
        set => SetValue(CurrentCaretInsertionTypeProperty, value);
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

    public bool IsWithinSolveBlock { get; set; }

    private bool IsResizingMatrix { get; set; }

    public int? ComponentHandle { get; set; }

    public IBookmarksManager BookmarksManager { get; set; }

    public EngineResult EngineResult { get; set; }

    public MathFormat MathFormat { get; private set; }

    private IList<TableCoordinate> CurrentNestedResultMatrixPosition
    {
        get => (IList<TableCoordinate>)GetValue(CurrentNestedResultMatrixPositionProperty);
        set => SetValue(CurrentNestedResultMatrixPositionProperty, value);
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

    public MathFormat CurrentMathFormat
    {
        get => MathFormat;
        set => throw new NotImplementedException();
    }

    public StaticMathFormat LocalMathFormatForXamlSerialization
    {
        get => MathFormat.ToStaticMathFormat();
        set => MathFormat.FromStaticMathFormat(value);
    }

    private void FirePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public IEnumerable<ICalculatable> CalculatableChildren => _calculatableChildren;

    public static bool WasSuspendedRegenerate
    {
        get => EquationPresenter.WasSuspendedRegenerate;
        set => EquationPresenter.WasSuspendedRegenerate = value;
    }

    public void OnSExpressionChanging(string SExpression)
    {
        if (SExpressionChanging != null)
        {
            SExpressionChanging(SExpression);
        }
    }

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
    public void ScopeHome() => _view.FireMoveToStart();
    public void ScopeEnd() => _view.FireMoveToEnd();
    public void Pi() => _view.AutomateKeypress('π');

    private void CanCopy(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanCopy(sender, e);

    private void CanCut(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanCut(sender, e);

    private void CanPaste(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPaste(sender, e);

    private void CanPerformColumnOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformColumnOperations(sender, e);

    private void CanPerformIdLabelOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformIdLabelOperations(sender, e);

    private void CanPerformLinearNaryOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformLinearNaryOperations(sender, e);

    private void CanPerformRowOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformRowOperations(sender, e);

    private void CanNavigateToOppositeOfScope(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanNavigateToOppositeOfScope(sender, e);

    public void InsertUnitMonomial(System.Collections.Generic.IEnumerable<Quire.Calculation.Core.Unit> unitMonomial) => _view.FireInsertUnitMonomial(unitMonomial);

    private void OnTraverse(System.Windows.Input.ExecutedRoutedEventArgs e, Direction direction) => _view.FireTraversalRequest(e, direction);

    private void OnTraverseBackward(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) => OnTraverse(e, Direction.Left);

    private void OnTraverseForward(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) => OnTraverse(e, Direction.Right);

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

    private void OnModelRequestEvaluation(IEquationModel obj)
    {
        FireRequestEvaluation(true);
    }

    private void FireRequestRemove(Direction direction)
    {
        RequestRemove.Invoke(this, direction);
    }

    private void OnModelEquationChanged(bool shouldRegenerate)
    {
        _model_EquationChanged();
    }

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

    private void HandleNumericResultChanged(IResultNode resultValue)
    {
        UpdateEngineResult(resultValue);
        ResultChanged?.Invoke(resultValue);
    }

    private void FireExpressionValueChanged()
    {
        if (ExpressionValueChanged != null)
        {
            ExpressionValueChanged(this);
        }
    }

    private void HandleCalculationErrorChanged(Quire.Wpf.PropertyChangedEventArgs<EngineError> eventArgs)
    {
        UpdateEngineResult(_model.CalculationError);
        FirePropertyChanged(ErrorChangedEventArgs.GetArgs());
        UpdateLhsVisibleOnView();
    }

    public void FireContentChangedEvents()
    {
        FireContentAndRegionChangedEvents(IORegionChangeType.Content);
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

    private void HandleVisibleLhsOfDefinitionChanged(Visibility visibility)
    {
        FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("VisibleLhsOfDefinition"));
        FireContentChangedEvents();
        UpdateLhsVisibleOnView();
    }

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

    private void BindCommands()
    {
        CommandBindings.AddRange(NonEditableCommandBindings);
        if (!_model.IsRestrictedReadOnly)
        {
            BindEditingCommands();
        }
    }

    private void BindEditingCommands() => CommandBindings.AddRange(EditableCommandBindings);

    private void UnBindEditingCommands()
    {
        foreach (System.Windows.Input.CommandBinding commandBinding in EditableCommandBindings)
        {
            CommandBindings.Remove(commandBinding);
        }
    }

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

    private void _model_EquationChanged() => RaiseEvent(new RoutedEventArgs(ChangedEvent));

    private void FireContentAndRegionChangedEvents(IORegionChangeType type)
    {
        RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
        if (IORegionChanged != null)
        {
            IORegionChanged.Invoke(this, type);
        }
    }

    private void OnVisibleWarningMessageChanged() => UpdateLhsVisibleOnView();

    private void ClearExpressionValueModified()
    {
        _model.IsExpressionValueModified = false;
        FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsCommitted"));
    }

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

    public bool InCustomInitialization
    {
        get => _model.InCustomInitialization;
        set => _model.InCustomInitialization = value;
    }

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

    public CaretInsertionTypes CurrentNodeCaretInsertionType => _model.CurrentModeNavigatorState.CaretInsertionType;

    public string Algorithm
    {
        get => CurrentNode?.Algorithm;
        set => CurrentNode.Algorithm = value;
    }

    public AlgorithmOptions AlgorithmOptions => CurrentNode?.AlgorithmOptions;

    public System.Xml.XmlNode EquationXmlNode
    {
        get => _model.EquationXmlNode;
        private set => _model.EquationXmlNode = value;
    }

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

    public int WarningsCount => _model.Warnings?.Count ?? 0;

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

    private void ReadDeserializationFormatsForPaste()
    {
        if (DebugTools.DevAssert(MathFormatForPaste.Instance != null && MathFormatForPaste.Instance.DocumentMathFormat != null && MathFormatForPaste.Instance.DocumentLabeledIdFormat != null).Failed)
        {
            return;
        }

        DeserializeMathFormat = MathFormatForPaste.Instance.DocumentMathFormat;
        DeserializeLabeledIdFormat = MathFormatForPaste.Instance.DocumentLabeledIdFormat;
    }
}
