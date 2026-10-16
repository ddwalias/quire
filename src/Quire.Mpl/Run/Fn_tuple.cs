#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Fn_tuple
{
    [MirrorVerified("MplRunFn_tupleMechanicalDifferentialTests")]
    private object[] m_args;

    [MirrorVerified("MplRunFn_tupleMechanicalDifferentialTests")]
    public Fn_tuple(object[] ptup, int purity) : base(purity, ptup.Length)
    {
        m_args = ptup;
    }
}
