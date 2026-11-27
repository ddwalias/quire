#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Internal;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.ViewModel;
using Quire.Controls.Plots.View;
using Quire.PersistentDataObjects;
using Quire.Core;
using Quire.Controls.Plots.PlotCommands;
using Quire.Undo;

namespace Quire.Controls.Plots;

public partial class PlotControl
{

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.PlotControl+NumberOfPointsAction, Ptc.Controls.Plots")]
        public enum NumberOfPointsAction
        {
            Increase = 0,
            Decrease = 1,
            Set = 2,
        }

    internal const string x0 = "PART_X0";
    internal const string y0 = "PART_Y0";
    internal const string partPlotGraph = "PART_PlotGraph";
    internal const string x1 = "PART_X1";
    internal const string sharedY1 = "PART_SharedY1";
    internal const string y1 = "PART_Y1";
    internal const string x2 = "PART_X2";
    internal const string x3 = "PART_X3";
    internal const string x4 = "PART_X4";
    internal const string x5 = "PART_X5";
    internal const string y2 = "PART_Y2";
    internal const string y3 = "PART_Y3";
    internal const string y4 = "PART_Y4";
    internal const string y5 = "PART_Y5";
    internal const string xSplitter = "PART_xSplitter";
    internal const string ySplitter = "PART_ySplitter";
    internal const string RadialAxis = "PART_PlotGraphRadialAxis";
    internal const string AngularAxis = "PART_PlotGraphAngularAxis";
    internal const string TitleTop = "PART_TitleTop";
    internal const string TitleBottom = "PART_TitleBottom";
    internal const string xAxisTitle = "PART_XAxisTitle";
    internal const string yAxisTitle = "PART_YAxisTitle";
    internal const string InitialPlotTitleText = "Plot Title";
    internal const string InitialXAxisTitleText = "X-axis Title";
    internal const string InitialYAxisTitleText = "Y-axis Title";
    internal const string LegendBelow = "PART_PlotLegendBelow";
    internal const string LegendTopLeft = "PART_PlotLegendTopLeft";
    internal const string LegendTopRight = "PART_PlotLegendTopRight";
    internal const string LegendBottomLeft = "PART_PlotLegendBottomLeft";
    internal const string LegendBottomRight = "PART_PlotLegendBottomRight";
    protected const string PlotGrid = "plotGrid";
    private const string MarkersControl = "PART_MarkersControl";
    public const int NumOfPoints_Step = 100;
    public const int NumberOfPoints_Min = 100;
    public const int NumberOfPoints_Max = 3000;
    private static readonly global::Quire.DoubleComparer _doubleComparer;
    private readonly global::Quire.Controls.Plots.EquationList _resultEquations = new EquationList();
    private readonly global::Quire.Controls.Plots.EquationList _rangeEquations = new EquationList();
    internal readonly global::Quire.Controls.Plots.PlotTitle _plotTitle = new PlotTitle(PlotTitleType.Plot);
    private readonly global::Quire.Controls.Plots.IPlotTitleModel _plotTitleModel;
    private readonly global::Quire.Controls.Plots.ViewModel.PlotTitleViewModel _plotTitleViewModel;
    internal readonly global::Quire.Controls.Plots.PlotTitle XAxisTitle = new PlotTitle(PlotTitleType.XAxis);
    internal readonly global::Quire.Controls.Plots.PlotTitle YAxisTitle = new PlotTitle(PlotTitleType.YAxis);
    private readonly global::System.Collections.Generic.Dictionary<string, global::Quire.Controls.Plots.Presenter.IPlotAxisView> _subregionMap = new Dictionary<string, IPlotAxisView>();
    private readonly global::Quire.Controls.Plots.Presenter.IPlotModel _model;
    private readonly global::Quire.Controls.Plots.ViewModel.PlotViewModel _viewModel;
    private readonly global::Quire.Controls.Plots.IPlotStrategy _plotStrategy;
    private readonly global::Quire.Calculation.Core.Support.CalculatableChildHider _childHider;
    private global::Quire.Controls.Plots.PlotLegend _plotLegend;
    private global::Quire.Controls.Plots.PlotItemAdorner _plotItemAdorner;
    private global::Quire.Controls.Plots.PlotItemPlaceholder[] _rangeLegendsPositions;
    private global::Quire.Controls.Plots.PlotItemPlaceholder[] _resultLegendsPositions;
    private global::Quire.Controls.Plots.PlotItemPlaceholder[] _plotTitlePositions;
    private global::Quire.Controls.Plots.PlotItemPlaceholder _sharedResultLegendPosition;
    private global::Quire.Controls.Plots.PlotItemPlaceholder[] _plotLegendPositions;
    protected global::System.Windows.Controls.Grid _plotGrid;
    private global::System.Windows.Controls.ItemsControl _markersControl;
    private global::System.Windows.Controls.ContentControl _graphPlaceholder;
    private global::Quire.Controls.Plots.PlotGraphAxis _rangeAxis;
    private global::Quire.Controls.Plots.PlotGraphAxis _primaryResultAxis;
    private int _rangeLegendPositionIndex;
    private int _resultLegendPositionIndex;
    private int _plotTitlePositionIndex;
    private int _plotLegendPositionIndex;
    private bool _plotSizeChanging;
    private global::System.Windows.Size _referenceSizeForPostUnfixate = Size.Empty;
    private int _numCells;
    private global::System.Nullable<bool> _xEquationSizeChangingDueToPlotSizeChanged;
    private global::System.Windows.Controls.ContentControl _dropTarget;
    private bool _resizeInProgress;
    private bool _expressionVisibilityChangeInProgress;
    private bool _isActivationPending;
    private bool _resizeEventsInitialized;
    private double _expressionHorizontalExtentBeforeChange;
    private double _graphHeightBeforeShowingLegendBelow = double.NaN;
    private static readonly global::System.Type _typeOfThis;
    private global::System.Action _attachGraphSizeChangedOnApplyTemplate;
    public static readonly global::System.Windows.DependencyProperty PlotTypeProperty;
    private static readonly global::System.Windows.Visibility HiddenExpressionsVisibility;
    private const string XAxisEquationDataObjectsPropertyName = "XAxisEquationDataObjects";
    private const string PrimaryYAxisEquationDataObjectsPropertyName = "PrimaryYAxisEquationDataObjects";
    private static void HandleContainedInCollapsedWhiteboardChanged(global::System.Windows.DependencyObject sender, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void HandleMinWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((PlotControl)obj).HandleMinWidthChanged();
    }

