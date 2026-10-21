#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class declare_keyword
{
    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public static declare_keyword singleton;

    static declare_keyword()
    {
        singleton = new declare_keyword();
    }

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public declare_keyword() : base("declare", 5) { }

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public static Tok NewTok(uint handle) => new Tok(singleton, handle);

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        uint length = form.Length;
        if (length == 3u)
        {
            return new Cons(form, null);
        }
        if (length == 4u)
        {
            Sym sym = form.second.sym;
            if (sym == null)
            {
                Sexpr.BadDeclaration(form);
            }
            Sexpr third = form.third;
            Sexpr fourth = form.fourth;
            if (IsSymMacro(third))
            {
                env.AddSymbolMacro(sym, form.handle, fourth);
                return null;
            }
            if (IsMacro(third))
            {
                env.AddForeignMacro(sym, form.handle, fourth.AsStr);
                return new Cons(form, null);
            }
            env.AddLocal(sym, form.handle);
            return new Cons(form, null);
        }
        Sexpr.BadDeclaration(form);
        return null;
    }

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public override SCons walk_definition_as(MplEnv env, SCons form) => form;

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public static bool IsMacro(Sexpr type)
    {
        Sym sym = type.sym;
        if (sym == null)
        {
            return false;
        }
        return sym.name == "Macro";
    }

    [MirrorVerified("MplRundeclare_keywordMechanicalDifferentialTests")]
    public static bool IsSymMacro(Sexpr type)
    {
        Sym sym = type.sym;
        if (sym == null)
        {
            return false;
        }
        return sym.name == "SymMacro";
    }
}
