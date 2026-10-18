#nullable disable
using Quire.Mirror;

namespace Quire.Matrices;

public partial class SparseMatrix<TItem>
{
    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    protected Dictionary<TableCoordinate, TItem> _itemMap;

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    public int PopulatedCount => _itemMap.Count;

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    public IEnumerable<TableCoordinate> PopulatedPositions => _itemMap.Keys;

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    public event SparseMatrix<TItem>.SparseMatrixChangedEventHandler Changed;

    partial class SparseMatrixItemChangedEventArgs
    {
        [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
        public TableCoordinate Position { get; set; }
    }
}
