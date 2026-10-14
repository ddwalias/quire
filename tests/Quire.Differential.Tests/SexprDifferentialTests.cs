using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares Quire's S-expression family (<c>Sexpr</c>, <c>Nul</c>, <c>SCons</c>, <c>Tok</c>, <c>Qt</c>, <c>Bq</c>,
/// <c>Cm</c>, <c>Ca</c>, <c>SAppend</c>, <c>SList</c>, <c>Tag</c>, <c>Atom</c>, <c>Sym</c> and the constants) with
/// mplrun's, member by member on the same trees. Results are compared as structure (type and fields), text or
/// exception type; the cases are listed in docs/mirror/evidence/Mpl.Run.Sexpr.md.
/// </summary>
public class SexprDifferentialTests
{
    private static Tok S(string? name, uint h = 0) => new(new Sym(name!, null), h);
    private static Tok I(int v, uint h = 0) => new(new int_constant(v), h);
    private static Tok R(double v, uint h = 0) => new(new real_constant(v), h);
    private static Tok Str(string? v, uint h = 0) => new(new str_constant(v!), h);
    private static SCons L(params Sexpr?[] xs)
    {
        Sexpr list = Nul.singleton;
        for (int i = xs.Length - 1; i >= 0; i--) list = new SCons(xs[i]!, list);
        return (SCons)list;
    }

    /// <summary>Trees covering every subclass, odd handles, null fields and improper lists.</summary>
    private static List<Sexpr> Trees()
    {
        var shared = S("shared", 3);
        return
        [
            Nul.singleton, new Nul(),
            S("define"), S("X", 0x1F), S("x", 5), S(""), S(null), S("Éa"), S("_u", 1),
            new Tok(new Sym("n", "q", 0), 0), new Tok(new Sym("k", "q", 5), 2), new Tok(new Sym("u", null, 77u), 0),
            I(42), I(-7, 300), R(1.5), R(double.NaN, 2), R(-0.0), R(1e300, 0xABCDEF),
            Str("a b"), Str("", 9), Str(null), new Tok(new char_constant('c'), 4), new Tok((Atom)null!, 0u),
            L(S("define", 1), S("x", 9), L(S("op_mult", 12), I(5, 20), S("m", 22))),
            new SCons(S("a", 1), I(1)), new SCons(S("a", 1), (Sexpr)null!), new SCons((Sexpr)null!, Nul.singleton),
            L(L(I(1), I(2)), I(3)), L(I(1), I(2), I(3)), L(S("Foo", 4), I(1), Str("s")), L(Str("h", 6), S("Foo")),
            L(S("a"), S("b"), S("c"), S("d"), S("e"), S("f")), L(S("x", 8)), L(shared, shared), L(L(S("Foo", 2)), I(4)),
            new Qt(L(S("a", 1), I(2))), new Qt(I(3, 3)), new Qt((Sexpr)null!), new Bq(S("y", 2)), new Cm(S("z")),
            new Ca(L(S("w"))), new SAppend(S("a"), new SAppend(S("b"), Nul.singleton)), new SAppend(S("a"), Nul.singleton),
            new SAppend(S("a"), S("b", 1)), new SList(new Cons(S("p"), new Cons(5, null)), Nul.singleton),
            new SList(new Cons(S("p"), null), S("t", 2)), new SList((Cons)null!, Nul.singleton),
            new SList(new Cons(S("p"), null), new Nul()), new SList(new Cons("x", "y"), Nul.singleton),
        ];
    }

    private static readonly string[] SexprMembers =
        ["ToString", "handle", "Key", "sym", "str", "AsSym", "AsStr", "AsKey", "quotation", "quote", "m_tag"];

    private static readonly string[] SConsMembers =
        ["Length", "head", "tail", "car", "cdr", "first", "second", "third", "fourth", "fifth"];

