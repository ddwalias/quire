#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class MplLocalFunctionMacro
{
    [MirrorVerified("MplLocalFunctionMacroDifferentialTests")]
    private MplEnv m_env;

    [MirrorVerified("MplLocalFunctionMacroDifferentialTests")]
    private uint m_handle;

    [MirrorVerified("MplLocalFunctionMacroDifferentialTests")]
    private Sexpr m_lambda;

    [MirrorVerified("MplLocalFunctionMacroDifferentialTests")]
    public MplLocalFunctionMacro(MplEnv env, Sym s, uint handle, Sexpr lambda) : base(s)
    {
        m_env = env;
        m_handle = handle;
        m_lambda = lambda;
    }

    [MirrorVerified("MplLocalFunctionMacroDifferentialTests")]
    public override Sexpr ExpandCall(MplEnv e, Sexpr form)
    {
        MplMacro.CanNotExpand(form.handle);
        return null;
    }

    [MirrorVerified("MplLocalFunctionMacroDifferentialTests")]
    public override Sexpr ExpandRef(MplEnv e, Tok s)
    {
        MplMacro.CanNotExpand(s.handle);
        return null;
    }
}
