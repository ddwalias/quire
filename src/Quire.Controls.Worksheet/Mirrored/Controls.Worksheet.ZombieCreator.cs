using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Worksheet;

public partial class ZombieCreator
{
    private readonly IDictionary<IWhiteboardItemZombie, int> _zombiePageIndexMap = new Dictionary<IWhiteboardItemZombie, int>();

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    private int PageIndex { get; set; }

    private IWhiteboardItem Item { get; set; }

    private Rect ItemBounds
    {
        get
        {
            Rect? itemBounds = _itemBounds;
            if (!itemBounds.HasValue)
            {
                return (_itemBounds = ItemManager.GetDescendantBounds(Item)).Value;
            }
            return itemBounds.GetValueOrDefault();
        }
    }

    private Size ItemSize
    {
        get
        {
            Size? itemSize = _itemSize;
            if (!itemSize.HasValue)
            {
                return (_itemSize = ItemManager.GetRenderSize(Item)).Value;
            }
            return itemSize.GetValueOrDefault();
        }
    }

    private double PageBodyHeight => _pageLayout.PageBodyHeight;

    public ZombieCreator(IWhiteboardItem item, int pageIndex, WorksheetPageLayout pageLayout)
    {
        Item = item;
        PageIndex = pageIndex;
        _pageLayout = pageLayout;
    }

    public static double RoundLocation(double value)
    {
        return Math.Round(value, 5);
    }

    public static int CompareLocation(double value1, double value2)
    {
        double firstRoundedLocation = RoundLocation(value1);
        double secondRoundedLocation = RoundLocation(value2);
        if (firstRoundedLocation != secondRoundedLocation)
        {
            if (!(firstRoundedLocation > secondRoundedLocation))
            {
                return -1;
            }
            return 1;
        }
        return 0;
    }

    private void UpdateItemClipAndOffset(CreateZombiesData data, ICustomSplitItem customSplitItem)
    {
        DebugTools.DevAssert(CompareLocation(data.CollectiveClipOffset, 0.0) == 0, null);
        double itemTop = data.ItemBoundsTop + data.CollectiveClipOffset;
        double itemBottom = itemTop + ItemBounds.Height;
        double clipBottom = ConvertYcoordFromZombieToItem(Math.Min(itemBottom, PageBodyHeight) - itemTop);
        double splitBottom;
        if (customSplitItem != null)
        {
            splitBottom = customSplitItem.GetValidSplitOffset(clipBottom, PageBodyHeight, ref data.LineNumberHint);
        }
        else
        {
            splitBottom = clipBottom;
        }
        data.CollectiveClipOffset += clipBottom - splitBottom;
        double clipTop = ItemBounds.Top + ((itemTop > 0.0) ? 0.0 : -itemTop);
        ItemManager.SetItemClipGeometry(Item, new RectangleGeometry(new Rect(data.ItemClipLeft, clipTop, data.ItemClipWidth, splitBottom - clipTop)));
    }

    private Point GetItemBoundsTopLeftRelativeToItemPage(Rect itemBounds)
    {
        return GetItemBoundsTopLeftRelativeToItemPage(Item, itemBounds, PageBodyHeight, PageIndex);
    }

    public static Point GetItemBoundsTopLeftRelativeToItemPage(IWhiteboardItem item, Rect itemBounds, double pageHeight, int pageIndex)
    {
        Point itemWorksheetLocation = ItemManager.GetItemWorksheetLocation(item);
        double top = itemWorksheetLocation.Y - pageHeight * (double)pageIndex + item.VerticalAlignmentOffset + itemBounds.Top;
        return new Point(itemWorksheetLocation.X + itemBounds.Left, top);
    }

    public static void ConvertPageLocationAndClipBoundsFromZombieToItem(IWhiteboardItemZombie zombie, out Point itemPageLocation, out Rect? itemClipBounds)
    {
        ConvertPageLocationAndClipBoundsFromTo(zombie, fromItemToZombie: false, out itemPageLocation, out itemClipBounds);
    }

    public static void ConvertPageLocationAndClipBoundsFromItemToZombie(IWhiteboardItemZombie zombie, out Point zombiePageLocation, out Rect? zombieClipBounds)
    {
        ConvertPageLocationAndClipBoundsFromTo(zombie, fromItemToZombie: true, out zombiePageLocation, out zombieClipBounds);
    }

    public static Point ConvertPointFromZombieToItem(Point pt, IWhiteboardItemZombie zombie)
    {
        return ConvertPointFromTo(pt, zombie, fromItemToZombie: false);
    }

    public static Point ConvertPointFromItemToZombie(Point pt, IWhiteboardItemZombie zombie)
    {
        return ConvertPointFromTo(pt, zombie, fromItemToZombie: true);
    }

