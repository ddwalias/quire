using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class deftype_keyword
{
    [MirrorVerified("DeftypeKeywordDifferentialTests")]
    public static deftype_keyword singleton;

    static deftype_keyword()
    {
        singleton = new deftype_keyword();
    }

    [MirrorVerified("DeftypeKeywordDifferentialTests")]
    public deftype_keyword() : base("deftype", 2)
    {
    }

    [MirrorVerified("DeftypeKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("DeftypeKeywordDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        return new Cons(form, null);
    }

    [MirrorVerified("DeftypeKeywordDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form)
    {
        return form;
    }
}
