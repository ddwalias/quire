#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

internal partial class EmptyWindow
{
    [MirrorVerified("Release IL")]
    public EmptyWindow()
    {
        base.WindowStyle = (WindowStyle)0;
        base.AllowsTransparency = true;
        base.Background = Brushes.Transparent;
        base.ShowInTaskbar = false;
    }

    [MirrorVerified("Release IL")]
    protected override void OnSourceInitialized(EventArgs e) => base.OnSourceInitialized(e);
}
