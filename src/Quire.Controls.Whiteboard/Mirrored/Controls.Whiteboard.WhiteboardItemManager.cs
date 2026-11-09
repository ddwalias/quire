#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Ole;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Services;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardItemManager : ContainerItemManager, IWhiteboardItemManager, IContainerItemManager, IFrameworkElementFacade, IUIElementFacade, IService, IUIElementArrangeInfoProvider
{
    public bool GetItemIsInPasting(IWhiteboardItem item)
    {
        return DocumentAttachedProperties.GetIsInPasting(UIElementFacade.GetUIElement(item));
    }

    public void SetItemIsInPasting(IWhiteboardItem item, bool value)
    {
        DocumentAttachedProperties.SetIsInPasting(UIElementFacade.GetUIElement(item), value);
    }

    public void ClearItemIsInPasting(IWhiteboardItem item)
    {
        DocumentAttachedProperties.ClearIsInPasting(UIElementFacade.GetUIElement(item));
    }

    public bool GetItemIsInPrinting(IWhiteboardItem item)
    {
        return DocumentAttachedProperties.GetIsInPrinting(UIElementFacade.GetUIElement(item));
    }

    public void SetItemIsInPrinting(IWhiteboardItem item, bool value)
    {
        DocumentAttachedProperties.SetIsInPrinting(UIElementFacade.GetUIElement(item), value);
    }

    public void ClearItemIsInPrinting(IWhiteboardItem item)
    {
        DocumentAttachedProperties.ClearIsInPrinting(UIElementFacade.GetUIElement(item));
    }

    public Point GetItemLocation(IWhiteboardItem item)
    {
        UIElement uIElement = UIElementFacade.GetUIElement(item);
        return new Point(Canvas.GetLeft(uIElement), Canvas.GetTop(uIElement));
    }

    public Point GetItemLocation(IWhiteboardItem item, Func<UIElement, double> getItemLeft, Func<UIElement, double> getItemTop)
    {
        return GetItemLocation(UIElementFacade.GetUIElement(item), getItemLeft, getItemTop);
    }

    public Point GetItemLocation(IWorksheetPageBreak pageBreak, Func<UIElement, double> getItemLeft, Func<UIElement, double> getItemTop)
    {
        return GetItemLocation(UIElementFacade.GetUIElement(pageBreak), getItemLeft, getItemTop);
    }

    public Point GetItemLocation(UIElement element, Func<UIElement, double> getItemLeft, Func<UIElement, double> getItemTop)
    {
        Point result = default;
        if (element == null)
        {
            return result;
        }
        double num = getItemLeft(element);
        double num2 = getItemTop(element);
        if (double.IsNaN(num) || double.IsNaN(num2))
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
        else
        {
            result = new Point(num, num2);
        }
        return result;
    }

    public void SetItemLocation(IWhiteboardItem item, Point point)
    {
        UIElement uIElement = UIElementFacade.GetUIElement(item);
        Canvas.SetLeft(uIElement, point.X);
        Canvas.SetTop(uIElement, point.Y);
        IWhiteboardItemPresenter itemPresenter = GetItemPresenter(item);
        if (itemPresenter != null)
        {
            itemPresenter.HandleItemLocationChanged(point);
        }
    }

    protected IWhiteboardItemPresenter GetItemPresenter(IWhiteboardItem item)
    {
        UIElement uIElement = UIElementFacade.GetUIElement(item);
        if (uIElement != null)
        {
            return VisualTreeHelper.GetParent(uIElement) as IWhiteboardItemPresenter;
        }
        return null;
    }

    public bool ShouldItemBeDeletedIfEmpty(IWhiteboardItem item)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (dependencyObject != null)
        {
            return (bool)dependencyObject.GetValue(WhiteboardControl.DeleteIfEmptyProperty);
        }
        return false;
    }

    public void SetShouldItemBeDeletedIfEmpty(IWhiteboardItem item, bool value)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (dependencyObject != null)
        {
            dependencyObject.SetValue(WhiteboardControl.DeleteIfEmptyProperty, value);
        }
    }

    public void ClearShouldItemBeDeletedIfEmpty(IWhiteboardItem item)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (dependencyObject != null)
        {
            dependencyObject.ClearValue(WhiteboardControl.DeleteIfEmptyProperty);
        }
    }

    public bool IsResizingItem(IWhiteboardItem item)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (dependencyObject != null)
        {
            bool? flag = dependencyObject.GetValue(ResizableElementAttachedProperties.IsResizingProperty) as bool?;
            if (!flag.HasValue)
            {
                return false;
            }
            return flag.Value;
        }
        return false;
    }

    public bool GetIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item)
    {
        return item is IOleContainer || item is IPageWideSizable;
    }

    public bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        if (items.IsNotEmpty(out var list))
        {
            Dictionary<IWhiteboardItem, Rect> dictionary = new Dictionary<IWhiteboardItem, Rect>();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                IWhiteboardItem whiteboardItem = list[i];
                Rect itemRect = GetItemRect(whiteboardItem);
                dictionary.Add(whiteboardItem, itemRect);
                for (int num = i - 1; num >= 0; num--)
                {
                    IWhiteboardItem key = list[num];
                    Rect rect = dictionary[key];
                    if (itemRect.IntersectsWithNonZeroArea(rect))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public virtual Rect GetItemRect(IWhiteboardItem item)
    {
        Point itemLocation = GetItemLocation(item);
        itemLocation.Offset(0.0, item.VerticalAlignmentOffset);
        return new Rect(itemLocation, GetRenderSize(item));
    }

    public double GetItemTop(IWhiteboardItem item)
    {
        Rect itemRect = GetItemRect(item);
        return itemRect.Top;
    }

    public double GetItemBottom(IWhiteboardItem item)
    {
        Rect itemRect = GetItemRect(item);
        return itemRect.Bottom;
    }

    public virtual IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending)
    {
        IEnumerable<IWhiteboardItem> enumerable = items.OrderBy(item => GetItemLocation(item).Y).ThenBy(item => GetItemLocation(item).X);
        if (descending)
        {
            enumerable = enumerable.Reverse();
        }
        return enumerable;
    }

    public WhiteboardItemManager()
    {
    }
}
