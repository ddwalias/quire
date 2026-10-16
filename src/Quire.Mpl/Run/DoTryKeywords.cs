using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class do_keyword
{
    [MirrorVerified("DoTryKeywordDifferentialTests")]
    public static do_keyword singleton;

    static do_keyword()
    {
        singleton = new do_keyword();
    }

    [MirrorVerified("DoTryKeywordDifferentialTests")]
    public do_keyword() : base("do", 11)
    {
    }

    [MirrorVerified("DoTryKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorTested("Mpl.Run.do_keyword")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr.NotImplemented(form);
        return null;
    }
}

public partial class try_keyword
{
    [MirrorVerified("DoTryKeywordDifferentialTests")]
    public static try_keyword singleton;

    static try_keyword()
    {
        singleton = new try_keyword();
    }

    [MirrorVerified("DoTryKeywordDifferentialTests")]
    public try_keyword() : base("try", 12)
    {
    }

    [MirrorVerified("DoTryKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorTested("Mpl.Run.try_keyword")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr.NotImplemented(form);
        return null;
    }
}
