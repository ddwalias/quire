#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockSiteDesignExtension
{
    [MirrorVerified("Release IL")]
    private DockSite _DockSite;

    [MirrorVerified("Release IL")]
    public DockSiteDesignExtension(DockSite dockSite)
    {
        _DockSite = dockSite;
    }
}