    public static double ConvertYcoordFromItemToZombie(double offset, IWhiteboardItemZombie zombie)
    {
        return ConvertYcoordFromTo(offset, zombie.ItemBounds, fromItemToZombie: true);
    }

    private double ConvertYcoordFromZombieToItem(double offset)
    {
        return ConvertYcoordFromTo(offset, ItemBounds, fromItemToZombie: false);
    }

    private double ConvertYcoordFromItemToZombie(double offset)
    {
        return ConvertYcoordFromTo(offset, ItemBounds, fromItemToZombie: true);
    }

    private double ConvertXcoordFromItemToZombie(double xCoord)
    {
        return ConvertXcoordFromTo(xCoord, ItemBounds, fromItemToZombie: true);
    }

    private static double ConvertYcoordFromTo(double offset, Rect itemBounds, bool fromItemToZombie)
    {
        if (!fromItemToZombie)
        {
            return offset + itemBounds.Top;
        }
        return offset - itemBounds.Top;
    }

    private static double ConvertXcoordFromTo(double offset, Rect itemBounds, bool fromItemToZombie)
    {
        if (!fromItemToZombie)
        {
            return offset + itemBounds.Left;
        }
        return offset - itemBounds.Left;
    }

    private static Point ConvertPointFromTo(Point pt, IWhiteboardItemZombie zombie, bool fromItemToZombie)
    {
        return new Point(ConvertXcoordFromTo(pt.X, zombie.ItemBounds, fromItemToZombie), ConvertYcoordFromTo(pt.Y, zombie.ItemBounds, fromItemToZombie));
    }

    private static void ConvertPageLocationAndClipBoundsFromTo(IWhiteboardItemZombie zombie, bool fromItemToZombie, out Point targetPageLocation, out Rect? targetClipBounds)
    {
        double verticalOffset;
        double itemVerticalOffset;
        Point itemLocation;
        Geometry itemClipGeometry;
        if (fromItemToZombie)
        {
            verticalOffset = zombie.ItemVerticalAlignmentOffset;
            itemVerticalOffset = zombie.VerticalAlignmentOffset;
            itemLocation = ItemManager.GetItemLocation(zombie.Item);
            itemClipGeometry = ItemManager.GetItemClipGeometry(zombie.Item);
        }
        else
        {
            verticalOffset = zombie.VerticalAlignmentOffset;
            itemVerticalOffset = zombie.ItemVerticalAlignmentOffset;
            itemLocation = ItemManager.GetItemLocation(zombie);
            itemClipGeometry = ItemManager.GetItemClipGeometry(zombie);
        }
        Point pageLocation = new Point(itemLocation.X, itemLocation.Y + verticalOffset);
        Point offset = ConvertPointFromTo(new Point(0.0, -itemVerticalOffset), zombie, !fromItemToZombie);
        targetPageLocation = pageLocation + (Vector)offset;
        if (itemClipGeometry != null)
        {
            Rect bounds = itemClipGeometry.Bounds;
            targetClipBounds = new Rect(ConvertPointFromTo(bounds.TopLeft, zombie, fromItemToZombie), bounds.Size);
        }
        else
        {
            targetClipBounds = null;
        }
    }

    private IWhiteboardItemZombie CreateZombie(Point zombieLocation, int zombiePageIndex)
    {
        IWhiteboardItemZombie whiteboardItemZombie = WorksheetViewModel.ControlCreator.CreateWhiteboardItemZombie(Item);
        whiteboardItemZombie.ItemBounds = ItemBounds;
        whiteboardItemZombie.ItemSize = ItemSize;
        whiteboardItemZombie.ItemVerticalAlignmentOffset = Item.VerticalAlignmentOffset;
        ItemManager.SetItemLocation(whiteboardItemZombie, zombieLocation);
        _zombiePageIndexMap.Add(whiteboardItemZombie, zombiePageIndex);
        return whiteboardItemZombie;
    }

    public int GetZombiePageIndex(IWhiteboardItemZombie zombie)
    {
        return _zombiePageIndexMap[zombie];
    }

    private partial class CreateZombiesData
    {
        public Point ItemBoundsTopLeft { get; set; }

        public double ItemBoundsTop => ItemBoundsTopLeft.Y;

        public double ItemBoundsLeft => ItemBoundsTopLeft.X;

        public double ItemClipLeft { get; set; }

        public double ZombieClipLeft { get; set; }

        public double ItemClipWidth { get; set; }

        public double ZombieClipWidth { get; set; }

        public double ZombieXLocation { get; set; }

        public double CollectiveClipOffset { get; set; }
    }
}