    private static void HandleMinHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((PlotControl)obj).HandleMinHeightChanged();
    }
    public static readonly global::System.Windows.DependencyProperty IsInPrintingProperty;
    public static readonly global::System.Windows.DependencyProperty IsDropTargetProperty;
    public static readonly global::System.Windows.Input.RoutedCommand ToggleTicks;
    static PlotControl()
        {
            _doubleComparer = DoubleComparer.DefaultDoubleComparer;
            _typeOfThis = typeof(PlotControl);
            PlotTypeProperty = DependencyProperty.Register("PlotType", typeof(PlotType), _typeOfThis, new PropertyMetadata(PlotType.XY));
            HiddenExpressionsVisibility = Visibility.Collapsed;
            IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(PlotControl));
            ToggleTicks = new RoutedCommand("ToggleTicks", _typeOfThis);
            ToggleNumbered = new RoutedCommand("ToggleNumbered", _typeOfThis);
            ChangeTraceTypeCommand = new RoutedCommand("ChangeTraceType", _typeOfThis);
            ToggleAxisScalingCommand = new RoutedCommand("ToggleAxisScaling", _typeOfThis);
            TogglePlotTitleVisibilityCommand = new RoutedUICommandWithFunctionalityKey("Toggle Plot Title Visibility", "TogglePlotTitleVisibility", _typeOfThis, EnumFunctionalityKey.PlotsCategoryTogglePlotTitle);
            ToggleAxisTitleVisibilityCommand = new RoutedUICommandWithFunctionalityKey("Toggle Axis Title Visibility", "ToggleAxisTitleVisibility", _typeOfThis, EnumFunctionalityKey.PlotsCategoryToggleAxisTitleVisibility);
            AddVerticalMarkerCommand = MarkersViewModel.AddVerticalMarkerCommand;
            AddHorizontalMarkerCommand = MarkersViewModel.AddHorizontalMarkerCommand;
            AddPointMarkerCommand = MarkersViewModel.AddPointMarkerCommand;
            DeleteMarkerCommand = MarkersViewModel.DeleteMarkerCommand;
            IsCalculatingProperty = DependencyProperty.Register("IsCalculating", typeof(bool), _typeOfThis);
            PlotXMinGapPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PlotXMinGap", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
            PlotYMinGapPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PlotYMinGap", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
            PlotXMinGapProperty = PlotXMinGapPropertyKey.DependencyProperty;
            PlotYMinGapProperty = PlotYMinGapPropertyKey.DependencyProperty;
            ShowCalloutsProperty = DependencyProperty.RegisterAttached("ShowCallouts", typeof(bool), typeof(PlotControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
            TraceLineStyleProperty = DependencyProperty.Register("TraceLineStyle", typeof(string), typeof(PlotControl));
            TraceLineWeightProperty = DependencyProperty.Register("TraceLineWeight", typeof(double?), typeof(PlotControl));
            TraceColorProperty = DependencyProperty.Register("TraceColor", typeof(SolidColorBrush), typeof(PlotControl));
            TraceSymbolProperty = DependencyProperty.Register("TraceSymbol", typeof(string), _typeOfThis);
            SelectedTraceTypeProperty = DependencyProperty.Register("SelectedTraceType", typeof(TraceType?), _typeOfThis);
            SelectedTracesProperty = DependencyProperty.Register("SelectedTraces", typeof(List<IPlotTrace>), typeof(PlotControl), new FrameworkPropertyMetadata(new List<IPlotTrace>(), SelectedTracesChanged));
            global::Quire.Controls.Plots.View.PlotControlBase.ShowAxisExpressionsProperty.OverrideMetadata(_typeOfThis, new PropertyMetadata(HandleShowAxisExpressionsChanged));
            PropertyMetadata val = new PropertyMetadata(new string[9] { "100", "250", "500", "750", "1000", "1500", "2000", "2500", "3000" });
            global::Quire.Controls.Plots.View.PlotControlBase.NumberOfPointsValuesCollectionProperty.OverrideMetadata(_typeOfThis, val);
            ResizableElementAttachedProperties.IsResizingProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsResizingChanged));
            FrameworkElement.MinWidthProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleMinWidthChanged));
            FrameworkElement.MinHeightProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleMinHeightChanged));
            IsInPrintingProperty = DocumentAttachedProperties.IsInPrintingProperty.AddOwner(_typeOfThis);
            SnapToGridProperty = DocumentAttachedProperties.SnapToGridProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(DocumentAttachedProperties.SnapToGridProperty.DefaultMetadata.DefaultValue, SnapToGridChanged));
            _changedEvent = EquationControl.ContentChangedEvent.AddOwner(_typeOfThis);
            ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
            AttachedProperties.ContainedInCollapsedWhiteboardProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleContainedInCollapsedWhiteboardChanged));
        }

    public PlotControl()
            : this(PlotType.XY, new XYPlotStrategy())
        {
        }
    internal PlotControl(PlotType type, IPlotStrategy strategy)
        {
            EventHandler layoutUpdatedEventHandler = null;
            layoutUpdatedEventHandler = delegate
            {
            LayoutUpdated -= layoutUpdatedEventHandler;
            WpfUtils.ExecuteOnLoad(this, PlotControlLoaded);
            };
            SizeChangedEventHandler sizeChangedEventHandler = null;
            sizeChangedEventHandler = delegate
            {
            SizeChanged -= sizeChangedEventHandler;
            LayoutUpdated += layoutUpdatedEventHandler;
            };
            SizeChanged += sizeChangedEventHandler;
            _plotTitleViewModel = _plotTitle?.ViewModel;
            _plotTitleModel = _plotTitleViewModel?.Model;
            PlotType = type;
            _model = new PlotModel(PlotType, new Size(96.0, 96.0), _plotTitleModel, XAxisTitle?.ViewModel.Model, YAxisTitle?.ViewModel.Model);
            _plotStrategy = strategy;
            InitializeAxes();
            InitializePlotLegend();
            ActiveAxis = _primaryResultAxis;
            _subregionMap["x"] = RangeAxis;
            _subregionMap["y"] = PrimaryResultAxis;
            _viewModel = new PlotViewModel(_model, RangeAxis, PrimaryResultAxis);
            DataContext = ViewModel;
            InitializeComponent();
            Focusable = false;
            _rangeEquations.ActiveEquationChanged += HandleActiveEquationChanged;
            _resultEquations.ActiveEquationChanged += HandleActiveEquationChanged;
            Graph = new PlotGraphControl(PlotType)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                OnTraceRemoved = GraphTraceRemoved,
                OnTraceAdded = GraphTraceAdded
            };
            Binding binding = new Binding
            {
                Path = new PropertyPath("ShowAxisExpressions"),
                Mode = BindingMode.TwoWay
            };
        BindingOperations.SetBinding(this, global::Quire.Controls.Plots.View.PlotControlBase.ShowAxisExpressionsProperty, binding);
            HasOriginPositioning = _model.HasOriginPositioning;
            new PlotPresenter(this, _model);
        Action<global::Quire.Calculation.Core.ICalculatable> fireCalculatableAdded = FireCalculatableContainerCalculatableAdded;
        Action<global::Quire.Calculation.Core.ICalculatable> fireCalculatableRemoved = FireCalculatableContainerCalculatableRemoved;
            if (ViewModel.Markers != null)
            {
                ViewModel.Markers.CalculatableAdded += FireCalculatableAdded;
                ViewModel.Markers.CalculatableRemoved += FireCalculatableRemoved;
            }
        _childHider = new global::Quire.Calculation.Core.Support.CalculatableChildHider(InternalCalculatables, fireCalculatableAdded, fireCalculatableRemoved, null, adoptCalculatableContainerChildren: true);
            AddHandler(RegionBorder.RequestMoveAndResizeEvent, new RegionBorder.RequestMoveAndResizeHandler(OnRequestMoveAndResize));
            AddHandler(CommonRoutedEvents.TraversedFocusEvent, new CustomRoutedEventHandler<FocusNavigationDirection>(OnTraversedFocusEvent), handledEventsToo: true);
            PropertyChanged += OnPropertyChanged;
            _plotStrategy.GetInitialPlaceholderIndices(this, out var xLegendIndex, out var yLegendIndex, out var plotTitleIndex, out var plotLegendIndex);
            XLegendPositionIndex = xLegendIndex;
            YLegendPositionIndex = yLegendIndex;
            PlotTitlePositionIndex = plotTitleIndex;
            PlotLegendPositionIndex = plotLegendIndex;
            _rangeEquations.DragStarted += PlotItemDragStarted;
            _rangeEquations.DragEnded += PlotItemDragEnded;
            _resultEquations.DragStarted += PlotItemDragStarted;
            _resultEquations.DragEnded += PlotItemDragEnded;
            if (_plotTitle != null)
            {
                _plotTitle.DragStarted += PlotItemDragStarted;
                _plotTitle.DragEnded += PlotItemDragEnded;
                if (_plotTitleViewModel != null)
                {
                    _plotTitleViewModel.PropertyChanged += PlotTitlePropertyChanged;
                }
            }
            if (XAxisTitle != null && YAxisTitle != null)
            {
                XAxisTitle.ViewModel.PropertyChanged += PlotTitlePropertyChanged;
                YAxisTitle.ViewModel.PropertyChanged += PlotTitlePropertyChanged;
            }
            if (_plotLegend != null)
            {
                _plotLegend.DragStarted += PlotItemDragStarted;
                _plotLegend.DragEnded += PlotItemDragEnded;
                if (_plotLegend.ViewModel != null)
                {
                    _plotLegend.ViewModel.PropertyChanged += PlotLegendPropertyChanged;
                }
            }
            PreviewDragOver += PlotControlPreviewDragOver;
            Drop += ContentControlDrop;
            _rangeAxis.HasOriginPositioningOwner = this;
            _primaryResultAxis.HasOriginPositioningOwner = this;
            Axes = new PlotGraphAxis[2] { _rangeAxis, _primaryResultAxis };
        }

    private void HandleMinHeightChanged()
    {
        if (Height < MinHeight)
        {
            Height = MinHeight;
        }
    }

    private void HandleMinWidthChanged()
    {
        if (Width < MinWidth)
        {
            Width = MinWidth;
        }
    }

    private void GraphTraceRemoved(PlotTrace trace)
    {
        trace.ErrorChanged -= TraceErrorChanged;
        trace.CalculatingStatusChangedEvent -= HandleTraceCalculatingStatusChanged;
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(PlotTrace.XMinGapProperty, trace, XMinGapPropertyChanged);
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(PlotTrace.YMinGapProperty, trace, YMinGapPropertyChanged);
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(PlotTrace.HasEquationErrorProperty, trace, HasEquationErrorPropertyChanged);
        trace.SetAxis(null, null);
        AttachedProperties.ClearContainedInCollapsedWhiteboard(trace);
    }

    private void GraphTraceAdded(PlotTrace newTrace)
    {
        AttachedProperties.SetContainedInCollapsedWhiteboard(newTrace, AttachedProperties.GetContainedInCollapsedWhiteboard(this));
    }

    private static void IsResizingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (!(sender is PlotControl plotControl))
        {
            DebugTools.DevAssert(false, null);
        }
        else
        {
            plotControl.Graph.SetIsResizing(plotControl.IsResizing);
        }
    }
    private void PlotControlLoaded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SelectAllTraces() {
		SelectedTraces = new List<IPlotTrace>(Traces);
	}
    private const string PlotTitleDataObjectPropertyName = "PlotTitleDataObject";
    private const string TracesPropertyName = "Traces";
    private void InitializeResizeEventsAfterLoading() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual void PostInitializeResizeEventsAfterLoading() {
		_rangeAxis.SetLegendWidthWhenLayoutReady();
		_primaryResultAxis.SetLegendWidthWhenLayoutReady();
		_plotLegend?.SetLegendWidthWhenLayoutReady();
		if (_postInitializeResizeEventsCallback != null)
		{
			_postInitializeResizeEventsCallback();
		}
		_postInitializeResizeEventsCallback = null;
	}
    private void OnPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) {
		if (e.PropertyName == "GridCellSize")
		{
			UpdatePlaceholders();
		}
	}
    protected override void BindCommands() {
		CommandBindings.Add(new CommandBinding(PlotControlBase.addRowCommand, AddRowCommandExecute, TraceCommandCanExecute));
		CommandBindings.Add(new CommandBinding(PlotControlBase.DeleteTraceCommand, DeleteTraceCommandExecute, TraceCommandCanExecute));
		base.BindCommands();
		CommandBindings.Add(new CommandBinding(PlotControlBase.ChangeTraceSymbolCommand, ChangeTraceSymbolCommandExecute, TraceSymbolCommandCanExecute));
		CommandBindings.Add(new CommandBinding(ChangeTraceTypeCommand, ChangeTraceTypeCommandExecute, TraceCommandCanExecuteByType));
		CommandBindings.Add(new CommandBinding(PlotControlBase.ChangeTraceLineCommand, ChangeTraceLineCommandExecute, TraceLineStyleCommandCanExecute));
		CommandBindings.Add(new CommandBinding(PlotControlBase.ChangeTraceWeightCommand, ChangeTraceWeightCommandExecute, TraceLineWeightCommandCanExecute));
		CommandBindings.Add(new CommandBinding(PlotControlBase.ChangeTraceColorCommand, ChangeTraceColorCommandExecute, TraceCommandCanExecute));
		CommandBindings.Add(new CommandBinding(ToggleAxisScalingCommand, ToggleAxisScalingExecuted, ToggleAxisScalingCanExecute));
		CommandBindings.Add(new CommandBinding(ToggleAxisTitleVisibilityCommand, ToggleAxisTitleVisibilityExecuted, ToggleAxisTitleVisibilityCanExecute));
		CommandBindings.Add(new CommandBinding(EquationCommands.NavigateLeft, NavigateToYEquationListEnd, CanNavigateToYEquationList));
		CommandBindings.Add(new CommandBinding(EquationCommands.NavigateRight, NavigateToXEquationList, CanNavigateToXEquationList));
		CommandBindings.Add(new CommandBinding(EquationCommands.NavigateUp, NavigateToYEquationListStart, CanNavigateToYEquationList));
		CommandBindings.Add(new CommandBinding(EquationCommands.NavigateDown, NavigateToXEquationList, CanNavigateToXEquationList));
		CommandBindings.Add(new CommandBinding(PlotControlBase.ChangeNumOfPointsCommand, ChangeNumOfPointsCommandExecute));
		CommandBindings.Add(new CommandBinding(TogglePlotTitleVisibilityCommand, TogglePlotTitleCommandExecute, TogglePlotTitleCommandCanExecute));
		CommandBindings.Add(new CommandBinding(PlotControlBase.TogglePlotLegendVisibilityCommand, TogglePlotLegendVisibilityExecuted, TogglePlotLegendVisibilityCanExecute));
	}
    protected override global::System.Collections.Generic.List<global::Quire.Controls.EquationControl> BuildEquationList() {
		List<EquationControl> list = new List<EquationControl>();
		AddEquationsFromList(_resultEquations, list);
		AddEquationsFromList(_rangeEquations, list);
		return list;
	}
    protected override global::System.Collections.Generic.List<global::Quire.Controls.Text.RichTextTitle> BuildLabelsList() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override global::System.Collections.Generic.List<global::Quire.Controls.EquationControl> CreateTickMarksAndMarkersList() {
		return _plotStrategy.BuildEditableAxisRelatedElements(this);
	}
    protected override void OnTraversedFocusEvent(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.FocusNavigationDirection> args) {
		ActivePlotEquation = (((int)args.Parameter == 0) ? _resultEquations[0] : _rangeEquations[_rangeEquations.EquationCount - 1]);
		ActivePlotEquation.DelegateFocus();
		CustomRoutedEventArgs<FocusNavigationDirection> e2 = new CustomRoutedEventArgs<FocusNavigationDirection>(args.RoutedEvent, args.Parameter);
		ActivePlotEquation.RaiseEvent(e2);
	}

    private static void HandleShowAxisExpressionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is PlotControl plotControl))
        {
            return;
        }
        if (!(bool)e.NewValue)
        {
            if (!plotControl.IsActiveRegion)
            {
                plotControl.SwitchExpressionsVisibility(HiddenExpressionsVisibility);
            }
        }
        else
        {
            plotControl.SwitchExpressionsVisibility(Visibility.Visible);
        }
    }
    public static readonly global::System.Windows.Input.RoutedCommand ToggleNumbered;
    public static readonly global::System.Windows.Input.RoutedCommand ChangeTraceTypeCommand;
    public static readonly global::System.Windows.Input.RoutedCommand ToggleAxisScalingCommand;
    public static readonly global::System.Windows.Input.RoutedUICommand TogglePlotTitleVisibilityCommand;
    public static readonly global::System.Windows.Input.RoutedUICommand ToggleAxisTitleVisibilityCommand;
    public static readonly global::System.Windows.Input.RoutedUICommand AddVerticalMarkerCommand;
    public static readonly global::System.Windows.Input.RoutedUICommand AddHorizontalMarkerCommand;
    private void SwitchExpressionsVisibility(global::System.Windows.Visibility visibility) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ShowHidePseudoErrorFrame(global::System.Windows.Visibility equationVisibility) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanNavigateToYEquationList(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ((object)_rangeEquations).Equals((object)WpfUtils.GetFocusedAncestorOfTypeElement<EquationList>(this));
    }

    private void CanNavigateToXEquationList(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ((object)_resultEquations).Equals((object)WpfUtils.GetFocusedAncestorOfTypeElement<EquationList>(this));
    }

    private void NavigateToYEquationListStart(object sender, ExecutedRoutedEventArgs e)
    {
        _resultEquations.MoveToEquationStart(_resultEquations.EquationCount - 1);
    }

    private void NavigateToYEquationListEnd(object sender, ExecutedRoutedEventArgs e)
    {
        _resultEquations.MoveToEquationEnd(_resultEquations.EquationCount - 1);
    }

    private void NavigateToXEquationList(object sender, ExecutedRoutedEventArgs e)
    {
        _rangeEquations.MoveToEquationStart(0);
    }

    private void XEquationsSizeChanged(object sender, SizeChangedEventArgs e)
    {
        EquationListOrPlotLegendSizeChanged();
    }

    private void YEquationsSizeChanged(object sender, SizeChangedEventArgs e)
    {
        EquationListOrPlotLegendSizeChanged();
    }

    private void PlotLegendSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!(sender is PlotLegend))
        {
            return;
        }
        EquationListOrPlotLegendSizeChanged();
    }

    private void EquationListOrPlotLegendSizeChanged()
    {
        MakeSureXEquationsNotOutsidePlot(changePlotSize: true);
        MakeSurePlotLegendNotOutsidePlot(changePlotSize: true);
        if (_resultLegendsPositions != null && (_resultLegendPositionIndex == 0 || _resultLegendPositionIndex == 1))
        {
            MakeSureYEquationsNotOutsidePlot(_resultLegendsPositions[_resultLegendPositionIndex], _resultEquations);
        }
    }
    private void PreserveGraphHeightAfterShowingBelowLegend() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void MakeSurePlotLegendNotOutsidePlot(bool changePlotSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void MakeSureXEquationsNotOutsidePlot(bool changePlotSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void MakeSureYEquationsNotOutsidePlot(global::Quire.Controls.Plots.PlotItemPlaceholder equationPlaceholder, global::Quire.Controls.Plots.EquationList equations) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void GraphSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnLayoutUpdated(object sender, global::System.EventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CorrectHeightIfXEquationSizeIsChanged(double verticalChange) {
		if (_xEquationSizeChangingDueToPlotSizeChanged.HasValue)
		{
			if (_doubleComparer.Compare(verticalChange, 0.0) < 0 && _xEquationSizeChangingDueToPlotSizeChanged.Value)
			{
				Height -= verticalChange;
			}
			if (_doubleComparer.Compare(verticalChange, 0.0) > 0 && !_xEquationSizeChangingDueToPlotSizeChanged.Value && DebugTools.DevAssert(Height - verticalChange > 0.0, "Trying to set negative value in Height property.").Passed)
			{
				Height -= verticalChange;
			}
			_xEquationSizeChangingDueToPlotSizeChanged = null;
		}
	}
    private void KeepGraphMinSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private double GetExpressionHorizontalExtent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void PlotControlSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static readonly global::System.Windows.Input.RoutedCommand AddPointMarkerCommand;
    public static readonly global::System.Windows.Input.RoutedUICommand DeleteMarkerCommand;
    public static readonly global::System.Windows.DependencyProperty IsCalculatingProperty;
    public static readonly global::System.Windows.DependencyPropertyKey PlotXMinGapPropertyKey;
    private void PlaceholderSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void InitializeAxes() {
		_plotStrategy.InitializeAxes(XAxisTitle, YAxisTitle, _plotTitle, _rangeEquations, _resultEquations, out _rangeAxis, out _primaryResultAxis);
	}
    private void InitializePlotLegend() {
		_plotLegend = new PlotLegend(PlotType, new PlotLegendViewModel(_model.PlotLegendModel));
		_plotLegend.Header = _plotLegend.Resources["PlotLegend.Header"] as string;
	}
    private void PlotItemDragEnded(global::Quire.Controls.Plots.PlotItem plotItem) {
		PlotItemPlaceholder[] array;
		int num;
		if (plotItem == _rangeEquations)
		{
			array = XLegendsPositions;
			num = XLegendPositionIndex;
		}
		else if (plotItem == _resultEquations)
		{
			array = YLegendsPositions;
			num = YLegendPositionIndex;
		}
		else if (plotItem == _plotLegend)
		{
			array = PlotLegendPositions;
			num = PlotLegendPositionIndex;
		}
		else
		{
			array = PlotTitlePositions;
			num = PlotTitlePositionIndex;
		}
		for (int positionIndex = 0; positionIndex < array.Length; positionIndex++)
		{
			if (num != positionIndex)
			{
				((DependencyObject)array[positionIndex]).ClearValue(UIElement.VisibilityProperty);
			}
		}
		RemovePlotItemAdorner();
		UpdateStaticXPlaceholdersVisibility();
	}
    private void PlotItemDragStarted(global::Quire.Controls.Plots.PlotItem plotItem) {
		PlotItemPlaceholder[] array;
		int num;
		if (plotItem == _rangeEquations)
		{
			array = XLegendsPositions;
			num = XLegendPositionIndex;
			CreatePlotItemAdorner(_rangeEquations);
		}
		else if (plotItem == _resultEquations)
		{
			array = YLegendsPositions;
			num = YLegendPositionIndex;
			CreatePlotItemAdorner(_resultEquations);
		}
		else if (plotItem == _plotLegend)
		{
			array = PlotLegendPositions;
			num = PlotLegendPositionIndex;
			CreatePlotItemAdorner(_plotLegend);
		}
		else
		{
			array = PlotTitlePositions;
			num = PlotTitlePositionIndex;
			if (_plotTitle != null)
			{
				CreatePlotItemAdorner(_plotTitle);
			}
		}
		for (int positionIndex = 0; positionIndex < array.Length; positionIndex++)
		{
			if (num != positionIndex)
			{
				array[positionIndex].Visibility = Visibility.Visible;
			}
		}
	}
    private void PlotTitlePropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void PlotLegendPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void UpdateStaticXPlaceholdersVisibility() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdatePlotTitlePlaceholderVisibility() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdatePlotLegendPlaceholderVisibility() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void UpdateStaticYPlaceholderVisibility() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void UpdateAxisTitlePlaceholderVisibility(global::Quire.Controls.Plots.ViewModel.PlotTitleViewModel axisTitleViewModel) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CreatePlotItemAdorner(global::Quire.Controls.Plots.PlotItem plotItem) {
		RemovePlotItemAdorner();
		AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
		UIElement visualBrushRectangle = plotItem.VisualBrushRectangle;
		_plotItemAdorner = new PlotItemAdorner(visualBrushRectangle, this);
		adornerLayer.Add(_plotItemAdorner);
	}

    private void RemovePlotItemAdorner()
    {
        if (_plotItemAdorner != null)
        {
            AdornerLayer.GetAdornerLayer(this).Remove(_plotItemAdorner);
            _plotItemAdorner = null;
        }
    }

    public static void SetIsDropTarget(UIElement element, bool value)
    {
        ((DependencyObject)element).SetValue(IsDropTargetProperty, value);
    }

    public static bool GetIsDropTarget(UIElement element)
    {
        return (bool)((DependencyObject)element).GetValue(IsDropTargetProperty);
    }
    private void PlotControlPreviewDragOver(object sender, global::System.Windows.DragEventArgs e) {
		if (_plotItemAdorner != null)
		{
			Point position = e.GetPosition(this);
			_plotItemAdorner.Left = position.X;
			_plotItemAdorner.Top = position.Y;
			PlotItem plotItem = e.Data.GetData(typeof(PlotItem).Name, autoConvert: true) as PlotItem;
			DropTarget = GetClosestPlaceholder(position, plotItem);
			e.Handled = true;
		}
	}
    private global::System.Windows.Controls.ContentControl GetClosestPlaceholder(global::System.Windows.Point position, global::Quire.Controls.Plots.PlotItem plotItem) {
		bool flag = false;
		ContentControl[] array;
		int num;
		ContentControl[] xLegendsPositions;
		if (plotItem == _rangeEquations)
		{
			xLegendsPositions = XLegendsPositions;
			array = xLegendsPositions;
			num = XLegendPositionIndex;
		}
		else if (plotItem == _resultEquations)
		{
			xLegendsPositions = YLegendsPositions;
			array = xLegendsPositions;
			num = YLegendPositionIndex;
		}
		else if (plotItem == _plotLegend)
		{
			xLegendsPositions = PlotLegendPositions;
			array = xLegendsPositions;
			num = PlotLegendPositionIndex;
		}
		else
		{
			xLegendsPositions = PlotTitlePositions;
			array = xLegendsPositions;
			num = PlotTitlePositionIndex;
		}
		ContentControl contentControl = null;
		Vector val = default;
		xLegendsPositions = array;
		foreach (ContentControl contentControl2 in xLegendsPositions)
		{
			if (!flag || contentControl2 != _sharedResultLegendPosition)
			{
				Rect placeholderBounds = contentControl2.TransformToAncestor(this).TransformBounds(new Rect(0.0, 0.0, contentControl2.ActualWidth, contentControl2.ActualHeight));
				Vector centerOffset = placeholderBounds.TopLeft + (placeholderBounds.BottomRight - placeholderBounds.TopLeft) / 2.0 - position;
				if (contentControl == null || centerOffset.Length < val.Length)
				{
					contentControl = contentControl2;
					val = centerOffset;
				}
			}
		}
		if (contentControl == array[num])
		{
			return null;
		}
		return contentControl;
	}
    public static readonly global::System.Windows.DependencyPropertyKey PlotYMinGapPropertyKey;
    public static readonly global::System.Windows.DependencyProperty PlotXMinGapProperty;
    protected override void Commit() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void CommitAllEquationListWithoutEvent(EquationList equationList)
    {
        for (int i = 0; i < equationList.EquationCount; i++)
        {
            equationList[i].Commit(fireEvent: false);
        }
    }
    private void OnRequestMoveAndResize(object sender, global::Quire.Controls.Core.ResizeEventMouseArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static bool TraceCanIncrease(IPlotTrace trace)
    {
        if (trace.IsNumOfPointsMutable)
        {
            return trace.NumberOfPoints < 3000;
        }
        return false;
    }

    private static bool TraceCanDecrease(IPlotTrace trace)
    {
        if (trace.IsNumOfPointsMutable)
        {
            return trace.NumberOfPoints > 100;
        }
        return false;
    }

    private bool AnySelectedTrace(Func<IPlotTrace, bool> predicate)
    {
        if (SelectedTraces != null)
        {
            return SelectedTraces.Any(predicate);
        }
        return false;
    }

    private void TogglePlotTitleCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _plotTitle != null;
    }
    public static readonly global::System.Windows.DependencyProperty PlotYMinGapProperty;
    public static readonly global::System.Windows.DependencyProperty SnapToGridProperty;
    public static readonly global::System.Windows.DependencyProperty ShowCalloutsProperty;
    public static readonly global::System.Windows.DependencyProperty TraceLineStyleProperty;
    public static readonly global::System.Windows.DependencyProperty TraceLineWeightProperty;
    public static readonly global::System.Windows.DependencyProperty TraceColorProperty;
    public static readonly global::System.Windows.DependencyProperty TraceSymbolProperty;
    private static readonly global::System.Windows.DependencyProperty SelectedTraceTypeProperty;
    public static readonly global::System.Windows.DependencyProperty SelectedTracesProperty;
    private global::Quire.PersistentDataObjects.plotEquation[] _xEquationsToAdd;
    private void TogglePlotTitleCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void AddRowCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        EquationCommands.InsertRow.Execute(null, ActivePlotEquation);
    }

    private void DeleteTraceCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        EquationList equationList = ActivePlotEquation == _rangeEquations.SelectedItem.Equation ? _rangeEquations : _resultEquations;
        equationList.RequestRemoveItem(equationList.SelectedItem, Direction.Right);
    }

    private void ChangeTraceTypeCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(e.Parameter is TraceType, null))
        {
            return;
        }
        TraceType traceType = (TraceType)e.Parameter;
        SetTraceType(traceType);
    }

    private void ChangeTraceSymbolCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(e.Parameter is TraceSymbol, null))
        {
            return;
        }
        TraceSymbol traceSymbol = (TraceSymbol)e.Parameter;
        SetTraceSymbol(traceSymbol);
    }

    private void ChangeTraceLineCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(e.Parameter is LineStyle, null))
        {
            return;
        }
        LineStyle lineStyle = (LineStyle)e.Parameter;
        SetTraceLineStyle(lineStyle);
    }

    private void ChangeTraceWeightCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if ((AssertionE)DebugTools.DevAssert(e.Parameter is double, null) == AssertionE.Passed)
        {
            SetSelectedTracesLineWeight((double)e.Parameter);
        }
    }
    protected static void TraceCommandCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool AnySelectedTraceIsForcedToDensityPoints() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool AnySelectedTraceIsRestrictedToDensityPointsOrSolidLine() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool AllSelectedTracesSupportDensityPoints() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool IsDensityUsedForAnySelectedTrace() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void TraceLineWeightCommandCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void TraceSymbolLineCommandCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args, global::System.Func<object, bool> isTypeAlwaysAllowed) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void TraceSymbolCommandCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected static void TraceLineStyleCommandCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void ChangeTraceColorCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected bool TraceCommandGenCheck() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void TraceCommandCanExecuteByType(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool TraceCanChangeToType(global::Quire.Calculation.Core.Trace2DResults.TraceType traceType) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleAxisScalingExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ToggleAxisScalingCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ToggleAxisTitleVisibilityExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void ToggleAxisTitleVisibilityCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void TogglePlotLegendVisibilityExecuted(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void TogglePlotLegendVisibilityCanExecute(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnTemplateChanged(global::System.Windows.Controls.ControlTemplate oldTemplate, global::System.Windows.Controls.ControlTemplate newTemplate) {
		base.OnTemplateChanged(oldTemplate, newTemplate);
		UnRegisterLegendsToEvents(XLegendsPositions);
		UnRegisterLegendsToEvents(YLegendsPositions);
		UnRegisterLegendsToEvents(PlotTitlePositions);
		UnRegisterLegendsToEvents(PlotLegendPositions);
	}
    protected void UpdatePlaceholders() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void OnApplyTemplate() {
		base.OnApplyTemplate();
		using (new CommandScope(new CompoundCommand()))
		{
			_plotStrategy.ApplyPlotTemplate(this);
			_plotStrategy.InitializeLegendPositions(this, out _rangeLegendsPositions, out _resultLegendsPositions, out _plotTitlePositions, out _plotLegendPositions);
			_sharedResultLegendPosition = _resultLegendsPositions[1];
			XLegendPositionIndex = _rangeLegendPositionIndex;
			YLegendPositionIndex = _resultLegendPositionIndex;
			PlotTitlePositionIndex = _plotTitlePositionIndex;
			PlotLegendPositionIndex = _plotLegendPositionIndex;
			DebugTools.DevAssert(_graphPlaceholder == null, null);
			_graphPlaceholder = Template.FindName("PART_PlotGraph", this) as ContentControl;
			if (DebugTools.AssertNotNull(_graphPlaceholder, null).Passed)
			{
				_graphPlaceholder.Content = Graph;
				if (_attachGraphSizeChangedOnApplyTemplate != null)
				{
					_attachGraphSizeChangedOnApplyTemplate();
				}
				_attachGraphSizeChangedOnApplyTemplate = null;
			}
			_plotGrid = Template.FindName("plotGrid", this) as Grid;
			_markersControl = Template.FindName("PART_MarkersControl", this) as ItemsControl;
			DebugTools.DevAssert(_markersControl == null == (ViewModel.Markers == null), null);
		}
		RegisterLegendsToEvents(XLegendsPositions);
		RegisterLegendsToEvents(YLegendsPositions);
		RegisterLegendsToEvents(PlotTitlePositions);
		RegisterLegendsToEvents(PlotLegendPositions);
		if (XAxisTitlePlaceholder != null && YAxisTitlePlaceholder != null)
		{
			RegisterLegendsToEvents(new PlotItemPlaceholder[1] { XAxisTitlePlaceholder });
			RegisterLegendsToEvents(new PlotItemPlaceholder[1] { YAxisTitlePlaceholder });
		}
		if (!ShowAxisExpressions)
		{
			SwitchExpressionsVisibility(HiddenExpressionsVisibility);
		}
		if (IsLoaded || HasWorksheetProperties)
		{
			UpdatePlaceholders();
			UpdateStaticXPlaceholdersVisibility();
			UpdateStaticYPlaceholderVisibility();
		}
	}
    private global::Quire.Calculation.Core.EngineResult[] _engineResults;
    private static readonly global::System.Windows.RoutedEvent _changedEvent;
    private global::System.Collections.Generic.IEnumerable<global::System.Windows.DependencyObject> GetElementsForAxis(global::Quire.Controls.Plots.Presenter.AxesType axisType, bool useEquations) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void HandleLostFocus(global::System.Windows.DependencyObject oldFocused, global::System.Windows.DependencyObject newFocused) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void HandleGotFocusOverride(global::System.Windows.DependencyObject newFocused) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal global::Quire.Controls.EquationControl GetMarkerEquation(global::Quire.Controls.Plots.ViewModel.BaseMarkerViewModel baseMarkerViewModel) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void HandlePlotGridSplitterMouseDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e) {
		if (!IsKeyboardFocusWithin)
		{
			DelegateFocus();
		}
	}
    protected virtual void RegisterLegendsToEvents(global::System.Collections.Generic.IEnumerable<global::System.Windows.Controls.ContentControl> legendsPositionsArray) {
		if (legendsPositionsArray == null)
		{
			return;
		}
		foreach (ContentControl item in legendsPositionsArray)
		{
			item.SizeChanged += PlaceholderSizeChanged;
		}
	}
    protected void UnRegisterLegendsToEvents(global::System.Collections.Generic.IEnumerable<global::System.Windows.Controls.ContentControl> legendsPositionsArray) {
		if (legendsPositionsArray == null)
		{
			return;
		}
		foreach (ContentControl item in legendsPositionsArray)
		{
			item.SizeChanged -= PlaceholderSizeChanged;
		}
	}
    private void ContentControlDrop(object sender, global::System.Windows.DragEventArgs e) {
		if (e.Data.GetDataPresent(typeof(PlotItem).Name))
		{
			PlotItem plotItem = e.Data.GetData(typeof(PlotItem).Name, autoConvert: true) as PlotItem;
			ContentControl dropTarget = DropTarget;
			if (dropTarget != null)
			{
				int moveToIndex = int.Parse(dropTarget.Tag.ToString(), CultureInfo.InvariantCulture);
				if (plotItem == _rangeEquations)
				{
					MoveLegendXToIndex(moveToIndex);
				}
				else if (plotItem == _resultEquations)
				{
					MoveLegendYToIndex(moveToIndex);
				}
				else if (plotItem == _plotLegend)
				{
					if (PlotLegendPositionIndex != 0 && Graph != null && _doubleComparer.Compare(Graph.ActualHeight, 0.0) > 0)
					{
						_graphHeightBeforeShowingLegendBelow = Graph.ActualHeight;
					}
					MovePlotLegendToIndex(moveToIndex);
					if (PlotLegendPositionIndex == 0)
					{
						PreserveGraphHeightAfterShowingBelowLegend();
					}
				}
				else
				{
					MovePlotTitleToIndex(moveToIndex);
				}
				DropTarget = null;
			}
		}
		e.Handled = true;
	}
    public void MoveLegendXToIndex(int moveToIndex) {
		new ChangeXLegendPositionCommand(this, moveToIndex).Execute();
	}
    public void MoveLegendYToIndex(int moveToIndex) {
		new ChangeYLegendPositionCommand(this, moveToIndex).Execute();
	}
    public void MovePlotTitleToIndex(int moveToIndex) {
		new ChangePlotTitlePositionCommand(this, moveToIndex).Execute();
	}
    public void MovePlotLegendToIndex(int moveToIndex) {
		new ChangPlotLegendPositionCommand(this, moveToIndex).Execute();
	}
    private void HandleActiveEquationChanged(global::Quire.Controls.Plots.Presenter.IPlotEquation equationControl) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void ChangeOriginPositioning(bool newValue) {
		base.ChangeOriginPositioning(newValue);
		PlotControlBase.UpdateHasOriginPositioningOfAxes(newValue, _rangeAxis, _primaryResultAxis);
	}
    private static readonly global::System.Windows.RoutedEvent ContentChangedEvent;
    protected global::System.Action _postInitializeResizeEventsCallback;
    private bool _contentLoaded;

    protected bool IsResizing => ResizableElementAttachedProperties.GetIsResizing(this);

    public override Size PlotGraphSize
    {
        get
        {
            return _model.GraphSize;
        }
    }
    private void XMinGapPropertyChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void YMinGapPropertyChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void SnapToGridChanged(global::System.Windows.DependencyObject obj, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static bool GetShowCallouts(global::System.Windows.UIElement uiElement) {
		return (bool)((DependencyObject)uiElement).GetValue(ShowCalloutsProperty);
	}
    public static void SetShowCallouts(global::System.Windows.UIElement uiElement, bool value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetTraceColor(global::System.Windows.Media.Color color) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override PlotType PlotType
    {
        get
        {
            return (PlotType)GetValue(PlotTypeProperty);
        }
        protected set
        {
            SetValue(PlotTypeProperty, value);
        }
    }

    private EquationList RightMostEquationList
    {
        get
        {
            switch (_resultLegendPositionIndex)
            {
            case 0:
                return _rangeEquations;
            case 1:
                return _resultEquations;
            case 2:
            case 4:
                switch (_rangeLegendPositionIndex)
                {
                case 0:
                case 1:
                case 2:
                case 3:
                    return _rangeEquations;
                case 4:
                case 5:
                    return _resultEquations;
                }
                break;
            case 3:
            case 5:
                switch (_rangeLegendPositionIndex)
                {
                case 2:
                case 3:
                    return _rangeEquations;
                case 0:
                case 1:
                case 4:
                case 5:
                    return _resultEquations;
                }
                break;
            }
            throw new Exception("not valid y legend position");
        }
    }

    private EquationList LeftMostEquationList
    {
        get
        {
            if (RightMostEquationList == _rangeEquations)
            {
                return _resultEquations;
            }
            return _rangeEquations;
        }
    }

    public bool IsInPrinting => (bool)GetValue(IsInPrintingProperty);

    public double PlotXMinGap
    {
        get
        {
            return (double)GetValue(PlotXMinGapProperty);
        }
        private set
        {
            SetValue(PlotXMinGapPropertyKey, value);
        }
    }

    public double PlotYMinGap
    {
        get
        {
            return (double)GetValue(PlotYMinGapProperty);
        }
        private set
        {
            SetValue(PlotYMinGapPropertyKey, value);
        }
    }

    public double XLegendWidthForXamlSerialization
    {
        get
        {
            return RangeAxis.LegendActualWidth;
        }
        set
        {
            RangeAxis.LegendActualWidth = value;
        }
    }

    public double YLegendWidthForXamlSerialization
    {
        get
        {
            return PrimaryResultAxis.LegendActualWidth;
        }
        set
        {
            PrimaryResultAxis.LegendActualWidth = value;
        }
    }

    public bool SnapToGrid
    {
        get
        {
            return (bool)GetValue(SnapToGridProperty);
        }
        set
        {
            SetValue(SnapToGridProperty, value);
        }
    }

    internal PlotItemPlaceholder[] XLegendsPositions => _rangeLegendsPositions;

    internal PlotItemPlaceholder[] YLegendsPositions => _resultLegendsPositions;

    internal PlotItemPlaceholder[] PlotTitlePositions => _plotTitlePositions;

    internal PlotItemPlaceholder[] PlotLegendPositions => _plotLegendPositions;

    public int TraceCount => Graph.Count;

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

    public global::Quire.Calculation.Core.Trace2DResults.TraceType? SelectedTraceType
    {
        get
        {
            return (global::Quire.Calculation.Core.Trace2DResults.TraceType?)GetValue(SelectedTraceTypeProperty);
        }
        set
        {
            SetValue(SelectedTraceTypeProperty, value);
        }
    }

    private List<IPlotTrace> SelectedTracesClone => new List<IPlotTrace>(SelectedTraces);

    public int XLegendPositionIndex
    {
        get
        {
            return _rangeLegendPositionIndex;
        }
        set
        {
            if (XLegendsPositions != null)
            {
                PlotItemPlaceholder plotItemPlaceholderAtOldPosition = XLegendsPositions[_rangeLegendPositionIndex];
                PlotItemPlaceholder plotItemPlaceholderAtNewPosition = XLegendsPositions[value];
                _rangeEquations.Move(plotItemPlaceholderAtNewPosition, plotItemPlaceholderAtOldPosition);
            }
            _rangeLegendPositionIndex = value;
            FireXLegendPositionsIndexChanged();
        }
    }

    public int YLegendPositionIndex
    {
        get
        {
            return _resultLegendPositionIndex;
        }
        set
        {
            if (YLegendsPositions != null)
            {
                PlotItemPlaceholder plotItemPlaceholderAtOldPosition = YLegendsPositions[_resultLegendPositionIndex];
                PlotItemPlaceholder plotItemPlaceholderAtNewPosition = YLegendsPositions[value];
                _resultEquations.Move(plotItemPlaceholderAtNewPosition, plotItemPlaceholderAtOldPosition);
            }
            _resultLegendPositionIndex = value;
            FireYLegendPositionsIndexChanged();
        }
    }

    public int PlotTitlePositionIndex
    {
        get
        {
            return _plotTitlePositionIndex;
        }
        set
        {
            if (_plotTitle == null)
            {
                return;
            }
            if (PlotTitlePositions != null)
            {
                PlotItemPlaceholder plotItemPlaceholderAtOldPosition = PlotTitlePositions[_plotTitlePositionIndex];
                PlotItemPlaceholder plotItemPlaceholderAtNewPosition = PlotTitlePositions[value];
                _plotTitle.Move(plotItemPlaceholderAtNewPosition, plotItemPlaceholderAtOldPosition);
            }
            _plotTitlePositionIndex = value;
            FirePlotTitlePositionsIndexChanged();
        }
    }

    public bool IsCalculating
    {
        get
        {
            return (bool)GetValue(IsCalculatingProperty);
        }
        set
        {
            SetValue(IsCalculatingProperty, value);
        }
    }

    public IPlotAxisView RangeAxis => _rangeAxis;
    private global::System.Windows.Media.SolidColorBrush GetSelectedTracesCommonColor() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private object GetSelectedTracesCommonValueForProperty(global::Quire.DelegateFunction1<object, global::Quire.Controls.Plots.Presenter.IPlotTrace> getPropertyForTrace) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public IPlotAxisView PrimaryResultAxis => _primaryResultAxis;

    public IPlotLegend PlotLegend => _plotLegend;
    private string GetSelectedTracesSymbol() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public List<IPlotTrace> SelectedTraces
    {
        get
        {
            return (List<IPlotTrace>)GetValue(SelectedTracesProperty);
        }
        set
        {
            SetValue(SelectedTracesProperty, value);
        }
    }

    internal double GraphMinHeight
    {
        get
        {
            double num = GridCellSize.Height * 4.0;
            if (_plotLegend == null || !PlotLegend.ViewModel.ShowPlotLegend)
            {
                return num;
            }
            int plotLegendPositionIndex = _plotLegendPositionIndex;
            if (plotLegendPositionIndex < 1 || plotLegendPositionIndex > 4)
            {
                return num;
            }
            return Math.Max(num, _plotLegend.MinHeight + _plotLegend.Margin.Top + _plotLegend.Margin.Bottom);
        }
    }
    private global::System.Nullable<global::Quire.Calculation.Core.Trace2DResults.TraceType> GetSelectedTracesType() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal IPlotModel Model => _model;

    internal Quire.Controls.Plots.ViewModel.PlotViewModel ViewModel => _viewModel;
    public void SetTraceLineStyle(global::Quire.Controls.Plots.Presenter.LineStyle lineStyle) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetTraceSymbol(global::Quire.Controls.Plots.Presenter.TraceSymbol symbol) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetSelectedTracesLineWeight(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Nullable<double> GetSelectedTracesLineWeight() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetTraceType(global::Quire.Calculation.Core.Trace2DResults.TraceType traceType) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void TraceErrorChanged(global::Quire.Controls.Plots.Presenter.IPlotTrace trace) {
		ShowHideErrorFrame();
		ShowHideErrorMessage();
	}

    public override int NumberOfPointsMin => 100;

    public override int NumberOfPointsMax => 3000;
    public global::Quire.Controls.Plots.PlotTitle PlotTitle { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal global::System.Windows.Visibility ResultEquationsVisibility { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal global::System.Windows.Visibility RangeEquationsVisibility { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private void HandleTraceCalculatingStatusChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal bool IsExpressionVisibilityChangeInProgress { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public static global::Quire.Controls.Internal.RubberBandHighlighter CreateErrorFrame(global::System.Windows.UIElement element) {
		return new RubberBandHighlighter(element, new List<UIElement>())
		{
			Color = Colors.Transparent,
			CornerStyle = HighlightCornerStyle.RoundedWithBorder
		};
	}
    public override void DelegateFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void RetryDelegateFocusAfterResizeEventsInitialized(object sender, global::System.EventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private int IndexByX(global::Quire.Controls.Plots.Presenter.ITraceModel traceModel) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private int IndexByY(global::Quire.Controls.Plots.Presenter.ITraceModel traceModel) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override global::Quire.Controls.Core.IMathFormat DeserializeMathFormat { get => base.DeserializeMathFormat; set => base.DeserializeMathFormat = value; }
    public override global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont> DeserializeLabeledIdFormat { get => base.DeserializeLabeledIdFormat; set => base.DeserializeLabeledIdFormat = value; }
    private global::System.Windows.Controls.ContentControl DropTarget { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Windows.Input.CommandBindingCollection CommandBindings { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private global::System.Func<global::Quire.Controls.Plots.ViewModel.BaseMarkerViewModel, global::System.Windows.DependencyObject> GetMarkerView { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Windows.Media.SolidColorBrush TraceColor { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    protected global::Quire.Controls.Plots.PlotGraphControl Graph { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public string SelectedTracesLineStyle { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public int PlotLegendPositionIndex { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    protected override bool ErrorIsNotNull { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    protected override string ErrorMessage { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private static void SelectedTracesChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override bool ShouldSerializeProperty(global::System.Windows.DependencyProperty dp) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.Controls.Plots.Presenter.IPlotTrace AddTrace() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HasEquationErrorPropertyChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
		ShowHidePseudoErrorFrame(_resultEquations.Visibility);
	}
    public void RemoveTrace(global::Quire.Controls.Plots.Presenter.IPlotTrace trace) {
		Graph.RemoveTrace(trace);
	}
    public void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calculatable) {
		if (calculatable is ICalculationHideableChild calculationHideableChild)
		{
			calculationHideableChild.HideFromCalculationContext = HideFromCalculationContext;
		}
		_childHider.ChildAdded(calculatable);
	}
    public void FireCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calculatable) {
		_childHider.ChildRemoved(calculatable);
	}
    private void FireXLegendPositionsIndexChanged() {
		if (XLegendPositionsIndexChanged != null)
		{
			XLegendPositionsIndexChanged(XLegendPositionIndex);
		}
	}
    private void FireYLegendPositionsIndexChanged() {
		if (YLegendPositionsIndexChanged != null)
		{
			YLegendPositionsIndexChanged(YLegendPositionIndex);
		}
	}
    private void FirePlotTitlePositionsIndexChanged() {
		if (PlotTitlePositionsIndexChanged != null)
		{
			PlotTitlePositionsIndexChanged((LegendPositions.PlotTitlePosition)PlotTitlePositionIndex);
		}
	}
    private void FirePlotLegendPositionsIndexChanged() {
		if (PlotLegendPositionsIndexChanged != null)
		{
			PlotLegendPositionsIndexChanged(PlotLegendPositionIndex);
		}
	}
    public void AddEquations(global::Quire.Controls.Plots.XyEquation xyEquation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public int ErrorCount() {
		int errorCount = 0;
		for (int index = 0; index < Graph.Count; index++)
		{
			if (Traces[index].PlotCalculatableError != null)
			{
				errorCount++;
			}
		}
		return errorCount;
	}
    public string Error() {
		for (int num = 0; num < Graph.Count; num++)
		{
			if (Traces[num].PlotCalculatableError != null)
			{
				return Traces[num].PlotCalculatableError.ErrorCode;
			}
		}
		return string.Empty;
	}
    internal string ErrorText() {
		for (int num = 0; num < Graph.Count; num++)
		{
			if (Traces[num].PlotCalculatableError != null)
			{
				return GetErrorMessage(Traces[num].PlotCalculatableError) ?? string.Empty;
			}
		}
		return string.Empty;
	}

    protected override void IsActiveRegionChanged(bool newValue)
    {
        base.IsActiveRegionChanged(newValue);
        if (newValue)
        {
            _isActivationPending = false;
            return;
        }
        if (!ShowAxisExpressions)
        {
            SwitchExpressionsVisibility(HiddenExpressionsVisibility);
        }
        SelectAllTraces();
    }

    protected override void IsSelectedChanged(bool newValue)
    {
        base.IsSelectedChanged(newValue);
        if (newValue)
        {
            SelectAllTraces();
        }
    }
    protected override global::Quire.Controls.Core.IWhiteboardItem LastChildItem { get => base.LastChildItem; }
    protected override global::Quire.Controls.Core.IWhiteboardItem FirstChildItem { get => base.FirstChildItem; }
    private global::System.Func<global::Quire.Controls.Plots.Presenter.ITraceModel, int> VisualOrder { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.Presenter.ITraceModel> TraceModels { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatableResultOwner> ResultOwners { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> InternalCalculatables { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public override global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren { get => base.CalculatableChildren; }
    public global::Quire.Controls.Plots.Presenter.IPlotTrace[] Traces { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    protected override global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.Presenter.IPlotEquation> GetPlotEquations() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override bool HideFromCalculationContext { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public void UpdateAxisOriginPositions(bool modelHasOriginPositioning) {
		HasOriginPositioning = modelHasOriginPositioning;
		if (HasOriginPositioning)
		{
			_rangeAxis.UpdateAxisOriginPositions();
			_primaryResultAxis.UpdateAxisOriginPositions();
		}
	}
    public global::Quire.PersistentDataObjects.plotEquation[] PrimaryYAxisEquationDataObjects { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private static global::Quire.PersistentDataObjects.plotEquation[] GetPlotEquationDataObjects(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView) {
		plotEquation[] array = new plotEquation[axisView.EquationCount];
		for (int num = 0; num < axisView.EquationCount; num++)
		{
			array[num] = axisView[num].GetDataObject();
		}
		return array;
	}
    private static void SetPlotEquationDataObjects(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView, global::Quire.PersistentDataObjects.plotEquation[] dataObjects) {
		for (int num = 0; num < dataObjects.Length; num++)
		{
			axisView[num].SetDataObject(dataObjects[num]);
		}
	}
    private static void AddEquations(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView, int count) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::Quire.PersistentDataObjects.textType[] GetPlotLegendItemsDataObjects(global::Quire.Controls.Plots.PlotLegend plotLegend) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetPlotLegendItemsDataObjects(global::Quire.Controls.Plots.PlotLegend plotLegend, global::Quire.PersistentDataObjects.textType[] legendlabelitems) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public virtual void SetGraphSizeForXamlSerialization() {
	}
    public override void EndInit() {
		base.EndInit();
		if (ViewModel != null)
		{
			ViewModel.UpdateFromModel();
		}
		if (_engineResults != null)
		{
			ResultManager.DispatchResults(_engineResults, ResultOwners);
		}
		if (InCustomInitialization)
		{
			InCustomInitialization = false;
		}
	}
    public global::Quire.PersistentDataObjects.plotEquation[] XAxisEquationDataObjects { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.PersistentDataObjects.plotBaseTypeLegend PlotLegendDataObject { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.PersistentDataObjects.plotTitleType PlotTitleDataObject { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.PersistentDataObjects.plotTitleType PlotXAxisTitleForXamlSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.PersistentDataObjects.plotTitleType PlotYAxisTitleForXamlSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.Controls.Plots.PlotAxis PlotXAxisForXamlSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.Controls.Plots.PlotAxis PlotYAxisForXamlSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Windows.Size PlotGraphSizeForXamlSerialization { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.Calculation.Core.EngineResult[] EngineResults { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal double GraphMinWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private bool IsNumberOfPointsAvailable { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public override bool InCustomInitialization { get => base.InCustomInitialization; set => base.InCustomInitialization = value; }
    public override void Dispose() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::System.Xml.XmlNode ConvertToWorkseetXmlDocumentAndValidate(global::Quire.PersistentDataObjects.plotType plotType, bool withOverrides) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void AssertPlotDataXml(global::Quire.PersistentDataObjects.plotType dummyPlotType) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal PlotItemPlaceholder XAxisTitlePlaceholder { get; set; }
    private static void CompareXmls(global::System.Xml.XmlNode node1, global::System.Xml.XmlNode node2) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static bool CompareDouble(double d1, double d2, int digits) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual bool SetGraphSize(int numCells) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ForceMc14GraphSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::System.Xml.XmlNode BuildPlotXmlNodeFromChildren(string nodeName, global::System.Xml.XmlNode[] nodes, global::System.Xml.XmlNamespaceManager namespaceManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, object data) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ChangeNumOfPointsCommandExecute(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void UpdateTraceNumOfPoints(global::Quire.Controls.Plots.PlotControl.NumberOfPointsAction action, int num) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::Quire.Delegate1<int> XLegendPositionsIndexChanged;
    private void UpdateSelectedTracesNumOfPoints(global::Quire.Controls.Plots.PlotControl.NumberOfPointsAction action, global::Quire.Pair<int, bool> numberOfPointsData) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateSelectedTracesNumOfPointsProperty() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private int CalculateNumberOfPointsInSelectedTraces() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override void DeserializeEpilog() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void PropagateTextStyleChangedHandler() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void PropagateInitialTextFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plotcontrol.xaml", UriKind.Relative);
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
        _contentLoaded = true;
    }
    public event global::Quire.Delegate1<int> YLegendPositionsIndexChanged;
    public event global::Quire.Delegate1<global::Quire.Controls.Plots.LegendPositions.PlotTitlePosition> PlotTitlePositionsIndexChanged;
    public event global::Quire.Delegate1<int> PlotLegendPositionsIndexChanged;

    internal PlotItemPlaceholder YAxisTitlePlaceholder { get; set; }

    public PlotViewType Type { get; set; }
}
