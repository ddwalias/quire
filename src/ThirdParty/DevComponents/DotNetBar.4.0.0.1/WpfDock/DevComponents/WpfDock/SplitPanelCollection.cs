#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class SplitPanelCollection
{
    [MirrorVerified("Release IL")]
    public SplitPanelCollection() { }

    [MirrorVerified("Release IL")]
    private DockSite m_Parent;

    [MirrorVerified("Release IL")]
    public SplitPanelCollection(DockSite parent)
    {
        m_Parent = parent;
    }
}
