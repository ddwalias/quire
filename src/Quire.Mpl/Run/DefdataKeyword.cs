using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class defdata_keyword
{
    [MirrorVerified("DefdataKeywordDifferentialTests")]
    public static defdata_keyword singleton;

    static defdata_keyword()
    {
        singleton = new defdata_keyword();
    }

    [MirrorVerified("DefdataKeywordDifferentialTests")]
    public defdata_keyword() : base("defdata", 3)
    {
    }

    [MirrorVerified("DefdataKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("DefdataKeywordDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        return new Cons(form, null);
    }

    [MirrorVerified("DefdataKeywordDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form)
    {
        return form;
    }
}
