#nullable disable
using System;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;

namespace Quire.Controls.Core.Whiteboard;

public partial class ContainerItemManager
{
    private static readonly Type _typeOfThis = typeof(ContainerItemManager);
    public static DependencyProperty MouseEnterHandlerProperty = DependencyProperty.RegisterAttached("MouseEnterHandler", typeof(MouseEventHandler), typeof(IWhiteboardItem));
    public static DependencyProperty DragEnterHandlerProperty = DependencyProperty.RegisterAttached("DragEnterHandler", typeof(DragEventHandler), typeof(IWhiteboardItem));
    public static DependencyProperty WhiteboardItemSizeChangedHandlerProperty = DependencyProperty.RegisterAttached("WhiteboardItemSizeChangedHandler", typeof(WhiteboardItemSizeChangedEventHandler), _typeOfThis);

    public static WhiteboardItemSizeChangedEventHandler GetWhiteboardItemSizeChangedHandler(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return null;
        }
        return (WhiteboardItemSizeChangedEventHandler)depObj.GetValue(WhiteboardItemSizeChangedHandlerProperty);
    }

    public static void SetWhiteboardItemSizeChangedHandler(DependencyObject depObj, WhiteboardItemSizeChangedEventHandler handler)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.SetValue(WhiteboardItemSizeChangedHandlerProperty, handler);
    }

    public static void ClearWhiteboardItemSizeChangedHandler(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.ClearValue(WhiteboardItemSizeChangedHandlerProperty);
    }
}
