using System;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal abstract partial class ClustersTraceRenderer : TraceRenderer
{
    private readonly ITraceOrderInfoProvider _traceOrderInfoProvider;

    private const double SpaceBetweenClusters = 0.2;

    private const double SpaceBetweenPoints = 0.1;

    private double Margin { get; set; }

    protected abstract double MaxSize { get; }

    protected double ActualSize { get; private set; }

    protected ClustersTraceRenderer(IPlotTrace ownerTrace, ITraceOrderInfoProvider traceOrderInfoProvider)
        : base(ownerTrace)
    {
        _traceOrderInfoProvider = traceOrderInfoProvider;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (_traceOrderInfoProvider.IsDiscrete)
        {
            double clusterSpace = _traceOrderInfoProvider.ClusterSpace;
            int traceCountByType = _traceOrderInfoProvider.TraceCountByType;
            int tracePlaceByType = _traceOrderInfoProvider.TracePlaceByType;
            double num = clusterSpace / (0.2 + (double)traceCountByType + (double)(traceCountByType - 1) * 0.1);
            ActualSize = Math.Min(num, MaxSize);
            Margin = num * (0.1 + (double)tracePlaceByType * 1.1) + (num - ActualSize) / 2.0 - clusterSpace / 2.0;
        }
        else
        {
            double val = _traceOrderInfoProvider.PlotMinGap * 0.9;
            ActualSize = Math.Min(val, MaxSize);
            Margin = ActualSize / 2.0 - _traceOrderInfoProvider.PlotMinGap / 2.0;
        }
    }

    protected double GetStart(double d)
    {
        if (_traceOrderInfoProvider.IsDiscrete)
        {
            return d + Margin;
        }
        return d - ActualSize / 2.0;
    }
}
