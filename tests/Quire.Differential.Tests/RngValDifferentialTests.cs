using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.RngVal</c>, <c>Mcd.RngValInterpolate</c>, <c>Mcd.Iterator</c> (with its scalar and
/// sequence iterators) and <c>Mcd.RangeIterator</c> with Quire's: range construction and its errors, element access
/// past either end, equality, copies, and iteration. Cases are listed in docs/mirror/evidence/Mcd.RngVal.md.
/// </summary>
public class RngValDifferentialTests
{
    private static readonly (double First, double Incr, double Last)[] Steps =
    [
        (0, 1, 10), (0, 0.1, 1), (0, 0.25, 1), (1, 0.3, 2), (10, -1, 0), (1, -0.1, 0), (0, 2, 9), (0, 1, 0), (0, 1, -1), (0, -1, 1),
        (0, 0, 1), (double.NaN, 1, 2), (0, double.NaN, 2), (0, 1, double.NaN), (0, 1e-9, 10), (0, 1, double.PositiveInfinity),
        (0, 1, double.NegativeInfinity), (-5, 0.5, 5), (0.1, 0.1, 0.3), (1e10, 1, 1e10 + 5), (0, 1e-300, 1), (0, 3, 10),
    ];

    /// <summary>The range, then its values and queries; indices run one past each end.</summary>
    private static void Describe(Side s, List<string> log, string n, object? r)
    {
        log.Add($"{n} = {s.Show(r)}");
        if (r == null || r.GetType().Name is not ("RngVal" or "RngValInterpolate")) return;
        int count = (int)Get(r, "m_n")!;
        foreach (var p in new[] { "Rsize", "Isize", "Integral", "PhysicalDimension", "IsDimless", "Rfinal" })
            log.Log(s, $"{n}.{p}", () => Get(r, p));
        foreach (int i in new[] { -1, 0, 1, count - 1, count }.Distinct().Where(i => count < 50 || i < 3 || i >= count - 1))
        {
            log.Log(s, $"{n}.get({i})", () => Call(r, "get", i));
            log.Log(s, $"{n}.range_get({i})", () => Call(r, "range_get", i));
            log.Log(s, $"{n}.iterator_get({i})", () => Call(r, "iterator_get", i));
        }
        log.Log(s, $"{n}.GetHashCode", () => r.GetHashCode());
        log.Log(s, $"{n}.asResult", () => { var c = Call(r, "asResult"); return $"{(ReferenceEquals(c, r) ? "same" : "copy")} {s.Show(c)} equal {r.Equals(c)}"; });
        log.Log(s, $"{n}.asRange is this", () => ReferenceEquals(CallSig(r, "asRange", ["String", "UInt32"], "m", 1u), r));
    }

    [SkippableFact]
    public void Construction() => Scenario.Compare((s, log) =>
    {
        var rv = s.T("Mcd.RngVal");
        foreach (var (f, i, l) in Steps)
        {
            string p = $"({Side.Bits(f)}, {Side.Bits(i)}, {Side.Bits(l)})";
            Describe(s, log, $"IncrRange{p}", OutcomeOrNull(() => CallSig(rv, "IncrRange", ["Double", "Double", "Double", "Dim"], f, i, l, s.D("L")), s, log, $"IncrRange{p} error"));
            Describe(s, log, $"IncrRange{p} m", OutcomeOrNull(() => CallSig(rv, "IncrRange", ["Double", "Double", "Double", "Dim", "String"], f, i, l, s.D("poly"), "my key"), s, log, $"IncrRange{p} m error"));
            Describe(s, log, $"Range({Side.Bits(f)}, {Side.Bits(l)})", OutcomeOrNull(() => CallSig(rv, "Range", ["Double", "Double"], f, l), s, log, "Range2 error"));
            Describe(s, log, $"Range{p}", OutcomeOrNull(() => CallSig(rv, "Range", ["Double", "Double", "Double", "Dim"], f, f + i, l, s.D("identity")), s, log, "Range3 error"));
        }
        foreach (int n in new[] { 0, 1, 5, -3 })
            Describe(s, log, $"IncrRange(2, 0.5, n {n})", CallSig(rv, "IncrRange", ["Double", "Double", "Int32", "Dim"], 2.0, 0.5, n, s.D("T")));
    });

    private static object? OutcomeOrNull(Func<object?> f, Side s, List<string> log, string what)
    {
        try { return f(); }
        catch (Exception e) { log.Add($"{what} = {Thrown(e)}"); return null; }
    }

    [SkippableFact]
    public void Range_operators() => Scenario.Compare((s, log) =>
    {
        var rv = s.T("Mcd.RngVal");
        (string, object)[] values =
        [
            ("0", s.Num(0, 0, "identity")), ("3", s.Num(3, 0, "identity")), ("0.5", s.Num(0.5, 0, "identity")), ("-2", s.Num(-2, 0, "identity")),
            ("1 m", s.Num(1, 0, "L")), ("4 m", s.Num(4, 0, "L")), ("2 s", s.Num(2, 0, "T")), ("1+i", s.Num(1, 1, "identity")),
            ("3 poly", s.Num(3, 0, "poly")), ("0 zero", s.Num(0, 0, "mcd_zero")), ("str", s.Str("x")),
        ];
        foreach (var (an, a) in values)
            foreach (var (bn, b) in values)
            {
                Describe(s, log, $"mkVec2({an}, {bn})", OutcomeOrNull(() => Call(rv, "mkVec2", a, b), s, log, $"mkVec2({an}, {bn}) error"));
                Describe(s, log, $"mkRange2({an}, {bn})", OutcomeOrNull(() => Call(rv, "mkRange2", a, b), s, log, $"mkRange2({an}, {bn}) error"));
                foreach (var (cn, c) in values.Where(v => v.Item1 is "3" or "4 m" or "2 s" or "str"))
                {
                    Describe(s, log, $"mkVec3({an}, {bn}, {cn})", OutcomeOrNull(() => Call(rv, "mkVec3", a, b, c), s, log, $"mkVec3({an}, {bn}, {cn}) error"));
                    Describe(s, log, $"mkRange3({an}, {bn}, {cn})", OutcomeOrNull(() => Call(rv, "mkRange3", a, b, c), s, log, $"mkRange3({an}, {bn}, {cn}) error"));
                }
            }
    });

    [SkippableFact]
    public void Equality() => Scenario.Compare((s, log) =>
    {
        var rv = s.T("Mcd.RngVal");
        object Make(double f, double i, int n, string d) => CallSig(rv, "IncrRange", ["Double", "Double", "Int32", "Dim"], f, i, n, s.D(d))!;
        (string, object)[] ranges =
        [
            ("a", Make(0, 1, 5, "L")), ("a'", Make(0, 1, 5, "L")), ("other n", Make(0, 1, 6, "L")), ("other dim", Make(0, 1, 5, "T")),
            ("zero dim", Make(0, 1, 5, "mcd_zero")), ("identity", Make(0, 1, 5, "identity")),
            ("interp", CallSig(rv, "IncrRange", ["Double", "Double", "Double", "Dim"], 0.0, 0.1, 1.0, s.D("L"))!),
            ("plain 0.1", Make(0, 0.1, 11, "L")),
        ];
        foreach (var (an, a) in ranges)
        {
            foreach (var (bn, b) in ranges) log.Log(s, $"{an} == {bn}", () => a.Equals(b));
            log.Log(s, $"{an} == number", () => a.Equals(s.Num(0, 0, "L")));
            log.Log(s, $"{an} == null", () => a.Equals(null));
        }
        log.Log(s, "polymorphic hash", () => Make(0, 1, 5, "poly").GetHashCode());
        log.Log(s, "polymorphic PhysicalDimension", () => Get(Make(0, 1, 5, "poly"), "PhysicalDimension"));
    });

    /// <summary>Walks an iterator with first/next, logging values, up to a limit.</summary>
    private static string Walk(Side s, object? it, int limit = 30)
    {
        var values = new List<string>();
        for (int k = 0; it != null && k < limit; k++)
        {
            object current = it;
            values.Add(s.Out(() => Call(current, "first")));
            try { it = Call(current, "next"); }
            catch (Exception e) { values.Add(Thrown(e)); break; }
            if (it != null && !ReferenceEquals(it, current)) values.Add("(new iterator)");
        }
        return string.Join(" | ", values) + (it == null ? " | end" : " | ...");
    }

    [SkippableFact]
    public void Iterators() => Scenario.Compare((s, log) =>
    {
        var it = s.T("Mcd.Iterator");
        var rv = s.T("Mcd.RngVal");
        log.Add("forRange(1, 0.5, 4) = " + Walk(s, CallSig(it, "forRange", ["Double", "Double", "Int32"], 1.0, 0.5, 4)));
        log.Add("forRange(1, 0.5, 0) = " + Walk(s, CallSig(it, "forRange", ["Double", "Double", "Int32"], 1.0, 0.5, 0)));
        log.Add("forRange(3, 0) = " + Walk(s, CallSig(it, "forRange", ["Double", "Double"], 3.0, 0.0)));
        log.Add("forRange(0, 0.1, 0.5) = " + Walk(s, CallSig(it, "forRange", ["Double", "Double", "Double"], 0.0, 0.1, 0.5)));
        log.Log(s, "forRange(0, 0, 1)", () => CallSig(it, "forRange", ["Double", "Double", "Double"], 0.0, 0.0, 1.0));
        log.Add("forRange(1 m, 1.5 m, 3 m) = " + Walk(s, CallSig(it, "forRange", ["DimVal", "DimVal", "DimVal"], s.Num(1, 0, "L"), s.Num(1.5, 0, "L"), s.Num(3, 0, "L"))));
        log.Log(s, "forRange(1 m, 2 s, 3 m)", () => CallSig(it, "forRange", ["DimVal", "DimVal", "DimVal"], s.Num(1, 0, "L"), s.Num(2, 0, "T"), s.Num(3, 0, "L")));
        log.Add("forRange(1, 4 dv) = " + Walk(s, CallSig(it, "forRange", ["DimVal", "DimVal"], s.Num(1, 0, "identity"), s.Num(4, 0, "identity"))));
        log.Log(s, "forRange(1 m, 4 m dv)", () => CallSig(it, "forRange", ["DimVal", "DimVal"], s.Num(1, 0, "L"), s.Num(4, 0, "L")));
        log.Add("forValue(range) = " + Walk(s, Call(it, "forValue", CallSig(rv, "IncrRange", ["Double", "Double", "Int32", "Dim"], 0.0, 2.0, 3, s.D("T")))));
        log.Add("forValue(number) = " + Walk(s, Call(it, "forValue", s.Num(7, 0, "L"))));
        log.Add("forValue(string) = " + Walk(s, Call(it, "forValue", "text")));
        log.Add("forValue(null) = " + Walk(s, Call(it, "forValue", [null])));
        // sequences: each part runs to its end; an empty range still yields its first value once
        object Range(int n) => CallSig(it, "forRange", ["Double", "Double", "Int32"], 10.0 * n, 1.0, n)!;
        object Scalar(object? v) => New(s.T("Mcd.Iterator+ScalarIterator"), v);
        Array Parts(params object[] parts)
        {
            var a = Array.CreateInstance(it, parts.Length);
            for (int i = 0; i < parts.Length; i++) a.SetValue(parts[i], i);
            return a;
        }
        log.Add("forValues(range 2, scalar, range 3) = " + Walk(s, Call(it, "forValues", Parts(Range(2), Scalar("x"), Range(3)))));
        log.Add("forValues(range 0, scalar) = " + Walk(s, Call(it, "forValues", Parts(Range(0), Scalar("y")))));
        log.Add("forValues(scalar) = " + Walk(s, Call(it, "forValues", Parts(Scalar(null)))));
        log.Log(s, "forValues(empty).first", () => Call(Call(it, "forValues", Parts())!, "first"));
        var counting = CallSig(it, "forRange", ["Double", "Double", "Int32"], 5.0, 1.0, 2)!;
        log.Log(s, "it_incr 1", () => Call(it, "it_incr", null, counting));
        log.Log(s, "it_incr 2", () => Call(it, "it_incr", null, counting));
        log.Log(s, "it_incr 3", () => Call(it, "it_incr", null, counting));
        var ri = New(s.T("Mcd.RangeIterator"), CallSig(rv, "IncrRange", ["Double", "Double", "Int32", "Dim"], 0.0, 1.0, 2, s.D("identity")));
        log.Log(s, "ri_first", () => Call(ri, "ri_first"));
        log.Log(s, "ri_next is this", () => ReferenceEquals(Call(ri, "ri_next"), ri));
        log.Log(s, "ri_first again", () => Call(ri, "ri_first"));
        log.Log(s, "ri_next at end", () => Call(ri, "ri_next"));
        log.Log(s, "ri_first past end", () => Call(ri, "ri_first"));
    });
}
