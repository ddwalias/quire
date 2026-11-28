#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

internal partial class DockSelectorAdorner
{
    [MirrorVerified("Release IL")]
    private DockWindowSelector m_Selector;

    [MirrorVerified("Release IL")]
    public DockSelectorAdorner(UIElement adornedElement) : base(adornedElement)
    {
        m_Selector = new DockWindowSelector();
        base.AddVisualChild(m_Selector);
        base.AddLogicalChild(m_Selector);
    }

    [MirrorVerified("Release IL")]
    public override GeneralTransform GetDesiredTransform(GeneralTransform transform) => null;

    [MirrorVerified("Release IL")]
    protected override int VisualChildrenCount => 1;

    [MirrorVerified("Release IL")]
    public DockWindowSelector DockWindowSelector => m_Selector;
}
