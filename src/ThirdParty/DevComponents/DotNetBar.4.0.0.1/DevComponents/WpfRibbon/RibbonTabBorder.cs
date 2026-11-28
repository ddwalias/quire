#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonTabBorder
{
    [MirrorVerified("Release IL")]
    private static bool IsThicknessValid(object value) => LayoutHelpers.IsThicknessValid(value);

    [MirrorVerified("Release IL")]
    private static void OnTabClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RibbonTabBorder)d).UpdateVisualState();

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PaddingProperty;

    [MirrorVerified("Release IL")]
    public Thickness Padding { get => (Thickness)base.GetValue(PaddingProperty); set => base.SetValue(PaddingProperty, value); }

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
    public static readonly DependencyProperty BorderOuterBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderOuterBrush { get => (Brush)base.GetValue(BorderOuterBrushProperty); set => base.SetValue(BorderOuterBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ColorClassProperty;

    [MirrorVerified("Release IL")]
    public string ColorClass { get => (string)base.GetValue(ColorClassProperty); set => base.SetValue(ColorClassProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TabStateProperty;

    [MirrorVerified("Release IL")]
    public eRibbonTabRenderingState TabState { get => (eRibbonTabRenderingState)base.GetValue(TabStateProperty); set => base.SetValue(TabStateProperty, value); }

    [MirrorVerified("Release IL")]
    private bool m_Animation;

    [MirrorVerified("Release IL")]
    internal bool Animation { get => m_Animation; set => m_Animation = value; }

    partial class RenderOverlayBrushes
    {
        [MirrorVerified("Release IL")]
        public RenderOverlayBrushes() { }
    }
}
