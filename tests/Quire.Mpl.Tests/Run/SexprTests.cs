using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Run;

/// <summary>
/// Behaviour of the S-expression family against the contracts in docs/mirror/evidence/Mpl.Run.Sexpr.md and the
/// records of its subclasses. Expected values are worked out by hand from those contracts; SexprDifferentialTests
/// compares the same members with Mathcad directly.
/// </summary>
public class SexprTests
{
    private static Tok S(string name, uint h = 0) => new(new Sym(name, null), h);
    private static Tok I(int v, uint h = 0) => new(new int_constant(v), h);

    [Fact]
    public void Lists_print_as_debugging_text()
    {
        var e = SCons.list3(S("define"), S("x", 0x1F), SCons.list2(I(5), new Tok(new str_constant("a b"), 0)));
        Assert.Equal("(define #T1F x (5 a b))", e.ToString());
        Assert.Equal("Nul", Nul.singleton.ToString());
        Assert.Equal("", Nul.singleton.FormatTail());
        Assert.Equal(" Nul", Nul.singleton.FormatAppend());
        Assert.Equal("(a . 1)", new SCons(S("a"), I(1)).ToString());
        Assert.Equal(" a . 1", new SCons(S("a"), I(1)).FormatTail());
        Assert.Throws<NullReferenceException>(() => new SCons(S("a"), (Sexpr)null!).ToString());
    }

    [Fact]
    public void Quote_forms_print_with_their_prefix()
    {
        Assert.Equal("'x", new Qt(S("x")).ToString());
        Assert.Equal("`x", new Bq(S("x")).ToString());
        Assert.Equal(",x", new Cm(S("x")).ToString());
        Assert.Equal(",@(x)", new Ca(SCons.list1(S("x"))).ToString());
        Assert.Equal(Sexpr.QtTag, new Qt(S("x")).m_tag);
        Assert.Equal(Sexpr.CaTag, new Ca(S("x")).m_tag);
        Assert.Null(new Qt("not an expression").Field1);
    }

    [Fact]
    public void Appends_and_spliced_lists_print_their_parts()
    {
        Assert.Equal("(SAPPEND a b Nul)", new SAppend(S("a"), new SAppend(S("b"), Nul.singleton)).ToString());
        Assert.Equal(" a Nul", new SAppend(S("a"), Nul.singleton).FormatAppend());
        var items = new Cons(S("p"), new Cons(5, null));
        Assert.Equal("(SLIST  p 5)", new SList(items, Nul.singleton).ToString());
        Assert.Equal("(SLIST#  p 5 t)", new SList(items, S("t")).ToString());
        // Only the shared empty list counts as the end.
        Assert.Equal("(SLIST#  p 5 Nul)", new SList(items, new Nul()).ToString());
        Assert.Equal("(SLIST )", new SList(null!, Nul.singleton).ToString());
    }

    [Fact]
    public void Tokens_print_their_handle_in_hex()
    {
        Assert.Equal("x", S("x").ToString());
        Assert.Equal("#TFF x", S("x", 255).ToString());
        Assert.Equal("#TFFFFFFFF 1", new Tok(new int_constant(1), -1).ToString());
        Assert.Equal(uint.MaxValue, new Tok(new int_constant(1), -1).handle);
        Assert.Throws<NullReferenceException>(() => new Tok((Atom)null!, 0u).ToString());
        Assert.Null(new Tok("not an atom", 3).Field1);
    }

    [Fact]
    public void Accessors_follow_the_head()
    {
        var e = SCons.list3(S("f", 7), I(1, 8), I(2));
        Assert.Equal(7u, e.handle);
        Assert.Equal(0u, SCons.list1(SCons.list1(I(1))).handle);
        Assert.Equal("f", e.Key.name);
        Assert.Null(SCons.list1(I(1)).Key);
        Assert.Same(e, e.scons);
        Assert.Null(Nul.singleton.scons);
        Assert.Throws<AppException>(() => S("x").scons);
        Assert.Equal(3u, e.Length);
        Assert.Equal(1u, new SCons(S("a"), I(1)).Length);
        Assert.Same(e.Field2, e.cdr);
        Assert.Same(((SCons)e.Field2).Field1, e.second);
        Assert.Throws<NullReferenceException>(() => e.fourth);
        Assert.Throws<NullReferenceException>(() => new SCons((Sexpr)null!, Nul.singleton).handle);
    }

