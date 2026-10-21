#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class defun_keyword
{
    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public static defun_keyword singleton;

    static defun_keyword()
    {
        singleton = new defun_keyword();
    }

    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public defun_keyword() : base("defun", 0) { }

    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        Sym asSym = form.second.AsSym;
        env.AddLocalEquation(asSym, form.handle);
        return new Cons(form, null);
    }

    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form)
    {
        if (form.Length != 4)
        {
            Sexpr.BadDeclaration(form);
        }
        Sexpr second = form.second;
        SCons scons = form.third.scons;
        Sexpr xpats;
        Sexpr expr = form.fourth.walk_with_bindings(env, scons, out xpats);
        return MakeDefun(form.handle, second, xpats, expr);
    }

    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public static SCons MakeDefun(uint handle, Sexpr forms) =>
        new SCons(new Tok(singleton, handle), forms);

    [MirrorVerified("MplRundefun_keywordMechanicalDifferentialTests")]
    public static SCons MakeDefun(uint handle, Sexpr id, Sexpr pats, Sexpr expr) =>
        MakeDefun(handle, SCons.list3(id, pats, expr));
}
