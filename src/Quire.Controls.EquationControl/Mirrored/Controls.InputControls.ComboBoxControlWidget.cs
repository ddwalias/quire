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

    public bool IsEditModeActive => _editableComboBoxTableBorder.IsEditModeActive;

    public Quire.EquationEditor.View.IEditableComboBoxVisualTable EditableComboBoxVisualTable => _editableComboBoxTableBorder.EditableTable;
}
