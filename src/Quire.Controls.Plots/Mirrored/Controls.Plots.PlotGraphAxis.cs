#nullable disable
#pragma warning disable

using Quire;
using Quire.Controls.Core;
using Quire.Undo;

namespace Quire.Controls.Plots;

public abstract partial class PlotGraphAxis
{
    public partial class Point2ScreenHelper
    {
        protected double _firstTickPostion;

        protected double _lastTickPositoin;

        protected double _start = -10.0;

        protected double _end = 10.0;

        protected double _axisLenght;

        public Point2ScreenHelper(double size)
        {
            _firstTickPostion = 0.0;
            _lastTickPositoin = size;
            _axisLenght = _start == _end ? 1.0 : _end - _start;
        }

        public Point2ScreenHelper(PlotGraphAxis axis)
        {
            _firstTickPostion = axis.GetAxisStartPosition();
            _lastTickPositoin = axis.GetAxisEndPosition();
            _start = axis.LinearizedStart;
            _end = axis.LinearizedEnd;
            _axisLenght = _start == _end ? 1.0 : _end - _start;
        }

        public virtual double Point2Screen(double offset)
        {
            if (double.IsNegativeInfinity(offset))
            {
                return _lastTickPositoin < _firstTickPostion ? double.MaxValue : double.MinValue;
            }
            return (offset - _start) / _axisLenght * (_lastTickPositoin - _firstTickPostion) + _firstTickPostion;
        }

    }

    private readonly double _tickMagnitudeThreashold;

    protected PlotGraphAxis(global::Quire.Controls.Plots.EquationList equationList, int defaultTickMarks, double defaultTickMagnitudeThreshold) { }

