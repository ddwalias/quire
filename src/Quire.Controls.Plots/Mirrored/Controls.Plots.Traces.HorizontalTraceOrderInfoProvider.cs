namespace Quire.Controls.Plots.Traces;

internal partial class HorizontalTraceOrderInfoProvider
{
    public override double ClusterSpace => _plotTrace.ActualWidth / (double)(_plotTrace.XAxis.TickMarks + 1);

    public override double PlotMinGap => (double)_plotTrace.GetValue(PlotControl.PlotXMinGapProperty);

    public override bool IsDiscrete => _plotTrace.XAxis.IsDiscrete;

    public HorizontalTraceOrderInfoProvider(PlotTrace plotTrace)
        : base(plotTrace)
    {
    }
}
