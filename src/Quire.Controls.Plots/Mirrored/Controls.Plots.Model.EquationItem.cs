using System.Collections.Generic;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

internal partial class EquationItem : IEquationItem
{
    private readonly List<ITraceModel> _traces = new List<ITraceModel>();

    public IPlotEquation Equation { get; set; }

    public IEnumerable<ITraceModel> Traces => _traces;

    public EquationItem(IPlotEquation equation)
    {
        Equation = equation;
    }

    public void AddTrace(ITraceModel trace)
    {
        _traces.Add(trace);
    }

    public void AddTrace(ITraceModel trace, int index)
    {
        _traces.Insert(index, trace);
    }

    public void RemoveTrace(int index)
    {
        _traces.RemoveAt(index);
    }
}
