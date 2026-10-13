using System.Globalization;

namespace Quire.Mcd;

/// <summary>An exact fraction in lowest terms with a positive denominator; unit exponents use it.</summary>
public partial struct Rational
{
    public readonly int Numerator;
    public readonly int Denominator;

    public Rational(int numerator) : this(numerator, 1) { }

    public Rational(int numerator, int denominator)
    {
        if (denominator == 0) throw new DivideByZeroException("A rational number can't have a zero denominator");
        // In long, so that negating int.MinValue can't overflow before reducing.
        long n = numerator, d = denominator;
        if (d < 0) { n = -n; d = -d; }
        long g = Gcd(n, d);
        Numerator = checked((int)(n / g));
        Denominator = checked((int)(d / g));
    }

    public Rational(double xx) : this(xx, 1e-9) { }

    /// <summary>The fraction with the smallest denominator within <paramref name="err"/> of <paramref name="xx"/>,
    /// found from the continued-fraction expansion.</summary>
    public Rational(double xx, double err)
    {
        if (double.IsNaN(xx) || double.IsInfinity(xx) || Math.Abs(xx) > int.MaxValue)
            throw new OverflowException($"{xx} can't be represented as a rational number");
        long h0 = 0, h1 = 1, k0 = 1, k1 = 0;
        double x = xx;
        for (int i = 0; i < 64; i++)
        {
            double a = Math.Floor(x);
            long h2 = (long)a * h1 + h0, k2 = (long)a * k1 + k0;
            if (Math.Abs(h2) > int.MaxValue || k2 > int.MaxValue) break;
            (h0, h1, k0, k1) = (h1, h2, k1, k2);
            if (Math.Abs(xx - (double)h1 / k1) <= err || x == a) break;
            x = 1 / (x - a);
        }
        this = new Rational((int)h1, (int)k1);
    }

    public bool IsIntegral => Denominator == 1;

    public static implicit operator Rational(int i) => new(i);
    public static implicit operator Rational(double d) => new(d);
    public static implicit operator double(Rational r) => (double)r.Numerator / r.Denominator;

    public static explicit operator int(Rational r) =>
        r.IsIntegral ? r.Numerator : throw new InvalidCastException($"{r} is not an integer");

    public static bool operator ==(Rational a, Rational b) => a.Numerator == b.Numerator && a.Denominator == b.Denominator;
    public static bool operator !=(Rational a, Rational b) => !(a == b);
    public static bool operator <=(Rational a, Rational b) => Compare(a, b) <= 0;
    public static bool operator >=(Rational a, Rational b) => Compare(a, b) >= 0;
    public static bool operator <(Rational a, Rational b) => Compare(a, b) < 0;
    public static bool operator >(Rational a, Rational b) => Compare(a, b) > 0;

    public static Rational operator *(Rational a, int b) => a * new Rational(b);
    public static Rational operator /(Rational a, int b) => a / new Rational(b);

    /// <summary>Cancels across before multiplying, so the product overflows only when the result doesn't fit.</summary>
    public static Rational operator *(Rational a, Rational b)
    {
        long g1 = Gcd(a.Numerator, b.Denominator), g2 = Gcd(b.Numerator, a.Denominator);
        return new(checked((int)(a.Numerator / g1 * (b.Numerator / g2))), checked((int)(a.Denominator / g2 * (b.Denominator / g1))));
    }

    public static Rational operator /(Rational a, Rational b)
    {
        if (b.Numerator == 0) throw new DivideByZeroException("Division of a rational number by zero");
        long g1 = Gcd(a.Numerator, b.Numerator), g2 = Gcd(a.Denominator, b.Denominator);
        return new(checked((int)(a.Numerator / g1 * (b.Denominator / g2))), checked((int)(a.Denominator / g2 * (b.Numerator / g1))));
    }

    private static int Compare(Rational a, Rational b) =>
        ((long)a.Numerator * b.Denominator).CompareTo((long)b.Numerator * a.Denominator);

    public override bool Equals(object o) => o is Rational r && this == r;

    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

    public override string ToString() =>
        IsIntegral
            ? Numerator.ToString(CultureInfo.InvariantCulture)
            : $"{Numerator.ToString(CultureInfo.InvariantCulture)}/{Denominator.ToString(CultureInfo.InvariantCulture)}";

    public static int GCD(int m, int n) => checked((int)Gcd(m, n));

    private static long Gcd(long m, long n)
    {
        m = Math.Abs(m); n = Math.Abs(n);
        while (n != 0) (m, n) = (n, m % n);
        return m;
    }
}
