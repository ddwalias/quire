#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class DisplayMatrixSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    private readonly EmptySimpleResultNode _emptyNode = new EmptySimpleResultNode();

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

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public TableSize Size => new TableSize
    {
        Rows = Rows,
        Columns = Columns
    };

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public SimpleResultNode this[int r, int c]
    {
        get
        {
            r -= ViewportOffset.Row;
            c -= ViewportOffset.Column;
            if (r < 0 || c < 0 || r >= Viewport.Rows || c >= Viewport.Columns)
            {
                return _emptyNode;
            }
            return (SimpleResultNode)Viewport[r, c];
        }
    }

    IResultNode IMatrixResultNode.this[int r, int c] => this[r, c];

    [MirrorVerified("PtcCalculationCoreDisplayMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public DisplayMatrixSimpleResultNode(int rowCount, int colCount, TableCoordinate viewportOffset, TableSize viewportSize)
    {
        Rows = rowCount;
        Columns = colCount;
        ViewportOffset = viewportOffset;
        Viewport = new EagerMatrixSimpleResultNode(viewportSize.Rows, viewportSize.Columns);
    }
}
