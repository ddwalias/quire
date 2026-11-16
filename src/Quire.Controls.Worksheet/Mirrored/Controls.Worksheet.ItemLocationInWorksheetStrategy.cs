using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Services;
using Quire.Wpf.Printing;

namespace Quire.Controls.Worksheet;

public partial class ItemLocationInWorksheetStrategy : IItemLocationStrategy
{
    private readonly Size _pageBodySize;

    private readonly IWorksheetItemManager _worksheetItemManager;

    private readonly IWorksheetPageManager _worksheetPageManager;

    public ItemLocationInWorksheetStrategy(IWorksheetPageManager worksheetPageManager)
    {
        _worksheetItemManager = AppServiceRegistry.GetService<IWorksheetItemManager>();
        _worksheetPageManager = worksheetPageManager;
        _pageBodySize = new Size(_worksheetPageManager.PageLayout.PageBodyWidth, _worksheetPageManager.PageLayout.PageBodyHeight);
    }

    public Point GetItemRectLocationInWhiteboard(IWhiteboardItem whiteboardItem)
    {
        Rect itemRect = _worksheetItemManager.GetItemRect(whiteboardItem);
        return itemRect.Location;
    }

    public Rect? GetItemClipping(IWhiteboardItem whiteboardItem, Rect contentBounds, out Rect? stickingPartRect)
    {
        int count = _worksheetPageManager.GetWorksheetItemParts(whiteboardItem).Count;
        return ItemLocationWorksheetStrategyHelper.GetItemContentClipping(whiteboardItem, contentBounds, count, out stickingPartRect, GetItemClippingInternal);
    }

    private Rect? GetItemClippingInternal(IWhiteboardItem whiteboardItem, int partIndex)
    {
        return _worksheetPageManager.GetClippedItemPartRect(whiteboardItem, partIndex, _worksheetItemManager);
    }

    public virtual int GetPageIndex(Point topLeft)
    {
        return _worksheetPageManager.GetPageIndex(topLeft.Y, false);
    }

    public void PrePrintUpdateWorksheetLayout(IWorksheetControl worksheet, IEnumerable<IWhiteboardItem> items)
    {
        IList<double> source = items.Select((IWhiteboardItem item) =>
        {
            Point itemRectLocationInWhiteboard = GetItemRectLocationInWhiteboard(item);
            return itemRectLocationInWhiteboard.Y;
        }).ToList();
        worksheet.PrePrintUpdateWorksheetLayout(new PageIndexRange(_worksheetPageManager.GetPageIndex(source.Min(), false), _worksheetPageManager.GetPageIndex(source.Max(), false)));
    }

    public void PostPrintUpdateWorksheetLayout(IWorksheetControl worksheet)
    {
        worksheet.PostPrintUpdateWorksheetLayout();
    }

    public Size GetPageBodySize => _pageBodySize;
}
