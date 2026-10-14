using static Quire.Differential.Tests.Reflect;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.Utilities</c> with Quire's for the members implemented so far; cases are listed in
/// docs/mirror/evidence/Mcd.Utilities.md.
/// </summary>
public class UtilitiesDifferentialTests
{
    private static System.Type M => Mathcad.Type("mplrun", "Mcd.Utilities");

    private static readonly double[] Doubles =
    [
        0, -0.0, 1, -1, 1e308, -1e308, double.MaxValue, double.Epsilon, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
        BitConverter.Int64BitsToDouble(0x7FF0000000000001), BitConverter.Int64BitsToDouble(unchecked((long)0xFFF8000000000000)),
    ];

    [SkippableFact]
    public void Finite_checks_scaling_and_units()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        var mNum = Mathcad.Type("mplrun", "Mcd.num_val");
        foreach (double d in Doubles)
        {
            Same(diffs, $"IsFinite({d:R})", () => Q.Utilities.IsFinite(d), () => Call(M, "IsFinite", d));
            Same(diffs, $"AssertFinite({d:R})", () => { Q.Utilities.AssertFinite(d); return "ok"; }, () => { CallSig(M, "AssertFinite", ["Double"], d); return "ok"; });
            foreach (double e in new[] { 0.0, double.NaN, double.PositiveInfinity })
                Same(diffs, $"AssertFinite(num_val {d:R}, {e:R})", () => { Q.Utilities.AssertFinite(new Q.num_val(d, e)); return "ok"; },
                    () => { CallSig(M, "AssertFinite", ["num_val"], Activator.CreateInstance(mNum, d, e)); return "ok"; });
        }
        double[][] vectors = [[1, 2, 3], [], [double.NaN, 0], [-4]];
        foreach (var v in vectors)
            foreach (double s in new[] { 2, 0, -0.5, double.NaN })
                Same(diffs, $"ScaleVector([{string.Join(",", v)}], {s:R})", () => { var a = (double[])v.Clone(); Q.Utilities.ScaleVector(a, s); return a; },
                    () => { var a = (double[])v.Clone(); Call(M, "ScaleVector", a, s); return a; });
        Same(diffs, "ScaleVector(null)", () => { Q.Utilities.ScaleVector(null!, 2); return "ok"; }, () => { Call(M, "ScaleVector", null, 2.0); return "ok"; });
        var mType = Mathcad.Type("mplrun", "Mcd.Type");
        foreach (var s in new[] { "V{Length 60000}", "V{}", "V{a Length 60000}", "V{Length 30000 Time-60000}", "V{Foo 60000}", "V{Money 60000 Mass-120000}" })
            Same(diffs, $"GetUnits({s})", () => Q.Utilities.GetUnits(Q.Type.Parse(s).GetDimensionTypeProduct()),
                () => Call(M, "GetUnits", Call(Call(mType, "Parse", s)!, "GetDimensionTypeProduct")), UnitSystemDifferentialTests.UnitsText);
        Same(diffs, "GetUnits(null)", () => Q.Utilities.GetUnits(null!), () => Call(M, "GetUnits", [null]), UnitSystemDifferentialTests.UnitsText);
        Same(diffs, "new Utilities()", () => new Q.Utilities() != null, () => Activator.CreateInstance(M) != null);
        NoDiffs(diffs);
    }
}
