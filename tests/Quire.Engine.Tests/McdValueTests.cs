using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Engine.Tests;

public class McdValueTests
{
    [Fact]
    public void Dimensions_are_interned()
    {
        var a = Dim.Parse("length*mass/time^2");
        var b = Dim.Mult(Dim.Parse("length"), Dim.Div(Dim.Parse("mass"), Dim.Parse("time^2")));
        Assert.Same(a, b);
        Assert.Same(Dim.identity, Dim.Div(a, a));
        Assert.Same(Dim.identity, Dim.Parse("1"));
    }

    [Fact]
    public void Roots_of_dimensions_stay_exact()
    {
        var area = Dim.Parse("length^2");
        Assert.Same(Dim.Parse("length"), Dim.Root(area, 2));
        Assert.Same(area, Dim.Pwr(Dim.Pwr(area, 1.0 / 3), 3));
        Assert.Equal("L^2/3", Dim.Pwr(area, 1.0 / 3).Format());
        Assert.Throws<ArithmeticException>(() => Dim.Pwr(Dim.Parse("length"), Math.PI));
        Assert.Throws<ArithmeticException>(() => Dim.Pwr(Dim.Parse("length"), 0.3333333));
        Assert.Throws<ArithmeticException>(() => Dim.Pwr(Dim.Parse("length^5"), 0.10000001));
    }

    [Fact]
    public void Special_dimensions_have_no_coordinates()
    {
        Assert.True(Dim.IsSpecial(Dim.string_dim));
        Assert.False(Dim.IsDimensioned(Dim.identity));
        Assert.Null(Dim.polymorphic);
        Assert.Throws<ArithmeticException>(() => Dim.Mult(Dim.function_dim, Dim.identity));
    }

    [Fact]
    public void Signatures_round_trip()
    {
        var t = (Mcd.Type.Product)Mcd.Type.Parse("mass/length/time^2");
        Assert.Equal("length*mass/time^2", ((Mcd.Type.Product)Mcd.Type.Parse("length*mass/time^2")).Name);
        Assert.Same(Dim.Parse(t.Name), Dim.New(t));
        Assert.Equal("1/time", ((Mcd.Type.Product)Mcd.Type.Parse("1/time")).Name);
    }

    [Fact]
    public void Numbers_carry_complex_parts_and_dimensions()
    {
        var n = new NumVal(3, -4, Dim.Parse("length"));
        Assert.Equal(0, n.IsReal);
        Assert.Equal("3-4i [L]", n.ToString());
        Assert.Same(Dim.Parse("time"), ((NumVal)n.SetDim(Dim.Parse("time"))).m_dim);
        Assert.True(NumVal.FuzzyEquals(1.0, 1.0 + 1e-14));
        Assert.Same(NumVal.One, NumVal.McdBool(true));
    }

    [Fact]
    public void Matrices_store_mixed_units_as_pointers()
    {
        var m = new MatVal(2, 2) { m_dim = Dim.Parse("length") };
        m.set_2(0, 0, new NumVal(1, 0, Dim.Parse("length")));
        m.set_2(1, 1, new NumVal(2, 0, Dim.Parse("time")));
        Assert.True(m.HasPtrs);
        Assert.Same(Dim.mixed_matrix_dim, m.PhysicalDimension);
        Assert.True(double.IsNaN(m.m_rarr[m.Index(1, 1)]));
        Assert.Same(Dim.Parse("time"), ((NumVal)m.get_2(1, 1)).m_dim);

        var w = new MatVal(2, 3) { m_dim = Dim.Parse("length") };
        w.set_2(1, 2, new NumVal(7, 0, Dim.Parse("time")));
        w.set_2(0, 1, 3.0);
        var t = w.Transpose();
        Assert.Equal((3, 2), (t.Rows, t.Cols));
        Assert.Same(Dim.Parse("time"), ((NumVal)t.get_2(2, 1)).m_dim);
        Assert.Equal(7.0, t.get_real(2, 1));
        Assert.Equal(3.0, t.get_real(1, 0));
        Assert.Same(Dim.mixed_matrix_dim, t.PhysicalDimension);
        Assert.Throws<IndexOutOfRangeException>(() => new MatVal(0, 0).set_1(0, 1.0));
        m.set_2(1, 1, new NumVal(5, 0, Dim.Parse("length")));
        Assert.False(m.HasPtrs);
        Assert.Same(Dim.Parse("length"), m.PhysicalDimension);
    }

    [Fact]
    public void Matrices_are_column_major()
    {
        var m = Val.Create(2, 3, (r, c) => new NumVal(10 * r + c));
        Assert.Equal(12.0, m.m_rarr[m.Index(1, 2)]);
        Assert.Equal(1.0, ((NumVal)m.get_1(2)).Real);
        Assert.Equal([0.0, 1, 2], m.Row(0).Elements().Select(e => e.Real));
        Assert.Equal([2.0, 12], m.Column(2).Elements().Select(e => e.Real));
        Assert.Equal([11.0, 12], m.SubMatrix(1, 1, 1, 2).Elements().Select(e => e.Real));
    }

    [Fact]
    public void Ranges_are_arithmetic()
    {
        var r = RngVal.IncrRange(0, 0.5, 5, Dim.Parse("time"));
        Assert.Equal(5, r.Rsize);
        Assert.Equal(2.0, ((NumVal)r.range_get(4)).Real);
        var v = r.asVector("test", 0);
        Assert.Equal(5, v.Rows);
        Assert.Same(Dim.Parse("time"), v.PhysicalDimension);
    }

    [Fact]
    public void Unit_systems_prefer_defaults_then_basis()
    {
        var si = BuiltinUnits.SI;
        Assert.Equal("Pa", si.Simplify(BuiltinUnits.Pressure).ToString());
        Assert.Equal("m·s^-1", si.Simplify(BuiltinUnits.Velocity).ToString());
        var units = BuiltinUnits.USCS.Simplify(Dim.Parse("length^2").Coordinates);
        Assert.Equal(0.3048 * 0.3048, units.Magnitude, 12);
        Assert.Equal("ft^1/2", BuiltinUnits.USCS.Simplify(Dim.Root(Dim.Parse("length"), 2).Coordinates).ToString());
    }

    [Fact]
    public void Sexprs_print_as_Rl()
    {
        var e = SCons.list3(Sym.MakeSym("define"), new Tok(new Sym("a b", null), 0),
            SCons.list2(new Tok(new real_constant(1.5), 0), new Tok(new str_constant("q\""), 0)));
        Assert.Equal("(define |a b| (1.5 \"q\\\"\"))", Rl.Write(e));
        Assert.Equal(3u, e.Length);
        Assert.Equal("()", Rl.Write(Nul.singleton));
        // Mathcad's own ToString is debugging text: bare names, raw strings, Nul for the empty list.
        Assert.Equal("Nul", Nul.singleton.ToString());
        Assert.Equal("(define a b)", SCons.list2(Sym.MakeSym("define"), new Tok(new Sym("a b", null), 0)).ToString());
        Assert.Equal("define", e.car.sym.name);
        const string tricky = "(eval |a \\\\b\\|c|)";
        Assert.Equal(tricky, Rl.Write(RlReader.Read(tricky)));
        Assert.Equal("a \\b|c", RlReader.Read(tricky).scons.cdr.scons.car.sym.name);
    }
}
