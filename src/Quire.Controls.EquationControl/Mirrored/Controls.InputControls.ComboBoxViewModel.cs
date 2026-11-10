namespace Quire.Controls.InputControls;

public partial class ComboBoxViewModel
{

    public Quire.Controls.InputControls.EditableComboBoxViewModel CreatedEditableViewModel()
    {
        return new EditableComboBoxViewModel(TableModel);
    }
}
