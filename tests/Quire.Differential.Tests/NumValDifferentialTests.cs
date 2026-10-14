using System.Globalization;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.NumVal</c>, <c>Mcd.num_val</c>, <c>Mcd.StrVal</c> and <c>Mcd.NotAVal</c> with Quire's:
/// statics, constructors, setters, exact and fuzzy equality and ordering (strict and not), integer detection,
/// formatting in two cultures, and the string and no-value types. Cases are listed in
/// docs/mirror/evidence/Mcd.NumVal.md.
/// </summary>
public class NumValDifferentialTests
{
    private static readonly double[] Doubles =
    [
        0, -0.0, 1, -1, 2.5, 3.5, -2.5, 0.1, 1e-14, -1e-14, 1 + 1e-13, 1 + 1e-12, 5.0000000000001, 1e300, -1e300,
        2147483647, 2147483648, -2147483648, -2147483649, 3e9, 4294967295, 4294967296, 4503599627370496, 9007199254740993,
        double.Epsilon, double.MaxValue, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
    ];

    /// <summary>Numbers over a spread of parts and dimensions.</summary>
    private static List<(string Name, object Value)> Numbers(Side s)
    {
        var list = new List<(string, object)>();
        foreach (var (re, im) in new[] { (0.0, 0.0), (1.0, 0.0), (1.0 + 1e-13, 0.0), (1.0 + 1e-11, 0.0), (2.0, 0.0), (-2.0, 0.0),
                     (0.0, 1.0), (0.0, 2.0), (1.0, 1.0), (1.0, -1.0), (1.0 + 1e-13, 1.0), (double.NaN, 0.0), (double.PositiveInfinity, 0.0) })
            foreach (var dim in new[] { "poly", "identity", "mcd_zero", "L", "T" })
                list.Add(($"({Side.Bits(re)}, {Side.Bits(im)}) {dim}", s.Num(re, im, dim)));
        return list;
    }

    [SkippableFact]
    public void Statics_and_construction() => Scenario.Compare((s, log) =>
    {
        var nv = s.T("Mcd.NumVal");
        foreach (var f in new[] { "Zero", "ZeroId", "ZeroPoly", "One", "NoValue", "mcNaN", "Inf", "McdNaN", "NaN", "NoNoNaN",
                     "normalize_exponent", "machine_eps", "default_fuzz" })
            log.Log(s, f, () => Get(nv, f));
        log.Log(s, "NoValue is NotAVal.singleton", () => ReferenceEquals(Get(nv, "NoValue"), Get(s.T("Mcd.NotAVal"), "singleton")));
        log.Log(s, "new NumVal()", () => New(nv));
        log.Log(s, "new NumVal(int)", () => New(nv, 7));
        log.Log(s, "new NumVal(double)", () => New(nv, 7.5));
        log.Log(s, "new NumVal(double, double)", () => New(nv, 7.5, -1.0));
        log.Log(s, "new NumVal(double, double, Dim)", () => s.Num(7.5, -1, "L"));
        log.Log(s, "new NumVal(NumVal)", () => New(nv, s.Num(7.5, -1, "L")));
        log.Log(s, "new NumVal(NumVal, Dim)", () => New(nv, s.Num(7.5, -1, "L"), s.D("T")));
        log.Log(s, "new NumVal(null)", () => CallCtor(nv, ["NumVal"], null));
        log.Log(s, "NumDv", () => Call(nv, "NumDv", 1.5, 2.5));
        log.Log(s, "IntDv", () => Call(nv, "IntDv", -3));
        log.Log(s, "RealDv", () => Call(nv, "RealDv", 4.5));
        foreach (var payload in new[] { 0, 1, 0x12345678, -1, int.MinValue })
            log.Log(s, $"LoadNaN({payload})", () => Call(nv, "LoadNaN", payload));
        var nvt = s.T("Mcd.num_val");
        log.Log(s, "num_val()", () => Activator.CreateInstance(nvt));
        log.Log(s, "num_val(r)", () => New(nvt, 2.5));
        log.Log(s, "num_val(r, i)", () => New(nvt, 2.5, -1.5));
        foreach (var d in Doubles)
        {
            var a = s.Num(9, 9, "L");
            log.Log(s, $"Set({Side.Bits(d)})", () => CallSig(a, "Set", ["Double"], d));
            var b = s.Num(9, 9, "L");
            log.Log(s, $"Set({Side.Bits(d)}, 2)", () => CallSig(b, "Set", ["Double", "Double"], d, 2.0));
        }
        foreach (int i in new[] { 0, 5, -5, int.MaxValue })
        {
            var a = s.Num(9, 9, "L");
            log.Log(s, $"Set(int {i})", () => CallSig(a, "Set", ["Int32"], i));
        }
        var r = s.Num(1, 2, "L");
        log.Log(s, "Real = 5", () => { Set(r, "Real", 5.0); return r; });
        log.Log(s, "Imag = 6", () => { Set(r, "Imag", 6.0); return r; });
        foreach (var o in new object?[] { null, "x", false, 0, r })
            log.Log(s, $"McdBool({Text(o)})", () => Call(nv, "McdBool", o));
        foreach (int i in new[] { 0, 1, -1 })
            log.Log(s, $"ibv({i})", () => Call(nv, "ibv", i));
        log.Log(s, "NotNoValue(NoValue)", () => Call(nv, "NotNoValue", Get(nv, "NoValue")));
        log.Log(s, "NotNoValue(new NotAVal)", () => Call(nv, "NotNoValue", s.NotAVal()));
        log.Log(s, "NotNoValue(null)", () => Call(nv, "NotNoValue", [null]));
        foreach (var (n, v) in Numbers(s))
            foreach (var d in new[] { 2.0, 0.0, double.NaN })
                log.Log(s, $"{n} / {Side.Bits(d)}", () => Call(nv, "op_Division", v, d));
    });

