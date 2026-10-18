#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial struct TraceTypeRange
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceTypeRangeMechanicalDifferentialTests")]
    public int Start { get; set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceTypeRangeMechanicalDifferentialTests")]
    public int End { get; set; }
}
