#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class CollapsedRibbonBarButton
{
    [MirrorVerified("Release IL")]
    public CollapsedRibbonBarButton() { }

    [MirrorVerified("Release IL")]
    protected override bool HasVisibleItems => AttachedRibbonBar != null;

    [MirrorVerified("Release IL")]
    internal CollapsedRibbonBarPanel CollapsedRibbonBarPanel => base.GetTemplateChild("PART_CollapsedRibbonBarPanel") as CollapsedRibbonBarPanel;
}
