using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public partial class UIElementManager : IUIElementManager
{
    public bool SetFocusOnItem(IWhiteboardItem item)
    {
        return GetUIElement(item).Focus();
    }

    public Point GetItemLocation(IWhiteboardItem item)
    {
        UIElement uIElement = GetUIElement(item);
        return new Point(Canvas.GetLeft(uIElement), Canvas.GetTop(uIElement));
    }

    public void SetItemLocation(IWhiteboardItem item, Point point)
    {
        UIElement uIElement = GetUIElement(item);
        Canvas.SetLeft(uIElement, point.X);
        Canvas.SetTop(uIElement, point.Y);
    }

    public void ArrangeItem(IWhiteboardItem item, Rect rect)
    {
        GetUIElement(item).Arrange(rect);
    }

    public Size GetItemRenderSize(IWhiteboardItem item)
    {
        return GetUIElement(item).RenderSize;
    }

    public Size GetItemDesiredSize(IWhiteboardItem item)
    {
        return GetUIElement(item).DesiredSize;
    }

    public void SetItemSize(IWhiteboardItem item, Size size)
    {
        FrameworkElement frameworkElement = item as FrameworkElement;
        if (frameworkElement == null)
        {
            return;
        }
        frameworkElement.Width = size.Width;
        frameworkElement.Height = size.Height;
    }

    public void SetRenderTransform(IWhiteboardItem item, Transform transform)
    {
        GetUIElement(item).RenderTransform = transform;
    }

    public void BlockUntilLoaded()
    {
        WpfUtils.DoEvents(Dispatcher.CurrentDispatcher, (DispatcherPriority)6);
    }

    public object GetDependencyPropertyValueOnItem(IWhiteboardItem item, DependencyProperty property)
    {
        if (item is DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(property);
        }
        return null;
    }

    private static UIElement GetUIElement(IWhiteboardItem item)
    {
        return item as UIElement;
    }

    public UIElementManager()
    {
    }
}
