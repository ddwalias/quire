#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockHint
{
    [MirrorVerified("Release IL")]
    public DockHint() { }

    [MirrorVerified("Release IL")]
    internal DockHint(eDockHintSide dh)
    {
        DockHintSide = dh;
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty DockHintSideProperty;

    [MirrorVerified("Release IL")]
    public eDockHintSide DockHintSide { get => (eDockHintSide)base.GetValue(DockHintSideProperty); set => base.SetValue(DockHintSideProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PartHitTestSizeProperty;

    [MirrorVerified("Release IL")]
    public Size PartHitTestSize { get => (Size)base.GetValue(PartHitTestSizeProperty); set => base.SetValue(PartHitTestSizeProperty, value); }
}
