#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class MplFunctionMacro
{
    [MirrorVerified("MplRunMplFunctionMacroMechanicalDifferentialTests")]
    private Function m_expander;

    [MirrorVerified("MplRunMplFunctionMacroMechanicalDifferentialTests")]
    public MplFunctionMacro(Sym name, Function expander) : base(name)
    {
        m_expander = expander;
    }

    [MirrorVerified("MplRunMplFunctionMacroMechanicalDifferentialTests")]
    public override Sexpr ExpandRef(MplEnv e, Tok s) => base.IllegalMacroCall(s);
}
