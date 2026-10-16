using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class lambda_keyword
{
    [MirrorVerified("LambdaKeywordDifferentialTests")]
    public static lambda_keyword singleton;

    static lambda_keyword()
    {
        singleton = new lambda_keyword();
    }

    [MirrorVerified("LambdaKeywordDifferentialTests")]
    public lambda_keyword() : base("lambda", 6)
    {
    }

    [MirrorVerified("LambdaKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("LambdaKeywordDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr body = form.third.walk_with_bindings(env, form.second.scons, out var xpats);
        return MakeLambda(form.handle, xpats, body);
    }

    [MirrorVerified("LambdaKeywordDifferentialTests")]
    public static SCons MakeLambda(uint handle, Sexpr forms)
    {
        return new SCons(new Tok(singleton, handle), forms);
    }

    [MirrorVerified("LambdaKeywordDifferentialTests")]
    public static SCons MakeLambda(uint handle, Sexpr pats, Sexpr body)
    {
        return MakeLambda(handle, SCons.list2(pats, body));
    }
}
