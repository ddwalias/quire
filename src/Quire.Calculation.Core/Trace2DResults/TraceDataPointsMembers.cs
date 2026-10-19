using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class TraceDataPoints
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests")]
    public TraceDataPoints(IVectorListResultNode rangeVectors, IVectorListResultNode resultVectors)
    {
        if (rangeVectors == null || resultVectors == null)
            throw new ArgumentNullException();

        RangePoints = rangeVectors;
        ResultPoints = resultVectors;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceDataPointsMechanicalDifferentialTests")]
    public bool IsEmpty
    {
        get
        {
            if (ResultPoints == null)
                return RangePoints == null;

            return false;
        }
    }
}