    [Fact]
    public void Checked_accessors_throw_AppException()
    {
        Assert.Equal("x", S("x").AsSym.name);
        Assert.Equal("s", new Tok(new str_constant("s"), 0).AsStr);
        Assert.Equal("x", S("x").AsKey.name);
        Assert.Throws<AppException>(() => I(1).AsSym);
        Assert.Throws<AppException>(() => I(1).AsStr);
        Assert.Throws<AppException>(() => Nul.singleton.AsKey);
        Assert.Throws<AppException>(() => S("x").quotation);
        Assert.Throws<AppException>(() => new SAppend(S("a"), Nul.singleton).quote);
        Assert.Null(S("x").str);
    }

    [Fact]
    public void Quote_builds_the_constructor_calls()
    {
        Assert.Same(Nul.token, Nul.singleton.quote);
        Assert.Equal("(#T5 Tok (IntCon 3) 0)", I(3, 5).quote.ToString());
        Assert.Equal("(Tok (Sym x) 0)", S("x").quote.ToString());
        Assert.Equal("(Cons (Tok (Sym x) 0) Nul)", SCons.list1(S("x")).quote.ToString());
        Assert.Equal("(#T3 Qt (Tok (Sym x) 0))", new Qt(S("x")).quote.ToString());
        Assert.Equal("(Tok (Sym x) 0)", new Qt(S("x")).quotation.ToString());
    }

    [Fact]
    public void Error_helpers_throw_AppException()
    {
        Action<Sexpr>[] helpers =
        [
            Sexpr.BadDeclaration, Sexpr.BadPattern, Sexpr.BadExpression, Sexpr.ExpectedAList, Sexpr.SyntaxError,
            Sexpr.BadApplication, Sexpr.NotASymbol, Sexpr.NotAString, Sexpr.NotAKeyword, Sexpr.NotImplemented,
            Sexpr.NotAQuotation, Sexpr.QuotationNotImplemented,
        ];
        foreach (var h in helpers)
        {
            Assert.Throws<AppException>(() => h(S("x", 4)));
            Assert.Throws<NullReferenceException>(() => h(null!));
        }
    }

    [Fact]
    public void Walking_expressions_without_macros()
    {
        var env = new LocalEnv();
        var ints = SCons.list3(I(1, 4), I(2), I(3));
        var applied = (SCons)ints.walk_expression(env);
        Assert.Equal("(#T4 apply #T4 1 2 3)", applied.ToString());
        Assert.Same(ints, applied.Field2);
        var upper = SCons.list2(S("Foo"), I(1));
        Assert.Same(upper, upper.walk_expression(env));
        var tok = S("Foo");
        Assert.Same(tok, tok.walk_expression(env));
        var one = I(1);
        Assert.Same(one, one.walk_expression(env));
        var q = new Qt(S("x"));
        Assert.Same(q, q.walk_expression(env));
        Assert.Throws<AppException>(() => Nul.singleton.walk_expression(env));
        Assert.Throws<NullReferenceException>(() => new SCons((Sexpr)null!, Nul.singleton).walk_expression(env));
        // An improper tail is dropped.
        Assert.Equal("(apply 1)", new SCons(I(1), I(2)).walk_expression(env).ToString());
        Assert.Same(Nul.singleton, SCons.walk_expressions(env, null!));
    }

    [Fact]
    public void Walking_patterns_without_macros()
    {
        var env = new LocalEnv();
        var pat = SCons.list3(S("Foo"), I(1), new Tok(new str_constant("s"), 0));
        var walked = (SCons)pat.walk_pattern(env);
        Assert.NotSame(pat, walked);
        Assert.Same(pat.Field2, walked.Field2);
        var one = I(1);
        Assert.Same(one, one.walk_pattern(env));
        Assert.Throws<AppException>(() => SCons.list2(I(1), I(2)).walk_pattern(env));
        Assert.Throws<AppException>(() => Nul.singleton.walk_pattern(env));
        Assert.Same(Nul.singleton, SCons.walk_patterns(env, null!));
        var nested = SCons.list2(SCons.list1(S("Bar")), I(2));
        Assert.NotSame(nested, SCons.walk_patterns(env, nested));
    }

