#nullable disable
#pragma warning disable

namespace Quire.Matrices;

partial class SparseMatrix<TItem>
{
    public TItem this[global::Quire.TableCoordinate position]
    {
        get
        {
            PositionCheck(position);
            if (_itemMap.TryGetValue(position, out TItem value))
            {
                return value;
            }
            return EmptyPosition;
        }
        set
        {
            PositionCheck(position);
            TItem oldItem = this[position];
            if (_itemEqualityComparer.Equals(value, EmptyPosition))
            {
                _itemMap.Remove(position);
            }
            else
            {
                _itemMap[position] = value;
            }
            FireChanged(new SparseMatrixItemChangedEventArgs(position, oldItem, value));
        }
    }

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
