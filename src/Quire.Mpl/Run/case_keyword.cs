#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class case_keyword
{
    [MirrorVerified("MplRuncase_keywordMechanicalDifferentialTests")]
    public static case_keyword singleton;

    static case_keyword()
    {
        singleton = new case_keyword();
    }

    [MirrorVerified("MplRuncase_keywordMechanicalDifferentialTests")]
    public case_keyword() : base("case", 10) { }

    [MirrorVerified("MplRuncase_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRuncase_keywordMechanicalDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr.NotImplemented(form);
        return null;
    }
}
