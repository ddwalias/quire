#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Matrices;

public partial class SparseMatrix<TItem>
{
    [MirrorVerified("Release IL")]
    private void FireChanged(SparseMatrixItemChangedEventArgs e)
    {
        if (Changed != null)
        {
            Changed(this, e);
        }
    }

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    protected Dictionary<TableCoordinate, TItem> _itemMap = new Dictionary<TableCoordinate, TItem>();

    private EqualityComparer<TItem> _itemEqualityComparer = EqualityComparer<TItem>.Default;

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    public int PopulatedCount => _itemMap.Count;

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    public IEnumerable<TableCoordinate> PopulatedPositions => _itemMap.Keys;

    [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
    public event SparseMatrix<TItem>.SparseMatrixChangedEventHandler Changed;

    public global::Quire.TableSize Size
    {
        get
        {
            global::Quire.TableSize size = default;
            size.Rows = Rows;
            size.Columns = Columns;
            return size;
        }
    }

    public SparseMatrix() { }

    public int Rows
    {
        get
        {
            if (PopulatedCount == 0)
            {
                return 0;
            }
            return PopulatedPositions.Max((TableCoordinate coord) => coord.Row) + 1;
        }
    }

    public int Columns
    {
        get
        {
            if (PopulatedCount == 0)
            {
                return 0;
            }
            return PopulatedPositions.Max((TableCoordinate coord) => coord.Column) + 1;
        }
    }

    partial class SparseMatrixItemChangedEventArgs
    {
        [MirrorVerified("PtcMatricesSparseMatrix1MechanicalDifferentialTests")]
        public TableCoordinate Position { get; set; }
    }

    partial class SparseMatrixItemChangedEventArgs
    {
        [MirrorVerified("Release IL")]
        public TItem OldItem;

        [MirrorVerified("Release IL")]
        public TItem NewItem;

        [MirrorVerified("Release IL")]
        public SparseMatrixItemChangedEventArgs(TableCoordinate position, TItem oldItem, TItem newItem)
        {
            Position = position;
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}
