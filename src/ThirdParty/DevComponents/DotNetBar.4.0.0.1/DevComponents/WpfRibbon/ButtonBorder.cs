#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonBorder
{
    [MirrorVerified("Release IL")]
    private static void OnButtonClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ButtonBorder)d).UpdateVisualState();

    [MirrorVerified("Release IL")]
    private static bool IsThicknessValid(object value) => LayoutHelpers.IsThicknessValid(value);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty RenderBorderProperty;

    [MirrorVerified("Release IL")]
    public bool RenderBorder { get => (bool)base.GetValue(RenderBorderProperty); set => base.SetValue(RenderBorderProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ColorClassProperty;

    [MirrorVerified("Release IL")]
    public string ColorClass { get => (string)base.GetValue(ColorClassProperty); set => base.SetValue(ColorClassProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CornerRadiusProperty;

    [MirrorVerified("Release IL")]
    public CornerRadius CornerRadius { get => (CornerRadius)base.GetValue(CornerRadiusProperty); set => base.SetValue(CornerRadiusProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ButtonStateProperty;

    [MirrorVerified("Release IL")]
    public eButtonRenderingState ButtonState { get => (eButtonRenderingState)base.GetValue(ButtonStateProperty); set => base.SetValue(ButtonStateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => base.SetValue(BackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TopHighlightProperty;

    [MirrorVerified("Release IL")]
    public Brush TopHighlight { get => (Brush)base.GetValue(TopHighlightProperty); set => base.SetValue(TopHighlightProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BottomHighlightProperty;

    [MirrorVerified("Release IL")]
    public Brush BottomHighlight { get => (Brush)base.GetValue(BottomHighlightProperty); set => base.SetValue(BottomHighlightProperty, value); }

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
    private bool m_Animation;

    [MirrorVerified("Release IL")]
    internal bool Animation { get => m_Animation; set => m_Animation = value; }

    partial class RenderOverlayBrushes
    {
        [MirrorVerified("Release IL")]
        public RenderOverlayBrushes() { }
    }
}
