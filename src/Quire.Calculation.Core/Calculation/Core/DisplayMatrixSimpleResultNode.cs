#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class DisplayMatrixSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Rows { get; private set; }

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Columns { get; private set; }

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public TableCoordinate ViewportOffset { get; private set; }

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public IEagerMatrixResultNode Viewport { get; private set; }

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Count => Rows * Columns;
}
