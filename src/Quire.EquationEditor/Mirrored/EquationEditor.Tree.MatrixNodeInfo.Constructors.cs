namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
    public MatrixNodeInfo()
        : this(new TableSize(1, 1), 0, new TableCoordinate(0, 0), new TableSize(1, 1), false, false)
    {
    }

    public MatrixNodeInfo(MatrixNodeInfo other)
        : this(other.ViewportSize, other.Origin, other.ViewportOffset, other.FullSize, other.IsInResult, other.ShowIndices)
    {
    }

    public MatrixNodeInfo(TableSize size, int origin, TableCoordinate offset)
        : this(size, origin, offset, new TableSize(size.Rows + offset.Row, size.Columns + offset.Column), false, false)
    {
    }

    public MatrixNodeInfo(TableSize size, int origin, TableCoordinate offset, TableSize totalSize, bool isInResult, bool showResultMatrixIndices)
    {
        MatrixFormat = new global::Quire.Controls.Core.MatrixFormat();
        FullSize = totalSize;
        ViewportSize = size;
        Origin = origin;
        ViewportOffset = offset;
        IsInResult = isInResult;
        ShowIndices = showResultMatrixIndices;
    }
}
