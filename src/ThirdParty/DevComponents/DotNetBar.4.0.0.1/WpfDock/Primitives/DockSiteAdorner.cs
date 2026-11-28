#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

internal partial class DockSiteAdorner
{
    [MirrorVerified("Release IL")]
    protected override Visual GetVisualChild(int index) => Children[index];

    [MirrorVerified("Release IL")]
    private UIElement _RealAdornedElement;

    [MirrorVerified("Release IL")]
    public UIElement RealAdornedElement { get => _RealAdornedElement; set => _RealAdornedElement = value; }

    [MirrorVerified("Release IL")]
    protected override int VisualChildrenCount => Children.Count;
}
