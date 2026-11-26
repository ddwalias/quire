#nullable disable
#pragma warning disable

using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Plot3D.Tools;
using Quire.Controls.Plots.Plot3D.ViewModel;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;
using Quire.Controls.Plots;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Undo;
using Quire.Wpf;
using Quire;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System;

namespace Quire.Controls.Plots.Plot3D.View;

[TemplatePart(Name = "PART_DragBorder", Type = typeof(RegionBorder))]
public partial class PlotControl3D
{
    private static readonly Type TypeOfThis;

    public static readonly Thickness AxesWidgetMargin;

    public static readonly DependencyProperty PlotCameraTypeProperty;

    public static readonly DependencyProperty TraceSymbolProperty;

    public static readonly DependencyProperty TraceLineStyleProperty;

    public static readonly DependencyProperty TraceColorProperty;

    public static readonly DependencyProperty TraceLineWeightProperty;

    public static readonly DependencyProperty TraceSurfaceFillColorProperty;

    public static readonly RoutedCommand ToggleTrackballVisualCuesCommand;

    public static readonly RoutedCommand ResetCameraLocationCommand;

    private static readonly Type _typeOfThis;

    private bool _intializationInProgress;

    private EngineResult[] _engineResultsToSetInEndInit;

    private readonly SpinTrackball _spinTrackball = new SpinTrackball();

    private readonly PanTrackball _panTrackball = new PanTrackball();

    private readonly ZoomTrackball _zoomTrackball = new ZoomTrackball();

    private IBaseTrackball _trackball;

    public CameraType3D PlotCameraType
    {
        get
        {
            return (CameraType3D)GetValue(PlotCameraTypeProperty);
        }
        set
        {
            SetValue(PlotCameraTypeProperty, value);
        }
    }

    public string TraceSymbol
    {
        get
        {
            return (string)GetValue(TraceSymbolProperty);
        }
        set
        {
            SetValue(TraceSymbolProperty, value);
        }
    }

    public string TraceLineStyle
    {
        get
        {
            return (string)GetValue(TraceLineStyleProperty);
        }
        set
        {
            SetValue(TraceLineStyleProperty, value);
        }
    }

    public SolidColorBrush TraceColor
    {
        get
        {
            return (SolidColorBrush)GetValue(TraceColorProperty);
        }
        set
        {
            SetValue(TraceColorProperty, value);
        }
    }

    public double? TraceLineWeight
    {
        get
        {
            return (double?)GetValue(TraceLineWeightProperty);
        }
        set
        {
            SetValue(TraceLineWeightProperty, value);
        }
    }

    public SolidColorBrush TraceSurfaceFillColor
    {
        get
        {
            return (SolidColorBrush)GetValue(TraceSurfaceFillColorProperty);
        }
        set
        {
            SetValue(TraceSurfaceFillColorProperty, value);
        }
    }

    internal IBaseTrackball Trackball
    {
        get
        {
            return _trackball;
        }
        set
        {
            if (_trackball != null)
            {
                _trackball.Detach();
                _trackball.MouseUp -= ViewModel.TrackballMouseUp;
                _trackball.MouseWheel -= ViewModel.TrackballMouseWheel;
            }
            _trackball = value;
            if (_trackball != null)
            {
                _trackball.Attach(_graphContainer, ViewModel, _graph.Viewport);
                _trackball.MouseUp += ViewModel.TrackballMouseUp;
                _trackball.MouseWheel += ViewModel.TrackballMouseWheel;
            }
        }
    }

    internal PlotAxesWidget3D AxesWidget
    {
        get
        {
            return AxesWidgetHost.Content as PlotAxesWidget3D;
        }
        private set
        {
            using (new CommandScope(new CompoundCommand()))
            {
                AxesWidgetHost.Content = value;
            }
        }
    }

    [DefaultValue(null)]
    public string GraphImageFileNameForPrintXamlSerialization
    {
        get
        {
            string result = null;
            if (_graph.IsInPrinting && _graph.CanPrint)
            {
                bool isActiveRegion = IsActiveRegion;
                try
                {
                    if (isActiveRegion)
                    {
                        IsActiveRegionChanged(false);
                    }
                    result = _graph.ImageFileNameForPrintXamlSerialization;
                }
                catch (Exception ex)
                {
                    DebugTools.FailAssertion(ex.Message);
                }
                finally
                {
                    if (isActiveRegion)
                    {
                        IsActiveRegionChanged(true);
                    }
                }
            }
            return result;
        }
        set
        {
            if (value != null)
            {
                _graph.ImageFileNameForPrintXamlSerialization = value;
            }
        }
    }

