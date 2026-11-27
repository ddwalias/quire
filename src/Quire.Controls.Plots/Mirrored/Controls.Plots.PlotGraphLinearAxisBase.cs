#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public abstract partial class PlotGraphLinearAxisBase
{
    private static readonly Type _typeOfThis = typeof(PlotGraphLinearAxisBase);
    public static readonly DependencyProperty ShowArrowHeadProperty = DependencyProperty.Register("ShowArrowHead", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
    protected const double EdgeTickMarkScale = 1.0 / 12.0;
    protected internal const double MainTicksRangeScale = 5.0 / 6.0;
    public static DependencyProperty ScaleEdgeEvenlyProperty = DependencyProperty.Register("ScaleEdgeEvenly", typeof(bool), typeof(PlotGraphAxisBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    protected bool ShowArrowHead
    {
        get
        {
            return (bool)GetValue(ShowArrowHeadProperty);
        }
        set
        {
            SetValue(ShowArrowHeadProperty, value);
        }
    }

    public bool ScaleEdgeEvenly
    {
        get
        {
            return (bool)GetValue(ScaleEdgeEvenlyProperty);
        }
        set
        {
            SetValue(ScaleEdgeEvenlyProperty, value);
        }
    }

    protected PlotGraphLinearAxisBase(EquationList equationList, int defaultTickMarks, double defaultTickMagnitudeThreshold)
        : base(equationList, defaultTickMarks, defaultTickMagnitudeThreshold)
    {
    }

    protected override double GetAxisStartPosition()
    {
        return GetTickPositionByTickNumber(0);
    }

    protected override double GetAxisEndPosition()
    {
        return GetTickPositionByTickNumber(TickMarks - 1);
    }

    internal double? GetMinorTickPositionByTickNumber(int minorTickNumber)
    {
        if (MinorTickmarks == null || !MinorTickmarks.Show || Scaling != AxisScaling.Logarithmic)
        {
            return null;
        }
        return GetTickPosition(MinorTickmarks, minorTickNumber);
    }

    internal TickScale GetMinorTickmarks()
    {
        if (MinorTickmarks != null && MinorTickmarks.Show && Scaling == AxisScaling.Logarithmic)
        {
            return MinorTickmarks;
        }
        return null;
    }

    protected override void HandleOrientationChanged()
    {
        if (Orientation == Orientation.Horizontal)
        {
            _startEquation.HorizontalAlignment = HorizontalAlignment.Center;
            _startEquation.VerticalAlignment = VerticalAlignment.Top;
            _secondTickEquation.HorizontalAlignment = HorizontalAlignment.Center;
            _secondTickEquation.VerticalAlignment = VerticalAlignment.Top;
            _endEquation.HorizontalAlignment = HorizontalAlignment.Center;
            _endEquation.VerticalAlignment = VerticalAlignment.Top;
        }
        else
        {
            _startEquation.HorizontalAlignment = HorizontalAlignment.Right;
            _startEquation.VerticalAlignment = VerticalAlignment.Center;
            _secondTickEquation.HorizontalAlignment = HorizontalAlignment.Right;
            _secondTickEquation.VerticalAlignment = VerticalAlignment.Center;
            _endEquation.HorizontalAlignment = HorizontalAlignment.Right;
            _endEquation.VerticalAlignment = VerticalAlignment.Center;
        }
    }

    private Geometry GetTickmarksGeometry(TickScale scale, bool addFirstTick)
    {
        GeometryGroup geometryGroup = new GeometryGroup();
        if (addFirstTick)
        {
            geometryGroup.Children.Add(GetTickLine(GetRealOffset(0.0), 2.0));
        }
        for (int i = 0; i < scale.Count; i++)
        {
            int tickNumber = i;
            if (scale.ShouldDrawTick(tickNumber))
            {
                geometryGroup.Children.Add(GetTickLine(GetTickPosition(scale, tickNumber), 2.0));
            }
        }
        return geometryGroup;
    }

    protected double GetTickPosition(TickScale scale, int tickNumber)
    {
        return GetTickRelativePosition(scale, tickNumber) * Length(fullSize: true);
    }

    private double GetTickRelativePosition(TickScale scale, int tickNumber)
    {
        if (TickMarks == 1)
        {
            return (double)(tickNumber + 1) / 2.0;
        }
        TickScale majorTickmaks = GetMajorTickmaks();
        double num = majorTickmaks.TickMarkPosition(TickMarks - 1) - majorTickmaks.TickMarkPosition(0);
        double offset;
        if (ScaleEdgeEvenly)
        {
            offset = (scale.TickMarkPosition(tickNumber) + 1.0) / (majorTickmaks.TickMarkPosition(TickMarks) + 1.0);
        }
        else
        {
            double tickPositionOffset = scale.TickMarkPosition(tickNumber) - majorTickmaks.TickMarkPosition(0);
            offset = 1.0 / 12.0 + tickPositionOffset / num * (5.0 / 6.0);
        }
        return GetRealRelaviveOffset(offset);
    }

    private Geometry AxisLineGeometry(bool fullSize)
    {
        Point val;
        Point endPoint;
        if (Orientation == Orientation.Horizontal)
        {
            val = new Point(0.0, 0.0);
            endPoint = val + new Vector(Length(fullSize), 0.0);
        }
        else
        {
            val = new Point(0.0, Length(fullSize: true));
            endPoint = val - new Vector(0.0, Length(fullSize));
        }
        return new LineGeometry(val, endPoint);
    }

    internal double GetTickPositionByTickNumber(int tickNumber)
    {
        return GetTickRelativePosition(tickNumber) * Length(fullSize: true);
    }

    protected double GetTickRelativePosition(int tickNumber)
    {
        if (tickNumber < 0)
        {
            return GetRealRelaviveOffset(0.0);
        }
        if (tickNumber >= TickMarks)
        {
            return GetRealRelaviveOffset(1.0);
        }
        return GetTickRelativePosition(GetMajorTickmaks(), tickNumber);
    }

    public void Draw(DrawingContext drawingContext, Brush fill, Brush numbersStroke, Brush lineStroke, double strokeThickness)
    {
        if (ShowArrowHead)
        {
            drawingContext.DrawGeometry(fill, null, ArrowHeadGeometry);
        }
        drawingContext.DrawGeometry(null, new Pen(Brushes.Transparent, 10.0), AxisLineGeometry(fullSize: true));
        drawingContext.DrawGeometry(null, new Pen(lineStroke, strokeThickness), AxisLineGeometry(!ShowArrowHead));
        if (ShowTicks)
        {
            drawingContext.DrawGeometry(null, new Pen(lineStroke, strokeThickness), GetTickmarksGeometry(GetMajorTickmaks(), addFirstTick: true));
            if (MinorTickmarks != null && MinorTickmarks.Show)
            {
                drawingContext.DrawGeometry(null, new Pen(lineStroke, strokeThickness), GetTickmarksGeometry(MinorTickmarks, addFirstTick: false));
            }
        }
        if (ShowNumbers && Labels != null)
        {
            DrawNumbers(drawingContext, numbersStroke);
        }
    }

    protected LineGeometry GetTickLine(double tickPosition, double size)
    {
        Point startPoint;
        Point endPoint;
        if (Orientation == Orientation.Horizontal)
        {
            startPoint = new Point(tickPosition, -size);
            endPoint = new Point(tickPosition, size);
        }
        else
        {
            startPoint = new Point(-size, tickPosition);
            endPoint = new Point(size, tickPosition);
        }
        return new LineGeometry(startPoint, endPoint);
    }

    protected double Length(bool fullSize)
    {
        double length;
        Size desiredSize;
        if (Orientation == Orientation.Horizontal)
        {
            desiredSize = DesiredSize;
            length = desiredSize.Width;
        }
        else
        {
            desiredSize = DesiredSize;
            length = desiredSize.Height;
        }
        if (!fullSize)
        {
            length -= 5.0;
        }
        return length;
    }

    protected double GetRealOffset(double offset)
    {
        if (Orientation != Orientation.Horizontal)
        {
            return Length(fullSize: true) - offset;
        }
        return offset;
    }

    private double GetRealRelaviveOffset(double offset)
    {
        if (Orientation != Orientation.Horizontal)
        {
            return 1.0 - offset;
        }
        return offset;
    }

    protected FormattedText GetTickValueTextAndPositionByTickNumber(int tickNumber, Brush stroke, out Point start)
    {
        double tickPositionByTickNumber = GetTickPositionByTickNumber(tickNumber);
        Point tickPosition = (Orientation == Orientation.Horizontal) ? new Point(tickPositionByTickNumber, 0.0) : new Point(0.0, tickPositionByTickNumber);
        FormattedText tickValueTextByTickNumber = GetTickValueTextByTickNumber(tickNumber, stroke);
        if (Orientation == Orientation.Horizontal)
        {
            start = tickPosition + new Vector((-tickValueTextByTickNumber.Width) / 2.0, 3.0);
        }
        else
        {
            start = tickPosition - new Vector(tickValueTextByTickNumber.Width + 3.0, tickValueTextByTickNumber.Height / 2.0);
        }
        return tickValueTextByTickNumber;
    }

    private void DrawNumbers(DrawingContext drawingContext, Brush stroke)
    {
        int firstTick = 2;
        int lastTick = TickMarks - 1;
        if (IsDiscrete)
        {
            firstTick = 0;
            lastTick = TickMarks;
        }
        else if (Scaling == AxisScaling.Logarithmic)
        {
            firstTick = 1;
        }
        for (int i = firstTick; i < lastTick; i++)
        {
            Point start;
            FormattedText tickValueText = GetTickValueTextAndPositionByTickNumber(i, stroke, out start);
            drawingContext.DrawText(tickValueText, start);
        }
    }

    private Geometry ArrowHeadGeometry
    {
        get
        {
            Point startPoint;
            if (Orientation == Orientation.Horizontal)
            {
                startPoint = new Point(Length(fullSize: false), 0.0);
            }
            else
            {
                startPoint = new Point(0.0, 5.0);
            }
            PathFigure[] array = new PathFigure[1];
            array[0] = new PathFigure
            {
                StartPoint = startPoint,
                IsClosed = true,
                IsFilled = true
            };
            if (Orientation == Orientation.Horizontal)
            {
                array[0].Segments.Add(new LineSegment(new Point(startPoint.X, startPoint.Y + 2.5), isStroked: true));
                array[0].Segments.Add(new LineSegment(new Point(startPoint.X + 5.0, startPoint.Y), isStroked: true));
                array[0].Segments.Add(new LineSegment(new Point(startPoint.X, startPoint.Y - 2.5), isStroked: true));
            }
            else
            {
                array[0].Segments.Add(new LineSegment(new Point(startPoint.X + 2.5, startPoint.Y), isStroked: true));
                array[0].Segments.Add(new LineSegment(new Point(startPoint.X, startPoint.Y - 5.0), isStroked: true));
                array[0].Segments.Add(new LineSegment(new Point(startPoint.X - 2.5, startPoint.Y), isStroked: true));
            }
            return new PathGeometry(array);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        GetPositions(out var firstEqnPosition, out var secondEqnPosition, out var lastEqnPosition, out var firstLabelPosition, out var secondLabelPosition, out var lastLabelPosition);
        StartExpressionEqn.Arrange(StartEquationRectangle(firstEqnPosition));
        EndExpressionEqn.Arrange(EndEquationRectangle(lastEqnPosition));
        ArrangeSecondTickExpressionEqn(secondEqnPosition);
        _firstLabel.Arrange(FirstLabelRectangle(firstLabelPosition));
        _lastLabel.Arrange(LastLabelRectangle(lastLabelPosition));
        _secondLabel.Arrange(SecondLabelRectangle(secondLabelPosition));
        Size result = base.ArrangeOverride(finalSize);
        DebugTools.DevAssert(IsMeasureValid, null);
        return result;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        Size result = base.MeasureOverride(availableSize);
        if (double.IsInfinity(result.Width) || double.IsInfinity(result.Height))
        {
            // Mathcad keeps the six position locals in slots before the bounds rectangle and builds the rectangle first.
            Point firstEqnPosition;
            Point secondEqnPosition;
            Point lastEqnPosition;
            Point firstLabelPosition;
            Point secondLabelPosition;
            Point lastLabelPosition;
            Rect labelsBounds = new Rect(0.0, 0.0, 0.0, 0.0);
            GetPositions(out firstEqnPosition, out secondEqnPosition, out lastEqnPosition, out firstLabelPosition, out secondLabelPosition, out lastLabelPosition);
            labelsBounds.Union(StartEquationRectangle(firstEqnPosition));
            labelsBounds.Union(EndEquationRectangle(lastEqnPosition));
            labelsBounds.Union(SecondExpressionEquation(secondEqnPosition));
            labelsBounds.Union(FirstLabelRectangle(firstLabelPosition));
            labelsBounds.Union(LastLabelRectangle(lastLabelPosition));
            labelsBounds.Union(SecondLabelRectangle(secondLabelPosition));
            if (double.IsInfinity(result.Width))
            {
                result.Width = labelsBounds.Width;
            }
            if (double.IsInfinity(result.Height))
            {
                result.Height = labelsBounds.Height;
            }
        }
        return result;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Draw(drawingContext, LineBrush, PlotGraphAxisBase.NumbersBrush, LineBrush, 0.5);
    }
}
