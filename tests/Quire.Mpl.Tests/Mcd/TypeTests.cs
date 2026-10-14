using System.Text;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Type = Quire.Mcd.Type;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="Type"/> and its nested types against docs/mirror/evidence/Mcd.Type.md. Expected values are
/// worked out from that contract; TypeDifferentialTests compares with Mathcad directly.
/// </summary>
public class TypeTests
{
    public TypeTests() => RuntimeThread.Ensure();

    [Fact]
    public void Value_signature_is_an_application_of_V_to_a_product()
    {
        var t = (Type.Application)Type.Parse("V{Length 60000 Time-120000}");
        Assert.Equal("V", t.Operator.Name);
        var p = Assert.IsType<Type.Product>(Assert.Single(t.Operands));
        Assert.Equal([("Length", 60000), ("Time", -120000)], p.Powers.Select(x => (x.Base.Name, x.Exponent)));
        Assert.True(p.IsMonomorphic);
        Assert.Same(p, t.GetDimensionTypeProduct());
    }

    [Fact]
    public void Products_put_variables_first_and_default_exponents()
    {
        var p = (Type.Product)Type.Parse("{a b-3 Mass c}");
        // c comes after a constant, so the product stops there; the closing brace is skipped whatever it is
        Assert.Equal(["a", "b", "Mass"], p.Powers.Select(x => x.Base.Name));
        Assert.Equal([1, -3, 1], p.Powers.Select(x => x.Exponent));
        Assert.True(p.IsPolymorphic);
        Assert.Equal(-1, ((Type.Product)Type.Parse("{Length -}")).Powers[0].Exponent);
        Assert.True(((Type.Product)Type.Parse("{}")).IsDimensionless);
    }

    [Fact]
    public void Full_slices_are_the_same_array()
    {
        var all = (Type.Product)Type.Parse("{a b}");
        Assert.Same(all.Powers, all.Variables);
        Assert.Empty(all.Constants);
        var mixed = (Type.Product)Type.Parse("{a Length}");
        Assert.NotSame(mixed.Powers, mixed.Constants);
        Assert.Equal("Length", Assert.Single(mixed.Constants).Base.Name);
    }

    [Fact]
    public void Functions_tuples_and_lists()
    {
        Assert.Equal("a->b->c", Type.Parse("a->b->c").ToString());
        Assert.Equal("a->b", ((Type.Application)Type.Parse("(a->b)->c"))[1].ToString());
        Assert.Equal("()", Type.Parse("()").Name);
        Assert.IsType<Type.Variable>(Type.Parse("(a)"));
        Assert.Equal("(a, b, c)", Type.Parse("(a,b,c)").ToString());
        Assert.Equal("([] a)", Type.Parse("[a]").ToString());
        Assert.Equal("(F a b)", Type.Parse("F a b").ToString());
        Assert.Equal("(V {Length 60000} trailing)", Type.Parse("  V{Length 60000} trailing").ToString());   // juxtaposition is flat
    }

    [Fact]
    public void Syntax_errors_and_name_kinds()
    {
        var e = Assert.Throws<Exception>(() => Type.Parse(")"));
        Assert.Equal("ParseAtomic: syntax error", e.Message);
        Assert.Throws<Exception>(() => Type.Parse(""));
        Assert.IsType<Type.Constant>(Type.Parse("Θ"));
        Assert.IsType<Type.Variable>(Type.Parse("θ"));
        Assert.Throws<OverflowException>(() => Type.Parse("{Length 99999999999}"));
    }

    [Fact]
    public void Implicit_arguments()
    {
        foreach (var c in new[] { "C", "O", "P", "T", "U", "W" }) Assert.True(Type.Parse(c).IsImplicitArgument);
        Assert.False(Type.Parse("CO").IsImplicitArgument);
        Assert.False(Type.Parse("c").IsImplicitArgument);
        Assert.True(Type.Parse("I a").IsImplicitArgument);
        Assert.False(Type.Parse("{}").IsImplicitArgument);
    }

    [Fact]
    public void Indexer_reaches_operator_and_operands()
    {
        var t = Type.Parse("F a b");
        Assert.Equal("F", t[0].Name);
        Assert.Equal("b", t[2].Name);
        t[1] = new Type.Constant("Z");
        Assert.Equal("(F Z b)", t.ToString());
        Assert.Throws<InvalidCastException>(() => Type.Parse("a")[0]);
        Assert.Throws<IndexOutOfRangeException>(() => t[3]);
        Assert.Equal(2, ((Type.Application)t).Arity);
    }