    public override IEnumerable<ICalculatable> CalculatableChildren => ViewModel.CalculatableChildren;

    public RegionBorder RegionBorder { get; private set; }

    private CameraControlWidget _cameraControlWidget;

    public CameraControlState CameraControlState => ViewModel.CameraControlState;

    private ContentControl AxesWidgetHost => _axesWidgetHost;

    static PlotControl3D()
    {
        TypeOfThis = typeof(PlotControl3D);
        AxesWidgetMargin = new Thickness
        {
            Left = WpfUtils.InchesToDips(0.2),
            Top = 0.0,
            Right = 0.0,
            Bottom = 0.0
        };
        TraceSurfaceFillColorProperty = DependencyProperty.Register("TraceSurfaceFillColor", typeof(SolidColorBrush), TypeOfThis);
        ToggleTrackballVisualCuesCommand = new RoutedCommand("ToggleTrackballHint", TypeOfThis, new InputGestureCollection
        {
            new KeyGesture(Key.F7, ModifierKeys.Control | ModifierKeys.Shift)
        });
        ResetCameraLocationCommand = new RoutedCommand("ResetCameraLocation", TypeOfThis, new InputGestureCollection
        {
            new KeyGesture(Key.F6, ModifierKeys.Control | ModifierKeys.Shift)
        });
        _typeOfThis = typeof(PlotControl3D);
        // Block scope keeps the Thickness temporary at local slot 0 before PropertyMetadata at slot 1.
        {
            PropertyMetadata metadata = new PropertyMetadata(new string[9] { "11", "21", "31", "41", "51", "61", "71", "81", "101" });
            PlotControlBase.NumberOfPointsValuesCollectionProperty.OverrideMetadata(_typeOfThis, metadata);
        }
        PlotCameraTypeProperty = AttachedProperties.PlotCameraTypeProperty.AddOwner(TypeOfThis, new PropertyMetadata(PlotCameraTypeChanged));
        TraceSymbolProperty = PlotControl.TraceSymbolProperty.AddOwner(TypeOfThis);
        TraceLineStyleProperty = PlotControl.TraceLineStyleProperty.AddOwner(TypeOfThis);
        TraceColorProperty = PlotControl.TraceColorProperty.AddOwner(TypeOfThis);
        TraceLineWeightProperty = PlotControl.TraceLineWeightProperty.AddOwner(TypeOfThis);
        _changedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
        AttachedProperties.ContainedInCollapsedWhiteboardProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        {
            AttachedProperties.SetContainedInCollapsedWhiteboard(((PlotControl3D)sender).Graph, (bool)args.NewValue);
        }));
    }

    public PlotControl3D()
    {
        ViewModel = new PlotViewModel3D();
        ViewModel.CalculatableAdded += FireCalculatableContainerCalculatableAdded;
        ViewModel.CalculatableRemoved += FireCalculatableContainerCalculatableRemoved;
        InitializeComponent();
        ViewModel.TraceInserted += ViewModelTraceInserted;
        ViewModel.TraceRemoved += ViewModelTraceRemoved;
        ViewModel.EquationAdded += HandleEquationAdded;
        ViewModel.EquationRemoved += HandleEquationRemoved;
        ViewModel.RequestActivation += ActivateIfNeeded;
        ZEquationsList.ViewModel = ViewModel;
        ZEquationsList.ActiveEquationChanged += HandleActiveEquationChanged;
        ZEquationsList.AddEquationByPosition();
        ViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs eArgs) =>
        {
            ViewModelPropertyChanged(eArgs.PropertyName);
        };
        PropertyChanged += OnPropertyChanged;
        ActivePlotEquation = ZEquationsList[0];
        AxesWidget = new PlotAxesWidget3D(ViewModel.XAxisViewModel, ViewModel.YAxisViewModel, ViewModel.ZAxisViewModel);
        AxesWidget.PropertyChanged += AxesWidgetPropertyChanged;
        AxisWidgetActiveAxisChanged();
        AxisWidgetHighlightedAxisChanged();
        Axes = new PlotGraphAxis[3] { AxesWidgetXAxis, AxesWidgetYAxis, AxesWidgetZAxis };
        _graph.XAxis.ViewModel = ViewModel.XAxisViewModel;
        _graph.YAxis.ViewModel = ViewModel.YAxisViewModel;
        _graph.ZAxis.ViewModel = ViewModel.ZAxisViewModel;
        ViewModel.XAxisViewModel.RequestFocus += () =>
        {
            AxisRequestFocus(AxesType.X);
        };
        ViewModel.YAxisViewModel.RequestFocus += () =>
        {
            AxisRequestFocus(AxesType.Y);
        };
        ViewModel.ZAxisViewModel.RequestFocus += () =>
        {
            AxisRequestFocus(AxesType.Z);
        };
        ViewModel.ModelChanged += ViewModelModelChanged;
        ViewModel.DeserializationEnded += ReadDataOnDeserializationEnd;
        MatchTrackballToCameraAndActiveState();
    }

    private void AxesWidgetPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        switch (propertyChangedEventArgs.PropertyName)
        {
            case "ActiveAxis":
                AxisWidgetActiveAxisChanged();
                break;
            case "HighlightedAxis":
                AxisWidgetHighlightedAxisChanged();
                break;
        }
    }

    private void ActivateIfNeeded()
    {
        if (!IsActiveRegion || !IsKeyboardFocusWithin)
        {
            Activate(isActive: true);
        }
    }

    private void AxisRequestFocus(AxesType type)
    {
        ActivateIfNeeded();
        AxesWidget.ActivateAxis(type, null);
    }

    private void MatchTrackballToCameraAndActiveState()
    {
        if (!IsActiveRegion)
        {
            Trackball = null;
            return;
        }
        switch (CameraControlState)
        {
            case CameraControlState.Spin:
                Trackball = _spinTrackball;
                break;
            case CameraControlState.Pan:
                Trackball = _panTrackball;
                break;
            case CameraControlState.Zoom:
                Trackball = _zoomTrackball;
                break;
            default:
                DebugTools.DevAssert(false, null);
                break;
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RegionBorder = Template.FindName("PART_DragBorder", this) as RegionBorder;
    }

    private void AxisWidgetHighlightedAxisChanged()
    {
        // Negated condition makes Roslyn 10 emit brfalse.s to ldnull matching Mathcad IL.
        ViewModel.HighlightedAxis = (!AxesWidget.HighlightedAxis.HasValue) ? null : GetViewModelForAxesType(AxesWidget.HighlightedAxis.Value);
    }

    private void AxisWidgetActiveAxisChanged()
    {
        ViewModel.ActiveAxis = GetViewModelForAxesType(AxesWidget.ActiveAxisType);
        ActiveAxis = AxesWidget.ActiveAxis;
    }

    private Base3DAxisViewModel GetViewModelForAxesType(AxesType axisType)
    {
        switch (axisType)
        {
            case AxesType.X:
                return ViewModel.XAxisViewModel;
            case AxesType.Y:
                return ViewModel.YAxisViewModel;
            case AxesType.Z:
                return ViewModel.ZAxisViewModel;
            default:
                DebugTools.DevAssert(false, null);
                return null;
        }
    }

    private void HandleEquationAdded(int index, int previousIndex, IPlotEquationViewModel3D equationViewModel3D)
    {
        bool isNew;
        if (equationViewModel3D.EquationProxy == null)
        {
            PlotEquation equationProxy = new PlotEquation
            {
                ViewModel = equationViewModel3D,
                InCustomInitialization = InCustomInitialization,
                DeserializeMathFormat = DeserializeMathFormat,
                DeserializeLabeledIdFormat = DeserializeLabeledIdFormat
            };
            equationViewModel3D.EquationProxy = equationProxy;
            isNew = true;
        }
        else
        {
            equationViewModel3D.EquationProxy.ResetBeforeAdding();
            isNew = false;
        }
        IEquationListItem equationListItem = new EquationListItem
        {
            Equation = equationViewModel3D.EquationProxy,
            Margin = new Thickness(0.0)
        };
        ZEquationsList.AddItemToLegend(index, equationListItem, !isNew);
    }

    private void HandleEquationRemoved(int index, int previousIndex, Direction direction)
    {
        ZEquationsList.RemoveItemAt(index, previousIndex, direction).Equation = null;
        ShowHideErrorFrame();
    }

    private void ViewModelTraceRemoved(IPlotTraceViewModel3D traceViewModel)
    {
        _graph.RemoveTrace(traceViewModel);
    }

    private void ViewModelTraceInserted(IPlotTraceViewModel3D traceViewModel, int index)
    {
        PlotTrace3D traceView = new PlotTrace3D(traceViewModel);
        _graph.InsertTraceView(traceView, index);
    }

    public PlotViewModel3D ViewModel { get; private set; }

    public override bool HideFromCalculationContext
    {
        get
        {
            return ViewModel.HideFromCalculationContext;
        }
        set
        {
            ViewModel.HideFromCalculationContext = value;
        }
    }

    protected override bool ErrorIsNotNull => _zEquationsList.Equations.Any((IPlotEquation plotEquation) => plotEquation.PlotCalculatableError != null);

    protected override string ErrorMessage
    {
        get
        {
            string result = null;
            IPlotEquation activePlotEquation = ActivePlotEquation;
            if (activePlotEquation != null)
            {
                result = GetErrorMessage(activePlotEquation.PlotCalculatableError);
            }
            return result;
        }
    }

    protected override void HandleLostFocus(DependencyObject oldFocused, DependencyObject newFocused)
    {
        Axes.Any((PlotGraphAxis axis) => axis.HandleAncestorLostFocus(oldFocused, newFocused, new List<DependencyObject> { axis }, null));
    }

    private PlotGraphAxis AxesWidgetZAxis => AxesWidget.ZAxis;

    private PlotGraphAxis AxesWidgetYAxis => AxesWidget.YAxis;

    private PlotGraphAxis AxesWidgetXAxis => AxesWidget.XAxis;

    public override Size PlotGraphSize
    {
        get
        {
            return SubElementToResizeData.Element.RenderSize;
        }
    }

    internal Size PlotGraphViewportSize
    {
        get
        {
            return Graph.Viewport.RenderSize;
        }
    }

    public override PlotType PlotType
    {
        get
        {
            return PlotType.Plot3D;
        }
        protected set
        {
            DebugTools.DevAssert(value == PlotType.Plot3D, null);
        }
    }

    protected override void IsActiveRegionChanged(bool active)
    {
        base.IsActiveRegionChanged(active);
        ViewModel.IsActiveRegion = active;
        BoxVisibility = active;
        MatchTrackballToCameraAndActiveState();
        if (_cameraControlWidget != null)
        {
            _cameraControlWidget.Remove();
            _cameraControlWidget = null;
        }
        if (active)
        {
            _cameraControlWidget = new CameraControlWidget(_graphContainerFullSize, ViewModel);
        }
    }

    internal bool IsCameraControlWidgetVisible => _cameraControlWidget != null;

    public bool BoxVisibility
    {
        get
        {
            return _graph.BoxVisibility;
        }
        private set
        {
            _graph.BoxVisibility = value;
        }
    }

    public void AddEquations(XyzEquation xyzEquation)
    {
        ZEquationsList[0].EquationSExpression = xyzEquation.ZsExpression;
    }

    internal EquationList ZEquationsList => _zEquationsList;

    protected override IEnumerable<IPlotEquation> GetPlotEquations()
    {
        return ZEquationsList.Equations;
    }

    public override void DelegateFocus()
    {
        ViewModel.PendingActivation = true;
        base.DelegateFocus();
        if (IsLoaded && !IsKeyboardFocusWithin)
        {
            DebugTools.DevAssert(true, null);
            ViewModel.PendingActivation = false;
        }
    }

    protected override IWhiteboardItem FirstChildItem => ZEquationsList[0].ExpressionItem;

    protected override IWhiteboardItem LastChildItem => ZEquationsList[ZEquationsList.EquationCount - 1].ScalingItem;

    public SubElementToResizeData SubElementToResizeData => new SubElementToResizeData(_graphContainerFullSize, serializeDimensions: false, isFullSize: false);

    public bool PreserveAspectRatio => false;

    [DependsOn("FirstPropertyForXamlSerialization")]
    public plotType XamlDataObject
    {
        get
        {
            return ViewModel.XamlDataObject;
        }
        set
        {
            ViewModel.XamlDataObject = value;
        }
    }

    private static void PlotCameraTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is PlotControl3D plotControl3D)
        {
            plotControl3D.PlotCameraTypeChanged(args);
        }
        else
        {
            DebugTools.DevAssert(false, null);
        }
    }

    private void PlotCameraTypeChanged(DependencyPropertyChangedEventArgs args)
    {
        if (!(args.NewValue is CameraType3D))
        {
            DebugTools.DevAssert(false, null);
        }
        else
        {
            ViewModel.PlotCameraType = (CameraType3D)args.NewValue;
        }
    }

    protected override void OnBackgroundTypeChanged(BackgroundTypeE newValue)
    {
        base.OnBackgroundTypeChanged(newValue);
        ViewModel.PlotBackgroundType = newValue;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        switch (propertyChangedEventArgs.PropertyName)
        {
            case "GridCellSize":
                ViewModel.GridCellSize = GridCellSize;
                break;
            case "ActivePlotEquation":
                ViewModel.ApplyActivePlotEquation(ActivePlotEquation.ViewModel);
                break;
        }
    }

    private void ViewModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "PlotCameraType":
                PlotCameraType = ViewModel.PlotCameraType;
                break;
            case "PlotBackgroundType":
                PlotBackgroundType = ViewModel.PlotBackgroundType;
                break;
            case "ShowControlLevelErrorFrameForNestedError":
                ShowHideErrorFrame();
                break;
            case "PlotCalculatableError":
                ShowHideErrorFrame();
                ShowHideErrorMessage();
                break;
            case "CameraControlState":
                MatchTrackballToCameraAndActiveState();
                break;
        }
    }

    private void ReadDataOnDeserializationEnd()
    {
        PlotCameraType = ViewModel.PlotCameraType;
        ViewModel.RequestResetTrackball();
    }

    // Declared here before ViewModelModelChanged to match Mathcad field and closure ordinals (55/56/77/80/134).
    private static readonly RoutedEvent _changedEvent;

    private void ViewModelModelChanged()
    {
        RaiseEvent(new RoutedEventArgs(_changedEvent, this));
    }

    protected override void BindCommands()
    {
        base.BindCommands();
        CommandBindings.Add(new CommandBinding(PlotControlBase.addRowCommand, AddRowCommandExecute, CanExecuteAddOrRemoveTraceCommand));
        CommandBindings.Add(new CommandBinding(PlotControlBase.DeleteTraceCommand, DeleteTraceCommandExecute, CanExecuteAddOrRemoveTraceCommand));
        CommandBindings.Add(new CommandBinding(ToggleTrackballVisualCuesCommand, ToggleTrackballVisualCues));
        CommandBindings.Add(new CommandBinding(ResetCameraLocationCommand, ResetCameraLocation));
    }

    private void ResetCameraLocation(object sender, ExecutedRoutedEventArgs args)
    {
        Point3D position = _graph.Camera.Position;
        position.X = 0.0;
        position.Y = 0.0;
        Vector3D lookDirection = new Vector3D(0.0, 0.0, -position.Z);
        _graph.Camera.Position = position;
        _graph.Camera.LookDirection = lookDirection;
        if (_graph.Camera is PerspectiveCamera)
        {
            ((PerspectiveCamera)_graph.Camera).FieldOfView = 9.55;
        }
        ViewModel.CameraTransform = Transform3D.Identity;
        ViewModel.RequestResetTrackball();
    }

    private void ToggleTrackballVisualCues(object sender, ExecutedRoutedEventArgs e)
    {
        _graph.ToggleVisualTrackballCues();
    }

    private void AddRowCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        EquationCommands.InsertRow.Execute(null, ZEquationsList);
    }

    private void DeleteTraceCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        ZEquationsList.RequestRemoveItem(ZEquationsList.SelectedItem, Direction.Right);
    }

    protected static void CanExecuteAddOrRemoveTraceCommand(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
    {
        if (sender is PlotControl3D plotControl3D)
        {
            canExecuteRoutedEventArgs.CanExecute = plotControl3D.ViewModel.EquationListHasFocus;
        }
    }

    private void HandleActiveEquationChanged(IPlotEquation equationControl)
    {
        if (equationControl != null && ActivePlotEquation != equationControl)
        {
            if (ActivePlotEquation != null && !ActivePlotEquation.IsCommitted && !ActivePlotEquation.IsPlotEquationEmpty)
            {
                ActivePlotEquation.Commit();
            }
            ActivePlotEquation = equationControl;
        }
        ShowHideErrorMessage();
    }

    internal bool IsAxisSelectorVisible => AxesWidgetHost.IsVisible;

    [DefaultValue(null)]
    [DependsOn("FirstPropertyForXamlSerialization")]
    public EngineResult[] EngineResultsForXamlSerialization
    {
        get
        {
            Func<ICalculatableResultOwner, EngineResult> selector = (ICalculatableResultOwner wrapper) => wrapper is IResultOwnerWithSerializableEngineResult resultOwner ? resultOwner.EngineResultForSerialization : wrapper.EngineResult;
            return ResultOwners.Select(selector).ToArray();
        }
        set
        {
            if (_intializationInProgress)
            {
                _engineResultsToSetInEndInit = value;
            }
            else
            {
                DispatchResults(value);
            }
        }
    }

    private void DispatchResults(EngineResult[] value)
    {
        if (value != null)
        {
            ResultManager.DispatchResults(value, ResultOwners);
        }
    }

    public IEnumerable<ICalculatableResultOwner> ResultOwners => ViewModel.CollectHiddenAndVisibleCalculatables().OfType<ICalculatableResultOwner>();

    internal PlotGraph3D Graph => _graph;

    public override void BeginInit()
    {
        DebugTools.DevAssert(!_intializationInProgress, null);
        DebugTools.DevAssert(_engineResultsToSetInEndInit == null, null);
        _intializationInProgress = true;
        base.BeginInit();
    }

    public override void EndInit()
    {
        if ((AssertionE)DebugTools.DevAssert(_intializationInProgress, null) == AssertionE.Passed)
        {
            base.EndInit();
            _intializationInProgress = false;
            DispatchResults(_engineResultsToSetInEndInit);
            _engineResultsToSetInEndInit = null;
            if (InCustomInitialization)
            {
                InCustomInitialization = false;
            }
        }
    }

    public IEnumerable<TResult> SelectTraceViews<TResult>(Func<PlotTrace3D, TResult> selector)
    {
        return _graph.SelectTraceViews(selector);
    }

    public int TraceCount => _graph.TraceViewsCount;

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (base.ShouldSerializeProperty(dp))
        {
            return DependencyPropertyDescriptor.FromProperty(dp, _typeOfThis).IsAttached;
        }
        return false;
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        ViewModel.SetDataObject(d11NProvider, (plotType)data);
        return null;
    }

    internal void ActivateAxis(AxesType axis)
    {
        AxesWidget.ActivateAxis(axis, FocusNavigationDirection.Next);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return ViewModel.GetDataObject(s11NProvider);
    }

    protected override List<EquationControl> BuildEquationList()
    {
        List<EquationControl> list = new List<EquationControl>();
        AddEquationsFromList(_zEquationsList, list);
        return list;
    }

    public override void Dispose()
    {
        _graph.Dispose();
    }

    protected override void Commit()
    {
        ViewModel.Commit();
    }

    protected override bool ErrorFrameShouldBeVisible
    {
        get
        {
            if (!base.ErrorFrameShouldBeVisible)
            {
                return ViewModel.ShowControlLevelErrorFrameForNestedError;
            }
            return true;
        }
    }

    private void OnSizeChangedOfElementAffectingGraphWidth(object sender, SizeChangedEventArgs args)
    {
    }

    public void UpdateTraceNumOfPoints(int numberOfPoints)
    {
        if (numberOfPoints <= NumberOfPointsMax && numberOfPoints >= NumberOfPointsMin)
        {
            ViewModel.UpdateSelectedTracesNumberOfPoints(numberOfPoints);
        }
    }

    public override int NumberOfPointsMin => 11;

    public override int NumberOfPointsMax => 101;

    private const int NumberOfPointsMinValue = 11;

    private const int NumberOfPointsMaxValue = 101;

    private Grid _graphContainerFullSize;

    private Grid _graphContainer;

    private Rectangle _background;

    private PlotGraph3D _graph;

    internal ContentControl _axesWidgetHost;

    private Rectangle _graphContainerFullSizeComplementary;

    private EquationList _zEquationsList;

    private Rectangle _zEquationListComplementary;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plot3d/view/plotcontrol3d.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _graphContainerFullSize = (Grid)target;
                break;
            case 2:
                _graphContainer = (Grid)target;
                break;
            case 3:
                _background = (Rectangle)target;
                break;
            case 4:
                _graph = (PlotGraph3D)target;
                break;
            case 5:
                _axesWidgetHost = (ContentControl)target;
                break;
            case 6:
                _graphContainerFullSizeComplementary = (Rectangle)target;
                _graphContainerFullSizeComplementary.SizeChanged += OnSizeChangedOfElementAffectingGraphWidth;
                break;
            case 7:
                _zEquationsList = (EquationList)target;
                break;
            case 8:
                _zEquationListComplementary = (Rectangle)target;
                _zEquationListComplementary.SizeChanged += OnSizeChangedOfElementAffectingGraphWidth;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
