#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls.Whiteboard;

public partial interface IUIElementManager
{
    bool SetFocusOnItem(IWhiteboardItem item);

    Point GetItemLocation(IWhiteboardItem item);

    void SetItemLocation(IWhiteboardItem item, Point point);

    void ArrangeItem(IWhiteboardItem item, Rect rect);

    Size GetItemRenderSize(IWhiteboardItem item);

    Size GetItemDesiredSize(IWhiteboardItem item);

    void SetItemSize(IWhiteboardItem item, Size size);

    void SetRenderTransform(IWhiteboardItem item, Transform transform);

    void BlockUntilLoaded();
}
