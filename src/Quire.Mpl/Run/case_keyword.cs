using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class case_keyword : Keyword
{
    [MirrorVerified("CaseKeywordDifferentialTests")]
    public static case_keyword singleton;

    static case_keyword()
    {
        singleton = new case_keyword();
    }

    [MirrorVerified("CaseKeywordDifferentialTests")]
    public case_keyword() : base("case", 10)
    {
    }

    [MirrorVerified("CaseKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorTested("Mpl.Run.case_keyword")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr.NotImplemented(form);
        return null;
    }
}
