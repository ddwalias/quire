#nullable disable
#pragma warning disable
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent;

public partial interface IOleBaseComponentItemManager
{
    Point GetItemOleComponentLocation(IWhiteboardItem item);
    void SetItemOleComponentLocation(IWhiteboardItem item, Point location);
}
