#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class lambda_keyword
{
    [MirrorVerified("MplRunlambda_keywordMechanicalDifferentialTests")]
    public static lambda_keyword singleton;

    static lambda_keyword()
    {
        singleton = new lambda_keyword();
    }

    [MirrorVerified("MplRunlambda_keywordMechanicalDifferentialTests")]
    public lambda_keyword() : base("lambda", 6) { }

    [MirrorVerified("MplRunlambda_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRunlambda_keywordMechanicalDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr xpats;
        Sexpr body = form.third.walk_with_bindings(env, form.second.scons, out xpats);
        return MakeLambda(form.handle, xpats, body);
    }

    [MirrorVerified("MplRunlambda_keywordMechanicalDifferentialTests")]
    public static SCons MakeLambda(uint handle, Sexpr forms) =>
        new SCons(new Tok(singleton, handle), forms);

    [MirrorVerified("MplRunlambda_keywordMechanicalDifferentialTests")]
    public static SCons MakeLambda(uint handle, Sexpr pats, Sexpr body) =>
        MakeLambda(handle, SCons.list2(pats, body));
}
