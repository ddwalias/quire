#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

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

        internal virtual bool Contains(double coordinate)
        {
            if (coordinate >= _start)
            {
                return coordinate <= _end;
            }
            return false;
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

    protected PlotGraphAxis(global::Quire.Controls.Plots.EquationList equationList, int defaultTickMarks, double defaultTickMagnitudeThreshold)
    {
        EquationList = equationList;
        if (HasEquationList())
        {
            EquationList.RequestAddEquation += HandleEquationAdded;
            EquationList.EquationRemoved += HandleEquationRemoved;
            EquationList.ActiveEquationChanged += HandleActiveEquationChanged;
            EquationList.SizeChanged += EquationListSizeChanged;
        }
        TickMarks = defaultTickMarks;
        _tickMagnitudeThreashold = defaultTickMagnitudeThreshold;
        InitializeLimitLabel(_firstLabel, FirstLabelMouseLeftButtonDown);
        BindLimitLabelVisibility(_firstLabel, StartExpressionEqn);
        InitializeLimitLabel(_secondLabel, SecondLabelMouseLeftButtonDown);
        MultiBinding visibilityBinding = GetVisibilityBinding(SecondTickExpressionEqn, SecondTicktLabelVisibilityMultiConverter.Instance);
        visibilityBinding.Bindings.Add(new Binding
        {
            Source = this,
            Path = new PropertyPath(ScalingProperty)
        });
        visibilityBinding.Bindings.Add(new Binding
        {
            Source = this,
            Path = new PropertyPath(PlotGraphAxisBase.TickMarksProperty)
        });
        _secondLabel.SetBinding(UIElement.VisibilityProperty, visibilityBinding);
        InitializeLimitLabel(_lastLabel, LastLabelMouseLeftButtonDown);
        BindLimitLabelVisibility(_lastLabel, EndExpressionEqn);
        StartExpressionEqn.PreviewGotKeyboardFocus += LimitExpressionEqnPreviewGotKeyboardFocus;
        SecondTickExpressionEqn.PreviewGotKeyboardFocus += LimitExpressionEqnPreviewGotKeyboardFocus;
        EndExpressionEqn.PreviewGotKeyboardFocus += LimitExpressionEqnPreviewGotKeyboardFocus;
        BindCommands();
    }

    internal void FireShowGridLinesChanged()
    {
        // Read the event once so the null check and the invocation use the same delegate.
        Action showGridLinesChanged = ShowGridLinesChanged;
        if (showGridLinesChanged != null)
        {
            showGridLinesChanged();
        }
        FireChanged();
    }

    internal void FireTickMarksChanged()
    {
        // Read the event once so the null check and the invocation use the same delegate.
        Action tickMarksChanged = TickMarksChangedEvent;
        if (tickMarksChanged != null)
        {
            tickMarksChanged();
        }
    }

    internal void GridLineColorPropertyChanged()
    {
        FireChanged();
    }

    protected override void HookToViewModel()
    {
        ViewModel.HookupView(this);
        ViewModel.CanTraverseEvent += CanTraverse;
        ViewModel.HandleTraverseEvent += HandleTraverse;
        StartExpressionEqn.RequestFocus += OnStartExpressionEqnRequestFocus;
        SecondTickExpressionEqn.RequestFocus += OnSecondTickExpressionEqnRequestFocus;
        EndExpressionEqn.RequestFocus += OnEndExpressionEqnRequestFocus;
        SetBinding(ShowTicksProperty);
        SetBinding(ShowNumbersProperty);
    }

    private void OnEndExpressionEqnRequestFocus()
    {
        ViewModel.LimitEquationRequestFocus();
        ActivateEndLimitEditor(false);
    }

    private void OnSecondTickExpressionEqnRequestFocus()
    {
        ViewModel.LimitEquationRequestFocus();
        ActivateSecondTickLimitEditor(false);
    }

    private void OnStartExpressionEqnRequestFocus()
    {
        ViewModel.LimitEquationRequestFocus();
        ActivateStartLimitEditor(false);
    }

    protected override void UnhookFromViewModel()
    {
        ViewModel.CanTraverseEvent -= CanTraverse;
        ViewModel.HandleTraverseEvent -= HandleTraverse;
        StartExpressionEqn.RequestFocus -= OnStartExpressionEqnRequestFocus;
        SecondTickExpressionEqn.RequestFocus -= OnSecondTickExpressionEqnRequestFocus;
        EndExpressionEqn.RequestFocus -= OnEndExpressionEqnRequestFocus;
        ClearBinding(ShowTicksProperty);
        ClearBinding(ShowNumbersProperty);
    }

    public bool CanTraverse(global::System.Windows.Input.FocusNavigationDirection direction)
    {
        if (direction != global::System.Windows.Input.FocusNavigationDirection.Next)
        {
            return StartExpressionEqn.IsKeyboardFocusWithin;
        }
        return EndExpressionEqn.IsKeyboardFocusWithin;
    }

    public void HandleTraverse(global::System.Windows.Input.FocusNavigationDirection direction)
    {
        if (direction == global::System.Windows.Input.FocusNavigationDirection.Next)
        {
            if (Orientation == global::System.Windows.Controls.Orientation.Horizontal)
            {
                OnNavigateLeft(this, null);
            }
            else
            {
                OnNavigateUp(this, null);
            }
            return;
        }
        if (Orientation == global::System.Windows.Controls.Orientation.Horizontal)
        {
            OnNavigateRight(this, null);
        }
        else
        {
            OnNavigateDown(this, null);
        }
    }

    public virtual double GetPerpendicularAxisPositionRatio()
    {
        DebugTools.DevAssert(false);
        return 0.5;
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    public virtual global::System.Windows.Controls.GridSplitter Splitter { get; set; }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateLeft, OnNavigateLeft));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateRight, OnNavigateRight));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateUp, OnNavigateUp));
        CommandBindings.Add(new CommandBinding(EquationCommands.NavigateDown, OnNavigateDown));
    }

    private void OnNavigateDown(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        ExpressionEqnNavigateOffOfEquation(global::Quire.Wpf.WpfUtils.GetFocusedAncestorOfTypeElement<EquationControl>(this, false), Direction.Down, false);
    }

    private void OnNavigateUp(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        ExpressionEqnNavigateOffOfEquation(global::Quire.Wpf.WpfUtils.GetFocusedAncestorOfTypeElement<EquationControl>(this, false), Direction.Up, false);
    }

    private void OnNavigateRight(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        ExpressionEqnNavigateOffOfEquation(global::Quire.Wpf.WpfUtils.GetFocusedAncestorOfTypeElement<EquationControl>(this, false), Direction.Right, false);
    }

    private void OnNavigateLeft(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        ExpressionEqnNavigateOffOfEquation(global::Quire.Wpf.WpfUtils.GetFocusedAncestorOfTypeElement<EquationControl>(this, false), Direction.Left, false);
    }

    private void HandleActiveEquationChanged(global::Quire.Controls.Plots.Presenter.IPlotEquation equation)
    {
        if (ActiveEquationChanged != null)
        {
            ActiveEquationChanged(equation);
        }
    }

    private void HandleEquationRemoved(global::Quire.Controls.Plots.Presenter.IPlotEquation equation, int deletedItemIndex, int priviousItemIndex, global::Quire.Direction direction)
    {
        if (EquationRemoved != null)
        {
            EquationRemoved(equation, deletedItemIndex, priviousItemIndex, direction);
        }
    }

    private void HandleEquationAdded(int index, int previousIndex)
    {
        if (RequestAddEquation != null)
        {
            RequestAddEquation(index, previousIndex);
        }
    }

    public void OnNumberFormatInitialized(global::Quire.Controls.Core.NumberFormat numberFormat)
    {
        DebugTools.DevAssert(numberFormat != null);
        DisplayNumberFormat = numberFormat.Clone();
    }

    private global::System.Action<double> _legendWidthChanged;

    event global::System.Action<double> global::Quire.Controls.Plots.Presenter.IPlotAxisView.LegendWidthChanged
    {
        add
        {
            _legendWidthChanged = (global::System.Action<double>)global::System.Delegate.Combine(_legendWidthChanged, value);
        }
        remove
        {
            _legendWidthChanged = (global::System.Action<double>)global::System.Delegate.Remove(_legendWidthChanged, value);
        }
    }

    private void EquationListSizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e)
    {
        if (e.PreviousSize.Width != e.NewSize.Width)
        {
            LegendPrevWidth = EquationList.ActualWidth;
            if (_legendWidthChanged != null)
            {
                _legendWidthChanged(e.NewSize.Width);
            }
        }
    }

    private bool HasEquationList()
    {
        return EquationList != null;
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    private double LegendPrevWidth { get; set; }

    double global::Quire.Controls.Plots.Presenter.IPlotAxisView.LegendActualWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    internal void SetLegendWidthWhenLayoutReady()
    {
        if (_pendingWidth.HasValue)
        {
            double width = _pendingWidth.Value;
            _pendingWidth = null;
            InternalSetLegendWidth(width);
        }
    }

    private bool CanSetLegendWidth(double width)
    {
        if (HasEquationList())
        {
            return width != 0.0;
        }
        return false;
    }

    private void InternalSetLegendWidth(double width)
    {
        if (!CanSetLegendWidth(width))
        {
            return;
        }
        PlotControl plotControl = global::Quire.Wpf.WpfUtils.TryFindVisualAncestor<PlotControl>(this, false);
        if (DebugTools.AssertNotNull(plotControl).Passed)
        {
            if (global::Quire.Controls.Core.Serialization.SerializationHelperUtils.IsConvertedFromMathcad14(plotControl) && ShouldArrangeConvertedEquationsHorizontally())
            {
                EquationList.ArrangeEquationsHorizontally();
                return;
            }
            EquationList.Width = width;
        }
    }

    protected virtual bool ShouldArrangeConvertedEquationsHorizontally()
    {
        return false;
    }

    private void BindLimitLabelVisibility(global::System.Windows.Controls.Label limitLabel, global::Quire.Controls.EquationControl control)
    {
        limitLabel.SetBinding(global::System.Windows.UIElement.VisibilityProperty, GetVisibilityBinding(control, LimitLabelVisibilityMultiConverter.Instance));
    }

    private global::System.Windows.Data.MultiBinding GetVisibilityBinding(global::Quire.Controls.EquationControl control, global::Quire.Controls.Plots.LimitLabelVisibilityMultiConverter limitLabelVisibilityMultiConverter)
    {
        return new MultiBinding
        {
            Converter = limitLabelVisibilityMultiConverter,
            Bindings =
            {
                new Binding
                {
                    Source = control,
                    Path = new PropertyPath(global::System.Windows.UIElement.OpacityProperty)
                },
                new Binding
                {
                    Source = this,
                    Path = new PropertyPath(IsDiscreteProperty)
                },
                new Binding
                {
                    Source = this,
                    Path = new PropertyPath(ShowNumbersProperty)
                }
            }
        };
    }

    private static void LimitExpressionEqnPreviewGotKeyboardFocus(object sender, global::System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        if (sender is EquationControl { Opacity: 0.0 } equationControl)
        {
            PlotGraphAxisBase.ShowLimit(equationControl);
        }
    }

    private void FirstLabelMouseLeftButtonDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e)
    {
        ActivateStartLimitEditor(true);
        e.Handled = true;
    }

    public void ActivateStartLimitEditor(bool suppressBringIntoView = false)
    {
        ActivateAxisExpression(StartExpressionEqn, suppressBringIntoView);
    }

    private void SecondLabelMouseLeftButtonDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e)
    {
        ActivateSecondTickLimitEditor(true);
        e.Handled = true;
    }

    public void ActivateSecondTickLimitEditor(bool suppressBringIntoView = false)
    {
        ActivateAxisExpression(SecondTickExpressionEqn, suppressBringIntoView);
    }

    private void LastLabelMouseLeftButtonDown(object sender, global::System.Windows.Input.MouseButtonEventArgs e)
    {
        ActivateEndLimitEditor(true);
        e.Handled = true;
    }

    public void ActivateEndLimitEditor(bool suppressBringIntoView = false)
    {
        ActivateAxisExpression(EndExpressionEqn, suppressBringIntoView);
    }

    private void ActivateAxisExpression(global::Quire.Controls.EquationControl eq, bool suppressBringIntoView)
    {
        ShowLimitsExpressions();
        AttachedProperties.SuppressBringIntoViewIfNeededAndExecute(eq, () =>
        {
            eq.Focus();
        }, suppressBringIntoView);
    }

    private void ExpressionEqnNavigateOffOfEquation(global::Quire.Controls.Core.IEquationControl equationControl, global::Quire.Direction direction, bool isJumpNavigation)
    {
        if (!isJumpNavigation)
        {
            if (equationControl == StartExpressionEqn)
            {
                StartExpressionEqnNavigateOffOfEquation(direction);
                return;
            }
            if (equationControl == SecondTickExpressionEqn)
            {
                SecondTickExpressionEqn_NavigateOffOfEquation(direction);
                return;
            }
            if (equationControl == EndExpressionEqn)
            {
                EndExpressionEqnNavigateOffOfEquation(direction);
            }
        }
    }

    private void ActivateSecondOrBoundaryTick(bool forward, bool navigateRegionStart)
    {
        if (Scaling != global::Quire.Controls.Plots.Presenter.AxisScaling.Logarithmic)
        {
            ActivateSecondTickLimitEditor();
            if (navigateRegionStart)
            {
                SecondTickExpressionEqn.NavigateRegionStart();
            }
            else
            {
                SecondTickExpressionEqn.NavigateRegionEnd();
            }
        }
        else if (forward)
        {
            ActivateEndLimitEditor();
            if (navigateRegionStart)
            {
                EndExpressionEqn.NavigateRegionStart();
            }
            else
            {
                EndExpressionEqn.NavigateRegionEnd();
            }
        }
        else
        {
            ActivateStartLimitEditor();
            if (navigateRegionStart)
            {
                StartExpressionEqn.NavigateRegionStart();
            }
            else
            {
                StartExpressionEqn.NavigateRegionEnd();
            }
        }
    }

    private void StartExpressionEqnNavigateOffOfEquation(global::Quire.Direction direction)
    {
        if (direction == global::Quire.Direction.Right)
        {
            ActivateSecondOrBoundaryTick(forward: true, navigateRegionStart: true);
        }
        switch (Orientation)
        {
            case global::System.Windows.Controls.Orientation.Horizontal:
                if (direction == global::Quire.Direction.Down)
                {
                    ActivateSecondOrBoundaryTick(forward: true, navigateRegionStart: true);
                }
                break;
            case global::System.Windows.Controls.Orientation.Vertical:
                if (direction == global::Quire.Direction.Up)
                {
                    ActivateSecondOrBoundaryTick(forward: true, navigateRegionStart: true);
                }
                break;
        }
    }

    protected virtual void SecondTickExpressionEqn_NavigateOffOfEquation(global::Quire.Direction direction)
    {
        switch (direction)
        {
            case global::Quire.Direction.Right:
                ActivateEndLimitEditor();
                break;
            case global::Quire.Direction.Left:
                ActivateStartLimitEditor();
                break;
            case global::Quire.Direction.Up:
                if (Orientation == global::System.Windows.Controls.Orientation.Horizontal)
                {
                    ActivateStartLimitEditor();
                }
                else
                {
                    ActivateEndLimitEditor();
                }
                break;
            case global::Quire.Direction.Down:
                if (Orientation == global::System.Windows.Controls.Orientation.Horizontal)
                {
                    ActivateEndLimitEditor();
                }
                else
                {
                    ActivateStartLimitEditor();
                }
                break;
        }
        if (direction == global::Quire.Direction.Left)
        {
            StartExpressionEqn.NavigateRegionEnd();
        }
        else if (EndExpressionEqn.IsKeyboardFocusWithin)
        {
            EndExpressionEqn.NavigateRegionStart();
        }
        else
        {
            StartExpressionEqn.NavigateRegionStart();
        }
    }

    private void EndExpressionEqnNavigateOffOfEquation(global::Quire.Direction direction)
    {
        if (direction == global::Quire.Direction.Left)
        {
            ActivateSecondOrBoundaryTick(forward: false, navigateRegionStart: false);
            return;
        }
        switch (Orientation)
        {
            case global::System.Windows.Controls.Orientation.Horizontal:
                if (direction == global::Quire.Direction.Up)
                {
                    ActivateSecondOrBoundaryTick(forward: false, navigateRegionStart: true);
                }
                break;
            case global::System.Windows.Controls.Orientation.Vertical:
                if (direction == global::Quire.Direction.Down)
                {
                    ActivateSecondOrBoundaryTick(forward: false, navigateRegionStart: true);
                }
                break;
        }
        SecondTickExpressionEqn.NavigateRegionStart();
    }

    protected override global::System.Windows.Media.Visual GetVisualChild(int index)
    {
        return VisualChildren[index];
    }

    protected override int VisualChildrenCount => VisualChildren.Count;

    protected virtual void SetTicksExpressionsEqnVisibility(global::System.Windows.Visibility visibility)
    {
        StartExpressionEqn.Visibility = visibility;
        SecondTickExpressionEqn.Visibility = visibility;
        EndExpressionEqn.Visibility = visibility;
    }

    protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size constraint)
    {
        SetTicksExpressionsEqnVisibility(IsDiscrete ? Visibility.Collapsed : Visibility.Visible);
        Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        StartExpressionEqn.Measure(availableSize);
        EndExpressionEqn.Measure(availableSize);
        SecondTickExpressionEqn.Measure(availableSize);
        _firstLabel.Measure(availableSize);
        _lastLabel.Measure(availableSize);
        _secondLabel.Measure(availableSize);
        DebugTools.DevAssert((StartExpressionEqn.Visibility != Visibility.Collapsed || _firstLabel.Visibility == StartExpressionEqn.Visibility) && (SecondTickExpressionEqn.Visibility != Visibility.Collapsed || _secondLabel.Visibility == SecondTickExpressionEqn.Visibility) && (EndExpressionEqn.Visibility != Visibility.Collapsed || _lastLabel.Visibility == EndExpressionEqn.Visibility), null);
        return constraint;
    }

    protected void ArrangeSecondTickExpressionEqn(global::System.Windows.Point secondEqnPosition)
    {
        if (Scaling != global::Quire.Controls.Plots.Presenter.AxisScaling.Logarithmic)
        {
            if (SecondTickExpressionEqn.Visibility == global::System.Windows.Visibility.Hidden)
            {
                SecondTickExpressionEqn.Visibility = global::System.Windows.Visibility.Visible;
            }
            SecondTickExpressionEqn.Arrange(SecondExpressionEquation(secondEqnPosition));
            return;
        }
        if (SecondTickExpressionEqn.Visibility == global::System.Windows.Visibility.Visible)
        {
            SecondTickExpressionEqn.Visibility = global::System.Windows.Visibility.Hidden;
        }
    }

    protected global::System.Windows.Rect StartEquationRectangle(global::System.Windows.Point firstEqnPosition)
    {
        return new global::System.Windows.Rect(firstEqnPosition, StartExpressionEqn.DesiredSize);
    }

    protected global::System.Windows.Rect SecondExpressionEquation(global::System.Windows.Point secondEqnPosition)
    {
        return new global::System.Windows.Rect(secondEqnPosition, SecondTickExpressionEqn.DesiredSize);
    }

    protected global::System.Windows.Rect EndEquationRectangle(global::System.Windows.Point lastEqnPosition)
    {
        return new global::System.Windows.Rect(lastEqnPosition, EndExpressionEqn.DesiredSize);
    }

    protected global::System.Windows.Rect FirstLabelRectangle(global::System.Windows.Point firstLabelPosition)
    {
        return new global::System.Windows.Rect(firstLabelPosition, _firstLabel.DesiredSize);
    }

    protected global::System.Windows.Rect SecondLabelRectangle(global::System.Windows.Point secondLabelPosition)
    {
        return new global::System.Windows.Rect(secondLabelPosition, _secondLabel.DesiredSize);
    }

    protected global::System.Windows.Rect LastLabelRectangle(global::System.Windows.Point lastLabelPosition)
    {
        return new global::System.Windows.Rect(lastLabelPosition, _lastLabel.DesiredSize);
    }

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

    public void AddTraceToEquation(global::Quire.Controls.Plots.Presenter.IPlotTrace trace, global::Quire.Controls.Plots.Presenter.IPlotEquation equation)
    {
        if (!HasEquationList())
        {
            return;
        }
        EquationList.AddTraceToEquation(trace, equation);
    }

    public void AddTraceToEquation(global::Quire.Controls.Plots.Presenter.IPlotTrace trace, global::Quire.Controls.Plots.Presenter.IPlotEquation equation, int index)
    {
        if (!HasEquationList())
        {
            return;
        }
        EquationList.AddTraceToEquation(trace, equation, index);
    }

    public void RemoveTraceFromEquation(int traceIndex, global::Quire.Controls.Plots.Presenter.IPlotEquation equation)
    {
        if (!HasEquationList())
        {
            return;
        }
        EquationList.RemoveTraceFromEquation(traceIndex, equation);
    }

    public virtual global::System.Collections.Generic.IEnumerable<global::Quire.Controls.EquationControl> CollectElementsForTraversal()
    {
        if (!IsDiscrete)
        {
            yield return StartExpressionEqn;
            if (Scaling == global::Quire.Controls.Plots.Presenter.AxisScaling.Linear && SecondTickExpressionEqn.IsVisible)
            {
                yield return SecondTickExpressionEqn;
            }
            yield return EndExpressionEqn;
        }
    }

    // Compile-order device: Mathcad declares this field after CollectElementsForTraversal and before set_PositionTickMark (closure ordinals).
    private global::System.Nullable<double> _pendingWidth;

    public void AddEquation(string expression)
    {
        if (!HasEquationList())
        {
            return;
        }
        EquationList.AddEquation(expression);
    }

    public global::Quire.Controls.Plots.Presenter.IEquationListItem AddEquation(int index)
    {
        if (!HasEquationList())
        {
            return null;
        }
        return EquationList.AddEquation(index);
    }

    public void AddEquation(int index, global::Quire.Controls.Plots.Presenter.IEquationListItem item)
    {
        if (!HasEquationList())
        {
            return;
        }
        EquationList.AddItemToLegend(index, item, item != null);
    }

    public global::Quire.Controls.Plots.Presenter.IEquationListItem RemoveEquation(int index, int previousIndex, global::Quire.Direction direction)
    {
        if (!HasEquationList())
        {
            return null;
        }
        return EquationList.RemoveItemAt(index, previousIndex, direction);
    }

    public global::Quire.Controls.Plots.Presenter.IPlotEquation AddEquation(global::System.Xml.XmlNode xmlNode)
    {
        if (!HasEquationList())
        {
            return null;
        }
        return EquationList.AddEquation(xmlNode);
    }

    public global::Quire.Controls.Plots.Presenter.IPlotEquation this[int index]
    {
        get
        {
            if (!HasEquationList())
            {
                return null;
            }
            return EquationList[index];
        }
    }

    public double Screen2Point(double value)
    {
        return (value - GetAxisStartPosition()) / (GetAxisEndPosition() - GetAxisStartPosition()) * (LinearizedEnd - LinearizedStart) + LinearizedStart;
    }

    public double Point2Screen(double offset)
    {
        return Helper.Point2Screen(offset);
    }

    public virtual global::Quire.Controls.Plots.PlotGraphAxis.Point2ScreenHelper Helper
    {
        get
        {
            return new Point2ScreenHelper(this);
        }
    }

    private global::Quire.Controls.Plots.PlotGraphAxis _otherAxis;

    public global::Quire.Controls.Plots.PlotGraphAxis OtherAxis
    {
        get
        {
            return _otherAxis;
        }
        set
        {
            SetOtherAxis(value);
        }
    }

    private global::Quire.Controls.Plots.Presenter.TickScale CreateMajorTickmaks()
    {
        return new global::Quire.Controls.Plots.Presenter.TickScale
        {
            Count = TickMarks,
            MagnitudeThreashold = _tickMagnitudeThreashold,
            UseMagnitudeThreashold = Scaling == global::Quire.Controls.Plots.Presenter.AxisScaling.Linear,
            Labels = Labels != null ? global::System.Linq.Enumerable.ToArray(Labels) : null,
            Pattern = TickmarksPattern,
            Show = ShowTicks,
            StartGap = TickmarksStartGap,
            VisabilityPattern = TickMarksVisabilityPattern,
        };
    }

    private global::Quire.Controls.Plots.Presenter.TickScale _majorTickmaks;

    internal global::Quire.Controls.Plots.Presenter.TickScale GetMajorTickmaks() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    private double TickmarksStartGap { get; set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    protected double[] TickmarksPattern { get; set; }

    public void SetTickmarkPattern(double startGap, double[] pattern)
    {
        TickmarksStartGap = startGap;
        TickmarksPattern = pattern;
    }

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

    public global::Quire.Controls.Plots.PlotTitle AxisTitle
    {
        get
        {
            return _axisTitle;
        }
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    private static void PositionTickMarkChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        PlotGraphAxis plotGraphAxis = (PlotGraphAxis)dependencyObject;
        int number = (int)arguments.NewValue;
        plotGraphAxis.PositionTickMarkChanged(number);
        FirePropertyChanged(dependencyObject as PlotGraphAxis, "PositionTickmrak");
    }

    protected abstract void PositionTickMarkChanged(int number);

    public virtual global::Quire.Pair<double, double> ClipBounds
    {
        get
        {
            return new global::Quire.Pair<double, double>(LinearizedStart, LinearizedEnd);
        }
    }

    public static readonly global::System.Windows.DependencyProperty PositionTickMarkProperty;

    public int PositionTickMark { get => (int)GetValue(PositionTickMarkProperty); set => Dispatch(() => SetValue(PositionTickMarkProperty, value)); }

    protected static global::System.Windows.FrameworkPropertyMetadata CreateOrientationPropertyMetadata(global::System.Windows.Controls.Orientation defaultValue)
    {
        return new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.AffectsArrange, OrientationChanged);
    }

    private static void OrientationChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        ((PlotGraphAxis)dependencyObject).HandleOrientationChanged();
    }

    protected abstract void HandleOrientationChanged();

    public static readonly global::System.Windows.DependencyProperty OrientationProperty;

    public global::System.Windows.Controls.Orientation Orientation
    {
        get => (global::System.Windows.Controls.Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    private static void StartChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        PlotGraphAxis axis = (PlotGraphAxis)dependencyObject;
        if (axis == null)
        {
            return;
        }
        if (axis.IsLinearizedRangeAssertEnabled.HasValue)
        {
            axis.IsLinearizedRangeAssertEnabled = true;
        }
        axis.HandleStartChanged();
    }

    protected virtual void HandleStartChanged()
    {
        FirePropertyChanged("StartValue");
    }

    public static readonly global::System.Windows.DependencyProperty StartProperty;

    public double Start { get => (double)GetValue(StartProperty); set => Dispatch(() => SetValue(StartProperty, value)); }

    protected internal double LinearizedStart
    {
        get
        {
            return LinearizeStartEnd(Start).GetValueOrDefault(double.MinValue);
        }
    }

    private static void EndChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        PlotGraphAxis axis = (PlotGraphAxis)dependencyObject;
        if (axis == null)
        {
            return;
        }
        if (axis.IsLinearizedRangeAssertEnabled.HasValue)
        {
            axis.IsLinearizedRangeAssertEnabled = true;
        }
        axis.HandleEndChanged();
    }

    protected virtual void HandleEndChanged()
    {
        FirePropertyChanged("EndValue");
    }

    public static readonly global::System.Windows.DependencyProperty EndProperty;

    public double End { get => (double)GetValue(EndProperty); set => Dispatch(() => SetValue(EndProperty, value)); }

    protected internal double LinearizedEnd
    {
        get
        {
            return LinearizeStartEnd(End).GetValueOrDefault(double.MaxValue);
        }
    }

    static PlotGraphAxis()
    {
        _typeOfThis = typeof(PlotGraphAxis);
        PositionTickMarkProperty = DependencyProperty.Register("PositionTickMark", typeof(int), _typeOfThis, new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, PositionTickMarkChanged));
        OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), _typeOfThis, CreateOrientationPropertyMetadata(Orientation.Horizontal));
        StartProperty = DependencyProperty.Register("StartValue", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(-10.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, StartChanged));
        EndProperty = DependencyProperty.Register("EndValue", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, EndChanged));
        LabelsProperty = DependencyProperty.Register("Labels", typeof(string[]), _typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, LabelsChanged));
        TickMarksValuesProperty = DependencyProperty.Register("TickMarksValues", typeof(double[]), _typeOfThis);
        IsDiscreteProperty = DependencyProperty.Register("IsDiscrete", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(DiscretePropertyChanged));
        _offsetPropertyKey = DependencyProperty.RegisterReadOnly("Offset", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(HandleOffsetPropertyChanged));
        OffsetProperty = _offsetPropertyKey.DependencyProperty;
        ScalingProperty = DependencyProperty.Register("Scaling", typeof(AxisScaling), _typeOfThis, new FrameworkPropertyMetadata(AxisScaling.Linear, FrameworkPropertyMetadataOptions.AffectsArrange, OnScalingChanged, CoerceScaling));
        ShowTicksProperty = PlotCommonProperties.ShowTicksProperty.AddOwner(_typeOfThis, PlotCommonProperties.CreateShowTicksMetadata(ShowTicksPropertyChanged));
        ShowNumbersProperty = PlotCommonProperties.ShowNumbersProperty.AddOwner(_typeOfThis, PlotCommonProperties.CreateShowNumbersMetadata(ShowNumbersPropertyChanged));
        DisplayNumberFormatProperty = EquationControl.DisplayNumberFormatProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(DisplayNumberFormatChanged));
        ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
    }

    public static readonly global::System.Windows.DependencyProperty ShowTicksProperty;

    public bool ShowTicks
    {
        get => (bool)GetValue(ShowTicksProperty);
        set => Dispatch(() => SetValue(ShowTicksProperty, value));
    }

    private static void LabelsChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        PlotGraphAxis axis = (PlotGraphAxis)d;
        if (axis != null)
        {
            axis.HandleLabelChanged();
        }
    }

    protected virtual void HandleLabelChanged()
    {
        if (!IsDiscrete)
        {
            _firstLabel.Content = Labels[0];
            _lastLabel.Content = Labels[Labels.Length - 1];
            if (Labels.Length > 1)
            {
                _secondLabel.Content = Labels[1];
            }
        }
    }

    public static readonly global::System.Windows.DependencyProperty LabelsProperty;

    public string[] Labels { get => (string[])GetValue(LabelsProperty); set => Dispatch(() => SetValue(LabelsProperty, value)); }

    public static readonly global::System.Windows.DependencyProperty TickMarksValuesProperty;

    public double[] TickMarksValues { get => (double[])GetValue(TickMarksValuesProperty); set => Dispatch(() => SetValue(TickMarksValuesProperty, value)); }

    public bool HasOriginPositioning
    {
        get
        {
            if (HasOriginPositioningOwner != null)
            {
                return HasOriginPositioningOwner.HasOriginPositioning;
            }
            return false;
        }
        set
        {
            if (HasOriginPositioningOwner != null)
            {
                HasOriginPositioningOwner.HasOriginPositioning = value;
            }
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("auto-property layout placeholder")]
    internal global::Quire.Controls.Plots.IOriginPositionControler HasOriginPositioningOwner { get; set; }

    private static void DiscretePropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        PlotGraphAxis axis = (PlotGraphAxis)dependencyObject;
        if (axis != null)
        {
            axis.HandleDiscreteChanged();
        }
    }

    protected virtual void HandleDiscreteChanged()
    {

    }

    public static readonly global::System.Windows.DependencyProperty IsDiscreteProperty;

    public virtual bool IsDiscrete { get => (bool)GetValue(IsDiscreteProperty); set => Dispatch(() => SetValue(IsDiscreteProperty, value)); }

    public static readonly global::System.Windows.DependencyProperty ShowNumbersProperty;

    public bool ShowNumbers
    {
        get => (bool)GetValue(ShowNumbersProperty);
        set => Dispatch(() => SetValue(ShowNumbersProperty, value));
    }

    // Compile-order device: Mathcad declares this field after set_ShowNumbers; earlier placement shifts the closure ordinals of the setters above.
    private static readonly global::System.Type _typeOfThis;

    private static void DisplayNumberFormatChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        NumberFormat oldFormat = arguments.OldValue as NumberFormat;
        NumberFormat newFormat = arguments.NewValue as NumberFormat;
        PlotGraphAxis axis = dependencyObject as PlotGraphAxis;
        if (axis != null)
        {
            axis.RegisterHandleTickmarksFormattingChanged(oldFormat, newFormat);
            if (newFormat != null)
            {
                axis.HandleTickmarksFormattingChanged(string.Empty, newFormat);
            }
        }
    }

    public bool IsDisplayNumberFormatLocal
    {
        get
        {
            return global::Quire.Wpf.WpfUtils.HasLocalValue(this, DisplayNumberFormatProperty);
        }
    }

    private void RegisterHandleTickmarksFormattingChanged(global::Quire.Controls.Core.NumberFormat oldFormat, global::Quire.Controls.Core.NumberFormat newFormat)
    {
        if (oldFormat != null)
        {
            oldFormat.FormatChanged -= HandleTickmarksFormattingChanged;
        }
        if (newFormat != null)
        {
            newFormat.FormatChanged += HandleTickmarksFormattingChanged;
        }
    }

    private void HandleTickmarksFormattingChanged(string propertyName, object newValue)
    {
        if (NumberFormatChanged != null)
        {
            NumberFormatChanged(DisplayNumberFormat, IsDisplayNumberFormatLocal);
        }
    }

    public static readonly global::System.Windows.DependencyProperty DisplayNumberFormatProperty;

    public global::Quire.Controls.Core.NumberFormat DisplayNumberFormat { get => (global::Quire.Controls.Core.NumberFormat)GetValue(DisplayNumberFormatProperty); set => SetValue(DisplayNumberFormatProperty, value); }

    private static void HandleOffsetPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs arguments)
    {
        FirePropertyChanged(dependencyObject as PlotGraphAxis, "Offset");
    }

    private static readonly global::System.Windows.DependencyPropertyKey _offsetPropertyKey;

    public static readonly global::System.Windows.DependencyProperty OffsetProperty;

    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => SetValue(_offsetPropertyKey, value);
    }

    private static void OnScalingChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        PlotGraphAxis axis = d as PlotGraphAxis;
        if (axis == null)
        {
            return;
        }
        if (axis.IsLinearizedRangeAssertEnabled.HasValue)
        {
            axis.IsLinearizedRangeAssertEnabled = true;
        }
        if (axis.Scaling == global::Quire.Controls.Plots.Presenter.AxisScaling.Logarithmic && axis.SecondTickExpressionEqn.IsKeyboardFocusWithin)
        {
            axis.HandleTraverse(global::System.Windows.Input.FocusNavigationDirection.Next);
        }
        axis.FirePropertyChanged("Scaling");
    }

    private static object CoerceScaling(global::System.Windows.DependencyObject d, object baseValue)
    {
        if (((PlotGraphAxis)d).IsDiscrete)
        {
            return global::Quire.Controls.Plots.Presenter.AxisScaling.Linear;
        }
        return baseValue;
    }

    public static readonly global::System.Windows.DependencyProperty ScalingProperty;

    public static readonly global::System.Windows.RoutedEvent ContentChangedEvent;

    public global::Quire.Controls.Plots.Presenter.AxisScaling Scaling
    {
        get => (global::Quire.Controls.Plots.Presenter.AxisScaling)GetValue(ScalingProperty);
        set
        {
            bool? isLinearizedRangeAssertEnabled = IsLinearizedRangeAssertEnabled;
            IsLinearizedRangeAssertEnabled = null;
            SetValue(ScalingProperty, value);
            IsLinearizedRangeAssertEnabled = isLinearizedRangeAssertEnabled;
        }
    }

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

    protected void FireChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
        RaiseEvent(new global::System.Windows.RoutedEventArgs(ContentChangedEvent));
    }

    protected override void ShowLimitsExpressions()
    {
        ShowStartExpression();
        if (Scaling != global::Quire.Controls.Plots.Presenter.AxisScaling.Logarithmic)
        {
            ShowSecondTickExpression();
        }
        ShowEndExpression();
    }

    protected override void HideLimitsExpressions()
    {
        HideStartExpression();
        if (Scaling != global::Quire.Controls.Plots.Presenter.AxisScaling.Logarithmic)
        {
            HideSecondTickExpression();
        }
        HideEndExpression();
    }

    private static void ShowTicksPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        ((PlotGraphAxis)dependencyObject).ShowTicksPropertyChanged((bool)dependencyPropertyChangedEventArgs.NewValue);
    }

    private void ShowTicksPropertyChanged(bool newValue)
    {
        if (ViewModel != null)
        {
            ViewModel.ShowTicks = newValue;
        }
        FireShowTicksChanged();
    }

    private void FireShowTicksChanged()
    {
        // Read the event once so the null check and the invocation use the same delegate.
        Action showTicksChanged = ShowTicksChanged;
        if (showTicksChanged != null)
        {
            showTicksChanged();
        }
        FireChanged();
    }

    private static void ShowNumbersPropertyChanged(global::System.Windows.DependencyObject dependencyObject, global::System.Windows.DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        ((PlotGraphAxis)dependencyObject).ShowNumbersPropertyChanged((bool)dependencyPropertyChangedEventArgs.NewValue);
    }

    private void ShowNumbersPropertyChanged(bool newValue)
    {
        if (ViewModel != null)
        {
            ViewModel.ShowNumbers = newValue;
        }
        FireShowNumbersChanged();
    }

    private void FireShowNumbersChanged()
    {
        // Read the event once so the null check and the invocation use the same delegate.
        Action showNumbersChanged = ShowNumbersChanged;
        if (showNumbersChanged != null)
        {
            showNumbersChanged();
        }
        FireChanged();
    }

    protected virtual void SetOtherAxis(global::Quire.Controls.Plots.PlotGraphAxis value)
    {
        _otherAxis = value;
    }

    public abstract global::Quire.Controls.Plots.Presenter.Positions Position { get; set; }

    public abstract void UpdateAxisOriginPositions();

    private double Length
    {
        get
        {
            if (Orientation != global::System.Windows.Controls.Orientation.Horizontal)
            {
                return ActualHeight;
            }
            return ActualWidth;
        }
    }

    protected virtual double GetAxisStartPosition()
    {
        return Length / (TickMarks + 1);
    }

    protected virtual double GetAxisEndPosition()
    {
        double length = Length;
        return length - length / (double)(TickMarks + 1);
    }

    private global::System.Nullable<bool> _isLinearizedRangeAssertEnabled;

    public global::System.Nullable<bool> IsLinearizedRangeAssertEnabled
    {
        get
        {
            return _isLinearizedRangeAssertEnabled;
        }
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool HasAxisError()
    {
        return StartLimitEditor.ErrorCount > 0 || EndLimitEditor.ErrorCount > 0;
    }

    private global::System.Nullable<double> LinearizeStartEnd(double value)
    {
        double num = Linearize(value);
        if (num.IsValid())
        {
            return num;
        }
        if (IsLinearizedRangeAssertEnabled.HasValue && IsLinearizedRangeAssertEnabled.Value && Scaling == global::Quire.Controls.Plots.Presenter.AxisScaling.Logarithmic && !HasAxisError())
        {
            IsLinearizedRangeAssertEnabled = false;
            DebugTools.FailAssertion("Start or End values of axis model should not be negative when scaling is logarithmic.");
        }
        return null;
    }

    private double Linearize(double value)
    {
        if (Scaling != global::Quire.Controls.Plots.Presenter.AxisScaling.Linear)
        {
            // !(value > 0) sends NaN to the -infinity branch, the ordered-compare shape the Release IL requires.
            if (!(value > 0))
            {
                return double.NegativeInfinity;
            }
            return Math.Log10(value);
        }
        return value;
    }

    protected double UnLinearize(double value)
    {
        if (Scaling != global::Quire.Controls.Plots.Presenter.AxisScaling.Linear)
        {
            return Math.Pow(10, value);
        }
        return value;
    }

    private static global::System.Windows.Media.Animation.DoubleAnimation CreateDoubleAnimation(double toValue, global::System.Windows.DependencyProperty property, double seconds)
    {
        DoubleAnimation doubleAnimation = new DoubleAnimation(toValue, new Duration(TimeSpan.FromSeconds(seconds)));
        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(property));
        return doubleAnimation;
    }

    public void SetRange(double start, double end)
    {
        Start = start;
        End = end;
        FireRangeChanged();
        LimitSExpressionChanging = false;
    }

    internal void ClearDisplayNumberFormatWithCommand()
    {
        global::Quire.Controls.Core.NumberFormat oldValue = DisplayNumberFormat.Clone();
        DelegateFunction0<bool> doMethod = () =>
        {
            ClearValue(DisplayNumberFormatProperty);
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            SetValue(DisplayNumberFormatProperty, oldValue);
        };
        CommandBuffer.Execute(new UndoableCommand("Clear format", doMethod, undoMethod));
    }

    public abstract void MoveToValue(double value);

    public abstract void MoveByValue(double value);

    public abstract bool FindClosestTickValueToZero(ref int index);

    protected global::System.Windows.Media.FormattedText GetTickValueTextByTickNumber(int tickNumber, global::System.Windows.Media.Brush stroke)
    {
        global::System.Windows.Media.Typeface typeface = new global::System.Windows.Media.Typeface(global::Quire.Controls.Core.FontConstants.DefaultMathFontFamily, global::System.Windows.FontStyles.Normal, global::System.Windows.FontWeights.Normal, global::System.Windows.FontStretches.Normal);
        string text = string.Empty;
        if (DebugTools.DevAssert(tickNumber < Labels.Length).Passed)
        {
            text = Labels[tickNumber];
        }
        return new global::System.Windows.Media.FormattedText(text, global::System.Globalization.CultureInfo.CurrentUICulture, global::System.Windows.FlowDirection.LeftToRight, typeface, 8, stroke);
    }

    public int PositionToTickNumber(global::Quire.Controls.Plots.Presenter.Positions position)
    {
        switch (position)
        {
            case global::Quire.Controls.Plots.Presenter.Positions.Top:
                DebugTools.DevAssert(Orientation == global::System.Windows.Controls.Orientation.Vertical, null);
                return TickMarks + 1;
            case global::Quire.Controls.Plots.Presenter.Positions.Left:
                DebugTools.DevAssert(Orientation == global::System.Windows.Controls.Orientation.Horizontal, null);
                return -1;
            case global::Quire.Controls.Plots.Presenter.Positions.Bottom:
                DebugTools.DevAssert(Orientation == global::System.Windows.Controls.Orientation.Vertical, null);
                return -1;
            case global::Quire.Controls.Plots.Presenter.Positions.Right:
                DebugTools.DevAssert(Orientation == global::System.Windows.Controls.Orientation.Horizontal, null);
                return TickMarks + 1;
            default:
                throw new Exception("Unknown Position");
        }
    }

    public virtual int PositionValue { get => PositionTickMark; set => ApplyTickNumber(value, b: false); }

    protected abstract void ApplyTickNumber(int value, bool b);

    public void SetStartExpression(string expression)
    {
        new PlotCommands.AxisSetStartExpressionCommand(this, expression).Execute();
    }

    public void SetEndExpression(string expression)
    {
        new PlotCommands.AxisSetEndExpressionCommand(this, expression).Execute();
    }

    public string GetFormattedMin()
    {
        return Labels[0];
    }

    public string GetFormattedMax()
    {
        return Labels[Labels.Length - 1];
    }

    public void SetScaling(global::Quire.Controls.Plots.Presenter.AxisScaling axisScaling)
    {
        new PlotCommands.AxisSetScalingCommand(this, axisScaling).Execute();
    }

    public event global::System.Action<object> ValueToScreenChanged;

    private void FireRangeChanged()
    {
        // Read the event once so the null check and the invocation use the same delegate.
        Action<object> valueToScreenChanged = ValueToScreenChanged;
        if (valueToScreenChanged != null)
        {
            valueToScreenChanged(this);
        }
    }

    double global::Quire.Controls.Plots.Presenter.IValueToScreen.CoerceScreenValue(double screenValue, bool toEdges)
    {
        double[] source = new double[2]
        {
            ((global::Quire.Controls.Plots.Presenter.IValueToScreen)this).ValueToScreen(Start),
            ((global::Quire.Controls.Plots.Presenter.IValueToScreen)this).ValueToScreen(End)
        };
        double num = source.Min();
        double num2 = source.Max();
        double result = num;
        double result2 = num2;
        if (toEdges)
        {
            result = 0.0;
            result2 = Length;
        }
        if (screenValue < num)
        {
            return result;
        }
        if (screenValue > num2)
        {
            return result2;
        }
        return screenValue;
    }

    public global::Quire.Controls.Plots.Presenter.EdgeBelongingStatus GetEdgeBelongingStatusForCoordinate(double d)
    {
        if (Start == d)
        {
            return global::Quire.Controls.Plots.Presenter.EdgeBelongingStatus.Start;
        }
        if (d == End)
        {
            return global::Quire.Controls.Plots.Presenter.EdgeBelongingStatus.End;
        }
        return global::Quire.Controls.Plots.Presenter.EdgeBelongingStatus.None;
    }

    double global::Quire.Controls.Plots.Presenter.IValueToScreen.ValueToScreen(double value)
    {
        return Point2Screen(Linearize(value));
    }

    double global::Quire.Controls.Plots.Presenter.IValueToScreen.ScreenToValue(double value)
    {
        return UnLinearize(Screen2Point(value));
    }

    internal static global::Quire.Controls.Plots.PlotAxis GetPlotAxis(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView)
    {
        PlotAxis result = new PlotAxis
        {
            ShowNumbers = axisView.ShowNumbers,
            ShowTicks = axisView.ShowTicks,
            PositionTickMark = axisView.PositionTickMark,
            Scaling = axisView.Scaling,
            ShowGridLines = axisView.ShowGridLines,
            GridLineColor = axisView.GridLineColor
        };
        if ((axisView as DependencyObject).HasLocalValue(EquationControl.DisplayNumberFormatProperty))
        {
            result.DisplayNumberFormat = axisView.DisplayNumberFormat;
        }
        if (axisView.StartLimitEditor != null && !axisView.StartLimitEditor.IsDefaultValue)
        {
            result.StartSExpression = axisView.StartExpressionEqn.EquationSExpression;
        }
        if (axisView.SecondTickEditor != null && !axisView.SecondTickEditor.IsDefaultValue)
        {
            result.SecondTickSExpression = axisView.SecondTickExpressionEqn.EquationSExpression;
        }
        if (axisView.EndLimitEditor != null && !axisView.EndLimitEditor.IsDefaultValue)
        {
            result.EndSExpression = axisView.EndExpressionEqn.EquationSExpression;
        }
        return result;
    }

    internal static void SetPlotAxis(global::Quire.Controls.Plots.Presenter.IPlotAxisView axisView, global::Quire.Controls.Plots.PlotAxis plotAxis)
    {
        if (!string.IsNullOrEmpty(plotAxis.StartSExpression))
        {
            axisView.StartExpressionEqn.EquationSExpression = plotAxis.StartSExpression;
        }
        if (!string.IsNullOrEmpty(plotAxis.SecondTickSExpression))
        {
            axisView.SecondTickExpressionEqn.EquationSExpression = plotAxis.SecondTickSExpression;
        }
        if (!string.IsNullOrEmpty(plotAxis.EndSExpression))
        {
            axisView.EndExpressionEqn.EquationSExpression = plotAxis.EndSExpression;
        }
        axisView.PositionTickMark = plotAxis.PositionTickMark;
        if (null != plotAxis.DisplayNumberFormat)
        {
            axisView.DisplayNumberFormat = plotAxis.DisplayNumberFormat;
        }
        axisView.Scaling = plotAxis.Scaling;
        axisView.ShowTicks = plotAxis.ShowTicks;
        axisView.ShowNumbers = plotAxis.ShowNumbers;
        axisView.ShowGridLines = plotAxis.ShowGridLines;
        axisView.GridLineColor = plotAxis.GridLineColor;
    }

    internal bool HandleAncestorLostFocus(global::System.Windows.DependencyObject element, global::System.Windows.DependencyObject focusedElement, global::System.Collections.Generic.IEnumerable<global::System.Windows.DependencyObject> axisElements, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> axisMarkers)
    {
        if (WpfUtils.FindParentInGroup(element, axisElements) == null)
        {
            return false;
        }
        if (WpfUtils.FindParentInGroup(focusedElement, axisElements) == null)
        {
            AxisLimitEditor[] array = new AxisLimitEditor[3] { StartLimitEditor, SecondTickEditor, EndLimitEditor };
            foreach (AxisLimitEditor axisLimitEditor in array)
            {
                if (!axisLimitEditor.IsCommitted)
                {
                    axisLimitEditor.Commit();
                }
            }
            if (axisMarkers != null)
            {
                foreach (global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel axisMarker in axisMarkers)
                {
                    if (!axisMarker.RealNumberCalculatable.IsCommitted)
                    {
                        axisMarker.RealNumberCalculatable.Commit();
                    }
                }
            }
        }
        return true;
    }

    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    private static void FirePropertyChanged(global::Quire.Controls.Plots.PlotGraphAxis owner, string propertyName)
    {
        if (owner == null)
        {
            DebugTools.DevAssert(false);
            return;
        }
        owner.FirePropertyChanged(propertyName);
    }

    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }

    private void ClearBinding(global::System.Windows.DependencyProperty dependencyProperty)
    {
        global::System.Windows.Data.BindingOperations.ClearBinding(this, dependencyProperty);
    }

    private void SetBinding(global::System.Windows.DependencyProperty dependencyProperty)
    {
        global::System.Windows.Data.BindingOperations.SetBinding(this, dependencyProperty, new global::System.Windows.Data.Binding
        {
            Source = ViewModel,
            Path = new global::System.Windows.PropertyPath(dependencyProperty.Name),
        });
    }
}
