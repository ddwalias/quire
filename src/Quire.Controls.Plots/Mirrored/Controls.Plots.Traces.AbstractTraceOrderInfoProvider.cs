namespace Quire.Controls.Plots.Traces;

internal abstract partial class AbstractTraceOrderInfoProvider : ITraceOrderInfoProvider
{
    protected readonly PlotTrace _plotTrace;

    public abstract double ClusterSpace { get; }

    public int TraceCountByType => PlotGraphControl.GetTraceCountByType(_plotTrace);

    public int TracePlaceByType => PlotGraphControl.GetTracePlaceByType(_plotTrace);

    public abstract double PlotMinGap { get; }

    public abstract bool IsDiscrete { get; }

    protected AbstractTraceOrderInfoProvider(PlotTrace plotTrace)
    {
        _plotTrace = plotTrace;
    }
}
