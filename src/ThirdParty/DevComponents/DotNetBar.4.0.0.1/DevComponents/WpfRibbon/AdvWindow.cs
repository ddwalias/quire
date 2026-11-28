#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class AdvWindow
{
    [MirrorVerified("Release IL")]
    public AdvWindow()
    {
        base.WindowStyle = (WindowStyle)0;
    }

    [MirrorVerified("Release IL")]
    private Border _WindowBorder;

    [MirrorVerified("Release IL")]
    private ResizeGrip _ResizeGrip;

    [MirrorVerified("Release IL")]
    public override void OnApplyTemplate()
    {
        _WindowBorder = base.GetTemplateChild("PART_WindowBorder") as Border;
        _ResizeGrip = base.GetTemplateChild("WindowResizeGrip") as ResizeGrip;
        base.OnApplyTemplate();
    }

    [MirrorVerified("Release IL")]
    private Thickness GetResizeBorderThickness() => new Thickness(6.0, 6.0, 6.0, 6.0);

    [MirrorVerified("Release IL")]
    private CornerRadius GetResizeCornerRadius() => new CornerRadius(7.0, 7.0, 7.0, 7.0);

    [MirrorVerified("Release IL")]
    private void ExecuteCloseWindow(object sender, ExecutedRoutedEventArgs e)
    {
        base.Close();
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    private void CanExecuteCloseWindow(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

    [MirrorVerified("Release IL")]
    private void ExecuteMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        base.WindowState = (WindowState)1;
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    private void CanExecuteMinimizeWindow(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = base.WindowState != (WindowState)1;

    [MirrorVerified("Release IL")]
    private void ExecuteMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        base.WindowState = (WindowState)2;
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    private void CanExecuteMaximizeWindow(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = base.WindowState != (WindowState)2;

    [MirrorVerified("Release IL")]
    private void ExecuteRestoreWindow(object sender, ExecutedRoutedEventArgs e)
    {
        base.WindowState = (WindowState)0;
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    private bool _SizingWindow;

    [MirrorVerified("Release IL")]
    private nint WmEnterSizeMove(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        _SizingWindow = true;
        return nint.Zero;
    }

    [MirrorVerified("Release IL")]
    private nint WmExitSizeMove(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        _SizingWindow = false;
        UpdateRegion(true);
        return nint.Zero;
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty EnableSystemCaptionMenuProperty;

    [MirrorVerified("Release IL")]
    public bool EnableSystemCaptionMenu { get => (bool)base.GetValue(EnableSystemCaptionMenuProperty); set => base.SetValue(EnableSystemCaptionMenuProperty, value); }
}
