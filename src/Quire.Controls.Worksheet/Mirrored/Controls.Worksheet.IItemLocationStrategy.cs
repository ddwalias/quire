using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IItemLocationStrategy
{
    Size GetPageBodySize { get; }

    Point GetItemRectLocationInWhiteboard(IWhiteboardItem whiteboardItem);

    int GetPageIndex(Point topLeft);

    void PrePrintUpdateWorksheetLayout(IWorksheetControl worksheet, IEnumerable<IWhiteboardItem> items);

    void PostPrintUpdateWorksheetLayout(IWorksheetControl worksheet);

    Rect? GetItemClipping(IWhiteboardItem whiteboardItem, Rect contentBounds, out Rect? stickingPartRect);
}
