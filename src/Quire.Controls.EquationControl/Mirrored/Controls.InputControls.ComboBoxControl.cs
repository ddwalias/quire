using System.Windows;
namespace Quire.Controls.InputControls;

public partial class ComboBoxControl
{

    private void OnCloseComboBoxDropDown(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        IsDropDownOpen = false;
    }

    public Quire.Controls.InputControls.EditableComboBoxViewModel CreatedEditableViewModel()
    {
        return _viewModel.CreatedEditableViewModel();
    }

    protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
    {
        e.Handled = true;
        base.OnSelectionChanged(e);
    }

    public void OnEditableComboBoxContentChanged()
    {
        RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
        if (!IsEditModeActive)
        {
            RefreshItems(false);
        }
    }

    public Quire.Controls.InputControls.ComboBoxViewModel ViewModel => _viewModel;

    public Quire.EquationEditor.View.IEditableComboBoxVisualTable EditableComboBoxVisualTable => _comboBoxWidget.EditableComboBoxVisualTable;
}
