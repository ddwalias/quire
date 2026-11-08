#nullable disable
namespace Quire.Controls;

public partial interface IDimensionSelectorView
{
    object CommandParameter { get; set; }
    string RowDisplayText { get; set; }
    string ColumnDisplayText { get; set; }
    event MouseOverCellHandler MouseOverCell;
    event Delegate0 Commit;
    void SetSelection(params int[] childIndex);
}
