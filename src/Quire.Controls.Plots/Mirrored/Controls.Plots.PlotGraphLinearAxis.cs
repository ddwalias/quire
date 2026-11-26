#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

internal partial class PlotGraphLinearAxis
{
    private double _savedLength;

    public PlotGraphLinearAxis(global::Quire.Controls.Plots.EquationList equationList, global::Quire.Controls.Plots.PlotTitle axisTitle = default) : base(default(global::Quire.Controls.Plots.EquationList), default(int), default(double)) { }

    static PlotGraphLinearAxis()
    {
        _typeOfThis = typeof(PlotGraphLinearAxis);
        PositionProperty = DependencyProperty.Register("Position", typeof(Positions), _typeOfThis, new FrameworkPropertyMetadata(Positions.Origin, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, PositionPropertyChanged));
    }

    private int ColOrRowIndex1 { get; set; }

    private int ColOrRowIndex2 { get; set; }

    private global::System.Windows.Controls.GridSplitter _splitter;

    public override GridSplitter Splitter
    {
        get
        {
            return _splitter;
        }
        set
        {
            if (_splitter == value)
            {
                return;
            }
            if (_splitter != null)
            {
                _splitter.DragCompleted -= Splitter_DragCompleted;
                _splitter.DragDelta -= Splitter_DragDelta;
            }
            _splitter = value;
            if (value != null)
            {
                _splitter.DragCompleted += Splitter_DragCompleted;
                _splitter.DragDelta += Splitter_DragDelta;
                InitializeColOrRowIndexProperties();
                FixateSizeOfRowsInner();
            }
        }
    }

    private void InitializeColOrRowIndexProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void Splitter_DragCompleted(object sender, global::System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        int tickNumber;
        Grid grid = (Grid)Splitter.Parent;
        if (Orientation == Orientation.Vertical)
        {
            double width = grid.ColumnDefinitions[ColOrRowIndex1].ActualWidth;
            tickNumber = OtherLinearAxis.OffsetToTickNumber(width);
        }
        else
        {
            double height = grid.RowDefinitions[ColOrRowIndex1].ActualHeight;
            tickNumber = OtherLinearAxis.OffsetToTickNumber(height);
        }
        ApplyTickNumber(tickNumber, false);
    }

