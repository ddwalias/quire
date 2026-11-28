#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class QatBorder
{
    [MirrorVerified("Release IL")]
    private static bool IsZero(double value) => Math.Abs(value) < 2.220446049250313E-15;

    [MirrorVerified("Release IL")]
    public QatBorder() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => base.SetValue(BackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderBrush { get => (Brush)base.GetValue(BorderBrushProperty); set => base.SetValue(BorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LightBorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush LightBorderBrush { get => (Brush)base.GetValue(LightBorderBrushProperty); set => base.SetValue(LightBorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderThicknessProperty;

    [MirrorVerified("Release IL")]
    public Thickness BorderThickness { get => (Thickness)base.GetValue(BorderThicknessProperty); set => base.SetValue(BorderThicknessProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PaddingProperty;

    [MirrorVerified("Release IL")]
    public Thickness Padding { get => (Thickness)base.GetValue(PaddingProperty); set => base.SetValue(PaddingProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsBorderVisibleProperty;

    [MirrorVerified("Release IL")]
    public bool IsBorderVisible { get => (bool)base.GetValue(IsBorderVisibleProperty); set => base.SetValue(IsBorderVisibleProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsBackgroundVisibleProperty;

    [MirrorVerified("Release IL")]
    public bool IsBackgroundVisible { get => (bool)base.GetValue(IsBackgroundVisibleProperty); set => base.SetValue(IsBackgroundVisibleProperty, value); }
}