    internal void FireShowGridLinesChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void FireTickMarksChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void GridLineColorPropertyChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void HookToViewModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnEndExpressionEqnRequestFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnSecondTickExpressionEqnRequestFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnStartExpressionEqnRequestFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void UnhookFromViewModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool CanTraverse(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void HandleTraverse(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual double GetPerpendicularAxisPositionRatio() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public virtual global::System.Windows.Controls.GridSplitter Splitter { get; set; }

    private void BindCommands() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnNavigateDown(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnNavigateUp(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnNavigateRight(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnNavigateLeft(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleActiveEquationChanged(global::Quire.Controls.Plots.Presenter.IPlotEquation equation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleEquationRemoved(global::Quire.Controls.Plots.Presenter.IPlotEquation equation, int deletedItemIndex, int priviousItemIndex, global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleEquationAdded(int index, int previousIndex) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void OnNumberFormatInitialized(global::Quire.Controls.Core.NumberFormat numberFormat) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Action<double> _legendWidthChanged;

    event global::System.Action<double> global::Quire.Controls.Plots.Presenter.IPlotAxisView.LegendWidthChanged { add { } remove { } }

    private void EquationListSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool HasEquationList() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    private double LegendPrevWidth { get; set; }

    double global::Quire.Controls.Plots.Presenter.IPlotAxisView.LegendActualWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::System.Nullable<double> _pendingWidth;

    internal void SetLegendWidthWhenLayoutReady() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool CanSetLegendWidth(double width) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void InternalSetLegendWidth(double width) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual bool ShouldArrangeConvertedEquationsHorizontally() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void BindLimitLabelVisibility(global::System.Windows.Controls.Label limitLabel, global::Quire.Controls.EquationControl control) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Windows.Data.MultiBinding GetVisibilityBinding(global::Quire.Controls.EquationControl control, global::Quire.Controls.Plots.LimitLabelVisibilityMultiConverter limitLabelVisibilityMultiConverter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void LimitExpressionEqnPreviewGotKeyboardFocus(object sender, global::System.Windows.Input.KeyboardFocusChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FirstLabelMouseLeftButtonDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ActivateStartLimitEditor(bool suppressBringIntoView = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SecondLabelMouseLeftButtonDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ActivateSecondTickLimitEditor(bool suppressBringIntoView = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void LastLabelMouseLeftButtonDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ActivateEndLimitEditor(bool suppressBringIntoView = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ActivateAxisExpression(global::Quire.Controls.EquationControl eq, bool suppressBringIntoView) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ExpressionEqnNavigateOffOfEquation(global::Quire.Controls.Core.IEquationControl equationControl, global::Quire.Direction direction, bool isJumpNavigation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ActivateSecondOrBoundaryTick(bool forward, bool navigateRegionStart) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void StartExpressionEqnNavigateOffOfEquation(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void SecondTickExpressionEqn_NavigateOffOfEquation(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void EndExpressionEqnNavigateOffOfEquation(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override global::System.Windows.Media.Visual GetVisualChild(int index) => base.GetVisualChild(index);

    protected override int VisualChildrenCount { get => base.VisualChildrenCount; }

    protected virtual void SetTicksExpressionsEqnVisibility(global::System.Windows.Visibility visibility) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size constraint) => base.MeasureOverride(constraint);

    protected void ArrangeSecondTickExpressionEqn(global::System.Windows.Point secondEqnPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Rect StartEquationRectangle(global::System.Windows.Point firstEqnPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Rect SecondExpressionEquation(global::System.Windows.Point secondEqnPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Rect EndEquationRectangle(global::System.Windows.Point lastEqnPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Rect FirstLabelRectangle(global::System.Windows.Point firstLabelPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Rect SecondLabelRectangle(global::System.Windows.Point secondLabelPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Rect LastLabelRectangle(global::System.Windows.Point lastLabelPosition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public global::Quire.Controls.Plots.EquationList EquationList { get; private set; }

    public int EquationCount
    {
        get
        {
            if (!HasEquationList())
            {
                return 0;
            }
            return EquationList.EquationCount;
        }
    }

    public void AddTraceToEquation(global::Quire.Controls.Plots.Presenter.IPlotTrace trace, global::Quire.Controls.Plots.Presenter.IPlotEquation equation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void AddTraceToEquation(global::Quire.Controls.Plots.Presenter.IPlotTrace trace, global::Quire.Controls.Plots.Presenter.IPlotEquation equation, int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void RemoveTraceFromEquation(int traceIndex, global::Quire.Controls.Plots.Presenter.IPlotEquation equation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual global::System.Collections.Generic.IEnumerable<global::Quire.Controls.EquationControl> CollectElementsForTraversal() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void AddEquation(string expression) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Plots.Presenter.IEquationListItem AddEquation(int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void AddEquation(int index, global::Quire.Controls.Plots.Presenter.IEquationListItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Plots.Presenter.IEquationListItem RemoveEquation(int index, int previousIndex, global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Plots.Presenter.IPlotEquation AddEquation(global::System.Xml.XmlNode xmlNode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Plots.Presenter.IPlotEquation this[int index] { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public double Screen2Point(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public double Point2Screen(double offset) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual global::Quire.Controls.Plots.PlotGraphAxis.Point2ScreenHelper Helper { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::Quire.Controls.Plots.PlotGraphAxis _otherAxis;

    public global::Quire.Controls.Plots.PlotGraphAxis OtherAxis { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::Quire.Controls.Plots.Presenter.TickScale CreateMajorTickmaks() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Plots.Presenter.TickScale _majorTickmaks;

    internal global::Quire.Controls.Plots.Presenter.TickScale GetMajorTickmaks() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    private double TickmarksStartGap { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    protected double[] TickmarksPattern { get; set; }

    public void SetTickmarkPattern(double startGap, double[] pattern) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public global::Quire.Controls.Plots.AxisLimitEditor StartLimitEditor { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public global::Quire.Controls.Plots.AxisLimitEditor SecondTickEditor { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public global::Quire.Controls.Plots.AxisLimitEditor EndLimitEditor { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public global::System.Windows.Thickness DesiredMargin { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public bool[] TickMarksVisabilityPattern { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public global::Quire.Controls.Plots.Presenter.TickScale MinorTickmarks { get; set; }

    private global::Quire.Controls.Plots.PlotTitle _axisTitle;

    public global::Quire.Controls.Plots.PlotTitle AxisTitle { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private static void PositionTickMarkChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected abstract void PositionTickMarkChanged(int number);

    public virtual global::Quire.Pair<double, double> ClipBounds { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public static readonly global::System.Windows.DependencyProperty PositionTickMarkProperty;

    public int PositionTickMark { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => Dispatch(() => SetValue(PositionTickMarkProperty, value)); }

    protected static global::System.Windows.FrameworkPropertyMetadata CreateOrientationPropertyMetadata(global::System.Windows.Controls.Orientation defaultValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void OrientationChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected abstract void HandleOrientationChanged();

    public static readonly global::System.Windows.DependencyProperty OrientationProperty;

    public global::System.Windows.Controls.Orientation Orientation { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private static void StartChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleStartChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty StartProperty;

    public double Start { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => Dispatch(() => SetValue(StartProperty, value)); }

    protected internal double LinearizedStart { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private static void EndChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleEndChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty EndProperty;

    public double End { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => Dispatch(() => SetValue(EndProperty, value)); }

    protected internal double LinearizedEnd { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private static readonly global::System.Type _typeOfThis;

    public static readonly global::System.Windows.DependencyProperty ShowTicksProperty;

    public bool ShowTicks
    {
        get => (bool)GetValue(ShowTicksProperty);
        set => Dispatch(() => SetValue(ShowTicksProperty, value));
    }

    private static void LabelsChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleLabelChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty LabelsProperty;

    public string[] Labels { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => Dispatch(() => SetValue(LabelsProperty, value)); }

    public static readonly global::System.Windows.DependencyProperty TickMarksValuesProperty;

    public double[] TickMarksValues { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => Dispatch(() => SetValue(TickMarksValuesProperty, value)); }

    public bool HasOriginPositioning { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    internal global::Quire.Controls.Plots.IOriginPositionControler HasOriginPositioningOwner { get; set; }

    private static void DiscretePropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleDiscreteChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty IsDiscreteProperty;

    public virtual bool IsDiscrete { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => Dispatch(() => SetValue(IsDiscreteProperty, value)); }

    public static readonly global::System.Windows.DependencyProperty ShowNumbersProperty;

    public bool ShowNumbers
    {
        get => (bool)GetValue(ShowNumbersProperty);
        set => Dispatch(() => SetValue(ShowNumbersProperty, value));
    }

    private static void DisplayNumberFormatChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsDisplayNumberFormatLocal { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private void RegisterHandleTickmarksFormattingChanged(global::Quire.Controls.Core.NumberFormat oldFormat, global::Quire.Controls.Core.NumberFormat newFormat) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleTickmarksFormattingChanged(string propertyName, object newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty DisplayNumberFormatProperty;

    public global::Quire.Controls.Core.NumberFormat DisplayNumberFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private static void HandleOffsetPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static readonly global::System.Windows.DependencyPropertyKey _offsetPropertyKey;

    public static readonly global::System.Windows.DependencyProperty OffsetProperty;

    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    private static void OnScalingChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static object CoerceScaling(global::System.Windows.DependencyObject d, object baseValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static readonly global::System.Windows.DependencyProperty ScalingProperty;

    public static readonly global::System.Windows.RoutedEvent ContentChangedEvent;

    public global::Quire.Controls.Plots.Presenter.AxisScaling Scaling { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    protected abstract void MouseMoveDraw(global::System.Windows.Media.DrawingContext context);

    public event global::System.Action<int, int> RequestAddEquation;

    public event global::System.Action<global::Quire.Controls.Plots.Presenter.IPlotEquation, int, int, global::Quire.Direction> EquationRemoved;

    public event global::System.Action Changed;

    public event global::System.Action<global::Quire.Controls.Plots.Presenter.IPlotEquation> ActiveEquationChanged;

    public event global::System.Action<global::Quire.Controls.Core.NumberFormat, bool> NumberFormatChanged;

    internal event global::System.Action ShowTicksChanged;

    internal event global::System.Action ShowNumbersChanged;

    internal event global::System.Action ShowGridLinesChanged;

    internal event global::System.Action TickMarksChangedEvent;

    protected void FireChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void ShowLimitsExpressions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void HideLimitsExpressions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void ShowTicksPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ShowTicksPropertyChanged(bool newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireShowTicksChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void ShowNumbersPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ShowNumbersPropertyChanged(bool newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireShowNumbersChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void SetOtherAxis(global::Quire.Controls.Plots.PlotGraphAxis value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public abstract global::Quire.Controls.Plots.Presenter.Positions Position { get; set; }

    public abstract void UpdateAxisOriginPositions();

    private double Length { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    protected virtual double GetAxisStartPosition() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual double GetAxisEndPosition() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Nullable<bool> _isLinearizedRangeAssertEnabled;

    public global::System.Nullable<bool> IsLinearizedRangeAssertEnabled { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool HasAxisError() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Nullable<double> LinearizeStartEnd(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private double Linearize(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected double UnLinearize(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static global::System.Windows.Media.Animation.DoubleAnimation CreateDoubleAnimation(double toValue, global::System.Windows.DependencyProperty property, double seconds) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetRange(double start, double end) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void ClearDisplayNumberFormatWithCommand() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public abstract void MoveToValue(double value);

    public abstract void MoveByValue(double value);

    public abstract bool FindClosestTickValueToZero(ref int index);

    protected global::System.Windows.Media.FormattedText GetTickValueTextByTickNumber(int tickNumber, global::System.Windows.Media.Brush stroke) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int PositionToTickNumber(global::Quire.Controls.Plots.Presenter.Positions position) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual int PositionValue { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    protected abstract void ApplyTickNumber(int value, bool b);

    public void SetStartExpression(string expression) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetEndExpression(string expression) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string GetFormattedMin() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string GetFormattedMax() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetScaling(global::Quire.Controls.Plots.Presenter.AxisScaling axisScaling) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public event global::System.Action<object> ValueToScreenChanged;

    private void FireRangeChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    double global::Quire.Controls.Plots.Presenter.IValueToScreen.CoerceScreenValue(double screenValue, bool toEdges) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Plots.Presenter.EdgeBelongingStatus GetEdgeBelongingStatusForCoordinate(double d) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    double global::Quire.Controls.Plots.Presenter.IValueToScreen.ValueToScreen(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    double global::Quire.Controls.Plots.Presenter.IValueToScreen.ScreenToValue(double value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal static global::Quire.Controls.Plots.PlotAxis GetPlotAxis(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal static void SetPlotAxis(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView, global::Quire.Controls.Plots.PlotAxis plotAxis) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal bool HandleAncestorLostFocus(global::System.Windows.DependencyObject element, global::System.Windows.DependencyObject focusedElement, global::System.Collections.Generic.IEnumerable<global::System.Windows.DependencyObject> axisElements, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> axisMarkers) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    private static void FirePropertyChanged(global::Quire.Controls.Plots.PlotGraphAxis owner, string propertyName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void FirePropertyChanged(string propertyName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ClearBinding(global::System.Windows.DependencyProperty dependencyProperty) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetBinding(global::System.Windows.DependencyProperty dependencyProperty) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
