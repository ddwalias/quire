using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Engine.Tests;

public class McdValueTests
{
    private static readonly Dim Length = Dim.Parse(BuiltinUnits.Length), Time = Dim.Parse(BuiltinUnits.Time);

    public McdValueTests() => RuntimeThread.Ensure();

    [Fact]
    public void Dimensions_are_interned()
    {
        var a = Dim.Parse(BuiltinUnits.Force);
        var b = Dim.Mult(Length, Dim.Div(Dim.Parse(BuiltinUnits.Mass), Dim.Parse("V{Time 120000}")));
        Assert.Same(a, b);
        Assert.Same(Dim.identity, Dim.Div(a, a));
        Assert.Same(Dim.identity, Dim.Parse(BuiltinUnits.Unitless));
    }

    [Fact]
    public void Roots_of_dimensions_stay_exact()
    {
        var area = Dim.Parse(BuiltinUnits.Area);
        Assert.Same(Length, Dim.Root(area, 2));
        Assert.Same(area, Dim.Pwr(Dim.Pwr(area, 1.0 / 3), 3));
        Assert.Equal("Length^2/3", Dim.Pwr(area, 1.0 / 3).Format());
    }

    [Fact]
    public void Inexact_unit_powers_are_calculation_errors()
    {
        Assert.Throws<CalcException>(() => Ops.Pwr(Length, Math.PI));
        Assert.Throws<CalcException>(() => Ops.Pwr(Length, 0.3333333));
        Assert.Throws<CalcException>(() => Ops.Pwr(Length, 1.0 / 7));
        Assert.Same(Dim.Parse("V{Length 20000}"), Ops.Pwr(Length, 1.0 / 3));
        Assert.Same(Dim.identity, Ops.Pwr(Dim.string_dim, Math.PI)); // markers have zero coordinates
    }

    [Fact]
    public void Markers_have_zero_coordinates()
    {
        Assert.True(Dim.IsSpecial(Dim.string_dim));
        Assert.False(Dim.IsDimensioned(Dim.identity));
        Assert.False(Dim.IsDimensioned(Dim.function_dim));
        Assert.Null(Dim.polymorphic);
        Assert.Same(Dim.function_dim, Dim.Mult(Dim.function_dim, Dim.identity));
    }

    [Fact]
    public void Signatures_round_trip()
    {
        var t = Mcd.Type.Parse(BuiltinUnits.Pressure);
        Assert.Equal("(V {Length -60000 Mass 60000 Time -120000})", t.ToString());
        Assert.Equal("{Length -60000 Mass 60000 Time -120000}", Dim.Parse(BuiltinUnits.Pressure).ToString());
        Assert.Same(Dim.Parse(BuiltinUnits.Pressure), Dim.New(t.GetDimensionTypeProduct()));
        Assert.Equal("Pressure", Dim.Parse(BuiltinUnits.Pressure).Format());
    }

    [Fact]
    public void Numbers_carry_complex_parts_and_dimensions()
    {
        var n = new NumVal(3, -4, Length);
        Assert.Equal(0, n.IsReal);
        Assert.Equal("3 - 4i", n.ToString());               // Mathcad's ToString leaves units out
        Assert.Equal("3 - 4i{m}", n.Format(true));
        Assert.Same(Time, ((NumVal)n.SetDim(Time)).m_dim);
        Assert.True(NumVal.FuzzyEquals(1.0, 1.0 + 1e-14));
        Assert.Same(NumVal.One, NumVal.McdBool(false));     // any non-null object is true
    }

    [Fact]
    public void Matrices_store_mixed_units_as_pointers()
    {
        var m = new MatVal(2, 2) { m_dim = Length };
        m.set_2(0, 0, new NumVal(1, 0, Length));
        m.set_2(1, 1, new NumVal(2, 0, Time));
        Assert.True(m.HasPtrs);
        Assert.Same(Dim.mixed_matrix_dim, m.PhysicalDimension);
        Assert.True(double.IsNaN(m.m_rarr[m.Index(1, 1)]));
        Assert.Same(Time, ((NumVal)m.get_2(1, 1)).m_dim);

        var w = new MatVal(2, 3) { m_dim = Length };
        w.set_2(1, 2, new NumVal(7, 0, Time));
        w.set_2(0, 1, 3.0);
        var t = w.Transpose();
        Assert.Equal((3, 2), (t.Rows, t.Cols));
        Assert.Same(Time, ((NumVal)t.get_2(2, 1)).m_dim);
        Assert.True(double.IsNaN(t.get_real(2, 1)));
        Assert.Equal(7.0, t.get_real_mixed(2, 1));
        Assert.Equal(3.0, t.get_real(1, 0));
        Assert.Same(Dim.mixed_matrix_dim, t.PhysicalDimension);
        Assert.Throws<IndexOutOfRangeException>(() => new MatVal(0, 0).set_1(0, 1.0));
        m.set_2(1, 1, new NumVal(5, 0, Length));
        Assert.False(m.HasPtrs);
        Assert.Same(Length, m.PhysicalDimension);
    }

    [Fact]
    public void Matrices_are_column_major()
    {
        var m = Val.Create(2, 3, (r, c) => new NumVal(10 * r + c));
        Assert.Equal(12.0, m.m_rarr[m.Index(1, 2)]);
        Assert.Equal(1.0, ((NumVal)m.get_1(2)).Real);
        Assert.Equal([0.0, 1, 2], m.Row(0).Elements().Select(e => e.Real));
        Assert.Equal([2.0, 12], m.Column(2).Elements().Select(e => e.Real));
        Assert.Equal([11.0, 12], m.SubMatrix(1, 2, 1, 3).Elements().Select(e => e.Real));
    }

    [Fact]
    public void Ranges_are_arithmetic()
    {
        var r = RngVal.IncrRange(0, 0.5, 5, Time);
        Assert.Equal(5, r.Rsize);
        Assert.Equal(2.0, ((NumVal)r.range_get(4)).Real);
        var v = r.asVector("test", 0);
        Assert.Equal(5, v.Rows);
        Assert.Same(Time, v.PhysicalDimension);
    }

    [Fact]
    public void Unit_systems_prefer_defaults_then_basis()
    {
        var si = BuiltinUnits.SI;
        Assert.Equal("{Pa}", si.Simplify(BuiltinUnits.Pressure).ToString());
        Assert.Equal("{m s -1}", si.Simplify(BuiltinUnits.Velocity).ToString());
        var units = BuiltinUnits.USCS.Simplify(Dim.Parse(BuiltinUnits.Area).Coordinates);
        Assert.Equal(0.3048 * 0.3048, units.Magnitude, 12);
        Assert.Equal("{ft 1/2}", BuiltinUnits.USCS.Simplify(Dim.Root(Length, 2).Coordinates).ToString());
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
