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
            double num2 = scale.TickMarkPosition(tickNumber) - majorTickmaks.TickMarkPosition(0);
            offset = 1.0 / 12.0 + num2 / num * (5.0 / 6.0);
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

}
