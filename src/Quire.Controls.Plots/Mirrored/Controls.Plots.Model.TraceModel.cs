#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire;
using Quire.Calculation.Core;
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

    private Interval GetParameterRange()
    {
        IAxisModel rangeAxisModel = RangeAxisModel;
        if (rangeAxisModel.Type != AxesType.Angular)
        {
            return new Interval
            {
                Lower = GetXAxisLimitEditorValue(rangeAxisModel.StartExpression, rangeAxisModel.StartDefault),
                Upper = GetXAxisLimitEditorValue(rangeAxisModel.EndExpression, rangeAxisModel.EndDefault)
            };
        }
        return _angularRange;
    }

    private static double GetXAxisLimitEditorValue(IAxisLimitEditor limitEditor, double defaultValue)
    {
        if (limitEditor != null && limitEditor.IsValueValid)
        {
            return limitEditor.CalculatedValue;
        }
        return defaultValue;
    }

    private string GetxyplotScalingRl(AxesIndex axesType)
    {
        IPlotEquation equation = _equations[(int)axesType].Equation;
        return GetReplacedRl(equation.ScalingEquation, equation.ScalingRl);
    }

    private string GetExpRl(AxesIndex axesType)
    {
        return GetReplacedRl(_equations[(int)axesType].Equation.Expression);
    }

    private void InvalidateEngineResult()
    {
        EngineResult = default;
    }

    private void FireIsTraceCommitted(bool isTraceCommitted)
    {
        if (IsTraceCommittedChanged != null)
        {
            IsTraceCommittedChanged(isTraceCommitted);
        }
    }

    private void FireResultValidChanged(bool resultValid)
    {
        if (ResultValidChanged != null)
        {
            ResultValidChanged(resultValid);
        }
    }

    private void FireAnalyzedDataChanged()
    {
        Action<ITraceModel> handler = AnalyzedDataChanged;
        if (handler != null)
        {
            handler(this);
        }
    }

    private void FireValidateTraceVsPlotType()
    {
        if (ValidateTraceVsPlotType != null)
        {
            ValidateTraceVsPlotType();
        }
    }

    private void FireStyleChanged()
    {
        if (StyleChanged != null)
        {
            StyleChanged();
        }
    }

    private void SetIsCommitted(bool value)
    {
        if (value != _isCommitted)
        {
            _isCommitted = value;
            FireIsTraceCommitted(_isCommitted);
        }
    }

    protected void FireEquationError(EngineError engineError)
    {
        Action<EngineError> handler = EquationError;
        if (handler != null)
        {
            handler(engineError);
        }
    }
}
