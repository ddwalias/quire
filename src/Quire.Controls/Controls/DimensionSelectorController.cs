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
        int num2 = childIndex % numberOfColumns + 1;
        _view.CommandParameter = new Size((double)num2, (double)num);
        List<int> list = new List<int>(num * num2);
        for (int num3 = 0; num3 < num; num3++)
        {
            for (int num4 = 0; num4 < num2; num4++)
            {
                list.Add(MatrixHelper.IndexOf(num3, num4, numberOfColumns));
            }
        }
        _view.SetSelection(list.ToArray());
        _view.RowDisplayText = num.ToString();
        _view.ColumnDisplayText = num2.ToString();
    }
}