    [Fact]
    public void Dimension_products_of_value_types()
    {
        Assert.Equal("{Time 60000}", Type.Parse("M(M(V{Time 60000}))").GetDimensionTypeProduct()!.ToString());
        var poly = Type.Parse("F a").GetDimensionTypeProduct()!;
        Assert.True(poly.IsPolymorphic);
        Assert.Equal("{a}", poly.ToString());
        var named = Type.Parse("R Length").GetDimensionTypeProduct()!;
        Assert.True(named.IsMonomorphic);
        Assert.Null(Type.Parse("a->b").GetDimensionTypeProduct());
        Assert.Null(Type.Parse("a").GetDimensionTypeProduct());
        Assert.Throws<InvalidCastException>(() => Type.Parse("V").GetDimensionTypeProduct());
    }

    [Fact]
    public void Format_names_variables_and_quantities()
    {
        Assert.Equal("ƒ(any1) → Length", Type.Parse("a->V{Length 60000}").Format());
        Assert.Equal("ƒ(any1, any2) → any3", Type.Parse("a->b->c").Format());
        Assert.Equal("any2", Type.Parse("(I a)->b").Format());
        Assert.Equal("[Length^2]", Type.Parse("M(V{Length 120000})").Format());
        Assert.Equal("any29 · Force", Type.Parse("{t3 Length 60000 Mass 60000 Time-120000}").Format());
        Assert.Equal("Unitless", Type.Parse("V{}").Format());
        Assert.Equal("(any1, Length)", Type.Parse("(a, V{Length 60000})").Format());
        Assert.Equal("F any1 any2", Type.Parse("F a b").Format());
        Assert.Throws<FormatException>(() => Type.Parse("t1x").Format());
        var sb = new StringBuilder(">");
        Type.Parse("V{Length 30000}").Format(sb, RuntimeOptions.s_default_unit_system);
        Assert.Equal(">Length^1/2", sb.ToString());
    }

    [Fact]
    public void Named_terms_compare_by_name()
    {
        var a = new Type.Constant("Length");
        var b = new Type.Variable("Length");
        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.True(a.CompareTo(new Type.Constant("Mass")) < 0);
        Assert.Throws<InvalidCastException>(() => a.Equals(Type.Parse("{}")));
        Assert.Throws<NullReferenceException>(() => a.Equals(null));
    }

    private sealed class Counter : Type.Visitor
    {
        public readonly List<string> Seen = [];
        public override void Visit(Type.Constant t) => Seen.Add("C:" + t.Name);
        public override void Visit(Type.Variable t) => Seen.Add("V:" + t.Name);
        public override void Visit(Type.Application t)
        {
            Seen.Add("A");
            Traverse(t.Operator);
            foreach (var o in t.Operands) Traverse(o);
        }
        public override void Visit(Type.Product t) => Seen.Add("P" + t.Powers.Length);
    }

    private sealed class Silent : Type.Visitor;

    [Fact]
    public void Accept_dispatches_to_the_visitor_overload()
    {
        var v = new Counter();
        v.Traverse(Type.Parse("F a (V{Length 60000})"));
        Assert.Equal(["A", "C:F", "V:a", "A", "C:V", "P1"], v.Seen);
        new Silent().Traverse(Type.Parse("F a {}")); // the base visits do nothing
    }

    [Fact]
    public void Factory_stack_discipline()
    {
        var f = new Type.Factory();
        f.Push(new Type.Constant("G"));
        f.Push(new Type.Constant("X"));
        f.Push(new Type.Constant("Y"));
        Assert.Equal("(G X Y)", f.NewApplication(3).ToString());
        f.Push(new Type.Constant("Length"), 60000);
        Assert.Throws<InvalidCastException>(() => f.Pop());
        var g = new Type.Factory();
        var k = g.NewConstant("K");
        Assert.Same(k, g.NewApplication(k, []));
        Assert.Equal("()", g.NewTuple(0).Name);
        Assert.Throws<InvalidOperationException>(() => g.Pop());
    }
}
