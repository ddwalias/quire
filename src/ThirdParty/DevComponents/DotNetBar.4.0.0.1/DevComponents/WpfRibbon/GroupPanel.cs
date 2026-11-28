#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class GroupPanel
{
    [MirrorVerified("Release IL")]
    public override ButtonPanel Copy(bool deepCopy) => CloningMachine.Clone(this, deepCopy);

    [MirrorVerified("Release IL")]
    public GroupPanel() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderBrush { get => (Brush)base.GetValue(BorderBrushProperty); set => base.SetValue(BorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LightBorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush LightBorderBrush { get => (Brush)base.GetValue(LightBorderBrushProperty); set => base.SetValue(LightBorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CornerRadiusProperty;

    [MirrorVerified("Release IL")]
    public CornerRadius CornerRadius { get => (CornerRadius)base.GetValue(CornerRadiusProperty); set => base.SetValue(CornerRadiusProperty, value); }
}
