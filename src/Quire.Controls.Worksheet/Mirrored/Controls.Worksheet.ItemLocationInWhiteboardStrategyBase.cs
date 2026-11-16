using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Services;

namespace Quire.Controls.Worksheet;

public abstract partial class ItemLocationInWhiteboardStrategyBase : IItemLocationStrategy
{
    private readonly Size _pageBodySize;

    protected readonly IWhiteboardItemManager WhiteboardItemManager;

    protected readonly IWorksheetPageManager WorksheetPageManager;

    public Size GetPageBodySize => _pageBodySize;

    protected ItemLocationInWhiteboardStrategyBase(IWorksheetPageManager worksheetPageManager)
    {
        WhiteboardItemManager = AppServiceRegistry.GetService<IWhiteboardItemManager>();
        WorksheetPageManager = worksheetPageManager;
        _pageBodySize = new Size(worksheetPageManager.PageLayout.PageBodyWidth, double.PositiveInfinity);
    }

    public virtual Point GetItemRectLocationInWhiteboard(IWhiteboardItem whiteboardItem)
    {
        Rect itemRect = WhiteboardItemManager.GetItemRect(whiteboardItem);
        return itemRect.Location;
    }

    public abstract int GetPageIndex(Point topLeft);

    public abstract Rect? GetItemClipping(IWhiteboardItem whiteboardItem, Rect contentBounds, out Rect? stickingPartRect);

    public void PrePrintUpdateWorksheetLayout(IWorksheetControl worksheet, IEnumerable<IWhiteboardItem> items)
    {
    }

    public void PostPrintUpdateWorksheetLayout(IWorksheetControl worksheet)
    {
    }
}
