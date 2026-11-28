#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class NavigationPanePanel
{
    [MirrorVerified("Release IL")]
    public NavigationPanePanel()
    {
        base.ClipToBounds = true;
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SmallItemsHeightProperty;

    [MirrorVerified("Release IL")]
    public double SmallItemsHeight { get => (double)base.GetValue(SmallItemsHeightProperty); set => base.SetValue(SmallItemsHeightProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LargeItemsCountProperty;

    [MirrorVerified("Release IL")]
    public int LargeItemsCount { get => (int)base.GetValue(LargeItemsCountProperty); set => base.SetValue(LargeItemsCountProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty EnableCustomizationProperty;

    [MirrorVerified("Release IL")]
    public bool EnableCustomization { get => (bool)base.GetValue(EnableCustomizationProperty); set => base.SetValue(EnableCustomizationProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SmallItemsBackgroundProperty;

    [MirrorVerified("Release IL")]
    public Brush SmallItemsBackground { get => (Brush)base.GetValue(SmallItemsBackgroundProperty); set => base.SetValue(SmallItemsBackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PaneExpandedProperty;

    [MirrorVerified("Release IL")]
    public bool PaneExpanded { get => (bool)base.GetValue(PaneExpandedProperty); set => base.SetValue(PaneExpandedProperty, value); }
}
