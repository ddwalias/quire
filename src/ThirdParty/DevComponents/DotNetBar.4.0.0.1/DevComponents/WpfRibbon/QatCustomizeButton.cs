#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class QatCustomizeButton
{
    [MirrorVerified("Release IL")]
    public QatCustomizeButton()
    {
        Ribbon.SetIsCustomizable(this, false);
    }
}
