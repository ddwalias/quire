#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardItemsSeparator
{
    private readonly IEnumerable<IWhiteboardItem> _itemsToSeparate;

    private readonly Dictionary<IWhiteboardItem, Rect> _oldItemRectMap = new Dictionary<IWhiteboardItem, Rect>();

    private readonly Dictionary<IWhiteboardItem, Rect> _newItemRectMap = new Dictionary<IWhiteboardItem, Rect>();

    private readonly Dimension _dimension;

    private IAlignmentGrid _grid;

    private readonly IWhiteboardItemManager _whiteboardItemManager;

    public WhiteboardItemsSeparator(IEnumerable<IWhiteboardItem> itemsToSeparate, Dimension dimension, IAlignmentGrid grid, IWhiteboardItemManager whiteboardItemManager)
    {
        _itemsToSeparate = itemsToSeparate;
        _dimension = dimension;
        _grid = grid;
        _whiteboardItemManager = whiteboardItemManager;
    }

    public bool CanSeparate()
    {
        return _whiteboardItemManager.ItemsOverlapped(_itemsToSeparate);
    }

    public IDictionary<IWhiteboardItem, Vector> Separate()
    {
        IEnumerable<IWhiteboardItem> itemsToSeparate = _whiteboardItemManager.SortItemsByLocation(_itemsToSeparate, false);
        itemsToSeparate.Each(item =>
        {
            Rect itemRect = _whiteboardItemManager.GetItemRect(item);
            _oldItemRectMap.Add(item, itemRect);
            _newItemRectMap.Add(item, itemRect);
        });
        itemsToSeparate.EachWithIndex((i, item) =>
        {
            if (i != 0)
            {
                foreach (IWhiteboardItem prevItem in itemsToSeparate.Take(i))
                {
                    Rect currentItemRect = _newItemRectMap[item];
                    Rect prevItemRect = _newItemRectMap[prevItem];
                    double prevTop = prevItemRect.Top - prevItem.VerticalAlignmentOffset;
                    double currentTop = currentItemRect.Top - item.VerticalAlignmentOffset;
                    if (_dimension == Dimension.Horizontal)
                    {
                        if (prevTop.IsEqualTo(currentTop) && prevItemRect.Left.IsGreaterThan(currentItemRect.Left))
                        {
                            currentItemRect.Offset(prevItemRect.Left - currentItemRect.Left, 0.0);
                        }
                        if (currentItemRect.IntersectsWithNonZeroArea(prevItemRect))
                        {
                            double offset = prevItemRect.Right.IsEqualTo(currentItemRect.Left)
                                ? _grid.GridCellSize.Width
                                : (prevItemRect.Right - currentItemRect.Left);
                            currentItemRect.Offset(offset, 0.0);
                        }
                        Point pt = _grid.GetNearestIntersectionPointDisregardingBounds(new Point(currentItemRect.Left, currentTop), GridRestriction.Ignore, Direction.Right);
                        double num5 = pt.X.IsEqualTo(currentItemRect.Left) ? 0.0 : (pt.X - currentItemRect.Left);
                        currentItemRect.Offset(num5 + (num5.IsGreaterThanOrEqual(0.0) ? 0.0 : _grid.GridCellSize.Width), 0.0);
                    }
                    else
                    {
                        if (prevTop.IsGreaterThan(currentTop))
                        {
                            currentItemRect.Offset(0.0, prevTop - currentTop);
                        }
                        currentTop = currentItemRect.Top - item.VerticalAlignmentOffset;
                        if (prevTop.IsEqualTo(currentTop) && prevItemRect.Left.IsGreaterThan(currentItemRect.Left))
                        {
                            currentItemRect.Offset(0.0, _grid.GridCellSize.Height);
                        }
                        if (currentItemRect.IntersectsWithNonZeroArea(prevItemRect))
                        {
                            double offset = prevItemRect.Bottom.IsEqualTo(currentItemRect.Top)
                                ? _grid.GridCellSize.Height
                                : (prevItemRect.Bottom - currentItemRect.Top);
                            currentItemRect.Offset(0.0, offset);
                        }
                        currentTop = currentItemRect.Top - item.VerticalAlignmentOffset;
                        Point pt2 = _grid.GetNearestIntersectionPointDisregardingBounds(new Point(currentItemRect.Left, currentTop), GridRestriction.Ignore, Direction.Down);
                        double num9 = pt2.Y.IsEqualTo(currentTop) ? 0.0 : (pt2.Y - currentTop);
                        currentItemRect.Offset(0.0, num9 + (num9.IsGreaterThanOrEqual(0.0) ? 0.0 : _grid.GridCellSize.Height));
                    }
                    _newItemRectMap[item] = currentItemRect;
                }
            }
        });
        IDictionary<IWhiteboardItem, Vector> itemsOffsetsMap = new Dictionary<IWhiteboardItem, Vector>();
        itemsToSeparate.Each(item =>
        {
            Rect val = _oldItemRectMap[item];
            Rect val2 = _newItemRectMap[item];
            if (val != val2)
            {
                Vector value = new Vector(val2.Left - val.Left, val2.Top - val.Top);
                itemsOffsetsMap.Add(item, value);
            }
        });
        return itemsOffsetsMap;
    }
}
