#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

internal sealed partial class PlotGraphRadialAxis
{
    private double _radius;

    private Point GetTickMarkElementPosition(int index, UIElement element)
    {
        return GetTickMarkPositionByIndex(index, element.DesiredSize.Width, 3.0);
    }

    public Point GetTickMarkPositionByIndex(int index, double tickWidth, double tickHeight)
    {
        double range = (double)index / (double)(TickMarks - 1);
        return GetTickMarkPositionByRange(range, tickWidth, tickHeight);
    }

    private Geometry GetTickLine(double range, int size)
    {
        Point tickMarkPositionByRange = GetTickMarkPositionByRange(range, 0.0, 0.0);
        Point startPoint = new Point(tickMarkPositionByRange.X, tickMarkPositionByRange.Y - (double)size);
        Point endPoint = new Point(tickMarkPositionByRange.X, tickMarkPositionByRange.Y + (double)size);
        return new LineGeometry(startPoint, endPoint);
    }

    private Geometry AxisLineGeometry()
    {
        return new LineGeometry(new Point(Radius, Radius), new Point(2.0 * RadiusWTickOffset + 1.1 * TickOffset, Radius));
    }

    private void DrawNumbers(DrawingContext drawingContext, Brush stroke)
    {
        int firstTick = 2;
        if (Scaling == AxisScaling.Logarithmic)
        {
            firstTick = 1;
        }
        int lastTick = TickMarks - 1;
        int labelCount = Labels.Length;
        if (DebugTools.DevAssert(lastTick <= labelCount, "Tick index limit is {0} > labels array length {1}. It is going to cause an out of range exception.", lastTick, labelCount).Passed)
        {
            for (int i = firstTick; i < lastTick; i++)
            {
                FormattedText tickValueTextByTickNumber = GetTickValueTextByTickNumber(i, stroke);
                Point tickMarkPositionByIndex = GetTickMarkPositionByIndex(i, tickValueTextByTickNumber.Width, 3.0);
                drawingContext.DrawText(tickValueTextByTickNumber, tickMarkPositionByIndex);
            }
        }
    }

    protected override void HandleOrientationChanged()
    {
    }

    protected override void PositionTickMarkChanged(int number)
    {
    }

    public override void UpdateAxisOriginPositions()
    {
    }

    protected override void MouseMoveDraw(DrawingContext context)
    {
    }

    public override void MoveToValue(double value)
    {
    }

    public override void MoveByValue(double value)
    {
    }

    public override bool FindClosestTickValueToZero(ref int index)
    {
        return true;
    }

    protected override void ApplyTickNumber(int value, bool b)
    {
    }

    private double Radius => _radius;

    private double TickOffset => Radius * 0.2;

    private double RadiusWTickOffset => Radius - TickOffset;

    private Geometry ArrowHeadGeometry
    {
        get
        {
            Point startPoint = new Point(2.0 * RadiusWTickOffset + 1.1 * TickOffset, Radius);
            PathFigure[] array = new PathFigure[1]
            {
                new PathFigure
                {
                    StartPoint = startPoint,
                    IsClosed = true,
                    IsFilled = true
                }
            };
            array[0].Segments.Add(new LineSegment(new Point(startPoint.X, startPoint.Y + 2.5), isStroked: true));
            array[0].Segments.Add(new LineSegment(new Point(startPoint.X + 5.0, startPoint.Y), isStroked: true));
            array[0].Segments.Add(new LineSegment(new Point(startPoint.X, startPoint.Y - 2.5), isStroked: true));
            return new PathGeometry(array);
        }
    }

    public override Positions Position
    {
        get
        {
            return Positions.Origin;
        }
        set
        {
        }
    }

    public override Point2ScreenHelper Helper => new Point2ScreenHelperImpl(this);

    static PlotGraphRadialAxis()
    {
        StartProperty.OverrideMetadata(typeof(PlotGraphRadialAxis), new FrameworkPropertyMetadata(0.0));
        EndProperty.OverrideMetadata(typeof(PlotGraphRadialAxis), new FrameworkPropertyMetadata(1.0));
    }