    private static object? CallCtor(System.Type t, string[] sig, params object?[] args)
    {
        var c = t.GetConstructors().Single(c => c.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(sig));
        try { return c.Invoke(args); }
        catch (System.Reflection.TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
    }

    [SkippableFact]
    public void Properties_and_integers() => Scenario.Compare((s, log) =>
    {
        foreach (var (n, v) in Numbers(s))
        {
            foreach (var p in new[] { "PhysicalDimension", "IsPureReal", "IsPureImag", "IsInt", "IsReal", "IsDimless", "IsDimlessReal", "PhysicalUnit", "Rsize" })
                log.Log(s, $"{n}.{p}", () => Get(v, p));
            log.Log(s, $"{n}.GetHashCode", () => v.GetHashCode());
            log.Log(s, $"{n}.asResult", () => { var r = Call(v, "asResult"); return $"{(ReferenceEquals(r, v) ? "same" : "copy")} {s.Show(r)}"; });
            log.Log(s, $"{n}.asReal(m, 3)", () => CallSig(v, "asReal", ["String", "UInt32"], "custom key", 3u));
            log.Log(s, $"{n}.asDimlessReal(4)", () => CallSig(v, "asDimlessReal", ["UInt32"], 4u));
            log.Log(s, $"{n}.asDimlessInt(4)", () => CallSig(v, "asDimlessInt", ["UInt32"], 4u));
            log.Log(s, $"{n}.SetDim(T)", () => { var r = Call(v, "SetDim", s.D("T")); return $"{(ReferenceEquals(r, v) ? "same" : "copy")} {s.Show(r)} / {s.Show(v)}"; });
        }
        foreach (var d in Doubles)
            foreach (var im in new[] { 0.0, 1.0 })
            {
                var v = s.Num(d, im, "identity");
                log.Add($"isInt({Side.Bits(d)}, {im}) = {s.IsInt(v)}");
                log.Log(s, $"asInt({Side.Bits(d)}, {im})", () => CallSig(v, "asInt", ["UInt32"], 2u));
                log.Log(s, $"IsInt({Side.Bits(d)}, {im})", () => Get(v, "IsInt"));
            }
    });

    [SkippableFact]
    public void Equality_and_order() => Scenario.Compare((s, log) =>
    {
        var nv = s.T("Mcd.NumVal");
        var numbers = Numbers(s);
        foreach (bool strict in new[] { false, true })
            s.Strict(strict, () =>
            {
                foreach (var (an, a) in numbers)
                    foreach (var (bn, b) in numbers)
                    {
                        string p = $"{(strict ? "strict " : "")}{an} ~ {bn}";
                        log.Log(s, $"{p} Equals", () => a.Equals(b));
                        log.Log(s, $"{p} Equals1", () => Call(a, "Equals1", b));
                        log.Log(s, $"{p} nearly_equal 1e-9", () => Call(a, "nearly_equal", b, 1e-9));
                        log.Log(s, $"{p} ExactEquals", () => Call(nv, "ExactEquals", a, b));
                        log.Log(s, $"{p} FuzzyEquals", () => CallSig(nv, "FuzzyEquals", ["NumVal", "NumVal"], a, b));
                        log.Log(s, $"{p} FuzzyEquals 1e-11", () => CallSig(nv, "FuzzyEquals", ["NumVal", "NumVal", "Double"], a, b, 1e-11));
                        foreach (var f in new[] { "LessThan", "GreaterThan", "LessEqThan", "GreaterEqThan" })
                            log.Log(s, $"{p} {f}", () => Call(nv, f, a, b));
                    }
                foreach (var x in Doubles)
                    foreach (var y in new[] { 0.0, 1.0, 1 + 1e-13, 1 - 1e-13, -1.0, 2147483648, double.NaN, double.PositiveInfinity })
                    {
                        string p = $"{(strict ? "strict " : "")}{Side.Bits(x)} ~ {Side.Bits(y)}";
                        foreach (var f in new[] { "LT", "GT", "LE", "GE" })
                            log.Log(s, $"{p} {f}", () => Call(nv, f, x, y));
                        log.Log(s, $"{p} FuzzyEquals", () => CallSig(nv, "FuzzyEquals", ["Double", "Double"], x, y));
                        log.Log(s, $"{p} FuzzyEquals 1e-3", () => CallSig(nv, "FuzzyEquals", ["Double", "Double", "Double"], x, y, 1e-3));
                    }
                return 0;
            });
        var one = s.Num(1, 0, "identity");
        log.Log(s, "Equals(null)", () => one.Equals(null));
        log.Log(s, "Equals(string)", () => one.Equals("1"));
        log.Log(s, "Equals1(StrVal)", () => Call(one, "Equals1", s.Str("1")));
        log.Log(s, "nearly_equal(StrVal)", () => Call(one, "nearly_equal", s.Str("1"), 0.1));
    });

