#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

public partial class DocumentTabBorder
{
    [MirrorVerified("Release IL")]
    public DocumentTabBorder() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => base.SetValue(BackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderBrush { get => (Brush)base.GetValue(BorderBrushProperty); set => base.SetValue(BorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderInnerBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderInnerBrush { get => (Brush)base.GetValue(BorderInnerBrushProperty); set => base.SetValue(BorderInnerBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BottomBorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BottomBorderBrush { get => (Brush)base.GetValue(BottomBorderBrushProperty); set => base.SetValue(BottomBorderBrushProperty, value); }
}
