using Quire.Mirror;

namespace Quire.Mcd;

/// <summary>
/// A fraction of two ints; unit exponents use it. Behaviour follows mplrun 12.0.1.0 member by member
/// (docs/mirror/evidence/Mcd.Rational.md), including its quirks: arithmetic wraps on overflow, comparisons go through
/// doubles, and <c>&lt;</c> answers <c>a &gt; b</c>.
/// </summary>
public partial struct Rational
{
    [MirrorVerified("RationalDifferentialTests")]
    public readonly int Numerator;
    [MirrorVerified("RationalDifferentialTests")]
    public readonly int Denominator;

    [MirrorVerified("RationalDifferentialTests")]
    public Rational(int numerator)
    {
        Numerator = numerator;
        Denominator = 1;
    }

    /// <summary>
    /// Reduces by the gcd, moving the sign to the numerator. A zero denominator is kept (<c>5/0</c> becomes
    /// <c>1/0</c>), but <c>0/0</c> divides by a zero gcd and throws.
    /// </summary>
    [MirrorVerified("RationalDifferentialTests")]
    public Rational(int numerator, int denominator)
    {
        int g = GCD(numerator, denominator);
        if (denominator < 0) g = -g;
        Numerator = numerator / g;
        Denominator = denominator / g;
    }

    [MirrorVerified("RationalDifferentialTests")]
    public Rational(double xx) : this(xx, 1E-05) { }

    /// <summary>
    /// The continued-fraction convergent of |<paramref name="xx"/>| that first comes within <paramref name="err"/>,
    /// with the sign applied last. Only the fractional part is expanded; expansion also stops once the remainder
    /// falls to 2^-30 or the next denominator (scaled by the whole part) would pass int.MaxValue.
    /// </summary>
    [MirrorVerified("RationalDifferentialTests")]
    public Rational(double xx, double err)
    {
        double magnitude = Math.Abs(xx);
        double whole = Math.Floor(magnitude);
        double fraction = magnitude - whole;
        double remainder = fraction;
        double previousNumerator = 1.0;
        double previousDenominator = 0.0;
        double numerator = 0.0;
        double denominator = 1.0;
        while (Math.Abs(numerator / denominator - fraction) > err && remainder > 9.313225746154785E-10)
        {
            remainder = 1.0 / remainder;
            double partial = Math.Floor(remainder);
            remainder -= partial;
            double nextNumerator = partial * numerator + previousNumerator;
            double nextDenominator = partial * denominator + previousDenominator;
            if (Math.Max(numerator + whole * nextDenominator, nextDenominator) > 2147483647.0)
            {
                break;
            }
            previousNumerator = numerator;
            previousDenominator = denominator;
            numerator = nextNumerator;
            denominator = nextDenominator;
        }
        Numerator = (int)(numerator + whole * denominator);
        Denominator = (int)denominator;
        if (xx < 0.0)
        {
            Numerator *= -1;
        }
    }

    [MirrorVerified("RationalDifferentialTests")]
    public bool IsIntegral => Denominator == 1;

    [MirrorVerified("RationalDifferentialTests")]
    public static implicit operator Rational(int i) => new(i);

    [MirrorVerified("RationalDifferentialTests")]
    public static implicit operator Rational(double d) => new(d);

    [MirrorVerified("RationalDifferentialTests")]
    public static implicit operator double(Rational r) => (double)r.Numerator / r.Denominator;

    /// <summary>Truncating division; throws for a zero denominator.</summary>
    [MirrorVerified("RationalDifferentialTests")]
    public static explicit operator int(Rational r) => r.Numerator / r.Denominator;

    /// <summary>Field by field, so unreduced values such as <c>default</c> (0/0) compare by representation.</summary>
    [MirrorVerified("RationalDifferentialTests")]
    public static bool operator ==(Rational a, Rational b) => a.Numerator == b.Numerator && a.Denominator == b.Denominator;

    [MirrorVerified("RationalDifferentialTests")]
    public static bool operator !=(Rational a, Rational b) => a.Numerator != b.Numerator || a.Denominator != b.Denominator;

    [MirrorVerified("RationalDifferentialTests")]
    public static bool operator <=(Rational a, Rational b) => (double)a <= (double)b;

    [MirrorVerified("RationalDifferentialTests")]
    public static bool operator >=(Rational a, Rational b) => (double)a >= (double)b;

    /// <summary>Mathcad's operator answers <c>a &gt; b</c>, and Quire keeps that. Compare the doubles for a real less-than.</summary>
    [MirrorVerified("RationalDifferentialTests")]
    public static bool operator <(Rational a, Rational b) => (double)a > (double)b;

    [MirrorVerified("RationalDifferentialTests")]
    public static bool operator >(Rational a, Rational b) => (double)a > (double)b;

    // The products below wrap on overflow (unchecked), then reduce.

    [MirrorVerified("RationalDifferentialTests")]
    public static Rational operator *(Rational a, int b) => new(unchecked(a.Numerator * b), a.Denominator);

    [MirrorVerified("RationalDifferentialTests")]
    public static Rational operator /(Rational a, int b) => new(a.Numerator, unchecked(a.Denominator * b));

    [MirrorVerified("RationalDifferentialTests")]
    public static Rational operator *(Rational a, Rational b) =>
        new(unchecked(a.Numerator * b.Numerator), unchecked(a.Denominator * b.Denominator));

    [MirrorVerified("RationalDifferentialTests")]
    public static Rational operator /(Rational a, Rational b) =>
        new(unchecked(a.Numerator * b.Denominator), unchecked(a.Denominator * b.Numerator));

    [MirrorVerified("RationalDifferentialTests")]
    public override bool Equals(object? o)
    {
        if (o is Rational)
        {
            return this == (Rational)o;
        }
        return false;
    }

    /// <summary>The truncated value, so it throws for a zero denominator, <c>default</c> included.</summary>
    [MirrorVerified("RationalDifferentialTests")]
    public override int GetHashCode() => Numerator / Denominator;

    /// <summary>"N", or "N/D" when the denominator isn't 1, in the current culture.</summary>
    [MirrorVerified("RationalDifferentialTests")]
    public override string ToString()
    {
        if (IsIntegral) return Numerator.ToString();
        return string.Format("{0}/{1}", Numerator, Denominator);
    }

    /// <summary>Euclid on the signed values; the result's sign is dropped at the end. GCD(0, 0) is 0.</summary>
    [MirrorVerified("RationalDifferentialTests")]
    public static int GCD(int m, int n)
    {
        while (n != 0)
        {
            int previousM = m;
            m = n;
            n = previousM;
            n %= m;
        }
        return Math.Abs(m);
    }
}
