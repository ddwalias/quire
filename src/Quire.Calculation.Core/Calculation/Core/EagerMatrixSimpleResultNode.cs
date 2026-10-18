#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EagerMatrixSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    private readonly IList<SimpleResultNode> _children;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Count => _children.Count;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Columns { get; private set; }
}
