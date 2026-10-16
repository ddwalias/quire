#nullable disable
using System.Globalization;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>The real and imaginary parts of a number, in SI base units (docs/mirror/evidence/Mcd.NumVal.md).</summary>
[Serializable]
public partial struct num_val
{
    [MirrorVerified("NumValDifferentialTests")]
    public double rp;

    [MirrorVerified("NumValDifferentialTests")]
    public double ip;

    [MirrorVerified("NumValDifferentialTests")]
    public num_val(double r)
    {
        rp = r;
        ip = 0.0;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public num_val(double r, double i)
    {
        rp = r;
        ip = i;
    }
}

/// <summary>
/// A complex number with a dimension; the parts are in SI base units. Constructors without a dimension leave it
/// null (polymorphic); <see cref="NumDv"/>, <see cref="RealDv"/> and <see cref="IntDv"/> make unitless numbers.
/// Comparisons are fuzzy (relative 5e-13) unless the thread's <see cref="RuntimeOptions.strict_comparisons"/> is
/// set. Behaviour follows mplrun 12.0.1.0 (docs/mirror/evidence/Mcd.NumVal.md).
/// </summary>
[Serializable]
public sealed partial class NumVal : DimVal
{
    public const double normalize_exponent = 4503599627370496.0;
    public const double machine_eps = 2.220446049250313E-16;
    public const double default_fuzz = 5E-13;

    [MirrorVerified("NumValDifferentialTests")]
    public num_val Data;

    /// <summary>Zero of the <see cref="Dim.mcd_zero"/> marker, compatible with any dimension.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal Zero;

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal ZeroId;

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal ZeroPoly;

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal One;

    [MirrorVerified("NumValDifferentialTests")]
    public static NotAVal NoValue;

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal mcNaN;

    [MirrorVerified("NumValDifferentialTests")]
    public static double Inf;

    /// <summary>Positive infinity, despite the name.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static double McdNaN;

    [MirrorVerified("NumValDifferentialTests")]
    public static double NaN;

    /// <summary>A quiet NaN with payload 0x12345678.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static double NoNoNaN;

