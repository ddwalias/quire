#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

internal partial class AutoHideAdorner
{
    [MirrorVerified("Release IL")]
    public AutoHideAdorner(DockSite adornedElement) : base(adornedElement) { }

    [MirrorVerified("Release IL")]
    protected override Visual GetVisualChild(int index) => Children[index];

    [MirrorVerified("Release IL")]
    internal static bool IsDoubleFiniteOrNaN(object o) => !double.IsInfinity((double)o);

    [MirrorVerified("Release IL")]
    internal static bool IsDoubleFiniteOrNaN(double d) => !double.IsInfinity(d);

    [MirrorVerified("Release IL")]
    protected override int VisualChildrenCount => Children.Count;
}