    internal PlotGraphRadialAxis(EquationList equationList)
        : base(equationList, AxisModelDefinitionCollection.Instance[AxesType.Radial, PlotType.Polar].TickMarksDefault, AxisModelDefinitionCollection.Instance[AxesType.Radial, PlotType.Polar].DefaultTickMagnitudeThreashold)
    {
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Point tickMarkElementPosition = GetTickMarkElementPosition(0, _firstLabel);
        Point tickMarkElementPosition2 = GetTickMarkElementPosition(TickMarks - 1, _lastLabel);
        Point tickMarkElementPosition3 = GetTickMarkElementPosition(1, _secondLabel);
        Point tickMarkElementPosition4 = GetTickMarkElementPosition(0, StartExpressionEqn);
        Point tickMarkElementPosition5 = GetTickMarkElementPosition(TickMarks - 1, EndExpressionEqn);
        Point tickMarkElementPosition6 = GetTickMarkElementPosition(1, SecondTickExpressionEqn);
        StartExpressionEqn.Arrange(StartEquationRectangle(tickMarkElementPosition4));
        EndExpressionEqn.Arrange(EndEquationRectangle(tickMarkElementPosition5));
        ArrangeSecondTickExpressionEqn(tickMarkElementPosition6);
        _firstLabel.Arrange(FirstLabelRectangle(tickMarkElementPosition));
        _lastLabel.Arrange(LastLabelRectangle(tickMarkElementPosition2));
        _secondLabel.Arrange(SecondLabelRectangle(tickMarkElementPosition3));
        Size result = base.ArrangeOverride(finalSize);
        DebugTools.DevAssert(IsMeasureValid, null);
        return result;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        double num = Math.Min(constraint.Height, constraint.Width);
        Size result = base.MeasureOverride(new Size(num, num));
        _radius = Math.Min(result.Height, result.Width) / 2.0;
        return result;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Draw(drawingContext, LineBrush, LineBrush, 0.5);
    }

    public void Draw(DrawingContext drawingContext, Brush fill, Brush stroke, double strokeThickness)
    {
        drawingContext.DrawGeometry(fill, null, ArrowHeadGeometry);
        drawingContext.DrawGeometry(null, new Pen(Brushes.Transparent, 10.0), AxisLineGeometry());
        drawingContext.DrawGeometry(null, new Pen(stroke, strokeThickness), AxisLineGeometry());
        if (ShowTicks)
        {
            drawingContext.DrawGeometry(null, new Pen(stroke, strokeThickness), GetTickmarksGeometry(GetMajorTickmaks()));
            if (MinorTickmarks != null && MinorTickmarks.Show)
            {
                drawingContext.DrawGeometry(null, new Pen(stroke, strokeThickness), GetTickmarksGeometry(MinorTickmarks));
            }
        }
        if (ShowNumbers && Labels != null && DebugTools.DevAssert(TickMarks == Labels.Length, null).Passed)
        {
            DrawNumbers(drawingContext, stroke);
        }
    }

    private Geometry GetTickmarksGeometry(TickScale scale)
    {
        GeometryGroup geometryGroup = new GeometryGroup();
        TickScale majorTickmaks = GetMajorTickmaks();
        for (int i = 0; i < scale.Count; i++)
        {
            if (scale.ShouldDrawTick(i))
            {
                double range = scale.TickMarkPosition(i) / (majorTickmaks.TickMarkPosition(TickMarks) - 1.0);
                geometryGroup.Children.Add(GetTickLine(range, 2));
            }
        }
        return geometryGroup;
    }

    private Point GetTickMarkPositionByRange(double range, double tickWidth, double tickHeight)
    {
        return new Point(Radius + range * RadiusWTickOffset - tickWidth / 2.0, Radius + tickHeight);
    }

    public override Pair<double, double> ClipBounds
    {
        get
        {
            Pair<double, double> clipBounds = base.ClipBounds;
            double range = clipBounds.Second - clipBounds.First;
            return new Pair<double, double>(-range, range);
        }
    }

    protected override double GetAxisStartPosition()
    {
        return 2.0 * Radius - GetTickMarkPositionByIndex(0, 0.0, 0.0).X;
    }

    protected override double GetAxisEndPosition()
    {
        return TickOffset;
    }

    protected override void TickMarksChanged()
    {
    }

    public partial class Point2ScreenHelperImpl
    {
        public Point2ScreenHelperImpl(PlotGraphRadialAxis axisView)
            : base(axisView)
        {
        }

        public override double Point2Screen(double offset)
        {
            return offset / _axisLenght * (_lastTickPositoin - _firstTickPostion) + _firstTickPostion;
        }

        internal override bool Contains(double coordinate)
        {
            return true;
        }
    }
}
