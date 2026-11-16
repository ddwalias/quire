using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class AreaItemLocationInWorksheetStrategy : ItemLocationInWhiteboardStrategyBase
{
    private readonly IWorksheetItemManager _worksheetItemManager;

    private readonly Point _areaRectLocation;

    public AreaItemLocationInWorksheetStrategy(IWorksheetPageManager worksheetPageManager, IWorksheetItemManager worksheetItemManager, Point areaRectLocation)
        : base(worksheetPageManager)
    {
        _areaRectLocation = areaRectLocation;
        _worksheetItemManager = worksheetItemManager;
    }

    public override Point GetItemRectLocationInWhiteboard(IWhiteboardItem whiteboardItem)
    {
        Point itemRectLocationInWhiteboard = base.GetItemRectLocationInWhiteboard(whiteboardItem);
        itemRectLocationInWhiteboard.Offset(_areaRectLocation.X, _areaRectLocation.Y);
        return itemRectLocationInWhiteboard;
    }

    public override int GetPageIndex(Point topLeft)
    {
        return WorksheetPageManager.GetPageIndex(topLeft.Y, false);
    }

    public override Rect? GetItemClipping(IWhiteboardItem whiteboardItem, Rect contentBounds, out Rect? stickingPartRect)
    {
        return ItemLocationWorksheetStrategyHelper.GetItemContentClipping(whiteboardItem, contentBounds, CountItemParts(whiteboardItem), out stickingPartRect, GetItemClippingInternal);
    }

    private int CountItemParts(IWhiteboardItem whiteboardItem)
    {
        Point itemRectLocationInWhiteboard = GetItemRectLocationInWhiteboard(whiteboardItem);
        FrameworkElement frameworkElement = whiteboardItem as FrameworkElement;
        Point val = itemRectLocationInWhiteboard;
        Point topLeft = itemRectLocationInWhiteboard;
        if (DebugTools.AssertNotNull(frameworkElement, null).Passed)
        {
            Rect descendantBounds = _worksheetItemManager.GetDescendantBounds(frameworkElement);
            Point location = descendantBounds.Location;
            val.Offset(location.X, location.Y);
            topLeft = val + new Vector(descendantBounds.Width, descendantBounds.Height);
        }

        return GetPageIndex(topLeft) - GetPageIndex(val) + 1;
    }

    private Rect? GetItemClippingInternal(IWhiteboardItem whiteboardItem, int partIndex)
    {
        Point itemRectLocationInWhiteboard = GetItemRectLocationInWhiteboard(whiteboardItem);
        FrameworkElement frameworkElement = whiteboardItem as FrameworkElement;
        Point topLeft = itemRectLocationInWhiteboard;
        if (DebugTools.AssertNotNull(frameworkElement, null).Passed)
        {
            Point location = _worksheetItemManager.GetDescendantBounds(frameworkElement).Location;
            topLeft.Offset(location.X, location.Y);
        }

        int pageIndex = GetPageIndex(topLeft);
        return CalculateVirtualClipping(itemRectLocationInWhiteboard, pageIndex + partIndex);
    }

    private Rect CalculateVirtualClipping(Point location, int pageIndex)
    {
        return new Rect(-location.X, WorksheetPageManager.PageLayout.PageBodyHeight * (double)pageIndex - location.Y, double.PositiveInfinity, WorksheetPageManager.PageLayout.PageBodyHeight);
    }
}
