#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class MplForeignMacro : MplMacro
{
    [MirrorVerified("MplForeignMacroDifferentialTests")]
    private MplEnv m_env;

    [MirrorVerified("MplForeignMacroDifferentialTests")]
    private string m_sig;

    [MirrorVerified("MplForeignMacroDifferentialTests")]
    private MplMacro m_macro;

    [MirrorVerified("MplForeignMacroDifferentialTests")]
    private MplMacro Macro
    {
        get
        {
            if (m_macro != null)
            {
                return m_macro;
            }
            m_macro = m_env.LoadForeignMacro(m_sig);
            return m_macro;
        }
    }

    [MirrorVerified("MplForeignMacroDifferentialTests")]
    public MplForeignMacro(MplEnv env, Sym s, string sig)
        : base(s)
    {
        m_env = env;
        m_sig = sig;
        m_macro = null;
    }

    [MirrorVerified("MplForeignMacroDifferentialTests")]
    public override Sexpr ExpandCall(MplEnv e, Sexpr form)
    {
        return Macro.ExpandCall(e, form);
    }

    [MirrorVerified("MplForeignMacroDifferentialTests")]
    public override Sexpr ExpandRef(MplEnv e, Tok s)
    {
        return Macro.ExpandRef(e, s);
    }
}
