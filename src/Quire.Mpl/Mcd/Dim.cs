#nullable disable
using System.Collections;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>
/// A physical dimension: one integer coordinate per base dimension of the unit system (60000 per unit power, so
/// fractional powers stay integral). Dimensions built from coordinates are interned, so equal coordinates give the
/// same object. Null is <see cref="polymorphic"/>. <see cref="mcd_zero"/> (the dimension of a zero that adapts to
/// anything), <see cref="string_dim"/>, <see cref="function_dim"/> and <see cref="mixed_matrix_dim"/> are markers:
/// distinct, uninterned objects with all-zero coordinates. Behaviour follows mplrun 12.0.1.0
/// (docs/mirror/evidence/Mcd.Dim.md).
/// </summary>
/// <remarks>
/// The interning table is shared and unlocked; the scratch vector <see cref="s_vec"/> is per thread and exists only
/// on the thread that initialised the type and on threads that called <see cref="ThreadInit"/> (which
/// <see cref="RuntimeOptions.SyncToThread"/> does). <see cref="Mult"/>, <see cref="Div"/> and <see cref="Pwr"/>
/// need it.
/// </remarks>
[Serializable]
public sealed partial class Dim
{
    [MirrorVerified("DimDifferentialTests")]
    public static readonly Dim identity;
    [MirrorVerified("DimDifferentialTests")]
    public const Dim polymorphic = null;
    [MirrorVerified("DimDifferentialTests")]
    public static readonly Dim mcd_zero;
    [MirrorVerified("DimDifferentialTests")]
    public static readonly Dim string_dim;
    [MirrorVerified("DimDifferentialTests")]
    public static readonly Dim function_dim;
    [MirrorVerified("DimDifferentialTests")]
    public static readonly Dim mixed_matrix_dim;

    [MirrorTested("Mcd.Dim")]
    private static readonly int s_rank;
    [MirrorTested("Mcd.Dim")]
    private static readonly IDictionary s_map;

    [ThreadStatic]
    [MirrorVerified("DimDifferentialTests")]
    public static int[] s_vec;

    [MirrorTested("Mcd.Dim")]
    private int[] m_vec;

    /// <summary>The rank comes from the initialising thread's unit system.</summary>
    [MirrorTested("Mcd.Dim")]
    static Dim()
    {
        s_rank = RuntimeOptions.the_unit_system.Rank;
        s_map = new Hashtable(64, new Hash());
        s_vec = new int[s_rank];
        identity = intern();
        mcd_zero = new Dim();
        string_dim = new Dim();
        function_dim = new Dim();
        mixed_matrix_dim = new Dim();
    }

    /// <summary>Gives the calling thread a scratch vector if it has none.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static void ThreadInit()
    {
        s_vec ??= new int[s_rank];
    }

    /// <summary>An interned dimension; registers itself under <paramref name="v"/>.</summary>
    [MirrorTested("Mcd.Dim")]
    private Dim(int[] v)
    {
        m_vec = v;
        s_map.Add(v, this);
    }

    /// <summary>A marker: zero coordinates, not interned.</summary>
    [MirrorTested("Mcd.Dim")]
    private Dim() { m_vec = new int[s_rank]; }

    /// <summary>The dimension with coordinates <paramref name="v"/>, which a new one keeps (so when that is the
    /// thread's scratch vector, the thread gets a fresh one).</summary>
    [MirrorVerified("DimDifferentialTests")]
    private static Dim intern(int[] v)
    {
        if (s_map[v] is Dim d) return d;
        if (v == s_vec) s_vec = new int[s_rank];
        return new Dim(v);
    }

    [MirrorVerified("DimDifferentialTests")]
    private static Dim intern() => intern(s_vec);

    /// <summary>The coordinates as a dimension product, in the calling thread's unit system.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public Type.Product Product => RuntimeOptions.the_unit_system.GetDimensions(m_vec);