    [MirrorVerified("NumValDifferentialTests")]
    static NumVal()
    {
        Inf = double.PositiveInfinity;
        McdNaN = double.PositiveInfinity;
        NaN = double.NaN;
        Zero = new NumVal(0.0, 0.0, Dim.mcd_zero);
        ZeroId = new NumVal(0.0, 0.0, Dim.identity);
        ZeroPoly = new NumVal(0);
        One = new NumVal(1.0, 0.0, Dim.identity);
        mcNaN = new NumVal(NaN, 0.0, Dim.mcd_zero);
        NoNoNaN = LoadNaN(0x12345678);
        NoValue = NotAVal.singleton;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal() { }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal(int i) { Data.rp = i; }

    /// <summary>Copies the parts only: the copy is polymorphic.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public NumVal(NumVal n) { Data = n.Data; }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal(NumVal n, Dim d)
    {
        Data = n.Data;
        m_dim = d;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal(double r) { Data.rp = r; }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal(double r, double i)
    {
        Data.rp = r;
        Data.ip = i;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal(double r, double i, Dim d)
    {
        Data.rp = r;
        Data.ip = i;
        m_dim = d;
    }

    /// <summary>The dimension, unitless when polymorphic.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override Dim PhysicalDimension
    {
        get
        {
            if (!IsPolymorphic) return m_dim;
            return Dim.identity;
        }
    }

    /// <summary><see cref="True.singleton"/> when the imaginary part is zero, else null.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public object IsPureReal => Data.ip != 0.0 ? null : True.singleton;

    [MirrorVerified("NumValDifferentialTests")]
    public object IsPureImag => Data.rp != 0.0 ? null : True.singleton;

    [MirrorVerified("NumValDifferentialTests")]
    public double Real
    {
        get => Data.rp;
        set => Data.rp = value;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public double Imag
    {
        get => Data.ip;
        set => Data.ip = value;
    }

    /// <summary>1 for a real whose (saturating) int conversion is exact, else 0.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public int IsInt => Data.ip == 0.0 && (int)Data.rp == Data.rp ? 1 : 0;

    [MirrorVerified("NumValDifferentialTests")]
    public override int IsReal => Data.ip == 0.0 ? 1 : 0;

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal NumDv(double r, double i) => new(r, i, Dim.identity);

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal IntDv(int i) => new(i, 0.0, Dim.identity);

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal RealDv(double r) => new(r, 0.0, Dim.identity);

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal Set(double r)
    {
        Data.rp = r;
        Data.ip = 0.0;
        return this;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public NumVal Set(double r, double i)
    {
        Data.rp = r;
        Data.ip = i;
        return this;
    }

    /// <summary>Leaves the real part 0 and the imaginary part unchanged, whatever <paramref name="i"/> is.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public NumVal Set(int i)
    {
        Data.rp = i;
        Data.rp = 0.0;
        return this;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool ExactEquals(NumVal x, NumVal y) => x.Data.rp == y.Data.rp && x.Data.ip == y.Data.ip;

    [MirrorVerified("NumValDifferentialTests")]
    public static bool FuzzyEquals(double x, double y) => FuzzyEquals(x, y, default_fuzz);

    /// <summary>Equal, or differing by less than <paramref name="fuzz"/> times |x + y|.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static bool FuzzyEquals(double x, double y, double fuzz)
    {
        if (x == y) return true;
        return Math.Abs(x - y) < fuzz * Math.Abs(x + y);
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool FuzzyEquals(NumVal x, NumVal y) => FuzzyEquals(x, y, default_fuzz);

    /// <summary>Two reals compare as doubles; otherwise exactly equal parts, or the summed part differences under
    /// <paramref name="fuzz"/> times the summed absolute part sums.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static bool FuzzyEquals(NumVal x, NumVal y, double fuzz)
    {
        if (x.Data.ip == 0.0 && y.Data.ip == 0.0)
        {
            if (x.Data.rp == y.Data.rp) return true;
            return Math.Abs(x.Data.rp - y.Data.rp) < fuzz * Math.Abs(x.Data.rp + y.Data.rp);
        }
        if (x.Data.rp == y.Data.rp && x.Data.ip == y.Data.ip) return true;
        return Math.Abs(x.Data.rp - y.Data.rp) + Math.Abs(x.Data.ip - y.Data.ip) < fuzz * (Math.Abs(x.Data.rp + y.Data.rp) + Math.Abs(x.Data.ip + y.Data.ip));
    }

    /// <summary>Compatible dimensions (<see cref="Dim.Eq_b"/>) and equal values, exact or fuzzy by the thread's
    /// strictness.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is not NumVal n) return false;
        if (Dim.Eq_b(m_dim, n.m_dim))
        {
            if (RuntimeOptions.strict_comparisons) return ExactEquals(this, n);
            return FuzzyEquals(this, n);
        }
        return false;
    }

    /// <summary>As <see cref="Equals(object)"/>, but the zero marker isn't compatible with other dimensions.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public bool Equals1(object obj)
    {
        if (obj is not NumVal n) return false;
        if (Dim.Eq_b1(m_dim, n.m_dim))
        {
            if (RuntimeOptions.strict_comparisons) return ExactEquals(this, n);
            return FuzzyEquals(this, n);
        }
        return false;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public override bool nearly_equal(DimVal obj, double fuzz)
    {
        if (obj is not NumVal n) return false;
        if (Dim.Eq_b(m_dim, n.m_dim)) return FuzzyEquals(this, n, fuzz);
        return false;
    }

    /// <summary>Less by more than the fuzz (any amount when strict).</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static bool LT(double n1, double n2) =>
        n1 < n2 && (RuntimeOptions.strict_comparisons || !(n2 - n1 < default_fuzz * Math.Abs(n1 + n2)));

    [MirrorVerified("NumValDifferentialTests")]
    public static bool GT(double n1, double n2) =>
        n1 > n2 && (RuntimeOptions.strict_comparisons || !(n1 - n2 < default_fuzz * Math.Abs(n1 + n2)));

    /// <summary>Less or equal, or (not strict) greater by less than the fuzz.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static bool LE(double n1, double n2)
    {
        if (n1 <= n2) return true;
        if (RuntimeOptions.strict_comparisons) return false;
        return n1 - n2 < default_fuzz * Math.Abs(n1 + n2);
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool GE(double n1, double n2)
    {
        if (n1 >= n2) return true;
        if (RuntimeOptions.strict_comparisons) return false;
        return n2 - n1 < default_fuzz * Math.Abs(n1 + n2);
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool LessThan(NumVal n1, NumVal n2)
    {
        Dim.Eq(n1.m_dim, n2.m_dim);
        if (n1.Data.ip == 0.0 && n2.Data.ip == 0.0)
        {
            return LT(n1.Data.rp, n2.Data.rp);
        }
        if (n1.Data.rp == 0.0 && n2.Data.rp == 0.0)
        {
            return LT(n1.Data.ip, n2.Data.ip);
        }
        AppException.Raise("incomparable");
        return false;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool GreaterThan(NumVal n1, NumVal n2)
    {
        Dim.Eq(n1.m_dim, n2.m_dim);
        if (n1.Data.ip == 0.0 && n2.Data.ip == 0.0)
        {
            return GT(n1.Data.rp, n2.Data.rp);
        }
        if (n1.Data.rp == 0.0 && n2.Data.rp == 0.0)
        {
            return GT(n1.Data.ip, n2.Data.ip);
        }
        AppException.Raise("incomparable");
        return false;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool LessEqThan(NumVal n1, NumVal n2)
    {
        Dim.Eq(n1.m_dim, n2.m_dim);
        if (n1.Data.ip == 0.0 && n2.Data.ip == 0.0)
        {
            return LE(n1.Data.rp, n2.Data.rp);
        }
        if (n1.Data.rp == 0.0 && n2.Data.rp == 0.0)
        {
            return LE(n1.Data.ip, n2.Data.ip);
        }
        AppException.Raise("incomparable");
        return false;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static bool GreaterEqThan(NumVal n1, NumVal n2)
    {
        Dim.Eq(n1.m_dim, n2.m_dim);
        if (n1.Data.ip == 0.0 && n2.Data.ip == 0.0)
        {
            return GE(n1.Data.rp, n2.Data.rp);
        }
        if (n1.Data.rp == 0.0 && n2.Data.rp == 0.0)
        {
            return GE(n1.Data.ip, n2.Data.ip);
        }
        AppException.Raise("incomparable");
        return false;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public override int GetHashCode() => Data.rp.GetHashCode() ^ Data.ip.GetHashCode();

    /// <summary>A copy with dimension <paramref name="d"/>; this number is unchanged.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override DimVal SetDim(Dim d) => new NumVal(this, d);

    /// <summary>
    /// Rounds <paramref name="r"/> to the nearest int (ties to even; <see cref="OverflowException"/> outside the int
    /// range or for NaN) into <paramref name="res"/>, and says whether it lies within |r|·2⁻⁴² of it.
    /// </summary>
    [MirrorVerified("NumValDifferentialTests")]
    private static unsafe bool isNearInt(int* res, double r)
    {
        double fuzz = 2.2737367544323206E-13;
        double bound = Math.Abs(r) * fuzz;
        *res = Convert.ToInt32(r);
        if (Math.Abs(*res - r) < bound) return true;
        return false;
    }

    /// <summary>
    /// A real is an integer when adding 2⁵² leaves it in the low 32 bits of the sum: for whole numbers in
    /// [0, 2³²) that holds, and the low word is returned as an int (so values from 2³¹ wrap negative). Anything else
    /// falls back to <see cref="isNearInt"/>. Complex numbers are not integers.
    /// </summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override unsafe bool isInt(int* res)
    {
        if (Data.ip == 0.0)
        {
            double normalized = Data.rp + normalize_exponent;
            if (Data.rp == *(uint*)&normalized)
            {
                *res = *(int*)&normalized;
                return true;
            }
            return isNearInt(res, Data.rp);
        }
        return false;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public override double asReal(string m, uint a)
    {
        if (Data.ip != 0.0) AppException.Raise(m, a);
        return Data.rp;
    }

    /// <summary><see cref="One"/> for any non-null object (false included), else <see cref="Zero"/>.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal McdBool(object o)
    {
        if (o != null) return One;
        return Zero;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal ibv(int o)
    {
        if (o != 0) return One;
        return Zero;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public string Format(string numeric_format, bool show_dimension, bool simplify) =>
        Format(NumberFormatInfo.InvariantInfo, numeric_format, show_dimension, simplify);

    /// <summary>
    /// <c>a</c>, <c>bi</c>, <c>a + bi</c> or <c>a - bi</c> with each part in <paramref name="numeric_format"/>; with
    /// <paramref name="show_dimension"/> and a real dimension, the parts are first divided by the magnitude of its
    /// units and the units text (<c>{m s -1}</c>) is appended without a space.
    /// </summary>
    [MirrorVerified("NumValDifferentialTests")]
    public string Format(IFormatProvider culture, string numeric_format, bool show_dimension, bool simplify)
    {
        string units = null;
        double re = Data.rp;
        double im = Data.ip;
        if (show_dimension && Dim.IsDimensioned(m_dim))
        {
            var u = GetUnits(simplify);
            re /= u.magnitude;
            im /= u.magnitude;
            units = u.ToString();
        }
        string realFormat = string.Format("{{0:{0}}}", numeric_format);
        string imagFormat = string.Format("{{1:{0}}}i", numeric_format);
        string subFormat = string.Format("{0} - {1}", realFormat, imagFormat);
        string addFormat = string.Format("{0} + {1}", realFormat, imagFormat);
        string pattern;
        if (im == 0.0)
        {
            pattern = realFormat;
        }
        else if (re == 0.0)
        {
            pattern = imagFormat;
        }
        else if (im < 0.0)
        {
            im = -im;
            pattern = subFormat;
        }
        else
        {
            pattern = addFormat;
        }
        string formatted = string.Format(culture, pattern, re, im);
        if (units != null) return string.Format("{0}{1}", formatted, units);
        return formatted;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public string Format(bool show_dimension) => Format("g15", show_dimension, false);

    [MirrorVerified("NumValDifferentialTests")]
    public string Format(bool show_dimension, bool simplify) => Format("g15", show_dimension, simplify);

    [MirrorVerified("NumValDifferentialTests")]
    public string Format(string numeric_format, bool show_dimension) => Format(numeric_format, show_dimension, false);

    [MirrorVerified("NumValDifferentialTests")]
    public string ToString(IFormatProvider culture, string numeric_format) => Format(culture, numeric_format, false, false);

    [MirrorVerified("NumValDifferentialTests")]
    public string ToString(string numeric_format) => Format(numeric_format, false, false);

    [MirrorVerified("NumValDifferentialTests")]
    public override string ToString(IFormatProvider culture) => ToString(culture, "g15");

    /// <summary>The value in g15, invariant culture, without units.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override string ToString() => ToString("g15");

    /// <summary>This number, or a unitless copy when it is polymorphic.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override DimVal asResult()
    {
        if (m_dim != null) return this;
        return new NumVal(Data.rp, Data.ip, Dim.FixDimension(m_dim));
    }

    /// <summary>A quiet NaN whose low word is <paramref name="payload"/>.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static unsafe double LoadNaN(int payload)
    {
        double result;
        int* words = (int*)&result;
        words[0] = payload;
        words[1] = 0x7FF80000;
        return result;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public static object NotNoValue(object o)
    {
        if (o == NoValue) AppException.Raise("invalid");
        return o;
    }

    /// <summary>Both parts divided, keeping the dimension.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public static NumVal operator /(NumVal n, double scalar) => (NumVal)new NumVal(n.Data.rp / scalar, n.Data.ip / scalar).dv(n.m_dim);
}

/// <summary>A string value; its dimension is <see cref="Dim.string_dim"/> (docs/mirror/evidence/Mcd.NumVal.md).</summary>
[Serializable]
public partial class StrVal : DimVal
{
    [MirrorVerified("NumValDifferentialTests")]
    public string Field1;

    [MirrorVerified("NumValDifferentialTests")]
    public StrVal(string str)
    {
        Field1 = str;
        m_dim = Dim.string_dim;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public override string IsString => Field1;

    [MirrorVerified("NumValDifferentialTests")]
    public string m_string => Field1;

    /// <summary>The text, whatever the message.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override string asString(string m, uint a) => Field1;

    /// <summary>The text itself, unquoted.</summary>
    [MirrorVerified("NumValDifferentialTests")]
    public override string ToString() => Field1;

    [MirrorVerified("NumValDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is not StrVal s) return false;
        return Field1 == s.Field1;
    }

    [MirrorVerified("NumValDifferentialTests")]
    public override int GetHashCode() => Field1.GetHashCode();
}

/// <summary>The "no value" marker (<see cref="NumVal.NoValue"/>).</summary>
public partial class NotAVal : DimVal
{
    [MirrorVerified("NumValDifferentialTests")]
    public static readonly NotAVal singleton;

    [MirrorVerified("NumValDifferentialTests")]
    static NotAVal() { singleton = new NotAVal(); }

    [MirrorVerified("NumValDifferentialTests")]
    public NotAVal() { }

    [MirrorVerified("NumValDifferentialTests")]
    public override string ToString() => "Not A Value";
}
