using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class if_keyword
{
    [MirrorVerified("IfKeywordDifferentialTests")]
    public static if_keyword singleton;

    static if_keyword()
    {
        singleton = new if_keyword();
    }

    [MirrorVerified("IfKeywordDifferentialTests")]
    public if_keyword() : base("if", 9)
    {
    }

    [MirrorVerified("IfKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("IfKeywordDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr rest = SCons.walk_expressions(env, form.tail);
        return new SCons(form.head, rest);
    }

    [MirrorVerified("IfKeywordDifferentialTests")]
    public static SCons MakeIf(uint handle, Sexpr forms)
    {
        return new SCons(new Tok(singleton, handle), forms);
    }

    [MirrorVerified("IfKeywordDifferentialTests")]
    public static SCons MakeIf(uint handle, Sexpr cond_expr, Sexpr then_expr, Sexpr else_expr)
    {
        return MakeIf(handle, SCons.list3(cond_expr, then_expr, else_expr));
    }
}
