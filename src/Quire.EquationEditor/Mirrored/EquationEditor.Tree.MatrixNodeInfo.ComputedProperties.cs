namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
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
