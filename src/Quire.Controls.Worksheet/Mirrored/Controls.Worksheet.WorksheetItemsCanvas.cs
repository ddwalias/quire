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
                double leftOffset = 0.0;
                double topOffset = 0.0;
                int pageIndex = pageManager.GetPageIndex(whiteboardItem);
                switch (worksheetPageLayoutMode)
                {
                    case PageLayoutMode.PageMode:
                        leftOffset = pageManager.PageLayout.PageMargin.Left;
                        topOffset = pageManager.PageLayout.DisplayHeight * (double)pageIndex + pageManager.PageLayout.PageMargin.Top;
                        break;
                    case PageLayoutMode.DraftMode:
                        leftOffset = 0.0;
                        topOffset = pageManager.PageLayout.PageBodyHeight * (double)pageIndex;
                        break;
                    default:
                        leftOffset = 0.0;
                        topOffset = 0.0;
                        DebugTools.FailAssertion("Unexpected PageLayoutMode: " + worksheetPageLayoutMode);
                        break;
                }
                Point itemLocation = ItemManager.GetItemLocation(whiteboardItem);
                double left = itemLocation.X + leftOffset;
                double top = itemLocation.Y + topOffset + whiteboardItem.VerticalAlignmentOffset;
                Canvas.SetLeft(internalChild, left);
                Canvas.SetTop(internalChild, top);
            }
        }
        return base.ArrangeOverride(finalSize);
    }

    public WorksheetItemsCanvas() { }
}
