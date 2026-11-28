#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonDropDownCommandExtender
{
    [MirrorVerified("Release IL")]
    public ButtonDropDownCommandExtender(string header) : this(header, null, null) { }

    [MirrorVerified("Release IL")]
    public ButtonDropDownCommandExtender(string header, string image) : this(header, image, null) { }

    [MirrorVerified("Release IL")]
    private bool m_SyncIsChecked;

    [MirrorVerified("Release IL")]
    public bool SyncIsChecked { get => m_SyncIsChecked; set => m_SyncIsChecked = value; }

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
