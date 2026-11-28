#nullable disable
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonDropDownCommand
{
    [MirrorVerified("Release IL")]
    public ButtonDropDownCommand(string header, string name, Type ownerType) : this(header, name, string.Empty, string.Empty, ownerType, null) { }

    [MirrorVerified("Release IL")]
    public ButtonDropDownCommand(string header, string name, Type ownerType, InputGestureCollection inputGestures) : this(header, name, string.Empty, string.Empty, ownerType, inputGestures) { }

    [MirrorVerified("Release IL")]
    public ButtonDropDownCommand(string header, string name, string image, Type ownerType) : this(header, name, image, string.Empty, ownerType, null) { }

    [MirrorVerified("Release IL")]
    public ButtonDropDownCommand(string header, string name, string image, Type ownerType, InputGestureCollection inputGestures) : this(header, name, image, string.Empty, ownerType, inputGestures) { }

    [MirrorVerified("Release IL")]
    public ButtonDropDownCommand(string header, string name, string image, string imageSmall, Type ownerType) : this(header, name, image, imageSmall, ownerType, null) { }

    [MirrorVerified("Release IL")]
    private bool m_SyncHeader;

    [MirrorVerified("Release IL")]
    public bool SyncHeader { get => m_SyncHeader; set => m_SyncHeader = value; }

    [MirrorVerified("Release IL")]
    private bool m_SyncImageSource;

    [MirrorVerified("Release IL")]
    public bool SyncImageSource { get => m_SyncImageSource; set => m_SyncImageSource = value; }

    [MirrorVerified("Release IL")]
    private bool m_SyncImageSmallSource;

    [MirrorVerified("Release IL")]
    public bool SyncImageSmallSource { get => m_SyncImageSmallSource; set => m_SyncImageSmallSource = value; }
}
