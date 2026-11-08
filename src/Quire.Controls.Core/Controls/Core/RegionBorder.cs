#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RegionBorder
{
    private const string PartLeftDragger = "LeftDragger";
    private const string PartRightDragger = "RightDragger";
    private const string PartBottomDragger = "BottomDragger";
    private const string PartBottomLeftDragger = "BottomLeftDragger";
    private const string PartBottomRightDragger = "BottomRightDragger";
    private const string PartTopDragger = "TopDragger";
    private const string PartTopLeftDragger = "TopLeftDragger";
    private const string PartTopRightDragger = "TopRightDragger";
    private const string PartMoveDragger = "MoveDragger";
    private const string PartBorderGrid = "borderGrid";
    private const string PartRootBorder = "rootBorder";
    private const string PartHeader = "header";
    private const string PartBackground = "background";
    private const string PartForeground = "foreground";
    private const string PartBusyClock = "PartBusyClock";

    private static readonly Type _typeOfThis;

    public static readonly RoutedEvent RequestMoveAndResizeEvent;
    public static readonly DependencyProperty DraggerBackgroundProperty;
    public static readonly DependencyProperty LeftDraggerVisibilityProperty;
    public static readonly DependencyProperty RightDraggerVisibilityProperty;
    public static readonly DependencyProperty BottomDraggerVisibilityProperty;
    public static readonly DependencyProperty BottomLeftDraggerVisibilityProperty;
    public static readonly DependencyProperty BottomRightDraggerVisibilityProperty;
    public static readonly DependencyProperty TopDraggerVisibilityProperty;
    public static readonly DependencyProperty TopLeftDraggerVisibilityProperty;
    public static readonly DependencyProperty TopRightDraggerVisibilityProperty;
    public static readonly DependencyProperty MoveDraggerVisibilityProperty;
    public static readonly DependencyProperty ShowBusyProperty;
    public static readonly DependencyProperty PageBackgroundColorProperty;
    public static readonly DependencyProperty IsActiveProperty;
    public static readonly DependencyProperty IsSelectedProperty;
    public static readonly DependencyProperty HeaderProperty;
    public static readonly DependencyProperty ActiveRegionBackgroundOpacityProperty;
    public static readonly DependencyProperty ForceBorderProperty;
    public static readonly DependencyProperty BorderVisibilityProperty;
    public static readonly DependencyProperty RightEdgeVisibilityProperty;
    public static readonly DependencyProperty BottomEdgeVisibilityProperty;
    public static readonly DependencyProperty CornerRadiusProperty;

    static RegionBorder()
    {
        _typeOfThis = typeof(RegionBorder);
        RequestMoveAndResizeEvent = EventManager.RegisterRoutedEvent("RequestMoveAndResize", RoutingStrategy.Bubble, typeof(RequestMoveAndResizeHandler), _typeOfThis);
        DraggerBackgroundProperty = DependencyProperty.Register("DraggerBackground", typeof(Brush), _typeOfThis, new FrameworkPropertyMetadata(OnDraggerBackgroundChanged));
        LeftDraggerVisibilityProperty = DependencyProperty.Register("LeftDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnLeftDraggerVisibilityChanged));
        RightDraggerVisibilityProperty = DependencyProperty.Register("RightDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnRightDraggerVisibilityChanged));
        BottomDraggerVisibilityProperty = DependencyProperty.Register("BottomDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnBottomDraggerVisibilityChanged));
        BottomLeftDraggerVisibilityProperty = DependencyProperty.Register("BottomLeftDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnBottomLeftDraggerVisibilityChanged));
        BottomRightDraggerVisibilityProperty = DependencyProperty.Register("BottomRightDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnBottomRightDraggerVisibilityChanged));
        TopDraggerVisibilityProperty = DependencyProperty.Register("TopDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnTopDraggerVisibilityChanged));
        TopLeftDraggerVisibilityProperty = DependencyProperty.Register("TopLeftDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnTopLeftDraggerVisibilityChanged));
        TopRightDraggerVisibilityProperty = DependencyProperty.Register("TopRightDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnTopRightDraggerVisibilityChanged));
        MoveDraggerVisibilityProperty = DependencyProperty.Register("MoveDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Visible));
        ShowBusyProperty = DependencyProperty.Register("ShowBusy", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnShowBusyChanged));
        PageBackgroundColorProperty = DependencyProperty.Register("PageBackgroundColor", typeof(Color), _typeOfThis);
        IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnActiveSelectedChanged));
        IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnActiveSelectedChanged));
        HeaderProperty = DependencyProperty.Register("Header", typeof(string), _typeOfThis);
        ActiveRegionBackgroundOpacityProperty = DependencyProperty.Register("ActiveRegionBackgroundOpacity", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(0.7));
        ForceBorderProperty = DependencyProperty.Register("ForceBorder", typeof(ForceBorderType), _typeOfThis, new FrameworkPropertyMetadata(OnForceBorderChanged));
        BorderVisibilityProperty = DependencyProperty.Register("BorderVisibility", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnForceBorderChanged));
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        Control.BackgroundProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnRegionBorderBackgroundChanged));
        Control.BorderBrushProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnRegionBorderBrushChanged));
        RightEdgeVisibilityProperty = BorderShape.RightEdgeVisibilityProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnRightEdgeVisibilityChanged));
        CornerRadiusProperty = BorderShape.CornerRadiusProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnCornerRadiusChanged));
    }

    private static void OnRegionBorderBackgroundChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        ((RegionBorder)depObj).UpdateBackgroundAndForeground();
    }

    private static void OnRegionBorderBrushChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        ((RegionBorder)depObj)._regionBorderBrushChanged?.Invoke();
    }

    private static void OnDraggerBackgroundChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        RegionBorder regionBorder = depObj as RegionBorder;
        if (DebugTools.AssertNotNull(regionBorder, null).Passed)
        {
            regionBorder.UpdateDraggersLook();
        }
    }

    private static void OnLeftDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartLeftDragger, (Visibility)arg.NewValue);
    }

    private static void OnRightDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartRightDragger, (Visibility)arg.NewValue);
    }

    private static void OnBottomDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartBottomDragger, (Visibility)arg.NewValue);
    }

    private static void OnBottomLeftDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartBottomLeftDragger, (Visibility)arg.NewValue);
    }

    private static void OnBottomRightDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartBottomRightDragger, (Visibility)arg.NewValue);
    }

    private static void OnTopDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartTopDragger, (Visibility)arg.NewValue);
    }

    private static void OnTopLeftDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartTopLeftDragger, (Visibility)arg.NewValue);
    }

    private static void OnTopRightDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartTopRightDragger, (Visibility)arg.NewValue);
    }

    private static void OnDraggerVisibilityChanged(RegionBorder regionBorder, string name, Visibility newValue)
    {
        if (DebugTools.AssertNotNull(regionBorder, null).Passed && regionBorder.AreDraggersShown)
        {
            BorderDragger borderDragger = regionBorder.FindBorderDraggerByName(name);
            if (borderDragger != null)
            {
                borderDragger.Visibility = newValue;
            }
        }
    }

    private static void OnShowBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RegionBorder regionBorder)
        {
            regionBorder.UpdateBusyState();
            regionBorder.UpdateBorderVisibilityState();
        }
    }

    private static void OnActiveSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RegionBorder regionBorder)
        {
            regionBorder.UpdateActiveState();
            regionBorder.UpdateBorderVisibilityState();
            regionBorder.UpdateZIndex();
        }
    }

    public static void OnForceBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as RegionBorder)?.UpdateBorderVisibilityState();
    }

    private static void OnRightEdgeVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        ((RegionBorder)depObj)._rightEdgeVisibilityChanged?.Invoke();
    }

    private static void OnBottomEdgeVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        ((RegionBorder)depObj)._bottomEdgeVisibilityChanged?.Invoke();
    }

    private static void OnCornerRadiusChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        ((RegionBorder)depObj)._cornerRadiusChanged?.Invoke();
    }

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    private Shape _busyProgressBar;

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    private void HideBusyStateElements() => _busyProgressBar.Visibility = (Visibility)2;

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    public RegionBorder() { }
}
