#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonContentPanel
{
    [MirrorVerified("Release IL")]
    protected override void OnInitialized(EventArgs e)
    {
        CreateBackgroundChrome();
        base.OnInitialized(e);
    }

    [MirrorVerified("Release IL")]
    private RibbonBackgroundChrome m_BackChrome;

    [MirrorVerified("Release IL")]
    internal RibbonBackgroundChrome BackgroundChrome => m_BackChrome;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty WindowBorderThicknessProperty;

    [MirrorVerified("Release IL")]
    public Thickness WindowBorderThickness { get => (Thickness)base.GetValue(WindowBorderThicknessProperty); set => base.SetValue(WindowBorderThicknessProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsGlassEnabledProperty;

    [MirrorVerified("Release IL")]
    public bool IsGlassEnabled { get => (bool)base.GetValue(IsGlassEnabledProperty); set => base.SetValue(IsGlassEnabledProperty, value); }
}
