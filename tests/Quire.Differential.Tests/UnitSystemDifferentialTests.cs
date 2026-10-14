using static Quire.Differential.Tests.Reflect;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Builds the same units and unit systems with mplrun's <c>Mcd.Unit</c>, <c>Mcd.Units</c> and <c>Mcd.UnitSystem</c>
/// and Quire's, and compares every public member over many dimensions; cases are listed in
/// docs/mirror/evidence/Mcd.Units.md and Mcd.UnitSystem.md.
/// </summary>
public class UnitSystemDifferentialTests
{
    private static System.Type MUnit => Mathcad.Type("mplrun", "Mcd.Unit");
    private static System.Type MUnits => Mathcad.Type("mplrun", "Mcd.Units");
    private static System.Type MSystem => Mathcad.Type("mplrun", "Mcd.UnitSystem");
    private static System.Type MType => Mathcad.Type("mplrun", "Mcd.Type");

    private record U(string Name, string Symbol, string Quantity, double Magnitude, string Signature);

    private static readonly U[] Base =
    [
        new("meter", "m", "Length", 1, "V{Length 60000}"), new("kilogram", "kg", "Mass", 1, "V{Mass 60000}"),
        new("second", "s", "Time", 1, "V{Time 60000}"), new("ampere", "A", "Current", 1, "V{Current 60000}"),
        new("kelvin", "K", "Temperature", 1, "V{Temperature 60000}"), new("mole", "mol", "Substance", 1, "V{Substance 60000}"),
        new("candela", "cd", "Luminosity", 1, "V{Luminosity 60000}"), new("base_currency", "¤", "Money", 1, "V{Money 60000}"),
    ];

    private static readonly U[] UsBase =
    [
        new("foot", "ft", "Length", 0.3048, "V{Length 60000}"), new("pound", "lb", "Mass", 0.45359237, "V{Mass 60000}"),
        new("second", "s", "Time", 1, "V{Time 60000}"), new("ampere", "A", "Current", 1, "V{Current 60000}"),
        new("rankine", "R", "Temperature", 5.0 / 9, "V{Temperature 60000}"), new("mole", "mol", "Substance", 1, "V{Substance 60000}"),
        new("candela", "cd", "Luminosity", 1, "V{Luminosity 60000}"), new("dollar", "$", "Money", 1, "V{Money 60000}"),
    ];

    private static readonly U[] Derived =
    [
        new("newton", "N", "Force", 1, "V{Length 60000 Mass 60000 Time-120000}"),
        new("kilonewton", "kN", "Force", 1000, "V{Length 60000 Mass 60000 Time-120000}"),
        new("joule", "J", "Energy", 1, "V{Length 120000 Mass 60000 Time-120000}"),
        new("liter", "L", "Volume", 0.001, "V{Length 180000}"), new("hectare", "ha", "Area", 1e4, "V{Length 120000}"),
        new("hertz", "Hz", "Frequency", 1, "V{Time-60000}"), new("minute", "min", "Time", 60, "V{Time 60000}"),
        new("pascal", "Pa", "Pressure", 1, "V{Length-60000 Mass 60000 Time-120000}"),
        new("volt", "V", "Potential", 1, "V{Current-60000 Length 120000 Mass 60000 Time-180000}"),
        new("rootmeter", "rm", "Root length", 1, "V{Length 30000}"), new("odd", "od", "Odd", 2, "V{Length 7 Time-3}"),
        new("nothing", "NOSIMPLIFY", "Velocity", 1, "V{Length 60000 Time-60000}"),
    ];

    /// <summary>Unit systems by name: basis and defaults.</summary>
    private static readonly (string Name, U[] Basis, U[] Defaults)[] Systems =
    [
        ("si", Base, Derived),
        ("us", UsBase, Derived[..6]),
        ("seven", Base[..7], Derived[..3]),
        ("six-no-current", [.. Base[..3], .. Base[4..7]], Derived[..1]),
        ("nine", [.. Base, new U("bit", "b", "Information", 1, "V{Information 60000}")], Derived[..2]),
        ("singular", [.. Base[..7], new U("foot", "ft", "Length", 0.3048, "V{Length 60000}")], Derived[..1]),
        ("composite-basis", [.. Base[..2], new U("hertz", "Hz", "Frequency", 1, "V{Time-60000}"), .. Base[3..]], Derived[..4]),
        ("three-forces", Base, [Derived[0], Derived[1], new U("dyne", "dyn", "Force", 1e-5, "V{Length 60000 Mass 60000 Time-120000}")]),
        ("unknown-default", Base, [new U("foo", "f", "Foo", 1, "V{Foo 60000}")]),
        ("zero-magnitude", Base, [new U("zero", "z", "Force", 0, "V{Length 60000 Mass 60000 Time-120000}")]),
        ("empty", [], []),
    ];

