#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

internal sealed partial class PlotGraphAngularAxis
{
    public const double RadiusRatio = 0.8;

    private double Radius
    {
        get
        {
            Size desiredSize = DesiredSize;
            double height = desiredSize.Height;
            desiredSize = DesiredSize;
            return Math.Min(height, desiredSize.Width) / 2.0;
        }
    }

    private double TickOffset => Radius - RadiusWTickOffset;

    private double RadiusWTickOffset => Radius * 0.8;

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

    public override bool IsDiscrete
    {
        get
        {
            return false;
        }
        set
        {
            DebugTools.DevAssert(!value, null);
        }
    }

    public override Point2ScreenHelper Helper => new Point2ScreenHelperImpl(this);

    static PlotGraphAngularAxis()
    {
        StartProperty.OverrideMetadata(typeof(PlotGraphAngularAxis), new FrameworkPropertyMetadata(0.0));
        EndProperty.OverrideMetadata(typeof(PlotGraphAngularAxis), new FrameworkPropertyMetadata(360.0));
    }

    internal PlotGraphAngularAxis(EquationList equationList)
        : base(equationList, AxisModelDefinitionCollection.Instance[AxesType.Angular, PlotType.Polar].TickMarksDefault, AxisModelDefinitionCollection.Instance[AxesType.Angular, PlotType.Polar].DefaultTickMagnitudeThreashold)
    {
        StartExpressionEqn.Visibility = (_firstLabel.Visibility = Visibility.Collapsed);
        EndExpressionEqn.Visibility = (_lastLabel.Visibility = Visibility.Collapsed);
    }

    private Point GetTickMarkElementPosition(int index, UIElement element)
    {
        return GetTickMarkPositionByIndex(index, element.DesiredSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Point tickMarkElementPosition = GetTickMarkElementPosition(1, _secondLabel);
        Point tickMarkElementPosition2 = GetTickMarkElementPosition(1, SecondTickExpressionEqn);
        DebugTools.DevAssert(Scaling != AxisScaling.Logarithmic, null);
        ArrangeSecondTickExpressionEqn(tickMarkElementPosition2);
        if (DebugTools.DevAssert(TickMarks > 1, null).Passed)
        {
            _secondLabel.Arrange(SecondLabelRectangle(tickMarkElementPosition));
        }
        DebugTools.DevAssert(SecondTickExpressionEqn.Visibility == Visibility.Visible && StartExpressionEqn.Visibility == Visibility.Collapsed && _firstLabel.Visibility == StartExpressionEqn.Visibility && EndExpressionEqn.Visibility == Visibility.Collapsed && _lastLabel.Visibility == EndExpressionEqn.Visibility, null);
        Size result = base.ArrangeOverride(finalSize);
        DebugTools.DevAssert(IsMeasureValid, null);
        return result;
    }

    protected override void SetTicksExpressionsEqnVisibility(Visibility visibility)
    {
    }

    protected override Size MeasureOverride(Size constraint)
    {
        double num = Math.Min(constraint.Height, constraint.Width);
        return base.MeasureOverride(new Size(num, num));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Draw(drawingContext, LineBrush, LineBrush, 0.5);
    }

    protected override void SecondTickExpressionEqn_NavigateOffOfEquation(Direction direction)
    {
    }

    public void Draw(DrawingContext drawingContext, Brush fill, Brush stroke, double strokeThickness)
    {
        drawingContext.DrawGeometry(null, new Pen(Brushes.Transparent, 10.0), AxisCircleGeometry());
        drawingContext.DrawGeometry(null, new Pen(stroke, strokeThickness), AxisCircleGeometry());
        if (ShowTicks)
        {
            drawingContext.DrawGeometry(null, new Pen(stroke, strokeThickness), GetTickmarksGeometry(GetMajorTickmaks()));
        }
        if (ShowNumbers && Labels != null)
        {
            DrawNumbers(drawingContext, stroke);
        }
    }

    private Geometry GetTickmarksGeometry(TickScale scale)
    {
        GeometryGroup geometryGroup = new GeometryGroup();
        for (int i = 0; i < TickMarks - 1; i++)
        {
            double angle = TickMarksValues[i] / 180.0 * Math.PI;
            if (scale.ShouldDrawTick(i))
            {
                geometryGroup.Children.Add(GetTickLine(angle, 2));
            }
        }
        return geometryGroup;
    }

    private Geometry GetTickLine(double angle, int size)
    {
        Point startPoint = new Point(Radius + (RadiusWTickOffset - (double)size) * Math.Cos(angle), Radius - (RadiusWTickOffset - (double)size) * Math.Sin(angle));
        Point endPoint = new Point(Radius + (RadiusWTickOffset + (double)size) * Math.Cos(angle), Radius - (RadiusWTickOffset + (double)size) * Math.Sin(angle));
        return new LineGeometry(startPoint, endPoint);
    }

    private Geometry AxisCircleGeometry()
    {
        return new EllipseGeometry(new Rect(new Point(TickOffset, TickOffset), new Size(2.0 * RadiusWTickOffset, 2.0 * RadiusWTickOffset)));
    }

    private Point GetTickMarkPositionByAngle(double angle, Size tickSize)
    {
        return new Point(Radius - tickSize.Width / 2.0 + (RadiusWTickOffset + 3.0 + tickSize.Width) * Math.Cos(angle), Radius - tickSize.Height / 2.0 - (RadiusWTickOffset + 3.0 + tickSize.Height) * Math.Sin(angle));
    }

    private Point GetTickMarkPositionByIndex(int index, Size tickSize)
    {
        double angle = TickMarksValues[index] / 180.0 * Math.PI;
        return GetTickMarkPositionByAngle(angle, tickSize);
    }

    private void DrawNumbers(DrawingContext drawingContext, Brush stroke)
    {
        for (int i = 0; i < TickMarks - 1; i++)
        {
            if (i != 1)
            {
                FormattedText tickValueTextByTickNumber = GetTickValueTextByTickNumber(i, stroke);
                Point tickMarkPositionByIndex = GetTickMarkPositionByIndex(i, new Size(tickValueTextByTickNumber.Width, tickValueTextByTickNumber.Height));
                if (i == 0)
                {
                    tickMarkPositionByIndex.X += 0.2 * TickOffset;
                }
                drawingContext.DrawText(tickValueTextByTickNumber, tickMarkPositionByIndex);
            }
        }
    }

    protected override bool ShouldArrangeConvertedEquationsHorizontally()
    {
        return true;
    }
}
