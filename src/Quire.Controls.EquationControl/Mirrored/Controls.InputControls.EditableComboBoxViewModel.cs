using Quire.Matrices;
using Quire.EquationEditor.View;
using System.Collections.Generic;
using Quire.Core;
using Quire.EquationEditor.Tree;
namespace Quire.Controls.InputControls;

public partial class EditableComboBoxViewModel
{

    public void ClearSelection()
    {
        SelectedCells = null;
    }

    public Quire.EquationEditor.View.IReadonlyCellData GetCellAt(int row, int column)
    {
        return ((IComboBoxTableModel)TableModel).GetCellAt(row, column);
    }

    public bool CanDeleteRow(int rowIndex)
    {
        return TableModel.CanRemoveRow(rowIndex);
    }

    public bool CanDeleteColumn(int columnIndex)
    {
        return TableModel.CanRemoveColumn(columnIndex);
    }

    public Quire.DiscreteRect GetTableRect(int cellIndexA, int cellIndexB)
    {
        return MatrixHelper.RectFromCornerIndices(cellIndexA, cellIndexB, Columns);
    }

    public static bool IsUnitCell(int row, int column)
    {
        return (row == 0) & (column > 0);
    }

    public static bool IsValueCell(int row, int column)
    {
        return (row > 0) & (column > 0);
    }

    public static bool IsTitleCell(int row, int column)
    {
        return (row >= 0) & (column == 0);
    }

    public bool AreAllSelectedCellsEmpty(int cellIndexA, int cellIndexB)
    {
        return TableModel.AreAllSelectedCellsEmpty(GetTableRect(cellIndexA, cellIndexB));
    }

    public void ClearCellContent(Quire.EquationEditor.View.IReadonlyCellData cellData)
    {
        TableModel.ClearCellContent(cellData);
    }

    public void FinishEditing()
    {
        TableModel.TrimWhitespacesInUnitCells();
        DeleteEmptyRows();
        DeleteEmptyColumns();
    }

    public int Rows => TableModel.Rows;

    public int Columns => TableModel.Columns;

    public int CellCount => Rows * Columns;

    public int SelectedTableRowIndex
    {
        get => TableModel.SelectedTableRowIndex;
        set => TableModel.SelectedTableRowIndex = value;
    }

    public bool AreThereSelectedCells => SelectedCells != null;

    public System.Tuple<int, int> SelectedCells
    {
        get => _selectedCells;
        private set
        {
            _selectedCells = value;
            OnPropertyChanged(nameof(SelectedCells));
        }
    }

    public bool InsertRow(Quire.EquationEditor.View.IReadonlyCellData cellData, Quire.Direction direction)
    {
        IEnumerable<ICellData> addedCells = TableModel.InsertRow(cellData.Row, direction);
        bool isNotEmpty = addedCells.IsNotEmpty();
        if (isNotEmpty)
        {
            CellsAdded?.Invoke(addedCells);
        }

        return isNotEmpty;
    }

    public bool InsertColumn(Quire.EquationEditor.View.IReadonlyCellData cellData, Quire.Direction direction)
    {
        IEnumerable<ICellData> addedCells = TableModel.InsertColumn(cellData.Column, direction);
        bool isNotEmpty = addedCells.IsNotEmpty();
        if (isNotEmpty)
        {
            CellsAdded?.Invoke(addedCells);
        }

        return isNotEmpty;
    }

    private static bool AreOnlyValuesSelected(Quire.DiscreteRect selectedRect)
    {
        if (selectedRect.Top >= 1)
        {
            return selectedRect.Left >= 1;
        }

        return false;
    }

    private void PasteEditModeBySelection()
    {
        if (!AreThereSelectedCells)
        {
            return;
        }

        PasteEditMode(GetTableRect(SelectedCells.Item1, SelectedCells.Item2));
    }

    private bool CanPasteEditMode()
    {
        MathNode mathNode = TryReadMathNodeFromClipboard();
        if (mathNode?.DescriptorName == "MATRIX")
        {
            return TransformMathNodeToComboBoxValues.CanTransform(mathNode, default);
        }

        return false;
    }
}
