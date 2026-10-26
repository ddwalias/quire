#nullable disable
using System;
using System.ComponentModel;
using System.Windows;

namespace Quire.Controls.Core.Whiteboard;

public static partial class WhiteboardDependencyProperties
{
    private static readonly Type _typeOfThis = typeof(WhiteboardDependencyProperties);

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, HandleIsSelectedPropertyChanged));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached("IsActive", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false, HandleIsActivePropertyChanged));

    public static DependencyProperty UserResizableProperty = DependencyProperty.RegisterAttached("UserResizable", typeof(bool), _typeOfThis, new PropertyMetadata(false));

    public static DependencyProperty ShowResizeIndicatorProperty = DependencyProperty.RegisterAttached("ShowResizeIndicator", typeof(bool), _typeOfThis, new PropertyMetadata(false));

    public static readonly DependencyProperty IsRegionBorderVisibleProperty = DependencyProperty.RegisterAttached("IsRegionBorderVisible", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false));

    private static void HandleIsSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
        if (!(bool)eventArgs.NewValue)
        {
            return;
        }

        IWhiteboardItem whiteboardItem = sender as IWhiteboardItem;
        DebugTools.DevAssert(whiteboardItem == null || whiteboardItem.GetCanSelect(), null);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool GetIsSelected(UIElement item)
    {
        if (item != null)
        {
            return (bool)item.GetValue(IsSelectedProperty);
        }
        return false;
    }

    public static void SetIsSelected(UIElement item, bool value)
    {
        if (item != null)
        {
            item.SetValue(IsSelectedProperty, value);
        }
    }

    private static void HandleIsActivePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
        if (!(bool)eventArgs.NewValue)
        {
            return;
        }

        DebugTools.DevAssert(AttachedProperties.GetCanActivate(sender), null);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool GetIsActive(UIElement item)
    {
        if (item != null)
        {
            return (bool)item.GetValue(IsActiveProperty);
        }
        return false;
    }

    public static void SetIsActive(UIElement item, bool value)
    {
        if (item != null)
        {
            item.SetValue(IsActiveProperty, value);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool GetUserResizable(DependencyObject whiteboardItem)
    {
        return (bool)whiteboardItem.GetValue(UserResizableProperty);
    }

    public static void SetUserResizable(DependencyObject whiteboardItem, bool resizable)
    {
        whiteboardItem.SetValue(UserResizableProperty, resizable);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static bool GetShowResizeIndicator(DependencyObject whiteboardItem)
    {
        return (bool)whiteboardItem.GetValue(ShowResizeIndicatorProperty);
    }

    public static void SetShowResizeIndicator(DependencyObject whiteboardItem, bool showResizeIndicator)
    {
        whiteboardItem.SetValue(ShowResizeIndicatorProperty, showResizeIndicator);
    }

    public static bool GetIsRegionBorderVisible(DependencyObject region)
    {
        return (bool)region.GetValue(IsRegionBorderVisibleProperty);
    }

    public static void SetIsRegionBorderVisible(DependencyObject region, bool value)
    {
        region.SetValue(IsRegionBorderVisibleProperty, value);
    }

    public static void ToggleRegionBorderVisibility(DependencyObject region)
    {
        bool isRegionBorderVisible = GetIsRegionBorderVisible(region);
        SetIsRegionBorderVisible(region, !isRegionBorderVisible);
    }
}
