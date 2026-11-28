#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class AutoHidePanel
{
    [MirrorVerified("Release IL")]
    public AutoHidePanel() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TabOrientationProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey TabOrientationPropertyKey;

    [MirrorVerified("Release IL")]
    public Orientation TabOrientation { get => (Orientation)base.GetValue(TabOrientationProperty); internal set => base.SetValue(TabOrientationPropertyKey, value); }
}
