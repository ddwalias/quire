namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mpl.Run.Primitive</c> comparisons with Quire's using direct
/// <see cref="MethodComparison.Compare"/> calls; cases are listed in docs/mirror/evidence/Mpl.Run.Primitive.md.
/// </summary>
public class PrimitiveDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly string[] Comparisons = ["ceq", "cne", "clt", "cle", "cgt", "cge"];

    private static void CompareAll<T>(string suffix, T[] values)
    {
        var quire = typeof(Mpl.Run.Primitive);
        var mathcad = Mathcad.Type("mplrun", "Mpl.Run.Primitive");
        foreach (var name in Comparisons)
        {
            var q = quire.GetMethod(name + suffix, [typeof(T), typeof(T)])
                ?? throw new HarnessException($"Quire method not found: Primitive.{name}{suffix}");
            var m = mathcad.GetMethod(name + suffix, [typeof(T), typeof(T)])
                ?? throw new HarnessException($"Mathcad method not found: Primitive.{name}{suffix}");
            foreach (var x in values)
                foreach (var y in values)
                    MethodComparison.Compare($"{name}{suffix}({x}, {y})", Contract, q, null, [x, y], m, null, [x, y]);
        }
    }

    [SkippableFact]
    public void Int_comparisons() => CompareAll("_i4", [int.MinValue, -1, 0, 1, int.MaxValue]);

    [SkippableFact]
    public void Double_comparisons() => CompareAll("_f8",
        [double.NegativeInfinity, -1.0, -0.0, 0.0, 1.0, double.PositiveInfinity, double.NaN]);
}
