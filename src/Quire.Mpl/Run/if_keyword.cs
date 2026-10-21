#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class if_keyword
{
    [MirrorVerified("MplRunif_keywordMechanicalDifferentialTests")]
    public static if_keyword singleton;

    static if_keyword()
    {
        singleton = new if_keyword();
    }

    [MirrorVerified("MplRunif_keywordMechanicalDifferentialTests")]
    public if_keyword() : base("if", 9) { }

    [MirrorVerified("MplRunif_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRunif_keywordMechanicalDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr rest = SCons.walk_expressions(env, form.tail);
        return new SCons(form.head, rest);
    }

    [MirrorVerified("MplRunif_keywordMechanicalDifferentialTests")]
    public static SCons MakeIf(uint handle, Sexpr forms) =>
        new SCons(new Tok(singleton, handle), forms);

    [MirrorVerified("MplRunif_keywordMechanicalDifferentialTests")]
    public static SCons MakeIf(uint handle, Sexpr cond_expr, Sexpr then_expr, Sexpr else_expr) =>
        MakeIf(handle, SCons.list3(cond_expr, then_expr, else_expr));
}
