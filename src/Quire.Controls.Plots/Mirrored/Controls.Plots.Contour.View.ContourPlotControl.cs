#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;
using Quire.Controls.Plots.ViewModel;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Plots.Contour.View;

public partial class ContourPlotControl
{
    public override void Dispose()
    {
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(ResizableElementAttachedProperties.IsResizingProperty, plotGrid, OnGridIsResizingChanged);
        ViewModel.Dispose();
    }

    public IEnumerable<ICalculatableInfo> CalculatableInfosWithErrors => CalculatableChildren.OfType<ICalculatableInfo>().Where((ICalculatableInfo calc) =>
    {
        if (!string.IsNullOrEmpty(calc.ErrorCode))
        {
            DebugTools.DevAssert(calc.ErrorCount == 1, null);
            return true;
        }
        DebugTools.DevAssert(calc.ErrorCount == 0, null);
        return false;
    });

    internal Size CanvasMeasureOverride(Size availableSize)
    {
        Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        Size gridCellSize = GridCellSize;
        double cellWidth = gridCellSize.Width;
        double cellHeight = gridCellSize.Height;
        Size result = new Size(cellWidth + 9.600000000000001, cellHeight + 9.600000000000001);
        plotGrid.Measure(infiniteSize);
        Size plotGridSize = plotGrid.DesiredSize;
        double extraHeight = 0.0;
        _zEquation.Measure(infiniteSize);
        Size zEquationSize;
        double zBaselineOffset;
        // Tuple deconstruction evaluates baseline offset before DesiredSize, matching Mathcad's stloc.s 7 / 8 order.
        (zBaselineOffset, zEquationSize) = (ViewModel.AreAxisExpressionsVisible ? _zEquation.TextBaselineOffset : 0.0, _zEquation.DesiredSize);
        if (zBaselineOffset > plotGridSize.Height / 12.0 * 13.0 + 0.0)
        {
            extraHeight += zBaselineOffset - (plotGridSize.Height / 12.0 * 13.0 + 0.0);
        }
        double colorBlockHeight = plotGridSize.Height / 12.0;
        _zAxisColorBlocks.Height = colorBlockHeight;
        _zAxisStackPanel.Measure(new Size(plotGridSize.Width * 10.0 / 12.0, availableSize.Height));
        _background.Height = _zAxisStackPanel.DesiredSize.Height + plotGridSize.Height + extraHeight;
        _background.Width = plotGridSize.Width;
        _background.Measure(new Size(_background.Width, _background.Height));
        double zAxisBelow = _zAxis.DesiredSize.Height - 0.0;
        double zAxisAbove = plotGridSize.Height + colorBlockHeight + 0.0;
        double maxAbove = Math.Max(zBaselineOffset, zAxisAbove);
        double maxBelow = Math.Max(zEquationSize.Height - zBaselineOffset, zAxisBelow);
        result.Height += maxAbove + maxBelow;
        result.Width += plotGridSize.Width + zEquationSize.Width;
        return result;
    }

    internal Size CanvasArrangeOverride(Size finalSize)
    {
        Size gridCellSize = GridCellSize;
        Rect backgroundRect = new Rect(gridCellSize.Width, gridCellSize.Height, _background.DesiredSize.Width, _background.DesiredSize.Height);
        _background.Arrange(backgroundRect);
        Size plotGridSize = plotGrid.DesiredSize;
        Rect plotGridRect = new Rect(gridCellSize.Width, gridCellSize.Height, plotGridSize.Width, plotGridSize.Height);
        plotGrid.Arrange(plotGridRect);
        double zBaselineOffset = ViewModel.AreAxisExpressionsVisible ? _zEquation.TextBaselineOffset : 0.0;
        double zBaselineCheck = zBaselineOffset;
        double extraHeight = 0.0;
        double zAxisBelow = _zAxis.DesiredSize.Height - 0.0;
        if (zBaselineCheck > plotGridSize.Height / 12.0 * 13.0 + 0.0)
        {
            extraHeight += zBaselineCheck - (plotGridSize.Height / 12.0 * 13.0 + 0.0);
        }
        Rect zAxisRect = new Rect(plotGridRect.Left + plotGridRect.Width / 12.0, plotGridRect.Bottom + extraHeight, _zAxisStackPanel.DesiredSize.Width, _zAxisStackPanel.DesiredSize.Height);
        _zAxisStackPanel.Arrange(zAxisRect);
        double zAxisBase = zAxisRect.Bottom - zAxisBelow;
        Rect zEquationRect = new Rect(new Point(plotGridRect.Right, zAxisBase - zBaselineOffset), _zEquation.DesiredSize);
        _zEquation.Arrange(zEquationRect);
        return DesiredSize;
    }

