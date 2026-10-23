#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class CloIntFunction
{
    public int m_clo;

    [MirrorVerified("MplRunCloIntFunctionMechanicalDifferentialTests")]
    private CloIntFunction() : base(null, 0) { }
}
