using Quire.Controls.InputControls;
using System.Collections.Generic;
using System.Linq;
namespace Quire.Controls.InputControls;

public partial class ComboBoxRow
{

    internal void RemoveLastCell()
    {
        if (DebugTools.DevAssert(CellCount > 2, null).Passed)
        {
            EnumerableExtensions.RemoveLast(_cells);
        }
    }

    internal void AddCell(CellData cellData)
    {
        if (DebugTools.AssertNotNull(cellData, null).Passed)
        {
            _cells.Add(cellData);
        }
    }

    internal ICellData GetCellAt(int index)
    {
        if (!(0 <= index && index < CellCount))
        {
            return null;
        }

        return _cells[index];
    }

    public IEnumerable<ICellData> AllCells => _cells;

    public int CellCount => _cells.Count;

    protected ICellData FirstCell
    {
        get
        {
            if (!DebugTools.DevAssert(CellCount > 0, null).Passed)
            {
                return null;
            }

            return _cells[0];
        }
    }

    internal void RemoveCellAt(int index)
    {
        if (DebugTools.DevAssert(CellCount > 2 && 0 <= index && index < CellCount, null).Passed)
        {
            _cells.RemoveAt(index);
        }
    }

    internal void InsertCell(int index, CellData cellData)
    {
        if (DebugTools.AssertNotNull(cellData, null).Passed)
        {
            if (DebugTools.DevAssert(0 <= index && index <= CellCount, null).Passed)
            {
                _cells.Insert(index, cellData);
            }
        }
    }
}
