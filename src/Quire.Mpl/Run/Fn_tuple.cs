#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Fn_tuple
{
    [MirrorAnalyzed("Mpl.Run.Fn_tuple")]
    private object[] m_args;

    [MirrorAnalyzed("Mpl.Run.Fn_tuple")]
    public Fn_tuple(object[] ptup, int purity) : base(purity, ptup.Length)
    {
        m_args = ptup;
    }

    [MirrorAnalyzed("Mpl.Run.Fn_tuple")]
    public override void invVec<R>(out R r, object[] args) => r = (R)(object)FunctionUtilities.concat(m_args, args);
}
