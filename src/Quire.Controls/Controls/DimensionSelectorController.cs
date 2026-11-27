#nullable disable
using System.Collections.Generic;
using System.Windows;
using Quire.Matrices;

namespace Quire.Controls;

internal partial class DimensionSelectorController
{
    private readonly IDimensionSelectorView _view;

    public DimensionSelectorController(IDimensionSelectorView view)
    {
        _view = view;
        _view.MouseOverCell += HandleMouseOverCell;
        _view.Commit += HandleCommit;
    }

    private void HandleCommit()
    {
        _view.SetSelection();
        _view.RowDisplayText = null;
        _view.ColumnDisplayText = null;
    }

    private void HandleMouseOverCell(int numberOfColumns, int childIndex)
    {
        int num = childIndex / numberOfColumns + 1;
        int columnCount = childIndex % numberOfColumns + 1;
        _view.CommandParameter = new Size((double)columnCount, (double)num);
        List<int> list = new List<int>(num * columnCount);
        for (int rowIndex = 0; rowIndex < num; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                list.Add(MatrixHelper.IndexOf(rowIndex, columnIndex, numberOfColumns));
            }
        }
        _view.SetSelection(list.ToArray());
        _view.RowDisplayText = num.ToString();
        _view.ColumnDisplayText = columnCount.ToString();
    }
}
