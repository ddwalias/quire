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

    public void RemoveEmptyPagesAfterLocation(Point location)
    {
        RemoveEmptyPagesAfterLocation(GetPageIndex(location.Y));
    }

    private void RemoveEmptyPagesAfterLocation(int locationPageIndex)
    {
        if (locationPageIndex >= Pages.Count - 1)
        {
            return;
        }
        IWorksheetPage locationPage = Pages[locationPageIndex];
        do
        {
            IWorksheetPage lastPage = Pages.Last();
            if (lastPage == locationPage)
            {
                break;
            }
            if (!IsTrailingEmptyPage(lastPage, Pages.Count - 1))
            {
                break;
            }
            Pages.Remove(lastPage);
        }
        while (Pages.Count > 1);
    }

    private IWorksheetPage GetPage(double yOffset)
    {
        int pageIndex = GetPageIndex(yOffset);
        if (DebugTools.DevAssert(pageIndex >= 0).Failed)
        {
            return Pages.First();
        }
        if (pageIndex >= Pages.Count)
        {
            return Pages.Last();
        }
        return Pages[pageIndex];
    }

    private void RemoveNonOwnedItemAndZombies(IWhiteboardItem item)
    {
        RemoveNonOwnedWhiteboardItem(item);
        _worksheetItemSplitter.RemoveZombies(item);
    }

    public void ScrollToPageIfNeeded(IWorksheetPage targetPage)
    {
        if (VisiblePages.IsEmpty() || VisiblePages.Contains(targetPage))
        {
            return;
        }
        VisualHelper.ScrollToPage(VisiblePages.First(), targetPage);
    }

    public void GoToPage(IWorksheetPage targetPage)
    {
        if (VisiblePages.IsEmpty())
        {
            return;
        }
        VisualHelper.ScrollToPage(VisiblePages.First(), targetPage, true);
    }

    public void RemoveItemFromAppropriatePage(IWhiteboardItem item)
    {
        RemoveItemFromAppropriatePage(item, true);
    }

    public void RemoveItemFromAppropriatePage(IWhiteboardItem item, bool removeEmptyPagesAtTheEnd)
    {
        IWorksheetPage page = GetItemPage(item);
        ItemManager.SetItemIsOnVisiblePage(item, false);
        RemoveItemFromPage(page, item);
        if (removeEmptyPagesAtTheEnd)
        {
            Point location = new Point(0, 0);
            if (page.PageBody.IsCrosshairVisible)
            {
                location = page.InsertionPoint;
                location.Y += Pages.IndexOf(page) * _worksheetPageLayout.PageBodyHeight;
            }
            RemoveEmptyPagesAfterLocation(location);
        }
    }

    public int GetPageIndex(double itemLocation)
    {
        return GetPageIndex(itemLocation, true);
    }

    public int GetPageIndex(IWhiteboardItem whiteboardItem)
    {
        IWorksheetPage page = GetItemPage(whiteboardItem);
        if (DebugTools.AssertNotNull(page, "page is null").Passed
            && DebugTools.DevAssert(page.PageNumber > 0, "page number is {0}", page.PageNumber).Passed)
        {
            return page.PageNumber - 1;
        }
        return GetPageIndex(ItemManager.GetItemWorksheetLocation(whiteboardItem).Y);
    }

    private void AddNonOwnedWhiteboardItem(IWhiteboardItem whiteboardItem, int firstPageIndex, int lastPageIndex)
    {
        for (int pageIndex = firstPageIndex; pageIndex <= lastPageIndex; pageIndex++)
        {
            if (!Pages[pageIndex].NonOwnedWhiteboardItems.Contains(whiteboardItem))
            {
                Pages[pageIndex].NonOwnedWhiteboardItems.Add(whiteboardItem);
            }
        }
    }

    private void RemoveNonOwnedWhiteboardItem(IWhiteboardItem whiteboardItem, int firstPageIndex = 0, int lastPageIndex = -1)
    {
        if (lastPageIndex == -1)
        {
            lastPageIndex = Pages.Count - 1;
        }
        for (int pageIndex = firstPageIndex; pageIndex <= lastPageIndex; pageIndex++)
        {
            Pages[pageIndex].NonOwnedWhiteboardItems.Remove(whiteboardItem);
        }
    }

    private void PageBodyActivatedHandler()
    {
        ContentActivated.Invoke();
    }

    public int GetPageIndex(double yOffset, bool roundUpIfNeeded)
    {
        if (DebugTools.DevAssert(PageLayout.PageBodyHeight > 0).Failed)
        {
            return 0;
        }
        double gridHeight = PageLayout.GridCellSize.Height;
        int pageIndex = (int)(yOffset / PageLayout.PageBodyHeight);
        double remainder = yOffset - PageLayout.PageBodyHeight * pageIndex;
        if (roundUpIfNeeded && (PageLayout.PageBodyHeight - remainder).IsLessThan(gridHeight / 2))
        {
            pageIndex++;
        }
        return pageIndex;
    }

    public void UpdateVisiblePagesOnPageAddedIfNeeded(int indexOfFirstAddedPage, double verticalOffset, double viewportHeight, double pageHeight)
    {
        _worksheetVisiblePages.UpdateVisiblePagesOnPageAddedIfNeeded(indexOfFirstAddedPage, verticalOffset, viewportHeight, pageHeight);
    }

    private static bool CheckIfSplitIsAllowedAndRequireFutureSplitIfNeeded(IWhiteboardItem item, long stamp)
    {
        if (stamp != ItemManager.GetItemAddMoveRemoveTimestamp(item))
        {
            return false;
        }
        if (ItemManager.ItemHasPendingSizeChangedEvents(item))
        {
            ItemManager.SetItemSplitCheckIsRequired(item, true);
            return false;
        }
        return true;
    }

    public void ActivateItemPart(IWhiteboardItem itemToActivate, int partIndex, bool replaceOnly)
    {
        IWhiteboardItem[] itemParts = _worksheetItemSplitter.GetItemParts(itemToActivate).ToArray();
        if (!DebugTools.DevAssert(partIndex < itemParts.Length).Passed)
        {
            return;
        }
        IWhiteboardItemZombie zombie = itemParts[partIndex] as IWhiteboardItemZombie;
        if (zombie != null)
        {
            ReplaceWithRealItemAndActivateIfNeeded(zombie, replaceOnly);
        }
    }

    private void ActivateMatchingPart(IWhiteboardItem itemToActivate, Point location, bool replaceOnly)
    {
        IWhiteboardItem matchingPart = _worksheetItemSplitter.GetMatchingPart(itemToActivate, location);
        IWhiteboardItemZombie matchingZombie = matchingPart as IWhiteboardItemZombie;
        if (matchingZombie != null)
        {
            ReplaceWithRealItemAndActivateIfNeeded(matchingZombie, replaceOnly);
            return;
        }
        DebugTools.DevAssert(matchingPart == itemToActivate);
    }

    private void ReplaceWithRealItemAndActivateIfNeeded(IWhiteboardItemZombie zombiePart, bool replaceOnly)
    {
        _worksheetItemSplitter.ReplaceZombieWithRealItem(zombiePart);
        if (!replaceOnly)
        {
            zombiePart.Item.Activate(true);
        }
    }

    public void ActivateDescendantParentIfNeeded(IWhiteboardItem activeItem, IWhiteboardItem activeDescendant)
    {
        ActivateDescendantParentIfNeeded(activeItem, activeDescendant, Rect.Empty, false);
    }

}
