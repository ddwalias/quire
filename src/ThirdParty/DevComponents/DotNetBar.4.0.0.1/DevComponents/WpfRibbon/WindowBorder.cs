#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class WindowBorder
{
    [MirrorVerified("Release IL")]
    public WindowBorder() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TransparentMarginProperty;

    [MirrorVerified("Release IL")]
    public Thickness TransparentMargin { get => (Thickness)base.GetValue(TransparentMarginProperty); set => base.SetValue(TransparentMarginProperty, value); }
}
