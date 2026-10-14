using System.Globalization;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Run;

/// <summary>
/// Behaviour of <see cref="Atom"/>, <see cref="Sym"/> and the constants against docs/mirror/evidence/Mpl.Run.Atom.md,
/// Mpl.Run.Sym.md and the constant records; SexprDifferentialTests compares the same members with Mathcad.
/// </summary>
public class AtomTests
{
    [Fact]
    public void Wrong_kind_accessors_return_defaults_without_throwing()
    {
        var s = new Sym("x", null);
        Assert.Equal(0.0, s.RealCon);
        Assert.Equal(0, s.IntCon);
        Assert.Equal('\0', s.CharCon);
        Assert.Null(s.StrCon);
        Assert.Equal(0, new int_constant(1).KeyNum);
        Assert.Equal(-1, s.KeyNum);
        Assert.Throws<AppException>(() => new char_constant('c').quote_atom);
        Assert.Same(s, s.sym);
        Assert.Null(new int_constant(1).sym);
        Assert.Null(s.Key);
    }

    [Fact]
    public void Only_the_tagged_constructor_keeps_the_qualifier()
    {
        Assert.Null(new Sym("a", "q").qualifier);
        Assert.Null(new Sym("a", "q", 7u).qualifier);
        Assert.Equal(7u, new Sym("a", "q", 7u).uid);
        var k = new Sym("a", "q", Atom.KeyTag);
        Assert.Equal("q", k.qualifier);
        Assert.Equal(Atom.KeyTag, k.m_tag);
        Assert.Equal("a.q", k.long_name);
        Assert.Equal("a", k.ToString());
        Assert.Null(Sym.NewTok("a", "q", 0u).sym.qualifier);
        Assert.Null(Sym.MakeSymQ("a", "q").sym.qualifier);
        Assert.Equal(3u, Sym.NewTok("a", "q", 9u, 3u).handle);
        Assert.Equal(9u, Sym.NewTok("a", 9u, 3u).sym.uid);
    }

    [Fact]
    public void Names_are_kept_as_given_and_not_interned()
    {
        Assert.Null(new Sym(5).name);
        Assert.Null(new Sym((object)null!).ToString());
        Assert.Equal("a b", new Sym("a b", null).ToString());
        Assert.NotSame(Sym.MakeSym("x").sym, Sym.MakeSym("x").sym);
        Assert.False(new Sym("x", null).Equals(new Sym("x", null)));
        var s = new Sym("x", null) { uid = 12 };
        Assert.Equal(12u, s.uid);
    }

    [Fact]
    public void Case_tests_look_at_the_first_character()
    {
        Assert.True(new Sym("Foo", null).UpId);
        Assert.False(new Sym("Foo", null).LowId);
        Assert.True(new Sym("foo", null).LowId);
        Assert.False(new Sym("_x", null).UpId);
        Assert.False(new Sym("_x", null).LowId);
        Assert.True(new Sym("Éa", null).UpId);
        Assert.Throws<ArgumentOutOfRangeException>(() => new Sym("", null).UpId);
        Assert.Throws<ArgumentNullException>(() => new Sym(null!, null).LowId);
    }

    [Fact]
    public void Symbols_quote_as_constructor_calls()
    {
        Assert.Equal("(Sym x)", new Sym("x", null).quote_atom.ToString());
        Assert.Equal("(make_symbolq x q)", new Sym("x", "q", 0).quote_atom.ToString());
        Assert.Equal("Sym", Sym.make_sym.ToString());
        Assert.Null(Sym.make_symq.sym.qualifier);
    }

    [Fact]
    public void Constants_parse_print_and_quote()
    {
        var saved = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Assert.Equal(17, new int_constant("17").Field1);
            Assert.Equal(0, new int_constant("abc").Field1);
            Assert.Equal(0, new int_constant("2147483648").Field1);
            Assert.Equal(0, new int_constant((string)null!).Field1);
            Assert.Equal(Atom.IntTag, new int_constant("1").m_tag);
            Assert.Equal(3.25, new real_constant("3.25").Field1);
            Assert.Equal(0.0, new real_constant("x").Field1);
            Assert.Equal(Atom.IntTag, new real_constant("3.25").m_tag);
            Assert.Equal(Atom.RealTag, new real_constant(3.25).m_tag);
            Assert.Equal("1.5", new real_constant(1.5).ToString());
            Assert.Equal("-7", new int_constant(-7).ToString());
            Assert.Equal("q\"", new str_constant("q\"").ToString());
            Assert.Null(new str_constant(null!).Field1);
            Assert.Equal("c", new char_constant('c').ToString());
            Assert.Equal('c', new char_constant('c').CharCon);
            Assert.Equal(Atom.CharTag, new char_constant('c').m_tag);
            Assert.Equal("(IntCon 4)", new int_constant(4).quote_atom.ToString());
            Assert.Equal("(RealCon 0.5)", new real_constant(0.5).quote_atom.ToString());
            Assert.Equal("(StrCon s)", new str_constant("s").quote_atom.ToString());
            Assert.Equal("#T2 4", int_constant.NewTok(4, 2).ToString());
            Assert.Equal("#T2 9", int_constant.NewTok("9", 2).ToString());
            Assert.Equal("#T2 0.5", real_constant.NewTok(0.5, 2).ToString());
            Assert.Equal("#T2 0.5", real_constant.NewTok("0.5", 2).ToString());
            Assert.Equal("#T2 s", str_constant.NewTok("s", 2).ToString());
            Assert.Equal("#T2 c", char_constant.NewTok('c', 2).ToString());
            Assert.Equal("0", int_constant.zero.ToString());
        }
        finally { CultureInfo.CurrentCulture = saved; }
    }

    [Fact]
    public void Tags_number_the_kinds()
    {
        Assert.Equal([0, 1, 2, 3, 4, 5], new[] { Atom.SymTag, Atom.IntTag, Atom.RealTag, Atom.CharTag, Atom.StrTag, Atom.KeyTag });
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8],
            new[] { Sexpr.NulTag, Sexpr.ConsTag, Sexpr.TokTag, Sexpr.QtTag, Sexpr.BqTag, Sexpr.CmTag, Sexpr.CaTag, Sexpr.SAppendTag, Sexpr.SListTag });
        Assert.Equal("Cons", SCons.token.ToString());
        Assert.Equal("apply", SCons.apply.name);
        Assert.Equal("Nul", Nul.token.ToString());
    }
}
