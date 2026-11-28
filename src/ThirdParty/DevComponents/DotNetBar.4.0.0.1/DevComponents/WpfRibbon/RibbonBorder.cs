#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonBorder
{
    [MirrorVerified("Release IL")]
    private static bool IsThicknessValid(object value) => LayoutHelpers.IsThicknessValid(value);

    [MirrorVerified("Release IL")]
    private static void OnColorClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RibbonBorder)d).OnColorClassChanged(e);

    [MirrorVerified("Release IL")]
    private void OnColorClassChanged(DependencyPropertyChangedEventArgs e) => UpdateVisualState();

    [MirrorVerified("Release IL")]
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) => base.OnPropertyChanged(e);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsRibbonPanelProperty;

    [MirrorVerified("Release IL")]
    public bool IsRibbonPanel { get => (bool)base.GetValue(IsRibbonPanelProperty); set => base.SetValue(IsRibbonPanelProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty InnerBorderProperty;

    [MirrorVerified("Release IL")]
    public bool InnerBorder { get => (bool)base.GetValue(InnerBorderProperty); set => base.SetValue(InnerBorderProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AutoAnimationProperty;

    [MirrorVerified("Release IL")]
    public bool AutoAnimation { get => (bool)base.GetValue(AutoAnimationProperty); set => base.SetValue(AutoAnimationProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderThicknessProperty;

    [MirrorVerified("Release IL")]
    public Thickness BorderThickness { get => (Thickness)base.GetValue(BorderThicknessProperty); set => base.SetValue(BorderThicknessProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LightBorderThicknessProperty;

    [MirrorVerified("Release IL")]
    public Thickness LightBorderThickness { get => (Thickness)base.GetValue(LightBorderThicknessProperty); set => base.SetValue(LightBorderThicknessProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderStateProperty;

    [MirrorVerified("Release IL")]
    public eRibbonBarRenderingState BorderState { get => (eRibbonBarRenderingState)base.GetValue(BorderStateProperty); set => base.SetValue(BorderStateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ColorClassProperty;

    [MirrorVerified("Release IL")]
    public string ColorClass { get => (string)base.GetValue(ColorClassProperty); set => base.SetValue(ColorClassProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CornerRadiusProperty;

    [MirrorVerified("Release IL")]
    public CornerRadius CornerRadius { get => (CornerRadius)base.GetValue(CornerRadiusProperty); set => base.SetValue(CornerRadiusProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => base.SetValue(BackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush BorderBrush { get => (Brush)base.GetValue(BorderBrushProperty); set => base.SetValue(BorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TopHighlightProperty;

    [MirrorVerified("Release IL")]
    public Brush TopHighlight { get => (Brush)base.GetValue(TopHighlightProperty); set => base.SetValue(TopHighlightProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LightBorderBrushProperty;

    [MirrorVerified("Release IL")]
    public Brush LightBorderBrush { get => (Brush)base.GetValue(LightBorderBrushProperty); set => base.SetValue(LightBorderBrushProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PaddingProperty;

    [MirrorVerified("Release IL")]
    public Thickness Padding { get => (Thickness)base.GetValue(PaddingProperty); set => base.SetValue(PaddingProperty, value); }

    partial class RenderOverlayBrushes
    {
        [MirrorVerified("Release IL")]
        public RenderOverlayBrushes() { }
    }
}
