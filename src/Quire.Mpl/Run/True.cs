#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>Mpl's boolean true, a singleton (docs/mirror/evidence/Mpl.Run.Rts.md).</summary>
public partial class True
{
    [MirrorVerified("RtsDifferentialTests")]
    public static True singleton;

    [MirrorVerified("RtsDifferentialTests")]
    static True() { singleton = new True(); }

    [MirrorVerified("RtsDifferentialTests")]
    public True() { }

    [MirrorVerified("RtsDifferentialTests")]
    public override string ToString() => "True";
}
