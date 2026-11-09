#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.Model;

internal partial class TraceModel
{
    private static void CalculateRadialRange(IEnumerable<Point> points, out double min, out double max)
    {
        min = double.PositiveInfinity;
        max = double.NegativeInfinity;
        foreach (Point point in points)
        {
            if (!double.IsNaN(point.Y))
            {
                min = Math.Min(min, point.Y);
                max = Math.Max(max, point.Y);
            }
        }
    }

    private PointCollection CalculateRadialTransformByOffset(PointCollection points)
    {
        if (points == null)
        {
            return null;
        }
        double radialPointsStart = _axes[1].GetRadialPointsStart();
        PointCollection pointCollection = new PointCollection();
        foreach (Point point in points)
        {
            if (!point.Y.IsValid() || (_axes[1].StartExpression.IsValueValid && point.Y < radialPointsStart))
            {
                pointCollection.Add(PointExtension.NaN);
            }
            else
            {
                pointCollection.Add(new Point(point.X, point.Y - radialPointsStart));
            }
        }
        return pointCollection;
    }

    private PointCollection CalculatePolarTransformPoints(PointCollection points)
    {
        if (points == null || _axes[1] == null)
        {
            return null;
        }
        PointCollection pointCollection = new PointCollection();
        foreach (Point point in points)
        {
            if (point.X.IsValid() && point.Y.IsValid())
            {
                double x = point.Y * Math.Cos(point.X);
                double y = point.Y * Math.Sin(point.X);
                pointCollection.Add(new Point(x, y));
            }
            else
            {
                pointCollection.Add(PointExtension.NaN);
            }
        }
        return pointCollection;
    }

    private PointCollection CalculateLogTransformPoints(IEnumerable<Point> points)
    {
        if (points == null)
        {
            return null;
        }
        PointCollection pointCollection = new PointCollection();
        foreach (Point point in points)
        {
            double x = point.X;
            double y = point.Y;
            if (_axes[0].Scaling == AxisScaling.Logarithmic)
            {
                x = ((x <= 0.0) ? double.NaN : Math.Log10(x));
            }
            if (_axes[1].Scaling == AxisScaling.Logarithmic)
            {
                y = ((y <= 0.0) ? double.NaN : Math.Log10(y));
            }
            pointCollection.Add(new Point(x, y));
        }
        return pointCollection;
    }

    private bool TraceTypeIsLinearAndAxesScalingIsLinear()
    {
        return Type == TraceType.Lines;
    }

    private bool Containes(Point arg)
    {
        if (RangeAxisModel.Contains(arg.X))
        {
            return ResultAxisModel.Contains(arg.Y);
        }
        return false;
    }
}
