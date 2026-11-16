using System;
using Quire.Calculation.Core.Trace2DResults;

namespace Quire.Controls.Plots.Traces;

internal partial class TraceRendererFactory
{
    private readonly PlotTrace _ownerTrace;

    public TraceRendererFactory(PlotTrace ownerTrace)
    {
        _ownerTrace = ownerTrace;
    }

    public TraceRenderer GetRenderer(TraceType traceType)
    {
        switch (traceType)
        {
            case TraceType.Lines:
                return new LinesTraceRenderer(_ownerTrace);
            case TraceType.Columns:
                return new ColumnsTraceRenderer(_ownerTrace, new HorizontalTraceOrderInfoProvider(_ownerTrace));
            case TraceType.Effects:
                return new LinesTraceRenderer(_ownerTrace);
            case TraceType.Boxes:
                return new BoxesTraceRenderer(_ownerTrace, new HorizontalTraceOrderInfoProvider(_ownerTrace));
            case TraceType.Bars:
                return new BarsTraceRenderer(_ownerTrace, new VerticalTraceOrderInfoProvider(_ownerTrace));
            case TraceType.Errors:
                return new ErrorsTraceRenderer(_ownerTrace, new HorizontalTraceOrderInfoProvider(_ownerTrace));
            case TraceType.Stems:
                return new StemsTraceRenderer(_ownerTrace);
            case TraceType.Waterfall:
                return new LinesTraceRenderer(_ownerTrace);
            default:
                throw new ArgumentOutOfRangeException("traceType");
        }
    }
}
