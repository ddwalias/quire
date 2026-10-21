#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class let_keyword
{
    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    public static let_keyword singleton;

    static let_keyword()
    {
        singleton = new let_keyword();
    }

    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    public let_keyword() : base("let", 7) { }

    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    private static Sexpr ExpandDefs(uint tag, SCons defs)
    {
        if (defs == null)
        {
            return Nul.singleton;
        }
        SCons sCons = defs.head as SCons;
        if (sCons == null)
        {
            Sexpr.SyntaxError(defs.head);
        }
        Sexpr first;
        if (sCons.Length == 2)
        {
            first = define_keyword.MakeDefine(tag, sCons);
        }
        else if (sCons.Length == 3)
        {
            Sexpr def = lambda_keyword.MakeLambda(tag, sCons.Field2);
            first = define_keyword.MakeDefine(tag, sCons.head, def);
        }
        else
        {
            Sexpr.SyntaxError(sCons);
            return null;
        }
        return new SCons(first, ExpandDefs(tag, defs.tail));
    }

    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    public static Sexpr ExpandLet(MplEnv env, SCons form)
    {
        Sexpr defs = ExpandDefs(form.handle, form.second.scons);
        return locally_keyword.MakeLocally(form.handle, defs, form.third);
    }

    [MirrorVerified("MplRunlet_keywordMechanicalDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        return ExpandLet(env, form).walk_expression(env);
    }
}
