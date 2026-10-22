#nullable disable
#pragma warning disable

namespace Quire.Matrices;

partial class SparseMatrix<TItem>
{
    public TItem EmptyPosition
    {
        get => _emptyPosition;
        set
        {
            TItem oldPosition = _emptyPosition;
            _emptyPosition = value;
            FireChanged(new SparseMatrixItemChangedEventArgs(new global::Quire.TableCoordinate(-1, -1), oldPosition, value));
        }
    }
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
}
