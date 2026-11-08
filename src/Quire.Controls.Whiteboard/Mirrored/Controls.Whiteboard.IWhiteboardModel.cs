#nullable disable
#pragma warning disable

using System;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Whiteboard;

public partial interface IWhiteboardModel
{
    WhiteboardItemsCollection WhiteboardItems { get; }

    IWhiteboardItemPositionComparer PositionComparer { get; }

    void InsertItemAtPosition(IWhiteboardItem item, int position);

    void RemoveItem(IWhiteboardItem item);

    void AddItemAtLocation(IWhiteboardItem item, Point location);

    void SetItemLocation(IWhiteboardItem item, Point location);
}
