using System.Collections.Generic;

namespace Quire.Controls.Plots.Presenter;

public partial interface IEquationItem
{
    IPlotEquation Equation { get; set; }

    IEnumerable<ITraceModel> Traces { get; }

    void AddTrace(ITraceModel trace);

    void RemoveTrace(int index);

    void AddTrace(ITraceModel trace, int index);
}
