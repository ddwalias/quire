#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonWindow
{
    [MirrorVerified("Release IL")]
    private bool m_IsGlassEnabled;

    [MirrorVerified("Release IL")]
    public RibbonWindow()
    {
        m_IsGlassEnabled = GetGlassEnabled();
    }

    [MirrorVerified("Release IL")]
    private Thickness GetResizeBorderThickness() => new Thickness(5.0, 5.0, 5.0, 5.0);

    [MirrorVerified("Release IL")]
    private CornerRadius GetResizeCornerRadius() => new CornerRadius(6.0, 6.0, 6.0, 6.0);

    [MirrorVerified("Release IL")]
    private nint WmDwmCompositionChanged(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        m_IsGlassEnabled = GetGlassEnabled();
        UpdateWindowStyle();
        return nint.Zero;
    }

    [MirrorVerified("Release IL")]
    private nint WmEnterSizeMove(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        SizingWindow = true;
        return nint.Zero;
    }

    [MirrorVerified("Release IL")]
    private nint WmExitSizeMove(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        SizingWindow = false;
        UpdateRegion(true);
        return nint.Zero;
    }

    [MirrorVerified("Release IL")]
    private bool GetGlassEnabled() => WinApi.IsGlassEnabled;

    [MirrorVerified("Release IL")]
    private void OnEnableGlassChanged(bool oldValue, bool newValue) => UpdateWindowStyle();

    [MirrorVerified("Release IL")]
    protected override Size MeasureOverride(Size availableSize) => base.MeasureOverride(availableSize);

    [MirrorVerified("Release IL")]
    protected override Size ArrangeOverride(Size arrangeBounds) => base.ArrangeOverride(arrangeBounds);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty EnableGlassProperty;

    [MirrorVerified("Release IL")]
    public bool EnableGlass { get => (bool)base.GetValue(EnableGlassProperty); set => base.SetValue(EnableGlassProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsUsingGlassProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsUsingGlassPropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsUsingGlass { get => (bool)base.GetValue(IsUsingGlassProperty); internal set => base.SetValue(IsUsingGlassPropertyKey, value); }
}