    /// <summary>A block whose declarations are given, and a symbol that defines each one as a marker.</summary>
    private sealed class Block(Cons decls) : Sexpr(99)
    {
        public override Cons walk_declaration_lhs(MplEnv env) => decls;
    }

    private sealed class DefSym(string name) : Sym(name, null)
    {
        public List<MplEnv> Envs { get; } = [];

        public override SCons walk_definition_as(MplEnv env, SCons form)
        {
            Envs.Add(env);
            return SCons.list2(S("defined"), form);
        }

        public override Cons walk_declaration_as(MplEnv env, SCons form) => new(form, null);
    }

    [Fact]
    public void Walk_defines_each_declaration_in_a_new_scope()
    {
        var sym = new DefSym("d");
        var d1 = SCons.list2(new Tok(sym, 1), I(1));
        var d2 = SCons.list2(new Tok(sym, 2), I(2));
        var decls = new Cons(d1, new Cons(d2, null));
        var block = new Block(decls);
        var env = new LocalEnv();
        Cons result = block.walk(env, out MplEnv outenv);
        Assert.Same(decls, result);
        Assert.IsType<LocalEnv>(outenv);
        Assert.NotSame(env, outenv);
        Assert.Equal("(defined (#T1 d 1))", result.Field1.ToString());
        Assert.Equal("(defined (#T2 d 2))", result.tail.Field1.ToString());
        Assert.All(sym.Envs, e => Assert.Same(outenv, e));

        object[] pair = Sexpr.Walk(env, Nul.singleton);
        Assert.IsType<LocalEnv>(pair[0]);
        Assert.Null(pair[1]);
        Assert.Throws<NullReferenceException>(() => new Block(new Cons(SCons.list1(I(1)), null)).walk(env, out _));
    }

    [Fact]
    public void Declarations_go_through_the_heading_symbol()
    {
        var env = new LocalEnv();
        var form = SCons.list2(new Tok(new DefSym("d"), 0), I(1));
        Assert.Same(form, form.walk_declaration_lhs(env).Field1);
        Assert.Throws<AppException>(() => SCons.list1(I(1)).walk_declaration_lhs(env));
        Assert.Null(Nul.singleton.walk_declaration_lhs(env));
        Assert.Throws<AppException>(() => new Sym("x", null).walk_definition_as(env, form));
    }

    [Fact]
    public void Walk_with_bindings_walks_patterns_then_the_body()
    {
        var body = I(9);
        var pats = SCons.list2(I(1), I(2));
        Assert.Same(body, body.walk_with_bindings(new LocalEnv(), pats, out Sexpr xpats));
        Assert.Same(pats, xpats);
        Assert.Same(body, body.walk_with_bindings(new LocalEnv(), null!, out xpats));
        Assert.Same(Nul.singleton, xpats);
    }

    [Fact]
    public void Cons_lists_convert_to_S_expressions()
    {
        Assert.Same(Nul.singleton, SCons.as_Sexpr(null!));
        var s = (SCons)SCons.as_Sexpr(new Cons(S("a"), new Cons(5, null)));
        Assert.Equal("a", s.Field1.ToString());
        Assert.Null(s.tail.Field1);
        Assert.Same(Nul.singleton, s.tail.Field2);
    }

    [Fact]
    public void Constructors_keep_null_and_drop_non_expressions()
    {
        Assert.Null(new SCons(S("a"), (Sexpr)null!).Field2);
        var c = new SCons("x", 5);
        Assert.Null(c.Field1);
        Assert.Null(c.Field2);
        Assert.Null(new SAppend("x", S("b")).Field1);
        Assert.Null(new SList("x", Nul.singleton).Field1);
        Assert.Equal(Sexpr.ConsTag, c.m_tag);
        Assert.Equal(Sexpr.SListTag, new SList(null!, null!).m_tag);
        Assert.Equal(9, new Tag(9).m_tag);
    }
}
