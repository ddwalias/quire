using System.Globalization;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.DimVal</c> base behaviour with Quire's through <c>NotAVal</c> (which overrides only
/// <c>ToString</c>) and <c>StrVal</c>, over every dimension: capability queries, the <c>as*</c> conversions and their
/// message keys, call errors and the static helpers. Cases are listed in docs/mirror/evidence/Mcd.DimVal.md.
/// </summary>
public class DimValDifferentialTests
{
    /// <summary>A value of each kind under test, with dimension <paramref name="dim"/> set through <c>dv</c>.</summary>
    private static IEnumerable<(string Name, object Value)> Values(Side s, string dim)
    {
        yield return ($"NotAVal {dim}", Call(s.NotAVal(), "dv", s.D(dim))!);
        yield return ($"StrVal {dim}", Call(s.Str("abc"), "dv", s.D(dim))!);
    }

    private static readonly string[] Queries =
    [
        "IsPolymorphic", "PhysicalUnit", "PhysicalDimension", "UniformDimension", "IsString", "IsReal", "IsDimless",
        "IsDimlessReal", "IsMatrix", "IsFunction", "Rsize", "Rfinal", "Isize",
    ];

    [SkippableFact]
    public void Queries_and_conversions() => Scenario.Compare((s, log) =>
    {
        foreach (var dim in Side.DimNames)
            foreach (var (n, v) in Values(s, dim))
            {
                foreach (var q in Queries) log.Log(s, $"{n}.{q}", () => Get(v, q));
                log.Log(s, $"{n}.ToString(culture)", () => CallSig(v, "ToString", ["IFormatProvider"], CultureInfo.InvariantCulture));
                log.Log(s, $"{n}.GetUnits(true)", () => CallSig(v, "GetUnits", ["Boolean"], true));
                log.Log(s, $"{n}.GetUnits(false)", () => CallSig(v, "GetUnits", ["Boolean"], false));
                foreach (var f in new[] { "asString", "asReal", "asInt", "asDimlessInt", "asVector", "asRealVector", "asRealMatrix", "asMatrix", "asRange", "asFunction" })
                {
                    log.Log(s, $"{n}.{f}()", () => CallSig(v, f, []));
                    log.Log(s, $"{n}.{f}(3)", () => CallSig(v, f, ["UInt32"], 3u));
                    log.Log(s, $"{n}.{f}(m, 4)", () => CallSig(v, f, ["String", "UInt32"], "custom key", 4u));
                }
                log.Add($"{n}.isInt = {s.IsInt(v)}");
                log.Log(s, $"{n}.asDimless()", () => CallSig(v, "asDimless", []));
                log.Log(s, $"{n}.asDimless(5)", () => CallSig(v, "asDimless", ["UInt32"], 5u));
                log.Log(s, $"{n}.asDimlessReal(5)", () => CallSig(v, "asDimlessReal", ["UInt32"], 5u));
                log.Log(s, $"{n}.asDimlessReal(m, 5)", () => CallSig(v, "asDimlessReal", ["String", "UInt32"], "custom key", 5u));
                log.Log(s, $"{n}.asTuple(1, 2, 7)", () => Call(v, "asTuple", 1u, 2u, 7u));
                log.Log(s, $"{n}.IsFunctionN(2)", () => Call(v, "IsFunctionN", 2));
                log.Log(s, $"{n}.asFunctionN(2)", () => CallSig(v, "asFunctionN", ["Int32"], 2));
                log.Log(s, $"{n}.asFunctionN(2, 6)", () => CallSig(v, "asFunctionN", ["Int32", "UInt32"], 2, 6u));
                foreach (var f in new[] { "range_get", "iterator_get" })
                    log.Log(s, $"{n}.{f}(5) is this", () => ReferenceEquals(Call(v, f, 5), v));
                foreach (var f in new[] { "asIterator", "asImmutable", "asResult" })
                    log.Log(s, $"{n}.{f}() is this", () => ReferenceEquals(Call(v, f), v));
                log.Log(s, $"{n}.matrix_get_1(0)", () => Call(v, "matrix_get_1", 0));
                log.Log(s, $"{n}.sym_matrix_get_1(0)", () => Call(v, "sym_matrix_get_1", 0));
                log.Log(s, $"{n}.matrix_get_2(0, 0)", () => Call(v, "matrix_get_2", 0, 0));
                log.Log(s, $"{n}.sym_matrix_get_2(0, 0)", () => Call(v, "sym_matrix_get_2", 0, 0));
                log.Log(s, $"{n}.Apply([])", () => Call(v, "Apply", new object?[] { Array.Empty<object>() }));
            }
    });