    private static object MkUnit(bool mathcad, U u) => mathcad
        ? New(MUnit, u.Name, u.Symbol, u.Quantity, u.Magnitude, u.Signature)
        : new Q.Unit(u.Name, u.Symbol, u.Quantity, u.Magnitude, u.Signature);

    private static Array Units(bool mathcad, U[] us)
    {
        var array = Array.CreateInstance(mathcad ? MUnit : typeof(Q.Unit), us.Length);
        for (int i = 0; i < us.Length; i++) array.SetValue(MkUnit(mathcad, us[i]), i);
        return array;
    }

    private static object MkSystem(bool mathcad, string name, U[] basis, U[] defaults) => mathcad
        ? New(MSystem, name, 7u, "guid", Units(true, basis), Units(true, defaults))
        : new Q.UnitSystem(name, 7u, "guid", (Q.Unit[])Units(false, basis), (Q.Unit[])Units(false, defaults));

    internal static string UnitText(object? u) => u == null ? "null"
        : $"{Get(u, "Symbol")}({Get(u, "Name")}, {Get(u, "Quantity")}, {Text(Get(u, "Magnitude"))}, {Get(u, "Type")})";

    internal static string UnitsText(object? u) => u == null ? "null"
        : $"{u} m={Text(Get(u, "Magnitude"))}/{Text(Get(u, "magnitude"))} [" + string.Join(" ", ((Array)Get(u, "Powers")!).Cast<object>()
            .Select(p => $"{Get(Get(p, "Base")!, "Symbol")}^{Get(Get(p, "Exponent")!, "Numerator")}/{Get(Get(p, "Exponent")!, "Denominator")}")) + "]";

    private static string Dims(object? a) => a == null ? "null" : string.Join(" ", ((Array)a).Cast<object?>().Select(t => t?.ToString() ?? "null"));

    private static string Symbols(object? a) => a == null ? "null" : string.Join(" ", ((Array)a).Cast<object?>().Select(u => u == null ? "null" : (string)Get(u, "Symbol")!));

    /// <summary>Coordinate vectors: whole, half and third powers, combinations, odd values and wrong lengths.</summary>
    private static IEnumerable<int[]> Vectors(int rank)
    {
        int[] steps = [0, 60000, -60000, 120000, -120000, 180000, 30000, -20000, 7, -1];
        var rng = new Random(20261014);
        yield return new int[rank];
        for (int i = 0; i < rank; i++)
            foreach (int s in steps)
            {
                var v = new int[rank];
                v[i] = s;
                yield return v;
            }
        for (int n = 0; n < 300; n++)
        {
            var v = new int[rank];
            for (int i = 0; i < rank; i++) v[i] = rng.Next(3) == 0 ? steps[rng.Next(steps.Length)] : 0;
            yield return v;
        }
        yield return [60000, -120000, 60000]; // shorter than the rank
        yield return new int[rank + 2];
    }

    private static readonly string[] Signatures =
    [
        "V{Length 60000}", "V{}", "V{Length 60000 Mass 60000 Time-120000}", "V{Length 180000}", "V{Length 30000}", "V{Foo 60000}",
        "V{a Length 60000}", "V a", "{Time-60000}", "F{Money 60000}", "M(V{Length 120000})", "a->b", "V{Length 1}", "V{Length -7 Time 3}",
    ];

