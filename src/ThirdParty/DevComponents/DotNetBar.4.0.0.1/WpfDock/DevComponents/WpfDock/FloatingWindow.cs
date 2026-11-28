#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class FloatingWindow
{
    [MirrorVerified("Release IL")]
    public FloatingWindow()
    {
        base.WindowStyle = (WindowStyle)3;
        base.ShowInTaskbar = false;
    }

    [MirrorVerified("Release IL")]
    private DockSite m_DockSite;

    [MirrorVerified("Release IL")]
    public DockSite DockSite { get => m_DockSite; internal set => m_DockSite = value; }
}
