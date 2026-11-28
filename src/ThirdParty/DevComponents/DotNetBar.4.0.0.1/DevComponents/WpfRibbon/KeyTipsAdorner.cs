#nullable disable
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

internal partial class KeyTipsAdorner
{
    [MirrorVerified("Release IL")]
    private Control m_RootControl;

    [MirrorVerified("Release IL")]
    public Control RootControl { get => m_RootControl; set => m_RootControl = value; }
}
