#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class define_keyword
{
    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public static define_keyword singleton;

    static define_keyword()
    {
        singleton = new define_keyword();
    }

    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public define_keyword() : base("define", 1) { }

    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        if (form.Length != 3)
        {
            Sexpr.BadDeclaration(form);
        }
        Sexpr sexpr = form.second.walk_pattern(env);
        Sexpr third = form.third;
        if (sexpr == form.second)
        {
            return new Cons(form, null);
        }
        return new Cons(SCons.list3(form.head, sexpr, third), null);
    }

    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form)
    {
        Sexpr sexpr = form.third.walk_expression(env);
        if (sexpr != form.third)
        {
            return SCons.list3(form.first, form.second, sexpr);
        }
        return form;
    }

    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public static SCons MakeDefine(uint handle, Sexpr forms) =>
        new SCons(new Tok(singleton, handle), forms);

    [MirrorVerified("MplRundefine_keywordMechanicalDifferentialTests")]
    public static SCons MakeDefine(uint handle, Sexpr pat, Sexpr def) =>
        MakeDefine(handle, SCons.list2(pat, def));
}
