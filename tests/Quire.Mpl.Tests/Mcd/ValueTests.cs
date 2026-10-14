using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="DimVal"/>, <see cref="NumVal"/>, <see cref="StrVal"/>, <see cref="RngVal"/> and the
/// iterators against docs/mirror/evidence/Mcd.DimVal.md, Mcd.NumVal.md and Mcd.RngVal.md. Expected values are worked
/// out from those contracts; the differential tests compare with Mathcad directly.
/// </summary>
public class ValueTests
{
    public ValueTests() => RuntimeThread.Ensure();

    private static readonly Dim Length = Dim.Parse("V{Length 60000}");

    private static unsafe (bool, int) IsInt(double d)
    {
        int r = -1;
        bool ok = new NumVal(d, 0, Dim.identity).isInt(&r);
        return (ok, r);
    }

    [Fact]
    public void Numbers_without_a_dimension_are_polymorphic()
    {
        Assert.Null(new NumVal(2.0).m_dim);
        Assert.Same(Dim.identity, new NumVal(2.0).PhysicalDimension);
        Assert.Same(Dim.identity, NumVal.RealDv(2).m_dim);
        Assert.Same(Dim.identity, ((NumVal)new NumVal(2.0).asResult()).m_dim);
        Assert.Null(new NumVal(new NumVal(1, 0, Length)).m_dim);   // copying keeps only the parts
        Assert.Same(Dim.mcd_zero, NumVal.Zero.m_dim);
        Assert.Equal(double.PositiveInfinity, NumVal.McdNaN);
    }

    [Fact]
    public void Integers_by_the_low_word_trick()
    {
        Assert.Equal((true, 7), IsInt(7));
        Assert.Equal((true, -1294967296), IsInt(3e9));                 // 3e9 fits the low word and wraps
        Assert.Equal((true, -3), IsInt(-3));                           // through the nearest-int fallback
        Assert.Equal((false, 2), IsInt(2.5));                          // ties go to even, and the int is still written
        Assert.Equal((true, 1), IsInt(1 + 1e-13));
        Assert.Throws<OverflowException>(() => IsInt(double.NaN));
        Assert.Throws<OverflowException>(() => IsInt(1e10));
        Assert.Equal("must be integer", Assert.Throws<AppException>(() => new NumVal(1, 1, Dim.identity).asInt("ignored", 3)).Message);
    }

    [Fact]
    public void Setters_and_booleans()
    {
        var n = new NumVal(9, 9, Length);
        n.Set(5);
        Assert.Equal((0.0, 9.0), (n.Real, n.Imag));                   // Set(int) clears the real part only
        Assert.Same(NumVal.One, NumVal.McdBool(false));
        Assert.Same(NumVal.Zero, NumVal.McdBool(null));
        Assert.Same(NumVal.One, NumVal.ibv(-1));
    }

