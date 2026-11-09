namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
    private bool b;

    private TableSize c;

    public bool IsMatrixComplete
    {
        get => b;
        set => b = value;
    }

    public TableSize FullSize
    {
        get => c;
        set => c = value;
    }

    public int RowsBeforeViewport => ViewportOffset.Row;

    public int ColsBeforeViewport => ViewportOffset.Column;

    public int RowsAfterViewport => FullSize.Rows - (ViewportOffset.Row + ViewportSize.Rows);

    public int ColsAfterViewport => FullSize.Columns - (ViewportOffset.Column + ViewportSize.Columns);
}
