using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValMatrixDimensionDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const int Token = 0x06000454;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Getter(Side side)
    {
        var getter = side.T("Mcd.MatVal").GetMethod("get_MatrixDimension", DeclaredInstance, null, Type.EmptyTypes, null)
            ?? throw new HarnessException($"Missing {side.Name} MatVal.get_MatrixDimension");
        if (side == Side.Mathcad) Assert.Equal(Token, getter.MetadataToken);
        return getter;
    }

    private static object Matrix(Side side, int rows, int cols, string? dimension = "L")
    {
        int stride = Math.Max(rows, 1);
        var matrix = New(side.T("Mcd.MatVal"), new double[stride * cols], new double[stride * cols],
            rows, cols, stride, new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Set(matrix, "m_dim", dimension is null ? null : side.D(dimension));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private sealed record Before(object? Real, object? Imag, object? Table, object? Dimension,
        Dictionary<int, object?> Pointers);

    private static Before Capture(object target)
    {
        var table = Get(target, "m_parr") as Hashtable;
        var pointers = table is null
            ? new Dictionary<int, object?>()
            : table.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value);
        return new Before(Get(target, "m_rarr"), Get(target, "m_iarr"), Get(target, "m_parr"), Get(target, "m_dim"), pointers);
    }

    private static string State(Side side, object target, Before before)
    {
        string Alias(object? original, object? current) => ReferenceEquals(original, current) ? "same" : "changed";
        var now = Get(target, "m_parr") as Hashtable;
        string entries = string.Join(";", before.Pointers.OrderBy(pair => pair.Key).Select(pair =>
            $"{pair.Key}:{(now is not null && now.ContainsKey(pair.Key) ? Alias(pair.Value, now[pair.Key]) : "removed")}"));
        var oldTable = before.Table as Hashtable;
        string oldEntries = oldTable is null ? "null" : string.Join(";", oldTable.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
            $"{entry.Key}:{side.Show(entry.Value)}:{(before.Pointers.TryGetValue((int)entry.Key, out var prior) ? Alias(prior, entry.Value) : "added")}"));
        return $"{MatValDifferentialTests.Storage(side, target)} aliases(real={Alias(before.Real, Get(target, "m_rarr"))}," +
            $"imag={Alias(before.Imag, Get(target, "m_iarr"))},table={Alias(before.Table, Get(target, "m_parr"))}," +
            $"dim={Alias(before.Dimension, Get(target, "m_dim"))}) prior-pointers[{entries}] old-table-alias[{oldEntries}]";
    }

    private static object? PointerDimension(object matrix, int row, int col)
    {
        int index = (int)Call(matrix, "Index", row, col)!;
        var table = Get(matrix, "m_parr") as Hashtable;
        if (table != null && table.ContainsKey(index) && table[index] is { } pointer && pointer.GetType().Name == "NumVal")
            return MaybeGet(pointer, "m_dim");
        return Get(matrix, "m_dim");
    }

    private static object? MaybeGet(object? target, string member)
    {
        if (target == null) return null;
        try { return Get(target, member); }
        catch (HarnessException) { return null; }
    }

    private static string AxisReferenceChecks(Side side, object matrix, object result)
    {
        string Same(object? a, object? b) => ReferenceEquals(a, b).ToString();
        string List(Array? values, Func<int, object?> expected)
        {
            if (values == null) return "missing";
            return "[" + string.Join(",", Enumerable.Range(0, values.Length).Select(i => Same(values.GetValue(i), expected(i)))) + "]";
        }

        string kind = result.GetType().Name;
        if (kind == "c_dim")
            return $" c-dim-source-row0-dim-refs={List(MaybeGet(result, "m_coldim") as Array, col => PointerDimension(matrix, 0, col))}";
        if (kind == "r_dim")
            return $" r-dim-source-col0-dim-refs={List(MaybeGet(result, "m_rowdim") as Array, row => PointerDimension(matrix, row, 0))}";
        if (kind == "rc_dim")
        {
            var rowRefs = List(MaybeGet(result, "m_rowdim") as Array, row => PointerDimension(matrix, row, 0));
            var colDims = MaybeGet(result, "m_coldim") as Array;
            var identity = Get(side.Dim, "identity");
            var firstBase = PointerDimension(matrix, 0, 0);
            var firstOther = PointerDimension(matrix, 0, 1);
            var secondBase = PointerDimension(matrix, 1, 0);
            var secondOther = PointerDimension(matrix, 1, 1);
            object? firstQuotient = firstBase == null || firstOther == null ? null : Call(side.Dim, "Div", firstOther, firstBase);
            object? secondQuotient = secondBase == null || secondOther == null ? null : Call(side.Dim, "Div", secondOther, secondBase);
            object? firstFactor = colDims is { Length: > 1 } ? colDims.GetValue(1) : null;
            return $" rc-dim-source-col0-dim-refs={rowRefs}" +
                $" rc-dim-factor0-is-identity={Same(colDims?.GetValue(0), identity)}" +
                $" rc-dim-factor1-is-first-row-div={Same(firstFactor, firstQuotient)}" +
                $" rc-dim-factor1-is-second-row-div={Same(firstFactor, secondQuotient)}";
        }
        return "";
    }

    private static string Result(Side side, object target, object? value)
    {
        if (value is null) return "null";
        object? targetDimension = Get(target, "m_dim");
        object? uniformDimension = Get(value, "UniformDim");
        object? mixedDimension = Get(side.Dim, "mixed_matrix_dim");
        string crazyMatrixIdentity = value.GetType().Name == "crazy_dim"
            ? ReferenceEquals(Get(value, "m_matrix"), target).ToString()
            : "n/a";
        return MatDimDifferentialTests.DescribeDimension(side, value) +
            $" result-is-target-dimension {ReferenceEquals(value, targetDimension)}" +
            $" uniform-is-target-dimension {ReferenceEquals(uniformDimension, targetDimension)}" +
            $" uniform-is-mixed-singleton {ReferenceEquals(uniformDimension, mixedDimension)}" +
            $" crazy-matrix-is-target {crazyMatrixIdentity}" + AxisReferenceChecks(side, target, value);
    }

    private static void AssertAllTrue(string rendered, string tag)
    {
        int start = rendered.IndexOf(tag, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing identity observation '{tag}' in: {rendered}");
        start += tag.Length;
        int end = rendered.IndexOf(' ', start);
        string value = rendered[start..(end < 0 ? rendered.Length : end)];
        Assert.All(value.Trim('[', ']').Split(','), item => Assert.True(bool.Parse(item)));
    }

    private static void AssertAxisIdentityTags(string rendered)
    {
        if (rendered.Contains(" c-dim-source-row0-dim-refs=", StringComparison.Ordinal))
            AssertAllTrue(rendered, " c-dim-source-row0-dim-refs=");
        if (rendered.Contains(" r-dim-source-col0-dim-refs=", StringComparison.Ordinal))
            AssertAllTrue(rendered, " r-dim-source-col0-dim-refs=");
        if (rendered.Contains(" rc-dim-source-col0-dim-refs=", StringComparison.Ordinal))
        {
            AssertAllTrue(rendered, " rc-dim-source-col0-dim-refs=");
            Assert.Contains("rc-dim-factor0-is-identity=True", rendered);
            Assert.Contains("rc-dim-factor1-is-first-row-div=True", rendered);
            Assert.Contains("rc-dim-factor1-is-second-row-div=True", rendered);
        }
    }

    private static void Compare(string caseId, Action<Side, object> setup, int rows, int cols, string? dimension = "L")
    {
        object q = Matrix(Side.Quire, rows, cols, dimension);
        object m = Matrix(Side.Mathcad, rows, cols, dimension);
        setup(Side.Quire, q);
        setup(Side.Mathcad, m);
        var qBefore = Capture(q);
        var mBefore = Capture(m);
        string? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, Getter(Side.Quire), q, [], Getter(Side.Mathcad), m, [],
            result => qResult = Result(Side.Quire, q, result), result => mResult = Result(Side.Mathcad, m, result),
            target => State(Side.Quire, target, qBefore), target => State(Side.Mathcad, target, mBefore));
        AssertAxisIdentityTags(qResult!);
        AssertAxisIdentityTags(mResult!);
    }

    [SkippableFact]
    public void MatrixDimension_dispatches_by_uniformity_and_vector_shape()
    {
        Runtime.Sync();
        Compare("MatrixDimension: uniform rectangular matrix", (_, _) => { }, 2, 3);
        Compare("MatrixDimension: uniform column vector", (_, _) => { }, 3, 1, "poly");
        Compare("MatrixDimension: uniform row vector", (_, _) => { }, 1, 3, "identity");
        Compare("MatrixDimension: uniform zero-area matrix", (_, _) => { }, 0, 3, null);
        Compare("MatrixDimension: nonuniform column dimensions", (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(2, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(3, 0, "T"));
            Pointer(side, matrix, 2, 0, side.Num(4, 0, "L"));
        }, 3, 1);
        Compare("MatrixDimension: nonuniform row dimensions", (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(2, 0, "L"));
            Pointer(side, matrix, 0, 1, side.Num(3, 0, "T"));
            Pointer(side, matrix, 0, 2, side.Num(4, 0, "L"));
        }, 1, 3);
        Compare("MatrixDimension: nonuniform rectangular pointer dimensions", (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(1, 0, "L"));
            Pointer(side, matrix, 0, 1, side.Num(2, 0, "T"));
            Pointer(side, matrix, 1, 0, side.Num(3, 0, "N"));
            Pointer(side, matrix, 1, 1, side.Num(4, 0, "v"));
        }, 2, 2);
        Compare("MatrixDimension: rectangular array with uniform columns", (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(1, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(2, 0, "L"));
            Pointer(side, matrix, 0, 1, side.Num(3, 0, "T"));
            Pointer(side, matrix, 1, 1, side.Num(4, 0, "T"));
        }, 2, 2);
        Compare("MatrixDimension: rectangular array with uniform rows", (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(1, 0, "L"));
            Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(3, 0, "T"));
            Pointer(side, matrix, 1, 1, side.Num(4, 0, "T"));
        }, 2, 2);
        Compare("MatrixDimension: rectangular scalar-multiple column factors", (side, matrix) =>
        {
            var l = side.D("L")!;
            var t = side.D("T")!;
            object l2 = Call(side.Dim, "Mult", l, l)!;
            object lt = Call(side.Dim, "Mult", l, t)!;
            Pointer(side, matrix, 0, 0, New(side.T("Mcd.NumVal"), 1.0, 0.0, l));
            Pointer(side, matrix, 1, 0, New(side.T("Mcd.NumVal"), 2.0, 0.0, t));
            Pointer(side, matrix, 0, 1, New(side.T("Mcd.NumVal"), 3.0, 0.0, l2));
            Pointer(side, matrix, 1, 1, New(side.T("Mcd.NumVal"), 4.0, 0.0, lt));
        }, 2, 2, "poly");
    }

    [SkippableFact]
    public void MatrixDimension_observes_pointer_cleanup_and_dimension_identity()
    {
        Runtime.Sync();
        Compare("MatrixDimension: stale pointer table", (side, matrix) =>
        {
            int index = (int)Call(matrix, "Index", 0, 0)!;
            ((Hashtable)Get(matrix, "m_parr")!)[index] = side.Num(8, 0, "T");
            ((double[])Get(matrix, "m_rarr")!)[index] = 8;
        }, 2, 2);
        Compare("MatrixDimension: all-live uniform pointer dimensions", (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "T"));
        }, 2, 2, "L");
        Compare("MatrixDimension: partial uniform pointer dimensions", (side, matrix) =>
        {
            Pointer(side, matrix, 0, 1, side.Num(7, 0, "T"));
        }, 2, 2, "L");
        Compare("MatrixDimension: non-NumVal pointer element", (side, matrix) =>
        {
            Pointer(side, matrix, 1, 0, "text");
        }, 2, 2);
        Compare("MatrixDimension: non-scalar nested matrix dimension falls back to crazy", (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), 2, 2);
            Set(nested, "m_dim", side.D("L"));
            Pointer(side, nested, 0, 0, side.Num(1, 0, "L"));
            Pointer(side, nested, 1, 1, side.Num(2, 0, "T"));
            Pointer(side, matrix, 0, 0, nested);
            Pointer(side, matrix, 1, 1, side.Num(3, 0, "T"));
        }, 2, 2);
    }
}