    [SkippableFact]
    public void Call_errors() => Scenario.Compare((s, log) =>
    {
        var v = s.NotAVal();
        log.Log(s, "DynCall()", () => CallSig(v, "DynCall", []));
        for (int n = 1; n <= 16; n++)
        {
            var args = Enumerable.Repeat<object?>("x", n).ToArray();
            log.Log(s, $"DynCall({n})", () => CallSig(v, "DynCall", Enumerable.Repeat("Object", n).ToArray(), args));
            if (n <= 3) log.Log(s, $"VecCall({n})", () => CallSig(v, "VecCall", Enumerable.Repeat("Object", n).ToArray(), args));
        }
        log.Log(s, "DynApply(3)", () => Call(v, "DynApply", new object?[] { new object[3] }));
        log.Log(s, "VecApply(2)", () => Call(v, "VecApply", new object?[] { new object[2] }));
        foreach (int required in new[] { -2, -1, 0, 1, 2, 3 })
            foreach (int actual in new[] { 0, 1, 2, 3, 4 })
            {
                log.Log(s, $"BadCall({required}, {actual})", () => CallSig(v, "BadCall", ["Int32", "Int32"], required, actual));
                log.Log(s, $"BadCall({required}, {actual}, 9)", () => CallSig(v, "BadCall", ["Int32", "Int32", "UInt32"], required, actual, 9u));
                log.Log(s, $"WrongNumberOfArguments({required}, {actual})",
                    () => CallSig(s.T("Mcd.DimVal"), "WrongNumberOfArguments", ["Int32", "Int32"], required, actual));
                log.Log(s, $"WrongNumberOfArguments({required}, {actual}, 8)",
                    () => CallSig(s.T("Mcd.DimVal"), "WrongNumberOfArguments", ["Int32", "Int32", "UInt32"], required, actual, 8u));
            }
        var dv = s.T("Mcd.DimVal");
        log.Log(s, "OnError(a, m, i, j)", () => CallSig(dv, "OnError", ["UInt32", "String", "Int32", "Int32"], 3u, "key", 1, 2));
        log.Log(s, "OnError(a, m, i)", () => CallSig(dv, "OnError", ["UInt32", "String", "Int32"], 3u, "key", 1));
        foreach (var thing in new object?[] { null, "text", 2.5, v, "12>tagged" })
            log.Log(s, $"OnError(a, m, {Text(thing)})", () => CallSig(dv, "OnError", ["UInt32", "String", "Object"], 3u, "key", thing));
        log.Log(s, "OnError(MaxValue, m@4, i)", () => CallSig(dv, "OnError", ["UInt32", "String", "Int32"], uint.MaxValue, "key@4", 1));
    });

    [SkippableFact]
    public void Static_helpers() => Scenario.Compare((s, log) =>
    {
        var dv = s.T("Mcd.DimVal");
        foreach (var dim in Side.DimNames)
        {
            log.Log(s, $"GetUnits({dim}, true)", () => CallSig(dv, "GetUnits", ["Dim", "Boolean"], s.D(dim), true));
            log.Log(s, $"GetUnits({dim}, false)", () => CallSig(dv, "GetUnits", ["Dim", "Boolean"], s.D(dim), false));
            foreach (var (n, v) in Values(s, "L"))
            {
                log.Log(s, $"IsDimOf({n}, {dim})", () => Call(dv, "IsDimOf", v, s.D(dim)));
                log.Log(s, $"{n}.IsUniformDimOf({dim})", () => Call(v, "IsUniformDimOf", s.D(dim)));
            }
            var a = s.NotAVal();
            log.Log(s, $"dyn(NotAVal, {dim}) is same", () => ReferenceEquals(Call(dv, "dyn", a, s.D(dim)), a));
            log.Log(s, $"  then", () => a);
            var b = s.NotAVal();
            log.Log(s, $"DimCast(NotAVal, {dim}) is same", () => ReferenceEquals(Call(dv, "DimCast", b, s.D(dim)), b));
            log.Log(s, $"  then", () => b);
            var c = s.Str("x");
            log.Log(s, $"StrVal.SetDim({dim}) is same", () => ReferenceEquals(Call(c, "SetDim", s.D(dim)), c));
            log.Log(s, $"  then", () => c);
        }
        foreach (var o in new object?[] { "plain", 3, null })
        {
            log.Log(s, $"dyn({Text(o)})", () => Call(dv, "dyn", o, s.D("L")));
            log.Log(s, $"DimCast({Text(o)})", () => Call(dv, "DimCast", o, s.D("L")));
            log.Log(s, $"IsDimOf({Text(o)})", () => Call(dv, "IsDimOf", o, s.D("L")));
            log.Log(s, $"as_result({Text(o)})", () => Call(dv, "as_result", o));
            log.Log(s, $"DimOf({Text(o)})", () => Call(dv, "DimOf", o));
        }
        foreach (var dim in Side.DimNames)
            log.Log(s, $"DimOf(NotAVal {dim})", () => Call(dv, "DimOf", Call(s.NotAVal(), "dv", s.D(dim))));
        log.Log(s, "as_result(NotAVal) is same", () => { var v = s.NotAVal(); return ReferenceEquals(Call(dv, "as_result", v), v); });
        // NearlyEqual: values compare with nearly_equal (Equals for these), anything else with x1.Equals
        object?[] xs = [s.Str("a"), s.Str("a"), s.Str("b"), s.NotAVal(), "a", 1.0, null];
        for (int i = 0; i < xs.Length; i++)
            for (int j = 0; j < xs.Length; j++)
            {
                object? x = xs[i], y = xs[j];
                log.Log(s, $"NearlyEqual({i}, {j})", () => Call(dv, "NearlyEqual", x, y, 0.1));
                if (x != null && x.GetType().Name is "StrVal" or "NotAVal")
                    log.Log(s, $"nearly_equal({i}, {j})", () => CallSig(x, "nearly_equal", ["DimVal"], y is string or double ? null : y, 0.1));
            }
        var d = s.NotAVal();
        log.Log(s, "dv is this", () => ReferenceEquals(Call(d, "dv", s.D("T")), d));
        log.Log(s, "  then", () => d);
    });
}
