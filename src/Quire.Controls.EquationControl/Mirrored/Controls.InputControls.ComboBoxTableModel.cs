using System;
using Quire.Controls.InputControls;
using System.Collections.Generic;
using System.Linq;
using Quire.EquationEditor.View;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class ComboBoxTableModel
{

    public ComboBoxTableModel()
        : this(2, 2)
    {
    }

    private ComboBoxTableModel(int rows, int columns)
    {
        if (DebugTools.DevAssert(rows >= 2 && columns >= 2, null).Failed)
        {
            return;
        }
        Columns = columns;
        _topRow = new ComboBoxTopRow(Columns);
        _nameValuesRows = new List<NameValuesRow>(rows);
        for (int i = 1; i < rows; i++)
        {
            _nameValuesRows.Add(new NameValuesRow(i, Columns));
        }
    }

    public int Rows => _nameValuesRows.Count + 1;

    public int Columns
    {
        get => _columns;
        private set
        {
            if (DebugTools.DevAssert(value > 0, null).Passed)
            {
                _columns = value;
            }
        }
    }

    public string TableTitle
    {
        get
        {
            return _topRow.TableTitleCell.Text;
        }
        private set
        {
            if (DebugTools.AssertNotNull(_topRow.TableTitleCell, null).Passed)
            {
                _topRow.TableTitleCell.Text = value;
            }
        }
    }

    public int SelectedTableRowIndex
    {
        get
        {
            return _selectedRowIndex;
        }
        set
        {
            if (value != _selectedRowIndex && DebugTools.DevAssert(0 <= value && value < Rows, null).Passed)
            {
                _selectedRowIndex = value;
            }
        }
    }

    public int SelectedNameValuesRowIndex
    {
        get
        {
            return Math.Max(SelectedTableRowIndex - 1, 0);
        }
        set
        {
            SelectedTableRowIndex = value + 1;
        }
    }

    public System.Collections.Generic.IEnumerable<Quire.Controls.InputControls.ComboBoxRow> AllRows => _topRow.Yield<ComboBoxRow>().Concat<ComboBoxRow>(_nameValuesRows);

    public System.Collections.Generic.IEnumerable<Quire.EquationEditor.View.IReadonlyCellData> UnitsForValueColumns => _topRow.UnitCells;

    public IEnumerable<INameValuesRow> NameValuesRows => _nameValuesRows;

    public IEnumerable<IReadonlyCellData> GetColumnCells(int columnIndex) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    IReadonlyCellData IComboBoxTableModel.GetCellAt(int rowIdx, int colIdx)
    {
        return GetCellAt(rowIdx, colIdx);
    }

    private ICellData GetCellAt(int rowIdx, int colIdx)
    {
        if (!DebugTools.DevAssert(0 <= rowIdx && rowIdx < Rows && 0 <= colIdx && colIdx < Columns, null).Passed)
        {
            return null;
        }
        if (rowIdx != 0)
        {
            return _nameValuesRows[rowIdx - 1].GetCellAt(colIdx);
        }
        return _topRow.GetCellAt(colIdx);
    }

    public void UpdateComboBoxTable(IComboBoxInputControlDataAdapter data)
    {
        if (DebugTools.AssertNotNull(data, null).Failed || DebugTools.AssertNotNull(AllRows, null).Failed)
        {
            return;
        }

        IList<(string rowName, IList<string> rowValues)> dataRowsList = data.GroupByRows;
        int num = dataRowsList.Count + 1;
        int num2 = data.Units.Count + 1;
        if (DebugTools.DevAssert(num >= 2 && num2 >= 2, null).Failed)
        {
            return;
        }

        UpdateTableSize(num, num2);
        TableTitle = data.TableTitle;
        _topRow.UnitCells.EachWithIndex((int idx, ICellData cell) =>
        {
            cell.Text = data.Units[idx];
        });
        _nameValuesRows.EachWithIndex((int rowIndex, NameValuesRow row) =>
        {
            row.NameCell.Text = dataRowsList[rowIndex].rowName;
            row.ValueCells.EachWithIndex((int columnIndex, ICellData cell) =>
            {
                cell.Text = dataRowsList[rowIndex].rowValues[columnIndex];
            });
        });
        SelectedNameValuesRowIndex = data.SelectedValueRow;
        if (DebugTools.IsDebug)
        {
            DebugTools.DevAssert(IsValid(), null);
        }
    }

    private bool IsValid()
    {
        if (Rows >= 2 && Columns >= 2 && 0 <= SelectedTableRowIndex && SelectedTableRowIndex < Rows && AllRows.All((ComboBoxRow row) => row.CellCount == Columns))
        {
            return AreCellPositionsValid();
        }

        return false;
    }

    private bool AreCellPositionsValid()
    {
        int num = 0;
        foreach (ComboBoxRow allRow in AllRows)
        {
            if (!allRow.IsValid(num++))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateTableSize(int rows, int columns)
    {
        int num = Rows - rows;
        if (num > 0)
        {
            num.Times(_nameValuesRows.RemoveLast);
        }
        else if (num < 0)
        {
            for (int num2 = Rows; num2 < rows; num2++)
            {
                _nameValuesRows.Add(new NameValuesRow(num2, Columns));
            }
        }
        int num3 = Columns - columns;
        if (num3 > 0)
        {
            foreach (ComboBoxRow allRow in AllRows)
            {
                num3.Times(allRow.RemoveLastCell);
            }
        }
        else if (num3 < 0)
        {
            int num4 = 0;
            foreach (ComboBoxRow allRow2 in AllRows)
            {
                for (int num5 = Columns; num5 < columns; num5++)
                {
                    allRow2.AddCell(new CellData(string.Empty, num4, num5));
                }
                num4++;
            }
        }
        Columns = columns;
        DebugTools.DevAssert(Rows == rows && Columns == columns && _topRow.CellCount == columns, null);
    }

    public IEnumerable<ICellData> InsertRow(int rowIndex, Direction direction)
    {
        if (DebugTools.AssertNotNull(0 <= rowIndex && rowIndex < Rows, null).Failed)
        {
            return Enumerable.Empty<ICellData>();
        }

        if (DebugTools.DevAssert(direction != Direction.Up || rowIndex != 0, null).Failed)
        {
            return Enumerable.Empty<ICellData>();
        }

        DebugTools.DevAssert(direction == Direction.Up || direction == Direction.Down, null);
        int num = rowIndex + ((direction == Direction.Down) ? 1 : 0);
        NameValuesRow nameValuesRow = new NameValuesRow(num, Columns);
        _nameValuesRows.Insert(num - 1, nameValuesRow);
        UpdateCellsPositionForShiftedNameValuesRows(num, (ICellData cell) =>
        {
            cell.Row += 1;
        });
        return nameValuesRow.AllCells;
    }

    public bool CanRemoveRow(int rowIndex)
    {
        if (0 < rowIndex && rowIndex < Rows)
        {
            return Rows > 2;
        }

        return false;
    }

    public bool RemoveRow(int index)
    {
        if (DebugTools.DevAssert(CanRemoveRow(index), null).Failed)
        {
            return false;
        }

        int num = index - 1;
        _nameValuesRows.RemoveAt(num);
        UpdateCellsPositionForShiftedNameValuesRows(num, (ICellData cell) =>
        {
            cell.Row -= 1;
        });
        return true;
    }

    private void UpdateCellsPositionForShiftedNameValuesRows(int numOfUnchangedRows, Action<ICellData> updateCellPosition)
    {
        _nameValuesRows.Skip(numOfUnchangedRows).Each((NameValuesRow row) =>
        {
            row.AllCells.Each(updateCellPosition);
        });
    }

    public IEnumerable<ICellData> InsertColumn(int columnIndex, Direction direction)
    {
        if (DebugTools.DevAssert(0 <= columnIndex && columnIndex < Columns, null).Failed)
        {
            return Enumerable.Empty<ICellData>();
        }

        if (DebugTools.DevAssert(direction != Direction.Left || columnIndex != 0, null).Failed)
        {
            return Enumerable.Empty<ICellData>();
        }

        DebugTools.DevAssert(direction == Direction.Right || direction == Direction.Left, null);
        int num = columnIndex + ((direction == Direction.Right) ? 1 : 0);
        List<ICellData> list = new List<ICellData>(Rows);
        int num2 = 0;
        foreach (ComboBoxRow allRow in AllRows)
        {
            CellData cellData = new CellData(string.Empty, num2, num);
            list.Add(cellData);
            allRow.InsertCell(num, cellData);
            UpdateShiftedCellsColumnPosition(allRow.AllCells, num + 1, (ICellData cell) =>
            {
                cell.Column += 1;
            });
            num2++;
        }

        Columns++;
        return list;
    }

    public bool CanRemoveColumn(int columnIndex)
    {
        if (0 < columnIndex && columnIndex < Columns)
        {
            return Columns > 2;
        }

        return false;
    }

    public bool RemoveColumn(int index)
    {
        if (DebugTools.DevAssert(CanRemoveColumn(index), null).Failed)
        {
            return false;
        }

        foreach (ComboBoxRow allRow in AllRows)
        {
            allRow.RemoveCellAt(index);
            UpdateShiftedCellsColumnPosition(allRow.AllCells, index, (ICellData cell) =>
            {
                cell.Column -= 1;
            });
        }

        Columns--;
        return true;
    }

    private void UpdateShiftedCellsColumnPosition(IEnumerable<ICellData> rowCells, int numOfUnchangedColumns, System.Action<ICellData> updateCellColumnPosition)
    {
        EnumerableExtensions.Each(rowCells.Skip(numOfUnchangedColumns), updateCellColumnPosition);
    }

    public void ClearSelectedCellsContent(DiscreteRect range)
    {
        CellsInRange(range).Each((ICellData cell) =>
        {
            cell.Clear();
        });
    }

    public bool AreAllSelectedCellsEmpty(DiscreteRect range)
    {
        return CellsInRange(range).All((ICellData cell) => cell.IsEmpty());
    }

    public void UpdateCellsInRangeValues(DiscreteRect range, IList<string> viewStrings)
    {
        List<ICellData> list = CellsInRange(range).ToList();
        if (DebugTools.DevAssert(viewStrings != null && list.Count == viewStrings.Count, null).Failed)
        {
            return;
        }

        for (int num = 0; num < viewStrings.Count; num++)
        {
            list.ElementAt(num).Text = viewStrings.ElementAt(num);
        }
    }

    private IEnumerable<ICellData> CellsInRange(DiscreteRect range)
    {
        return TakeSubsequence(AllRows, range.Top, range.Bottom).SelectMany((ComboBoxRow row) => TakeSubsequence(row.AllCells, range.Left, range.Right));
    }

    public static IEnumerable<T> TakeSubsequence<T>(IEnumerable<T> sequence, int fromIdx, int toIdx)
    {
        if (!DebugTools.AssertNotNull(sequence, null).Failed && !DebugTools.DevAssert(0 <= fromIdx && fromIdx <= toIdx, null).Failed)
        {
            return sequence.Skip(fromIdx).Take(toIdx + 1 - fromIdx);
        }

        return Enumerable.Empty<T>();
    }

    public void ClearCellContent(IReadonlyCellData cellData)
    {
        if (DebugTools.AssertNotNull(cellData, null).Failed)
        {
            return;
        }

        ICellData cell = GetCellAt(cellData.Row, cellData.Column);
        if (DebugTools.AssertNotNull(cell, null).Passed)
        {
            cell.Clear();
        }
    }

    public void TrimWhitespacesInUnitCells()
    {
        _topRow.UnitCells.Each((ICellData unitCell) =>
        {
            unitCell.Text = unitCell.Text.Trim();
        });
    }

}
