#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Fn_papp : Function
{
    [MirrorAnalyzed("Mpl.Run.Fn_papp")]
    private object[] m_args;

    [MirrorAnalyzed("Mpl.Run.Fn_papp")]
    private Function m_f;

    [MirrorVerified("FnPappDifferentialTests")]
    public override string Name => string.Format("{0}{1}", m_f.Name, format_vector(m_args));

    [MirrorVerified("FnPappDifferentialTests")]
    public Fn_papp(Function f, object[] args)
        : base(f.Purity, args.Length)
    {
        m_f = f;
        m_args = args;
        m_varargs = f.Varargs;
    }

    [MirrorVerified("FnPappDifferentialTests")]
    public override void invVec<R>(out R r, object[] args)
    {
        m_f.invVec<R>(out r, FunctionUtilities.concat(m_args, args));
    }
}
