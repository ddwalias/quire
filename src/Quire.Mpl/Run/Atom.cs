#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// An indivisible S-expression value: a symbol or a constant (docs/mirror/evidence/Mpl.Run.Atom.md). The typed
/// accessors don't throw on the wrong kind of atom: they build an <see cref="AppException"/>, drop it, and return a
/// default.
/// </summary>
public abstract partial class Atom : Tag
{
    [MirrorVerified("SexprDifferentialTests")] public const int SymTag = 0;
    [MirrorVerified("SexprDifferentialTests")] public const int IntTag = 1;
    [MirrorVerified("SexprDifferentialTests")] public const int RealTag = 2;
    [MirrorVerified("SexprDifferentialTests")] public const int CharTag = 3;
    [MirrorVerified("SexprDifferentialTests")] public const int StrTag = 4;
    [MirrorVerified("SexprDifferentialTests")] public const int KeyTag = 5;

    [MirrorTested("Mpl.Run.Atom")]
    public Atom(int tag) : base(tag) { }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual int KeyNum
    {
        get
        {
            _ = new AppException("not a keyword", (uint)m_tag);
            return 0;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual double RealCon
    {
        get
        {
            _ = new AppException("not a real constant", (uint)m_tag);
            return 0.0;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual int IntCon
    {
        get
        {
            _ = new AppException("not an integer constant", (uint)m_tag);
            return 0;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual char CharCon
    {
        get
        {
            _ = new AppException("not a character constant");
            return '\0';
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual string StrCon
    {
        get
        {
            _ = new AppException("not a string constant");
            return null;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Sexpr quote_atom => throw new AppException("quotation not implemented", 0u, 0u);

    [MirrorVerified("SexprDifferentialTests")]
    public Sym sym => this as Sym;

    [MirrorVerified("SexprDifferentialTests")]
    public Keyword Key => this as Keyword;
}

/// <summary>
/// A symbol (docs/mirror/evidence/Mpl.Run.Sym.md). Symbols aren't interned: each is its own object, compared by
/// reference. Only the constructor taking a tag keeps the qualifier; the two-argument one drops it.
/// </summary>
public partial class Sym : Atom
{
    [MirrorVerified("SexprDifferentialTests")]
    public string Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public string Field2;

    [MirrorVerified("SexprDifferentialTests")]
    private uint m_umgd;

    [MirrorVerified("SexprDifferentialTests")]
    public static readonly Tok make_sym;

    [MirrorVerified("SexprDifferentialTests")]
    public static readonly Tok make_symq;

    static Sym()
    {
        make_sym = MakeSymQ("Sym", "MplMacro");
        make_symq = MakeSymQ("make_symbolq", "MplMacro");
    }

    /// <summary>A symbol named <paramref name="nm"/> when it is a string, otherwise with a null name.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public Sym(object nm) : this(nm as string, null) { }

    /// <summary>Ignores <paramref name="q"/>: the qualifier is always null.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public Sym(string nm, string q) : base(SymTag)
    {
        Field1 = nm;
        Field2 = null;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public Sym(string nm, string q, uint id) : this(nm, q) { uid = id; }

    /// <summary>The one constructor that keeps the qualifier, with the atom tag given (keywords pass theirs).</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public Sym(string nm, string q, int tag) : base(tag)
    {
        Field1 = nm;
        Field2 = q;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public string name => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public string qualifier => Field2;

    /// <summary>Whether the first character is upper case; throws on an empty or null name.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public bool UpId => char.IsUpper(Field1, 0);

    [MirrorVerified("SexprDifferentialTests")]
    public bool LowId => char.IsLower(Field1, 0);

    /// <summary>The name, then <c>.</c> and the qualifier when there is one (name first).</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public string long_name
    {
        get
        {
            if (Field2 != null)
            {
                return Field1 + "." + Field2;
            }
            return Field1;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public uint uid
    {
        get => m_umgd;
        set => m_umgd = value;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override int KeyNum => -1;

    /// <summary><c>(Sym "name")</c>, or <c>(make_symbolq "name" "qualifier")</c> for a qualified symbol.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote_atom
    {
        get
        {
            if (qualifier != null)
            {
                return SCons.list3(make_symq, str_constant.NewTok(Field1, 0u), str_constant.NewTok(Field2, 0u));
            }
            return SCons.list2(make_sym, str_constant.NewTok(Field1, 0u));
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string nm, uint tag) => new(new Sym(nm, null), tag);

    /// <summary>Goes through the two-argument constructor, so the qualifier is dropped.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string nm, string q, uint tag) => new(new Sym(nm, q), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string nm, uint uid, uint tag) => new(new Sym(nm, null, uid), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string nm, string q, uint uid, uint tag) => new(new Sym(nm, q, uid), tag);

    /// <summary>A token for the symbol <paramref name="nm"/>, with handle 0.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public static Tok MakeSym(string nm) => NewTok(nm, 0u);

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok MakeSymQ(string nm, string q) => NewTok(nm, q, 0u);

    /// <summary>The bare name: no qualifier, no quoting.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Cons walk_declaration_as(MplEnv env, SCons form)
    {
        MplMacro macro = env.LoadMacro(this);
        if (macro == null) Sexpr.BadDeclaration(form);
        return macro.ExpandCall(env, form).walk_declaration_lhs(env);
    }

    [MirrorTested("Mpl.Run.Sym")]
    public virtual SCons walk_definition_as(MplEnv env, SCons form)
    {
        Sexpr.BadDeclaration(form);
        return null;
    }

    /// <summary>
    /// A lower-case symbol naming a macro expands the form; otherwise the arguments are walked as expressions,
    /// keeping <paramref name="form"/> when none changed.
    /// </summary>
    [MirrorVerified("SexprDifferentialTests")]
    public virtual Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        MplMacro macro = LowId ? env.LoadMacro(this) : null;
        if (macro == null)
        {
            Sexpr sexpr = SCons.walk_expressions(env, form.tail);
            if (sexpr == form.Field2)
            {
                return form;
            }
            return new SCons(form.head, sexpr);
        }
        return macro.ExpandCall(env, form).walk_expression(env);
    }
}

public partial class int_constant : Atom
{
    [MirrorVerified("SexprDifferentialTests")]
    public int Field1;

    [MirrorVerified("SexprDifferentialTests")]
    private static Tok token;

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok zero;

    static int_constant()
    {
        token = Sym.NewTok("IntCon", "MplMacro", 0u);
        zero = NewTok(0, 0u);
    }

    /// <summary>Parses with the current culture; text that isn't an int gives 0.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public int_constant(string name) : base(IntTag) { int.TryParse(name, out Field1); }

    [MirrorVerified("SexprDifferentialTests")]
    public int_constant(int num) : base(IntTag) { Field1 = num; }

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(int num, uint tag) => new(new int_constant(num), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string nm, uint tag) => new(new int_constant(nm), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public override int IntCon => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote_atom => SCons.list2(token, NewTok(Field1, 0u));

    /// <summary>The current culture's formatting.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => Field1.ToString();
}

public partial class real_constant : Atom
{
    [MirrorVerified("SexprDifferentialTests")]
    public double Field1;

    [MirrorVerified("SexprDifferentialTests")]
    private static Tok token;

    static real_constant()
    {
        token = Sym.NewTok("RealCon", "MplMacro", 0u);
    }

    /// <summary>
    /// Parses with the current culture (unparsable text gives 0). Tags the atom <see cref="Atom.IntTag"/>, not
    /// <see cref="Atom.RealTag"/>, as Mathcad does.
    /// </summary>
    [MirrorVerified("SexprDifferentialTests")]
    public real_constant(string name) : base(IntTag) { double.TryParse(name, out Field1); }

    [MirrorVerified("SexprDifferentialTests")]
    public real_constant(double v) : base(RealTag) { Field1 = v; }

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(double v, uint tag) => new(new real_constant(v), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string nm, uint tag) => new(new real_constant(nm), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public override double RealCon => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote_atom => SCons.list2(token, NewTok(Field1, 0u));

    /// <summary>The current culture's default (shortest round-trip) formatting.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => Field1.ToString();
}

public partial class str_constant : Atom
{
    [MirrorVerified("SexprDifferentialTests")]
    public string Field1;

    [MirrorVerified("SexprDifferentialTests")]
    private static Tok token;

    static str_constant()
    {
        token = Sym.NewTok("StrCon", "MplMacro", 0u);
    }

    [MirrorVerified("SexprDifferentialTests")]
    public str_constant(string v) : base(StrTag) { Field1 = v; }

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(string v, uint tag) => new(new str_constant(v), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public override string StrCon => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote_atom => SCons.list2(token, NewTok(Field1, 0u));

    /// <summary>The raw text, without quotes or escapes.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => Field1;
}

public partial class char_constant : Atom
{
    [MirrorVerified("SexprDifferentialTests")]
    public char Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public char_constant(char v) : base(CharTag) { Field1 = v; }

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(char v, uint tag) => new(new char_constant(v), tag);

    [MirrorVerified("SexprDifferentialTests")]
    public override char CharCon => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => Field1.ToString();
}

public partial class Keyword : Sym
{
    [MirrorVerified("SexprDifferentialTests")]
    public static string keyword;

    [MirrorVerified("SexprDifferentialTests")]
    protected readonly int m_keynum;

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok key_tok;

    [MirrorVerified("SexprDifferentialTests")]
    public override int KeyNum => m_keynum;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote_atom => SCons.list2(key_tok, int_constant.NewTok(m_keynum, 0u));

    static Keyword()
    {
        key_tok = Sym.MakeSymQ("Key", "MplMacro");
        keyword = string.Intern("Keyword");
    }

    [MirrorVerified("SexprDifferentialTests")]
    public Keyword(string nm, int keynum) : base(nm, keyword, 5)
    {
        m_keynum = keynum;
    }

    [MirrorTested("Mpl.Run.Keyword")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        Sexpr.BadDeclaration(form);
        return null;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr walk_expression_as(MplEnv env, SCons form)
    {
        Sexpr sexpr = SCons.walk_expressions(env, form.tail);
        if (sexpr == form.Field2)
        {
            return form;
        }
        return new SCons(form.head, sexpr);
    }
}

public partial class block_keyword : Keyword
{
    [MirrorVerified("SexprDifferentialTests")]
    public static block_keyword singleton;

    static block_keyword()
    {
        singleton = new block_keyword();
    }

    [MirrorVerified("SexprDifferentialTests")]
    public block_keyword() : base("block", 4)
    {
    }

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok NewTok(uint handle)
    {
        return new Tok(singleton, handle);
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override Cons walk_declaration_as(MplEnv env, SCons form)
    {
        SCons tail = form.tail;
        if (tail != null)
        {
            return Cons.destructive_append(tail.head.walk_declaration_lhs(env), walk_declaration_as(env, tail));
        }
        return form.Field2.walk_declaration_lhs(env);
    }

    [MirrorVerified("SexprDifferentialTests")]
    public static SCons MakeBlock(uint handle, Sexpr forms)
    {
        return new SCons(new Tok(singleton, handle), forms);
    }
}
