#nullable disable
#pragma warning disable

namespace Quire.Matrices;

partial class SparseMatrix<TItem>
{
    public TItem this[int r, int c]
    {
        get
        {
            return this[new global::Quire.TableCoordinate
            {
                Row = r,
                Column = c
            }];
        }
        set
        {
            this[new global::Quire.TableCoordinate
            {
                Row = r,
                Column = c
            }] = value;
        }
    }

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

    [global::Quire.Mirror.MirrorVerified("Release IL")]
    private void PositionCheck(global::Quire.TableCoordinate position)
    {
        if (position.Row < 0 || position.Column < 0)
        {
            throw new global::System.ArgumentOutOfRangeException("position", "The position row and column values must be 0 or greater.");
        }
    }
}