    public ContourPlotViewModel ViewModel { get; private set; }

    internal VisualDataCollection VisualDataCollection => ViewModel.ContourVisualDataCollection;

    // Field placement matches Mathcad metadata order and preserves <.ctor>b__13_* ordinals.
    private static readonly Type _typeOfThis;

    static ContourPlotControl()
    {
        _typeOfThis = typeof(ContourPlotControl);
        TraceLineStyleProperty = DependencyProperty.Register("TraceLineStyle", typeof(LineStyle), _typeOfThis, new PropertyMetadata(LineStyle.Solid));
        TraceLineWeightProperty = DependencyProperty.Register("TraceLineWeight", typeof(double), _typeOfThis, new PropertyMetadata(1.0));
        TraceColorProperty = DependencyProperty.Register("TraceColor", typeof(SolidColorBrush), _typeOfThis, new PropertyMetadata(Brushes.Black));
        ColorSchemeProperty = DependencyProperty.Register("ColorScheme", typeof(ColorScheme), _typeOfThis, new PropertyMetadata(ColorScheme.Rainbow));
        ShowContourValuesProperty = DependencyProperty.Register("ShowContourValues", typeof(bool), _typeOfThis, new PropertyMetadata(false));
        ToggleShowContourValuesCommand = new RoutedCommand("ToggleShowContourValues", _typeOfThis);
        SetColorSchemeCommand = VisualDataCollection.SetColorSchemeCommand;
        PropertyMetadata metadata = new PropertyMetadata(new string[9]
        {
            "11",
            "21",
            "31",
            "41",
            "51",
            "61",
            "71",
            "81",
            "101"
        });
        NumberOfPointsValuesCollectionProperty.OverrideMetadata(_typeOfThis, metadata);
        LabelFormatterProperty = VisualData.LabelFormatterProperty.AddOwner(_typeOfThis);
        _changedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
    }

    public ContourPlotControl()
    {
        ViewModel = new ContourPlotViewModel();
        ViewModel.CalculatableAdded += FireCalculatableContainerCalculatableAdded;
        ViewModel.CalculatableRemoved += FireCalculatableContainerCalculatableRemoved;
        InitializeComponent();
        _yAxis.OtherAxis = _xAxis;
        _xAxis.OtherAxis = _yAxis;
        _xAxis.ViewModel = ViewModel.XAxisViewModel;
        _yAxis.ViewModel = ViewModel.YAxisViewModel;
        _zAxis.ViewModel = ViewModel.ZAxisViewModel;
        Axes = new PlotGraphAxis[3] { _xAxis, _yAxis, _zAxis };
        ViewModel.Initialize(ZExpression, _xAxis, _yAxis);
        SetBindingsForVisualProperties();
        ViewModel.PropertyChanged += (sender, eArgs) => ViewModelPropertyChanged(eArgs.PropertyName);
        PropertyChanged += OnPropertyChanged;
        ActivePlotEquation = ZExpression;
        SubElementToResizeData = new SubElementToResizeData(plotGrid, serializeDimensions: false, isFullSize: false);
        SubElementToResizeData.Element.SizeChanged += (sender, args) => ViewModel.OnGraphSizeChanged(args.NewSize);
        DependencyPropertyChangeManager.AddPropertyChangedHandler(ResizableElementAttachedProperties.IsResizingProperty, plotGrid, OnGridIsResizingChanged);
        _xAxis.IsKeyboardFocusWithinChanged += AxisIsKeyboardFocusWithinChanged;
        _yAxis.IsKeyboardFocusWithinChanged += AxisIsKeyboardFocusWithinChanged;
        _zAxis.IsKeyboardFocusWithinChanged += AxisIsKeyboardFocusWithinChanged;
        _zAxis.NumberFormatChanged += ZAxisNumberFormatChanged;
        _xAxis.HasOriginPositioningOwner = this;
        _yAxis.HasOriginPositioningOwner = this;
        ViewModel.ModelChanged += ViewModelModelChanged;
        ViewModel.DeserializationEnded += ReadDataOnDeserializationEnd;
    }

