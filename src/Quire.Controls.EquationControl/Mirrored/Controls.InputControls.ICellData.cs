namespace Quire.Controls.InputControls;

public partial interface ICellData
{
    new string Text { get; set; }
    new int Row { get; set; }
    new int Column { get; set; }
    void Clear();
}
