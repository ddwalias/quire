namespace Quire.Controls.InputControls;

public partial class ComboBoxControlWidget
{

    private void OnEditableComboBoxContentChanged()
    {
        _comboBox.OnEditableComboBoxContentChanged();
    }

    public void Refresh(Quire.Controls.InputControls.EditableComboBoxTableCellsFontFormats cellsFontFormats, Quire.Controls.Core.ExtendedFont operatorFormat)
    {
        _editableComboBoxTableBorder.Refresh(cellsFontFormats, operatorFormat);
    }
}
