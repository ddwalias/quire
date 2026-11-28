#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockSplitter
{
    [MirrorVerified("Release IL")]
    public DockSplitter() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty OrientationProperty;

    [MirrorVerified("Release IL")]
    public Orientation Orientation { get => (Orientation)base.GetValue(OrientationProperty); set => base.SetValue(OrientationProperty, value); }

    [MirrorVerified("Release IL")]
    private UIElement m_LeftControl;

    [MirrorVerified("Release IL")]
    public UIElement LeftControl { get => m_LeftControl; internal set => m_LeftControl = value; }

    [MirrorVerified("Release IL")]
    private UIElement m_RightControl;

    [MirrorVerified("Release IL")]
    public UIElement RightControl { get => m_RightControl; internal set => m_RightControl = value; }
}
