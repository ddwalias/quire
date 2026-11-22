namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
    public TableSize ViewportSize
    {
        get => MatrixFormat.Size;
        set
        {
            if (value.Rows >= 1 && value.Rows <= FullSize.Rows && value.Columns >= 1 && value.Columns <= FullSize.Columns)
            {
                MatrixFormat.Size = value;
            }
            else
            {
                DebugTools.FailAssertion("Viewport size is incorrect.");
                MatrixFormat.Size = new TableSize(1, 1);
            }
        }
    }

    public TableCoordinate ViewportOffset
    {
        get => MatrixFormat.Offset;
        set
        {
            if (value.Row >= 0 && value.Row < FullSize.Rows && value.Column >= 0 && value.Column < FullSize.Columns)
            {
                MatrixFormat.Offset = value;
            }
            else
            {
                DebugTools.FailAssertion("Viewport offset is incorrect.");
                MatrixFormat.Offset = new TableCoordinate(0, 0);
            }
        }
    }

    public bool ShowIndices
    {
        get => MatrixFormat.ShowIndices;
        set => MatrixFormat.ShowIndices = value;
    }

    public bool CollapseNestedMatrices
    {
        get => MatrixFormat.CollapseNestedMatrices;
        set => MatrixFormat.CollapseNestedMatrices = value;
    }

    public int Origin
    {
        get => MatrixFormat.Origin;
        set => MatrixFormat.Origin = value;
    }
}
