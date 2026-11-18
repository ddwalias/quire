using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

internal partial class MatrixDisplayHelper
{
    public TableSize DisplaySize => _displaySize;

    public static bool MatrixNodeIsInResult(MathNode mathNode)
    {
        MathNode evaluation = mathNode.GetNearestAncestor("EVALUATION");
        if (evaluation != null)
        {
            return mathNode.PathFromAncestor(evaluation)[0] == 1;
        }

        return false;
    }

    public bool ShowChildAsIs(TableCoordinate cellPosition)
    {
        return cellPosition.Row < DisplaySize.Rows && cellPosition.Column < DisplaySize.Columns;
    }

    public bool ShowEmptyCell(TableCoordinate cellPosition)
    {
        return (cellPosition.Row == 0 && HasRowOffset())
            || (cellPosition.Column == 0 && HasColumnOffset())
            || (cellPosition.Row == DisplaySize.Rows - 1 && (HasTooManyRows() || HasRowRemainder()))
            || (cellPosition.Column == DisplaySize.Columns - 1 && (HasTooManyColumns() || HasColumnRemainder()));
    }

    private static bool IsChildInFirstDisplayRow(int childRow)
    {
        return childRow == 0;
    }

    private static bool IsChildInFirstDisplayColumn(int childColumn)
    {
        return childColumn == 0;
    }

    private bool IsChildInLastDisplayRow(int childRow)
    {
        return childRow == DisplaySize.Rows - 1;
    }

    private bool IsChildInLastDisplayColumn(int childColumn)
    {
        return childColumn == DisplaySize.Columns - 1;
    }

    public bool HasRowOffset()
    {
        return _matrixNodeInfo.ViewportOffset.Row > 0;
    }

    public bool HasColumnOffset()
    {
        return _matrixNodeInfo.ViewportOffset.Column > 0;
    }

    private bool HasRowRemainder()
    {
        return _matrixNodeInfo.Remainder.Rows > 0;
    }

    private bool HasColumnRemainder()
    {
        return _matrixNodeInfo.Remainder.Columns > 0;
    }

    private bool HasTooManyRows()
    {
        return _matrixNodeInfo.ViewportSize.Rows > _displaySize.Rows;
    }

    private bool HasTooManyColumns()
    {
        return _matrixNodeInfo.ViewportSize.Columns > _displaySize.Columns;
    }
}
