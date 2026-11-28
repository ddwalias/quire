#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

internal partial class DockHintWindow
{
    [MirrorVerified("Release IL")]
    internal UIElement GetElementToAdorn() => base.GetTemplateChild("PART_ContentPresenter") as UIElement;

    [MirrorVerified("Release IL")]
    public DockHintWindow() { }
}
