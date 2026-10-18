#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public void SetElementAt(int r, int c, IResultNode element) => this[r, c] = (SimpleResultNode)element;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Matrix: ({0},{1})", Rows, Columns);

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Rows => Count / Columns;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public SimpleResultNode this[int r, int c] { get => _children[r * Columns + c]; set => _children[r * Columns + c] = value; }
}
