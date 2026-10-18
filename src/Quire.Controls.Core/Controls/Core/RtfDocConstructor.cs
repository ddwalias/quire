#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RtfDocConstructor
{
    [MirrorVerified("PtcControlsCoreRtfDocConstructorMechanicalDifferentialTests")]
    public static bool IsRtfCopyEnabled { get; set; }

    [MirrorVerified("PtcControlsCoreRtfDocConstructorMechanicalDifferentialTests")]
    private bool IsOnePageContent { get; set; }
}
