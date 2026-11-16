using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class WorksheetItemPositionComparer
{
    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    public int Compare(IWhiteboardItem itemOne, IWhiteboardItem itemTwo)
    {
        if (itemOne == itemTwo)
        {
            return 0;
        }
        Point itemWorksheetLocation = ItemManager.GetItemWorksheetLocation(itemOne);
        Point itemWorksheetLocation2 = ItemManager.GetItemWorksheetLocation(itemTwo);
        return Compare(itemWorksheetLocation, itemWorksheetLocation2);
    }

    public WorksheetItemPositionComparer() { }
}
