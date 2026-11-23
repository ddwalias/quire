using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetPageManager
{
    private static IControlFactory ControlCreator => WorksheetViewModel.ControlCreator;

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    private double RightEdgeOfPage => PageLayout.PageBodyWidth;

    public ObservableRangeCollection<IWorksheetPage> Pages => _pages;

    public ObservableRangeCollection<IWorksheetPage> VisiblePages => _worksheetVisiblePages.VisiblePages;

    public WorksheetPageLayout PageLayout => _worksheetPageLayout;

    public IWorksheetVisualHelper VisualHelper => _worksheetVisualHelper;

    public bool IsVisiblePagesLoaded => _worksheetVisiblePages.IsVisiblePagesLoaded;

    public ObservableRangeCollection<IWorksheetPageBreak> Pagebreaks
    {
        private get
        {
            return _pagebreaks;
        }
        set
        {
            if (_pagebreaks != null)
            {
                _pagebreaks.RangeChanged -= HandlePagebreaksCollectionChanged;
            }
            _pagebreaks = value;
            if (_pagebreaks != null)
            {
                _pagebreaks.RangeChanged += HandlePagebreaksCollectionChanged;
            }
        }
    }

    private void Initialize()
    {
        Pages.RangeChanged += HandlePagesCollectionChanged;
    }

    private void ValidateAndSplitItemIfNeeded(IWhiteboardItem item, long stamp, IWorksheetPage prevItemContainingPage, bool forceSplitEvenIfNotVisiblePages)
    {
        if (CheckIfSplitIsAllowedAndRequireFutureSplitIfNeeded(item, stamp))
        {
            if (item is ICustomSplitItem customSplitItem)
            {
                customSplitItem.ValidateLayout();
            }
            if (CheckIfSplitIsAllowedAndRequireFutureSplitIfNeeded(item, stamp))
            {
                SplitItemIfNeeded(item, prevItemContainingPage, forceSplitEvenIfNotVisiblePages);
            }
        }
    }

    private bool ItemSplitIsNeeded(IWhiteboardItem item)
    {
        int itemPageIndex;
        Rect itemBounds;
        return ItemSplitIsNeeded(item, out itemPageIndex, out itemBounds);
    }

    private void FireItemSplit(IWhiteboardItem whiteboardItem)
    {
        ItemSplit?.Invoke(whiteboardItem);
    }

    public IWorksheetPage AppendNewPage()
    {
        IWorksheetPage page = CreatePage();
        Pages.Add(page);
        return page;
    }

    public int GetTrailingEmptyPageCount()
    {
        int trailingEmptyPageCount = 0;
        for (int pageIndex = Pages.Count - 1; pageIndex > 0; pageIndex--)
        {
            if (!IsTrailingEmptyPage(Pages[pageIndex], pageIndex))
            {
                break;
            }
            trailingEmptyPageCount++;
        }
        return trailingEmptyPageCount;
    }

    public IWorksheetPage GetItemPage(IWhiteboardItem item)
    {
        IWorksheetPage containingPage = ItemManager.GetContainingPage(item);
        if (containingPage != null)
        {
            return containingPage;
        }
        Point itemLocation = ItemManager.GetItemWorksheetLocation(item);
        return GetPage(itemLocation.Y);
    }

    public IList<IWhiteboardItem> GetWorksheetItemParts(IWhiteboardItem item)
    {
        return _worksheetItemSplitter.GetItemParts(item);
    }

    public bool IsItemSplit(IWhiteboardItem item)
    {
        return _worksheetItemSplitter.IsItemSplit(item);
    }

    public void HideZombies(IWhiteboardItem item, bool clearItemClip)
    {
        _worksheetItemSplitter.HideZombies(item, clearItemClip);
    }

    public void ShowZombies(IWhiteboardItem item)
    {
        _worksheetItemSplitter.ShowZombies(item);
    }

    public void UpdateVisiblePagesOnPageRemovedIfNeeded(int indexOfFirstRemovedPage)
    {
        _worksheetVisiblePages.UpdateVisiblePagesOnPageRemovedIfNeeded(indexOfFirstRemovedPage);
    }

    private void RemoveItemFromPage(IWorksheetPage page, IWhiteboardItem item)
    {
        RemoveItemsFromPage(page, new[] { item });
    }

    private IWorksheetPage CreatePage()
    {
        return ControlCreator.CreateWorksheetPageControl(PageLayout.GridCellSize);
    }
}
