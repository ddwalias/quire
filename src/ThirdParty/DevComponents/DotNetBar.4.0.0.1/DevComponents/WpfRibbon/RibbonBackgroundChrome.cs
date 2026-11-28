#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonBackgroundChrome
{
    [MirrorVerified("Release IL")]
    private bool IsBrowserWindow(Window w) => w.ToString().Contains("RootBrowserWindow");

    [MirrorVerified("Release IL")]
    private static void WindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RibbonBackgroundChrome)d).OnWindowStateChanged();

    [MirrorVerified("Release IL")]
    private void OnWindowStateChanged()
    {
        UpdateSystemButtonVisibility();
        UpdateTitleLabelAppearance();
        UpdateTotalChromeHeight();
    }

    [MirrorVerified("Release IL")]
    private static void IsGlassEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RibbonBackgroundChrome)d).OnIsGlassEnabledChanged();

    [MirrorVerified("Release IL")]
    private void OnIsGlassEnabledChanged()
    {
        UpdateSystemButtonVisibility();
        UpdateTitleLabelAppearance();
        UpdateContextGroupGlassEffect();
        TitleChromeHeight = GetSystemCaptionHeight();
    }

    [MirrorVerified("Release IL")]
    private void OnTitleChromeHeightChanged(double d)
    {
        UpdateTotalChromeHeight();
        UpdateTitlePanelChromeHeight();
    }

    [MirrorVerified("Release IL")]
    private static void OnSystemButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RibbonBackgroundChrome)d).OnSystemButtonsChanged();

    [MirrorVerified("Release IL")]
    private void OnSystemButtonsChanged()
    {
        UpdateSystemButtonVisibility();
        UpdateTitleLabelAppearance();
    }

    [MirrorVerified("Release IL")]
    private bool SystemTitleElementsVisible => SystemButtons;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty QatRightPositionProperty;

    [MirrorVerified("Release IL")]
    public GridLength QatRightPosition { get => (GridLength)base.GetValue(QatRightPositionProperty); set => base.SetValue(QatRightPositionProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsActiveProperty;

    [MirrorVerified("Release IL")]
    public bool IsActive { get => (bool)base.GetValue(IsActiveProperty); set => base.SetValue(IsActiveProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty WindowStateProperty;

    [MirrorVerified("Release IL")]
    public WindowState WindowState { get => (WindowState)base.GetValue(WindowStateProperty); set => base.SetValue(WindowStateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsGlassEnabledProperty;

    [MirrorVerified("Release IL")]
    public bool IsGlassEnabled { get => (bool)base.GetValue(IsGlassEnabledProperty); set => base.SetValue(IsGlassEnabledProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TitleChromeHeightProperty;

    [MirrorVerified("Release IL")]
    public double TitleChromeHeight { get => (double)base.GetValue(TitleChromeHeightProperty); set => base.SetValue(TitleChromeHeightProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TotalTitleChromeHeightProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey TotalTitleChromeHeightPropertyKey;

    [MirrorVerified("Release IL")]
    public double TotalTitleChromeHeight { get => (double)base.GetValue(TotalTitleChromeHeightProperty); internal set => base.SetValue(TotalTitleChromeHeightPropertyKey, value); }

    [MirrorVerified("Release IL")]
    private double m_TabXPosition;

    [MirrorVerified("Release IL")]
    internal double TabXPosition { get => m_TabXPosition; set => m_TabXPosition = value; }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CloseButtonVisibilityProperty;

    [MirrorVerified("Release IL")]
    public Visibility CloseButtonVisibility { get => (Visibility)base.GetValue(CloseButtonVisibilityProperty); set => base.SetValue(CloseButtonVisibilityProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty RestoreButtonVisibilityProperty;

    [MirrorVerified("Release IL")]
    public Visibility RestoreButtonVisibility { get => (Visibility)base.GetValue(RestoreButtonVisibilityProperty); set => base.SetValue(RestoreButtonVisibilityProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MinimizeButtonVisibilityProperty;

    [MirrorVerified("Release IL")]
    public Visibility MinimizeButtonVisibility { get => (Visibility)base.GetValue(MinimizeButtonVisibilityProperty); set => base.SetValue(MinimizeButtonVisibilityProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MaximizeButtonVisibilityProperty;

    [MirrorVerified("Release IL")]
    public Visibility MaximizeButtonVisibility { get => (Visibility)base.GetValue(MaximizeButtonVisibilityProperty); set => base.SetValue(MaximizeButtonVisibilityProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty WindowBorderThicknessProperty;

    [MirrorVerified("Release IL")]
    public Thickness WindowBorderThickness { get => (Thickness)base.GetValue(WindowBorderThicknessProperty); set => base.SetValue(WindowBorderThicknessProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HorizontalWindowBorderThicknessProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey HorizontalWindowBorderThicknessPropertyKey;

    [MirrorVerified("Release IL")]
    public Thickness HorizontalWindowBorderThickness { get => (Thickness)base.GetValue(HorizontalWindowBorderThicknessProperty); internal set => base.SetValue(HorizontalWindowBorderThicknessPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SystemButtonsProperty;

    [MirrorVerified("Release IL")]
    public bool SystemButtons { get => (bool)base.GetValue(SystemButtonsProperty); set => base.SetValue(SystemButtonsProperty, value); }
}
