#nullable disable
using System;
using Quire;
using Quire.Matrices;

namespace Quire.Controls.Core;

public partial class RowColumnHelper
{
    private int _columnCount;

    public int ColumnCount => _columnCount;

    public RowColumnHelper(int columnCount)
    {
        _columnCount = columnCount;
    }

    public int RowIndexFromCellIndex(int cellIndex)
    {
        return MatrixHelper.RowOf(cellIndex, ColumnCount);
    }

    public int ColIndexFromCellIndex(int cellIndex)
    {
        return MatrixHelper.ColumnOf(cellIndex, ColumnCount);
    }

    public int IndexFromRowColumn(int rowIndex, int columnIndex)
    {
        return MatrixHelper.IndexOf(rowIndex, columnIndex, ColumnCount);
    }

    public void GetRowIndices(int cellIndexA, int cellIndexB, out int minRowIndex, out int maxRowIndex)
    {
        int val = RowIndexFromCellIndex(cellIndexA);
        int val2 = RowIndexFromCellIndex(cellIndexB);
        minRowIndex = Math.Min(val, val2);
        maxRowIndex = Math.Max(val, val2);
    }

    public void GetColumnsIndices(int cellIndexA, int cellIndexB, out int minColumnIndex, out int maxColumnIndex)
    {
        int val = ColIndexFromCellIndex(cellIndexA);
        int val2 = ColIndexFromCellIndex(cellIndexB);
        minColumnIndex = Math.Min(val, val2);
        maxColumnIndex = Math.Max(val, val2);
    }

    public void GetMinMaxRowColIndices(int corner1, int corner2, out int left, out int right, out int top, out int bottom)
    {
        DiscreteRect discreteRect = MatrixHelper.RectFromCornerIndices(corner1, corner2, ColumnCount);
        left = discreteRect.Left;
        right = discreteRect.Right;
        top = discreteRect.Top;
        bottom = discreteRect.Bottom;
    }
}
