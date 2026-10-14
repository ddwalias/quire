#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>Mpl's boolean true, a singleton (docs/mirror/evidence/Mpl.Run.Rts.md).</summary>
public partial class True
{
    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public static True singleton;

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    static True() { singleton = new True(); }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public True() { }

    [MirrorVerified("MplRunHelpersDifferentialTests")]
    public override string ToString() => "True";
}
