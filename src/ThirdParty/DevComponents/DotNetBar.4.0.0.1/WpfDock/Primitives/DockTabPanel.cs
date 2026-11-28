#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

public partial class DockTabPanel
{
    [MirrorVerified("Release IL")]
    private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockTabPanel)d).OnBorderBrushChanged();

    [MirrorVerified("Release IL")]
    private static void OnBorderInnerBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockTabPanel)d).OnBorderInnerBrushChanged();

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderInnerBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderInnerBrush { get => (Brush)base.GetValue(BorderInnerBrushProperty); set => base.SetValue(BorderInnerBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderBrush { get => (Brush)base.GetValue(BorderBrushProperty); set => base.SetValue(BorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundInnerProperty;

    [MirrorVerified("Release IL")]
    public Brush BackgroundInner { get => (Brush)base.GetValue(BackgroundInnerProperty); set => base.SetValue(BackgroundInnerProperty, value); }
}
