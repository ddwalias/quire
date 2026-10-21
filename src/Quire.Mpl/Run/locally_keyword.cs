#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class locally_keyword
{
    [MirrorVerified("LocallyKeywordDifferentialTests")]
    public static locally_keyword singleton = new locally_keyword();

    [MirrorVerified("MplRunlocally_keywordMechanicalDifferentialTests")]
    public locally_keyword() : base("locally", 8) { }

    [MirrorVerified("LocallyKeywordDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorTested("Mpl.Run.locally_keyword")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        if (form.Length != 3) Sexpr.BadExpression(form);
        MplEnv outenv;
        Cons cons = block_keyword.MakeBlock(form.handle, form.second).walk(env, out outenv);
        Sexpr sexpr = form.third.walk_expression(outenv);
        if (cons != null) return MakeLocally(form.handle, cons, sexpr);
        return sexpr;
    }

    [MirrorVerified("LocallyKeywordDifferentialTests")]
    public static SCons MakeLocally(uint handle, Sexpr forms) =>
        new SCons(new Tok(singleton, handle), forms);

    [MirrorVerified("LocallyKeywordDifferentialTests")]
    public static SCons MakeLocally(uint handle, Sexpr defs, Sexpr body) =>
        MakeLocally(handle, SCons.list2(defs, body));

    [MirrorVerified("LocallyKeywordDifferentialTests")]
    public static SCons MakeLocally(uint handle, Cons defs, Sexpr body) =>
        MakeLocally(handle, SCons.list2(SCons.as_Sexpr(defs), body));
}
