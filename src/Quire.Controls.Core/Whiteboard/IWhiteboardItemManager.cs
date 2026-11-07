#nullable disable
using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Core.Whiteboard;

public partial interface IWhiteboardItemManager
{
    Point GetItemLocation(IWhiteboardItem item);
    void SetItemLocation(IWhiteboardItem item, Point point);
    bool ShouldItemBeDeletedIfEmpty(IWhiteboardItem item);
    void SetShouldItemBeDeletedIfEmpty(IWhiteboardItem item, bool value);
    void ClearShouldItemBeDeletedIfEmpty(IWhiteboardItem item);
    bool IsResizingItem(IWhiteboardItem item);
    bool GetIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item);
    bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items);
    Rect GetItemRect(IWhiteboardItem item);
    double GetItemTop(IWhiteboardItem item);
    double GetItemBottom(IWhiteboardItem item);
    IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending);
    bool GetItemIsInPasting(IWhiteboardItem item);
    void SetItemIsInPasting(IWhiteboardItem item, bool value);
    void ClearItemIsInPasting(IWhiteboardItem item);
    bool GetItemIsInPrinting(IWhiteboardItem item);
    void SetItemIsInPrinting(IWhiteboardItem item, bool value);
    void ClearItemIsInPrinting(IWhiteboardItem item);
}
