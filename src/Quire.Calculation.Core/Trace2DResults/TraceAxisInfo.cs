#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceAxisInfoMechanicalDifferentialTests")]
    public TraceAxisInfo(double min, double max, IEnumerable<Unit> units)
    {
        Units = Enumerable.ToList<Unit>(units);
        Min = min;
        Max = max;
    }
}
