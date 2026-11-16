namespace Quire.Controls.Plots.Traces;

internal partial class VerticalTraceOrderInfoProvider
{
    public override double ClusterSpace => _plotTrace.ActualHeight / (double)(_plotTrace.YAxis.TickMarks + 1);

    public override double PlotMinGap => (double)_plotTrace.GetValue(PlotControl.PlotYMinGapProperty);

    public override bool IsDiscrete => _plotTrace.YAxis.IsDiscrete;

    public VerticalTraceOrderInfoProvider(PlotTrace plotTrace)
        : base(plotTrace)
    {
    }
}
