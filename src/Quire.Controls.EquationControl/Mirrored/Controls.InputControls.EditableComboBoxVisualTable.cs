using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.EquationEditor.View;
using Quire.Matrices;
using System.Windows;
using System.Windows.Input;

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
}
