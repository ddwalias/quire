#nullable disable
using Quire.Matrices;
using Quire.EquationEditor.View;
using System.Collections.Generic;
using Quire.Core;
using Quire.EquationEditor.Tree;
using System;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Linq;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Internal;
using Quire.Wpf;
namespace Quire.Controls.InputControls;

public partial class EditableComboBoxViewModel
{

    public EditableComboBoxViewModel(ComboBoxTableModel tableModel)
        : base(tableModel)
    {
        SetupCommandBindings();
    }

    public IEnumerable<ICellData> AllCells => TableModel.AllRows.SelectMany((ComboBoxRow row) => row.AllCells);

    public int Rows => TableModel.Rows;

    public int Columns => TableModel.Columns;

    public int CellCount => Rows * Columns;

    public int SelectedTableRowIndex
    {
        get => TableModel.SelectedTableRowIndex;
        set => TableModel.SelectedTableRowIndex = value;
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

    public System.Tuple<int, int> SelectedCells
    {
        get => _selectedCells;
        private set
        {
            _selectedCells = value;
            OnPropertyChanged(nameof(SelectedCells));
        }
    }

    public bool AreThereSelectedCells => SelectedCells != null;

    public void ClearSelection()
    {
        SelectedCells = null;
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

    private List<ICellData> ExpandComboBoxRowsColumns(int targetRectBottom, int targetRectRight)
    {
        IReadonlyCellData cellAt = GetCellAt(Rows - 1, Columns - 1);
        List<ICellData> list = new List<ICellData>();
        if (cellAt == null)
        {
            return list;
        }

        while (Columns <= targetRectRight)
        {
            list.AddRange(TableModel.InsertColumn(cellAt.Column, Direction.Right));
        }

        while (Rows <= targetRectBottom)
        {
            list.AddRange(TableModel.InsertRow(cellAt.Row, Direction.Down));
        }

        return list;
    }

    private DiscreteRect? PasteSelectedValues(DiscreteRect selectedRect, MathNode matrix)
    {
        int rows = ((MatrixNodeInfo)matrix.NodeInfo).FullSize.Rows;
        int columns = ((MatrixNodeInfo)matrix.NodeInfo).FullSize.Columns;
        int bottom = selectedRect.Top + rows - 1;
        int right = selectedRect.Left + columns - 1;
        List<ICellData> addedCells = ExpandComboBoxRowsColumns(bottom, right);
        if (DebugTools.DevAssert(bottom < Rows && right < Columns, null).Failed)
        {
            return null;
        }

        DiscreteRect discreteRect = new DiscreteRect(selectedRect.Top, selectedRect.Left, bottom, right);
        IList<string> viewStrings = TransformMathNodeToComboBoxValues.TransformToViewStrings(matrix);
        TableModel.UpdateCellsInRangeValues(discreteRect, viewStrings);
        if (addedCells.IsNotEmpty())
        {
            CellsAdded?.Invoke(addedCells);
        }

        return discreteRect;
    }

    private MathNode TryReadMathNodeFromClipboard()
    {
        string sExpression = WpfClipboard.Current.GetSExpression();
        if (sExpression == null || !sExpression.IsNotEmpty())
        {
            return null;
        }

        MathNode result = null;
        try
        {
            result = TransformMathNodeToComboBoxValues.SExpressionToMathNode(sExpression);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }

        return result;
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

    private static bool AreOnlyValuesSelected(Quire.DiscreteRect selectedRect)
    {
        if (selectedRect.Top >= 1)
        {
            return selectedRect.Left >= 1;
        }

        return false;
    }

    private void PasteEditMode(DiscreteRect sourceRect)
    {
        if (!AreOnlyValuesSelected(sourceRect))
        {
            return;
        }

        MathNode matrix = TryReadMathNodeFromClipboard();
        if (PasteSelectedValues(sourceRect, matrix) is DiscreteRect pastedRect)
        {
            int cellIndexA = MatrixHelper.IndexOf(pastedRect.Top, pastedRect.Left, Columns);
            int cellIndexB = MatrixHelper.IndexOf(pastedRect.Bottom, pastedRect.Right, Columns);
            CreateSelection(cellIndexA, cellIndexB);
        }
    }

    private void PasteEditModeBySelection()
    {
        if (!AreThereSelectedCells)
        {
            return;
        }

        PasteEditMode(GetTableRect(SelectedCells.Item1, SelectedCells.Item2));
    }

    private void CopyEditMode()
    {
        DiscreteRect tableRect = GetTableRect(SelectedCells.Item1, SelectedCells.Item2);
        if (!AreOnlyValuesSelected(tableRect))
        {
            return;
        }
        IComboBoxInputControlDataAdapter comboBoxInputControlDataAdapter = CreateDataAdapter();
        if (comboBoxInputControlDataAdapter != null)
        {
            ComboBoxInputControlData comboBoxInputControlData = new ComboBoxInputControlData(comboBoxInputControlDataAdapter);
            string matrixSExprFromSelection = comboBoxInputControlData.GetMatrixSExprFromSelection(tableRect);
            if (DebugTools.DevAssert(matrixSExprFromSelection.IsNotEmpty(), null).Passed)
            {
                WpfClipboard.Current.SetSExpression(matrixSExprFromSelection);
            }
        }
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

    public void TextBox_OnPaste(object sender, ComboBoxCellDataType cellDataType)
    {
        if (sender is ICellData cellData)
        {
            MathNode mathNode = TryReadMathNodeFromClipboard();
            int cellIndex = MatrixHelper.IndexOf(cellData.Row, cellData.Column, Columns);
            if (mathNode != null && mathNode.DescriptorName == "MATRIX")
            {
                PasteEditMode(GetTableRect(cellIndex, cellIndex));
                return;
            }

            cellData.Text = mathNode.TransformToViewStringByType(cellDataType);
            CreateSelection(cellIndex, cellIndex);
        }
        else
        {
            DebugTools.FailAssertion("Unhandled case");
        }
    }

    private void SetupCommandBindings()
    {
        CreateCommandBinding(ApplicationCommands.Copy, (object param) =>
        {
            CopyEditMode();
        }, (object param) => AreThereSelectedCells);
        CreateCommandBinding(ApplicationCommands.Paste, (object param) =>
        {
            PasteEditModeBySelection();
        }, (object param) => CanPasteEditMode());
    }

    public event Action<IEnumerable<ICellData>> CellsAdded;

    public event Action<int, int, bool> RowsDeleted;

    public event Action<int, int, bool> ColumnsDeleted;

    public void UpdatePlaceHolderBoxSize(DpiScale dpiScale)
    {
        FormattedText formattedText = VisualPlaceholderNode.CreateFormattedCharPlaceholder(new Typeface(new FontFamily("Mathcad UniMath Prime"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), WpfUtils.PointsToDips(FontConstants.DefaultMathFontSizeInPoints), FontConstants.DefaultMathFontColor, dpiScale);
        PlaceHolderBoxSize = new Size(formattedText.Width / 2.0, formattedText.Height / 2.0);
    }

    public Quire.EquationEditor.View.IReadonlyCellData GetCellAt(int row, int column)
    {
        return ((IComboBoxTableModel)TableModel).GetCellAt(row, column);
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

    public bool CanDeleteRow(int rowIndex)
    {
        return TableModel.CanRemoveRow(rowIndex);
    }

    public void DeleteRows(int fromIndex, int toIndex, bool assertIfCannotDelete, bool isFinishedEditingMode)
    {
        ClearSelection();
        ProcessConsecutiveIntervalsInReverseOrderAndNotify(fromIndex, toIndex, (int rowIdx) => (assertIfCannotDelete || CanDeleteRow(rowIdx)) && TableModel.RemoveRow(rowIdx), (int startIdx, int endIdx) =>
        {
            RowsDeleted?.Invoke(startIdx, endIdx, isFinishedEditingMode);
        });
    }

    public bool CanDeleteColumn(int columnIndex)
    {
        return TableModel.CanRemoveColumn(columnIndex);
    }

    public void DeleteColumns(int fromIndex, int toIndex, bool assertIfCannotDelete, bool isFinishedEditingMode)
    {
        ClearSelection();
        ProcessConsecutiveIntervalsInReverseOrderAndNotify(fromIndex, toIndex, (int colIdx) => (assertIfCannotDelete || CanDeleteColumn(colIdx)) && TableModel.RemoveColumn(colIdx), (int startIdx, int endIdx) =>
        {
            ColumnsDeleted?.Invoke(startIdx, endIdx, isFinishedEditingMode);
        });
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

    private void DeleteEmptyRows()
    {
        List<ComboBoxRow> rowsList = TableModel.AllRows.ToList();
        ProcessConsecutiveIntervalsInReverseOrderAndNotify(0, Rows - 1, (int rowIndex) => rowsList[rowIndex].AllCells.All((ICellData cell) => cell.IsEmptyOrWhitespace()), (int startIndex, int endIndex) =>
        {
            DeleteRows(startIndex, endIndex, assertIfCannotDelete: false, isFinishedEditingMode: true);
        });
    }

    private void DeleteEmptyColumns()
    {
        ProcessConsecutiveIntervalsInReverseOrderAndNotify(0, Columns - 1, (int colIndex) => TableModel.GetColumnCells(colIndex).All((IReadonlyCellData cell) => cell.IsEmptyOrWhitespace()), (int startIndex, int endIndex) =>
        {
            DeleteColumns(startIndex, endIndex, assertIfCannotDelete: false, isFinishedEditingMode: true);
        });
    }

}
