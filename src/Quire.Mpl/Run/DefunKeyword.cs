using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class defun_keyword
{
    [MirrorVerified("DefunKeywordDifferentialTests")]
    public static defun_keyword singleton;

    static defun_keyword()
    {
        singleton = new defun_keyword();
    }

    [MirrorVerified("DefunKeywordDifferentialTests")]
    public defun_keyword() : base("defun", 0)
    {
    }

    [MirrorVerified("DefunKeywordDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("DefunKeywordDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        Sym asSym = form.second.AsSym;
        env.AddLocalEquation(asSym, form.handle);
        return new Cons(form, null);
    }

    [MirrorVerified("DefunKeywordDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form)
    {
        if (form.Length != 4)
        {
            Sexpr.BadDeclaration(form);
        }
        Sexpr second = form.second;
        SCons scons = form.third.scons;
        Sexpr expr = form.fourth.walk_with_bindings(env, scons, out var xpats);
        return MakeDefun(form.handle, second, xpats, expr);
    }

    [MirrorVerified("DefunKeywordDifferentialTests")]
    public static SCons MakeDefun(uint handle, Sexpr forms)
    {
        return new SCons(new Tok(singleton, handle), forms);
    }

    [MirrorVerified("DefunKeywordDifferentialTests")]
    public static SCons MakeDefun(uint handle, Sexpr id, Sexpr pats, Sexpr expr)
    {
        return MakeDefun(handle, SCons.list3(id, pats, expr));
    }
}
