#nullable disable
#pragma warning disable
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Whiteboard;
using Quire.Services;

namespace Quire.Controls.ExcelComponent;

public partial class OleBaseComponentItemManager : WhiteboardItemManager, IOleBaseComponentItemManager, IWhiteboardItemManager, IContainerItemManager, IFrameworkElementFacade, IUIElementFacade, IService, IUIElementArrangeInfoProvider
{
    public Point GetItemOleComponentLocation(IWhiteboardItem item)
    {
        return GetItemLocation(item, OleBaseComponentControl.GetOleComponentLocationLeft, OleBaseComponentControl.GetOleComponentLocationTop);
    }

    public void SetItemOleComponentLocation(IWhiteboardItem item, Point location)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (dependencyObject != null)
        {
            dependencyObject.SetValue(OleBaseComponentControl.OleComponentLocationLeftProperty, location.X);
            dependencyObject.SetValue(OleBaseComponentControl.OleComponentLocationTopProperty, location.Y);
        }
    }

    public override Rect GetItemRect(IWhiteboardItem item)
    {
        Point itemOleComponentLocation = GetItemOleComponentLocation(item);
        itemOleComponentLocation.Offset(0.0, item.VerticalAlignmentOffset);
        return new Rect(itemOleComponentLocation, GetRenderSize(item));
    }

    public override IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending)
    {
        IEnumerable<IWhiteboardItem> enumerable = items.OrderBy(item => GetItemOleComponentLocation(item).Y).ThenBy(item => GetItemOleComponentLocation(item).X);
        if (descending)
        {
            enumerable = enumerable.Reverse();
        }
        return enumerable;
    }

    public OleBaseComponentItemManager()
    {
    }
}
