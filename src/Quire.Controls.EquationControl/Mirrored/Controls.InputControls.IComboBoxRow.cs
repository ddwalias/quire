namespace Quire.Controls.InputControls;

public partial interface IComboBoxRow
{
    int CellCount { get; }

    bool IsValid(int rowIndex);
}