    /// <summary>The coordinate array itself, not a copy.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public int[] Coordinates => m_vec;

    /// <summary>Equal coordinates: markers equal <see cref="identity"/> and each other.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public override bool Equals(object o) => o is Dim d && Hash.Equals(m_vec, d.m_vec);

    [MirrorVerified("DimDifferentialTests")]
    public override int GetHashCode() => Hash.GetHashCode(m_vec);

    /// <summary>The product in signature syntax, such as <c>{Length 60000 Time -120000}</c>.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public override string ToString() => Product.ToString();

    /// <summary><c>String</c>, <c>Function</c>, or the product's display form (<c>Unitless</c>, <c>Force</c>, …).</summary>
    [MirrorVerified("DimDifferentialTests")]
    public string Format()
    {
        if (this == string_dim) return "String";
        if (this == function_dim) return "Function";
        return Product.Format();
    }

    [MirrorVerified("DimDifferentialTests")]
    public static void OnError(string m, Dim d, uint a) => OnError(m, identity, d, a);

    /// <summary>Raises <see cref="AppException"/> "<paramref name="m"/>\t<paramref name="d"/>\t<paramref name="e"/>"
    /// (formatted, null as unitless) for argument <paramref name="a"/>.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static void OnError(string m, Dim d, Dim e, uint a) =>
        AppException.Raise($"{m}\t{FixDimension(d).Format()}\t{FixDimension(e).Format()}", a);

    [MirrorVerified("DimDifferentialTests")]
    public static Dim Parse(string signature) => New(Type.Parse(signature).GetDimensionTypeProduct());

    /// <summary>Null for a polymorphic product, <see cref="identity"/> for a dimensionless one, else the interned
    /// dimension; a null product throws <see cref="NonCriticalException"/>.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim New(Type.Product p)
    {
        if (p == null) throw new NonCriticalException(NonCriticalException.ArgumentIsNull);
        if (p.IsPolymorphic) return null;
        if (p.IsDimensionless) return identity;
        return intern(RuntimeOptions.the_unit_system.GetCoordinates(p));
    }

    /// <summary><see cref="mcd_zero"/> becomes <see cref="identity"/>; anything else is returned as is.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim intern(Dim d) => d == mcd_zero ? identity : d;

    [MirrorVerified("DimDifferentialTests")]
    public static Dim ValDim(Dim x) => x == mcd_zero ? identity : x;

    /// <summary>Has a nonzero coordinate (so never null or a marker).</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static bool IsDimensioned(Dim x)
    {
        if (x == null) return false;
        for (int i = 0; i != s_rank; i++)
            if (x.m_vec[i] != 0) return true;
        return false;
    }

    /// <summary>Acceptable where no dimension is: identity, polymorphic or <see cref="mcd_zero"/>.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static bool Id_b(Dim x) => x == identity || x == null || x == mcd_zero;

    /// <summary>Compatible: the same object, or either is polymorphic or <see cref="mcd_zero"/>.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static bool Eq_b(Dim x, Dim y) => x == y || x == null || y == null || x == mcd_zero || y == mcd_zero;

    /// <summary>The same object, or either is polymorphic.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static bool Eq_b1(Dim x, Dim y) => x == y || x == null || y == null;

    [MirrorVerified("DimDifferentialTests")]
    public static bool NonScalar_b(Dim x) => x == function_dim || x == string_dim || x == mixed_matrix_dim;

    [MirrorVerified("DimDifferentialTests")]
    public static bool IsSpecial(Dim x) => x == mcd_zero || x == null || NonScalar_b(x);

    [MirrorVerified("DimDifferentialTests")]
    public static Dim Id(Dim x) => Id(x, 1u);

    /// <summary><see cref="identity"/> when <see cref="Id_b"/> holds; otherwise raises "bad_dimensions_unitless"
    /// for argument <paramref name="a"/>.</summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim Id(Dim x, uint a)
    {
        if (Id_b(x)) return identity;
        OnError("bad_dimensions_unitless", x, a);
        return null;
    }

