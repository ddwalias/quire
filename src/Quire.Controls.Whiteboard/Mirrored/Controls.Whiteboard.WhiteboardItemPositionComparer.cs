#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardItemPositionComparer
{
    public WhiteboardItemPositionComparer() { }

    public virtual int Compare(IWhiteboardItem itemOne, IWhiteboardItem itemTwo)
    {
        if (itemOne == itemTwo)
        {
            return 0;
        }

        Point itemLocation = WhiteboardViewModel.WhiteboardItemMgr.GetItemLocation(itemOne);
        Point itemLocation2 = WhiteboardViewModel.WhiteboardItemMgr.GetItemLocation(itemTwo);
        return Compare(itemLocation, itemLocation2);
    }
}
