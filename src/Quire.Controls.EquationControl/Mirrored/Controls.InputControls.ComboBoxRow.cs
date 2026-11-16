using System.Collections.Generic;
using System.Linq;
using Quire.Core;

namespace Quire.Controls.InputControls;

public abstract partial class ComboBoxRow
{
    private readonly List<CellData> _cells;

    protected ComboBoxRow(int rowIndex, int count)
    {
        if (DebugTools.DevAssert(rowIndex >= 0 && count >= 2, null).Failed)
        {
            return;
        }

        _cells = new List<CellData>(count);
        count.Times(colIndex =>
        {
            AddCell(new CellData(string.Empty, rowIndex, colIndex));
        });
    }

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

    public IEnumerable<ICellData> AllCells => _cells;

    // Parked compiler difference: Roslyn in Release mode eliminates the stloc.0/ldloc.0 return shape emitted by Mathcad's compiler.
    protected IEnumerable<ICellData> AllButFirstCell => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int CellCount => _cells.Count;

    public bool IsValid(int rowIndex)
    {
        int num = 0;
        foreach (CellData cell in _cells)
        {
            if (cell.Row != rowIndex || cell.Column != num)
            {
                return false;
            }
            num++;
        }

        return true;
    }

    internal void RemoveLastCell()
    {
        if (DebugTools.DevAssert(CellCount > 2, null).Passed)
        {
            _cells.RemoveLast();
        }
    }

    internal void RemoveCellAt(int index)
    {
        if (DebugTools.DevAssert(CellCount > 2 && 0 <= index && index < CellCount, null).Passed)
        {
            _cells.RemoveAt(index);
        }
    }

    internal void AddCell(CellData cellData)
    {
        if (DebugTools.AssertNotNull(cellData, null).Passed)
        {
            _cells.Add(cellData);
        }
    }

    internal void InsertCell(int index, CellData cellData)
    {
        if (DebugTools.AssertNotNull(cellData, null).Passed && DebugTools.DevAssert(0 <= index && index <= CellCount, null).Passed)
        {
            _cells.Insert(index, cellData);
        }
    }

    internal ICellData GetCellAt(int index)
    {
        if (0 > index || index >= CellCount)
        {
            return null;
        }

        return _cells[index];
    }
}
