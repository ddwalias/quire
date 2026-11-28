#nullable disable
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class QatPanel
{
    [MirrorVerified("Release IL")]
    public QatPanel() { }

    [MirrorVerified("Release IL")]
    private Qat Qat => base.TemplatedParent as Qat;
}
