using System.Collections.Generic;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class TraceLineCommand
{
    private readonly LineStyle _newLine;

    public TraceLineCommand(IEnumerable<IPlotTraceStyle> traceList, LineStyle lineStyle)
        : base(traceList, "TraceLineCommand")
    {
        _newLine = lineStyle;
    }

    protected override bool DoTrace(IPlotTraceStyle trace)
    {
        return trace.ChangeLineStyle(_newLine);
    }
}
