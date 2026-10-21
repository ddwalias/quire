#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class try_keyword
{
    [MirrorVerified("MplRuntry_keywordMechanicalDifferentialTests")]
    public static try_keyword singleton;

    static try_keyword()
    {
        singleton = new try_keyword();
    }

    [MirrorVerified("MplRuntry_keywordMechanicalDifferentialTests")]
    public try_keyword() : base("try", 12) { }

    [MirrorVerified("MplRuntry_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRuntry_keywordMechanicalDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr.NotImplemented(form);
        return null;
    }
}