    [SkippableFact]
    public void Members_on_trees()
    {
        var diffs = new List<string>();
        foreach (var q in Trees())
        {
            var m = ToMathcad(q)!;
            string label = Outcome(() => q);
            Same(diffs, $"copy of {label}", () => q, () => m);
            foreach (string member in SexprMembers)
                Same(diffs, $"{label}.{member}", () => member == "ToString" ? q.ToString() : Get(q, member),
                    () => member == "ToString" ? m.ToString() : Get(m, member));
            Same(diffs, $"{label}.FormatTail", () => Call(q, "FormatTail", []), () => Call(m, "FormatTail", []));
            Same(diffs, $"{label}.FormatAppend", () => Call(q, "FormatAppend", []), () => Call(m, "FormatAppend", []));
            Same(diffs, $"{label}.scons is self", () => ScOutcome(q), () => ScOutcome(m));
            if (q is SCons)
                foreach (string member in SConsMembers)
                    Same(diffs, $"{label}.{member}", () => Get(q, member), () => Get(m, member));
            if (q is Tok)
                Same(diffs, $"{label}.atom", () => Get(q, "atom"), () => Get(m, "atom"));
        }
        NoDiffs(diffs);
    }

    private static object? ScOutcome(object o)
    {
        var s = Get(o, "scons");
        return s == null ? "null" : ReferenceEquals(s, o) ? "self" : s;
    }

    /// <summary>Walks that never reach the macro environment (whose Quire side belongs to another batch).</summary>
    [SkippableFact]
    public void Environment_free_walks()
    {
        var diffs = new List<string>();
        var qEnv = new LocalEnv();
        var mEnv = New(M("Mpl.Run.LocalEnv"), [])!;
        Sexpr[] expressions =
        [
            I(1, 2), S("Foo", 3), Str("s"), new Tok((Atom)null!, 0u), S(null), S(""), Nul.singleton,
            new Qt(S("x")), new Cm(S("x")), new SAppend(I(1), Nul.singleton), new SList(null!, Nul.singleton),
            L(I(1, 4), I(2), I(3)), L(L(I(1, 5), I(2)), I(3)), L(S("Foo", 6), I(1), I(2)), L(S("Foo"), L(I(1), I(2))),
            L(S("Foo")), L(Str("s", 7), S("Bar")), new SCons((Sexpr)null!, Nul.singleton), new SCons(I(1, 1), I(2)),
            L(L(S("Foo", 2)), I(4)),
        ];
        Sexpr[] patterns =
        [
            I(1), Str("s"), new Tok((Atom)null!, 0u), S(""), Nul.singleton, new Qt(S("x")), L(I(1, 3), I(2)),
            L(S("Foo", 4), I(1), Str("s")), L(S("Foo"), L(S("Bar", 2), I(2))), L(S("Foo")), new SCons(S("Foo"), I(2)),
        ];
        Sexpr[] declarations = [Nul.singleton, I(1), new Qt(S("x")), L(I(1, 6), I(2)), new SAppend(I(1), Nul.singleton)];
        foreach (var q in expressions)
            Walk(diffs, q, "walk_expression", qEnv, mEnv);
        foreach (var q in patterns)
            Walk(diffs, q, "walk_pattern", qEnv, mEnv);
        foreach (var q in declarations)
            Walk(diffs, q, "walk_declaration_lhs", qEnv, mEnv);

        var mSCons = M("Mpl.Run.SCons");
        foreach (var pats in new SCons?[] { null, L(I(1), Str("s")), L(L(S("Foo"), I(1)), I(2)) })
        {
            var mp = ToMathcad(pats);
            Same(diffs, $"walk_patterns({Outcome(() => pats)})", () => SameOrNew(pats, SCons.walk_patterns(qEnv, pats!)),
                () => SameOrNew(mp, Static(mSCons, "walk_patterns", ["MplEnv", "SCons"], mEnv, mp)));
            Same(diffs, $"walk_expressions({Outcome(() => pats)})", () => SameOrNew(pats, SCons.walk_expressions(qEnv, pats!)),
                () => SameOrNew(mp, Static(mSCons, "walk_expressions", ["MplEnv", "SCons"], mEnv, mp)));
            foreach (var body in new Sexpr[] { I(9, 1), Nul.singleton, L(I(1), I(2)) })
            {
                var mb = ToMathcad(body)!;
                object?[] qa = [qEnv, pats, null], ma = [mEnv, mp, null];
                Same(diffs, $"{Outcome(() => body)}.walk_with_bindings({Outcome(() => pats)})",
                    () => Pair(Call(body, "walk_with_bindings", ["MplEnv", "SCons", "Sexpr"], qa), qa[2]),
                    () => Pair(Call(mb, "walk_with_bindings", ["MplEnv", "SCons", "Sexpr"], ma), ma[2]));
            }
        }

        foreach (var block in new Sexpr[] { Nul.singleton, I(1), new Qt(S("x")) })
        {
            var mb = ToMathcad(block)!;
            Same(diffs, $"Walk({Outcome(() => block)})", () => Sexpr.Walk(qEnv, block),
                () => Static(M("Mpl.Run.Sexpr"), "Walk", ["MplEnv", "Sexpr"], mEnv, mb));
            object?[] qa = [qEnv, null], ma = [mEnv, null];
            Same(diffs, $"{Outcome(() => block)}.walk", () => Pair(Call(block, "walk", ["MplEnv", "MplEnv"], qa), qa[1]),
                () => Pair(Call(mb, "walk", ["MplEnv", "MplEnv"], ma), ma[1]));
        }

        var form = L(I(1, 6), I(2));
        var mForm = ToMathcad(form);
        Same(diffs, "Sym.walk_definition_as", () => new Sym("Foo", null).walk_definition_as(qEnv, form),
            () => Call(ToMathcad(new Sym("Foo", null))!, "walk_definition_as", ["MplEnv", "SCons"], mEnv, mForm));

        var cons = new Cons(S("a", 1), new Cons(7, new Cons(Nul.singleton, new Cons(L(I(1)), null))));
        foreach (var c in new[] { null, cons, new Cons(I(1), null) })
        {
            var mc = ToMathcad(c);
            Same(diffs, $"as_Sexpr({Outcome(() => c)})", () => SCons.as_Sexpr(c!),
                () => Static(mSCons, "as_Sexpr", ["Cons"], mc));
        }
        NoDiffs(diffs);
    }

