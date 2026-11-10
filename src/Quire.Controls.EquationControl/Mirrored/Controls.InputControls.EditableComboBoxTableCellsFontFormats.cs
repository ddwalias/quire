namespace Quire.Controls.InputControls;

public partial class EditableComboBoxTableCellsFontFormats
{

    public System.Windows.Media.Brush GetCellForeground(bool isUnitCell)
    {
        if (!isUnitCell)
        {
            return _normalCellForeground;
        }

        return _unitCellForeground;
    }
}
