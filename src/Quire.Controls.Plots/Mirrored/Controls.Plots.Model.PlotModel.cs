#nullable disable
#pragma warning disable

using System.Linq;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class PlotModel
{
    public IAxisModel rangeAxis => _rangeAxis;

    public IAxisModel PrimaryResultAxis => _primaryResultAxis;

    public IPlotTitleModel PlotTitleModel => _plotTitleModel;

    public IPlotLegendModel PlotLegendModel => _plotLegendModel;

    public void FireDeserializationStarted()
    {
        if (DeserializationStarted != null)
        {
            DeserializationStarted();
        }
    }

    public void FireDeserializationFinished()
    {
        if (DeserializationFinished != null)
        {
            DeserializationFinished();
        }
    }

    public void RemoveTrace(ITraceModel trace)
    {
        _traces.Remove(trace);
    }

    public ITraceModel FirstTraceInAppearanceOrder()
    {
        ITraceModel result = null;
        IEquationItem equation = rangeAxis.Equations.FirstOrDefault();
        if (equation != null)
        {
            result = equation.Traces.FirstOrDefault();
        }
        return result;
    }
}
