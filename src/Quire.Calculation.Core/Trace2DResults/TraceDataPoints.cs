#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class TraceDataPoints
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests")]
    public TraceDataPoints() { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests")]
    public IVectorListResultNode RangePoints { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests")]
    public IVectorListResultNode ResultPoints { get; private set; }
}
