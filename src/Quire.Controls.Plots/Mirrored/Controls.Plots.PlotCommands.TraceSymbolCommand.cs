using System.Collections.Generic;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class TraceSymbolCommand
{
    private readonly TraceSymbol _newSymbol;

    public TraceSymbolCommand(IEnumerable<IPlotTraceStyle> traceList, TraceSymbol symbol)
        : base(traceList, "TraceSymbolCommand")
    {
        _newSymbol = symbol;
    }

    protected override bool DoTrace(IPlotTraceStyle trace)
    {
        return trace.ChangeSymbol(_newSymbol);
    }
}
