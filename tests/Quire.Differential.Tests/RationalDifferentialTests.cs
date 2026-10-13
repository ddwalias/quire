using System.Reflection;
using Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Calls mplrun's <c>Mcd.Rational</c> and Quire's <see cref="Rational"/> with the same inputs and compares fields,
/// results and exception types. Covers every member; the cases are listed in docs/mirror/evidence/Mcd.Rational.md.
/// </summary>
public class RationalDifferentialTests
{
    private static readonly int[] Ints =
        [0, 1, -1, 2, -2, 3, 4, 6, -6, 7, 12, -18, 65536, 46341, int.MaxValue, -int.MaxValue, int.MinValue, int.MaxValue / 2];

    /// <summary>The operands for pairwise operator tests: a smaller set, since every pair meets every other.</summary>
    private static readonly int[] PairInts = [0, 1, -1, 2, -6, 7, 65536, int.MaxValue, int.MinValue];

    private static readonly double[] Doubles =
    [
        0, 0.5, -0.5, 1.0 / 3, -2.25, Math.PI, Math.E, Math.Sqrt(2), 0.1, 1e-3, 123.456, -987.654321, 3, 1e10, -1e10,
        int.MaxValue, int.MaxValue + 0.5, 4500675881.0, 1.0 / (1 << 30), 1.0 / ((1 << 30) - 1), 5 + 1.0 / (1 << 30),
        double.NaN, double.PositiveInfinity, double.NegativeInfinity, double.Epsilon, 1e-300,
    ];

    private static readonly double[] Errors = [1e-5, 1e-2, 0.5, 1e-9, 1e-12, 0, -1, double.NaN];

    private static System.Type M => Mathcad.Type("mplrun", "Mcd.Rational");

    /// <summary>The outcome of a call on either side, as text: the value, or the exception type.</summary>
    private static string Outcome(Func<object?> f)
    {
        try
        {
            return f() switch
            {
                null => "null",
                double d => d.ToString("R"),
                object o when o.GetType().Name == "Rational" => Fields(o),
                object o => o.ToString()!,
            };
        }
        catch (TargetInvocationException e) { return "throws " + e.InnerException!.GetType().Name; }
        catch (Exception e) { return "throws " + e.GetType().Name; }
    }

    private static string Fields(object r) =>
        $"{r.GetType().GetField("Numerator")!.GetValue(r)}/{r.GetType().GetField("Denominator")!.GetValue(r)}";

    private static object New(params object[] args) => Activator.CreateInstance(M, args)!;

    private static object? Op(string name, params object[] args) =>
        M.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == name && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(args.Select(a => a.GetType())))
            .Invoke(null, args);

    private static object? Op(string name, System.Type ret, object arg) =>
        M.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == name && m.ReturnType == ret && m.GetParameters()[0].ParameterType == arg.GetType())
            .Invoke(null, [arg]);

    /// <summary>Rationals built on both sides from the same (n, d) pairs; pairs that throw are left out.</summary>
    private static List<(Rational Q, object M)> Pairs()
    {
        var list = new List<(Rational, object)> { (default, Activator.CreateInstance(M)!) };
        foreach (int n in PairInts)
            foreach (int d in PairInts)
            {
                Rational q;
                try { q = new Rational(n, d); } catch { continue; }
                list.Add((q, New(n, d)));
            }
        return list;
    }

    private static void Same(List<string> diffs, string what, Func<object?> quire, Func<object?> mathcad)
    {
        string a = Outcome(quire), b = Outcome(mathcad);
        if (a != b) diffs.Add($"{what}: Quire {a}, Mathcad {b}");
    }

    private static void NoDiffs(List<string> diffs) =>
        Assert.True(diffs.Count == 0, $"{diffs.Count} differences:\n" + string.Join("\n", diffs.Take(30)));

    [SkippableFact]
    public void Integer_constructors_and_GCD()
    {
        var diffs = new List<string>();
        foreach (int n in Ints)
        {
            Same(diffs, $"new({n})", () => new Rational(n), () => New(n));
            Same(diffs, $"(Rational){n}", () => (Rational)n, () => Op("op_Implicit", M, n));
            foreach (int d in Ints)
            {
                Same(diffs, $"new({n}, {d})", () => new Rational(n, d), () => New(n, d));
                Same(diffs, $"GCD({n}, {d})", () => Rational.GCD(n, d), () => Op("GCD", n, d));
            }
        }
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Double_constructors()
    {
        var diffs = new List<string>();
        var rng = new Random(20261014);
        var xs = Doubles.Concat(Enumerable.Range(0, 3000).Select(_ => (rng.NextDouble() - 0.5) * Math.Pow(10, rng.Next(-6, 8)))).ToList();
        foreach (double x in xs)
        {
            Same(diffs, $"new({x:R})", () => new Rational(x), () => New(x));
            Same(diffs, $"(Rational){x:R}", () => (Rational)x, () => Op("op_Implicit", M, x));
            foreach (double e in Errors)
                Same(diffs, $"new({x:R}, {e:R})", () => new Rational(x, e), () => New(x, e));
        }
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Operators_on_pairs()
    {
        var diffs = new List<string>();
        var pairs = Pairs();
        foreach (var (qa, ma) in pairs)
        {
            Same(diffs, $"(double){qa}", () => (double)qa, () => Op("op_Implicit", typeof(double), ma));
            Same(diffs, $"(int){qa}", () => (int)qa, () => Op("op_Explicit", typeof(int), ma));
            Same(diffs, $"{qa}.GetHashCode", () => qa.GetHashCode(), () => ma.GetHashCode());
            Same(diffs, $"{qa}.ToString", () => qa.ToString(), () => ma.ToString());
            Same(diffs, $"{qa}.IsIntegral", () => qa.IsIntegral, () => M.GetProperty("IsIntegral")!.GetValue(ma));
            Same(diffs, $"{qa}.Equals(int)", () => qa.Equals(1), () => ma.Equals(1));
            foreach (int k in Ints)
            {
                Same(diffs, $"{qa} * {k}", () => qa * k, () => Op("op_Multiply", ma, k));
                Same(diffs, $"{qa} / {k}", () => qa / k, () => Op("op_Division", ma, k));
            }
            foreach (var (qb, mb) in pairs)
            {
                Same(diffs, $"{qa}.Equals({qb})", () => qa.Equals(qb), () => ma.Equals(mb));
                Same(diffs, $"{qa} == {qb}", () => qa == qb, () => Op("op_Equality", ma, mb));
                Same(diffs, $"{qa} != {qb}", () => qa != qb, () => Op("op_Inequality", ma, mb));
                Same(diffs, $"{qa} < {qb}", () => qa < qb, () => Op("op_LessThan", ma, mb));
                Same(diffs, $"{qa} > {qb}", () => qa > qb, () => Op("op_GreaterThan", ma, mb));
                Same(diffs, $"{qa} <= {qb}", () => qa <= qb, () => Op("op_LessThanOrEqual", ma, mb));
                Same(diffs, $"{qa} >= {qb}", () => qa >= qb, () => Op("op_GreaterThanOrEqual", ma, mb));
                Same(diffs, $"{qa} * {qb}", () => qa * qb, () => Op("op_Multiply", ma, mb));
                Same(diffs, $"{qa} / {qb}", () => qa / qb, () => Op("op_Division", ma, mb));
            }
        }
        NoDiffs(diffs);
    }
}