    [SkippableFact]
    public void Formatting() => Scenario.Compare((s, log) =>
    {
        var values = Numbers(s).Concat(new (string, object)[]
        {
            ("1500 v", s.Num(1500, 0, "v")), ("2 N", s.Num(2, -3, "N")), ("1e-3 L2", s.Num(1e-3, 0, "L2")),
            ("-0.5i", s.Num(0, -0.5, "identity")), ("1/3", s.Num(1.0 / 3, 2.0 / 3, "identity")), ("-0 -0", s.Num(-0.0, -0.0, "identity")),
            ("NaN i", s.Num(1, double.NaN, "identity")),
        });
        foreach (var (n, v) in values)
        {
            log.Log(s, $"{n}.ToString()", () => v.ToString());
            log.Log(s, $"{n}.ToString(F2)", () => CallSig(v, "ToString", ["String"], "F2"));
            log.Log(s, $"{n}.Format(true)", () => CallSig(v, "Format", ["Boolean"], true));
            log.Log(s, $"{n}.Format(true, true)", () => CallSig(v, "Format", ["Boolean", "Boolean"], true, true));
            log.Log(s, $"{n}.Format(E3, true)", () => CallSig(v, "Format", ["String", "Boolean"], "E3", true));
            log.Log(s, $"{n}.Format(G4, false, true)", () => CallSig(v, "Format", ["String", "Boolean", "Boolean"], "G4", false, true));
            foreach (var culture in new[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de-DE") })
            {
                log.Log(s, $"{n}.ToString({culture.Name})", () => CallSig(v, "ToString", ["IFormatProvider"], culture));
                log.Log(s, $"{n}.ToString({culture.Name}, N3)", () => CallSig(v, "ToString", ["IFormatProvider", "String"], culture, "N3"));
                log.Log(s, $"{n}.Format({culture.Name}, g15, true, false)",
                    () => CallSig(v, "Format", ["IFormatProvider", "String", "Boolean", "Boolean"], culture, "g15", true, false));
            }
            log.Log(s, $"{n}.Format(bad format)", () => CallSig(v, "Format", ["String", "Boolean"], "{", false));
        }
    });

    [SkippableFact]
    public void Strings_and_no_value() => Scenario.Compare((s, log) =>
    {
        foreach (var text in new[] { "abc", "", "a\tb", null })
        {
            var v = s.Str(text);
            log.Log(s, $"StrVal({Text(text)})", () => v);
            foreach (var p in new[] { "IsString", "m_string", "PhysicalDimension", "UniformDimension", "IsDimless", "Rsize" })
                log.Log(s, $"StrVal({Text(text)}).{p}", () => Get(v, p));
            log.Log(s, $"StrVal({Text(text)}).ToString", () => v.ToString());
            log.Log(s, $"StrVal({Text(text)}).asString()", () => CallSig(v, "asString", []));
            log.Log(s, $"StrVal({Text(text)}).asString(m, 2)", () => CallSig(v, "asString", ["String", "UInt32"], "key", 2u));
            log.Log(s, $"StrVal({Text(text)}).asReal()", () => CallSig(v, "asReal", []));
            log.Log(s, $"StrVal({Text(text)}).GetHashCode == string's", () => v.GetHashCode() == text?.GetHashCode());
            foreach (var other in new object?[] { s.Str("abc"), s.Str(null), "abc", null })
                log.Log(s, $"StrVal({Text(text)}).Equals({s.Show(other)})", () => v.Equals(other));
        }
        var na = s.T("Mcd.NotAVal");
        log.Log(s, "singleton", () => Get(na, "singleton"));
        log.Log(s, "singleton.ToString", () => Get(na, "singleton")!.ToString());
        log.Log(s, "new NotAVal()", () => s.NotAVal());
        log.Log(s, "new NotAVal() equals singleton", () => s.NotAVal().Equals(Get(na, "singleton")));
    });
}
