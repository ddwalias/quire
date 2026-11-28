#nullable disable
using System.Windows.Controls.Primitives;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonTabPanel
{
    [MirrorVerified("Release IL")]
    private int m_HorizontalOffset;

    [MirrorVerified("Release IL")]
    private bool m_RaiseTabsScrolledEvent;

    [MirrorVerified("Release IL")]
    private void SetScrollOffset(int offset)
    {
        m_HorizontalOffset = m_HorizontalOffset + offset;
        base.InvalidateArrange();
        UpdateScrollButtonVisibility();
        m_RaiseTabsScrolledEvent = true;
    }

    [MirrorVerified("Release IL")]
    private RepeatButton m_ScrollLeft;

    [MirrorVerified("Release IL")]
    private RepeatButton m_ScrollRight;

    [MirrorVerified("Release IL")]
    private void DestroyScrollButtons()
    {
        m_HorizontalOffset = 0;
        RemoveButton(m_ScrollLeft);
        RemoveButton(m_ScrollRight);
        m_ScrollRight = null;
        m_ScrollLeft = null;
    }

    [MirrorVerified("Release IL")]
    public RibbonTabPanel() { }

    [MirrorVerified("Release IL")]
    private bool IsScrollUsed => m_ScrollLeft != null;
}
