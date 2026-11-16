using System.Linq;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Worksheet;

public partial class PageBodyViewModel
{
    public PageBodyViewModel(Size? gridCellSize)
        : base(new WhiteboardModel(), false, gridCellSize)
    {
    }

    protected override IWhiteboardItem GetChildItemFromLocation(Point point)
    {
        if (WhiteboardItems == null || WhiteboardItems.Count == 0)
        {
            return null;
        }

        foreach (IWhiteboardItem item in WhiteboardItems.Reverse())
        {
            if (item is not IWhiteboardItemZombie && GetItemRect(item).ContainsAccountingForMeasurementError(point))
            {
                return item;
            }
        }

        return null;
    }
}
