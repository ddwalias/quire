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

    [MirrorVerified("MplFunctionMacroDifferentialTests")]
    public MplFunctionMacro(string name, Function expander) : this(new Sym(name, null), expander) { }

    [MirrorVerified("MplFunctionMacroDifferentialTests")]
    public static MplMacro make(string name, Function expander) => new MplFunctionMacro(name, expander);

    [MirrorVerified("MplFunctionMacroDifferentialTests")]
    public override Sexpr ExpandCall(MplEnv e, Sexpr form)
    {
        try
        {
            return (Sexpr)m_expander.Call(e, form);
        }
        catch (AppException inner)
        {
            throw new AppException("illegal macro call", inner, form.handle);
        }
    }

    [MirrorVerified("MplRunMplFunctionMacroMechanicalDifferentialTests")]
    public override Sexpr ExpandRef(MplEnv e, Tok s) => base.IllegalMacroCall(s);
}
