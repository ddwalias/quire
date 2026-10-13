using System.Text;

namespace Quire.Values;

/// <summary>Physical dimension: exponents of the seven SI base quantities.</summary>
public readonly struct Dim : IEquatable<Dim>
{
    public const int Count = 7;
    public static readonly string[] BaseNames = ["L", "M", "T", "I", "Θ", "N", "J"];

    private readonly double _l, _m, _t, _i, _k, _n, _j;

    public Dim(double length = 0, double mass = 0, double time = 0, double current = 0,
        double temperature = 0, double amount = 0, double luminosity = 0)
    {
        _l = Clean(length); _m = Clean(mass); _t = Clean(time); _i = Clean(current);
        _k = Clean(temperature); _n = Clean(amount); _j = Clean(luminosity);
    }

    public static readonly Dim None = default;
    public static readonly Dim Length = new(length: 1);
    public static readonly Dim Mass = new(mass: 1);
    public static readonly Dim Time = new(time: 1);
    public static readonly Dim Current = new(current: 1);
    public static readonly Dim Temperature = new(temperature: 1);
    public static readonly Dim Amount = new(amount: 1);
    public static readonly Dim Luminosity = new(luminosity: 1);

    public double this[int index] => index switch
    {
        0 => _l, 1 => _m, 2 => _t, 3 => _i, 4 => _k, 5 => _n, 6 => _j,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public bool IsNone => Equals(None);

    private static Dim From(Func<int, double> f) => new(f(0), f(1), f(2), f(3), f(4), f(5), f(6));

    public static Dim operator *(Dim a, Dim b) => From(i => a[i] + b[i]);
    public static Dim operator /(Dim a, Dim b) => From(i => a[i] - b[i]);
    public Dim Pow(double p)
    {
        var self = this;
        return From(i => self[i] * p);
    }
    public static bool operator ==(Dim a, Dim b) => a.Equals(b);
    public static bool operator !=(Dim a, Dim b) => !a.Equals(b);

    private static double Clean(double x)
    {
        var r = Math.Round(x);
        return Math.Abs(x - r) < 1e-9 ? r : x;
    }

    public bool Equals(Dim other)
    {
        for (int i = 0; i < Count; i++)
            if (Math.Abs(this[i] - other[i]) > 1e-9) return false;
        return true;
    }

    public override bool Equals(object? obj) => obj is Dim d && Equals(d);

    public override int GetHashCode() => HashCode.Combine(_l, _m, _t, _i, _k, _n, _j);

    public override string ToString()
    {
        if (IsNone) return "1";
        var sb = new StringBuilder();
        for (int i = 0; i < Count; i++)
        {
            if (this[i] == 0) continue;
            if (sb.Length > 0) sb.Append('·');
            sb.Append(BaseNames[i]);
            if (this[i] != 1) sb.Append('^').Append(this[i]);
        }
        return sb.ToString();
    }
}
