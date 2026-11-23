using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;
using Quire.Wpf.Printing;

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

    public void SplitItemIfNeeded(IWhiteboardItem item, IWorksheetPage prevItemContainingPage = null, bool forceSplitEvenIfNoVisiblePages = false)
    {
        ItemManager.SetItemSplitCheckIsRequired(item, false);
        int itemPageIndex;
        Rect itemBounds;
        if (ItemSplitIsNeeded(item, out itemPageIndex, out itemBounds))
        {
            IList<int> pageIndices;
            bool isAnyPageVisible;
            double offsetDiff;
            pageIndices = CalculateItemPageIndices(item, itemBounds, out offsetDiff);
            AppendNewPageIfNeeded(pageIndices.Last());
            RemoveNonOwnedWhiteboardItem(item, 0, -1);
            AddNonOwnedWhiteboardItem(item, pageIndices[0], pageIndices[pageIndices.Count - 1]);
            isAnyPageVisible = _worksheetVisiblePages.IsAnyPageIndexVisible(pageIndices.First(), pageIndices.Last());
            if (forceSplitEvenIfNoVisiblePages | isAnyPageVisible)
            {
                DebugTools.DevAssert(!ItemManager.GetItemSplitCheckIsRequired(item));
                if (isAnyPageVisible)
                {
                    ItemManager.SetItemIsOnVisiblePage(item, true);
                }
                IWhiteboardItemZombie zombie;
                _worksheetItemSplitter.Split(item, itemPageIndex, out zombie, prevItemContainingPage);
                FireItemSplit(item);
                if (zombie != null)
                {
                    IWhiteboardItemContainer container = zombie.Item as IWhiteboardItemContainer;
                    if (container != null && container.ActiveItem != null)
                    {
                        Size renderSize = ItemManager.GetRenderSize(container.ActiveItem);
                        ItemManager.BringIntoView(container.ActiveItem, new Rect(default(Point), renderSize));
                    }
                }
            }
            else
            {
                ItemManager.SetItemIsOnVisiblePage(item, false);
                ItemManager.SetItemSplitCheckIsRequired(item, true);
            }
        }
        else
        {
            if (DebugTools.DevAssert(itemPageIndex < Pages.Count).Passed)
            {
                IWhiteboardItemZombie existingZombie = _worksheetItemSplitter.GetItemZombieOnPage(item, Pages[itemPageIndex]);
                if (existingZombie != null)
                {
                    _worksheetItemSplitter.ReplaceZombieWithRealItem(existingZombie);
                }
            }
            RemoveNonOwnedItemAndZombies(item);
        }
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

    public IWorksheetPage SetInsertionPointOnAppropriatePage(Point insertionPoint)
    {
        int pageIndex = GetPageIndex(insertionPoint.Y);
        AppendNewPageIfNeeded(pageIndex);
        IWorksheetPage page = Pages[pageIndex];
        Point location = insertionPoint;
        if (pageIndex > 0)
        {
            double insertionPointOffset = insertionPoint.Y - PageLayout.PageBodyHeight * pageIndex;
            location = new Point(insertionPoint.X, insertionPointOffset);
        }
        ScrollToPageIfNeeded(page);
        page.InsertionPoint = location;
        RemoveEmptyPagesAfterLocation(pageIndex);
        return page;
    }

    public IWorksheetPage AppendNewPage()
    {
        IWorksheetPage page = CreatePage();
        Pages.Add(page);
        return page;
    }

    public void RemoveEmptyPagesAfterLocation(Point location)
    {
        RemoveEmptyPagesAfterLocation(GetPageIndex(location.Y));
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

    private bool IsTrailingEmptyPage(IWorksheetPage page, int pageIndex)
    {
        if (!page.WhiteboardItems.IsNullOrEmpty())
        {
            return false;
        }
        if (!page.NonOwnedWhiteboardItems.IsNullOrEmpty())
        {
            return false;
        }
        if (Pagebreaks.Any(pageBreak => pageBreak.NextPage == pageIndex))
        {
            return false;
        }
        return true;
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

    private void RemoveNonOwnedItemAndZombies(IWhiteboardItem item)
    {
        RemoveNonOwnedWhiteboardItem(item);
        _worksheetItemSplitter.RemoveZombies(item);
    }

    public void HandleItemSizeChanged(IWhiteboardItem item, Size oldSize, Size newSize)
    {
        if (oldSize == newSize && !ItemManager.GetItemSplitCheckIsRequired(item))
        {
            return;
        }
        ItemManager.SetItemSplitCheckIsRequired(item, false);
        IWorksheetPage oldPage = ItemManager.GetContainingPage(item);
        if (IsItemSplit(item) && oldPage != null)
        {
            IWorksheetPage newPage = GetPage(ItemManager.GetItemWorksheetLocation(item).Y);
            if (oldPage != newPage)
            {
                IWhiteboardItemZombie zombie = _worksheetItemSplitter.GetItemZombieOnPage(item, newPage);
                if (DebugTools.AssertNotNull(zombie).Passed)
                {
                    _worksheetItemSplitter.ReplaceZombieWithRealItem(zombie);
                }
            }
        }
        if (ItemSplitIsNeeded(item))
        {
            ExecuteOrScheduleSplitAttempt(item, oldPage, false);
            return;
        }
        RemoveNonOwnedItemAndZombies(item);
    }

    public bool ExecuteOrScheduleSplitAttempt(global::Quire.Controls.Core.IWhiteboardItem item, global::Quire.Controls.Worksheet.IWorksheetPage prevItemContainingPage = default, bool mustExecuteImmediately = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    public bool IsItemSplit(IWhiteboardItem item)
    {
        return _worksheetItemSplitter.IsItemSplit(item);
    }

    private global::System.Collections.Generic.IList<int> CalculateItemPageIndices(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Rect itemBoundingRect, out double offsetDiff) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetPageManager+SplitAction, Ptc.Controls.Worksheet")]
    private delegate void SplitAction(global::Quire.Controls.Core.IWhiteboardItem item, long stamp, global::Quire.Controls.Worksheet.IWorksheetPage prevItemContainingPage, bool forceSplitEvenIfNotVisiblePages);

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

    public Rect? GetClippedItemPartRect(IWhiteboardItem realItem, int itemPartIndex, IWorksheetItemManager worksheetItemManager)
    {
        IWhiteboardItem itemPart = null;
        IList<IWhiteboardItem> itemParts = _worksheetItemSplitter.GetItemParts(realItem);
        if (itemParts.IsNullOrEmpty())
        {
            if (itemPartIndex == 0)
            {
                itemPart = realItem;
            }
        }
        else if (itemPartIndex < itemParts.Count)
        {
            itemPart = itemParts[itemPartIndex];
        }
        if (DebugTools.AssertNotNull(itemPart, "itemPart {0} is out of bounds for realItem {1}", itemPartIndex, realItem).Failed)
        {
            return null;
        }
        IWhiteboardItemZombie zombie = itemPart as IWhiteboardItemZombie;
        Rect? clipBounds;
        if (zombie != null)
        {
            Point location;
            ZombieCreator.ConvertPageLocationAndClipBoundsFromZombieToItem(zombie, out location, out clipBounds);
        }
        else
        {
            clipBounds = ItemManager.GetItemClipGeometry(itemPart)?.Bounds;
        }
        return clipBounds;
    }

    public PageIndexRange GetPagesContaining(IWhiteboardItem item)
    {
        PageIndexRange pageRange;
        if (DebugTools.DevAssert(!ItemManager.GetItemSplitCheckIsRequired(item)).Failed)
        {
            IList<int> pageIndices = CalculateItemPageIndices(item, ItemManager.GetDescendantBounds(item), out double offsetDiff);
            pageRange = new PageIndexRange(pageIndices[0], pageIndices[pageIndices.Count - 1]);
        }
        else
        {
            IList<IWhiteboardItem> itemParts;
            if (GetWorksheetItemParts(item).IsNotEmpty(out itemParts))
            {
                pageRange = new PageIndexRange(
                    ItemManager.GetContainingPage(itemParts[0]).PageNumber - 1,
                    ItemManager.GetContainingPage(itemParts[itemParts.Count - 1]).PageNumber - 1);
            }
            else
            {
                pageRange = new PageIndexRange(GetPageIndex(item));
            }
        }
        return pageRange;
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

    public void ActivateWorksheetItemPartForcingSplitIfNeeded(global::Quire.Controls.Core.IWhiteboardItem topItem, global::Quire.Controls.Core.IWhiteboardItem intoViewItem, global::System.Windows.Rect intoViewItemRect, bool replaceOnly) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ActivateDescendantParentIfNeeded(IWhiteboardItem activeItem, IWhiteboardItem activeDescendant)
    {
        ActivateDescendantParentIfNeeded(activeItem, activeDescendant, Rect.Empty, false);
    }

    private void ActivateDescendantParentIfNeeded(global::Quire.Controls.Core.IWhiteboardItem activeItem, global::Quire.Controls.Core.IWhiteboardItem activeDescendant, global::System.Windows.Rect activeDescendantRect, bool replaceOnly) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ActivateSplitItemPartContainingDescendant(IWhiteboardItem topItemToActivate, IHasChildArrangeInfoProvider parentControl, IWhiteboardItem activeDescendant, Rect activeDescendantRect, bool replaceOnly)
    {
        IUIElementArrangeInfoProvider childArrangeInfoProvider = parentControl.ChildArrangeInfoProvider;
        Rect ancestorBounds;
        Point descendantTopLeft;
        if (activeDescendantRect.IsEmpty)
        {
            IWhiteboardControl parentWhiteboard = parentControl as IWhiteboardControl;
            if (parentWhiteboard != null)
            {
                ancestorBounds = parentWhiteboard.GetItemRect(activeDescendant);
                descendantTopLeft = parentWhiteboard.GetItemLocation(activeDescendant);
            }
            else
            {
                ancestorBounds = childArrangeInfoProvider.TransformBoundsToAncestor(activeDescendant, parentControl, new Rect(childArrangeInfoProvider.GetRenderSize(activeDescendant)));
                descendantTopLeft = ancestorBounds.TopLeft;
            }
        }
        else
        {
            ancestorBounds = childArrangeInfoProvider.TransformBoundsToAncestor(activeDescendant, parentControl, activeDescendantRect);
            descendantTopLeft = ancestorBounds.TopLeft;
        }
        if (parentControl != topItemToActivate)
        {
            ancestorBounds = childArrangeInfoProvider.TransformBoundsToAncestor(parentControl, topItemToActivate, ancestorBounds);
            descendantTopLeft = childArrangeInfoProvider.TranslatePoint(parentControl, topItemToActivate, descendantTopLeft);
        }
        ActivateMatchingPartIfNeeded(topItemToActivate, ancestorBounds, descendantTopLeft, replaceOnly);
    }

    private void ActivateMatchingPartIfNeeded(IWhiteboardItem topItemToActivate, Rect descendantRect, Point descendantLocation, bool replaceOnly)
    {
        if (!IsItemSplit(topItemToActivate))
        {
            return;
        }
        IWhiteboardItem topLeftPart = _worksheetItemSplitter.GetMatchingPart(topItemToActivate, descendantRect.TopLeft);
        IWhiteboardItem bottomRightPart = _worksheetItemSplitter.GetMatchingPart(topItemToActivate, descendantRect.BottomRight);
        int topLeftPageIndex = ItemManager.GetContainingPage(topLeftPart).PageNumber - 1;
        int bottomRightPageIndex = ItemManager.GetContainingPage(bottomRightPart).PageNumber - 1;
        int itemPageIndex = ItemManager.GetContainingPage(topItemToActivate).PageNumber - 1;
        if (itemPageIndex < topLeftPageIndex || bottomRightPageIndex < itemPageIndex)
        {
            ActivateMatchingPart(topItemToActivate, descendantLocation, replaceOnly);
        }
    }

    private double GetSplitVerticalAdjustment(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemParts, global::System.Windows.Rect itemBounds, global::System.Windows.Size itemRenderSize, double itemVerticalOffsetLim) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::System.Windows.Size GetCustomSplitItemRenderSize(global::Quire.Controls.Core.ICustomSplitItem customSplitItem) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    public void HideZombies(IWhiteboardItem item, bool clearItemClip)
    {
        _worksheetItemSplitter.HideZombies(item, clearItemClip);
    }

    public void ShowZombies(IWhiteboardItem item)
    {
        _worksheetItemSplitter.ShowZombies(item);
    }

    public double UpdateVisiblePagesOnScrollChanged(double verticalOffset, double viewportHeight, double pageHeight)
    {
        PageIndexRange previousRange = new PageIndexRange(_worksheetVisiblePages.FirstVisiblePageIndex, _worksheetVisiblePages.LastVisiblePageIndex);
        double newPageOffset = _worksheetVisiblePages.UpdateVisiblePagesOnScrollChanged(verticalOffset, viewportHeight, pageHeight);
        if (_pageRangeOnPrePrint.HasValue)
        {
            PageIndexRange currentRange = new PageIndexRange(_worksheetVisiblePages.FirstVisiblePageIndex, _worksheetVisiblePages.LastVisiblePageIndex);
            if (DebugTools.DevAssert(_pageRangeOnPrePrint.Value == currentRange).Passed)
            {
                newPageOffset = _worksheetVisiblePages.SetVisiblePages(previousRange.FirstIndex, previousRange.LastIndex, pageHeight);
            }
            else
            {
                newPageOffset = _worksheetVisiblePages.SetVisiblePages(
                    Math.Min(previousRange.FirstIndex, currentRange.FirstIndex),
                    Math.Max(previousRange.LastIndex, currentRange.LastIndex),
                    pageHeight);
            }
        }
        return newPageOffset;
    }

    public void UpdateVisiblePagesOnPageAddedIfNeeded(int indexOfFirstAddedPage, double verticalOffset, double viewportHeight, double pageHeight)
    {
        _worksheetVisiblePages.UpdateVisiblePagesOnPageAddedIfNeeded(indexOfFirstAddedPage, verticalOffset, viewportHeight, pageHeight);
    }

    public void UpdateVisiblePagesOnPageRemovedIfNeeded(int indexOfFirstRemovedPage)
    {
        _worksheetVisiblePages.UpdateVisiblePagesOnPageRemovedIfNeeded(indexOfFirstRemovedPage);
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

    public string VisiblePagesRangeToString(int startingPageNumber = 1) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public double PrePrintUpdateVisiblePages(PageIndexRange pageRange, double pageHeight)
    {
        DebugTools.DevAssert(!_pageRangeOnPrePrint.HasValue);
        _pageRangeOnPrePrint = new PageIndexRange(_worksheetVisiblePages.FirstVisiblePageIndex, _worksheetVisiblePages.LastVisiblePageIndex);
        int firstIndex = Math.Min(pageRange.FirstIndex, _worksheetVisiblePages.FirstVisiblePageIndex);
        int lastIndex = Math.Max(pageRange.LastIndex, _worksheetVisiblePages.LastVisiblePageIndex);
        return _worksheetVisiblePages.SetVisiblePages(firstIndex, lastIndex, pageHeight);
    }

    public double PostPrintUpdateVisiblePages(double pageHeight)
    {
        int firstIndex;
        int lastIndex;
        if (DebugTools.AssertNotNull(_pageRangeOnPrePrint).Passed)
        {
            firstIndex = _pageRangeOnPrePrint.Value.FirstIndex;
            lastIndex = _pageRangeOnPrePrint.Value.LastIndex;
        }
        else
        {
            firstIndex = 0;
            lastIndex = Pages.Count - 1;
        }
        _pageRangeOnPrePrint = null;
        return _worksheetVisiblePages.SetVisiblePages(firstIndex, lastIndex, pageHeight);
    }

    private void AppendNewPageIfNeeded(int pageIndex)
    {
        int requiredPageCount = pageIndex + 1;
        if (requiredPageCount > Pages.Count)
        {
            (requiredPageCount - Pages.Count).Times(i => AppendNewPage());
        }
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

    private void HandlePagebreaksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems.OfType<IWorksheetPageBreak>().Each(HandlePagebreakAdded);
                break;
            case NotifyCollectionChangedAction.Remove:
                e.OldItems.OfType<IWorksheetPageBreak>().Each(HandlePagebreakRemoved);
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
    }

    private void HandlePagebreakAdded(IWorksheetPageBreak pagebreak)
    {
        if (pagebreak.NextPage <= 0)
        {
            pagebreak.NextPage = GetPageIndex(ItemManager.GetItemWorksheetLocation(pagebreak).Y);
        }
        if (pagebreak.NextPage >= Pages.Count)
        {
            Pages.Count.UpTo(pagebreak.NextPage, i => AppendNewPage());
        }
    }

    private void HandlePagebreakRemoved(IWorksheetPageBreak pagebreak)
    {
    }

    private void HandlePagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems.OfType<IWorksheetPage>().Each(page => page.PageBodyActivated += PageBodyActivatedHandler);
                break;
            case NotifyCollectionChangedAction.Remove:
                e.OldItems.OfType<IWorksheetPage>().Each(page => page.PageBodyActivated -= PageBodyActivatedHandler);
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
        Pages.EachWithIndex((index, page) => page.PageNumber = index + 1);
    }

    private void RemoveAllItemsFromPage(global::Quire.Controls.Worksheet.IWorksheetPage page) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void RemoveItemFromPage(IWorksheetPage page, IWhiteboardItem item)
    {
        RemoveItemsFromPage(page, new[] { item });
    }

    private void RemoveItemsFromPage(IWorksheetPage page, IEnumerable<IWhiteboardItem> items)
    {
        foreach (IWhiteboardItem item in items)
        {
            long stamp = ItemManager.GetItemAddMoveRemoveTimestamp(item);
            ItemManager.SetItemAddMoveRemoveTimestamp(item, ++stamp);
            RemoveNonOwnedItemAndZombies(item);
        }
        page.RemoveItems(items);
        if (items.Any())
        {
            ItemManager.ExecuteOnUnload(items.First(), () => page.UpdateNonPrintableSpaceIndicator(RightEdgeOfPage));
        }
    }

    private void PageBodyActivatedHandler()
    {
        ContentActivated.Invoke();
    }

    private IWorksheetPage CreatePage()
    {
        return ControlCreator.CreateWorksheetPageControl(PageLayout.GridCellSize);
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

    public WorksheetPageManager(global::Quire.Controls.Worksheet.IWorksheetItemSplitter worksheetItemSplitter) { }
}
