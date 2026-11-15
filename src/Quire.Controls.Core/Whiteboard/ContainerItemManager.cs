#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Wpf;
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

    public void SelectItem(IWhiteboardItem item) => SetIsSelectedProperty(item, true);

    public void DeselectItem(IWhiteboardItem item) => SetIsSelectedProperty(item, null);

    public bool IsItemSelected(IWhiteboardItem item)
    {
        if (item is DependencyObject dependencyObject)
        {
            bool? isSelected = dependencyObject.GetValue(WhiteboardDependencyProperties.IsSelectedProperty) as bool?;
            if (!isSelected.HasValue)
            {
                return false;
            }
            return isSelected.Value;
        }
        return false;
    }

    public void ActivateItem(IWhiteboardItem item) => SetActiveItemProperty(item, true);

    public void DeactivateItem(IWhiteboardItem item) => SetActiveItemProperty(item, false);

    public bool IsItemActivated(IWhiteboardItem item)
    {
        if (item is DependencyObject dependencyObject)
        {
            bool? isActive = dependencyObject.GetValue(WhiteboardDependencyProperties.IsActiveProperty) as bool?;
            if (!isActive.HasValue)
            {
                return false;
            }
            return isActive.Value;
        }
        return false;
    }

    public void SetIsTabNavigatable(IWhiteboardItem item, bool isTabStop)
    {
        if (item is DependencyObject dependencyObject)
        {
            dependencyObject.SetValue(Control.IsTabStopProperty, isTabStop);
        }
    }

    public bool GetIsTabNavigatable(IWhiteboardItem item)
    {
        if (item is DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(Control.IsTabStopProperty);
        }
        return false;
    }

    public bool IsTabNavigatable(IWhiteboardItem item)
    {
        if (item is IWhiteboardItemContainer container)
        {
            return container.IsTabNavigatable;
        }
        return GetIsTabNavigatable(item);
    }

    public void TabInto(IWhiteboardItem item, FocusNavigationDirection dir)
    {
        if (item is IWhiteboardItemContainer container)
        {
            container.TabInto(dir);
        }
        else
        {
            item.NavigateIntoItem(dir == FocusNavigationDirection.Previous ? Direction.Left : Direction.Right);
        }
    }

    public void BringIntoView(IWhiteboardItem item, Rect areaOfInterest)
    {
        FrameworkElement frameworkElement = item as FrameworkElement;
        if (frameworkElement == null || !GetIsBringToView(item))
        {
            return;
        }
        frameworkElement.BringIntoView(areaOfInterest);
    }

    public void SetIsBringToView(IWhiteboardItem item, bool value) => AttachedProperties.SetIsBringToView(item as DependencyObject, value);

    public bool GetIsBringToView(IWhiteboardItem item) => AttachedProperties.GetIsBringToView(item as DependencyObject);

    public bool IsSuppressBringIntoView(IWhiteboardItem item) => !GetIsBringToView(item);

    public void SuppressBringIntoViewIfNeededAndExecute(IWhiteboardItem item, Action action, bool suppressBringToView) =>
        AttachedProperties.SuppressBringIntoViewIfNeededAndExecute(item as DependencyObject, action, suppressBringToView);

    public WhiteboardItemSizeChangedEventHandler HookupSizeChanged(IWhiteboardItem item, Action<IWhiteboardItem, Size> sizeChangedHandler)
    {
        WhiteboardItemSizeChangedEventHandler handler = (sender, argsList) => sizeChangedHandler(sender, argsList.Last().NewSize);
        HookupSizeChanged(item, handler);
        return handler;
    }

    public WhiteboardItemSizeChangedEventHandler HookupSizeChanged(IWhiteboardItem item, Action<IWhiteboardItem, Size, Size> sizeChangedHandler)
    {
        WhiteboardItemSizeChangedEventHandler handler = (sender, argsList) => sizeChangedHandler(sender, argsList.First().PreviousSize, argsList.Last().NewSize);
        HookupSizeChanged(item, handler);
        return handler;
    }

    public void HookupSizeChanged(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler handler)
    {
        FrameworkElement frameworkElement = FrameworkElementFacade.GetFrameworkElement(item);
        WhiteboardItemSizeChangedEventHandler combinedHandler = (WhiteboardItemSizeChangedEventHandler)Delegate.Combine(GetItemSizeChangedHandler(item), handler);
        SetItemSizeChangedHandler(item, combinedHandler);
        frameworkElement.SizeChanged -= OnItemSizeChanged;
        frameworkElement.SizeChanged += OnItemSizeChanged;
    }

    public void UnhookSizeChanged(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler sizeChangedHandler)
    {
        FrameworkElement frameworkElement = FrameworkElementFacade.GetFrameworkElement(item);
        WhiteboardItemSizeChangedEventHandler remaining = GetItemSizeChangedHandler(item) - sizeChangedHandler;
        if (remaining == null)
        {
            ClearItemSizeChangedHandler(item);
            frameworkElement.SizeChanged -= OnItemSizeChanged;
        }
        else
        {
            SetItemSizeChangedHandler(item, remaining);
        }
    }

    protected virtual void OnItemSizeChanged(object sender, SizeChangedEventArgs args)
    {
        IWhiteboardItem whiteboardItem = sender as IWhiteboardItem;
        if (DebugTools.AssertNotNull(whiteboardItem, null).Failed)
        {
            return;
        }
        if (IsSizeChangedEventHandlerProhibited(whiteboardItem))
        {
            return;
        }
        WhiteboardItemSizeChangedEventHandler itemSizeChangedHandler = GetItemSizeChangedHandler(whiteboardItem);
        if (DebugTools.AssertNotNull(itemSizeChangedHandler, null).Passed)
        {
            itemSizeChangedHandler(whiteboardItem, new List<WhiteboardItemSizeChangedEventArgs> { new WhiteboardItemSizeChangedEventArgs(args) });
        }
    }

    protected virtual WhiteboardItemSizeChangedEventHandler GetItemSizeChangedHandler(IWhiteboardItem item) =>
        GetWhiteboardItemSizeChangedHandler(FrameworkElementFacade.GetFrameworkElement(item));

    protected virtual void SetItemSizeChangedHandler(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler handler) =>
        SetWhiteboardItemSizeChangedHandler(FrameworkElementFacade.GetFrameworkElement(item), handler);

    protected virtual void ClearItemSizeChangedHandler(IWhiteboardItem item) =>
        ClearWhiteboardItemSizeChangedHandler(FrameworkElementFacade.GetFrameworkElement(item));

    public void HookupMouseEnter(IWhiteboardItem item, Action<IWhiteboardItem> mouseEnterHandler)
    {
        UIElement uiElement = UIElementFacade.GetUIElement(item);
        MouseEventHandler mouseEventHandler = (sender, args) =>
        {
            args.Handled = true;
            mouseEnterHandler(item);
        };
        ExecuteOnLoad(item, () =>
        {
            uiElement.SetValue(MouseEnterHandlerProperty, mouseEventHandler);
            uiElement.MouseEnter += mouseEventHandler;
        });
    }

    public void HookupLeftMouseButtonDown(IWhiteboardItem item, Action<IWhiteboardItem> handler)
    {
        UIElement uiElement = UIElementFacade.GetUIElement(item);
        MouseButtonEventHandler mouseButtonEventHandler = (sender, args) =>
        {
            args.Handled = true;
            handler(item);
        };
        uiElement.MouseLeftButtonDown += mouseButtonEventHandler;
    }

    public void UnhookMouseEnter(IWhiteboardItem item)
    {
        UIElement uiElement = UIElementFacade.GetUIElement(item);
        MouseEventHandler mouseEventHandler = uiElement.GetValue(MouseEnterHandlerProperty) as MouseEventHandler;
        if (mouseEventHandler == null)
        {
            return;
        }
        uiElement.ClearValue(MouseEnterHandlerProperty);
        uiElement.MouseEnter -= mouseEventHandler;
    }

    public void HookupDragEnter(IWhiteboardItem item, Action<IWhiteboardItem> dragEnterHandler)
    {
        UIElement uiElement = UIElementFacade.GetUIElement(item);
        DragEventHandler dragEventHandler = (sender, args) =>
        {
            args.Handled = true;
            dragEnterHandler(item);
        };
        ExecuteOnLoad(item, () =>
        {
            uiElement.SetValue(DragEnterHandlerProperty, dragEventHandler);
            uiElement.DragEnter += dragEventHandler;
        });
    }

    public void UnhookDragEnter(IWhiteboardItem item)
    {
        UIElement uiElement = UIElementFacade.GetUIElement(item);
        DragEventHandler dragEventHandler = uiElement.GetValue(DragEnterHandlerProperty) as DragEventHandler;
        if (dragEventHandler == null)
        {
            return;
        }
        uiElement.ClearValue(DragEnterHandlerProperty);
        uiElement.DragEnter -= dragEventHandler;
    }

    public void ProhibitSizeChangedEventHandler(IWhiteboardItem item, bool prohibit) =>
        AttachedProperties.SetProhibitSizeChangedEventHandler(FrameworkElementFacade.GetFrameworkElement(item), prohibit);

    public bool IsSizeChangedEventHandlerProhibited(IWhiteboardItem item) =>
        AttachedProperties.GetProhibitSizeChangedEventHandler(FrameworkElementFacade.GetFrameworkElement(item));

    public bool GetItemHandlesContextMenus(IWhiteboardItem item)
    {
        bool result = false;
        DependencyObject dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject, null).Passed)
        {
            result = AttachedProperties.GetHandlesContextMenu(dependencyObject);
        }
        return result;
    }

    public void SetItemHandlesContextMenus(IWhiteboardItem item, bool handlesContextMenus)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject, null).Passed)
        {
            AttachedProperties.SetHandlesContextMenu(dependencyObject, handlesContextMenus);
        }
    }

    public void ClearItemHandlesContextMenus(IWhiteboardItem item)
    {
        DependencyObject dependencyObject = item as DependencyObject;
        if (DebugTools.AssertNotNull(dependencyObject, null).Passed)
        {
            AttachedProperties.ClearHandlesContextMenu(dependencyObject);
        }
    }

    public bool GetCanActivate(IWhiteboardItem item) => AttachedProperties.GetCanActivate(item as DependencyObject);

    public void SetCanActivate(IWhiteboardItem item, bool value) => AttachedProperties.SetCanActivate(item as DependencyObject, value);

    public bool GetItemIsInCopy(IWhiteboardItem item) => DocumentAttachedProperties.GetIsInCopyNonSerializable(UIElementFacade.GetUIElement(item));

    public void SetItemIsInCopy(IWhiteboardItem item, bool value) => DocumentAttachedProperties.SetIsInCopy(UIElementFacade.GetUIElement(item), value);

    public void ClearItemIsInCopy(IWhiteboardItem item) => DocumentAttachedProperties.ClearIsInCopy(UIElementFacade.GetUIElement(item));

    private void SetIsSelectedProperty(IWhiteboardItem item, bool? isSelected)
    {
        DependencyObject itemObject = item as DependencyObject;
        if (itemObject == null)
        {
            return;
        }
        Action action = () =>
        {
            if (!isSelected.HasValue)
            {
                itemObject.ClearValue(WhiteboardDependencyProperties.IsSelectedProperty);
            }
            else
            {
                itemObject.SetValue(WhiteboardDependencyProperties.IsSelectedProperty, isSelected.Value);
            }
        };
        ExecuteOnLoad(item, action);
    }

    private void SetActiveItemProperty(IWhiteboardItem item, bool isActive)
    {
        if (item is DependencyObject dependencyObject)
        {
            dependencyObject.SetValue(WhiteboardDependencyProperties.IsActiveProperty, isActive);
        }
    }

    public void SetItemSupportsInputsOutputs(IWhiteboardItem item, bool visible) =>
        AttachedProperties.SetSupportsInputsOutputs(FrameworkElementFacade.GetFrameworkElement(item), visible);

    public bool GetItemSupportsInputsOutputs(IWhiteboardItem item) =>
        AttachedProperties.GetSupportsInputsOutputs(FrameworkElementFacade.GetFrameworkElement(item));

    public void ClearItemSupportsInputsOutputs(IWhiteboardItem item) =>
        AttachedProperties.ClearSupportsInputsOutputs(FrameworkElementFacade.GetFrameworkElement(item));

    public void SetItemIsOnVisiblePage(IWhiteboardItem item, bool visible) =>
        AttachedProperties.SetIsOnVisiblePage(FrameworkElementFacade.GetFrameworkElement(item), visible);

    public bool GetItemIsOnVisiblePage(IWhiteboardItem item) =>
        AttachedProperties.GetIsOnVisiblePage(FrameworkElementFacade.GetFrameworkElement(item));

    public void ClearItemIsOnVisiblePage(IWhiteboardItem item) =>
        AttachedProperties.ClearIsOnVisiblePage(FrameworkElementFacade.GetFrameworkElement(item));

    public void SetElementIsOnVisiblePage(Quire.Core.IMaybeHasPointThatSnapsToPixel item, bool isPageVisible) =>
        AttachedProperties.SetIsOnVisiblePage(item.SnappingHelper.ItemElement, isPageVisible);

    public bool GetElementIsOnVisiblePage(Quire.Core.IMaybeHasPointThatSnapsToPixel item) =>
        AttachedProperties.GetIsOnVisiblePage(item.SnappingHelper.ItemElement);

    public void AddItemIsOnVisiblePageChangedHandler(IWhiteboardItem item, DependencyPropertyChangedEventHandler handler) =>
        DependencyPropertyChangeManager.AddPropertyChangedHandler(AttachedProperties.IsOnVisiblePageProperty, FrameworkElementFacade.GetFrameworkElement(item), handler);

    public void RemoveItemIsOnVisiblePageChangedHandler(IWhiteboardItem item, DependencyPropertyChangedEventHandler handler) =>
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(AttachedProperties.IsOnVisiblePageProperty, FrameworkElementFacade.GetFrameworkElement(item), handler);

    public void SetContainedInCollapsedWhiteboard(IWhiteboardItem item, bool isContainedInCollapsedWhiteboard) =>
        AttachedProperties.SetContainedInCollapsedWhiteboard(UIElementFacade.GetUIElement(item), isContainedInCollapsedWhiteboard);

    public bool GetContainedInCollapsedWhiteboard(IWhiteboardItem item) =>
        AttachedProperties.GetContainedInCollapsedWhiteboard(UIElementFacade.GetUIElement(item));

    public void ClearContainedInCollapsedWhiteboard(IWhiteboardItem item) =>
        AttachedProperties.ClearContainedInCollapsedWhiteboard(UIElementFacade.GetUIElement(item));

    public ContainerItemManager() { }
}
