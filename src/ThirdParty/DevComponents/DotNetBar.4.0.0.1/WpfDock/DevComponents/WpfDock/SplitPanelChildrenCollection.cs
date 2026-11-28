#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class SplitPanelChildrenCollection
{
    [MirrorVerified("Release IL")]
    private SplitPanel m_Parent;

    [MirrorVerified("Release IL")]
    public SplitPanelChildrenCollection(SplitPanel parent)
    {
        m_Parent = parent;
    }

    [MirrorVerified("Release IL")]
    protected override void ClearItems()
    {
        m_Parent.OnChildrenClear();
        base.ClearItems();
    }
}