    internal void MoveRowOrCol(int tickNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private double GetGridSplitPosition(int index)
    {
        Grid grid = (Grid)Splitter.Parent;
        if (Orientation != Orientation.Vertical)
        {
            return grid.RowDefinitions[index].ActualHeight;
        }
        return grid.ColumnDefinitions[index].ActualWidth;
    }

    private static void AdjustSnap(ref double snapOffset, ref double index2SnapOffset, double totalWidthOrHeight)
    {
        if (snapOffset < 0)
        {
            snapOffset = 0;
            index2SnapOffset = totalWidthOrHeight;
            return;
        }
        if (index2SnapOffset < 0)
        {
            snapOffset = totalWidthOrHeight;
            index2SnapOffset = 0;
        }
    }

    private void Splitter_DragDelta(object sender, global::System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        UpdateOffset();
        if (MouseMoveAdorner != null)
        {
            MouseMoveAdorner.InvalidateVisual();
        }
    }

    private void UpdateOffset()
    {
        Grid grid = (Grid)Splitter.Parent;
        if (Orientation == Orientation.Vertical)
        {
            Offset = grid.ColumnDefinitions[ColOrRowIndex1].ActualWidth;
        }
        else
        {
            Offset = grid.RowDefinitions[ColOrRowIndex1].ActualHeight;
        }
    }

    private void PlotGraphAxis_SizeChanged(object sender, global::System.Windows.SizeChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override bool ShouldArrangeConvertedEquationsHorizontally()
    {
        return Orientation == Orientation.Horizontal;
    }

    private bool IsEffectAxis => TickmarksPattern != null;

    private PlotGraphLinearAxis OtherLinearAxis => OtherAxis as PlotGraphLinearAxis;

    protected override void SetOtherAxis(PlotGraphAxis value)
    {
        if (OtherLinearAxis != null)
        {
            OtherLinearAxis.CalcPositionTickMarkNeeded -= CalcPositionTickMark;
            SizeChanged -= PlotGraphAxis_SizeChanged;
        }
        PlotGraphLinearAxis axis = value as PlotGraphLinearAxis;
        if (axis != null)
        {
            axis.CalcPositionTickMarkNeeded += CalcPositionTickMark;
            SizeChanged += PlotGraphAxis_SizeChanged;
        }
        base.SetOtherAxis(value);
    }

    public event Action<Positions> CalcPositionTickMarkNeeded;

    protected override void HandleStartChanged()
    {
        base.HandleStartChanged();
        CalcOriginPositioning();
    }

    protected override void HandleEndChanged()
    {
        base.HandleEndChanged();
        CalcOriginPositioning();
    }

    protected override void PositionTickMarkChanged(int tickNumber)
    {
        if (OtherLinearAxis == null)
        {
            return;
        }
        CalcNewPosition(tickNumber);
        MoveRowOrCol(tickNumber);
    }

    private void CalcNewPosition(int tickNumber)
    {
        Positions newPosition = Positions.TickNumberLock;
        if (HasOriginPositioning && OtherLinearAxis.HasZeroInRange())
        {
            newPosition = Positions.Origin;
        }
        else if (Orientation == Orientation.Horizontal)
        {
            if (tickNumber < 0)
            {
                newPosition = Positions.Bottom;
            }
            else if (tickNumber >= OtherLinearAxis.TickMarks)
            {
                newPosition = Positions.Top;
            }
        }
        else
        {
            if (tickNumber < 0)
            {
                newPosition = Positions.Left;
            }
            else if (tickNumber >= OtherLinearAxis.TickMarks)
            {
                newPosition = Positions.Right;
            }
        }
        Position = newPosition;
        FireChanged();
    }

    private static void PositionPropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        PlotGraphLinearAxis axis = d as PlotGraphLinearAxis;
        if (axis != null)
        {
            axis.CalcPositionTickMark((Positions)e.NewValue);
        }
    }

    public static readonly global::System.Windows.DependencyProperty PositionProperty;

    public override Positions Position
    {
        get => (Positions)GetValue(PositionProperty);
        set => Dispatch(() => SetValue(PositionProperty, value));
    }

    private void CalcPositionTickMark(global::Quire.Controls.Plots.Presenter.Positions newPosition)
    {
        if (HasOriginPositioning)
        {
            int tickNumber = 0;
            OtherLinearAxis.FindClosestTickValueToZero(ref tickNumber);
            PositionTickMark = tickNumber;
        }
        else
        {
            switch (newPosition)
            {
                case Positions.Bottom:
                case Positions.Left:
                    PositionTickMark = -1;
                    break;
                case Positions.Top:
                case Positions.Right:
                    PositionTickMark = OtherLinearAxis.TickMarks;
                    break;
                case Positions.Origin:
                case Positions.TickNumberLock:
                    if (OtherLinearAxis.IsEffectAxis)
                    {
                        PositionTickMark = -1;
                    }
                    break;
            }
        }
        MoveRowOrCol(PositionTickMark);
        FireChanged();
        SetupAxisArrowHeadsDisplay();
    }

    private void SetupAxisArrowHeadsDisplay()
    {
        if (Orientation == Orientation.Vertical)
        {
            bool showArrowHead = Position != Positions.Right;
            bool otherShowArrowHead = OtherLinearAxis == null || OtherLinearAxis.Position != Positions.Top;
            if (OtherLinearAxis != null)
            {
                OtherLinearAxis.ShowArrowHead = showArrowHead;
            }
            ShowArrowHead = otherShowArrowHead;
        }
        else
        {
            bool showArrowHead = OtherLinearAxis == null || OtherLinearAxis.Position != Positions.Right;
            bool otherShowArrowHead = Position != Positions.Top;
            ShowArrowHead = showArrowHead;
            if (OtherLinearAxis != null)
            {
                OtherLinearAxis.ShowArrowHead = otherShowArrowHead;
            }
        }
    }

    private void CalcOriginPositioning()
    {
        if (OtherLinearAxis != null)
        {
            FireCalcPositionTickMarkNeeded(OtherLinearAxis.Position);
        }
    }

    private void FireCalcPositionTickMarkNeeded(global::Quire.Controls.Plots.Presenter.Positions newPosition)
    {
        if (CalcPositionTickMarkNeeded != null)
        {
            CalcPositionTickMarkNeeded(newPosition);
        }
    }

    protected override void HandleDiscreteChanged()
    {
        ScaleEdgeEvenly = IsDiscrete;
        CalcOriginPositioning();
    }

    public override void UpdateAxisOriginPositions()
    {
        CalcOriginPositioning();
    }

    protected override void TickMarksChanged()
    {
        if (OtherLinearAxis != null && OtherLinearAxis.PositionTickMark > TickMarks)
        {
            OtherLinearAxis.Position = OtherLinearAxis.Orientation == Orientation.Vertical ? Positions.Right : Positions.Top;
        }
        CalcOriginPositioning();
    }

    private MouseMoveRTDataAdorner MouseMoveAdorner { get; set; }

    private global::System.Windows.Media.LineGeometry GetTickLineByTickNumber(int totalTickNumber)
    {
        return GetTickLine(GetTickPositionByTickNumber(totalTickNumber), 2);
    }

    private bool HasZeroInRange()
    {
        int index = 0;
        return FindClosestTickValueToZero(ref index);
    }

    private bool HasZeroInRange(ref int index) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override bool FindClosestTickValueToZero(ref int index)
    {
        if (IsDiscrete)
        {
            index = -1;
            return false;
        }
        if (TickMarksValues != null)
        {
            if (HasZeroInRange(ref index))
            {
                return true;
            }
            index = TickMarksValues[0] > 0 ? -1 : TickMarks;
            return false;
        }
        index = (int)Math.Floor(TickMarks / 2.0);
        return true;
    }

    public override int PositionValue
    {
        get
        {
            int positionValue = base.PositionValue;
            DebugTools.DevAssert(positionValue == OtherLinearAxis.OffsetToTickNumber(GetGridSplitPosition(ColOrRowIndex1)), null);
            return positionValue;
        }
        set
        {
            base.PositionValue = value;
        }
    }

    protected override void MouseMoveDraw(global::System.Windows.Media.DrawingContext drawingContext)
    {
        DrawTickLineAndValue(OffsetToTickNumber(OtherAxis.Offset), drawingContext, Brushes.Black, Brushes.Black, 0.5);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        if (OtherAxis != null)
        {
            e.Handled = true;
            AddAdorner();
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        RemoveAdorner();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (MouseMoveAdorner != null)
        {
            MouseMoveAdorner.InvalidateVisual();
        }
    }

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
    {
        if (OtherAxis != null)
        {
            Size size = Size.Empty;
            if (PositionTickMark == 0)
            {
                size = OtherAxis._firstLabel.DesiredSize;
            }
            else if (PositionTickMark == 1)
            {
                size = OtherAxis._secondLabel.DesiredSize;
            }
            else if (PositionTickMark == OtherAxis.TickMarks)
            {
                size = OtherAxis._lastLabel.DesiredSize;
            }
            if (size != Size.Empty)
            {
                Rect rect = new Rect(size);
                int tickMarkPosition = OtherAxis.PositionTickMark;
                double tickPosition = GetTickPosition(GetMajorTickmaks(), tickMarkPosition);
                if (Orientation == Orientation.Vertical)
                {
                    rect.X = -size.Width / 2.0;
                    rect.Y = tickPosition + 3.0;
                }
                else
                {
                    rect.X = tickPosition - 3.0 - size.Width;
                    rect.Y = -size.Height / 2.0;
                }
                if (rect.Contains(hitTestParameters.HitPoint))
                {
                    GeneralTransform transform = TransformToVisual(OtherAxis);
                    return VisualTreeHelper.HitTest(OtherAxis, transform.Transform(hitTestParameters.HitPoint));
                }
            }
        }
        return base.HitTestCore(hitTestParameters);
    }

    public void DrawTickLineAndValue(int tickNumber, global::System.Windows.Media.DrawingContext drawingContext, global::System.Windows.Media.Brush fill, global::System.Windows.Media.Brush stroke, double strokeThickness)
    {
        tickNumber = Math.Min(TickMarks, Math.Max(-1, tickNumber));
        drawingContext.DrawGeometry(null, new Pen(stroke, strokeThickness), GetTickLineByTickNumber(tickNumber));
        if (tickNumber >= 0 && tickNumber < TickMarks)
        {
            Point position;
            FormattedText text = GetTickValueTextAndPositionByTickNumber(tickNumber, stroke, out position);
            drawingContext.DrawText(text, position);
        }
    }

    public override void MoveToValue(double value)
    {
        double screen = OtherAxis.Point2Screen(value);
        ApplyTickNumber(OtherLinearAxis.OffsetToTickNumber(screen), false);
    }

    public override void MoveByValue(double value)
    {
        double point = OtherAxis.Screen2Point(Offset);
        double screen = OtherAxis.Point2Screen(point + value);
        ApplyTickNumber(OtherLinearAxis.OffsetToTickNumber(screen), false);
    }

    protected override void ApplyTickNumber(int tickNumber, bool hasOriginPositioning)
    {
        if (tickNumber != PositionTickMark)
        {
            new PlotCommands.AxisSetTickNumberCommand(this, tickNumber, hasOriginPositioning).Execute();
        }
        else
        {
            MoveRowOrCol(tickNumber);
        }
    }

    private int OffsetToTickNumber(double offset) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void AddAdorner()
    {
        RemoveAdorner();
        MouseMoveAdorner = new MouseMoveRTDataAdorner(OtherAxis);
        AdornerLayer.GetAdornerLayer(OtherAxis).Add(MouseMoveAdorner);
    }

    private void RemoveAdorner()
    {
        if (MouseMoveAdorner == null)
        {
            return;
        }
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(OtherAxis);
        if (adornerLayer != null)
        {
            adornerLayer.Remove(MouseMoveAdorner);
        }
        MouseMoveAdorner = null;
        if (OtherAxis != null)
        {
            OtherAxis.IsHitTestVisible = true;
        }
    }

    internal void FixateSize(double savedLength)
    {
        _savedLength = savedLength;
        if (Splitter != null)
        {
            FixateSizeOfRowsInner();
        }
    }

    internal bool SizeFrozen { get; private set; }

    private void FixateSizeOfRowsInner() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override double GetPerpendicularAxisPositionRatio()
    {
        if (OtherAxis == null)
        {
            return base.GetPerpendicularAxisPositionRatio();
        }
        return GetTickRelativePosition(OtherAxis.PositionTickMark);
    }

    internal bool ReleaseAutoReSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static readonly global::System.Type _typeOfThis;

    public PlotGraphLinearAxis()
        : this(null)
    {
    }
}
