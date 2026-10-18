#nullable disable
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

// The S-expression family of mplrun 12.0.1.0, member by member (docs/mirror/evidence/Mpl.Run.Sexpr.md and the records
// of each subclass). Printing is Mathcad's debugging text, not Rl: tokens with a handle print as "#T<hex> atom", the
// empty list prints as "Nul" and strings print raw. Quire's Rl writer is Quire.Engine.Internal.Rl.Write.

/// <summary>A value carrying a small integer tag that tells its subclasses apart.</summary>
public partial class Tag : DimVal
{
    [MirrorVerified("SexprDifferentialTests")]
    public int m_tag;

    [MirrorVerified("SexprDifferentialTests")]
    public Tag(int tag) { m_tag = tag; }
}

/// <summary>
/// An S-expression. Lists are <see cref="SCons"/> cells ending in <see cref="Nul"/>; atoms appear wrapped in a
/// <see cref="Tok"/>, which also carries the source handle used to place errors. The calculation engine receives
/// every region as such an expression ("Rl"), for example <c>(define x (op_mult 5 m))</c>.
/// </summary>
public abstract partial class Sexpr : Tag
{
    [MirrorVerified("SexprDifferentialTests")] public const int NulTag = 0;
    [MirrorVerified("SexprDifferentialTests")] public const int ConsTag = 1;
    [MirrorVerified("SexprDifferentialTests")] public const int TokTag = 2;
    [MirrorVerified("SexprDifferentialTests")] public const int QtTag = 3;
    [MirrorVerified("SexprDifferentialTests")] public const int BqTag = 4;
    [MirrorVerified("SexprDifferentialTests")] public const int CmTag = 5;
    [MirrorVerified("SexprDifferentialTests")] public const int CaTag = 6;
    [MirrorVerified("SexprDifferentialTests")] public const int SAppendTag = 7;
    [MirrorVerified("SexprDifferentialTests")] public const int SListTag = 8;

    [MirrorTested("Mpl.Run.Sexpr")]
    public Sexpr(int tag) : base(tag) { }

    /// <summary>The source handle; 0 unless a token (or a list, through its first element) carries one.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public virtual uint handle => 0;

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Sym Key => null;

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Sym sym => null;

