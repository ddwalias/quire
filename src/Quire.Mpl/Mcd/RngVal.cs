#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>
/// A range variable: <see cref="m_n"/> values from <see cref="m_init"/> in steps of <see cref="m_incr"/>, all of
/// dimension <c>m_dim</c>. Indices are not checked. Behaviour follows mplrun 12.0.1.0
/// (docs/mirror/evidence/Mcd.RngVal.md).
/// </summary>
public partial class RngVal : DimVal
{
    [MirrorVerified("RngValDifferentialTests")]
    protected double m_init;

    [MirrorVerified("RngValDifferentialTests")]
    protected double m_incr;

    [MirrorVerified("RngValDifferentialTests")]
    protected int m_n;

    [MirrorVerified("RngValDifferentialTests")]
    protected bool m_integral;

    /// <summary>Integral when the start and step survive a (saturating) int conversion unchanged.</summary>
    [MirrorVerified("RngValDifferentialTests")]
    protected RngVal(double init, double incr, int n, Dim d)
    {
        m_init = init;
        m_incr = incr;
        m_n = n;
        m_dim = d;
        m_integral = (int)init == init && (int)incr == incr;
    }

    [MirrorVerified("RngValDifferentialTests")]
    protected RngVal(RngVal r)
    {
        m_init = r.m_init;
        m_incr = r.m_incr;
        m_n = r.m_n;
        m_dim = r.m_dim;
        m_integral = r.m_integral;
    }

    [MirrorVerified("RngValDifferentialTests")]
    public bool Integral => m_integral;

    [MirrorVerified("RngValDifferentialTests")]
    public override Dim PhysicalDimension
    {
        get
        {
            if (!IsPolymorphic) return m_dim;
            return Dim.identity;
        }
    }

    [MirrorVerified("RngValDifferentialTests")]
    public override int Rsize => m_n;

    [MirrorVerified("RngValDifferentialTests")]
    public override int Isize => m_n;

    /// <summary>A plain copy (an interpolating range loses its interpolation).</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public override DimVal asResult() => new RngVal(this);

    [MirrorVerified("RngValDifferentialTests")]
    public override bool Equals(object obj)
    {
        RngVal rngVal = obj as RngVal;
        if (rngVal == null)
        {
            return false;
        }
        if (Dim.Eq_b(m_dim, rngVal.m_dim))
        {
            if (m_init == rngVal.m_init && m_incr == rngVal.m_incr)
            {
                return m_n == rngVal.m_n;
            }
            return false;
        }
        return false;
    }

    /// <summary>Throws <see cref="NullReferenceException"/> for a polymorphic range.</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public override int GetHashCode() => m_init.GetHashCode() ^ m_incr.GetHashCode() ^ m_dim.GetHashCode() + m_n;

    [MirrorVerified("RngValDifferentialTests")]
    public virtual double get(int i) => m_init + i * m_incr;

    [MirrorVerified("RngValDifferentialTests")]
    public override DimVal range_get(int n) => new NumVal(m_init + n * m_incr, 0.0, m_dim);

    [MirrorVerified("RngValDifferentialTests")]
    public override DimVal iterator_get(int n) => new NumVal(m_init + n * m_incr, 0.0, m_dim);

    private const string TwoArgumentUnits = "This function must be defined with 3 arguments when used with units";
    private const string NotSequential = "The arguments must be sequential values";

    /// <summary>
    /// Unit steps from <paramref name="beg"/> towards <paramref name="end"/>: both must be real (arguments 1, 2) and
    /// unitless; a backwards or empty request is "The arguments must be sequential values".
    /// </summary>
    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal mkVec2(DimVal beg, DimVal end)
    {
        beg.asReal(1u);
        end.asReal(2u);
        double first = beg.asDimlessReal(TwoArgumentUnits, 1u);
        double last = end.asDimlessReal(TwoArgumentUnits, 2u);
        return IncrRange(first, last >= first ? 1 : -1, last, Dim.identity, NotSequential);
    }

