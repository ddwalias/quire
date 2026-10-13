#nullable disable
using System.Globalization;

namespace Quire.Mcd;

/// <summary>The real and imaginary parts of a number, in SI base units.</summary>
public partial struct num_val
{
    public double rp;
    public double ip;

    public num_val(double r) { rp = r; ip = 0; }

    public num_val(double r, double i) { rp = r; ip = i; }
}

/// <summary>A complex number with a physical dimension; the parts are stored in SI base units.</summary>
public sealed partial class NumVal : DimVal
{
    public const double default_fuzz = 5E-13d;

    public num_val Data;

    public static readonly NumVal Zero = new(0.0);
    public static readonly NumVal One = new(1.0);

    public NumVal() : this(0.0) { }

    public NumVal(int i) : this((double)i) { }

    public NumVal(double r) : this(r, 0, Dim.identity) { }

    public NumVal(double r, double i) : this(r, i, Dim.identity) { }

    public NumVal(double r, double i, Dim d)
    {
        Data = new num_val(r, i);
        m_dim = d;
    }

    public NumVal(NumVal n) : this(n.Data.rp, n.Data.ip, n.m_dim) { }

    public NumVal(NumVal n, Dim d) : this(n.Data.rp, n.Data.ip, d) { }

    public double Real
    {
        get => Data.rp;
        set => Data.rp = value;
    }

    public double Imag
    {
        get => Data.ip;
        set => Data.ip = value;
    }

    public override Dim PhysicalDimension => m_dim;

    public override int IsReal => Data.ip == 0 ? 1 : 0;

    /// <summary>1 for true, 0 for false.</summary>
    public static NumVal McdBool(object o) => o is true ? One : Zero;

    public override DimVal SetDim(Dim d) => new NumVal(this, d);

    public static bool ExactEquals(NumVal x, NumVal y) =>
        x.Data.rp.Equals(y.Data.rp) && x.Data.ip.Equals(y.Data.ip) && Dim.Eq_b(x.m_dim, y.m_dim);

    public static bool FuzzyEquals(double x, double y) => FuzzyEquals(x, y, default_fuzz);

    /// <summary>True when <paramref name="x"/> and <paramref name="y"/> agree to a relative tolerance.</summary>
    public static bool FuzzyEquals(double x, double y, double fuzz) =>
        x == y || Math.Abs(x - y) <= fuzz * Math.Max(Math.Abs(x), Math.Abs(y));

    public static bool FuzzyEquals(NumVal x, NumVal y) => FuzzyEquals(x, y, default_fuzz);

    public static bool FuzzyEquals(NumVal x, NumVal y, double fuzz) =>
        Dim.Eq_b(x.m_dim, y.m_dim) && FuzzyEquals(x.Data.rp, y.Data.rp, fuzz) && FuzzyEquals(x.Data.ip, y.Data.ip, fuzz);

    public override bool Equals(object obj) => obj is NumVal n && ExactEquals(this, n);

    public override int GetHashCode() => HashCode.Combine(Data.rp, Data.ip, m_dim);

    public string Format(bool show_dimension)
    {
        string re = Data.rp.ToString("G10", CultureInfo.InvariantCulture);
        string text = Data.ip == 0 ? re
            : $"{re}{(Data.ip < 0 ? "-" : "+")}{Math.Abs(Data.ip).ToString("G10", CultureInfo.InvariantCulture)}i";
        return show_dimension && Dim.IsDimensioned(m_dim) ? $"{text} [{m_dim}]" : text;
    }

    public override string ToString() => Format(true);
}

/// <summary>A string value.</summary>
public partial class StrVal : DimVal
{
    public string Field1;

    public StrVal(string str)
    {
        Field1 = str ?? "";
        m_dim = Dim.string_dim;
    }

    public string m_string => Field1;

    public override string IsString => Field1;

    public override string ToString() => $"\"{Field1}\"";

    public override bool Equals(object obj) => obj is StrVal s && s.Field1 == Field1;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Field1);
}

/// <summary>A range variable such as <c>x := 0, 0.5 .. 10</c>: <see cref="m_n"/> values from
/// <see cref="m_init"/> in steps of <see cref="m_incr"/>.</summary>
public partial class RngVal : DimVal
{
    protected double m_init;
    protected double m_incr;
    protected int m_n;
    protected bool m_integral;

    protected RngVal(double init, double incr, int n, Dim d)
    {
        m_init = init;
        m_incr = incr;
        m_n = Math.Max(n, 0);
        m_integral = init == Math.Floor(init) && incr == Math.Floor(incr);
        m_dim = d;
    }

    public bool Integral => m_integral;

    public override Dim PhysicalDimension => m_dim;

    public override int Rsize => m_n;

    public static RngVal IncrRange(double first, double incr, int n, Dim d) => new(first, incr, n, d);

    public virtual double get(int i) => m_init + i * m_incr;

    public override DimVal range_get(int n) =>
        n >= 0 && n < m_n ? new NumVal(get(n), 0, m_dim) : throw new IndexOutOfRangeException($"Range index {n} is out of range");

    public override MatVal asVector(string m, uint a)
    {
        var v = new MatVal(m_n, 1);
        v.m_dim = m_dim;
        for (int i = 0; i < m_n; i++) v.set_1(i, get(i));
        return v;
    }

    public override bool Equals(object obj) =>
        obj is RngVal r && r.m_init.Equals(m_init) && r.m_incr.Equals(m_incr) && r.m_n == m_n && Dim.Eq_b(r.m_dim, m_dim);

    public override int GetHashCode() => HashCode.Combine(m_init, m_incr, m_n, m_dim);

    public override string ToString() => $"range[{m_n}]";
}
