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
}
