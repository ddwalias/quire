#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class CloObjFunction
{
    public object m_clo;

    [MirrorVerified("MplRunCloObjFunctionMechanicalDifferentialTests")]
    private CloObjFunction() : base(null, 0) { }
}