    /// <summary>Steps of <c>next − beg</c>; the three dimensions must agree (argument 3 when the last differs).</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal mkVec3(DimVal beg, DimVal next, DimVal end)
    {
        double first = beg.asReal(1u), second = next.asReal(2u), last = end.asReal(3u);
        return IncrRange(first, second - first, last, Dim.Eq(end.m_dim, Dim.Eq(beg.m_dim, next.m_dim), 3u), NotSequential);
    }

    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal mkRange2(DimVal beg, DimVal end)
    {
        beg.asReal(1u);
        end.asReal(2u);
        return Range(beg.asDimlessReal("bad_range_dimensionless_argument", 1u), end.asDimlessReal("bad_range_dimensionless_argument", 2u));
    }

    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal mkRange3(DimVal beg, DimVal next, DimVal end) =>
        Range(beg.asReal(1u), next.asReal(2u), end.asReal(3u), Dim.Eq(end.m_dim, Dim.Eq(beg.m_dim, next.m_dim), 3u));

    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal Range(double beg, double end) => IncrRange(beg, end >= beg ? 1 : -1, end, Dim.identity);

    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal Range(double first, double second, double last, Dim d) => IncrRange(first, second - first, last, d);

    /// <summary>
    /// The range from <paramref name="first"/> stepping <paramref name="incr"/> up to <paramref name="last"/>. The
    /// step count is <c>(last − first)/incr</c>, stretched by 1 + 1e-11 and truncated to int (saturating). A zero step,
    /// a NaN, or a negative count is error <paramref name="m"/>; a saturated count is "range too many elements".
    /// An integral step, or a last value missed by more than 5e-12 of the span (always, for descending spans), gives
    /// a plain range of count + 1 values; otherwise the values are interpolated to end exactly on
    /// <paramref name="last"/>.
    /// </summary>
    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal IncrRange(double first, double incr, double last, Dim d, string m)
    {
        int steps = (int)(1.00000000001 * (last - first) / incr);
        if (incr == 0.0 || double.IsNaN(first) || double.IsNaN(last) || double.IsNaN(incr) || steps < 0 && steps != int.MinValue)
            AppException.Raise(m);
        else if (steps == int.MaxValue || steps == int.MinValue)
            AppException.Raise("range too many elements");
        if (Math.Floor(incr) == incr || Math.Abs(first + steps * incr - last) > (last - first) * 5E-12)
            return new RngVal(first, incr, steps + 1, d);
        return new RngValInterpolate(first, last, steps, d);
    }

    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal IncrRange(double first, double incr, double last, Dim d) => IncrRange(first, incr, last, d, "illegal range");

    /// <summary><paramref name="n"/> values, unchecked.</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public static RngVal IncrRange(double first, double incr, int n, Dim d) => new(first, incr, n, d);

    [MirrorVerified("RngValConversionsDifferentialTests")]
    public override MatVal asVector(string m, uint a) => asRealVector(m, a);

    [MirrorVerified("RngValConversionsDifferentialTests")]
    public override MatVal asMatrix(string m, uint a) => asRealVector(m, a);

    [MirrorVerified("RngValConversionsDifferentialTests")]
    public override MatVal asRealMatrix(string m, uint a) => asRealVector(m, a);

    /// <summary>A column of the values (by <see cref="get"/>) with the range's dimension.</summary>
    [MirrorVerified("RngValConversionsDifferentialTests")]
    public override MatVal asRealVector(string m, uint a)
    {
        int n = m_n;
        var v = new MatVal(n);
        for (int i = 0; i < n; i++) v.set_1(i, get(i));
        v.m_dim = m_dim;
        return v;
    }

    [MirrorVerified("RngValDifferentialTests")]
    public override RngVal asRange(string m, uint a) => this;
}

/// <summary>A range of <c>n + 1</c> values that interpolates between its ends, so the last is exact.</summary>
public partial class RngValInterpolate : RngVal
{
    [MirrorVerified("RngValDifferentialTests")]
    protected double m_last;

    [MirrorVerified("RngValDifferentialTests")]
    protected int m_ninterval;

    [MirrorVerified("RngValDifferentialTests")]
    public RngValInterpolate(double init, double last, int n, Dim d) : base(init, (last - init) / n, n + 1, d)
    {
        m_last = last;
        m_ninterval = n;
    }

    /// <summary>The weighted mean <c>(init·(n − i) + last·i) / n</c>.</summary>
    [MirrorVerified("RngValDifferentialTests")]
    public override double get(int i) => (m_init * (m_ninterval - i) + m_last * i) / m_ninterval;

    [MirrorVerified("RngValDifferentialTests")]
    public override DimVal range_get(int n) => new NumVal(get(n), 0.0, m_dim);

    [MirrorVerified("RngValDifferentialTests")]
    public override DimVal iterator_get(int n) => new NumVal(get(n), 0.0, m_dim);
}
