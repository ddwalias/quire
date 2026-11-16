using System;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetItemsCanvas : Canvas
{
    private static readonly Type _typeOfThis = typeof(WorksheetItemsCanvas);

    public static readonly DependencyProperty PageManagerProperty = DependencyProperty.Register("PageManager", typeof(IWorksheetPageManager), _typeOfThis);

    public static readonly DependencyProperty WorksheetPageLayoutModeProperty = DependencyProperty.Register("WorksheetPageLayoutMode", typeof(PageLayoutMode), _typeOfThis);

    public IWorksheetPageManager PageManager
    {
        get => (IWorksheetPageManager)GetValue(PageManagerProperty);
        set => SetValue(PageManagerProperty, value);
    }

    public PageLayoutMode WorksheetPageLayoutMode
    {
        get => (PageLayoutMode)GetValue(WorksheetPageLayoutModeProperty);
        set => SetValue(WorksheetPageLayoutModeProperty, value);
    }

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (finalSize.Width > 0.0 && InternalChildren.Count > 0)
        {
            IWorksheetPageManager pageManager = PageManager;
            PageLayoutMode worksheetPageLayoutMode = WorksheetPageLayoutMode;
            foreach (UIElement internalChild in InternalChildren)
            {
                IWhiteboardItem whiteboardItem = ((WorksheetItemPresenter)internalChild).Child as IWhiteboardItem;
                DebugTools.DevAssert(whiteboardItem != null, "Object must be IWhiteboardItem");
                WhiteboardViewModel.WhiteboardItemMgr.ClearHandleChangesEnabled(whiteboardItem);
                double num = 0.0;
                double num2 = 0.0;
                int pageIndex = pageManager.GetPageIndex(whiteboardItem);
                switch (worksheetPageLayoutMode)
                {
                    case PageLayoutMode.PageMode:
                        num = pageManager.PageLayout.PageMargin.Left;
                        num2 = pageManager.PageLayout.DisplayHeight * (double)pageIndex + pageManager.PageLayout.PageMargin.Top;
                        break;
                    case PageLayoutMode.DraftMode:
                        num = 0.0;
                        num2 = pageManager.PageLayout.PageBodyHeight * (double)pageIndex;
                        break;
                    default:
                        num = 0.0;
                        num2 = 0.0;
                        DebugTools.FailAssertion("Unexpected PageLayoutMode: " + worksheetPageLayoutMode);
                        break;
                }
                Point itemLocation = ItemManager.GetItemLocation(whiteboardItem);
                double length = itemLocation.X + num;
                double length2 = itemLocation.Y + num2 + whiteboardItem.VerticalAlignmentOffset;
                Canvas.SetLeft(internalChild, length);
                Canvas.SetTop(internalChild, length2);
            }
        }
        return base.ArrangeOverride(finalSize);
    }

    public WorksheetItemsCanvas() { }
}
