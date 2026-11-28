#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

internal partial class AutoHidePopup
{
    [MirrorVerified("Release IL")]
    public AutoHidePopup()
    {
        base.ClipToBounds = true;
        base.Focusable = true;
    }

    [MirrorVerified("Release IL")]
    private void OptionsContextMenuClosed(object sender, RoutedEventArgs e) => base.Focus();

    [MirrorVerified("Release IL")]
    internal void AutoHideCloseAnimationCompleted(object sender, EventArgs e) => CleanupPopup();

    [MirrorVerified("Release IL")]
    private DockWindow m_DockWindow;

    [MirrorVerified("Release IL")]
    public DockWindow DockWindow { get => m_DockWindow; set => m_DockWindow = value; }
}
