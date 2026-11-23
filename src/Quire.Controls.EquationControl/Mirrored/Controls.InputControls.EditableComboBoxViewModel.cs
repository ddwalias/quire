#nullable disable
using Quire.Matrices;
using Quire.EquationEditor.View;
using System.Collections.Generic;
using Quire.Core;
using Quire.EquationEditor.Tree;
using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Internal;
using Quire.Wpf;
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

    public void CreateSelection(int cellIndexA, int cellIndexB)
    {
        // Release IL tests 0 <= min with bgt; min >= 0 compiles to blt.
        if (DebugTools.DevAssert(0 <= Math.Min(cellIndexA, cellIndexB) && Math.Max(cellIndexA, cellIndexB) < CellCount, null).Failed)
        {
            return;
        }

        SelectedCells = Tuple.Create(cellIndexA, cellIndexB);
    }

    public void TextBox_CanPaste(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is ComboBoxCellDataType cellDataType)
        {
            MathNode mathNode = TryReadMathNodeFromClipboard();
            if (mathNode != null)
            {
                e.Handled = true;
            }

            e.CanExecute = TransformMathNodeToComboBoxValues.CanTransform(mathNode, cellDataType);
            return;
        }

        DebugTools.FailAssertion("Unhandled case");
    }

    public Size PlaceHolderBoxSize
    {
        get
        {
            return _placeHolderBoxSize;
        }
        private set
        {
            if (_placeHolderBoxSize == value)
            {
                return;
            }
            _placeHolderBoxSize = value;
            OnPropertyChanged("PlaceHolderBoxSize");
        }
    }

    public double MidPointBetweenFirstAndSecondRow
    {
        get
        {
            return _midPointBetweenFirstAndSecondRow;
        }
        set
        {
            if (DebugTools.DevAssert(value.IsGreaterThanOrEqual(0.0, 1E-05), null).Failed)
            {
                return;
            }
            if (_midPointBetweenFirstAndSecondRow.IsEqualTo(value, 1E-05))
            {
                return;
            }
            _midPointBetweenFirstAndSecondRow = value;
            OnPropertyChanged("MidPointBetweenFirstAndSecondRow");
        }
    }

    public double MidPointBetweenFirstAndSecondColumn
    {
        get
        {
            return _midPointBetweenFirstAndSecondColumn;
        }
        set
        {
            if (DebugTools.DevAssert(value.IsGreaterThanOrEqual(0.0, 1E-05), null).Failed)
            {
                return;
            }
            if (_midPointBetweenFirstAndSecondColumn.IsEqualTo(value, 1E-05))
            {
                return;
            }
            _midPointBetweenFirstAndSecondColumn = value;
            OnPropertyChanged("MidPointBetweenFirstAndSecondColumn");
        }
    }

    public Size TableSize
    {
        get
        {
            return _tableSize;
        }
        set
        {
            if (_tableSize.IsEqualTo(value, 1E-05))
            {
                return;
            }
            _tableSize = value;
            OnPropertyChanged("TableSize");
        }
    }

    public EditableComboBoxViewModel(ComboBoxTableModel tableModel)
        : base(tableModel)
    {
        SetupCommandBindings();
    }

    public void UpdatePlaceHolderBoxSize(DpiScale dpiScale)
    {
        FormattedText formattedText = VisualPlaceholderNode.CreateFormattedCharPlaceholder(new Typeface(new FontFamily("Mathcad UniMath Prime"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), WpfUtils.PointsToDips(FontConstants.DefaultMathFontSizeInPoints), FontConstants.DefaultMathFontColor, dpiScale);
        PlaceHolderBoxSize = new Size(formattedText.Width / 2.0, formattedText.Height / 2.0);
    }

    internal static void ProcessConsecutiveIntervalsInReverseOrderAndNotify(int fromIndex, int toIndex, Func<int, bool> condition, Action<int, int> notify)
    {
        if (fromIndex > toIndex)
        {
            int num = fromIndex;
            fromIndex = toIndex;
            toIndex = num;
        }
        int? num2 = null;
        for (int num3 = toIndex; num3 >= fromIndex; num3--)
        {
            if (condition(num3))
            {
                if (!num2.HasValue)
                {
                    num2 = num3;
                }
            }
            else if (num2.HasValue)
            {
                notify(num3 + 1, num2.Value);
                num2 = null;
            }
        }
        if (num2.HasValue)
        {
            notify(fromIndex, num2.Value);
        }
    }

    public void RemoveOrClearSelectedCells(int cellIndexA, int cellIndexB, bool onlyClearSelectionContent)
    {
        ClearSelection();
        DiscreteRect tableRect = GetTableRect(cellIndexA, cellIndexB);
        bool flag = tableRect.Top <= 1 && tableRect.Bottom == Rows - 1;
        bool num = tableRect.Top == 0 && tableRect.Bottom == Rows - 1;
        bool flag2 = tableRect.Left == 0 && tableRect.Right == Columns - 1;
        onlyClearSelectionContent |= flag2 & flag;
        if (num && !onlyClearSelectionContent)
        {
            DeleteColumns(tableRect.Left, tableRect.Right, assertIfCannotDelete: false, isFinishedEditingMode: false);
        }
        else if (flag2 && !onlyClearSelectionContent)
        {
            DeleteRows(tableRect.Top, tableRect.Bottom, assertIfCannotDelete: false, isFinishedEditingMode: false);
        }
        else
        {
            TableModel.ClearSelectedCellsContent(tableRect);
        }
    }
}
