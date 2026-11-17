using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Worksheet.Printing;
using Quire.Services;

namespace Quire.Controls.Worksheet;

public static partial class PrintTreeXpsDecorator
{
    private static IWhiteboardItemManager ItemManager
    {
        get
        {
            if (_whiteboardItemMgr == null || !_whiteboardItemMgr.IsConnected)
            {
                return _whiteboardItemMgr = AppServiceRegistry.GetService<IWhiteboardItemManager>();
            }
            return _whiteboardItemMgr;
        }
    }

    public static bool CanEnrichPrintingTree { get; set; }

    public static void EnrichPrintingTree(IEnumerable<IPageSnapshot> pages, bool shouldEnrich)
    {
        if (!shouldEnrich || !CanEnrichPrintingTree)
        {
            return;
        }
        DebugTools.LogData("PrintTreeXpsDecorator.EnrichPrintingTree: Enter");
        foreach (IPageSnapshot page in pages)
        {
            EnrichPagePart(page.PageBody, "PageBody");
            EnrichPagePart(page.Header, "Header");
            EnrichPagePart(page.Footer, "Footer");
        }
    }

    private static Rect GetItemRectForPrint(IWhiteboardItem clone, UIElement parent)
    {
        Rect result = Rect.Empty;
        if (DebugTools.AssertNotNull(parent, null).Failed)
        {
            return result;
        }
        if (parent is IWhiteboardControl)
        {
            result = ItemManager.GetItemRect(clone);
        }
        else if (parent is ICalculatableContainer && clone is UIElement)
        {
            UIElement uIElement = (UIElement)clone;
            Point val = uIElement.TransformToAncestor(parent).Transform(new Point(0.0, 0.0));
            result = new Rect(val, uIElement.RenderSize);
        }
        return result;
    }
}
