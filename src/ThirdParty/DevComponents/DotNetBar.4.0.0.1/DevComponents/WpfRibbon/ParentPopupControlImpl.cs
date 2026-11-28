#nullable disable
using System.Windows;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

internal partial class ParentPopupControlImpl
{
    [MirrorVerified("Release IL")]
    private FrameworkElement m_Parent;

    [MirrorVerified("Release IL")]
    public ParentPopupControlImpl(FrameworkElement parent)
    {
        m_Parent = parent;
    }

    [MirrorVerified("Release IL")]
    public UIElement Parent => m_Parent;

    [MirrorVerified("Release IL")]
    public bool HasCapture => Mouse.Captured == m_Parent;

    [MirrorVerified("Release IL")]
    internal event EventHandler InternalMenuModeChanged;
}