    [MirrorVerified("SexprDifferentialTests")]
    public virtual string str => null;

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Sym AsSym
    {
        get
        {
            Sym s = sym;
            if (s == null) NotASymbol(this);
            return s;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual string AsStr
    {
        get
        {
            string s = str;
            if (s == null) NotAString(this);
            return s;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Sym AsKey
    {
        get
        {
            Sym k = Key;
            if (k == null) NotAKeyword(this);
            return k;
        }
    }

    [MirrorTested("Mpl.Run.Sexpr")]
    public virtual Sexpr quotation
    {
        get
        {
            NotAQuotation(this);
            return null;
        }
    }

    [MirrorTested("Mpl.Run.Sexpr")]
    public virtual Sexpr quote
    {
        get
        {
            QuotationNotImplemented(this);
            return null;
        }
    }

    /// <summary>The cons cell of a non-empty list; <see cref="Nul"/> gives null and anything else throws.</summary>
    [MirrorTested("Mpl.Run.Sexpr")]
    public virtual SCons scons
    {
        get
        {
            ExpectedAList(this);
            return null;
        }
    }

    /// <summary>How this prints after the first element of a list: a dotted tail, without the closing parenthesis.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public virtual string FormatTail() => " . " + ToString();

    [MirrorVerified("SexprDifferentialTests")]
    public virtual string FormatAppend() => " " + ToString();

    /// <summary><see cref="walk"/>, returning the new environment and the definitions as a pair.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public static object[] Walk(MplEnv env, Sexpr block)
    {
        Cons defs = block.walk(env, out MplEnv outenv);
        return [outenv, defs];
    }

    [MirrorVerified("SexprDifferentialTests")]
    public static Sexpr WalkModule(MplEnv env, Sexpr block)
    {
        Cons defs = block.walk(env, out _);
        return new SCons(block_keyword.NewTok(0u), SCons.as_Sexpr(defs));
    }

    /// <summary>
    /// Walks a block of declarations in a fresh <see cref="LocalEnv"/> over <paramref name="env"/>: first the
    /// left-hand sides, then each definition in place.
    /// </summary>
    [MirrorVerified("SexprDifferentialTests")]
    public Cons walk(MplEnv env, out MplEnv outenv)
    {
        outenv = new LocalEnv(env);
        return walk_definitions(walk_declaration_lhs(outenv), outenv);
    }

    [MirrorVerified("SexprDifferentialTests")]
    private static Cons walk_definitions(Cons decls, MplEnv env)
    {
        for (Cons c = decls; c != null; c = c.Field2 as Cons)
            c.Field1 = walk_definition(c.Field1 as SCons, env);
        return decls;
    }

    [MirrorTested("Mpl.Run.Sexpr")]
    private static Sexpr walk_definition(SCons decl, MplEnv env) => decl.Key.walk_definition_as(env, decl);

    [MirrorVerified("SexprDifferentialTests")]
    public virtual Cons walk_declaration_lhs(MplEnv env) => null;

    [MirrorTested("Mpl.Run.Sexpr")]
    public virtual Sexpr walk_pattern(MplEnv env)
    {
        BadPattern(this);
        return null;
    }

    [MirrorTested("Mpl.Run.Sexpr")]
    public virtual Sexpr walk_expression(MplEnv env)
    {
        BadExpression(this);
        return null;
    }

    /// <summary>Walks <paramref name="pats"/> as patterns into a new local scope, then this expression in it.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr walk_with_bindings(MplEnv env, SCons pats, out Sexpr xpats)
    {
        var scope = new LocalEnv(env);
        xpats = SCons.walk_patterns(scope, pats);
        return walk_expression(scope);
    }

    [MirrorVerified("SexprDifferentialTests")]
    public static void BadDeclaration(Sexpr form) => throw new AppException("illegal declaration", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void BadPattern(Sexpr form) => throw new AppException("bad pattern", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void BadExpression(Sexpr form) => throw new AppException("bad expression", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void ExpectedAList(Sexpr form) => throw new AppException("expected a list", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void SyntaxError(Sexpr form) => throw new AppException("syntax error", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void BadApplication(Sexpr form) => throw new AppException("bad application", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void NotASymbol(Sexpr form) => throw new AppException("expected a symbol", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void NotAString(Sexpr form) => throw new AppException("expected a string", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void NotAKeyword(Sexpr form) => throw new AppException("expected a keyword", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void NotImplemented(Sexpr form) => throw new AppException("not implemented", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void NotAQuotation(Sexpr form) => throw new AppException("not a quotation", 0u, form.handle);

    [MirrorVerified("SexprDifferentialTests")]
    public static void QuotationNotImplemented(Sexpr form) =>
        throw new AppException("quotation not implemented", 0u, form.handle);
}

/// <summary>The empty list. It prints as <c>Nul</c>, and as nothing at the end of a list.</summary>
public partial class Nul : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public static Nul singleton;

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok token;

    static Nul()
    {
        singleton = new Nul();
        token = Sym.NewTok("Nul", "MplMacro", 0u);
    }

    [MirrorVerified("SexprDifferentialTests")]
    public Nul() : base(NulTag) { }

    [MirrorVerified("SexprDifferentialTests")]
    public override SCons scons => null;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote => token;

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => "Nul";

    [MirrorVerified("SexprDifferentialTests")]
    public override string FormatTail() => "";
}

/// <summary>A list cell: <see cref="car"/> is the element and <see cref="cdr"/> the rest of the list.</summary>
public partial class SCons : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field2;

    [MirrorVerified("SexprDifferentialTests")]
    public static Tok token;

    [MirrorVerified("SexprDifferentialTests")]
    public static Sym apply;

    static SCons()
    {
        token = Sym.NewTok("Cons", "MplMacro", 0u);
        apply = new Sym("apply", "MplMacro");
    }

    /// <summary>Arguments that aren't S-expressions become null.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public SCons(object first, object rest) : base(ConsTag)
    {
        Field1 = first as Sexpr;
        Field2 = rest as Sexpr;
    }

    /// <summary>A null <paramref name="rest"/> is kept as null, not read as the empty list.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public SCons(Sexpr first, Sexpr rest) : base(ConsTag)
    {
        Field1 = first;
        Field2 = rest;
    }

    /// <summary>The handle of the first element.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override uint handle => Field1.handle;

    [MirrorVerified("SexprDifferentialTests")]
    public override SCons scons => this;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr head => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public SCons tail => Field2 as SCons;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr car => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr cdr => Field2;

    /// <summary>The symbol heading the list, or null.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override Sym Key => Field1.sym;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr first => Field1;

    /// <summary>Throws <see cref="NullReferenceException"/> when the list is shorter, like the later positions.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr second => tail.head;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr third => tail.tail.head;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr fourth => tail.tail.tail.head;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr fifth => tail.tail.tail.tail.head;

    /// <summary>The number of cells reachable through <see cref="tail"/>, this one included.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public uint Length
    {
        get
        {
            uint n = 0;
            SCons c = this;
            do
            {
                n++;
                c = c.tail;
            }
            while (c != null);
            return n;
        }
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote => list3(token, Field1.quote, Field2.quote);

    [MirrorVerified("SexprDifferentialTests")]
    public override string FormatTail() => " " + Field1.ToString() + Field2.FormatTail();

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => "(" + Field1.ToString() + Field2.FormatTail() + ")";

    [MirrorVerified("SexprDifferentialTests")]
    public override Cons walk_declaration_lhs(MplEnv env)
    {
        Sym key = Key;
        if (key == null) BadDeclaration(this);
        return key.walk_declaration_as(env, this);
    }

    /// <summary>Walks each element as a pattern, reusing the original cells when nothing changed.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public static Sexpr walk_patterns(MplEnv env, SCons pats)
    {
        if (pats == null) return Nul.singleton;
        Sexpr h = pats.head.walk_pattern(env);
        Sexpr t = walk_patterns(env, pats.tail);
        if (h == pats.head && t == pats.Field2) return pats;
        return new SCons(h, t);
    }

    [MirrorTested("Mpl.Run.SCons")]
    public override Sexpr walk_pattern(MplEnv env)
    {
        Sym key = Key;
        if (key == null) BadPattern(this);
        if (key.LowId)
        {
            MplMacro macro = env.LoadMacro(key);
            if (macro == null)
            {
                if (Length == 1)
                {
                    env.AddLocal(key, handle);
                    return this;
                }
                BadPattern(this);
            }
            return macro.ExpandCall(env, this).walk_pattern(env);
        }
        return new SCons(head, walk_patterns(env, tail));
    }

    /// <summary>Walks each element as an expression, reusing the original cells when nothing changed.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public static Sexpr walk_expressions(MplEnv env, SCons exps)
    {
        if (exps == null) return Nul.singleton;
        Sexpr h = exps.head.walk_expression(env);
        Sexpr t = walk_expressions(env, exps.tail);
        if (h == exps.head)
        {
            if (t == exps.Field2) return exps;
        }
        return new SCons(h, t);
    }

    /// <summary>
    /// A list headed by a symbol is walked by that symbol; any other list becomes an <c>apply</c> of all its
    /// elements. The null-head check can't fire: <see cref="Key"/> already dereferences the head.
    /// </summary>
    [MirrorTested("Mpl.Run.SCons")]
    public override Sexpr walk_expression(MplEnv env)
    {
        Sym key = Key;
        if (key == null)
        {
            _ = head;
            if (head == null)
            {
                BadApplication(this);
            }
            return new SCons(new Tok(apply, handle), walk_expressions(env, this));
        }
        return key.walk_expression_as(env, this);
    }

    [MirrorVerified("SexprDifferentialTests")]
    public static SCons list1(Sexpr x1) => new(x1, Nul.singleton);

    [MirrorVerified("SexprDifferentialTests")]
    public static SCons list2(Sexpr x1, Sexpr x2) => new(x1, list1(x2));

    [MirrorVerified("SexprDifferentialTests")]
    public static SCons list3(Sexpr x1, Sexpr x2, Sexpr x3) => new(x1, list2(x2, x3));

    [MirrorVerified("SexprDifferentialTests")]
    public static SCons list4(Sexpr x1, Sexpr x2, Sexpr x3, Sexpr x4) => new(x1, list3(x2, x3, x4));

    /// <summary>An S-expression list of a <see cref="Cons"/> list; items that aren't S-expressions become null.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public static Sexpr as_Sexpr(Cons list)
    {
        if (list == null) return Nul.singleton;
        return new SCons(list.head, (object)as_Sexpr(list.tail));
    }
}

/// <summary>An atom in an S-expression, with the source handle it came from.</summary>
public partial class Tok : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Atom Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public uint Field2;

    [MirrorVerified("SexprDifferentialTests")]
    private static Sym tok;

    static Tok()
    {
        tok = new Sym("Tok", "MplMacro");
    }

    [MirrorVerified("SexprDifferentialTests")]
    public Tok(object a, int tag) : this(a as Atom, tag) { }

    [MirrorVerified("SexprDifferentialTests")]
    public Tok(Atom a, int tag) : base(TokTag)
    {
        Field1 = a;
        Field2 = (uint)tag;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public Tok(Atom a, uint tag) : base(TokTag)
    {
        Field1 = a;
        Field2 = tag;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override uint handle => Field2;

    [MirrorVerified("SexprDifferentialTests")]
    public Atom atom => Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sym Key => Field1 as Sym;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sym sym => Field1 as Sym;

    /// <summary>The atom's <see cref="Atom.StrCon"/>: null unless it is a string constant.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override string str => Field1.StrCon;

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote => SCons.list3(new Tok(tok, Field2), Field1.quote_atom, int_constant.zero);

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString()
    {
        if (Field2 == 0) return Field1.ToString();
        return "#T" + Field2.ToString("X") + " " + Field1.ToString();
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr walk_pattern(MplEnv env)
    {
        Sym key = Key;
        if (key == null) return this;
        if (key.UpId)
        {
            MplMacro macro = env.LoadMacro(key);
            return macro == null ? this : macro.ExpandRef(env, this).walk_pattern(env);
        }
        env.AddLocal(key, handle);
        return this;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr walk_expression(MplEnv env)
    {
        Sym key = Key;
        if (key == null || !key.LowId) return this;
        MplMacro macro = env.LoadMacro(key);
        return macro == null ? this : macro.ExpandRef(env, this).walk_expression(env);
    }
}

/// <summary>A quoted expression, <c>'x</c>.</summary>
public partial class Qt : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field1;

    [MirrorVerified("SexprDifferentialTests")]
    private static Sym Qt_constructor;

    static Qt()
    {
        Qt_constructor = new Sym("Qt", "MplMacro");
    }

    [MirrorVerified("SexprDifferentialTests")]
    public Qt(object a) : base(QtTag) { Field1 = a as Sexpr; }

    [MirrorVerified("SexprDifferentialTests")]
    public Qt(Sexpr a) : base(QtTag) { Field1 = a; }

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quotation => Field1.quote;

    /// <summary>The constructor token carries the tag (3) as its handle.</summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr quote => SCons.list2(new Tok(Qt_constructor, m_tag), Field1.quote);

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => "'" + Field1.ToString();

    [MirrorVerified("SexprDifferentialTests")]
    public override Sexpr walk_expression(MplEnv env) => this;
}

/// <summary>A backquoted expression, <c>`x</c>.</summary>
public partial class Bq : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Bq(object a) : base(BqTag) { Field1 = a as Sexpr; }

    [MirrorVerified("SexprDifferentialTests")]
    public Bq(Sexpr a) : base(BqTag) { Field1 = a; }

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => "`" + Field1.ToString();

    [MirrorVerified("BqDifferentialTests")]
    public override Sexpr walk_expression(MplEnv env) =>
        MplBackquoteMacro.singleton.ExpandCall(env, this).walk_expression(env);
}

/// <summary>An unquoted expression inside a backquote, <c>,x</c>.</summary>
public partial class Cm : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Cm(object a) : base(CmTag) { Field1 = a as Sexpr; }

    [MirrorVerified("SexprDifferentialTests")]
    public Cm(Sexpr a) : base(CmTag) { Field1 = a; }

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => "," + Field1.ToString();
}

/// <summary>A spliced expression inside a backquote, <c>,@x</c>.</summary>
public partial class Ca : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Ca(object a) : base(CaTag) { Field1 = a as Sexpr; }

    [MirrorVerified("SexprDifferentialTests")]
    public Ca(Sexpr a) : base(CaTag) { Field1 = a; }

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => ",@" + Field1.ToString();
}

/// <summary>An append of S-expressions, printed <c>(SAPPEND a b …)</c>.</summary>
public partial class SAppend : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field2;

    [MirrorVerified("SexprDifferentialTests")]
    public SAppend(object first, object rest) : this(first as Sexpr, rest as Sexpr) { }

    [MirrorVerified("SexprDifferentialTests")]
    public SAppend(Sexpr first, Sexpr rest) : base(SAppendTag)
    {
        Field1 = first;
        Field2 = rest;
    }

    [MirrorVerified("SexprDifferentialTests")]
    public override string FormatAppend() => " " + Field1.ToString() + Field2.FormatAppend();

    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString() => "(SAPPEND " + Field1.ToString() + Field2.FormatAppend() + ")";
}

/// <summary>A <see cref="Cons"/> list spliced in front of an S-expression tail.</summary>
public partial class SList : Sexpr
{
    [MirrorVerified("SexprDifferentialTests")]
    public Cons Field1;

    [MirrorVerified("SexprDifferentialTests")]
    public Sexpr Field2;

    [MirrorVerified("SexprDifferentialTests")]
    public SList(object first, object tail) : this(first as Cons, tail as Sexpr) { }

    [MirrorVerified("SexprDifferentialTests")]
    public SList(Cons first, Sexpr tail) : base(SListTag)
    {
        Field1 = first;
        Field2 = tail;
    }

    [MirrorVerified("SexprDifferentialTests")]
    private static string FormatCons(Cons c) => c == null ? "" : " " + c.Field1.ToString() + FormatCons(c.Field2 as Cons);

    /// <summary>
    /// <c>(SLIST  a b)</c> when the tail is <see cref="Nul.singleton"/> itself (two spaces: one after the word, one
    /// before each item), otherwise <c>(SLIST#  a b tail)</c>.
    /// </summary>
    [MirrorVerified("SexprDifferentialTests")]
    public override string ToString()
    {
        if (Field2 == Nul.singleton)
        {
            return "(SLIST " + FormatCons(Field1) + ")";
        }
        return "(SLIST# " + FormatCons(Field1) + " " + Field2.ToString() + ")";
    }
}
