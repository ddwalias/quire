using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetItemSplitter
{
    public static DependencyProperty ContainingPageProperty = DependencyProperty.RegisterAttached(
        "ContainingPage",
        typeof(IWorksheetPage),
        typeof(IWorksheetItemSplitter));

    private readonly IDictionary<IWhiteboardItem, IList<IWhiteboardItemZombie>> _worksheetItemPartsMap = new Dictionary<IWhiteboardItem, IList<IWhiteboardItemZombie>>();
    private readonly IDictionary<IWhiteboardItem, double> _worksheetItemCollectiveSplitOffsetMap = new Dictionary<IWhiteboardItem, double>();
    private readonly IDictionary<IWhiteboardItem, IList<WhiteboardItemSizeChangedEventHandler>> _itemSizeChangedHandlerMap = new Dictionary<IWhiteboardItem, IList<WhiteboardItemSizeChangedEventHandler>>();
    private readonly IWorksheetPageManager _pageManager;

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    public WorksheetItemSplitter(IWorksheetPageManager pageManager)
    {
        _pageManager = pageManager;
    }

    private IList<IWorksheetPage> Pages => _pageManager.Pages;

    private WorksheetPageLayout PageLayout => _pageManager.PageLayout;

    public IList<IWhiteboardItem> GetItemParts(IWhiteboardItem item)
    {
        DebugTools.DevAssert(!ItemManager.GetItemSplitCheckIsRequired(item), null);
        List<IWhiteboardItem> list = new List<IWhiteboardItem>();
        if (_worksheetItemPartsMap.TryGetValue(item, out IList<IWhiteboardItemZombie> zombies))
        {
            list.Add(item);
            zombies.Each(list.Add);
        }
        list.Sort(delegate (IWhiteboardItem part1, IWhiteboardItem part2)
        {
            int pageNumber1 = 0;
            int pageNumber2 = 0;
            IWorksheetPage page1 = ItemManager.GetContainingPage(part1);
            if (page1 != null)
            {
                pageNumber1 = page1.PageNumber;
            }
            IWorksheetPage page2 = ItemManager.GetContainingPage(part2);
            if (page2 != null)
            {
                pageNumber2 = page2.PageNumber;
            }
            return pageNumber1.CompareTo(pageNumber2);
        });
        return list;
    }

    public bool IsItemSplit(IWhiteboardItem item)
    {
        DebugTools.DevAssert(!ItemManager.GetItemSplitCheckIsRequired(item), null);
        return _worksheetItemPartsMap.ContainsKey(item);
    }

    public void Split(IWhiteboardItem item, int pageIndex, out IWhiteboardItemZombie activeZombie, IWorksheetPage prevItemContainingPage = null)
    {
        IWorksheetPage newContainingPage = Pages[pageIndex];
        // Release IL reads prevItemContainingPage before the null-coalescing assignment.
        _ = prevItemContainingPage;
        prevItemContainingPage = prevItemContainingPage ?? ItemManager.GetContainingPage(item);
        // Pre-declare zombies so zombieOnNewPage receives local slot 3 as in Mathcad IL.
        ICollection<IWhiteboardItemZombie> zombies;
        IWhiteboardItemZombie zombieOnNewPage = GetItemZombieOnPage(item, newContainingPage);
        if (zombieOnNewPage != null)
        {
            ReplaceZombieWithRealItem(zombieOnNewPage);
        }
        RemoveZombies(item);
        if (_worksheetItemCollectiveSplitOffsetMap.ContainsKey(item))
        {
            _worksheetItemCollectiveSplitOffsetMap.Remove(item);
        }
        ItemManager.SetContainingPage(item, newContainingPage);
        ZombieCreator zombieCreator = new ZombieCreator(item, pageIndex, PageLayout);
        zombies = zombieCreator.CreateZombies(out activeZombie);
        DebugTools.DevAssert(!_worksheetItemCollectiveSplitOffsetMap.ContainsKey(item), "SplitOffsetMap got populated prematurely!");
        DebugTools.DevAssert(!_worksheetItemPartsMap.ContainsKey(item), "ItemPartsMap got populated prematurely!");
        zombies.Each((IWhiteboardItemZombie zombie) =>
        {
            int zombiePageIndex = zombieCreator.GetZombiePageIndex(zombie);
            AddItemToPage(zombie, zombiePageIndex);
            ItemManager.HookupMouseEnter(zombie, (IWhiteboardItem sender) =>
            {
                ReplaceZombieWithRealItem(sender as IWhiteboardItemZombie);
            });
            ItemManager.HookupDragEnter(zombie, (IWhiteboardItem sender) =>
            {
                ReplaceZombieWithRealItem(sender as IWhiteboardItemZombie);
            });
            AddSizeChangedHandlerToItemMap(item, ItemManager.HookupSizeChanged(item, UpdateZombieRectAction));
            AddZombieToPartsMap(item, zombie);

            void UpdateZombieRectAction(IWhiteboardItem i, Size size)
            {
                zombie.UpdateZombieRect();
            }
        });
        if (activeZombie == null)
        {
            activeZombie = zombies.SingleOrDefault((IWhiteboardItemZombie z) => ItemManager.GetContainingPage(z) == prevItemContainingPage);
        }
        if (activeZombie != null)
        {
            ReplaceZombieWithRealItem(activeZombie);
        }
    }

    public void RemoveZombies(IWhiteboardItem item)
    {
        if (_worksheetItemPartsMap.ContainsKey(item))
        {
            GetZombiesForItem(item).Each(delegate (IWhiteboardItemZombie zombie)
            {
                RemoveItemFromContainingPage(zombie);
                UnhookAndDispose(zombie);
            });
            RemoveSizeChangedHandlerFromItemMap(item);
            _worksheetItemPartsMap.Remove(item);
        }
        ItemManager.SetContainingPage(item, null);
        ItemManager.SetItemClipGeometry(item, null);
    }

    private void AddItemToPage(IWhiteboardItem item, int pageIndex)
    {
        if (DebugTools.DevAssert(pageIndex < Pages.Count, null).Failed)
        {
            _pageManager.AppendNewPage();
        }
        IWorksheetPage worksheetPage = Pages[pageIndex];
        Point itemLocation = ItemManager.GetItemLocation(item);
        worksheetPage.AddItemAtLocation(item, itemLocation);
        ItemManager.SetContainingPage(item, worksheetPage);
    }

    private IList<IWhiteboardItemZombie> GetZombiesForItem(IWhiteboardItem item)
    {
        if (!_worksheetItemPartsMap.TryGetValue(item, out IList<IWhiteboardItemZombie> value))
        {
            value = new List<IWhiteboardItemZombie>();
        }
        return value;
    }

    private static void RemoveItemFromContainingPage(IWhiteboardItem item)
    {
        ItemManager.GetContainingPage(item)?.RemoveItem(item);
    }

    public void ReplaceZombieWithRealItem(IWhiteboardItemZombie zombie)
    {
        IWorksheetPage zombiePage = ItemManager.GetContainingPage(zombie);
        IWorksheetPage itemPage = ItemManager.GetContainingPage(zombie.Item);
        if (DebugTools.AssertNotNull(zombiePage, "The zombie page was null").Failed ||
            DebugTools.AssertNotNull(itemPage, "The item page was null").Failed)
        {
            return;
        }
        ZombieCreator.ConvertPageLocationAndClipBoundsFromZombieToItem(zombie, out Point itemLocation, out Rect? itemClipBounds);
        ZombieCreator.ConvertPageLocationAndClipBoundsFromItemToZombie(zombie, out Point zombieLocation, out Rect? zombieClipBounds);
        zombiePage.RemoveItem(zombie);
        itemPage.RemoveItem(zombie.Item);
        zombiePage.AddItemAtLocation(zombie.Item, itemLocation);
        ItemManager.SetContainingPage(zombie.Item, zombiePage);
        ItemManager.SetItemClipGeometry(zombie.Item, !itemClipBounds.HasValue ? null : new RectangleGeometry(itemClipBounds.Value));
        itemPage.AddItemAtLocation(zombie, zombieLocation);
        ItemManager.SetContainingPage(zombie, itemPage);
        ItemManager.SetItemClipGeometry(zombie, !zombieClipBounds.HasValue ? null : new RectangleGeometry(zombieClipBounds.Value));
    }

    public double GetCollectiveSplitOffsetDiff(IWhiteboardItem item, Rect itemBounds, double itemTopOnPage)
    {
        double collectiveOffset = 0.0;
        if (!_worksheetItemCollectiveSplitOffsetMap.ContainsKey(item))
        {
            if (item is ICustomSplitItem customSplitItem)
            {
                Size renderSize = ItemManager.GetRenderSize(item);
                int itemPageCount = 0;
                double desiredSplitOffset = PageLayout.PageBodyHeight - itemTopOnPage - (itemBounds.Height - renderSize.Height) / 2.0;
                double remainingHeight = itemBounds.Height - (-itemTopOnPage + PageLayout.PageBodyHeight);
                double validSplitOffset = customSplitItem.GetValidSplitOffset(desiredSplitOffset, PageLayout.PageBodyHeight, ref itemPageCount);
                collectiveOffset += desiredSplitOffset - validSplitOffset;
                if (remainingHeight > 0.0)
                {
                    int pageOffsetIndex = 0;
                    while (remainingHeight > 0.0)
                    {
                        double prevValidSplitOffset = validSplitOffset;
                        double nextItemTopOnPage = ZombieCreator.RoundLocation(itemTopOnPage - PageLayout.PageBodyHeight * (double)(pageOffsetIndex + 1)) + collectiveOffset;
                        desiredSplitOffset = PageLayout.PageBodyHeight - nextItemTopOnPage - (itemBounds.Height - renderSize.Height) / 2.0;
                        validSplitOffset = customSplitItem.GetValidSplitOffset(desiredSplitOffset, PageLayout.PageBodyHeight, ref itemPageCount);
                        remainingHeight -= validSplitOffset - prevValidSplitOffset;
                        collectiveOffset += desiredSplitOffset - validSplitOffset;
                        pageOffsetIndex++;
                    }
                }
            }
            if (!_worksheetItemCollectiveSplitOffsetMap.ContainsKey(item))
            {
                _worksheetItemCollectiveSplitOffsetMap.Add(item, collectiveOffset);
            }
            else
            {
                double oldOffset = _worksheetItemCollectiveSplitOffsetMap[item];
                _worksheetItemCollectiveSplitOffsetMap[item] = collectiveOffset;
                DebugTools.FailAssertion("Re-entrancy detected - oldOffset: {0}, newOffset; {1}", oldOffset, collectiveOffset);
            }
        }
        else
        {
            collectiveOffset = _worksheetItemCollectiveSplitOffsetMap[item];
        }
        return collectiveOffset;
    }

    public IWhiteboardItem GetMatchingPart(IWhiteboardItem item, Point pt)
    {
        List<IWhiteboardItem> list = GetItemParts(item).ToList();
        if (DebugTools.DevAssert(list.IsNotEmpty(), null).Failed)
        {
            return item;
        }
        IWhiteboardItemZombie zombie = list.OfType<IWhiteboardItemZombie>().First();
        double num = ZombieCreator.ConvertYcoordFromItemToZombie(pt.Y, zombie);
        IWhiteboardItem whiteboardItem = null;
        foreach (IWhiteboardItem item2 in list)
        {
            Rect bounds = ItemManager.GetItemClipGeometry(item2).Bounds;
            double value = ((item2 == item) ? pt.Y : num);
            if (ZombieCreator.CompareLocation(value, bounds.Top) < 0)
            {
                if (DebugTools.DevAssert(whiteboardItem == null, null).Passed)
                {
                    whiteboardItem = item2;
                }
                break;
            }
            whiteboardItem = item2;
            if (ZombieCreator.CompareLocation(value, bounds.Bottom) <= 0 && ZombieCreator.CompareLocation(value, bounds.Bottom) < 0)
            {
                break;
            }
        }
        return whiteboardItem;
    }

    public bool TranslateToRepresentMatchingPartLocation(IWhiteboardItem item, Point ptIn, out Point ptOut)
    {
        ptOut = ptIn;
        if (!IsItemSplit(item))
        {
            return false;
        }
        IWhiteboardItem matchingPart = GetMatchingPart(item, ptIn);
        if (matchingPart == item)
        {
            return false;
        }
        IWhiteboardItemZombie whiteboardItemZombie = matchingPart as IWhiteboardItemZombie;
        if (DebugTools.AssertNotNull(whiteboardItemZombie, null).Failed)
        {
            return false;
        }
        IWorksheetPage containingPage = ItemManager.GetContainingPage(whiteboardItemZombie);
        if (DebugTools.DevAssert(containingPage != null, "The zombie page was null").Failed)
        {
            return false;
        }
        IWorksheetPage containingPage2 = ItemManager.GetContainingPage(item);
        if (DebugTools.DevAssert(containingPage2 != null, "The item page was null").Failed)
        {
            return false;
        }
        int num = containingPage.PageNumber - containingPage2.PageNumber;
        double num2 = ZombieCreator.ConvertYcoordFromItemToZombie(ptIn.Y, whiteboardItemZombie);
        Point val = containingPage2.PageBody.GetItemLocation(item) + new Vector(0.0, whiteboardItemZombie.ItemVerticalAlignmentOffset);
        Point val2 = containingPage.PageBody.GetItemLocation(whiteboardItemZombie) + new Vector(0.0, whiteboardItemZombie.VerticalAlignmentOffset);
        ptOut = new Point(ptIn.X, PageLayout.DisplayHeight * (double)num - val.Y + val2.Y + num2);
        return true;
    }

    public IWhiteboardItemZombie GetItemZombieOnPage(IWhiteboardItem item, IWorksheetPage page)
    {
        return GetZombiesForItem(item).SingleOrDefault((IWhiteboardItemZombie z) => ItemManager.GetContainingPage(z) == page);
    }

    public void Dispose()
    {
        _worksheetItemPartsMap.Values.SelectMany((IList<IWhiteboardItemZombie> zombies) => zombies).Each(UnhookAndDispose);
        _worksheetItemPartsMap.Clear();
        _itemSizeChangedHandlerMap.Each(delegate (KeyValuePair<IWhiteboardItem, IList<WhiteboardItemSizeChangedEventHandler>> pair)
        {
            DisposeEventHandlers(pair.Key, pair.Value);
        });
        _itemSizeChangedHandlerMap.Clear();
        _worksheetItemCollectiveSplitOffsetMap.Clear();
    }

    public void HideZombies(IWhiteboardItem item, bool clearItemClip)
    {
        GetZombiesForItem(item).Each(delegate (IWhiteboardItemZombie z)
        {
            z.Hide();
        });
        if (clearItemClip)
        {
            ItemManager.SetItemClipGeometry(item, null);
        }
    }

    public void ShowZombies(IWhiteboardItem item)
    {
        GetZombiesForItem(item).Each(delegate (IWhiteboardItemZombie z)
        {
            z.Show();
        });
    }

    private void AddZombieToPartsMap(IWhiteboardItem item, IWhiteboardItemZombie zombie)
    {
        if (!_worksheetItemPartsMap.TryGetValue(item, out IList<IWhiteboardItemZombie> value))
        {
            value = new List<IWhiteboardItemZombie>();
            _worksheetItemPartsMap.Add(item, value);
        }
        value.Add(zombie);
    }

    private void AddSizeChangedHandlerToItemMap(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler eventHandler)
    {
        if (!_itemSizeChangedHandlerMap.TryGetValue(item, out IList<WhiteboardItemSizeChangedEventHandler> value))
        {
            value = new List<WhiteboardItemSizeChangedEventHandler>();
            _itemSizeChangedHandlerMap.Add(item, value);
        }
        value.Add(eventHandler);
    }

    private void RemoveSizeChangedHandlerFromItemMap(IWhiteboardItem item)
    {
        if (_itemSizeChangedHandlerMap.TryGetValue(item, out IList<WhiteboardItemSizeChangedEventHandler> value))
        {
            DisposeEventHandlers(item, value);
            _itemSizeChangedHandlerMap.Remove(item);
        }
    }

    private static void DisposeEventHandlers(IWhiteboardItem item, IList<WhiteboardItemSizeChangedEventHandler> eventHandlers)
    {
        eventHandlers.Each(delegate (WhiteboardItemSizeChangedEventHandler sizeChangedEventHandler)
        {
            ItemManager.UnhookSizeChanged(item, sizeChangedEventHandler);
        });
        eventHandlers.Clear();
    }

    private static void UnhookAndDispose(IWhiteboardItemZombie zombie)
    {
        ItemManager.UnhookMouseEnter(zombie);
        ItemManager.UnhookDragEnter(zombie);
        zombie.Dispose();
    }
}
