#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class OleBaseItemPositionComparer
{
    public int Compare(IWhiteboardItem itemOne, IWhiteboardItem itemTwo)
    {
        Point point = OleBaseComponentModel.OleComponentItemMgr.GetItemOleComponentLocation(itemOne);
        Point point2 = OleBaseComponentModel.OleComponentItemMgr.GetItemOleComponentLocation(itemTwo);
        return Compare(point, point2);
    }

    public OleBaseItemPositionComparer() { }
}
