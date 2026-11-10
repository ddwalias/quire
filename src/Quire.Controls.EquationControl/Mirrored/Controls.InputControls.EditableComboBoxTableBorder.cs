namespace Quire.Controls.InputControls;

public partial class EditableComboBoxTableBorder
{

    public void Refresh(Quire.Controls.InputControls.EditableComboBoxTableCellsFontFormats cellsFontFormats, Quire.Controls.Core.ExtendedFont operatorFormat)
    {
        _editableComboBoxView.Refresh(cellsFontFormats, operatorFormat);
    }

    public bool IsEditModeActive => _editableComboBoxView.IsActive;

    public Quire.EquationEditor.View.IEditableComboBoxVisualTable EditableTable => _editableComboBoxView;
}
