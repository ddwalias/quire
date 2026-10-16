using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValAsRealVectorDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const int Token = 0x06000468;
    private const BindingFlags DeclaredStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private sealed record Retained(object? Real, object? Imag, object? Table, object? Dimension, double[]? RealArray,
        double[]? ImagArray, IReadOnlyDictionary<object, object?> Entries);

    private static MethodInfo Method(Side side) => side.T("Mcd.MatVal").GetMethod("asRealVector", DeclaredStatic,
        null, [typeof(object)], null) ?? throw new HarnessException($"Missing {side.Name} MatVal.asRealVector(object)");

    private static object Matrix(Side side, int rows = 2, int cols = 1, int stride = 3, int offset = 1,
        double[]? imaginary = null, double[]? real = null, Hashtable? pointers = null)
    {
        int length = offset + stride * cols;
        real ??= Enumerable.Range(0, length).Select(i => 10.0 + i).ToArray();
        imaginary ??= new double[length];
        var matrix = New(side.T("Mcd.MatVal"), real, imaginary, rows, cols, stride, pointers ?? new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static Retained Keep(object? input)
    {
        if (input?.GetType().Name != "MatVal") return new(null, null, null, null, null, null, new Dictionary<object, object?>());
        object? real = Get(input, "m_rarr"), imag = Get(input, "m_iarr"), table = Get(input, "m_parr");
        object? dimension = Get(input, "m_dim");
        var entries = table is Hashtable ht
            ? ht.Cast<DictionaryEntry>().ToDictionary(e => e.Key, e => e.Value)
            : new Dictionary<object, object?>();
        return new(real, imag, table, dimension, real as double[], imag as double[], entries);
    }

    private static string ArrayText(object? array) => array is double[] values
        ? "[" + string.Join(",", values.Select(Side.Bits)) + "]"
        : "null";

    private static string ValueText(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().Name == "MatVal") return MatValDifferentialTests.Storage(side, value);
        return side.Show(value);
    }

    private static string State(Side side, Retained old, object matrix)
    {
        object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr");
        string Ref(object? a, object? b) => ReferenceEquals(a, b) ? "same" : "changed";
        string Contents(object? value) => value is Hashtable ht
            ? string.Join(";", ht.Cast<DictionaryEntry>().OrderBy(e => Convert.ToString(e.Key), StringComparer.Ordinal)
                .Select(e => $"{e.Key}={ValueText(side, e.Value)}"))
            : "null";
        string OldContents() => old.Table is Hashtable ht
            ? string.Join(";", old.Entries.OrderBy(e => Convert.ToString(e.Key), StringComparer.Ordinal)
                .Select(e => $"{e.Key}={(ht.ContainsKey(e.Key) ? ValueText(side, ht[e.Key]) : "<removed>")}"))
            : "null";
        string OldEntryRefs() => string.Join(";", old.Entries.OrderBy(e => Convert.ToString(e.Key), StringComparer.Ordinal)
            .Select(e => $"{e.Key}:{(old.Table is Hashtable ht && ht.ContainsKey(e.Key) ? (ReferenceEquals(e.Value, ht[e.Key]) ? "same" : "changed") : "removed")}"));
        return $"{MatValDifferentialTests.Storage(side, matrix)} refs(real={Ref(old.Real, real)},imag={Ref(old.Imag, imag)},table={Ref(old.Table, table)},dimension={Ref(old.Dimension, Get(matrix, "m_dim"))})" +
            $" retained-real={ArrayText(old.RealArray)} retained-imag={ArrayText(old.ImagArray)} retained-table=[{OldContents()}]" +
            $" retained-entry-refs=[{OldEntryRefs()}] current-table=[{Contents(table)}]";
    }

    private static string Show(Side side, Retained retained, object? input, object? value)
    {
        if (value?.GetType().Name == "MatVal")
        {
            string relation = ReferenceEquals(input, value) ? "same-as-input" : "other-MatVal";
            return $"MatVal({relation}) {State(side, retained, value)}";
        }
        return side.Show(value);
    }

    private static void Compare(string caseId, object? qInput, object? mInput)
    {
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(Token, mMethod.MetadataToken);
        var qRetained = Keep(qInput); var mRetained = Keep(mInput);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qInput], mMethod, null, [mInput],
            value => Show(Side.Quire, qRetained, qInput, value),
            value => Show(Side.Mathcad, mRetained, mInput, value));
    }

    private static void CompareBoth(string caseId, Func<Side, object?> make)
    {
        Compare(caseId, make(Side.Quire), make(Side.Mathcad));
    }

    [SkippableFact]
    public void AsRealVector_returns_only_real_vector_matrices_and_preserves_or_observes_input_state()
    {
        Runtime.Sync();
        Compare("null input", null, null);
        Compare("non-MatVal string input", "not a matrix", "not a matrix");
        CompareBoth("real vector with null imaginary backing returns the identical matrix", side =>
        {
            var matrix = Matrix(side);
            Set(matrix, "m_iarr", null);
            return matrix;
        });
        CompareBoth("rectangular matrix returns null", side => Matrix(side, rows: 2, cols: 2));
        CompareBoth("complex vector with nonzero imaginary value returns null and retains data", side =>
        {
            int length = 1 + 3;
            var imag = new double[length]; imag[1] = 0.5;
            return Matrix(side, imaginary: imag);
        });
        CompareBoth("all-zero imaginary vector is normalized while returned by identity", side => Matrix(side));
        CompareBoth("full live pointer coverage canonicalizes before return", side =>
        {
            var matrix = Matrix(side);
            for (int row = 0; row < 2; row++)
            {
                int index = (int)Call(matrix, "Index", row, 0)!;
                ((double[])Get(matrix, "m_rarr")!)[index] = row + 1;
                Call(matrix, "set_ptrs_2", row, 0, side.Num(row + 1, 0, "L"));
            }
            return matrix;
        });
        CompareBoth("nonuniform live pointer still clears zero imaginary backing", side =>
        {
            var matrix = Matrix(side);
            int index = (int)Call(matrix, "Index", 0, 0)!;
            ((double[])Get(matrix, "m_rarr")!)[index] = 1;
            Call(matrix, "set_ptrs_2", 0, 0, "pointer");
            return matrix;
        });
        CompareBoth("live out-of-range pointer marker forwards index exception", side =>
        {
            var real = new[] { 1.0 };
            var imag = new[] { 0.0 };
            return Matrix(side, stride: 1, offset: 0, real: real, imaginary: imag,
                pointers: new Hashtable { [1] = side.Num(7, 0, "L") });
        });
    }
}
