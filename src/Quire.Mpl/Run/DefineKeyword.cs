using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class define_keyword
{
    [MirrorVerified("DefineKeywordDifferentialTests")]
    public static define_keyword singleton;

    static define_keyword()
    {
        singleton = new define_keyword();
    }

    [MirrorVerified("DefineKeywordDifferentialTests")]
    public define_keyword() : base("define", 1)
    {
    }

    [MirrorVerified("DefineKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("DefineKeywordDifferentialTests")]
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

    [MirrorVerified("DefineKeywordDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form)
    {
        Sexpr sexpr = form.third.walk_expression(env);
        if (sexpr != form.third)
        {
            return SCons.list3(form.first, form.second, sexpr);
        }
        return form;
    }

    [MirrorVerified("DefineKeywordDifferentialTests")]
    public static SCons MakeDefine(uint handle, Sexpr forms)
    {
        return new SCons(new Tok(singleton, handle), forms);
    }

    [MirrorVerified("DefineKeywordDifferentialTests")]
    public static SCons MakeDefine(uint handle, Sexpr pat, Sexpr def)
    {
        return MakeDefine(handle, SCons.list2(pat, def));
    }
}
