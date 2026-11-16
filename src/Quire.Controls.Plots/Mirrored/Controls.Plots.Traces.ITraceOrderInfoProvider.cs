namespace Quire.Controls.Plots.Traces;

internal partial interface ITraceOrderInfoProvider
{
    double ClusterSpace { get; }

    int TraceCountByType { get; }

    int TracePlaceByType { get; }

    double PlotMinGap { get; }

    bool IsDiscrete { get; }
}
