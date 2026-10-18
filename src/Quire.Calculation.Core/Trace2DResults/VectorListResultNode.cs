#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class VectorListResultNode
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public int SingleVectorLength { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public VectorsType VectorsType { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    private readonly IList<IListResultNode> _vectors;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public IList<IListResultNode> Vectors => _vectors;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public int Count => _vectors.Count;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public IListResultNode this[int index] { get => _vectors[index]; set => _vectors[index] = value; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public ResultValueType GetVectorValuesType(int index) => GetResultType(Vectors[index]);

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public int ElementsCount => Count * SingleVectorLength;
}