    private void OnGridIsResizingChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        VisualDataCollection.IsResizing = ResizableElementAttachedProperties.GetIsResizing(plotGrid);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "GridCellSize")
        {
            _content.InvalidateMeasure();
        }
    }

    private EngineError PlotCalculatableError
    {
        get
        {
            if (ViewModel == null)
            {
                return null;
            }
            return ViewModel.PlotCalculatableError;
        }
    }

    private void AxisIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        DebugTools.DevAssert(args.NewValue is bool, null);
        if ((bool)args.NewValue)
        {
            PlotGraphLinearAxis axis = sender as PlotGraphLinearAxis;
            DebugTools.DevAssert(axis != null, null);
            ActiveAxis = axis;
        }
    }

    // Field placement matches Mathcad metadata order and preserves <>c::<get_EngineResultsForXamlSerialization>b__39_0.
    public static readonly DependencyProperty LabelFormatterProperty;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TickmarkFormatter LabelFormatter
    {
        get => (TickmarkFormatter)GetValue(LabelFormatterProperty);
        set => SetValue(LabelFormatterProperty, value);
    }

    private void ZAxisNumberFormatChanged(NumberFormat numberFormat, bool isLocal)
    {
        if (LabelFormatter == null || !numberFormat.Equals(LabelFormatter.DisplayNumberFormat))
        {
            LabelFormatter = new TickmarkFormatter(numberFormat);
            VisualDataCollection.PositionLabels();
        }
    }

    protected override string ErrorMessage
    {
        get
        {
            if (!ErrorIsNotNull)
            {
                return null;
            }
            return GetErrorMessage(PlotCalculatableError);
        }
    }

    protected override bool ErrorIsNotNull => PlotCalculatableError != null;

    private void ViewModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "PlotBackgroundType":
                PlotBackgroundType = ViewModel.PlotBackgroundType;
                break;
            case "PlotCalculatableError":
                _zEquation.PlotCalculatableError = PlotCalculatableError;
                ShowHideErrorFrame();
                ShowHideErrorMessage();
                break;
            case "ShowControlLevelErrorFrameForNestedError":
                ShowHideErrorFrame();
                break;
        }
    }

    private void ReadDataOnDeserializationEnd()
    {
        SubElementToResizeData.Element.Width = ViewModel.GraphWidth;
        SubElementToResizeData.Element.Height = ViewModel.GraphHeight;
    }

    private void ViewModelModelChanged()
    {
        RaiseEvent(new RoutedEventArgs(_changedEvent, this));
    }

    public override Size PlotGraphSize => new Size(SubElementToResizeData.Element.ActualWidth, SubElementToResizeData.Element.ActualHeight);

    public override PlotType PlotType
    {
        get => PlotType.Contour;
        protected set => DebugTools.DevAssert(value == PlotType.Contour, null);
    }

    private void SetBindingsForVisualProperties()
    {
        BindingOperations.SetBinding(this, ShowContourValuesProperty, new Binding
        {
            Source = VisualDataCollection,
            Path = new PropertyPath("ShowLabels"),
            Mode = BindingMode.TwoWay
        });
        BindingOperations.SetBinding(VisualDataCollection, VisualDataCollection.LabelVisibilityProperty, new Binding
        {
            Source = this,
            Path = new PropertyPath(ShowContourValuesProperty),
            Converter = Plots.View.BooleanToVisibilityConverter.Instance
        });
        BindingOperations.SetBinding(VisualDataCollection, VisualDataCollection.LabelFormatterProperty, new Binding
        {
            Source = this,
            Path = new PropertyPath(LabelFormatterProperty),
            Mode = BindingMode.OneWay
        });
    }

    // Field placement matches Mathcad metadata order and preserves <>c::<get_EngineResultsForXamlSerialization>b__39_0.
    private EngineResult[] _engineResults;

    public EngineResult[] EngineResultsForXamlSerialization
    {
        get
        {
            _engineResults = ResultOwners.Select((ICalculatableResultOwner wrapper) => wrapper.EngineResult).ToArray();
            return _engineResults;
        }
        set
        {
            _engineResults = value;
        }
    }

    public IEnumerable<ICalculatableResultOwner> ResultOwners => ViewModel.CollectHiddenAndVisibleCalculatables().OfType<ICalculatableResultOwner>();

    public override void EndInit()
    {
        base.EndInit();
        if (_engineResults != null)
        {
            ResultManager.DispatchResults(_engineResults, ResultOwners);
            _engineResults = null;
        }
        if (InCustomInitialization)
        {
            InCustomInitialization = false;
        }
    }

    public void AddEquations(XyzEquation xyzEquation)
    {
        ZExpression.EquationSExpression = xyzEquation.ZsExpression;
    }

    public override IEnumerable<ICalculatable> CalculatableChildren => ViewModel.CalculatableChildren;

    internal IPlotEquation ZExpression => _zEquation;

    protected override IEnumerable<IPlotEquation> GetPlotEquations()
    {
        yield return _zEquation;
    }

    protected override IWhiteboardItem FirstChildItem => ZExpression.ExpressionItem;

    protected override IWhiteboardItem LastChildItem => ZExpression.ScalingItem;

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

    // Static fields placed here so field order and <GetPlotEquations>d__49 ordinal match Mathcad.
    private static readonly RoutedEvent _changedEvent;
    public static readonly DependencyProperty TraceLineStyleProperty;
    public static readonly DependencyProperty TraceLineWeightProperty;
    public static readonly DependencyProperty TraceColorProperty;
    public static readonly DependencyProperty ColorSchemeProperty;
    public static readonly DependencyProperty ShowContourValuesProperty;
    public static readonly RoutedCommand ToggleShowContourValuesCommand;
    public static readonly RoutedCommand SetColorSchemeCommand;

    public ColorScheme ColorScheme
    {
        get => (ColorScheme)GetValue(ColorSchemeProperty);
        set => SetValue(ColorSchemeProperty, value);
    }

    public LineStyle TraceLineStyle
    {
        get => (LineStyle)GetValue(TraceLineStyleProperty);
        set => SetValue(TraceLineStyleProperty, value);
    }

    public double TraceLineWeight
    {
        get => (double)GetValue(TraceLineWeightProperty);
        set => SetValue(TraceLineWeightProperty, value);
    }

    public SolidColorBrush TraceColor
    {
        get => (SolidColorBrush)GetValue(TraceColorProperty);
        set => SetValue(TraceColorProperty, value);
    }

    protected override void BindCommands()
    {
        base.BindCommands();
        CommandBindings.Add(new CommandBinding(ToggleShowContourValuesCommand, ShowContourValuesExecute));
        BindChangeOriginPositioningCommand();
    }

    protected override void OnBackgroundTypeChanged(BackgroundTypeE newValue)
    {
        base.OnBackgroundTypeChanged(newValue);
        if (this.HasLocalValue(PlotBackgroundTypeProperty))
        {
            ViewModel.PlotBackgroundType = newValue;
        }
    }

    public override void ChangeOriginPositioning(bool newValue)
    {
        base.ChangeOriginPositioning(newValue);
        UpdateHasOriginPositioningOfAxes(newValue, _xAxis, _yAxis);
    }

    public bool ShowContourValues
    {
        get => (bool)GetValue(ShowContourValuesProperty);
        set => SetValue(ShowContourValuesProperty, value);
    }

    private static void ShowContourValuesExecute(object sender, ExecutedRoutedEventArgs e)
    {
        SetPropertyCommand.Toggle((DependencyObject)sender, ShowContourValuesProperty);
    }

    private void ZEquation_OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        bool isFocused = (bool)e.NewValue;
        _zEquation.ShowOrHideScaling(isFocused);
        if (isFocused)
        {
            ActiveAxis = _zAxis;
        }
    }

    protected override void HandleLostFocus(DependencyObject oldFocused, DependencyObject newFocused)
    {
        foreach (PlotGraphAxis axis in Axes)
        {
            if (axis.HandleAncestorLostFocus(oldFocused, newFocused, new List<DependencyObject> { axis }, null))
            {
                return;
            }
        }
    }

    public ContourPlotState StateForXamlSerialization
    {
        get
        {
            return new ContourPlotState
            {
                ZEquationDataObject = ZExpression.GetDataObject(),
                XAxis = PlotGraphAxis.GetPlotAxis(_xAxis),
                YAxis = PlotGraphAxis.GetPlotAxis(_yAxis),
                ZAxis = PlotGraphAxis.GetPlotAxis(_zAxis),
                ZAxisRange = _zAxis.ViewModel.AxisRangeForXamlSerialization,
                contourState = ViewModel.ContourDataObjectForXamlSerialization,
                HasOriginPositioning = ViewModel.HasOriginPositioning,
                ShowAxisExpressions = ViewModel.ShowAxisExpressions,
                PlotGraphSize = ViewModel.PlotGraphSizeDataObjectForXamlSerialization
            };
        }
        set
        {
            PlotGraphAxis.SetPlotAxis(_yAxis, value.YAxis);
            PlotGraphAxis.SetPlotAxis(_xAxis, value.XAxis);
            PlotGraphAxis.SetPlotAxis(_zAxis, value.ZAxis);
            _zAxis.ViewModel.AxisRangeForXamlSerialization = value.ZAxisRange;
            ZExpression.SetDataObject(value.ZEquationDataObject);
            ViewModel.ContourDataObjectForXamlSerialization = value.contourState;
            ViewModel.HasOriginPositioning = value.HasOriginPositioning;
            ViewModel.ShowAxisExpressions = value.ShowAxisExpressions;
            ViewModel.PlotGraphSizeDataObjectForXamlSerialization = value.PlotGraphSize;
        }
    }

    public override bool HideFromCalculationContext
    {
        get => ViewModel.HideFromCalculationContext;
        set => ViewModel.HideFromCalculationContext = value;
    }

    protected override void IsActiveRegionChanged(bool newValue)
    {
        base.IsActiveRegionChanged(newValue);
        ViewModel.IsActiveRegion = newValue;
    }

    protected override List<EquationControl> BuildEquationList()
    {
        return new List<EquationControl>
        {
            ZExpression.Exp,
            ZExpression.Scaling
        };
    }

    protected override void Commit()
    {
        ViewModel.Commit();
    }

    internal PlotGraphLinearAxis XAxis => _xAxis;

    internal PlotGraphLinearAxis YAxis => _yAxis;

    internal PlotGraphLinearAxis ZAxis => _zAxis;

    public SubElementToResizeData SubElementToResizeData { get; private set; }

    public bool PreserveAspectRatio => false;

    protected override bool ErrorFrameShouldBeVisible => base.ErrorFrameShouldBeVisible || ViewModel.ShowControlLevelErrorFrameForNestedError;

    private const int NumberOfPoints_Min = 11;

    private const int NumberOfPoints_Max = 101;

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        ViewModel.SetDataObject(d11NProvider, data as plotType);
        return null;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return ViewModel.GetDataObject(s11NProvider);
    }

    private void ZEquation_ScalingChanged(IPlotEquationScaled equation)
    {
        _zEquation.ShowOrHideScaling(IsKeyboardFocusWithin);
    }

    public override int NumberOfPointsMin => NumberOfPoints_Min;

    public override int NumberOfPointsMax => NumberOfPoints_Max;

    double IPlotControl<XyzEquation>.Width
    {
        get => base.Width;
        set => base.Width = value;
    }

    double IPlotControl<XyzEquation>.Height
    {
        get => base.Height;
        set => base.Height = value;
    }

    private ContourPlotCanvas _content;
    internal Rectangle _background;
    private Grid plotGrid;
    private ContoursView _contourLines;
    internal PlotGridSplitter PART_ySplitter;
    private PlotGraphLinearAxis _yAxis;
    internal PlotGridSplitter PART_xSplitter;
    private PlotGraphLinearAxis _xAxis;
    private StackPanel _zAxisStackPanel;
    private ItemsControl _zAxisColorBlocks;
    private PlotGraphLinearAxis _zAxis;
    private PlotEquation _zEquation;
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
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/contour/view/contourplotcontrol.xaml", UriKind.Relative);
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
                _content = (ContourPlotCanvas)target;
                break;
            case 2:
                _background = (Rectangle)target;
                break;
            case 3:
                plotGrid = (Grid)target;
                break;
            case 4:
                _contourLines = (ContoursView)target;
                break;
            case 5:
                PART_ySplitter = (PlotGridSplitter)target;
                break;
            case 6:
                _yAxis = (PlotGraphLinearAxis)target;
                break;
            case 7:
                PART_xSplitter = (PlotGridSplitter)target;
                break;
            case 8:
                _xAxis = (PlotGraphLinearAxis)target;
                break;
            case 9:
                _zAxisStackPanel = (StackPanel)target;
                break;
            case 10:
                _zAxisColorBlocks = (ItemsControl)target;
                break;
            case 11:
                _zAxis = (PlotGraphLinearAxis)target;
                break;
            case 12:
                _zEquation = (PlotEquation)target;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
