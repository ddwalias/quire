using System.Globalization;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// One side of a comparison: Quire's or Mathcad's <c>Mcd</c> types, with helpers that build the same values on
/// either side and render any value, dimension or result as side-neutral text, so a scenario written once against
/// a <see cref="Side"/> can run on both and its logs be compared line by line.
/// </summary>
public sealed class Side
{
    private readonly Func<string, System.Type> _type;

    private Side(string name, Func<string, System.Type> type)
    {
        Name = name;
        _type = type;
    }

    public string Name { get; }

    public static Side Quire { get; } = new("Quire", n => typeof(global::Quire.Mcd.Dim).Assembly.GetType("Quire." + n) ?? throw new HarnessException($"Quire type not found: Quire.{n}"));

    public static Side Mathcad { get; } = new("Mathcad", n => global::Quire.Differential.Tests.Mathcad.Type("mplrun", n));

    /// <summary>A type by its Mathcad name (<c>Mcd.NumVal</c>, <c>Mcd.Iterator+SeqIterator</c>).</summary>
    public System.Type T(string name) => _type(name);

    public System.Type Dim => T("Mcd.Dim");

    public object? Marker(string name) => Get(Dim, name);

    public object? Parse(string signature) => Call(Dim, "Parse", signature);

    /// <summary>Named dimensions: polymorphic null, the markers and a few parsed ones.</summary>
    public object? D(string name) => name switch
    {
        "poly" => null,
        "identity" or "mcd_zero" or "string_dim" or "function_dim" or "mixed_matrix_dim" => Marker(name),
        "L" => Parse("V{Length 60000}"),
        "T" => Parse("V{Time 60000}"),
        "v" => Parse("V{Length 60000 Time-60000}"),
        "N" => Parse("V{Length 60000 Mass 60000 Time-120000}"),
        "L2" => Parse("V{Length 120000}"),
        _ => throw new ArgumentException(name),
    };

    public static readonly string[] DimNames = ["poly", "identity", "mcd_zero", "string_dim", "L", "T", "v", "N"];

    public object Num(double re, double im, string dim) => New(T("Mcd.NumVal"), re, im, D(dim));

    public object Str(string? s) => New(T("Mcd.StrVal"), s);

    public object NotAVal() => New(T("Mcd.NotAVal"));

    /// <summary>Runs <paramref name="f"/> with the thread's strict-comparison option set, then restores it.</summary>
    public T2 Strict<T2>(bool strict, Func<T2> f)
    {
        var options = T("Mcd.RuntimeOptions");
        object? old = Get(options, "strict_comparisons");
        Set(options, "strict_comparisons", strict);
        try { return f(); }
        finally { Set(options, "strict_comparisons", old); }
    }

    public string DimText(object? d)
    {
        if (d == null) return "poly";
        foreach (var m in new[] { "identity", "mcd_zero", "string_dim", "function_dim", "mixed_matrix_dim" })
            if (ReferenceEquals(d, Marker(m))) return m;
        return "[" + string.Join(",", (int[])Get(d, "Coordinates")!) + "]";
    }

    /// <summary>Any outcome as text: values with their fields and dimension, dimensions, units, scalars.</summary>
    public string Show(object? o)
    {
        if (o == null) return "null";
        var t = o.GetType();
        string kind = t.FullName!.StartsWith("Quire.") ? t.FullName[6..] : t.FullName;
        switch (kind)
        {
            case "Mcd.Dim": return "Dim " + DimText(o);
            case "Mcd.NumVal":
                var data = Get(o, "Data")!;
                return $"NumVal({Bits((double)Get(data, "rp")!)}, {Bits((double)Get(data, "ip")!)}) {DimText(Get(o, "m_dim"))}";
            case "Mcd.StrVal": return $"StrVal({Text(Get(o, "Field1"))}) {DimText(Get(o, "m_dim"))}";
            case "Mcd.NotAVal":
                return $"NotAVal{(ReferenceEquals(o, Get(T("Mcd.NotAVal"), "singleton")) ? " singleton" : "")} {DimText(Get(o, "m_dim"))}";
            case "Mcd.RngVal":
            case "Mcd.RngValInterpolate":
                string s = $"{kind[4..]}(init {Bits((double)Get(o, "m_init")!)}, incr {Bits((double)Get(o, "m_incr")!)}, n {Get(o, "m_n")}, " +
                           $"integral {Get(o, "m_integral")}) {DimText(Get(o, "m_dim"))}";
                if (kind == "Mcd.RngValInterpolate") s += $" last {Bits((double)Get(o, "m_last")!)} intervals {Get(o, "m_ninterval")}";
                return s;
            case "Mcd.Units": return "Units " + UnitsText(o);
            case "Mcd.num_val": return $"num_val({Bits((double)Get(o, "rp")!)}, {Bits((double)Get(o, "ip")!)})";
            case "Mpl.Run.True": return "True";
        }
        if (o is double d) return Bits(d);
        if (o is System.Collections.IEnumerable e && o is not string) return "[" + string.Join(", ", e.Cast<object?>().Select(Show)) + "]";
        if (kind.StartsWith("Mcd.") || kind.StartsWith("Mpl.")) return kind;
        return Text(o);
    }

    /// <summary>A double with its NaN payload when it has one.</summary>
    public static string Bits(double d) =>
        double.IsNaN(d) ? $"NaN:{BitConverter.DoubleToInt64Bits(d):X16}" : d.ToString("R", CultureInfo.InvariantCulture);

    public string Out(Func<object?> f) => Outcome(f, Show);

    /// <summary><c>isInt(int*)</c>: whether it reports an integer, and the int it wrote.</summary>
    public string IsInt(object value)
    {
        var method = value.GetType().GetMethod("isInt", BindingFlags.Public | BindingFlags.Instance)!;
        return Outcome(() => InvokeIsInt(method, value));
    }

    private static unsafe string InvokeIsInt(MethodInfo method, object value)
    {
        int result = -12345;
        try
        {
            object? r = method.Invoke(value, [Pointer.Box(&result, typeof(int*))]);
            return $"{r} {result}";
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw new InvalidOperationException($"{Thrown(e.InnerException)}; wrote {result}");
        }
    }
}

/// <summary>Runs one scenario on each side and compares the logs line by line.</summary>
public static class Scenario
{
    public static void Compare(Action<Side, List<string>> scenario)
    {
        Runtime.Sync();
        var q = new List<string>();
        var m = new List<string>();
        scenario(Side.Quire, q);
        scenario(Side.Mathcad, m);
        Assert.True(q.Count > 0 && m.Count > 0, "Differential scenario must record outcomes on both sides");
        var diffs = new List<string>();
        Same(diffs, "line count", q.Count.ToString(), m.Count.ToString());
        for (int i = 0; i < Math.Min(q.Count, m.Count); i++)
            if (q[i] != m[i]) diffs.Add($"line {i}:\n  Quire   {q[i]}\n  Mathcad {m[i]}");
        NoDiffs(diffs);
    }

    /// <summary>Logs "<paramref name="what"/> = outcome".</summary>
    public static void Log(this List<string> log, Side s, string what, Func<object?> f) => log.Add($"{what} = {s.Out(f)}");
}
