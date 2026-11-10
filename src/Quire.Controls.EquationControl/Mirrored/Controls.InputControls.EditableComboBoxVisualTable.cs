using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.EquationEditor.View;
using Quire.Matrices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Quire.Controls.InputControls;

public partial class EditableComboBoxVisualTable
{
    public IEditableComboBoxViewModel ViewModel => _viewModel;

    public ExtendedFont OperatorFormat
    {
        get;
        private set;
    }

    private EditableComboBoxTableCellsFontFormats CellsFontFormats
    {
        get;
        set;
    }

    private void CanDeleteSelection(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _viewModel.AreThereSelectedCells;
    }

    private int GetRowFromTextBoxIndex(int idx)
    {
        return MatrixHelper.RowOf(idx, Columns);
    }

    private int GetColumnFromTextBoxIndex(int idx)
    {
        return MatrixHelper.ColumnOf(idx, Columns);
    }

    private System.Windows.UIElement GetChildElemAtRowColumn(int row, int column)
    {
        return GetChildAt(MatrixHelper.IndexOf(row, column, Columns));
    }

    private void FireContentChanged()
    {
        ContentChanged?.Invoke();
    }

    private void CanTabNavigate(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = IsKeyboardFocusWithin && !IsKeyboardFocused;
    }

    private void ForwardTabNavigate(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MoveFocusedElement(FocusNavigationDirection.Next, GetCellDataFromTextBox(e.Source, out _));
        e.Handled = true;
    }

    private void BackwardTabNavigate(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MoveFocusedElement(FocusNavigationDirection.Previous, GetCellDataFromTextBox(e.Source, out _));
        e.Handled = true;
    }

    public bool IsActive => IsVisible;

    public System.Tuple<int, int> SelectedCells => _viewModel.SelectedCells;

    private void UpdateRowsColumnsIndex()
    {
        _rows = _viewModel.Rows;
        Columns = _viewModel.Columns;
    }

    private bool IsCellCountValid()
    {
        return Rows == _viewModel.Rows && Columns == _viewModel.Columns && Children.Count == Rows * Columns;
    }

    private System.Windows.UIElement GetChildAt(int index)
    {
        if (!DebugTools.DevAssert(0 <= index && index < Children.Count, null).Passed)
        {
            return null;
        }

        return Children[index];
    }

    protected override void OnDpiChanged(System.Windows.DpiScale oldDpi, System.Windows.DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        _viewModel.UpdatePlaceHolderBoxSize(newDpi);
    }

    private static bool IsTextBoxDataContextIsCellData(object element, out Quire.EquationEditor.View.IReadonlyCellData cellData, out System.Windows.Controls.TextBox textBox)
    {
        cellData = GetCellDataFromTextBox(element, out textBox);
        return cellData != null;
    }

    private Quire.EquationEditor.View.IReadonlyCellData GetCellDataAtRowColumnFromVisualTableChildren(int row, int column)
    {
        return GetCellDataFromTextBox(GetChildElemAtRowColumn(row, column), out _);
    }
}
