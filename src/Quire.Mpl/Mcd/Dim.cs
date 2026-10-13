#nullable disable
using System.Collections;
using System.Globalization;
using System.Text;

namespace Quire.Mcd;

/// <summary>
/// A physical dimension: the exponents of the seven SI base dimensions. Instances are interned, so two dimensions
/// are equal exactly when they are the same object. Exponents are stored as multiples of
/// 1/<see cref="Denominator"/>, which lets roots of dimensioned values (√m², ∛m³) stay exact. A null dimension is
/// <see cref="polymorphic"/>; <see cref="mcd_zero"/>, <see cref="string_dim"/>, <see cref="function_dim"/> and
/// <see cref="mixed_matrix_dim"/> are special markers without coordinates.
/// </summary>
public sealed partial class Dim
{
    /// <summary>Exponent scale: 2520 is divisible by every integer from 1 to 10.</summary>
    internal const int Denominator = 2520;

    /// <summary>The base dimensions, in coordinate order; unit signatures use these names.</summary>
    internal static readonly string[] BaseNames = ["length", "mass", "time", "current", "temperature", "substance", "luminosity"];

    private static readonly string[] Symbols = ["L", "M", "T", "I", "Θ", "N", "J"];

    private static readonly int s_rank = BaseNames.Length;
    private static readonly IDictionary s_map = new Hashtable(new Hash());

    public const Dim polymorphic = null;
    public static readonly Dim identity = intern(new int[s_rank]);
    public static readonly Dim mcd_zero = new();
    public static readonly Dim string_dim = new();
    public static readonly Dim function_dim = new();
    public static readonly Dim mixed_matrix_dim = new();

    private int[] m_vec;

    private Dim() { m_vec = null; }

    private Dim(int[] v) { m_vec = v; }

    /// <summary>The exponents, each scaled by <see cref="Denominator"/>; null for the special markers.</summary>
    public int[] Coordinates => m_vec == null ? null : (int[])m_vec.Clone();

    public static Dim intern(Dim d) => d?.m_vec == null ? d! : intern(d.m_vec);

    private static Dim intern(int[] v)
    {
        lock (s_map)
        {
            if (s_map[v] is Dim d) return d;
            var copy = (int[])v.Clone();
            d = new Dim(copy);
            s_map[copy] = d;
            return d;
        }
    }

    /// <summary>The dimension of a unit signature such as <c>length*mass/time^2</c>.</summary>
    public static Dim Parse(string signature) => New((Type.Product)Type.Parse(signature));

    public static Dim New(Type.Product p)
    {
        var v = new int[s_rank];
        foreach (var power in p.Powers)
        {
            int i = Array.IndexOf(BaseNames, power.Base.Name);
            if (i < 0) throw new FormatException($"'{power.Base.Name}' is not a base dimension");
            v[i] += checked(power.Exponent * Denominator);
        }
        return intern(v);
    }

    public static bool IsDimensioned(Dim x) => x?.m_vec != null && !ReferenceEquals(x, identity);

    public static bool Id_b(Dim x) => ReferenceEquals(x, identity);

    public static bool Eq_b(Dim x, Dim y) => ReferenceEquals(x, y);

    public static bool IsSpecial(Dim x) => x != null && x.m_vec == null;

    public static Dim Mult(Dim x, Dim y) => Combine(x, y, +1);

    public static Dim Div(Dim x, Dim y) => Combine(x, y, -1);

    private static Dim Combine(Dim x, Dim y, int sign)
    {
        Require(x, "multiply");
        Require(y, "multiply");
        var v = new int[s_rank];
        for (int i = 0; i < s_rank; i++) v[i] = checked(x.m_vec[i] + sign * y.m_vec[i]);
        return intern(v);
    }

    /// <summary>Raises a dimension to a power; fails when an exponent would not be a multiple of
    /// 1/<see cref="Denominator"/>.</summary>
    public static Dim Pwr(Dim x, double e)
    {
        Require(x, "raise");
        if (ReferenceEquals(x, identity)) return x;
        var v = new int[s_rank];
        for (int i = 0; i < s_rank; i++)
        {
            double scaled = x.m_vec[i] * e;
            double r = Math.Round(scaled);
            if (Math.Abs(scaled - r) > 1e-6 || Math.Abs(r) > int.MaxValue) // 1e-6/2520 in the exponent
                throw new ArithmeticException($"Units can't be raised to the power {e.ToString("G6", CultureInfo.InvariantCulture)}");
            v[i] = (int)r;
        }
        return intern(v);
    }

    public static Dim Root(Dim x, double n) => Pwr(x, 1 / n);

    private static void Require(Dim x, string op)
    {
        if (x?.m_vec == null) throw new ArithmeticException($"Can't {op} a {(x == null ? "polymorphic" : "special")} dimension");
    }

    public override bool Equals(object o) => ReferenceEquals(this, o);

    public override int GetHashCode() => m_vec == null ? base.GetHashCode() : Hash.GetHashCode(m_vec);

    public override string ToString() => Format();

    /// <summary>The dimension in base-dimension symbols, such as <c>L·M·T^-2</c>; <c>1</c> when dimensionless.</summary>
    public string Format()
    {
        if (m_vec == null)
        {
            return ReferenceEquals(this, string_dim) ? "string" : ReferenceEquals(this, function_dim) ? "function"
                : ReferenceEquals(this, mixed_matrix_dim) ? "mixed" : "0";
        }
        var sb = new StringBuilder();
        for (int i = 0; i < s_rank; i++)
        {
            if (m_vec[i] == 0) continue;
            if (sb.Length > 0) sb.Append('·');
            sb.Append(Symbols[i]);
            if (m_vec[i] != Denominator) sb.Append('^').Append(new Rational(m_vec[i], Denominator).ToString());
        }
        return sb.Length == 0 ? "1" : sb.ToString();
    }

    private sealed partial class Hash : IEqualityComparer
    {
        public Hash() { }

        public new bool Equals(object o1, object o2) => o1 is int[] u && o2 is int[] v && Equals(u, v);

        public int GetHashCode(object o) => GetHashCode((int[])o);

        public static bool Equals(int[] u, int[] v) => u.AsSpan().SequenceEqual(v);

        public static int GetHashCode(int[] v)
        {
            var h = new HashCode();
            foreach (int x in v) h.Add(x);
            return h.ToHashCode();
        }
    }
}
