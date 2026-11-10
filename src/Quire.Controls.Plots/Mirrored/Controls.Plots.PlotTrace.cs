#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotTrace
{
    public void SelectTrace()
    {
        IsSelected = true;
    }

    public void DeselectTrace()
    {
        IsSelected = false;
    }

    public void fireTraceClicked()
    {
        if (TraceClicked != null)
        {
            TraceClicked(this);
        }
        IsSelected = true;
    }

    public void ResetIsTypeDefault()
    {
        if (RequestResetIsTypeDefault != null)
        {
            RequestResetIsTypeDefault();
        }
    }

    private void FireTypeChanged()
    {
        if (TypeChanged != null)
        {
            TypeChanged();
        }
        FireTraceChanged();
    }

    private void FireErrorChanged()
    {
        if (ErrorChanged != null)
        {
            ErrorChanged(this);
        }
    }

    private void FireCalculatingStatusChangedEvent()
    {
        if (CalculatingStatusChangedEvent != null)
        {
            CalculatingStatusChangedEvent();
        }
    }

    private void FireTraceChanged()
    {
        Action<IPlotTrace> handler = TraceChanged;
        if (handler != null)
        {
            handler(this);
        }
    }

    public bool ChangeSymbol(TraceSymbol value)
    {
        Symbol = value;
        if (RestrictedToDensityPointsOrSolidLine)
        {
            LineStyle = Symbol == Presenter.TraceSymbol.none ? LineStyle.Solid : LineStyle.None;
        }
        return Symbol == value;
    }

    public bool ChangeLineStyle(LineStyle value)
    {
        LineStyle = value;
        if (RestrictedToDensityPointsOrSolidLine)
        {
            Symbol = LineStyle == LineStyle.None ? Presenter.TraceSymbol.point : Presenter.TraceSymbol.none;
        }
        return LineStyle == value;
    }

    public void SetEquation(AxesType axis, IPlotEquation equation)
    {
        _plotEquations[(int)AxesTypeToIndex.Index(axis)] = equation;
    }

    public IPlotEquation GetEquation(AxesIndex axesIndex)
    {
        return _plotEquations[(int)axesIndex];
    }

    private static double GetMinGap(List<double> values)
    {
        values.Sort();
        double num = double.PositiveInfinity;
        for (int i = 1; i < values.Count; i++)
        {
            double num2 = values[i] - values[i - 1];
            if (num2 > 0.0)
            {
                num = Math.Min(num, num2);
            }
        }
        return num;
    }
}
