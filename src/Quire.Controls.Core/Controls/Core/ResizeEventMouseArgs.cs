#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ResizeEventMouseArgs
{
    [MirrorVerified("PtcControlsCoreResizeEventMouseArgsMechanicalDifferentialTests")]
    public IResizer Resizer { internal get; set; }
}
