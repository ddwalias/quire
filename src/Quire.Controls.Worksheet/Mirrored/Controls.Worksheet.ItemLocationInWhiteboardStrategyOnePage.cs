using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class ItemLocationInWhiteboardStrategyOnePage
{
    public ItemLocationInWhiteboardStrategyOnePage(IWorksheetPageManager worksheetPageManager)
        : base(worksheetPageManager)
    {
    }

    public override int GetPageIndex(Point topLeft)
    {
        return 0;
    }

    public override Rect? GetItemClipping(IWhiteboardItem whiteboardItem, Rect contentBounds, out Rect? stickingPartRect)
    {
        stickingPartRect = null;
        return null;
    }
}
