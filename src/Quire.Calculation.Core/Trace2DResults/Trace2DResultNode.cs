#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class Trace2DResultNode : SimpleResultNode, ITrace2DResultNode, IResultNode, IVisitable<IResultNodeVisitor>
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultNodeMechanicalDifferentialTests")]
    public Trace2DResultNode(ITraceAxisInfo rangeAxisInfo, ITraceAxisInfo resultAxisInfo, ITraceDataPoints dataDataPoints, Trace2DResultType type)
    {
        RangeAxisInfo = rangeAxisInfo;
        ResultAxisInfo = resultAxisInfo;
        DataPoints = dataDataPoints;
        TraceResultType = type;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultNodeMechanicalDifferentialTests")]
    public ITraceAxisInfo RangeAxisInfo { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultNodeMechanicalDifferentialTests")]
    public ITraceAxisInfo ResultAxisInfo { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultNodeMechanicalDifferentialTests")]
    public ITraceDataPoints DataPoints { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultNodeMechanicalDifferentialTests")]
    public Trace2DResultType TraceResultType { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTrace2DResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
