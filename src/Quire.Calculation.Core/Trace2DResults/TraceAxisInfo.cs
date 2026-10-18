#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class TraceAxisInfo
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceAxisInfoMechanicalDifferentialTests")]
    public double Max { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceAxisInfoMechanicalDifferentialTests")]
    public double Min { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceAxisInfoMechanicalDifferentialTests")]
    public IList<Unit> Units { get; private set; }
}
