using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
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

    public void Commit()
    {
        FireRequestEvaluation();
    }
    public void SetStatusSynchronized()
    {
        // The IL reads the event field twice (null check, then invoke); "?.Invoke" reads it once.
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
        _calculatableChildren.Each(child => child.SetStatusSynchronized());
    }
    public void SetSynchronizationStatus(global::Quire.Calculation.Core.SynchronizationStatus syncStatus)
    {
        _syncStatus = syncStatus;
        UpdateSynchronizationStatus();
    }
    public void SetCalculatingStatus(global::Quire.Calculation.Core.CalculatingStatus calculatingStatus)
    {
        UpdateCalculatingStatus(calculatingStatus);
    }
    public void SetResult(global::Quire.Calculation.Core.IResultNode result)
    {
    }
    public void SetError(global::Quire.Calculation.Core.EngineError error)
    {
        _model.HandleError(error);
    }
    public void SetScriptingEngineError(string error)
    {
        _model.HandleScriptingEngineError(error);
    }

    public bool OverrideIdLabelByResolvedForDefinitionLhs
    {
        get => _model.OverrideIdLabelByResolvedForDefinitionLhs;
        set => _model.OverrideIdLabelByResolvedForDefinitionLhs = value;
    }

    public void SetIdentifierLabels(global::System.Collections.Generic.IDictionary<global::Quire.Calculation.Core.ResolvedId, string[]> idLabelMap)
    {
        _model.SetIdentifierLabels(idLabelMap);
    }
    public void SetAutoSelectAlgorithm(global::System.Collections.Generic.IDictionary<int, string> autoAlgorithmMap)
    {
        _model.SetAutoSelectAlgorithm(autoAlgorithmMap);
    }
    public void SetRedefinitionWarnings(global::System.Collections.Generic.IList<global::Quire.Calculation.Core.EngineRedefInfo> warnings)
    {
        _model.SetRedefinitionWarnings(warnings);
    }
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;
    public IEnumerable<ICalculatable> CalculatableChildren => _calculatableChildren;
    private void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(this, calc);
        }
    }
    private void FireCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calc);
        }
    }
    private void FireCalculatableMoved(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(calc);
        }
    }
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
    public bool HideFromCalculationContext { get => CalcIsDisabled; set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private void UpdateSynchronizationStatus()
    {
        SynchronizationStatus synchronizationStatus = SynchronizationStatus.Synchronized;
        if (_syncStatus == SynchronizationStatus.Pending || _calculatableChildren.Any(child => child.SynchronizationStatus == SynchronizationStatus.Pending))
        {
            synchronizationStatus = SynchronizationStatus.Pending;
        }
        SetValue(SynchronizationStatusProperty, synchronizationStatus);
        UpdateEngineResult(synchronizationStatus);
    }
    private void SetMatrixOrigin(global::Quire.Calculation.Core.RealSimpleResultNode result = default)
    {
        if (result == null)
        {
            return;
        }
        using (new CommandScope(new CompoundCommand()))
        {
            if (ResultMatrixFormat != null)
            {
                ResultMatrixFormat.Origin = (int)result.Value;
            }
        }
    }
    private void UpdateCalculatingStatus(global::Quire.Calculation.Core.CalculatingStatus calculatingStatus)
    {
        if (_calculatableChildren.Any(child => child.CalculatingStatus == CalculatingStatus.Calculating))
        {
            calculatingStatus = CalculatingStatus.Calculating;
        }
        SetValue(CalculatingStatusProperty, calculatingStatus);
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

    private global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> UpdateAggregateFromModel()
    {
        MathNodeToRlTransformer mathNodeToRlTransformer = MathNodeToRlTransformer.Default;
        IList<string> rlList = mathNodeToRlTransformer.TransformForAggregateWithReplacementScalingFactor(_model.EquationRoot, _model.ResultScalingFactor, IsWithinSolveBlock);
        if (DebugTools.DevAssert(rlList.IsNotEmpty(), null).Failed)
        {
            return Enumerable.Empty<ICalculatable>();
        }
        int childCount = rlList.Count - 1;
        if (childCount < _calculatableChildren.Count)
        {
            // The IL keeps "childCount - 1" in the loop test; "i >= childCount" compiles to a different branch.
            for (int i = _calculatableChildren.Count - 1; i > childCount - 1; i--)
            {
                ICalculatable calc = _calculatableChildren[i];
                _calculatableChildren.RemoveAt(i);
                FireCalculatableRemoved(calc);
            }
        }
        else
        {
            for (int i = _calculatableChildren.Count; i < childCount; i++)
            {
                EquationControlChildCalculatable child = new EquationControlChildCalculatable(this)
                {
                    Rl = "nil",
                    RlIndex = i + 1
                };
                _calculatableChildren.Add(child);
                FireCalculatableAdded(child);
            }
        }
        List<ICalculatable> updatesNeeded = new List<ICalculatable>();
        _calculatableChildren.EachWithIndex((index, child) =>
        {
            // The IL advances the index in place before the first lookup.
            if (child.Rl != rlList[++index])
            {
                updatesNeeded.Add(child);
                child.Rl = rlList[index];
            }
        });
        if (rlList[0] != _rl)
        {
            updatesNeeded.Add(this);
            _rl = mathNodeToRlTransformer.ApplyRlParams(rlList[0], new MathNodeToRlTransformer.MathNodeToRlTransformerParams
            {
                Handle = ComponentHandle,
                HasError = _model.IsScriptError
            });
        }
        return updatesNeeded;
    }
    public IDataExchangeModel DataExchangeModel => _model.DataExchangeModel;
    public int? ComponentHandle { get; set; }
    public int ComponentCookieMultiThreaded { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal readonly global::Quire.EquationEditor.IMathNodeToVisualNodeMap _map;
    internal readonly global::Quire.EquationEditor.IEquationModel _model;
    private const string FirstPropertyForXamlSerializationPropertyName = "FirstPropertyForXamlSerialization";
    internal readonly global::Quire.Controls.Internal.VisualMathRegionNode _view;
    public int IncrementComponentCookieMultiThreaded(bool isAutoCalc = false)
    {
        if (ComponentHandle.HasValue && DataExchangeModel is IDataExchangeScriptingModel dataExchangeScriptingModel)
        {
            dataExchangeScriptingModel.SetState(CalculatableState.Commit, isAutoCalc);
        }
        return 0;
    }
    public event global::Quire.Delegate1<global::Quire.Controls.Core.IEquationControl> ChangeToTextBox;
    private bool _isSymbolicEvalKeywordsShown = true;
    public const double RecommendedSymbolicResultsWidthInInches = 5d;
    public static readonly double RecommendedSymbolicResultsWidth;

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

    public void HandleSizeChanged(global::System.Windows.Vector offset, bool changeWidth, bool changeHeight, bool resizeInProgress)
    {
        ((global::Quire.Controls.Internal.VisualMathRegionNode)Content).MeasureResizableChildNode(offset, changeWidth, changeHeight, resizeInProgress);
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

    protected override void OnTemplateChanged(global::System.Windows.Controls.ControlTemplate oldTemplate, global::System.Windows.Controls.ControlTemplate newTemplate) => base.OnTemplateChanged(oldTemplate, newTemplate);

    public string Algorithm
    {
        get => CurrentNode?.Algorithm;
        set => CurrentNode.Algorithm = value;
    }

    public AlgorithmOptions AlgorithmOptions => CurrentNode?.AlgorithmOptions;
    public void FireAlgorithmChanged()
    {
        _model.FireAlgorithmChanged();
    }
    public bool IsSelected => WhiteboardDependencyProperties.GetIsSelected(this);

    public bool InCustomInitialization
    {
        get => _model.InCustomInitialization;
        set => _model.InCustomInitialization = value;
    }

    public bool SubstituteAndCommitInputValue(string sExpr, string valueSExpr, string scalingSExpr)
    {
        string origSExpr = EquationSExpression;
        DelegateFunction0<bool> doMethod = () =>
        {
            if (!_model.SetDefinitionValue(sExpr, valueSExpr, scalingSExpr))
            {
                return false;
            }
            Commit();
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            _model.SetEquationSExpression(origSExpr, isUserOperation: false);
            Commit();
        };
        return CommandBuffer.Execute(new UndoableCommand("CommitEquationScalarValue", doMethod, undoMethod));
    }
    public bool IsSuitableForInputTaggingPrivate()
    {
        return _model.IsSuitableForInputTagging();
    }
    public bool IsSuitableForOutputTaggingPrivate()
    {
        if (EquationType != EquationTypes.Evaluation)
        {
            return EquationType == EquationTypes.DefinitionToEvaluatedResult;
        }
        return true;
    }
    public void ActivateAndScopeIdentifierPrivate(string idSExpr)
    {
        _model.ScopeIdentifier(idSExpr);
        Activate(isActive: true);
    }
    public string GetDefinitionSExpression(string idSExpr)
    {
        return _model.GetDefinitionSExpression(idSExpr);
    }
    public global::Quire.Undo.UndoableCommand CreateToggleInputCommand(global::System.Action<global::Quire.Controls.Core.IORegions.IInputRegion, bool> toggleInputTag, bool setTag)
    {
        string undoSExpr = EquationSExpression;
        string reDoSExpr = null;
        DelegateFunction0<bool> doMethod = () =>
        {
            if (setTag)
            {
                Commit();
            }
            toggleInputTag(this, setTag);
            if (IsKeyboardFocusWithin)
            {
                EquationCommands.NavigateToEquationStart.Execute(null, this);
            }
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            reDoSExpr = EquationSExpression;
            ToggleInput(!setTag, undoSExpr, toggleInputTag);
        };
        Delegate0 redoMethod = () =>
        {
            if (DebugTools.AssertNotNull(reDoSExpr, null).Failed)
            {
                return;
            }
            ToggleInput(setTag, reDoSExpr, toggleInputTag);
        };
        return new UndoableCommand("EquationPreSetInput", doMethod, undoMethod, redoMethod);
    }
    private void ToggleInput(bool setTag, string sExpr, global::System.Action<global::Quire.Controls.Core.IORegions.IInputRegion, bool> toggleInputTag)
    {
        if (!setTag)
        {
            toggleInputTag(this, false);
        }
        bool isKeyboardFocusWithin = IsKeyboardFocusWithin;
        EquationSExpression = sExpr;
        Commit();
        if (isKeyboardFocusWithin)
        {
            EquationCommands.NavigateToEquationStart.Execute(null, this);
        }
        if (setTag)
        {
            toggleInputTag(this, true);
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

    private readonly global::Quire.DelegateFunction1<string, global::Quire.Calculation.Core.EngineError> GetErrorDisplayText;
    public event Action<IWhiteboardItem, Direction> RequestRemove = delegate { };
    public event global::System.Action RootNodeChanged;
    public event global::System.Action MeasureOverideCompleted;
    public event Action<IWhiteboardItem> OnGotFocusEvent = delegate { };
    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem> OnLostFocusEvent;
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    public event global::System.Action<object, global::System.Windows.RoutedEventArgs> OnAdornedElementReady;
    public event global::System.Windows.RoutedEventHandler Changed;

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.EquationControl+EquationControlChildCalculatable, Ptc.Controls.EquationControl")]
    private partial class EquationControlChildCalculatable : global::Quire.Calculation.Core.Support.SimpleCalculatable
    {
        private global::Quire.Controls.EquationControl _parent;
        public EquationControlChildCalculatable(global::Quire.Controls.EquationControl parent) { }
        public global::Quire.Controls.EquationControl.EquationControlChildCalculatable Clone(global::Quire.Controls.EquationControl newParent)
        {
            EquationControlChildCalculatable equationControlChildCalculatable = new EquationControlChildCalculatable(newParent);
            equationControlChildCalculatable.RlIndex = RlIndex;
            equationControlChildCalculatable.Rl = Rl;
            equationControlChildCalculatable.SetSynchronizationStatus(SynchronizationStatus);
            equationControlChildCalculatable.SetCalculatingStatus(CalculatingStatus);
            return equationControlChildCalculatable;
        }
        public override void SetSynchronizationStatus(global::Quire.Calculation.Core.SynchronizationStatus syncStatus) => base.SetSynchronizationStatus(syncStatus);
        public override void SetCalculatingStatus(global::Quire.Calculation.Core.CalculatingStatus computingStatus) => base.SetCalculatingStatus(computingStatus);
        public override void SetResult(global::Quire.Calculation.Core.IResultNode result) => base.SetResult(result);
        public override void SetError(global::Quire.Calculation.Core.EngineError error) => base.SetError(error);
        public override void SetIdentifierLabels(global::System.Collections.Generic.IDictionary<global::Quire.Calculation.Core.ResolvedId, string[]> idLabelMap) => base.SetIdentifierLabels(idLabelMap);
        public override void SetAutoSelectAlgorithm(global::System.Collections.Generic.IDictionary<int, string> autoAlgorithmMap) => base.SetAutoSelectAlgorithm(autoAlgorithmMap);
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.EquationControl+SizeList2String, Ptc.Controls.EquationControl")]
    internal static partial class SizeList2String
    {
        public static string WriteWrappedSizesToString(List<Size> sizeData, int numDecimalPlaces = -1)
        {
            Func<Size, Size> doRound = size => numDecimalPlaces < 0 ? size : new Size(Math.Round(size.Width, numDecimalPlaces), Math.Round(size.Height, numDecimalPlaces));
            return string.Join(", ", sizeData.Select(sizeItem => string.Format("{{{0}}}", doRound(sizeItem))).ToList());
        }
        public static global::System.Collections.Generic.List<global::System.Windows.Size> ReadWrappedSizesFromString(string sizesString)
        {
            return sizesString.Trim('{', '}').Split(new[] { "}, {" }, StringSplitOptions.RemoveEmptyEntries).Select(Size.Parse).ToList();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.EquationControl+PropertyNames, Ptc.Controls.EquationControl")]
    public static partial class PropertyNames
    {
        public const string CurrentNumberFormat = "CurrentNumberFormat";
        public const string CurrentMatrixFormat = "CurrentMatrixFormat";
        public const string CurrentIdLabel = "CurrentIdLabel";
        public const string CurrentDisplayIdLabel = "CurrentDisplayIdLabel";
        public const string IsCommitted = "IsCommitted";
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.EquationControl+TestHelper, Ptc.Controls.EquationControl")]
    internal partial class TestHelper
    {
    }

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
    private void SetupInputGestures()
    {
        InputBindings.Add(new global::System.Windows.Input.KeyBinding(global::System.Windows.Documents.EditingCommands.MoveDownByPage, global::Quire.Wpf.Gestures.PageDown));
        InputBindings.Add(new global::System.Windows.Input.KeyBinding(global::System.Windows.Documents.EditingCommands.MoveUpByPage, global::Quire.Wpf.Gestures.PageUp));
    }
    protected override void OnContextMenuClosing(global::System.Windows.Controls.ContextMenuEventArgs e) => base.OnContextMenuClosing(e);
    private void SetContextMenu(string contextMenuKey)
    {
        ContextMenu = (ContextMenu)FindResource(contextMenuKey);
        ContextMenu.DataContext = this;
    }
    protected override void OnContextMenuOpening(global::System.Windows.Controls.ContextMenuEventArgs e) => base.OnContextMenuOpening(e);
    private void HandleRequestChangeToTextBox()
    {
        ChangeToTextBox?.Invoke(this);
    }
    public bool IsChangeToTextBoxReady()
    {
        return ChangeToTextBox != null;
    }
    public bool IsWithinSolveBlock { get; set; }
    private global::Quire.Controls.Core.MatrixFormat _currentMatrixFormat;
    private void ActivateEquationForComboBoxNode(object sender, global::System.Windows.RoutedEventArgs e)
    {
        if (e.Source is global::Quire.EquationEditor.View.IVisualInputControlNode visual)
        {
            MathNode mathNode = _map.AssociateNode(visual);
            if (mathNode != _model.CurrentModeNavigatorState.Current || _model.CurrentModeNavigatorState.CaretSide != Direction.Indeterminate)
            {
                _model.JumpToNode(mathNode, Direction.Indeterminate);
            }
            if (!_view.IsLogicalFocusWithin || mathNode.DescriptorName == "ScriptableControl")
            {
                Activate(isActive: true);
            }
            e.Handled = true;
        }
        else
        {
            DebugTools.FailAssertion(e.Source?.GetType().Name);
        }
    }
    private void OnViewDeActivated()
    {
        AnimateHide();
        UpdateLhsVisibleOnView();
    }
    private void OnViewOnActivated(bool skipAnimation = false)
    {
        if (!skipAnimation)
        {
            AnimateShow();
        }
        UpdateLhsVisibleOnView();
        OnGotFocusEvent(this);
    }
    private void HandleRequestToAllowContextMenu(bool allow)
    {
        if (allow)
        {
            AttachedProperties.SetHandlesContextMenu(this, true);
        }
        else
        {
            AttachedProperties.ClearHandlesContextMenu(this);
        }
    }
    private void HandleRequestNavigateOutOfInputControlNode(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.RoutedUICommand> args)
    {
        global::System.Windows.Input.RoutedCommand parameter = args.Parameter;
        if (parameter != null && DebugTools.DevAssert(parameter.Name == EquationCommands.NavigateToEquationEnd.Name || parameter.Name == EquationCommands.NavigateToEquationStart.Name || parameter.Name == EquationCommands.NavigateLeft.Name || parameter.Name == EquationCommands.NavigateRight.Name, null).Passed)
        {
            _view.EnsureFocus();
            parameter.Execute(null, this);
        }
    }
    private void FireMeasureOverideCompleted()
    {
        if (MeasureOverideCompleted != null)
        {
            MeasureOverideCompleted();
        }
    }
    private void OnRootNodeChanged()
    {
        ExecuteOnRootLoaded(OnRootLoaded);
        FireRootNodeChanged();
    }
    private void FireRootNodeChanged()
    {
        if (RootNodeChanged != null)
        {
            RootNodeChanged();
        }
    }
    object global::System.ICloneable.Clone()
    {
        return Clone();
    }
    public global::Quire.Controls.EquationControl Clone() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnTraversedFocusEvent(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.FocusNavigationDirection> args)
    {
        if ((int)args.Parameter == 0)
        {
            NavigateRegionStart();
        }
        else
        {
            NavigateRegionEnd();
        }
    }
    private void HandleExpressionValueChanged(global::Quire.Controls.Core.IEquationControl equation)
    {
        if (EquationType == EquationTypes.Evaluation || EquationType == EquationTypes.DefinitionToEvaluatedResult)
        {
            CurrentNumberFormat = DisplayNumberFormat;
        }
        else
        {
            CurrentNumberFormat = null;
        }
        FirePropertyChanged(new global::System.ComponentModel.PropertyChangedEventArgs("IsCommitted"));
    }
    public EquationControl() : this((Quire.EquationEditor.IEquationModel)null) { }

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("Constructor body not mirrored yet; the this() chain keeps the field initializers out of this constructor.")]
    public EquationControl(string sExpression) : this() { }

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("Constructor body not mirrored yet; the this() chain keeps the field initializers out of this constructor.")]
    public EquationControl(string sExpression, bool isForEngineCalculation) : this(sExpression) { }

    public static global::Quire.Controls.EquationControl CreateWithTextInput(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        using (new CommandScope("Create equation from text input"))
        {
            EquationControl equationControl = new EquationControl();
            char key = s[0];
            equationControl._view.AutomateKeypress(key);
            if (!equationControl._model.EquationRoot.IsPlaceholder)
            {
                for (int i = 1; i < s.Length; i++)
                {
                    equationControl._view.AutomateKeypress(s[i]);
                }
                return equationControl;
            }
        }
        return null;
    }
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
    public void StartResizingMatrix(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix)
    {
        IsResizingMatrix = true;
        AttachedProperties.SetIsBringToView(this, false);
        IList<TableCoordinate> nestedPositionForVisualMatrix = GetNestedPositionForVisualMatrix(visualMatrix);
        if (!AreTableCoordinateListsEqual(CurrentNestedResultMatrixPosition, nestedPositionForVisualMatrix))
        {
            CurrentNestedResultMatrixPosition = nestedPositionForVisualMatrix;
        }
        else
        {
            CreateTemporaryResultMatrixFormat();
        }
    }
    public void StopResizingMatrix()
    {
        IsResizingMatrix = false;
        try
        {
            if (!IsKeyboardFocusWithin)
            {
                AttachedProperties.SetIsBringToView(this, false);
                Activate(isActive: true);
            }
        }
        finally
        {
            AttachedProperties.SetIsBringToView(this, true);
        }
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

    public double VerticalAlignmentOffset => -TextBaselineOffset;
    public bool IsEmpty => _model.IsEmpty;
    private event global::System.Action<global::Quire.Controls.Core.FocusChangeLevel> _postActivated;
    public void Activate(bool isActive)
    {
        if (isActive)
        {
            if (!IsKeyboardFocusWithin)
            {
                AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(this, () =>
                {
                    if (!IsKeyboardFocusWithin)
                    {
                        bool activating = _view.Activating;
                        _view.Activating = true;
                        try
                        {
                            Focus();
                        }
                        finally
                        {
                            _view.Activating = activating;
                        }
                    }
                });
            }
            else if (!_view.IsLogicalFocusWithin)
            {
                DebugTools.DevAssert(_view.IsKeyboardFocusWithin, null);
                _view.FireActivated(setLogicalFocus: true);
                _postActivated?.Invoke(FocusChangeLevel.Logical);
            }
            else
            {
                OnViewOnActivated(skipAnimation: true);
                _postActivated?.Invoke(FocusChangeLevel.None);
            }
        }
    }
    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction)
    {
        Activate(isActive: true);
        switch (direction)
        {
            case global::Quire.Direction.Left:
            case global::Quire.Direction.Up:
                NavigateRegionEnd();
                break;
            case global::Quire.Direction.Right:
            case global::Quire.Direction.Down:
                NavigateRegionStart();
                break;
        }
        return null;
    }
    public void SimulateMouseClick(global::System.Windows.Point location, int numberOfClicks)
    {
        Activate(isActive: true);
        WpfUtils.ExecuteOnLoad(this, () =>
        {
            _view.SimulateMouseClick(location, numberOfClicks);
        });
    }
    public bool CanNavigate(global::Quire.Direction direction)
    {
        return _model.CanNavigate(direction);
    }
    public bool CanReturn()
    {
        bool canPerform = false;
        _model.CanPerformLinearNaryOperations(ref canPerform);
        return canPerform;
    }
    public bool CanTraverse(global::System.Windows.Input.FocusNavigationDirection direction)
    {
        bool canTraverse = false;
        if ((int)direction == 0)
        {
            _model.OnCanTraverseForward(ref canTraverse);
        }
        else
        {
            _model.OnCanTraverseBackward(ref canTraverse);
        }
        return canTraverse;
    }

    public bool IsEmbedded
    {
        get => (bool)GetValue(IsEmbeddedProperty);
        set => SetValue(IsEmbeddedProperty, value);
    }

    public bool IsScriptableControl => _model.IsScriptableControl;
    protected override void OnGotFocus(global::System.Windows.RoutedEventArgs e) => base.OnGotFocus(e);
    private void FocusViewAndUpdateNavigatorStateIfNeeded()
    {
        bool navigatorStateWasChanged = false;
        _model.NavigatorStateChanged += OnNavigatorStateChangedMarkStateChanged;
        _view.Focus();
        _model.NavigatorStateChanged -= OnNavigatorStateChangedMarkStateChanged;
        if (!navigatorStateWasChanged && _model.Mode == EquationEditModeEnum.Normal && DebugTools.DevAssert(_view.IsFocused, null).Passed)
        {
            _model.FireNavigatorStateChanged();
        }
        _postActivated?.Invoke(FocusChangeLevel.Keyboard);

        void OnNavigatorStateChangedMarkStateChanged(NavigatorStateChangeEventArgs _)
        {
            navigatorStateWasChanged = true;
        }
    }
    private void ViewOnIsVisibleChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (DebugTools.DevAssert(_view.IsVisible, null).Passed)
        {
            _view.IsVisibleChanged -= ViewOnIsVisibleChanged;
            FocusViewAndUpdateNavigatorStateIfNeeded();
        }
    }
    protected override void OnGotKeyboardFocus(global::System.Windows.Input.KeyboardFocusChangedEventArgs e) => base.OnGotKeyboardFocus(e);
    protected override void OnKeyDown(global::System.Windows.Input.KeyEventArgs e) => base.OnKeyDown(e);
    public global::System.Windows.Point GetImeWindowPosition() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnTextInput(global::System.Windows.Input.TextCompositionEventArgs e) => base.OnTextInput(e);
    protected void AnimateShow()
    {
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 25));
        DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, 1.0, duration);
        Storyboard storyboard = new Storyboard
        {
            Children = { doubleAnimation }
        };
        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ActiveElementOpacityProperty));
        DoubleAnimation doubleAnimation2 = new DoubleAnimation(1.0, 0.3, duration);
        storyboard.Children.Add(doubleAnimation2);
        Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(ActiveNonNavigatableElementOpacityProperty));
        storyboard.Begin(this);
    }
    protected void AnimateHide()
    {
        Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 25));
        DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, 0.0, duration);
        Storyboard storyboard = new Storyboard
        {
            Children = { doubleAnimation }
        };
        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ActiveElementOpacityProperty));
        DoubleAnimation doubleAnimation2 = new DoubleAnimation(0.3, 1.0, duration);
        storyboard.Children.Add(doubleAnimation2);
        Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(ActiveNonNavigatableElementOpacityProperty));
        storyboard.Begin(this);
    }
    internal MathNode CurrentNode => _model.NavigatorState.Current;
    public EquationEditModeEnum Mode => _model.Mode;
    public override void EndInit() => base.EndInit();
    public EngineResult EngineResult { get; set; }
    public bool InterceptAndUpdateResult => false;
    public bool UseICalculatableToRestoreResult => false;
    public void RestoreResult()
    {
        if (EngineResult.IsEmpty())
        {
            return;
        }
        EngineResult saved = EngineResult;
        UpdateAggregateFromModel();
        if (_model.HasNumericEvaluation)
        {
            int num = _model.GetRlIndexForNumericResult();
            if (IsScriptableControl || num < 1)
            {
                num = _calculatableChildren.Count;
            }
            _calculatableChildren[num - 1].SetResult(saved.ResultNode);
        }
        global::Quire.Calculation.Core.EngineError error = null;
        if (saved.Errors != null)
        {
            error = saved.Errors.FirstOrDefault();
        }
        _model.HandleError(error);
        if (saved.Warnings.IsNotEmpty())
        {
            _model.SetRedefinitionWarnings(saved.Warnings);
        }
        SetSynchronizationStatus(saved.SynchronizationStatus);
        if (!CalculatableChildren.IsNullOrEmpty())
        {
            CalculatableChildren.Each((global::Quire.Calculation.Core.ICalculatable child) =>
            {
                child.SetSynchronizationStatus(saved.SynchronizationStatus);
            });
        }
        UpdateSynchronizationStatus();
    }
    public event global::System.Action<string> SExpressionChanging;
    public event global::System.Action<global::Quire.Controls.Core.IEquationControl> ExpressionValueChanged;
    public EquationTypes EquationType => _model.EquationType;
    public global::System.Windows.Media.ScaleTransform GetTransformBasedOnNumericNodeHeight(double desiredHeight)
    {
        return _view.GetTransformBasedOnNumericNodeHeight(desiredHeight);
    }

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

    public string EquationSExpression
    {
        get
        {
            return _model.EquationSExpression;
        }
        set
        {
            OnSExpressionChanging(value);
            _model.SetEquationSExpression(value, !InCustomInitialization);
            if (_model.EvaluationRoot != null)
            {
                FireRequestEvaluation();
            }
        }
    }
    public string EquationSExpressionWithResolvedLabels => _model.EquationSExpressionWithResolvedLabels;
    public string RegionType => "equation";

    public System.Xml.XmlNode EquationXmlNode
    {
        get => _model.EquationXmlNode;
        private set => _model.EquationXmlNode = value;
    }

    private void SetRegionResultFormat(global::System.Xml.XmlNode xmlElement, string ns)
    {
        if (xmlElement != null)
        {
            global::Quire.Controls.Core.NumberFormat numberFormat = (DisplayNumberFormat != null) ? DisplayNumberFormat.Clone() : new global::Quire.Controls.Core.NumberFormat();
            global::Quire.Controls.Core.MatrixFormat matrixFormat = (ResultMatrixFormat != null) ? ResultMatrixFormat.Clone() : new global::Quire.Controls.Core.MatrixFormat();
            global::Quire.Controls.Core.ResultFormat resultFormat = new global::Quire.Controls.Core.ResultFormat(numberFormat, matrixFormat, ns);
            using (global::System.Xml.XmlNodeReader reader = new global::System.Xml.XmlNodeReader(xmlElement))
            {
                resultFormat.ReadXml(reader);
            }
            if (resultFormat.NumberFormatWasRead)
            {
                DisplayNumberFormat = numberFormat;
            }
            if (resultFormat.MatrixFormatWasRead)
            {
                _temporaryResultMatrixFormatShouldClear = false;
                ResultMatrixFormat = matrixFormat;
            }
        }
    }
    private void CreateFakeEngineResults()
    {
        global::Quire.Calculation.Core.IResultNode resultNode = null;
        bool flag = true;
        global::Quire.EquationEditor.Tree.MathNode evaluationRoot = _model.EvaluationRoot;
        if (evaluationRoot == null)
        {
            return;
        }
        if (evaluationRoot.DescriptorName == "EVALUATION" && evaluationRoot[1].DescriptorName == "RSCALE")
        {
            global::Quire.EquationEditor.Tree.MathNode mathNode = evaluationRoot[1][0];
            if (mathNode.DescriptorName == "RESULT")
            {
                flag = false;
            }
            else
            {
                DebugTools.DevAssert(false, "Should never have results inside math region XML, now that we save ResultManager in the Converter !");
                try
                {
                    global::Quire.Calculation.Core.XmcdToIResultNodeTransform xmcdToIResultNodeTransform = new global::Quire.Calculation.Core.XmcdToIResultNodeTransform(new global::Quire.Calculation.Core.SimpleResultNodeFactory());
                    mathNode.MakeReadOnly();
                    resultNode = xmcdToIResultNodeTransform.Transform(global::Quire.EquationEditor.Tree.Transform.MathNodeToXmlTransform.Default.Transform(mathNode));
                }
                catch
                {
                    flag = false;
                }
            }
        }
        if (evaluationRoot.DescriptorName == "SYMBOLIC_EVALUATION")
        {
            if (evaluationRoot[2].DescriptorName == "RESULT")
            {
                flag = false;
            }
            else
            {
                ClearExpressionValueModified();
                _model.FireEquationChanged();
                _model.MoveToRegionStart();
            }
        }
        if (flag && resultNode != null)
        {
            global::Quire.Calculation.Core.ResultManager.Instance.CreateOldResultRefId(resultNode, this);
        }
    }

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

    public void ExecuteOnRootLoaded(global::System.Action action)
    {
        Action postponedAction = null;
        if (_view.RootNode == null)
        {
            postponedAction = () =>
            {
                if (_view.RootNode == null)
                {
                    return;
                }
                if (postponedAction != null)
                {
                    _view.RootNodeChanged -= postponedAction;
                }
                global::Quire.Services.AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(_view.RootNode as global::System.Windows.FrameworkElement, action);
            };
            _view.RootNodeChanged += postponedAction;
        }
        else
        {
            global::Quire.Services.AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(_view.RootNode as global::System.Windows.FrameworkElement, action);
        }
    }
    private void ExecuteOnRootSizeChanged(global::System.Action action)
    {
        if (_view.RootNode is FrameworkElement frameworkElement)
        {
            frameworkElement.SizeChanged += (object sender, SizeChangedEventArgs args) =>
            {
                action();
            };
        }
    }
    public static global::System.Windows.RoutedEvent ChangedEvent;
    public void SetRegionXmlNode(global::System.Xml.XmlNode value)
    {
        bool flag = false;
        if (value.LocalName == "math")
        {
            EquationXmlNode = value.FirstChild;
            flag = ResultManager.Instance.UpdateResultRef(value, this);
            global::System.Xml.XmlElement xmlElement = value["resultFormat"];
            SetRegionResultFormat(xmlElement, value.NamespaceURI);
        }
        else
        {
            EquationXmlNode = value;
        }
        if (!flag)
        {
            CreateFakeEngineResults();
        }
    }
    public global::System.Xml.XmlNode GetRegionXmlNode()
    {
        DebugTools.DevAssert(_isForEngineCalculation, null);
        global::System.Xml.XmlDocument xmlDocument = new global::System.Xml.XmlDocument();
        global::System.Xml.XmlElement xmlElement = xmlDocument.CreateElement("math", global::Quire.Xml.XmlWorksheetData.McdxNameSpaceManager.LookupNamespace(string.Empty));
        global::System.Xml.XmlNode equationXmlNode = EquationXmlNode;
        if (equationXmlNode != null)
        {
            global::System.Xml.XmlNode newChild = xmlDocument.ImportNode(equationXmlNode, deep: true);
            xmlElement.AppendChild(newChild);
        }
        if (!AllowExpressionsOnly)
        {
            bool flag = global::Quire.Wpf.WpfUtils.HasLocalValue(this, DisplayNumberFormatProperty);
            bool flag2 = global::Quire.Wpf.WpfUtils.HasLocalValue(this, ResultMatrixFormatProperty);
            if (flag | flag2)
            {
                using global::Quire.Xml.XmlNodeWriter writer = new global::Quire.Xml.XmlNodeWriter(xmlElement);
                new global::Quire.Controls.Core.ResultFormat(flag ? DisplayNumberFormat : null, flag2 ? ResultMatrixFormat : null, xmlElement.NamespaceURI).WriteXml(writer);
            }
        }
        if (IsMathFormatHandledLocally && MathFormat.AreAnyPropertiesSet)
        {
            using global::Quire.Xml.XmlNodeWriter writer2 = new global::Quire.Xml.XmlNodeWriter(xmlElement);
            new global::Quire.Controls.Core.Serialization.MathFormatSerializer("formattingOverride", xmlElement.NamespaceURI, MathFormat).WriteXml((global::System.Xml.XmlWriter)writer2);
        }
        return xmlElement;
    }
    public global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, object data)
    {
        if (data is global::Quire.PersistentDataObjects.mathType mathType2)
        {
            for (int i = 1; i < mathType2.InnerXml.Length; i++)
            {
                global::System.Xml.XmlNode xmlNode = mathType2.InnerXml[i];
                if (xmlNode.Name == "resultFormat")
                {
                    SetRegionResultFormat(xmlNode, xmlNode.NamespaceURI);
                }
                else if (xmlNode.Name == "formattingOverride")
                {
                    using global::System.Xml.XmlNodeReader reader = new global::System.Xml.XmlNodeReader(xmlNode);
                    new global::Quire.Controls.Core.Serialization.MathFormatSerializer("formattingOverride", xmlNode.NamespaceURI, MathFormat, ignoreUnsetProperty: false).ReadXml((global::System.Xml.XmlReader)reader);
                }
            }
            EquationXmlNode = mathType2.InnerXml[0];
            if (mathType2.resultRefSpecified)
            {
                global::Quire.Calculation.Core.ResultManager.Instance.SetResultByRefId(mathType2.resultRef, this);
            }
            else
            {
                if (IsScriptableControl)
                {
                    global::Quire.Calculation.Core.ResultManager.Instance.SetResultByRefId(mathType2.resultRef, this, mathType2.resultRef == 0);
                }
                CreateFakeEngineResults();
            }
            HideFromCalculationContext = mathType2.disablecalc;
            if (!mathType2.showLhsOfDefinition)
            {
                mathType2.visibilityLhsOfDefinition = global::Quire.PersistentDataObjects.LhsOfDefinitionVisibility.Hidden;
            }
            if (mathType2.visibilityLhsOfDefinition != (global::Quire.PersistentDataObjects.LhsOfDefinitionVisibility)VisibleLhsOfDefinition)
            {
                _model.ToggleVisibleLhsOfDefinition((Visibility)mathType2.visibilityLhsOfDefinition);
            }
            SymbolicEvalKeywordsVisibilityState = mathType2.symbolicEvalKeywordsVisibilityState;
            return null;
        }
        throw new ArgumentException("should be of mathType");
    }
    public object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider)
    {
        int num = global::Quire.Calculation.Core.ResultManager.Instance.CreateResultRefId(this);
        global::System.Xml.XmlNodeList childNodes = GetRegionXmlNode().ChildNodes;
        global::System.Xml.XmlNode[] array = new global::System.Xml.XmlNode[childNodes.Count];
        for (int num2 = 0; num2 < childNodes.Count; num2++)
        {
            array[num2] = childNodes[num2];
        }
        global::Quire.PersistentDataObjects.mathType mathType2 = new global::Quire.PersistentDataObjects.mathType
        {
            InnerXml = array,
            disablecalc = HideFromCalculationContext
        };
        if (global::Quire.Calculation.Core.IResultManagerHelper.IsResultRefIdSpecified(num))
        {
            mathType2.resultRefSpecified = true;
            mathType2.resultRef = num;
        }
        mathType2.visibilityLhsOfDefinition = (global::Quire.PersistentDataObjects.LhsOfDefinitionVisibility)VisibleLhsOfDefinition;
        mathType2.symbolicEvalKeywordsVisibilityState = SymbolicEvalKeywordsVisibilityState;
        return mathType2;
    }
    public string TypeoverSExpression => _model.TypeoverSExpression;
    public string[] History
    {
        get
        {
            List<string> history = new global::System.Collections.Generic.List<string>(_model.VisualHistory);
            return history.ToArray();
        }
    }
    public int GetRegionHistoryCount()
    {
        return History.Length;
    }
    public void Restore(int historyIndex)
    {
        _model.RestoreFromHistory(historyIndex);
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

    public void EditNode(string label)
    {
        EditNodeOnCaretSide(label, global::Quire.Direction.Indeterminate);
    }
    public void EditNode(string label, int caretPosition)
    {
        MathNode node = _model.Root.FindDescendant(label);
        _model.EditNode(node, caretPosition);
    }
    public void EditNodeOnCaretSide(string label, global::Quire.Direction caretSide)
    {
        MathNode node = _model.Root.FindDescendant(label);
        _model.EditNode(node, caretSide, Direction.Indeterminate);
    }
    public void EditNodeOnNodeSide(string label, global::Quire.Direction nodeSide)
    {
        MathNode node = _model.Root.FindDescendant(label);
        _model.EditNode(node, Direction.Indeterminate, nodeSide);
    }
    public void ScopeTo(string label, global::Quire.Direction direction)
    {
        MathNode node = _model.Root.FindDescendant(label);
        _model.SelectTo(node, Direction.Indeterminate, direction);
    }
    public void ScopeLhs()
    {
        MathNode mathNode = _model.Root?.Children?[0]?.Children?[0];
        if (mathNode != null)
        {
            _model.SelectTo(mathNode, Direction.Indeterminate, Direction.Left);
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
    public void Pi() => _view.AutomateKeypress('π');
    public void Keypress(string s)
    {
        foreach (char key in s)
        {
            _view.AutomateKeypress(key);
        }
    }

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
    public string[] HighlightedOperands
    {
        get
        {
            NearestNeighborSelector nearestNeighborSelector = new NearestNeighborSelector();
            MathNode mathNode = _model.NavigatorState.Current;
            if (mathNode.NoLogicOnlyVisual)
            {
                mathNode = mathNode.Parent;
            }
            MathNode[] neighbors = nearestNeighborSelector.SelectNeighbors(mathNode);
            string[] sExpressions = new string[neighbors.Length];
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (mathNode.IsInfix && i == 0 && neighbors[i].Parent != mathNode)
                {
                    sExpressions[i] = mathNode.LeftNeighborSExp;
                }
                else
                {
                    sExpressions[i] = neighbors[i].SExpression;
                }
            }
            return sExpressions;
        }
    }
    public bool CurrentNodeIsIdentifier => _model.CurrentModeNavigatorState.Current.DescriptorName == "IDENTIFIER";
    public bool CurrentNodeIsNumeric => _model.CurrentModeNavigatorState.Current.DescriptorName == "NUMERIC";
    public bool IsReadonly => _model.IsReadonly;
    public bool IsScoped => _model.Mode == EquationEditModeEnum.Scope;
    public bool IsTypeover => _model.Mode == EquationEditModeEnum.Typeover;
    public Direction CurrentNodeNodeSide
    {
        get
        {
            EquationEditModeEnum mode = _model.Mode;
            if (mode == EquationEditModeEnum.Normal || mode == EquationEditModeEnum.Typeover)
            {
                return _model.CurrentModeNavigatorState.NodeSide;
            }
            return _model.ScopeSide;
        }
    }
    public int CurrentNodeOperatorPartIndex
    {
        get
        {
            int? operatorPartIndex = null;
            EquationEditModeEnum mode = _model.Mode;
            if (mode == EquationEditModeEnum.Normal || mode == EquationEditModeEnum.Typeover)
            {
                operatorPartIndex = _model.CurrentModeNavigatorState.OperatorPartIndex;
            }
            return operatorPartIndex ?? -1;
        }
    }
    public Direction CurrentNodeCaretSide
    {
        get
        {
            EquationEditModeEnum mode = _model.Mode;
            if (mode == EquationEditModeEnum.Normal || mode == EquationEditModeEnum.Typeover)
            {
                return _model.CurrentModeNavigatorState.CaretSide;
            }
            return _model.ScopeSide;
        }
    }
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

    public string CurrentNodeSExpression
    {
        get
        {
            if (_model.Mode == EquationEditModeEnum.Scope)
            {
                return _model.ScopedSExpression;
            }
            MathNode mathNode = _model.CurrentModeNavigatorState.Current;
            if (mathNode.NoLogicOnlyVisual)
            {
                mathNode = mathNode.Parent;
            }
            if (mathNode.DescriptorName == "ARGUMENT_LIST")
            {
                return mathNode.Parent.SExpression;
            }
            return mathNode.SExpression;
        }
    }
    public TableCoordinate ActiveMatrixCell
    {
        get
        {
            MathNode current = _model.CurrentModeNavigatorState.Current;
            MathNode nearestAncestor = current.GetNearestAncestor("MATRIX");
            if (nearestAncestor == null)
            {
                throw new InvalidOperationException("Nice Try! Property is valid only for Matrices!! Nyaaahh!!!");
            }
            int matrixCellIndex = current.GetMatrixCellIndex(nearestAncestor);
            TableCoordinate result = default;
            int columns = ((MatrixNodeInfo)nearestAncestor.NodeInfo).ViewportSize.Columns;
            result.Row = global::Quire.Matrices.MatrixHelper.RowOf(matrixCellIndex, columns);
            result.Column = global::Quire.Matrices.MatrixHelper.ColumnOf(matrixCellIndex, columns);
            return result;
        }
    }
    public int MatrixRows
    {
        get
        {
            MathNode matrix = _model.CurrentModeNavigatorState.Current.GetNearestAncestor("MATRIX");
            if (matrix == null)
            {
                throw new InvalidOperationException("Nice Try! Property is valid only for Matrices!! Nyaaahh!!!");
            }
            return ((MatrixNodeInfo)matrix.NodeInfo).ViewportSize.Rows;
        }
    }
    public int MatrixColumns
    {
        get
        {
            MathNode matrix = _model.CurrentModeNavigatorState.Current.GetNearestAncestor("MATRIX");
            if (matrix == null)
            {
                throw new InvalidOperationException("Nice Try! Property is valid only for Matrices!! Nyaaahh!!!");
            }
            return ((MatrixNodeInfo)matrix.NodeInfo).ViewportSize.Columns;
        }
    }
    public void GetResultMatrixHeadingIndices(out string[] rowIndices, out string[] columnIndices)
    {
        global::Quire.EquationEditor.View.IVisualMathNode visualMathNode = null;
        foreach (global::Quire.EquationEditor.Tree.MathNode item in _model.Root.WalkSubTree())
        {
            if (item.DescriptorName == "MATRIX" || item.DescriptorName == "ScriptableControl1")
            {
                visualMathNode = _map.AssociateNode(item);
                break;
            }
        }
        if (visualMathNode != null)
        {
            global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrixNode = (global::Quire.EquationEditor.View.IVisualMatrixNode)visualMathNode;
            rowIndices = visualMatrixNode.MatrixHeadingRowIndices;
            columnIndices = visualMatrixNode.MatrixHeadingColumnIndices;
        }
        else
        {
            rowIndices = new string[0];
            columnIndices = new string[0];
        }
    }
    public void SetResultMatrixSize(int rows, int columns)
    {
        MatrixFormat matrixFormat = _model.ResultMatrixFormat.Clone();
        matrixFormat.Size = new TableSize(rows, columns);
        SetValue(ResultMatrixFormatProperty, matrixFormat);
    }
    public void SetResultMatrixOffset(int rows, int columns)
    {
        MatrixFormat matrixFormat = _model.ResultMatrixFormat.Clone();
        matrixFormat.Offset = new TableCoordinate(rows, columns);
        SetValue(ResultMatrixFormatProperty, matrixFormat);
    }
    public bool IsResultMatrixFormatIndexVisible()
    {
        global::Quire.EquationEditor.View.IVisualMathNode visualMathNode = null;
        foreach (MathNode item in _model.Root.WalkSubTree())
        {
            if (item.DescriptorName == "MATRIX" || item.DescriptorName == "ScriptableControl1")
            {
                visualMathNode = _map.AssociateNode(item);
                break;
            }
        }
        if (visualMathNode != null && ((global::Quire.EquationEditor.View.IVisualMatrixNode)visualMathNode).HeadingsVisibility == Visibility.Visible)
        {
            return true;
        }
        return false;
    }
    public global::System.Windows.Rect GetLabeledScreenInkBounds(string nodeLabel)
    {
        global::System.Windows.Rect result = global::System.Windows.Rect.Empty;
        global::Quire.EquationEditor.Tree.MathNode mathNode = _model.Root.FindDescendant(nodeLabel);
        if (mathNode != null)
        {
            result = _map.AssociateNode(mathNode).ScreenInkBounds;
        }
        return result;
    }

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

    private void UnhookFromModelEvents()
    {
        _view.Dispose();
        _model.RequestEvaluation -= OnModelRequestEvaluation;
        _model.RequestRemoveEquation -= FireRequestRemove;
        _model.RequestFocus -= FocusWhenAble;
        _model.EquationChanged -= OnModelEquationChanged;
        _model.RestrictedReadonlyChanged -= HandleModelIsRestrictedReadonlyChanged;
        _model.NavigatorStateChanged -= HandleNavigatorStateChanged;
        _model.ScopeChanged -= HandleScopeChanged;
        _model.TypeoverChanged -= HandleTypeoverChanged;
        _model.NumericResultChanged -= HandleNumericResultChanged;
        _model.ExpressionValueChanged -= FireExpressionValueChanged;
        _model.CalculationErrorChanged -= HandleCalculationErrorChanged;
        _model.WarningsOccurred -= HandleWarningOccured;
        _model.ContentChanged -= FireContentChangedEvents;
        _model.VisibleLhsOfDefinitionChanged -= HandleVisibleLhsOfDefinitionChanged;
    }
    public void DetachFromModel()
    {
        _view.Dispose();
        UnhookFromModelEvents();
    }

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

    private void HandleWarningOccured()
    {
        UpdateEngineResult(_model.Warnings?.CollectAsList(((RedefinitionWarning parsedWarning, MathNode warningNode) tuple) => tuple.parsedWarning.Original));
    }

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

    private void HandleScopeChanged(ScopeStateChangeEventArgs arg)
    {
        if (arg.ScopedNodes.ScopedLines.Count != 1)
        {
            CurrentIdLabel = null;
            CurrentDisplayIdLabel = null;
        }
        else
        {
            MathNode getFirstScopedNode = arg.ScopedNodes.GetFirstScopedNode;
            if (_model.ResolvedLabelOverridesIdLabel(getFirstScopedNode))
            {
                CurrentIdLabel = IdLabels.None;
                CurrentDisplayIdLabel = ((IdentifierNodeInfo)getFirstScopedNode.NodeInfo).ResolvedIdLabel;
            }
            else
            {
                CurrentIdLabel = MathNode.Utilities.IdLabelOfNode(getFirstScopedNode);
                CurrentDisplayIdLabel = MathNode.Utilities.DisplayIdLabelOfNode(getFirstScopedNode);
            }
        }
        if (!IsResizingMatrix)
        {
            MathNode node = MathNode.Utilities.FindCommonAncestor(arg.ScopedNodes.GetFirstScopedNode, arg.ScopedNodes.GetLastScopedNode);
            IList<TableCoordinate> nestedPositionForMatrix = GetNestedPositionForMatrix(node);
            if (!AreTableCoordinateListsEqual(CurrentNestedResultMatrixPosition, nestedPositionForMatrix))
            {
                CurrentNestedResultMatrixPosition = nestedPositionForMatrix;
            }
            CurrentMatrixFormat = GetFormatForMatrix(node);
        }
        MathNode mathNode = null;
        try
        {
            MathNode currentNode = SExpressionToMathNodeTransform.Default.Transform(_model.ScopedSExpression);
            mathNode = _model.ScopedNodesByLines.FirstOrDefault((MathNode mathNode2) => currentNode.SExpression == mathNode2.SExpression);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
        }
        if (mathNode != null)
        {
            BringCurrentNodeIntoView(mathNode);
        }
        IncreaseEquationWidthIfNessessary(_model.GetNavigatorStateFromScoperCaretSide());
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

    private void HandleSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size constraint) => base.MeasureOverride(constraint);
    private void FixCaretToRightMostPositionAfterClipping() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void NavigateLeftIfNeccessary()
    {
        if (!ContainsSymbolicEvaluation)
        {
            return;
        }
        if (_model.Mode != EquationEditModeEnum.Normal)
        {
            return;
        }
        if (XFromNavigatorState(_model.NavigatorState) > ViewClippedWidth)
        {
            FixCaretToRightMostPositionAfterClipping();
        }
    }
    private void IncreaseEquationWidthIfNessessary(NavigatorState navigatorState)
    {
        Action action = () =>
        {
            if (!IsShrunk)
            {
                return;
            }
            double xFromNavigator = XFromNavigatorState(navigatorState);
            if (xFromNavigator > ViewClippedWidth)
            {
                double leftOverhang = (ViewFullWidth - (_view.RootNode as FrameworkElement).ActualWidth) / 2.0;
                double rightOverhang = DesiredSize.Width - ViewClippedWidth;
                Width = xFromNavigator + leftOverhang + rightOverhang;
            }
        };
        ExecuteOnRootLoaded(action);
    }
    private double XFromNavigatorState(global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Nullable<double> MidYFromCurrentMathNode(global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState)
    {
        global::Quire.EquationEditor.Tree.MathNode current = navigatorState.Current;
        if (_map.AssociateNode(current) is global::Quire.Controls.Internal.VisualMathNode visualMathNode)
        {
            global::System.Windows.Point val = visualMathNode.TranslatePoint(new global::System.Windows.Point(0.0, visualMathNode.ActualHeight / 2.0), _view);
            return val.Y;
        }
        return null;
    }
    private void UpdateWidth(bool widthNotSet)
    {
        if (!IsArrangeValid)
        {
            return;
        }
        if (IsResizable && _model.EvaluationRoot != null)
        {
            global::Quire.EquationEditor.Tree.MathNode mathNode = _model.EvaluationRoot;
            while (mathNode[0].IsEvaluation)
            {
                mathNode = mathNode[0];
            }
            global::Quire.EquationEditor.Tree.MathNode modelNode = mathNode[mathNode.ChildCount - 1];
            global::Quire.Controls.Internal.VisualMathNode leftMostVisualResult = _map.AssociateNode(modelNode) as global::Quire.Controls.Internal.VisualMathNode;
            global::System.Action action = () =>
            {
                double num = leftMostVisualResult.TranslatePoint(new global::System.Windows.Point(0.0, 0.0), _view).X;
                double val2 = num + RecommendedSymbolicResultsWidth;
                MinWidth = num + ResizeIndicatorWidth;
                MaxWidth = ViewFullWidth + ResizeIndicatorWidth;
                if (widthNotSet)
                {
                    double val3 = 0.0;
                    if (_model.Mode == global::Quire.EquationEditor.EquationEditModeEnum.Normal)
                    {
                        val3 = XFromNavigatorState(_model.NavigatorState);
                    }
                    else if (_model.Mode == global::Quire.EquationEditor.EquationEditModeEnum.Scope)
                    {
                        val3 = XFromNavigatorState(_model.GetNavigatorStateFromScoperCaretSide());
                    }
                    Width = Math.Max(val3, val2) + ResizeIndicatorWidth;
                }
                else if (ViewClippedWidth + ResizeIndicatorWidth < Width)
                {
                    Width = ViewClippedWidth + ResizeIndicatorWidth;
                }
            };
            global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Controls.Core.IFrameworkElementFacade>().ExecuteOnLayoutUpdated(leftMostVisualResult, action);
        }
        else
        {
            MinWidth = IsScriptableControl ? MinWidth : 0.0;
            MaxWidth = double.PositiveInfinity;
            Width = double.NaN;
        }
    }
    internal double ViewFullWidth => _view.ActualWidth;
    internal double ViewClippedWidth => _view.DesiredSize.Width;
    private void BringCurrentNodeIntoView(MathNode currentNode)
    {
        if (IsKeyboardFocusWithin)
        {
            Quire.EquationEditor.View.IVisualMathNode visualMathNode = _map.AssociateNode(currentNode);
            FrameworkElement current = visualMathNode as FrameworkElement;
            if (current != null)
            {
                WpfUtils.ExecuteOnLoad(current, () =>
                {
                    WpfUtils.ExecuteOnLayoutUpdated(current, () =>
                    {
                        current.BringIntoView();
                    });
                });
            }
        }
    }
    private void OnRootLoaded()
    {
        RemoveMatrixResizeAdornerIfNotNeeded();
        bool widthNotSet = double.IsNaN(Width);
        AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLayoutUpdated(this, () =>
        {
            UpdateWidth(widthNotSet);
        });
        ExecuteOnRootSizeChanged(() =>
        {
            UpdateWidth(widthNotSet);
        });
    }
    private void _model_EquationChanged() => RaiseEvent(new RoutedEventArgs(ChangedEvent));
    private void RemoveMatrixResizeAdorner()
    {
        if (_matrixResizeAdorner != null)
        {
            _matrixResizeAdorner.Remove();
            _matrixResizeAdorner = null;
        }
    }
    private void RemoveMatrixResizeAdornerIfNotNeeded()
    {
        bool flag = true;
        if (_view.LhsOfDefinitionVisibility == Visibility.Visible && _matrixResizeAdorner?.MatrixPathFromRoot != null)
        {
            MathNode modelNode = _model.EquationRoot.NodeFromPath(_matrixResizeAdorner.MatrixPathFromRoot);
            if (_map.AssociateNode(modelNode) is global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrixNode)
            {
                UpdateMatrixForResizeAdorner(visualMatrixNode);
                flag = false;
            }
        }
        if (flag)
        {
            RemoveMatrixResizeAdorner();
        }
    }
    // Placed after BringCurrentNodeIntoView: closure ordinals in Release IL count this field there.
    private global::Quire.Controls.Core.NumberFormat _currentNumberFormat;
    public event global::System.Action RequestFocus;
    private global::System.Windows.Input.CommandBindingCollection _nonEditableCommandBindings;
    private global::System.Windows.Input.CommandBindingCollection _editableCommandBindings;

    private void FocusWhenAble(IEquationModel notUsed)
    {
        Activate(isActive: true);
        _view.FireActivated(setLogicalFocus: false);
        Action action = RequestFocus ?? (() =>
        {
            _view.Focus();
        });
        Quire.EquationEditor.View.IVisualNode viewRootNode = _view.RootNode;
        RoutedEventHandler loadedHandler = null;
        loadedHandler = delegate
        {
            action();
            viewRootNode.Loaded -= loadedHandler;
        };
        if (viewRootNode != null && !viewRootNode.IsLoaded)
        {
            viewRootNode.Loaded += loadedHandler;
        }
        else
        {
            action();
        }
    }

    private void FirePropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    internal void FireRequestEvaluation(bool force = true)
    {
        ClearExpressionValueModified();
        _model.ResultScalingModified = false;
        IEnumerable<ICalculatable> items = UpdateAggregateFromModel();
        if (force)
        {
            CalculatableChildren.Each((ICalculatable child) =>
            {
                child.Commit();
            });
            FireRequestEvaluationControlOnly();
            return;
        }
        items.Each((ICalculatable calc) =>
        {
            if (calc == this)
            {
                FireRequestEvaluationControlOnly();
            }
            else
            {
                calc.Commit();
            }
        });
    }

    internal void FireRequestEvaluationControlOnly()
    {
        RequestEvaluation?.Invoke(this);
    }

    private void FireRequestRemove(Direction direction)
    {
        RequestRemove.Invoke(this, direction);
    }

    public void InsertUnitMonomial(System.Collections.Generic.IEnumerable<Quire.Calculation.Core.Unit> unitMonomial) => _view.FireInsertUnitMonomial(unitMonomial);
    public override bool ShouldSerializeContent() => base.ShouldSerializeContent();
    protected override bool ShouldSerializeProperty(global::System.Windows.DependencyProperty dp) => base.ShouldSerializeProperty(dp);
    internal void SetNavigatorStateFromLabel(string label, global::Quire.Direction caretSide, global::Quire.Direction nodeSide)
    {
        global::Quire.EquationEditor.Tree.MathNode current = _model.Root.FindDescendant(label);
        global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState = new global::Quire.EquationEditor.Tree.Navigation.NavigatorState(current, caretSide, nodeSide);
        _model.NavigatorState = navigatorState;
    }
    internal void SetNavigatorStateFromLabel(string label, int caretPosition)
    {
        MathNode current = _model.Root.FindDescendant(label);
        global::Quire.EquationEditor.Tree.Navigation.NavigatorState navigatorState = new global::Quire.EquationEditor.Tree.Navigation.NavigatorState(current, caretPosition);
        _model.NavigatorState = navigatorState;
    }

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

    private void CanToggleVisibleLhsOfDefinition(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !IOTag.IsIOTagged(this) && _model.CanToggleVisibleLhsOfDefinition();
    }
    private readonly bool _isForEngineCalculation = true;
    private global::Quire.Controls.ControlAdorners.MatrixResizeAdorner _matrixResizeAdorner;

    private void BindEditingCommands() => CommandBindings.AddRange(EditableCommandBindings);

    private void UnBindEditingCommands()
    {
        foreach (System.Windows.Input.CommandBinding commandBinding in EditableCommandBindings)
        {
            CommandBindings.Remove(commandBinding);
        }
    }

    private void ChangeAlgorithm(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Algorithm = e.Parameter as string;
        ResetAlgorithmOptionIfNeeded();
        FireAlgorithmChanged();
    }
    private bool IsAlgorithmSupportOptions()
    {
        return EquationControlConstants.EnabledAlgorithmsForOptions.Contains(Algorithm);
    }
    private void ResetAlgorithmOptionIfNeeded()
    {
        if (!IsAlgorithmSupportOptions())
        {
            AlgorithmOptions.Reset();
        }
    }
    private void ToggleAlgorithmOption(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SymbolicEvalKeywordsStateUndoRedoSupport()
    {
        DelegateFunction0<bool> doMethod = () => true;
        Delegate0 undoMethod = () =>
        {
            SymbolicEvalKeywordsVisibilityState = !SymbolicEvalKeywordsVisibilityState;
            UpdateSymbolicEvalKeywordsVisibility(SymbolicEvalKeywordsVisibilityState);
        };
        CommandBuffer.Execute("SymbolicEvalKeywordsVisibilityState", doMethod, undoMethod, undoMethod);
    }
    private void ToggleSymbolicKeywordsVisibility(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        SymbolicEvalKeywordsVisibilityState = !SymbolicEvalKeywordsVisibilityState;
        SymbolicEvalKeywordsStateUndoRedoSupport();
        FireContentChangedEvents();
    }
    private void UpdateSymbolicEvalKeywordsVisibility(bool showHideKeywords)
    {
        if (!ContainsSymbolicEvaluation)
        {
            SymbolicEvalKeywordsVisibilityState = true;
            return;
        }
        global::Quire.EquationEditor.Tree.RootMathNode root = _model.Root;
        if (root == null)
        {
            return;
        }
        foreach (global::Quire.EquationEditor.Tree.MathNode item in root.WalkSubTree())
        {
            if (item != null && _map.AssociateNode(item) as global::Quire.Controls.Internal.VisualMathNode is global::Quire.Controls.Internal.VisualSymbolicEvaluationNode visualSymbolicEvaluationNode && visualSymbolicEvaluationNode.Children.Count > 2)
            {
                visualSymbolicEvaluationNode.Children[2].Visibility = showHideKeywords ? global::System.Windows.Visibility.Visible : global::System.Windows.Visibility.Collapsed;
            }
        }
    }
    private void SymbolicEvalKeywordsRegionActivatedEventHandler()
    {
        UpdateSymbolicEvalKeywordsVisibility(showHideKeywords: true);
    }
    private void SymbolicEvalKeywordsRegionDeactivatedEventHandler()
    {
        if (ErrorCount <= 0)
        {
            UpdateSymbolicEvalKeywordsVisibility(showHideKeywords: false);
        }
    }
    private void HandleSymCalculationErrorChanged(global::Quire.Wpf.PropertyChangedEventArgs<global::Quire.Calculation.Core.EngineError> eventArgs)
    {
        if (eventArgs.NewValue != null)
        {
            _view.SymbolicEvalKeywordsVisibilityState = true;
            UpdateSymbolicEvalKeywordsVisibility(showHideKeywords: true);
        }
        else
        {
            _view.SymbolicEvalKeywordsVisibilityState = false;
            UpdateSymbolicEvalKeywordsVisibility(showHideKeywords: false);
        }
        FireContentChangedEvents();
    }
    public bool SymbolicEvalKeywordsVisibilityState
    {
        get
        {
            return _isSymbolicEvalKeywordsShown;
        }
        set
        {
            if (_isSymbolicEvalKeywordsShown != value)
            {
                _isSymbolicEvalKeywordsShown = value;
                _view.SymbolicEvalKeywordsVisibilityState = _isSymbolicEvalKeywordsShown;
                if (!value)
                {
                    _view.Activated += SymbolicEvalKeywordsRegionActivatedEventHandler;
                    _view.Deactivated += SymbolicEvalKeywordsRegionDeactivatedEventHandler;
                    _model.CalculationErrorChanged += HandleSymCalculationErrorChanged;
                }
                else
                {
                    _view.Activated -= SymbolicEvalKeywordsRegionActivatedEventHandler;
                    _view.Deactivated -= SymbolicEvalKeywordsRegionDeactivatedEventHandler;
                    _model.CalculationErrorChanged -= HandleSymCalculationErrorChanged;
                }
            }
        }
    }
    private void ExecuteClearMathFormat(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ClearFormat();
    }
    private void ExecuteIncreaseFontSize(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.IncreaseFontSize();
    }
    private void ExecuteDecreaseFontSize(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.DecreaseFontSize();
    }
    private void ExecuteChangeFontColor(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeFontColor((global::System.Windows.Media.Color)e.Parameter);
    }
    private void ExecuteChangeHighlightColor(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeHighlightColor((global::System.Windows.Media.Color?)e.Parameter);
    }
    private void CanNavigateUp(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _model.CanNavigate(Direction.Up);
    }
    private void CanNavigateDown(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _model.CanNavigate(Direction.Down);
    }
    private void CanNavigateRight(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _model.CanNavigate(Direction.Right);
    }
    private void CanNavigateLeft(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _model.CanNavigate(Direction.Left);
    }
    private void CanClearResultFormat(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args)
    {
        args.CanExecute = global::Quire.Wpf.WpfUtils.HasLocalValue(this, DisplayNumberFormatProperty);
        args.Handled = true;
    }
    private void CanClearMathFormat(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args)
    {
        args.CanExecute = CurrentMathFormat.AreAnyPropertiesSet;
        args.Handled = true;
    }
    private void OnSetNumberFormatField(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnSetMatrixFormatField(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        if (((global::System.Windows.DependencyObject)this).ReadLocalValue(ResultMatrixFormatProperty).Equals(global::System.Windows.DependencyProperty.UnsetValue))
        {
            ResultMatrixFormat = ResultMatrixFormat != null ? ResultMatrixFormat.Clone() : new global::Quire.Controls.Core.MatrixFormat();
        }
        global::Quire.Controls.Core.SetPropertyValueCommandParameter setPropertyValueCommandParameter = (global::Quire.Controls.Core.SetPropertyValueCommandParameter)args.Parameter;
        ((global::Quire.Controls.Core.MatrixFormat)setPropertyValueCommandParameter.TargetObject).SetPropertyValueWithCommand(setPropertyValueCommandParameter);
    }
    private void OnToggleUnitIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireToggleUnitIdLabel();
    }
    private void OnToggleIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireToggleIdLabel();
    }
    private void OnUnsetIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireUnsetIdLabel();
    }
    private void OnSetIdLabel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireSetIdLabel((IdLabels)e.Parameter);
    }
    private void CanPerformIdLabelOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformIdLabelOperations(sender, e);
    private void OnToggleHideFromCalculationContext(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        HideFromCalculationContext = !HideFromCalculationContext;
    }
    private void OnInsertPi(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.OnPiCommand();
    }
    private void OnInsertInfinity(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.OnInfinityCommand();
    }
    private void OnCopy(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireCopy();
    }
    private void OnCut(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireCut();
    }
    private void OnPaste(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FirePaste();
    }
    private void CanCopy(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanCopy(sender, e);
    private void CanCut(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanCut(sender, e);
    private void CanPaste(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPaste(sender, e);
    private void OnBackspace(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireDeleteLeft();
        PostDeleteNodeAction();
    }
    private void OnContractScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireContractScope();
    }
    private void OnDelete(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireDeleteRight();
        PostDeleteNodeAction();
    }
    private void PostDeleteNodeAction()
    {
        if (!IsScriptableControl && IsResizableInputControl)
        {
            IsResizableInputControl = false;
        }
    }
    private void CanReplace(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _view.FireCanPerformReplace(e.Parameter as FindReplaceQuery);
    }
    private void OnReplace(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireReplace(e.Parameter as FindReplaceQuery);
    }
    private void OnExpandScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireExpandScope();
    }
    private void OnFullyContractScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireFullyContractScope();
    }
    private void OnFullyExpandScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireFullyExpandScope();
    }
    private void OnInsertOperator(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (!(e.Parameter is string nodeName))
        {
            return;
        }
        _view.FireInsertOperator(nodeName);
    }
    private void OnSwitchToLineBreakOperator(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs executedRoutedEventArgs)
    {
        _view.FireSwitchLineBreakOperator();
    }
    private void CanSwitchToLineBreakOperator(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        _view.FireCanSwitchToLineBreakOperator(sender, e);
    }
    private void OnInsertEvaluation(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireInsertEvaluation();
    }
    private void OnInsertSymbolicEvaluation(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireInsertSymbolicEvaluation();
    }
    private void OnInsertValue(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (!(e.Parameter is string text))
        {
            return;
        }
        string text2 = text;
        foreach (char c2 in text2)
        {
            _view.FireKeypress(c2);
        }
    }
    private void OnInsertId(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (!(e.Parameter is InsertIdCommandParameter insertIdCommandParameter) || insertIdCommandParameter.Name == null)
        {
            return;
        }
        _view.FireInsertId(insertIdCommandParameter.Name, insertIdCommandParameter.Label);
    }
    private void OnInsertString(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        string text = e.Parameter as string;
        _view.FireInsertString(text ?? string.Empty);
    }
    private void OpenComboBoxEditingMode(global::Quire.EquationEditor.Tree.MathNode mathNode)
    {
        DebugTools.DevAssert(mathNode == _model.CurrentModeNavigatorState.Current, null);
        global::Quire.EquationEditor.View.IVisualComboBoxControlNode visualComboBoxControlNode = _map.AssociateNode(mathNode) as global::Quire.EquationEditor.View.IVisualComboBoxControlNode;
        if (DebugTools.AssertNotNull(visualComboBoxControlNode, null).Passed)
        {
            visualComboBoxControlNode.OpenEditingOnComboBoxIsVisible();
        }
    }
    public void InputControlOnEquationActivated(object param) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnInsertInputControl(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is InputControlTypes inputControlTypes)
        {
            _view.FireInsertInputControl(inputControlTypes);
            MathNode current = _model.CurrentModeNavigatorState.Current;
            if (inputControlTypes == InputControlTypes.ComboBoxControl && _view.IsVisible)
            {
                OpenComboBoxEditingMode(current);
            }
        }
    }
    private void CanInsertInputControl(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        _view.FireCanInsertInputControl(sender, e);
    }
    private void OnInsertMatrix(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        TableSize? matrixSize = global::System.ComponentModel.TypeDescriptor.GetConverter(typeof(TableSize)).ConvertFrom(e.Parameter) as TableSize?;
        _view.FireInsertMatrix(matrixSize);
    }
    private void OnInsertFunction(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        InsertFunctionCommandParameter insertFunctionCommandParameter = e.Parameter as InsertFunctionCommandParameter;
        if (insertFunctionCommandParameter == null)
        {
            return;
        }
        if (insertFunctionCommandParameter.Args != null)
        {
            _view.FireInsertFunction(insertFunctionCommandParameter.Name, insertFunctionCommandParameter.ArgsCount, insertFunctionCommandParameter.Label, insertFunctionCommandParameter.Args);
        }
        else
        {
            _view.FireInsertFunction(insertFunctionCommandParameter.Name, insertFunctionCommandParameter.ArgsCount, insertFunctionCommandParameter.Label);
        }
    }
    private void CanInsertFunction(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs eArgs)
    {
        _view.FireCanInsertFunction(sender, eArgs);
    }
    private void OnInsertKeyword(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        string text = e.Parameter as string;
        if (text.IsNullOrEmpty())
        {
            return;
        }
        _view.FireInsertKeyword(text);
    }
    private void CanInsertKeywordOrModifier(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        _view.FireCanInsertKeyword(sender, e);
    }
    private void OnInsertModifier(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        string text = e.Parameter as string;
        if (text.IsNullOrEmpty())
        {
            return;
        }
        _view.FireInsertModifier(text);
    }
    private void CanPerformColumnOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformColumnOperations(sender, e);
    private void CanPerformCearCellsOperation(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        _view.FireCanPerformCearCellsOperation(sender, e);
    }
    private void OnInsertRow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Direction direction = Direction.Indeterminate;
        if (e.Parameter != null)
        {
            direction = (Direction)e.Parameter;
        }
        _view.FireInsertRow(direction);
    }
    private void OnInsertMatrixColumn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Direction direction = Direction.Indeterminate;
        if (e.Parameter != null)
        {
            direction = (Direction)e.Parameter;
        }
        _view.FireInsertMatrixColumn(direction);
    }
    private void OnDeleteRow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireDeleteRow();
    }
    private void OnDeleteMatrixColumn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireDeleteMatrixColumn();
    }
    private void OnClearCells(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireClearCells();
    }
    private void CanPerformLinearNaryOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformLinearNaryOperations(sender, e);
    private void CanPerformRowOperations(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanPerformRowOperations(sender, e);
    private void OnSplitLinearNaryLine(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (FunctionalityServiceWrapper.Instance.IsFunctionalityEnabled(global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.ProgrammingOperatorsSplitLinearNaryLineCommand))
        {
            _view.FireSplitLinearNaryLine();
        }
    }
    private void OnNavigateUp(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireNavigateUp();
    }
    private void OnNavigateDown(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireNavigateDown();
    }
    private void OnNavigateLeft(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveLeft();
    }
    private void OnNavigateLeftByTerm(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveLeftByTerm();
    }
    private void OnNavigateRight(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveRight();
    }
    private void OnNavigateRightByTerm(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveRightByTerm();
    }
    private void OnNavigateToEndOfScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveToEnd();
    }
    private void CanNavigateToOppositeOfScope(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) => _view.FireCanNavigateToOppositeOfScope(sender, e);
    private void OnNavigateToOppositeOfScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveToOpposite();
    }
    private void OnNavigateToEquationEnd(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveToRegionEnd();
    }
    private void OnNavigateToEquationStart(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveToRegionStart();
    }
    private void OnNavigateToStartOfScope(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireMoveToStart();
    }
    private void OnRegionHistory(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireShowRegionHistory(sender, e);
    }
    private void OnScopeLeft(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireScopeLeft();
    }
    private void OnScopeRight(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireScopeRight();
    }
    private void OnTransformCharacter(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireGreek();
    }
    private void OnToggleSubscript(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireToggleSubscript();
    }
    private void OnToggleVisibleLhsOfDefinition(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is Visibility paramVisibility)
        {
            _model.ToggleVisibleLhsOfDefinition(paramVisibility);
        }
    }

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

    private void UpdateLhsVisibleOnView()
    {
        bool isVisible = ErrorCount > 0 || _view.VisibleWarningMessage != null || (_view.IsLogicalFocusWithin && !_view.IsLogicalFocusWithinScriptableControl) || VisibleLhsOfDefinition == Visibility.Visible;
        bool canCollapse = ErrorCount <= 0 && _view.VisibleWarningMessage == null && (!_view.IsLogicalFocusWithin || _view.IsLogicalFocusWithinScriptableControl) && VisibleLhsOfDefinition == Visibility.Collapsed;
        Visibility visibility = isVisible ? Visibility.Visible : (canCollapse ? Visibility.Collapsed : Visibility.Hidden);
        if (_view.LhsOfDefinitionVisibility == visibility)
        {
            return;
        }
        _view.LhsOfDefinitionVisibility = visibility;
        if (_view.LhsOfDefinitionVisibility != Visibility.Visible)
        {
            RemoveMatrixResizeAdornerIfNotNeeded();
        }
        ReApplyBackground();
    }
    private void OnConvertToOperator(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        _view.FireConvertToOperator();
    }
    private void OnCommit(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is EquationCommands.CommitCommandParameter { IsAsynchronous: not false } commitCommandParameter)
        {
            if (commitCommandParameter.IsForceControlOnly)
            {
                Dispatcher.BeginInvoke(global::System.Windows.Threading.DispatcherPriority.Normal, new global::System.Action(FireRequestEvaluationControlOnly));
            }
            else if (commitCommandParameter.IsForce)
            {
                Dispatcher.BeginInvoke(global::System.Windows.Threading.DispatcherPriority.Normal, new global::System.Action(Commit));
            }
            else
            {
                Dispatcher.BeginInvoke(global::System.Windows.Threading.DispatcherPriority.Normal, new global::System.Action<bool>(FireRequestEvaluation), commitCommandParameter.IsForce);
            }
        }
        else
        {
            Commit();
        }
    }
    private void CanTraverseForward(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        _view.FireCanTraverse(e, (FocusNavigationDirection)0);
    }
    private void CanTraverseBackward(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        _view.FireCanTraverse(e, (FocusNavigationDirection)1);
    }
    private void OnTraverseForward(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) => OnTraverse(e, Direction.Right);
    private void OnTraverseBackward(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) => OnTraverse(e, Direction.Left);
    private void OnTraverse(System.Windows.Input.ExecutedRoutedEventArgs e, Direction direction) => _view.FireTraversalRequest(e, direction);
    protected void OnClearResultFormat(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        ClearResultFormat();
    }
    public global::System.TypeCode GetTypeCode()
    {
        return global::System.TypeCode.Object;
    }
    public bool ToBoolean(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public char ToChar(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public sbyte ToSByte(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public byte ToByte(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public short ToInt16(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public ushort ToUInt16(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public int ToInt32(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public uint ToUInt32(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public long ToInt64(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public ulong ToUInt64(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public float ToSingle(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public double ToDouble(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public decimal ToDecimal(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public global::System.DateTime ToDateTime(global::System.IFormatProvider provider)
    {
        throw new NotImplementedException();
    }
    public string ToString(global::System.IFormatProvider provider)
    {
        return EquationSExpression;
    }
    public object ToType(global::System.Type conversionType, global::System.IFormatProvider provider)
    {
        if (conversionType == _model.GetType())
        {
            return _model;
        }
        if (conversionType.IsInstanceOfType(this))
        {
            return this;
        }
        return null;
    }
    public void ClearKeyStrokesCache()
    {
        _view.ClearKeyStrokesCache();
        if (!_model.NavigatorState.Equals(global::Quire.EquationEditor.Tree.Navigation.NavigatorState.Empty))
        {
            _model.FireNavigatorStateChanged();
        }
    }
    public void ClearResultFormat()
    {
        global::Quire.Controls.Core.NumberFormat oldValue = DisplayNumberFormat.Clone();
        DelegateFunction0<bool> doMethod = () =>
        {
            ((global::System.Windows.DependencyObject)this).ClearValue(DisplayNumberFormatProperty);
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            DisplayNumberFormat = oldValue;
        };
        CommandBuffer.Execute("Clear DisplayNumberFormat - EQUATION CONTROL", doMethod, undoMethod);
    }
    public bool HighlightAll(string text, global::Quire.Controls.Core.FindOptions options)
    {
        throw new NotImplementedException();
    }
    public bool FindOncePrivate(string text, bool startFromCaret, global::System.Windows.Documents.LogicalDirection direction, global::Quire.Controls.Core.FindOptions options, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions)
    {
        if (!options.HasFlag(global::Quire.Controls.Core.FindOptions.FindInMath))
        {
            return false;
        }
        bool matchCase = options.HasFlag(global::Quire.Controls.Core.FindOptions.MatchCase);
        bool wholeWordOnly = options.HasFlag(global::Quire.Controls.Core.FindOptions.WholeWordOnly);
        bool found = _model.Find(text, startFromCaret, direction, matchCase, wholeWordOnly, subscriptPositions);
        if (found)
        {
            Activate(isActive: true);
        }
        return found;
    }
    public virtual string GetHelpContext()
    {
        string text = "EquationControl.";
        if (_model.CurrentModeNavigatorState.Current.IsIdentifier || _model.CurrentModeNavigatorState.Current.IsLiteral)
        {
            string text2 = _model.CurrentModeNavigatorState.Current.DescriptorName[0].ToString().ToUpper();
            text2 += _model.CurrentModeNavigatorState.Current.DescriptorName.Substring(1).ToLower();
            text = text + text2 + "." + _model.CurrentModeNavigatorState.Current.NodeInfoValueAsString;
        }
        else
        {
            text = text + "Operator." + _model.CurrentModeNavigatorState.Current.DescriptorName;
        }
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(text, "\\.").NextMatch();
        string value = text.Substring(match.Index + 1);
        if (CurrentNodeLabel != null && EquationControlConstants.specialIdentifiers.Contains(value) && _unitLabel == CurrentNodeLabel.ToLower())
        {
            text = text.Insert(match.Index + 1, global::System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_unitLabel) + ".");
        }
        return text;
    }
    public string GetCurrentNodeIdentifier()
    {
        if (!_model.CurrentModeNavigatorState.Current.IsIdentifier)
        {
            return null;
        }
        string helpContext = GetHelpContext();
        DebugTools.AssertNotNull(helpContext.StartsWith(EquationControlConstants.GetEquationIdentifierHelpContext()), null);
        return helpContext.Substring(EquationControlConstants.GetEquationIdentifierHelpContext().Length + 1);
    }
    internal global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IVisualComboBoxControlNode> GetAllComboBoxControlsFromEquation()
    {
        return _map.AllVisualNodesOfType<global::Quire.EquationEditor.View.IVisualComboBoxControlNode>();
    }
    public int WarningsCount => _model.Warnings?.Count ?? 0;
    internal List<(RedefinitionWarning parsedWarning, MathNode warningNode, global::Quire.EquationEditor.View.IVisualMathNode visualMathNode)> Warnings => _model.Warnings?.Select(((RedefinitionWarning parsedWarning, MathNode warningNode) warning) => (parsedWarning: warning.parsedWarning, warningNode: warning.warningNode, _map.AssociateNode(warning.warningNode))).ToList();

    internal global::Quire.EquationEditor.View.IVisualMathNode CurrentVisualMathNode()
    {
        return _map.AssociateNode(CurrentNode);
    }
    public bool ContainsError(global::Quire.Calculation.Core.EngineError error)
    {
        if (!Equals(error.ErrorSource))
        {
            return CalculatableChildren.Contains(error.ErrorSource);
        }
        return true;
    }
    public void ShowError(global::Quire.Calculation.Core.EngineError error)
    {
        if (!IsKeyboardFocusWithin)
        {
            Activate(isActive: true);
        }
        if (Error == null)
        {
            error.IsRuntime = true;
            if (Equals(error.ErrorSource))
            {
                SetError(error);
                return;
            }
            ICalculatable errorSourceChild = CalculatableChildren.FirstOrDefault(child => child.Equals(error.ErrorSource));
            if (errorSourceChild != null)
            {
                errorSourceChild.SetError(error);
            }
        }
    }
    public EngineError CurrentError => Error;
    public bool IsMatrixResized
    {
        get
        {
            MathNode mathNode = _model.CurrentModeNavigatorState.Current;
            if (mathNode.DescriptorName != "MATRIX")
            {
                mathNode = mathNode.GetNearestAncestor("MATRIX");
            }
            if (mathNode != null)
            {
                MatrixNodeInfo matrixNodeInfo = (MatrixNodeInfo)mathNode.NodeInfo;
                if (!matrixNodeInfo.IsInResult)
                {
                    return matrixNodeInfo.IsResized;
                }
            }
            throw new InvalidOperationException("Property is valid only for Matrices!");
        }
    }
    public int[] VisibleMatrixCellRange
    {
        get
        {
            MathNode mathNode = _model.CurrentModeNavigatorState.Current;
            if (mathNode.DescriptorName != "MATRIX")
            {
                mathNode = mathNode.GetNearestAncestor("MATRIX");
            }
            if (mathNode != null)
            {
                MatrixNodeInfo matrixNodeInfo = (MatrixNodeInfo)mathNode.NodeInfo;
                if (!matrixNodeInfo.IsInResult)
                {
                    return new int[4]
                    {
                        matrixNodeInfo.ViewportOffset.Row,
                        matrixNodeInfo.ViewportOffset.Column,
                        matrixNodeInfo.ViewportOffset.Row + matrixNodeInfo.ViewportSize.Rows - 1,
                        matrixNodeInfo.ViewportOffset.Column + matrixNodeInfo.ViewportSize.Columns - 1
                    };
                }
            }
            throw new InvalidOperationException("Property is valid only for Matrices!");
        }
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

    public global::System.Collections.Generic.IList<string> GetDefinedIdentifierSExpressions()
    {
        return _model.GetDefinedIdentifierSExpressions(includeFunctions: true);
    }
    public string GetEvalIdentifierSExpression()
    {
        return string.Empty;
    }
    public event global::System.Action<global::Quire.Calculation.Core.IResultNode> ResultChanged;

    private void HandleNumericResultChanged(IResultNode resultValue)
    {
        UpdateEngineResult(resultValue);
        ResultChanged?.Invoke(resultValue);
    }

    public double GetRealResult()
    {
        return _model.GetRealResult();
    }
    public double GetRealUnitedResult(string unitsOfResult)
    {
        throw new NotImplementedException();
    }
    public void ActivateAndScopeResultPrivate()
    {
        _model.ScopeResult();
        Activate(isActive: true);
    }
    public string GetEvaluationSExp(bool getDefineForDefineEval)
    {
        return _model.GetEvaluationSExp(getDefineForDefineEval);
    }
    public event global::System.Action<global::System.Windows.DependencyObject, global::Quire.Controls.Core.IORegions.IORegionChangeType> IORegionChanged;
    public global::System.Windows.FrameworkElement GetAdornedElement()
    {
        return _view;
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

    internal global::Quire.Controls.Core.MatrixFormat GetFormatForVisualMatrix(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix)
    {
        global::Quire.EquationEditor.Tree.MathNode node = _map.AssociateNode(visualMatrix);
        return GetFormatForMatrix(node);
    }
    internal void SetMatrixForResizeAdorner(global::Quire.EquationEditor.View.IVisualMatrixNode matrix)
    {
        if (!IsScriptableControl && DebugTools.DevAssert(_view.LhsOfDefinitionVisibility == Visibility.Visible, null).Failed)
        {
            return;
        }
        MathNode mathNode = _map.AssociateNode(matrix);
        if (mathNode == null || mathNode.IsInSymbolicResult)
        {
            return;
        }
        VisualMatrixNode visualMatrixNode = matrix as VisualMatrixNode;
        if (_matrixResizeAdorner == null)
        {
            _matrixResizeAdorner = new global::Quire.Controls.ControlAdorners.MatrixResizeAdorner(this, visualMatrixNode);
            _matrixResizeAdorner.CanActivate = AttachedProperties.GetCanActivate(this);
        }
        else if (!_matrixResizeAdorner.VisualMatrix.Equals(visualMatrixNode) && (!_matrixResizeAdorner.VisualMatrix.IsMouseOver || !visualMatrixNode.IsAncestorOf(_matrixResizeAdorner.VisualMatrix)))
        {
            _matrixResizeAdorner.VisualMatrix = visualMatrixNode;
        }
    }
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

    internal global::System.Collections.Generic.IList<int> GetMatrixPathFromRoot(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix)
    {
        return _map.AssociateNode(visualMatrix).PathFromEquationRoot();
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

    internal global::System.Collections.Generic.IList<global::Quire.TableCoordinate> GetNestedPositionForVisualMatrix(global::Quire.EquationEditor.View.IVisualMatrixNode visualMatrix)
    {
        global::Quire.EquationEditor.Tree.MathNode node = _map.AssociateNode(visualMatrix);
        return GetNestedPositionForMatrix(node);
    }
    private void CreateOrClearTemporaryResultMatrixFormat(bool create)
    {
        if (create)
        {
            CreateTemporaryResultMatrixFormat();
        }
        else
        {
            ClearTemporaryResultMatrixFormat();
        }
    }
    private void ClearTemporaryResultMatrixFormat()
    {
        if (!_temporaryResultMatrixFormatShouldClear)
        {
            return;
        }
        using (new CommandScope(new CompoundCommand()))
        {
            if (_nestedMatrixPositionWhichFormatShouldClear.IsNotEmpty())
            {
                MatrixFormat matrixFormat = ResultMatrixFormat;
                int count = _nestedMatrixPositionWhichFormatShouldClear.Count;
                for (int num = 0; num < count - 1; num++)
                {
                    matrixFormat = matrixFormat.NestedMatrixFormats[_nestedMatrixPositionWhichFormatShouldClear[num]];
                }
                matrixFormat.NestedMatrixFormats[_nestedMatrixPositionWhichFormatShouldClear[count - 1]] = matrixFormat.NestedMatrixFormats.EmptyPosition;
            }
            else
            {
                ClearValue(ResultMatrixFormatProperty);
            }
        }
    }
    private void CreateTemporaryResultMatrixFormat()
    {
        if (ResultMatrixFormat == null)
        {
            return;
        }
        using (new CommandScope(new CompoundCommand()))
        {
            ClearTemporaryResultMatrixFormat();
            bool flag = false;
            if (ReadLocalValue(ResultMatrixFormatProperty).Equals(DependencyProperty.UnsetValue))
            {
                ResultMatrixFormat = ResultMatrixFormat.Clone();
                _temporaryResultMatrixFormatShouldClear = true;
                flag = true;
                _nestedMatrixPositionWhichFormatShouldClear = null;
            }
        IList<global::Quire.TableCoordinate> currentNestedResultMatrixPosition = CurrentNestedResultMatrixPosition;
        if (currentNestedResultMatrixPosition == null)
            {
                return;
            }
        IList<global::Quire.TableCoordinate> list = new List<global::Quire.TableCoordinate>();
        MatrixFormat matrixFormat = ResultMatrixFormat;
        foreach (global::Quire.TableCoordinate item in currentNestedResultMatrixPosition)
            {
                if (!flag)
                {
                    list.Add(item);
                }
                MatrixFormat matrixFormat2 = matrixFormat.NestedMatrixFormats[item];
                if (matrixFormat2 == matrixFormat.NestedMatrixFormats.EmptyPosition)
                {
                    if (!flag)
                    {
                        flag = true;
                        _nestedMatrixPositionWhichFormatShouldClear = list;
                    }
                    matrixFormat2 = new MatrixFormat
                    {
                        Size = matrixFormat.Size,
                        Offset = matrixFormat.Offset,
                        Origin = matrixFormat.Origin,
                        ShowIndices = matrixFormat.ShowIndices,
                        CollapseNestedMatrices = matrixFormat.CollapseNestedMatrices
                    };
                    matrixFormat.NestedMatrixFormats[item] = matrixFormat2;
                    _temporaryResultMatrixFormatShouldClear = true;
                }
                matrixFormat = matrixFormat2;
            }
        }
    }
    public void SetVisibleMatrixCellRange(int startRow, int startCol, int endRow, int endCol)
    {
        global::Quire.EquationEditor.Tree.MathNode mathNode = _model.CurrentModeNavigatorState.Current;
        if (mathNode.DescriptorName != "MATRIX")
        {
            mathNode = mathNode.GetNearestAncestor("MATRIX");
        }
        if (mathNode != null)
        {
            global::Quire.EquationEditor.Tree.MatrixNodeInfo matrixNodeInfo = (global::Quire.EquationEditor.Tree.MatrixNodeInfo)mathNode.NodeInfo;
            if (!matrixNodeInfo.IsInResult)
            {
                global::Quire.TableCoordinate viewportOffset = matrixNodeInfo.ViewportOffset;
                global::Quire.TableSize viewportSize = new global::Quire.TableSize(endRow - startRow + 1, endCol - startCol + 1);
                global::Quire.TableCoordinate viewportOffset2 = new global::Quire.TableCoordinate(startRow, startCol);
                if (startRow > viewportOffset.Row || startCol > viewportOffset.Column)
                {
                    matrixNodeInfo.ViewportOffset = new global::Quire.TableCoordinate(startRow > viewportOffset.Row ? startRow : viewportOffset.Row, startCol > viewportOffset.Column ? startCol : viewportOffset.Column);
                }
                matrixNodeInfo.ViewportSize = viewportSize;
                matrixNodeInfo.ViewportOffset = viewportOffset2;
                return;
            }
        }
        throw new InvalidOperationException("Property is valid only for Matrices!");
    }
    public global::Quire.Calculation.Core.EngineResult EngineResultForSerialization
    {
        get
        {
            global::Quire.Calculation.Core.IResultNode resultNode = ConvertAnyFormattedMatrixToDisplayMatrix(
                (DataExchangeModel is global::Quire.Calculation.Core.IDataExchangeScriptingModel scriptingModel && EngineResult.Errors == null)
                    ? scriptingModel.EngineResultForSerialization
                    : EngineResult.ResultNode,
                ResultMatrixFormat ?? new global::Quire.Controls.Core.MatrixFormat(),
                true);
            if (resultNode == EngineResult.ResultNode)
            {
                return EngineResult;
            }
            return new global::Quire.Calculation.Core.EngineResult(resultNode, EngineResult.Errors, EngineResult.Warnings, EngineResult.SynchronizationStatus);
        }
        set
        {
            _model.EngineResultForSerialization = value.ResultNode;
            EngineResult = value;
        }
    }
    private static bool IsUnitedMatrixUniform(global::Quire.Calculation.Core.IUnitedValueResultNode unitedElement)
    {
        if (unitedElement.Value is LazyMatrixSimpleResultNode lazyMatrixSimpleResultNode)
        {
            return lazyMatrixSimpleResultNode.MatrixProxy.IsUniform;
        }
        return false;
    }
    private static global::Quire.Calculation.Core.IResultNode ConvertAnyFormattedMatrixToDisplayMatrix(global::Quire.Calculation.Core.IResultNode resultNode, global::Quire.Controls.Core.MatrixFormat matrixFormat, bool useEagerMatrixForSmallMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    // Measured: closure ordinals count these fields after ConvertAnyFormattedMatrixToDisplayMatrix, not before ShowError.
    private bool _temporaryResultMatrixFormatShouldClear;
    private global::System.Collections.Generic.IList<global::Quire.TableCoordinate> _nestedMatrixPositionWhichFormatShouldClear;
    private static global::Quire.Calculation.Core.IMatrixResultNode ConvertToDisplayMatrix(global::Quire.Calculation.Core.IMatrixResultNode originalMatrix, global::Quire.Controls.Core.MatrixFormat matrixFormat, bool useEagerMatrixForSmallMatrix)
    {
        if (useEagerMatrixForSmallMatrix)
        {
            int maxCells = 2500;
            bool isSmall = originalMatrix.Rows * originalMatrix.Columns <= maxCells;
            if (isSmall)
            {
                return ConvertToEagerMatrix(originalMatrix, matrixFormat, useEagerMatrixForSmallMatrix);
            }
        }
        int rows = originalMatrix.Rows;
        int columns = originalMatrix.Columns;
        TableCoordinate offset = matrixFormat.Offset;
        TableSize size = matrixFormat.Size;
        if (offset.Row >= rows)
        {
            offset.Row = rows - 1;
        }
        if (offset.Column >= columns)
        {
            offset.Column = columns - 1;
        }
        if (offset.Row + size.Rows > rows)
        {
            size.Rows = rows - offset.Row;
        }
        if (offset.Column + size.Columns > columns)
        {
            size.Columns = columns - offset.Column;
        }
        IDisplayMatrixResultNode displayMatrixResultNode = new SimpleResultNodeFactory().CreateDisplayMatrixResultNode(rows, columns, offset, size);
        for (int i = 0; i < size.Rows; i++)
        {
            for (int j = 0; j < size.Columns; j++)
            {
                IResultNode displayMatrixCell = GetDisplayMatrixCell(originalMatrix, matrixFormat, i + offset.Row, j + offset.Column, useEagerMatrixForSmallMatrix);
                displayMatrixResultNode.Viewport.SetElementAt(i, j, displayMatrixCell);
            }
        }
        return displayMatrixResultNode;
    }
    private static global::Quire.Calculation.Core.IResultNode GetDisplayMatrixCell(global::Quire.Calculation.Core.IMatrixResultNode matrix, global::Quire.Controls.Core.MatrixFormat matrixFormat, int r, int c, bool useEagerMatrixForSmallMatrix)
    {
        global::Quire.Calculation.Core.IResultNode resultNode = matrix[r, c];
        global::Quire.Controls.Core.MatrixFormat matrixFormat2 = matrixFormat.NestedMatrixFormats[r, c];
        if (matrixFormat2 != matrixFormat.NestedMatrixFormats.EmptyPosition)
        {
            return ConvertAnyFormattedMatrixToDisplayMatrix(resultNode, matrixFormat2, useEagerMatrixForSmallMatrix);
        }
        return resultNode;
    }
    private static global::Quire.Calculation.Core.IMatrixResultNode ConvertToEagerMatrix(global::Quire.Calculation.Core.IMatrixResultNode originalMatrix, global::Quire.Controls.Core.MatrixFormat matrixFormat, bool useEagerMatrixForSmallMatrix)
    {
        global::Quire.Calculation.Core.IEagerMatrixResultNode eagerMatrixResultNode = new global::Quire.Calculation.Core.SimpleResultNodeFactory().CreateEagerMatrixResultNode(originalMatrix.Rows, originalMatrix.Columns);
        for (int row = 0; row < eagerMatrixResultNode.Rows; row++)
        {
            for (int column = 0; column < eagerMatrixResultNode.Columns; column++)
            {
                global::Quire.Calculation.Core.IResultNode displayMatrixCell = GetDisplayMatrixCell(originalMatrix, matrixFormat, row, column, useEagerMatrixForSmallMatrix);
                eagerMatrixResultNode.SetElementAt(row, column, displayMatrixCell);
            }
        }
        return eagerMatrixResultNode;
    }
    public void HandleMathFormatChanged(global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == global::Quire.Controls.Core.MathFormatAttachedProperties.FontColorProperty && IsMathFormatHandledLocally)
        {
            if (MathFormat.IsFontColorSet)
            {
                SetOverrideLabelColor((global::System.Windows.DependencyObject)Content, val: true);
            }
            else
            {
                ClearOverrideLabelColor((global::System.Windows.DependencyObject)Content);
            }
        }
        if (e.Property == global::Quire.Controls.Core.MathFormatAttachedProperties.HighlightColorProperty)
        {
            ReApplyBackground();
        }
        ((global::System.Windows.DependencyObject)Content).SetValue(e.Property, e.NewValue);
        FirePropertyChanged(new global::System.ComponentModel.PropertyChangedEventArgs(e.Property.Name));
        FireContentChangedEvents();
    }
    private void ReApplyBackground()
    {
        Background = _view.LhsOfDefinitionVisibility == global::System.Windows.Visibility.Visible
            ? new global::System.Windows.Media.SolidColorBrush(MathFormat.HighlightColor.GetValueOrDefault())
            : global::System.Windows.Media.Brushes.Transparent;
        ((global::System.Windows.Freezable)Background).Freeze();
    }
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

    public bool AddToDataObject(global::System.Windows.IDataObject dataObject)
    {
        return WpfClipboard.CopySExpressionToDataObject(dataObject, _model.EquationSExpression);
    }
    public void Accept(global::Quire.ISpecialCopyVisitor visitor)
    {
        visitor.Visit(this);
    }
    public string GetEquationSizesAsString_ForTestsOnly(int numDecimalPlaces)
    {
        return SizeList2String.WriteWrappedSizesToString(GetEquationSizes_ForTestsOnly(), numDecimalPlaces);
    }
    public global::System.Collections.Generic.List<global::System.Windows.Size> GetEquationSizes_ForTestsOnly()
    {
        DebugTools.DevAssert(DesiredSize == RenderSize, null);
        List<Size> list = new List<Size>();
        list.Add(RenderSize);
        string text = CollectSizes_ForTestsOnly(_view, list);
        // The IL formats this message with String.Format rather than a concatenation.
        DebugTools.DevAssert(text == null, string.Format("Sexpr: {0} has wrong measure: {1}", EquationSExpression, text));
        return list;
    }
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
    private static void HandleCalcIsDisabledChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (d is EquationControl equationControl)
        {
            equationControl.FireContentAndRegionChangedEvents(IORegionChangeType.CalcIsDisabledStatus);
        }
    }

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

    public static bool GetOverrideLabelColor(global::System.Windows.DependencyObject obj)
    {
        return (bool)obj.GetValue(MathFormatAttachedProperties.OverrideLabelColorProperty);
    }
    public static void SetOverrideLabelColor(global::System.Windows.DependencyObject obj, bool val)
    {
        obj.SetValue(MathFormatAttachedProperties.OverrideLabelColorProperty, (object)val);
    }
    public static void ClearOverrideLabelColor(global::System.Windows.DependencyObject obj)
    {
        obj.ClearValue(MathFormatAttachedProperties.OverrideLabelColorProperty);
    }
    public static global::System.Windows.Thickness GetRootPadding(global::System.Windows.DependencyObject obj)
    {
        return (global::System.Windows.Thickness)obj.GetValue(RootPaddingProperty);
    }
    public static void SetRootPadding(global::System.Windows.DependencyObject obj, object val)
    {
        obj.SetValue(RootPaddingProperty, val);
    }

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

    private static void HandleCanActivateChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (!(d is EquationControl equationControl))
        {
            return;
        }
        if (equationControl._matrixResizeAdorner != null)
        {
            equationControl._matrixResizeAdorner.CanActivate = (bool)e.NewValue;
        }
    }
    private static void HandleSynchronizationStatusChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args)
    {
        if (d is EquationControl equationControl)
        {
            equationControl.FirePropertyChanged(SyncStatusChangedEventArgs.GetArgs());
        }
    }
    private static void HandleCalculatingStatusChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args)
    {
        if (d is EquationControl equationControl)
        {
            equationControl.FirePropertyChanged(CalculatingStatusChangedEventArgs.GetArgs());
        }
    }
    private static void HandleDisplayNumberFormatChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args)
    {
        global::Quire.Controls.Core.NumberFormat numberFormat = args.OldValue as global::Quire.Controls.Core.NumberFormat;
        global::Quire.Controls.Core.NumberFormat numberFormat2 = args.NewValue as global::Quire.Controls.Core.NumberFormat;
        if (d is EquationControl equationControl)
        {
            if (numberFormat != null)
            {
                numberFormat.FormatChanged -= equationControl.HandleResultFormattingChanged;
            }
            if (numberFormat2 != null)
            {
                numberFormat2.FormatChanged += equationControl.HandleResultFormattingChanged;
            }
            equationControl.HandleResultFormattingChanged(string.Empty, null);
            if (equationControl.EquationType == EquationTypes.Evaluation || equationControl.EquationType == EquationTypes.DefinitionToEvaluatedResult)
            {
                equationControl.CurrentNumberFormat = equationControl.DisplayNumberFormat;
            }
        }
    }
    private void HandleResultFormattingChanged(string propertyName, object newValue)
    {
        global::Quire.Controls.Core.NumberFormat numberFormat = (global::Quire.Controls.Core.NumberFormat)((global::System.Windows.DependencyObject)this).GetValue(DisplayNumberFormatProperty);
        if (numberFormat != null && numberFormat != _model.ResultNumberFormat)
        {
            _model.ResultNumberFormat = numberFormat;
        }
    }
    private void HandleResultMatrixFormattingChanged(string propertyName, object newValue)
    {
        _temporaryResultMatrixFormatShouldClear = false;
        global::Quire.Controls.Core.MatrixFormat matrixFormat = (global::Quire.Controls.Core.MatrixFormat)((global::System.Windows.DependencyObject)this).GetValue(ResultMatrixFormatProperty);
        if (matrixFormat != null && matrixFormat != _model.ResultMatrixFormat)
        {
            _model.ResultMatrixFormat = matrixFormat;
        }
    }
    private static void HandleResultMatrixFormatChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs args)
    {
        global::Quire.Controls.Core.MatrixFormat matrixFormat = args.OldValue as global::Quire.Controls.Core.MatrixFormat;
        global::Quire.Controls.Core.MatrixFormat matrixFormat2 = args.NewValue as global::Quire.Controls.Core.MatrixFormat;
        if (d is EquationControl equationControl)
        {
            if (matrixFormat != null)
            {
                matrixFormat.RemoveFormatChangedHandlerFromNestedMatrixFormat(equationControl.HandleResultMatrixFormattingChanged);
            }
            if (matrixFormat2 != null)
            {
                matrixFormat2.AddFormatChangedHandlerToNestedMatrixFormat(equationControl.HandleResultMatrixFormattingChanged);
                if (matrixFormat2 != equationControl._model.ResultMatrixFormat)
                {
                    equationControl._model.ResultMatrixFormat = matrixFormat2;
                }
            }
            equationControl.CurrentMatrixFormat = equationControl.ResultMatrixFormat;
        }
    }
    private static void HandleLabeledIdFormatChanged(global::System.Windows.DependencyObject depObj, global::System.Windows.DependencyPropertyChangedEventArgs args)
    {
        ((global::System.Windows.DependencyObject)((EquationControl)(object)depObj)._view).SetValue(args.Property, args.NewValue);
    }
    private static void HandleIOTagPropertyChanged(global::System.Windows.DependencyObject depObj, global::System.Windows.DependencyPropertyChangedEventArgs args)
    {
        global::Quire.Controls.Core.IORegions.IOTagType iOTagType = (global::Quire.Controls.Core.IORegions.IOTagType)args.OldValue;
        global::Quire.Controls.Core.IORegions.IOTagType iOTagType2 = (global::Quire.Controls.Core.IORegions.IOTagType)args.NewValue;
        bool flag = iOTagType.HasFlag(global::Quire.Controls.Core.IORegions.IOTagType.Input);
        bool flag2 = iOTagType2.HasFlag(global::Quire.Controls.Core.IORegions.IOTagType.Input);
        if (flag != flag2)
        {
            ((EquationControl)(object)depObj)._view.FireInputTagChanged(flag2);
        }
    }
    private static void HandleCurrentNestedResultMatrixPositionChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (!(d is EquationControl equationControl))
        {
            return;
        }
        if (e.OldValue != null)
        {
            equationControl.ClearTemporaryResultMatrixFormat();
        }
        if (e.NewValue != null)
        {
            equationControl.CreateTemporaryResultMatrixFormat();
        }
    }
    private static void IsSelectedChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (d is EquationControl && e.NewValue is bool)
        {
            (d as EquationControl).IsSelectedChanged((bool)e.NewValue);
        }
    }
    private static void IsActiveChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (d is EquationControl && e.NewValue is bool)
        {
            (d as EquationControl).IsActiveChanged((bool)e.NewValue);
        }
    }
    private void IsSelectedChanged(bool isSelected)
    {
        CreateOrClearTemporaryResultMatrixFormat(isSelected);
    }
    private void IsActiveChanged(bool isActive)
    {
        CreateOrClearTemporaryResultMatrixFormat(isActive);
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
}