    private static string Describe(object system, bool mathcad)
    {
        var log = new List<string>();
        void Step(string what, Func<object?> f, Func<object?, string>? show = null) => log.Add(what + " = " + Outcome(f, show));
        Step("Name", () => Get(system, "Name"));
        Step("Code", () => Get(system, "Code"));
        Step("Guid", () => Get(system, "Guid"));
        Step("ToString", () => system.ToString());
        Step("RO_number", () => Get(system, "RO_number"));
        Step("Rank", () => Get(system, "Rank"));
        Step("Dimensions", () => Get(system, "Dimensions"), Dims);
        Step("Basis", () => Get(system, "Basis"), Symbols);
        Step("Defaults", () => Get(system, "Defaults"), Symbols);
        foreach (var sym in new[] { "m", "kg", "A", "N", "kN", "NOSIMPLIFY", "ft", "x", "" })
            Step($"IsUnitSymbol({sym})", () => Call(system, "IsUnitSymbol", sym));
        int rank = Get(system, "Rank") is int r ? r : 8;
        foreach (var v in Vectors(rank))
        {
            string vs = string.Join(",", v);
            Step($"Simplify([{vs}])", () => CallSig(system, "Simplify", ["Int32[]"], v), UnitsText);
            Step($"Normalize([{vs}])", () => CallSig(system, "Normalize", ["Int32[]"], v), UnitsText);
            Step($"ChangeBasis([{vs}])", () => CallSig(system, "ChangeBasis", ["Int32[]"], v));
            Step($"GetDimensions([{vs}])", () => CallSig(system, "GetDimensions", ["Int32[]"], v));
            var local = v.Select(x => x / 60000.0).ToArray();
            Step($"ChangeBasis(local [{vs}])", () => CallSig(system, "ChangeBasis", ["Double[]"], local));
            Step($"GetUnits(local [{vs}])", () => CallSig(system, "GetUnits", ["Double[]"], local), UnitsText);
        }
        foreach (var s in Signatures)
        {
            Step($"Simplify({s})", () => CallSig(system, "Simplify", ["String"], s), UnitsText);
            Step($"Normalize({s})", () => CallSig(system, "Normalize", ["String"], s), UnitsText);
            object? product = ProductOf(s, mathcad);
            Step($"Simplify(product {s})", () => CallSig(system, "Simplify", ["Product"], product), UnitsText);
            Step($"Normalize(product {s})", () => CallSig(system, "Normalize", ["Product"], product), UnitsText);
            Step($"GetCoordinates({s})", () => Call(system, "GetCoordinates", product));
        }
        return string.Join("\n", log);
    }

    private static object? ProductOf(string signature, bool mathcad)
    {
        try { return Call(Call(mathcad ? MType : typeof(Q.Type), "Parse", signature)!, "GetDimensionTypeProduct"); }
        catch { return null; }
    }

