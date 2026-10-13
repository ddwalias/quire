using Quire.Mcd;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="Rational"/> against the contract in docs/mirror/evidence/Mcd.Rational.md. Expected values
/// are worked out by hand from that contract; the differential class RationalDifferentialTests compares the same
/// members with Mathcad directly.
/// </summary>
public class RationalTests
{
    private static void Is(int n, int d, Rational r)
    {
        Assert.Equal(n, r.Numerator);
        Assert.Equal(d, r.Denominator);
    }

    [Fact]
    public void Constructor_reduces_and_moves_the_sign_up()
    {
        Is(-3, 2, new Rational(6, -4));
        Is(1, 2, new Rational(-3, -6));
        Is(1, 2, new Rational(2, 4));
        Is(int.MinValue, 1, new Rational(int.MinValue, 1));
        Is(7, 1, new Rational(7));
    }

    [Fact]
    public void Constructor_keeps_zero_denominators_except_zero_over_zero()
    {
        Is(1, 0, new Rational(5, 0));
        Is(-1, 0, new Rational(-5, 0));
        Is(0, 1, new Rational(0, -5));
        Assert.Throws<DivideByZeroException>(() => new Rational(0, 0));
        Assert.Throws<OverflowException>(() => new Rational(1, int.MinValue));
        Assert.Throws<OverflowException>(() => new Rational(int.MinValue, -1));
    }

    [Fact]
    public void GCD_is_Euclid_on_signed_values()
    {
        Assert.Equal(6, Rational.GCD(-12, 18));
        Assert.Equal(5, Rational.GCD(0, -5));
        Assert.Equal(0, Rational.GCD(0, 0));
        Assert.Throws<OverflowException>(() => Rational.GCD(int.MinValue, 0));
    }

    [Fact]
    public void Arithmetic_wraps_then_reduces()
    {
        Is(0, 1, new Rational(65536) * new Rational(65536));
        Is(2, 1, new Rational(1, 2) * 4);
        Is(-1, 4, new Rational(1, 2) / -2);
        Is(-3, 2, new Rational(2, 3) / new Rational(-4, 9));
        Is(1, 0, new Rational(1, 2) / new Rational(0));
        Assert.Throws<DivideByZeroException>(() => new Rational(1, 0) * new Rational(0));
    }

    [Fact]
    public void Equality_is_field_wise()
    {
        Assert.True(default(Rational) == default);
        Assert.False(default(Rational) != default);
        Assert.True(new Rational(2, 4) == new Rational(1, 2));
        Assert.True(new Rational(1, 2).Equals(new Rational(1, 2)));
        Assert.False(new Rational(1).Equals(1));
    }

    [Fact]
    public void Ordering_goes_through_doubles_and_less_than_means_greater()
    {
        Assert.False(new Rational(1, 3) < new Rational(1, 2));
        Assert.True(new Rational(1, 2) < new Rational(1, 3));
        Assert.True(new Rational(1, 2) > new Rational(1, 3));
        Assert.True(new Rational(1, 2) >= new Rational(1, 2));
        Assert.True(new Rational(1, 3) <= new Rational(1, 2));
        Assert.True(new Rational(1, 0) > new Rational(5));
        Assert.False(default(Rational) <= default(Rational));
        Assert.False(default(Rational) > new Rational(1));
    }

    [Fact]
    public void Conversions()
    {
        Assert.Equal(0.75, (double)new Rational(3, 4));
        Assert.Equal(double.PositiveInfinity, (double)new Rational(1, 0));
        Assert.True(double.IsNaN((double)default(Rational)));
        Assert.Equal(3, (int)new Rational(7, 2));
        Assert.Equal(-3, (int)new Rational(-7, 2));
        Assert.Throws<DivideByZeroException>(() => (int)default(Rational));
        Is(5, 1, (Rational)5);
    }

    [Fact]
    public void Hash_is_the_truncated_value()
    {
        Assert.Equal(3, new Rational(7, 2).GetHashCode());
        Assert.Throws<DivideByZeroException>(() => default(Rational).GetHashCode());
    }

    [Fact]
    public void Text_and_integrality()
    {
        Assert.Equal("3", new Rational(3).ToString());
        Assert.Equal("-2/3", new Rational(4, -6).ToString());
        Assert.Equal("1/0", new Rational(1, 0).ToString());
        Assert.Equal("0/0", default(Rational).ToString());
        Assert.True(new Rational(4, 2).IsIntegral);
        Assert.False(new Rational(1, 0).IsIntegral);
    }

    [Fact]
    public void From_double_takes_the_first_close_convergent()
    {
        Is(1, 3, new Rational(1.0 / 3));
        Is(1, 2, 0.5);
        Is(-9, 4, new Rational(-2.25));
        Is(355, 113, new Rational(Math.PI));
        Is(22, 7, new Rational(Math.PI, 1e-2));
        Is(3, 1, new Rational(3.0));
        Is(0, 1, new Rational(0.5, 0.5));
    }

    [Fact]
    public void From_double_limits()
    {
        Is(0, 1, new Rational(double.NaN));
        Is(int.MaxValue, 1, new Rational(1e10));
        Is(-int.MaxValue, 1, new Rational(-1e10));
        Is(0, 1, new Rational(1.0 / (1 << 30), 0));
        Is(1, (1 << 30) - 1, new Rational(1.0 / ((1 << 30) - 1), 0));
    }
}
