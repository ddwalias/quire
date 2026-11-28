#nullable disable
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonBarPanel
{
    [MirrorVerified("Release IL")]
    protected override void OnInitialized(EventArgs e)
    {
        KeyboardNavigation.SetTabNavigation(this, (KeyboardNavigationMode)2);
        KeyboardNavigation.SetDirectionalNavigation(this, (KeyboardNavigationMode)2);
        base.OnInitialized(e);
    }

    [MirrorVerified("Release IL")]
    private int m_HorizontalOffset;

    [MirrorVerified("Release IL")]
    private void SetScrollOffset(int offset)
    {
        m_HorizontalOffset = m_HorizontalOffset + offset;
        base.InvalidateArrange();
        UpdateScrollButtonVisibility();
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
    private bool IsScrollUsed => m_ScrollLeft != null;

    partial class ResizeOrderComparer
    {
        [MirrorVerified("Release IL")]
        public int Compare(RibbonBar x, RibbonBar y) => x.ResizeOrderIndex - y.ResizeOrderIndex;

        [MirrorVerified("Release IL")]
        public ResizeOrderComparer() { }
    }
}