    [SkippableFact]
    public void Unit_systems_answer_alike()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        foreach (var (name, basis, defaults) in Systems)
        {
            object? q = null, m = null;
            Same(diffs, $"new UnitSystem({name})", Outcome(() => q = MkSystem(false, name, basis, defaults), _ => "ok"),
                Outcome(() => m = MkSystem(true, name, basis, defaults), _ => "ok"));
            if (q == null || m == null) continue;
            CompareLines(diffs, name, Describe(q, false), Describe(m, true));
        }
        // the built-in default system
        CompareLines(diffs, "default SI", Describe(Q.RuntimeOptions.s_default_unit_system, false),
            Describe(Get(Mathcad.Type("mplrun", "Mcd.RuntimeOptions"), "s_default_unit_system")!, true));
        NoDiffs(diffs);
    }

    private static void CompareLines(List<string> diffs, string what, string quire, string mathcad)
    {
        var a = quire.Split('\n');
        var b = mathcad.Split('\n');
        for (int i = 0; i < Math.Max(a.Length, b.Length); i++)
            Same(diffs, $"{what} line {i}", i < a.Length ? a[i] : "<none>", i < b.Length ? b[i] : "<none>");
    }

    [SkippableFact]
    public void Setters_reset_and_ro_number()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        string Script(bool mathcad)
        {
            var log = new List<string>();
            void Step(string what, Func<object?> f, Func<object?, string>? show = null) => log.Add(what + " = " + Outcome(f, show));
            var system = MkSystem(mathcad, "s", Base, Derived[..4]);
            Step("Defaults set", () => { Set(system, "Defaults", Units(mathcad, Derived[4..9])); return Get(system, "Defaults"); }, Symbols);
            Step("IsUnitSymbol(N) after defaults", () => Call(system, "IsUnitSymbol", "N"));
            Step("Simplify force", () => CallSig(system, "Simplify", ["String"], "V{Length 60000 Mass 60000 Time-120000}"), UnitsText);
            Step("Basis set us", () => { Set(system, "Basis", Units(mathcad, UsBase)); return Get(system, "Basis"); }, Symbols);
            Step("Normalize area", () => CallSig(system, "Normalize", ["String"], "V{Length 120000}"), UnitsText);
            Step("Basis set singular", () => { Set(system, "Basis", Units(mathcad, [.. Base[..7], Base[0]])); return "ok"; });
            Step("Rank after failed basis", () => Get(system, "Rank"));
            Step("Basis after failed basis", () => Get(system, "Basis"), Symbols);
            Step("Dimensions after failed basis", () => Get(system, "Dimensions"), Dims);
            Step("Reset", () => ReferenceEquals(Call(system, "Reset"), system));
            Step("Basis after reset", () => Get(system, "Basis"), Symbols);
            Step("Defaults after reset", () => Get(system, "Defaults"), Symbols);
            Step("IsUnitSymbol(Pa) after reset", () => Call(system, "IsUnitSymbol", "Pa"));
            Step("IsUnitSymbol(ft) after reset", () => Call(system, "IsUnitSymbol", "ft"));
            Step("RO_number set", () => { Set(system, "RO_number", 42); return Get(system, "RO_number"); });
            Step("Basis set null", () => { Set(system, "Basis", null); return "ok"; });
            Step("Defaults set null", () => { Set(system, "Defaults", null); return "ok"; });
            return string.Join("\n", log);
        }
        CompareLines(diffs, "script", Script(false), Script(true));
        NoDiffs(diffs);
    }

    [SkippableFact]
    public void Units_and_unit_members()
    {
        Runtime.Sync();
        var diffs = new List<string>();
        U[] units =
        [
            .. Base, .. Derived, new("zero", "z", "Q", 0, "V{Length 60000}"), new("inf", "i", "Q", double.PositiveInfinity, "V{Length 60000}"),
            new("nan", "n", "Q", double.NaN, "V{Length 60000}"), new("neg", "-u", "Q", -2.5, "V{Length 60000}"), new("tiny", "t", "Q", 1e-300, "V{}"),
            new("big", "B", "Q", 1e300, "F{Time 60000}"), new("poly", "p", "Q", 1, "V a"), new("matrix", "M", "Q", 1, "M(V{Length 60000})"),
            new("fn", "f", "Q", 1, "a->b"), new("const", "c", "Q", 1, "Length"), new("bad", "b", "Q", 1, ")"), new("null", "0", "Q", 1, null!),
            new("frac", "fr", "Q", 1234.5678, "V{Length 60000}"),
        ];
        (int N, int D)[] exponents = [(1, 1), (2, 1), (-1, 1), (0, 1), (1, 2), (-3, 2), (2, 3), (1, 0), (1000, 1), (-1000, 1)];
        foreach (var u in units)
        {
            object? q = null, m = null;
            Same(diffs, $"new Unit({u.Symbol})", Outcome(() => q = MkUnit(false, u), UnitText), Outcome(() => m = MkUnit(true, u), UnitText));
            if (q == null || m == null) continue;
            Same(diffs, $"{u.Symbol}.ToString", () => q.ToString(), () => m.ToString());
            Same(diffs, $"{u.Symbol}.Dimensions", () => Get(q, "Dimensions"), () => Get(m, "Dimensions"));
            foreach (var (n, d) in exponents)
                Same(diffs, $"new Units({u.Symbol}, {n}/{d})", () => new Q.Units((Q.Unit)q, new Q.Rational(n, d)),
                    () => New(MUnits, m, New(Mathcad.Type("mplrun", "Mcd.Rational"), n, d)), UnitsText);
        }
        // products of powers
        var rng = new Random(7);
        for (int k = 0; k < 200; k++)
        {
            int count = rng.Next(4);
            var pick = Enumerable.Range(0, count).Select(_ => (U: units[rng.Next(units.Length - 4)], E: exponents[rng.Next(exponents.Length)])).ToArray();
            string what = $"new Units([{string.Join(" ", pick.Select(p => $"{p.U.Symbol}^{p.E.N}/{p.E.D}"))}])";
            Same(diffs, what, () => new Q.Units(pick.Select(p => new Q.Units.Power { Base = (Q.Unit)MkUnit(false, p.U), Exponent = new Q.Rational(p.E.N, p.E.D) }).ToArray()),
                () =>
                {
                    var powerType = Nested(MUnits, "Power");
                    var array = Array.CreateInstance(powerType, pick.Length);
                    for (int i = 0; i < pick.Length; i++)
                    {
                        object p = Activator.CreateInstance(powerType)!;
                        powerType.GetField("Base")!.SetValue(p, MkUnit(true, pick[i].U));
                        powerType.GetField("Exponent")!.SetValue(p, New(Mathcad.Type("mplrun", "Mcd.Rational"), pick[i].E.N, pick[i].E.D));
                        array.SetValue(p, i);
                    }
                    return New(MUnits, array);
                }, UnitsText);
        }
        Same(diffs, "new Units(null)", () => new Q.Units(null!), () => New(MUnits, [null]), UnitsText);
        NoDiffs(diffs);
    }
}
