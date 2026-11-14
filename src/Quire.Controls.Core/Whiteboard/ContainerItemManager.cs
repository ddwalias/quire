#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
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

    public void SetIsBringToView(IWhiteboardItem item, bool value) => AttachedProperties.SetIsBringToView(item as DependencyObject, value);

    public bool GetIsBringToView(IWhiteboardItem item) => AttachedProperties.GetIsBringToView(item as DependencyObject);

    public bool IsSuppressBringIntoView(IWhiteboardItem item) => !GetIsBringToView(item);
}
