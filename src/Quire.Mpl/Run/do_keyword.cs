#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class do_keyword
{
    [MirrorVerified("MplRundo_keywordMechanicalDifferentialTests")]
    public static do_keyword singleton;

    static do_keyword()
    {
        singleton = new do_keyword();
    }

    [MirrorVerified("MplRundo_keywordMechanicalDifferentialTests")]
    public do_keyword() : base("do", 11) { }

    [MirrorVerified("MplRundo_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRundo_keywordMechanicalDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr.NotImplemented(form);
        return null;
    }
}
