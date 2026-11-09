#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
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
}