    private static void Walk(List<string> diffs, Sexpr q, string how, object qEnv, object mEnv)
    {
        var m = ToMathcad(q)!;
        Same(diffs, $"{Outcome(() => q)}.{how}", () => SameOrNew(q, Call(q, how, ["MplEnv"], qEnv)),
            () => SameOrNew(m, Call(m, how, ["MplEnv"], mEnv)));
    }

    /// <summary>"same" when a walk returned its input, otherwise the result with "new" marking reused tails.</summary>
    private static object? SameOrNew(object? input, object? result) => ReferenceEquals(input, result) && input != null ? "same" : result;

    private static object?[] Pair(object? a, object? b) => [a, b];

    [SkippableFact]
    public void Error_helpers()
    {
        var diffs = new List<string>();
        string[] helpers =
        [
            "BadDeclaration", "BadPattern", "BadExpression", "ExpectedAList", "SyntaxError", "BadApplication",
            "NotASymbol", "NotAString", "NotAKeyword", "NotImplemented", "NotAQuotation", "QuotationNotImplemented",
        ];
        foreach (var form in new Sexpr?[] { I(1, 7), L(S("a", 9)), Nul.singleton, null })
        {
            var m = ToMathcad(form);
            foreach (string h in helpers)
                Same(diffs, $"{h}({Outcome(() => form)})", () => Static(typeof(Sexpr), h, ["Sexpr"], form),
                    () => Static(M("Mpl.Run.Sexpr"), h, ["Sexpr"], m));
        }
        NoDiffs(diffs);
    }

    private static readonly string[] AtomMembers =
        ["RealCon", "IntCon", "CharCon", "StrCon", "KeyNum", "quote_atom", "sym", "Key", "m_tag"];

    private static readonly string[] SymMembers = ["name", "qualifier", "UpId", "LowId", "long_name", "uid"];