    [MirrorVerified("DimDifferentialTests")]
    public static Dim Eq(Dim x, Dim y) => Eq(x, y, 2u);

    /// <summary>
    /// The common dimension of compatible <paramref name="x"/> and <paramref name="y"/>: the other one when either
    /// is <see cref="mcd_zero"/> (x first), x when they are the same or y is polymorphic, y when x is polymorphic.
    /// Otherwise raises "bad_dimensions" for argument <paramref name="arg"/>.
    /// </summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim Eq(Dim x, Dim y, uint arg)
    {
        if (x == mcd_zero) return y;
        if (y == mcd_zero) return x;
        if (x == y || y == null) return x;
        if (x == null) return y;
        OnError("bad_dimensions", x, y, arg);
        return null;
    }

    [MirrorVerified("DimDifferentialTests")]
    public static Dim FixDimension(Dim d) => d ?? identity;

    /// <summary>
    /// The product. An identity or polymorphic operand gives the other one (polymorphic winning, with
    /// <see cref="mcd_zero"/> becoming identity); otherwise the coordinates add, markers counting as zero.
    /// </summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim Mult(Dim x, Dim y)
    {
        if (x == identity || y == null) return y == mcd_zero ? identity : y;
        if (y == identity || x == null) return x == mcd_zero ? identity : x;
        for (int i = 0; i != s_rank; i++) s_vec[i] = x.m_vec[i] + y.m_vec[i];
        return intern();
    }

    /// <summary>
    /// The quotient: x when y is identity or x polymorphic, polymorphic when y is, identity when they are the same
    /// object; otherwise the coordinates subtract, markers counting as zero.
    /// </summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim Div(Dim x, Dim y)
    {
        if (y == identity || x == null) return x;
        if (y == null) return null;
        if (x == y) return identity;
        for (int i = 0; i != s_rank; i++) s_vec[i] = x.m_vec[i] - y.m_vec[i];
        return intern();
    }

    /// <summary>
    /// x itself for identity, polymorphic or <paramref name="e"/> = 1; identity for e = 0; otherwise each coordinate
    /// times e, rounded half to even and converted to int (NaN to 0, out-of-range values saturating). Inexact powers
    /// are not detected.
    /// </summary>
    [MirrorVerified("DimDifferentialTests")]
    public static Dim Pwr(Dim x, double e)
    {
        if (x == identity || x == null || e == 1.0) return x;
        if (e == 0.0) return identity;
        for (int i = 0; i != s_rank; i++) s_vec[i] = (int)Math.Round(x.m_vec[i] * e, 0);
        return intern();
    }

    [MirrorVerified("DimDifferentialTests")]
    public static Dim Root(Dim x, double n) => Pwr(x, 1.0 / n);

    /// <summary>Coordinate vectors compared over the first <see cref="s_rank"/> entries.</summary>
    private sealed partial class Hash : IEqualityComparer
    {
        [MirrorVerified("DimDifferentialTests")]
        public Hash() { }

        [MirrorVerified("DimDifferentialTests")]
        public new bool Equals(object o1, object o2) => Equals((int[])o1, (int[])o2);

        [MirrorVerified("DimDifferentialTests")]
        public int GetHashCode(object o) => GetHashCode((int[])o);

        [MirrorVerified("DimDifferentialTests")]
        public static bool Equals(int[] u, int[] v)
        {
            for (int i = 0; i != s_rank; i++)
                if (u[i] != v[i]) return false;
            return true;
        }

        /// <summary>Whole unit powers (truncated, modulo 8) in 4-bit fields.</summary>
        [MirrorVerified("DimDifferentialTests")]
        public static int GetHashCode(int[] v)
        {
            int h = 0;
            for (int i = 0; i != s_rank; i++) h ^= v[i] / 60000 % 8 << i % 8 * 4;
            return h;
        }
    }
}
