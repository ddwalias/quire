#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class GalleryWrapPanel
{
    [MirrorVerified("Release IL")]
    protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) => new UIElementCollection(this, null);

    [MirrorVerified("Release IL")]
    public GalleryWrapPanel() { }
}
