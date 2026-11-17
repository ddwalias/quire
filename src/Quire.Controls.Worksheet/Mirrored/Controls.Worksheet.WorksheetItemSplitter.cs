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
    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    private IList<IWorksheetPage> Pages => _pageManager.Pages;

    private WorksheetPageLayout PageLayout => _pageManager.PageLayout;

    public bool IsItemSplit(IWhiteboardItem item)
    {
        DebugTools.DevAssert(!ItemManager.GetItemSplitCheckIsRequired(item), null);
        return _worksheetItemPartsMap.ContainsKey(item);
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

    private static void RemoveItemFromContainingPage(IWhiteboardItem item)
    {
        ItemManager.GetContainingPage(item)?.RemoveItem(item);
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

    private void AddZombieToPartsMap(IWhiteboardItem item, IWhiteboardItemZombie zombie)
    {
        if (!_worksheetItemPartsMap.TryGetValue(item, out var value))
        {
            value = new List<IWhiteboardItemZombie>();
            _worksheetItemPartsMap.Add(item, value);
        }
        value.Add(zombie);
    }

    private void AddSizeChangedHandlerToItemMap(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler eventHandler)
    {
        if (!_itemSizeChangedHandlerMap.TryGetValue(item, out var value))
        {
            value = new List<WhiteboardItemSizeChangedEventHandler>();
            _itemSizeChangedHandlerMap.Add(item, value);
        }
        value.Add(eventHandler);
    }

    private void RemoveSizeChangedHandlerFromItemMap(IWhiteboardItem item)
    {
        if (_itemSizeChangedHandlerMap.TryGetValue(item, out var value))
        {
            DisposeEventHandlers(item, value);
            _itemSizeChangedHandlerMap.Remove(item);
        }
    }

    private static void UnhookAndDispose(IWhiteboardItemZombie zombie)
    {
        ItemManager.UnhookMouseEnter(zombie);
        ItemManager.UnhookDragEnter(zombie);
        zombie.Dispose();
    }
}
