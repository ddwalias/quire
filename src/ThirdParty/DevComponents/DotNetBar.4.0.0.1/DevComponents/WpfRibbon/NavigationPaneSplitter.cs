#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class NavigationPaneSplitter
{
    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PanePanelProperty;

    [MirrorVerified("Release IL")]
    public NavigationPanePanel PanePanel { get => (NavigationPanePanel)base.GetValue(PanePanelProperty); set => base.SetValue(PanePanelProperty, value); }
}