    [Fact]
    public void Comparisons_are_fuzzy_unless_strict()
    {
        var a = new NumVal(1, 0, Length);
        var b = new NumVal(1 + 1e-13, 0, Length);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals(new NumVal(1, 0, Dim.mcd_zero)));
        Assert.False(a.Equals1(new NumVal(1, 0, Dim.mcd_zero)));
        Assert.False(NumVal.LessThan(a, b));
        Assert.True(NumVal.GreaterEqThan(a, b));
        RuntimeOptions.strict_comparisons = true;
        try
        {
            Assert.False(a.Equals(b));
            Assert.True(NumVal.LessThan(a, b));
        }
        finally { RuntimeOptions.strict_comparisons = false; }
        Assert.True(NumVal.LessThan(new NumVal(0, 1, Dim.identity), new NumVal(0, 2, Dim.identity)));
        Assert.Equal("incomparable", Assert.Throws<AppException>(() => NumVal.LessThan(new NumVal(1, 1, Dim.identity), NumVal.One)).Message);
        Assert.Equal("bad_dimensions\tLength\tUnitless", Assert.Throws<AppException>(() => NumVal.LessThan(a, NumVal.One)).Message);
    }

    [Fact]
    public void Formatting()
    {
        Assert.Equal("3 - 4i", new NumVal(3, -4, Length).ToString());
        Assert.Equal("3 - 4i{m}", new NumVal(3, -4, Length).Format(true));
        Assert.Equal("-4i", new NumVal(0, -4, Dim.identity).ToString());
        Assert.Equal("1.50 + 2.00i", new NumVal(1.5, 2, Dim.identity).ToString("F2"));
        Assert.Equal("1,5", new NumVal(1.5).ToString(System.Globalization.CultureInfo.GetCultureInfo("de-DE")));
        Assert.Equal("NaN", NumVal.mcNaN.ToString());
        Assert.Equal(0x7FF80000_12345678L, BitConverter.DoubleToInt64Bits(NumVal.NoNoNaN));
    }

    [Fact]
    public void Base_conversions_raise_message_keys()
    {
        var v = new StrVal("s");
        Assert.Equal("s", v.asString());
        Assert.Equal(("must be real", 0u), Raised(() => v.asReal()));
        Assert.Equal(("must be vector", 4u), Raised(() => v.asVector(4)));
        Assert.Equal(("incompatible shape", 7u), Raised(() => v.asTuple(1, 2, 7)));
        Assert.Equal(("bad_function\ts", 0u), Raised(() => v.asFunction()));
        Assert.Equal(("bad_function\ts", 0u), Raised(() => v.DynCall("x")));
        Assert.Equal(("must be real", 0u), Raised(() => NotAVal.singleton.asString()));   // the original's key
        Assert.Equal(("too_many_arguments1\t3", 2u), Raised(() => DimVal.WrongNumberOfArguments(1, 3, 2)));
        Assert.Equal(("too_few_arguments\t2\t2", 0u), Raised(() => DimVal.WrongNumberOfArguments(2, 2)));
        Assert.Throws<NullReferenceException>(() => DimVal.DimOf("not a value"));
        Assert.Equal(1, v.Rsize);
        Assert.Same(v, v.range_get(99));
    }

    private static (string, uint) Raised(Action f)
    {
        var e = Assert.Throws<AppException>(f);
        return (e.Message, e.Argument);
    }

    [Fact]
    public void Strings_are_raw_text()
    {
        Assert.Equal("a", new StrVal("a").ToString());
        Assert.Null(new StrVal(null!).ToString());
        Assert.Throws<NullReferenceException>(() => new StrVal(null!).GetHashCode());
        Assert.Same(Dim.string_dim, new StrVal("a").m_dim);
        Assert.Equal("Not A Value", NotAVal.singleton.ToString());
    }

    [Fact]
    public void Ranges_step_or_interpolate()
    {
        var plain = RngVal.IncrRange(0, 1, 10.0, Length);
        Assert.IsNotType<RngValInterpolate>(plain);
        Assert.Equal(11, plain.Rsize);
        Assert.True(plain.Integral);
        var tenths = RngVal.IncrRange(0, 0.1, 1.0, Length);
        Assert.IsType<RngValInterpolate>(tenths);
        Assert.Equal(1.0, tenths.get(10));                          // exact at the end
        Assert.IsNotType<RngValInterpolate>(RngVal.IncrRange(1, -0.1, 0.0, Length));   // descending spans never interpolate
        Assert.IsNotType<RngValInterpolate>(tenths.asResult());
        Assert.Equal(-1.0, ((NumVal)plain.range_get(-1)).Real);     // indices are not checked
        Assert.Equal("illegal range", Assert.Throws<AppException>(() => RngVal.IncrRange(0, 0, 1.0, Length)).Message);
        Assert.Equal("range too many elements", Assert.Throws<AppException>(() => RngVal.IncrRange(0, 1, double.PositiveInfinity, Length)).Message);
        Assert.Equal("The arguments must be sequential values",
            Assert.Throws<AppException>(() => RngVal.mkVec3(NumVal.RealDv(0), NumVal.RealDv(1), NumVal.RealDv(-1))).Message);
        Assert.Throws<NullReferenceException>(() => RngVal.IncrRange(0, 1, 3, null!).GetHashCode());
    }

    [Fact]
    public void Iteration()
    {
        var values = new List<double>();
        for (Iterator? it = Iterator.forRange(1, 0.5, 3); it != null; it = it.next()) values.Add(((NumVal)it.first()).Real);
        Assert.Equal([1, 1.5, 2], values);
        var seq = Iterator.forValues([Iterator.forRange(0, 1, 2), new Iterator.ScalarIterator("x")]);
        var seen = new List<object>();
        for (Iterator? it = seq; it != null; it = it.next()) seen.Add(it.first() is NumVal n ? n.Real : it.first());
        Assert.Equal([0.0, 1.0, "x"], seen);
        Assert.IsType<Iterator.ScalarIterator>(Iterator.forValue("text"));
        var counting = Iterator.forRange(5, 1, 2);
        Assert.Equal(5.0, ((NumVal)Iterator.it_incr(null!, counting)).Real);
        Assert.Equal(6.0, ((NumVal)Iterator.it_incr(null!, counting)).Real);
    }
}
