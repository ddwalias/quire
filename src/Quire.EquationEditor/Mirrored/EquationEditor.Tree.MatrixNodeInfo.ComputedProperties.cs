namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
    public bool IsResized
    {
        get
        {
            // Mathcad branches to the true return when the offset differs from zero.
            if (!(ViewportOffset != new TableCoordinate(0, 0)))
            {
                return ViewportSize != FullSize;
            }

            return true;
        }
    }

    public TableSize Remainder
    {
        get
        {
            int rows = FullSize.Rows - ViewportOffset.Row - ViewportSize.Rows;
            int columns = FullSize.Columns - ViewportOffset.Column - ViewportSize.Columns;
            return new TableSize(rows > 0 ? rows : 0, columns > 0 ? columns : 0);
        }
    }

    public TableSize ActualSize
    {
        get
        {
            if (!IsMatrixComplete)
            {
                return ViewportSize;
            }

            return FullSize;
        }
    }

    public bool IsInResult
    {
        get => !IsMatrixComplete;
        set => IsMatrixComplete = !value;
    }
}