    [SkippableFact]
    public void Atom_members()
    {
        var diffs = new List<string>();
        Atom[] atoms =
        [
            new Sym("abc", null), new Sym("Abc", null), new Sym("", null), new Sym(null!, null), new Sym("_x", null),
            new Sym("n", "q", 0), new Sym("k", "q", 5), new Sym("u", "q", 9u), new Sym("1a", null),
            new int_constant(0), new int_constant(-12), new int_constant(int.MinValue), new int_constant("17"),
            new real_constant(2.5), new real_constant(double.NegativeInfinity), new real_constant("3.25"),
            new real_constant(double.Epsilon), new str_constant("t"), new str_constant(""), new str_constant(null!),
            new char_constant('q'), new char_constant('\0'),
        ];
        foreach (var q in atoms)
        {
            var m = ToMathcad(q)!;
            string label = Outcome(() => q);
            Same(diffs, $"copy of {label}", () => q, () => m);
            Same(diffs, $"{label}.ToString", () => q.ToString(), () => m.ToString());
            foreach (string member in AtomMembers.Concat(q is Sym ? SymMembers : []))
                Same(diffs, $"{label}.{member}", () => Get(q, member), () => Get(m, member));
        }
        var qs = new Sym("v", null);
        var ms = ToMathcad(qs)!;
        qs.uid = 0xFFFF_FFFF;
        Set(ms, "uid", 0xFFFF_FFFFu);
        Same(diffs, "uid after set", () => qs, () => ms);
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Constructors_and_factories()
    {
        var diffs = new List<string>();
        void Ctor(string type, string[] sig, params object?[] args) =>
            Same(diffs, $"new {type}({string.Join(", ", args.Select(a => Outcome(() => a)))})",
                () => New(Q(type), sig, args), () => New(M(type), sig, args.Select(a => ToMathcad(a)).ToArray()));
        void Fact(string type, string name, string[] sig, params object?[] args) =>
            Same(diffs, $"{type}.{name}({string.Join(", ", args.Select(a => Outcome(() => a)))})",
                () => Static(Q(type), name, sig, args), () => Static(M(type), name, sig, args.Select(a => ToMathcad(a)).ToArray()));

        Ctor("Mpl.Run.Tag", ["Int32"], 9);
        Ctor("Mpl.Run.Tag", ["Int32"], -1);
        foreach (object? nm in new object?[] { "a", null, 5, new object(), "" })
            Ctor("Mpl.Run.Sym", ["Object"], nm);
        foreach (var (nm, q) in new[] { ("a", "b"), ("a", null), (null, "b"), ("", "") })
        {
            Ctor("Mpl.Run.Sym", ["String", "String"], nm, q);
            Ctor("Mpl.Run.Sym", ["String", "String", "UInt32"], nm, q, 42u);
            Ctor("Mpl.Run.Sym", ["String", "String", "Int32"], nm, q, 5);
            Fact("Mpl.Run.Sym", "NewTok", ["String", "UInt32"], nm, 3u);
            Fact("Mpl.Run.Sym", "NewTok", ["String", "String", "UInt32"], nm, q, 3u);
            Fact("Mpl.Run.Sym", "NewTok", ["String", "UInt32", "UInt32"], nm, 8u, 3u);
            Fact("Mpl.Run.Sym", "NewTok", ["String", "String", "UInt32", "UInt32"], nm, q, 8u, 3u);
            Fact("Mpl.Run.Sym", "MakeSym", ["String"], nm);
            Fact("Mpl.Run.Sym", "MakeSymQ", ["String", "String"], nm, q);
        }
        foreach (string? s in new[] { "12", " 12 ", "-5", "+7", "1e3", "abc", null, "2147483648", "1,000", "0x10", "" })
        {
            Ctor("Mpl.Run.int_constant", ["String"], s);
            Ctor("Mpl.Run.real_constant", ["String"], s);
            Fact("Mpl.Run.int_constant", "NewTok", ["String", "UInt32"], s, 1u);
            Fact("Mpl.Run.real_constant", "NewTok", ["String", "UInt32"], s, 1u);
            Ctor("Mpl.Run.str_constant", ["String"], s);
            Fact("Mpl.Run.str_constant", "NewTok", ["String", "UInt32"], s, 1u);
        }
        foreach (string s in new[] { "1.5", "NaN", "Infinity", "-0", "1e400", "∞" })
            Ctor("Mpl.Run.real_constant", ["String"], s);
        foreach (int v in new[] { 0, -1, int.MaxValue })
        {
            Ctor("Mpl.Run.int_constant", ["Int32"], v);
            Fact("Mpl.Run.int_constant", "NewTok", ["Int32", "UInt32"], v, (uint)v);
        }
        foreach (double v in new[] { 0.0, -0.0, double.NaN, 1e-320 })
        {
            Ctor("Mpl.Run.real_constant", ["Double"], v);
            Fact("Mpl.Run.real_constant", "NewTok", ["Double", "UInt32"], v, 2u);
        }
        Ctor("Mpl.Run.char_constant", ["Char"], 'z');
        Fact("Mpl.Run.char_constant", "NewTok", ["Char", "UInt32"], 'z', 0u);

        var atom = new Sym("a", null);
        foreach (object? a in new object?[] { atom, "not an atom", null, new int_constant(1) })
        {
            Ctor("Mpl.Run.Tok", ["Object", "Int32"], a, -1);
            Ctor("Mpl.Run.Tok", ["Object", "Int32"], a, 16);
        }
        Ctor("Mpl.Run.Tok", ["Atom", "Int32"], atom, int.MinValue);
        Ctor("Mpl.Run.Tok", ["Atom", "UInt32"], atom, uint.MaxValue);
        Ctor("Mpl.Run.Tok", ["Atom", "UInt32"], null, 0u);
        Ctor("Mpl.Run.Nul", []);

        var tok = S("e", 1);
        foreach (object? x in new object?[] { tok, "x", null, Nul.singleton })
        {
            Ctor("Mpl.Run.SCons", ["Object", "Object"], x, x);
            Ctor("Mpl.Run.SAppend", ["Object", "Object"], x, x);
            Ctor("Mpl.Run.SList", ["Object", "Object"], x, x);
            Ctor("Mpl.Run.SList", ["Object", "Object"], new Cons(x, null), x);
            foreach (string t in new[] { "Qt", "Bq", "Cm", "Ca" })
                Ctor("Mpl.Run." + t, ["Object"], x);
        }
        foreach (var x in new Sexpr?[] { tok, null })
        {
            Ctor("Mpl.Run.SCons", ["Sexpr", "Sexpr"], x, x);
            Ctor("Mpl.Run.SAppend", ["Sexpr", "Sexpr"], x, x);
            Ctor("Mpl.Run.SList", ["Cons", "Sexpr"], null, x);
            foreach (string t in new[] { "Qt", "Bq", "Cm", "Ca" })
                Ctor("Mpl.Run." + t, ["Sexpr"], x);
        }
        Fact("Mpl.Run.SCons", "list1", ["Sexpr"], tok);
        Fact("Mpl.Run.SCons", "list2", ["Sexpr", "Sexpr"], tok, null);
        Fact("Mpl.Run.SCons", "list3", ["Sexpr", "Sexpr", "Sexpr"], tok, I(2), tok);
        Fact("Mpl.Run.SCons", "list4", ["Sexpr", "Sexpr", "Sexpr", "Sexpr"], tok, I(2), Nul.singleton, L(tok));
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Constants_and_static_fields()
    {
        var diffs = new List<string>();
        (string Type, string[] Names)[] statics =
        [
            ("Mpl.Run.Sexpr", ["NulTag", "ConsTag", "TokTag", "QtTag", "BqTag", "CmTag", "CaTag", "SAppendTag", "SListTag"]),
            ("Mpl.Run.Atom", ["SymTag", "IntTag", "RealTag", "CharTag", "StrTag", "KeyTag"]),
            ("Mpl.Run.Nul", ["singleton", "token"]),
            ("Mpl.Run.SCons", ["token", "apply"]),
            ("Mpl.Run.Tok", ["tok"]),
            ("Mpl.Run.Qt", ["Qt_constructor"]),
            ("Mpl.Run.Sym", ["make_sym", "make_symq"]),
            ("Mpl.Run.int_constant", ["token", "zero"]),
            ("Mpl.Run.real_constant", ["token"]),
            ("Mpl.Run.str_constant", ["token"]),
        ];
        foreach (var (type, names) in statics)
            foreach (string n in names)
                Same(diffs, $"{type}.{n}", () => Get(Q(type), n), () => Get(M(type), n));
        Same(diffs, "Nul.singleton.quote is Nul.token", () => ReferenceEquals(Nul.singleton.quote, Nul.token),
            () => ReferenceEquals(Get(Get(M("Mpl.Run.Nul"), "singleton")!, "quote"), Get(M("Mpl.Run.Nul"), "token")));
        NoDiffs(diffs);
    }
}
