using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValRangeIntegralDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const int JoeyBit = 2;
    private const int Integral = 8;
    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatVal").GetMethod(name, Declared) ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}");

    private static object Matrix(Side side, int flags = 0, int rows = 2, int cols = 1, int stride = 3, int offset = 1)
    {
        int length = offset + stride * cols;
        var matrix = New(side.T("Mcd.MatVal"), Enumerable.Range(0, length).Select(i => 10.0 + i).ToArray(),
            new double[length], rows, cols, stride, new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), flags), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static string ArrayText(Side side, object? value) => value is double[] array
        ? "[" + string.Join(",", array.Select(Side.Bits)) + "]"
        : "null";

    private static string ValueText(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
            return MatValDifferentialTests.Storage(side, value);
        return side.Show(value);
    }

    private sealed record EntrySnapshot(object? Value, string Before);
    private sealed record Retained(object? Real, object? Imag, object? Table, double[]? RealArray,
        double[]? ImagArray, IReadOnlyDictionary<object, EntrySnapshot> Entries);

    private static Retained Keep(Side side, object target)
    {
        var real = Get(target, "m_rarr"); var imag = Get(target, "m_iarr"); var table = Get(target, "m_parr");
        var entries = table is Hashtable ht
            ? ht.Cast<DictionaryEntry>().ToDictionary(e => e.Key, e => new EntrySnapshot(e.Value, ValueText(side, e.Value)))
            : new Dictionary<object, EntrySnapshot>();
        return new(real, imag, table, real as double[], imag as double[], entries);
    }

    private static string State(Side side, Retained retained, object target)
    {
        object? real = Get(target, "m_rarr"), imag = Get(target, "m_iarr"), table = Get(target, "m_parr");
        string Ref(object? old, object? now) => ReferenceEquals(old, now) ? "same" : "changed";
        string Table(object? value) => value is Hashtable ht
            ? string.Join(";", ht.Cast<DictionaryEntry>().OrderBy(e => Convert.ToInt32(e.Key))
                .Select(e => $"{e.Key}={ValueText(side, e.Value)}"))
            : "null";
        string CurrentPointerRefs() => table is Hashtable current
            ? string.Join(";", current.Cast<DictionaryEntry>().OrderBy(e => Convert.ToInt32(e.Key))
                .Select(e => $"{e.Key}:{(retained.Entries.TryGetValue(e.Key, out var old) ? (ReferenceEquals(old.Value, e.Value) ? "same" : "replaced") : "new")}"))
            : "null";
        string RetainedEntries() => string.Join(";", retained.Entries.OrderBy(e => Convert.ToInt32(e.Key))
            .Select(e => $"{e.Key}={e.Value.Before} => {ValueText(side, e.Value.Value)}"));
        string RetainedRefs() => string.Join(";", retained.Entries.OrderBy(e => Convert.ToInt32(e.Key))
            .Select(e => $"{e.Key}:{(retained.Table is Hashtable old && old.ContainsKey(e.Key) ? (ReferenceEquals(e.Value.Value, old[e.Key]) ? "same" : "changed") : "removed")}"));
        return $"{MatValDifferentialTests.Storage(side, target)} refs real={Ref(retained.Real, real)} imag={Ref(retained.Imag, imag)} table={Ref(retained.Table, table)}" +
            $" retained-real={ArrayText(side, retained.RealArray)} retained-imag={ArrayText(side, retained.ImagArray)}" +
            $" retained-table-snapshot=[{RetainedEntries()}] retained-pointer-refs=[{RetainedRefs()}] current-table=[{Table(table)}] current-pointer-refs=[{CurrentPointerRefs()}]";
    }

    private static string Render(Side side, object? value) => value == null ? "null" : side.Show(value);

    private static void Compare(string caseId, string accessor, int token, object qTarget, object mTarget, object?[] qArgs, object?[] mArgs)
    {
        var qm = Method(Side.Quire, accessor); var mm = Method(Side.Mathcad, accessor);
        Assert.Equal(token, mm.MetadataToken);
        var qr = Keep(Side.Quire, qTarget); var mr = Keep(Side.Mathcad, mTarget);
        MethodComparison.Compare(caseId, Contract, qm, qTarget, qArgs, mm, mTarget, mArgs,
            value => Render(Side.Quire, value), value => Render(Side.Mathcad, value),
            target => State(Side.Quire, qr, target), target => State(Side.Mathcad, mr, target));
    }

    private static void CompareBoth(string caseId, string accessor, int token, int flags,
        Action<Side, object>? setup = null, int rows = 2, int cols = 1)
    {
        var q = Matrix(Side.Quire, flags, rows, cols); var m = Matrix(Side.Mathcad, flags, rows, cols);
        setup?.Invoke(Side.Quire, q); setup?.Invoke(Side.Mathcad, m);
        Compare(caseId, accessor, token, q, m, [], []);
    }

    private static void CompareSetter(string caseId, string accessor, int token, int initial, bool value)
    {
        var q = Matrix(Side.Quire, initial); var m = Matrix(Side.Mathcad, initial);
        Compare(caseId, accessor, token, q, m, [value], [value]);
    }

    private static void SetLive(Side side, object matrix, int row, object value)
    {
        int index = (int)Call(matrix, "Index", row, 0)!;
        var real = (double[])Get(matrix, "m_rarr")!;
        real[index] = (double)Get(Get(value, "Data")!, "rp")!;
        Call(matrix, "set_ptrs_2", row, 0, value);
    }

    private static void MalformedPointerMap(Side side, object matrix)
    {
        Set(matrix, "m_rarr", new[] { 3.0 });
        Set(matrix, "m_iarr", new double[1]);
        Set(matrix, "m_parr", new Hashtable { [1] = side.Num(4, 0, "L") });
    }

    [SkippableFact]
    public void Range_and_integral_bits_preserve_other_flags_and_getters_follow_vector_gate()
    {
        Runtime.Sync();
        const int setRange = 0x06000465, setIntegral = 0x06000467;
        const int getRange = 0x06000464, getIntegral = 0x06000466;
        const int unrelated = unchecked((int)0x80000000);

        CompareSetter("set IsRange true preserves unrelated flags", "set_IsRange", setRange, unrelated, true);
        CompareSetter("set IsRange false preserves Integral and unrelated flags", "set_IsRange", setRange, unrelated | Integral | JoeyBit, false);
        CompareSetter("set IsIntegral true preserves JoeyBit and unrelated flags", "set_IsIntegral", setIntegral, unrelated | JoeyBit, true);
        CompareSetter("set IsIntegral false preserves JoeyBit and unrelated flags", "set_IsIntegral", setIntegral, unrelated | Integral | JoeyBit, false);

        CompareBoth("IsRange true on real vector", "get_IsRange", getRange, JoeyBit);
        CompareBoth("IsIntegral true on real vector", "get_IsIntegral", getIntegral, Integral);
        CompareBoth("IsRange stored bit gates nonvector false", "get_IsRange", getRange, JoeyBit, rows: 2, cols: 2);
        CompareBoth("IsIntegral stored bit gates nonvector false", "get_IsIntegral", getIntegral, Integral, rows: 2, cols: 2);
        CompareBoth("IsRange absent bit short-circuits malformed pointer map", "get_IsRange", getRange, 0, MalformedPointerMap);
        CompareBoth("IsIntegral absent bit short-circuits malformed pointer map", "get_IsIntegral", getIntegral, 0, MalformedPointerMap);
        CompareBoth("IsRange present bit reaches malformed pointer index", "get_IsRange", getRange, JoeyBit, MalformedPointerMap);
        CompareBoth("IsIntegral present bit reaches malformed pointer index", "get_IsIntegral", getIntegral, Integral, MalformedPointerMap);

        CompareBoth("IsRange vector gate clears all-zero imaginary storage", "get_IsRange", getRange, JoeyBit, (side, matrix) =>
        {
            var imaginary = (double[])Get(matrix, "m_iarr")!;
            Array.Clear(imaginary);
        });
        CompareBoth("IsIntegral vector gate canonicalizes full pointer coverage", "get_IsIntegral", getIntegral, Integral, (side, matrix) =>
        {
            for (int row = 0; row < 2; row++) SetLive(side, matrix, row, side.Num(row + 1, 0, "L"));
        });
        CompareBoth("IsRange vector gate preserves nonzero imaginary storage", "get_IsRange", getRange, JoeyBit, (side, matrix) =>
        {
            ((double[])Get(matrix, "m_iarr")!)[1] = 0.5;
        });
        CompareBoth("IsIntegral vector gate preserves nonzero imaginary storage", "get_IsIntegral", getIntegral, Integral, (side, matrix) =>
        {
            ((double[])Get(matrix, "m_iarr")!)[1] = 0.5;
        });
    }
}
