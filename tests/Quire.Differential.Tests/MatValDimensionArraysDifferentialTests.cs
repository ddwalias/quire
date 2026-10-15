using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValDimensionArraysDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const int ColDimArrayToken = 0x06000438;
    private const int RowDimArrayToken = 0x06000439;
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private static readonly System.Type QMat = typeof(global::Quire.Mcd.MatVal);

    private sealed record Snapshot(object Matrix, object? Dimension, object? Table, object? Real, object? Imag,
        object? Flags, IReadOnlyDictionary<int, object?> Entries, IReadOnlyDictionary<int, object?> PointerDimensions,
        IReadOnlyList<(object Matrix, Snapshot Before)> Children);

    private static System.Type MapType(Side side, System.Type type) =>
        side == Side.Quire || type != QMat ? type : side.T("Mcd.MatVal");

    private static object Matrix(Side side, int rows = 2, int cols = 2, int stride = 3,
        int offset = 1, string dimension = "L", int extra = 16)
    {
        var real = new double[offset + stride * cols + extra];
        var imag = new double[offset + stride * cols + extra];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
        {
            real[offset + row + stride * col] = 10 * col + row + 1;
            imag[offset + row + stride * col] = -10 * col - row - 1;
        }
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static int Index(object matrix, int row, int col) => (int)Call(matrix, "Index", row, col)!;

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static object Number(Side side, string dimension, double real = 2, double imag = 0) =>
        side.Num(real, imag, dimension);

    private static object NestedStringMatrix(Side side)
    {
        var child = Matrix(side, 1, 1, 2, 1, "L");
        var text = side.Str("nested");
        Set(text, "m_dim", side.D("L"));
        Pointer(side, child, 0, 0, text);
        return child;
    }

    private static Snapshot Capture(object matrix)
    {
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        Snapshot CaptureNode(object value)
        {
            var table = Get(value, "m_parr") as Hashtable;
            var entries = table?.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
                ?? new Dictionary<int, object?>();
            var pointerDimensions = entries.Where(pair => pair.Value?.GetType().Name == "NumVal")
                .ToDictionary(pair => pair.Key, pair => Get(pair.Value!, "m_dim"));
            var children = new List<(object Matrix, Snapshot Before)>();
            if (visited.Add(value) && table != null)
                foreach (var child in entries.Values.Where(item => item?.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true))
                    if (child != null) children.Add((child, CaptureNode(child)));
            return new(value, Get(value, "m_dim"), table, Get(value, "m_rarr"), Get(value, "m_iarr"),
                Get(Get(value, "Data")!, "m_flags"), entries, pointerDimensions, children);
        }
        return CaptureNode(matrix);
    }

    private static string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";

    private static string ArrayContents(object? value, Side side) => value is double[] array
        ? $"[{string.Join(",", array.Select(item => side.Show(item)))}]"
        : value == null ? "null" : side.Show(value);

    private static string PointerValue(Side side, int key, object? value, Snapshot? before = null)
    {
        if (value == null) return "null";
        string identity = before != null && before.Entries.TryGetValue(key, out var old) && ReferenceEquals(old, value) ? "retained" : "object";
        if (value.GetType().Name == "NumVal")
        {
            object? currentDim = Get(value, "m_dim");
            string dimRef = before != null && before.PointerDimensions.TryGetValue(key, out var oldDim) && ReferenceEquals(oldDim, currentDim)
                ? "same-dim-ref" : "changed-dim-ref";
            return $"NumVal:{identity}:real={Get(value, "Real")}:imag={Get(value, "Imag")}:dim={side.DimText(currentDim)}:{dimRef}";
        }
        return $"{value.GetType().Name}:{identity}:{Text(value)}";
    }

    private static string State(Side side, Snapshot before, object matrix)
    {
        var currentTable = Get(matrix, "m_parr") as Hashtable;
        string Entries(IEnumerable<DictionaryEntry> entries) => string.Join(";", entries.OrderBy(item => (int)item.Key)
            .Select(item => $"{item.Key}={PointerValue(side, (int)item.Key, item.Value, before)}"));
        string children = string.Join(" | ", before.Children.Select((child, index) =>
            $"child[{index}] source-same={ReferenceEquals(child.Matrix, child.Before.Matrix)} " +
            State(side, child.Before, child.Matrix)));
        string flagsRef = Equals(before.Flags, Get(Get(matrix, "Data")!, "m_flags")) ? "same" : "changed";
        return $"{MatValDifferentialTests.Storage(side, matrix)} dim-ref={Ref(before.Dimension, Get(matrix, "m_dim"))}" +
            $" flags-ref={flagsRef}" +
            $" table-ref={Ref(before.Table, currentTable)} real-ref={Ref(before.Real, Get(matrix, "m_rarr"))}" +
            $" imag-ref={Ref(before.Imag, Get(matrix, "m_iarr"))}" +
            $" real-contents={ArrayContents(Get(matrix, "m_rarr"), side)} imag-contents={ArrayContents(Get(matrix, "m_iarr"), side)}" +
            $" retained-real-alias={ArrayContents(before.Real, side)} retained-imag-alias={ArrayContents(before.Imag, side)}" +
            $" table-before=[{Entries(before.Entries.Select(pair => new DictionaryEntry(pair.Key, pair.Value)))}]" +
            $" table-after=[{(currentTable == null ? "null" : Entries(currentTable.Cast<DictionaryEntry>()))}]" +
            $" retained-table-alias-after=[{(before.Table is Hashtable oldTable ? Entries(oldTable.Cast<DictionaryEntry>()) : "null")}]" +
            $" nested=[{children}]";
    }

    private static string ShowResult(Side side, object? value, Snapshot before)
    {
        if (value == null) return "null";
        if (value is not Array dimensions) return Text(value);
        IEnumerable<(string Name, object? Dimension)> Aliases(Snapshot snapshot, string prefix)
        {
            yield return ($"{prefix}matrix-dim", snapshot.Dimension);
            foreach (var pair in snapshot.PointerDimensions) yield return ($"{prefix}pointer-dim@{pair.Key}", pair.Value);
            for (int i = 0; i < snapshot.Children.Count; i++)
                foreach (var childAlias in Aliases(snapshot.Children[i].Before, $"{prefix}child[{i}]."))
                    yield return childAlias;
        }
        var allAliases = Aliases(before, "").ToArray();
        var values = dimensions.Cast<object?>().Select((dimension, index) =>
        {
            string aliases = string.Join(",", allAliases.Where(alias => ReferenceEquals(alias.Dimension, dimension)).Select(alias => alias.Name));
            return $"{index}:{side.DimText(dimension)}:{(aliases.Length == 0 ? "other" : aliases)}";
        });
        return $"{value.GetType().Name}[{string.Join(";", values)}]";
    }

    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}(int)");

    private static void Compare(string caseId, string name, int token, object q, object m, int index)
    {
        var qBefore = Capture(q); var mBefore = Capture(m);
        var qMethod = Method(Side.Quire, name); var mMethod = Method(Side.Mathcad, name);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, q, [index], mMethod, m, [index],
            result => ShowResult(Side.Quire, result, qBefore), result => ShowResult(Side.Mathcad, result, mBefore),
            target => State(Side.Quire, qBefore, target), target => State(Side.Mathcad, mBefore, target));
    }

    private static object NonScalar(Side side) => Number(side, "function_dim");

    private static void MalformLivePointer(Side side, object matrix, int row, int col, string kind)
    {
        int index = Index(matrix, row, col);
        var table = (Hashtable)Get(matrix, "m_parr")!;
        if (kind == "missing") table.Remove(index);
        else if (kind == "null-value") table[index] = null;
        else if (kind == "null-table") Set(matrix, "m_parr", null);
        else if (kind == "non-DimVal") table[index] = new object();
    }

    [SkippableFact]
    public void Direct_ColDimArray_matches_pinned_IL_offset_stride_and_pointer_paths()
    {
        Runtime.Sync();
        var qDense = Matrix(Side.Quire, 3, 2, 5, 2, "L"); var mDense = Matrix(Side.Mathcad, 3, 2, 5, 2, "L");
        Compare("ColDimArray dense numeric slots with padded stride and nonzero offset", "ColDimArray", ColDimArrayToken, qDense, mDense, 1);

        var qLive = Matrix(Side.Quire, 3, 2, 5, 2, "identity"); var mLive = Matrix(Side.Mathcad, 3, 2, 5, 2, "identity");
        foreach (var sideMatrix in new[] { (Side.Quire, qLive), (Side.Mathcad, mLive) })
        {
            Pointer(sideMatrix.Item1, sideMatrix.Item2, 0, 1, Number(sideMatrix.Item1, "T"));
            Pointer(sideMatrix.Item1, sideMatrix.Item2, 1, 1, Number(sideMatrix.Item1, "L"));
        }
        Compare("ColDimArray live NumVal dimensions at logical column with padding", "ColDimArray", ColDimArrayToken, qLive, mLive, 1);

        var qNested = Matrix(Side.Quire, 2, 2, 4, 2); var mNested = Matrix(Side.Mathcad, 2, 2, 4, 2);
        Pointer(Side.Quire, qNested, 1, 0, NestedStringMatrix(Side.Quire));
        Pointer(Side.Mathcad, mNested, 1, 0, NestedStringMatrix(Side.Mathcad));
        Compare("ColDimArray live nested MatVal canonicalizes child and retains child state", "ColDimArray", ColDimArrayToken, qNested, mNested, 0);

        var qStale = Matrix(Side.Quire, 2, 2, 4, 2); var mStale = Matrix(Side.Mathcad, 2, 2, 4, 2);
        Pointer(Side.Quire, qStale, 0, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, mStale, 0, 0, Number(Side.Mathcad, "T"));
        ((double[])Get(qStale, "m_rarr")!)[Index(qStale, 0, 0)] = 7;
        ((double[])Get(mStale, "m_rarr")!)[Index(mStale, 0, 0)] = 7;
        Compare("ColDimArray ignores stale pointer in table", "ColDimArray", ColDimArrayToken, qStale, mStale, 0);

        var qNonScalar = Matrix(Side.Quire); var mNonScalar = Matrix(Side.Mathcad);
        Pointer(Side.Quire, qNonScalar, 0, 0, NonScalar(Side.Quire)); Pointer(Side.Mathcad, mNonScalar, 0, 0, NonScalar(Side.Mathcad));
        Compare("ColDimArray returns null immediately for non-scalar live pointer", "ColDimArray", ColDimArrayToken, qNonScalar, mNonScalar, 0);

        foreach (string malformed in new[] { "missing", "null-value", "null-table", "non-DimVal" })
        {
            var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
            Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
            MalformLivePointer(Side.Quire, q, 0, 0, malformed); MalformLivePointer(Side.Mathcad, m, 0, 0, malformed);
            Compare("ColDimArray live pointer " + malformed + " exception", "ColDimArray", ColDimArrayToken, q, m, 0);
        }

        var qZero = Matrix(Side.Quire, 0, 2, 3, 1); var mZero = Matrix(Side.Mathcad, 0, 2, 3, 1);
        Compare("ColDimArray zero rows returns empty without reading storage", "ColDimArray", ColDimArrayToken, qZero, mZero, 1);
        var qOut = Matrix(Side.Quire, 2, 1, 3, 1); var mOut = Matrix(Side.Mathcad, 2, 1, 3, 1);
        Compare("ColDimArray physically safe column beyond logical shape follows offset arithmetic", "ColDimArray", ColDimArrayToken, qOut, mOut, 2);
    }

    [SkippableFact]
    public void Direct_RowDimArray_matches_pinned_IL_offset_stride_and_pointer_paths()
    {
        Runtime.Sync();
        var qDense = Matrix(Side.Quire, 2, 3, 4, 2, "L"); var mDense = Matrix(Side.Mathcad, 2, 3, 4, 2, "L");
        Compare("RowDimArray dense numeric slots with padded stride and nonzero offset", "RowDimArray", RowDimArrayToken, qDense, mDense, 1);

        var qLive = Matrix(Side.Quire, 2, 3, 4, 2, "identity"); var mLive = Matrix(Side.Mathcad, 2, 3, 4, 2, "identity");
        foreach (var sideMatrix in new[] { (Side.Quire, qLive), (Side.Mathcad, mLive) })
        {
            Pointer(sideMatrix.Item1, sideMatrix.Item2, 1, 0, Number(sideMatrix.Item1, "T"));
            Pointer(sideMatrix.Item1, sideMatrix.Item2, 1, 2, Number(sideMatrix.Item1, "L"));
        }
        Compare("RowDimArray live NumVal dimensions with padded stride", "RowDimArray", RowDimArrayToken, qLive, mLive, 1);

        var qNested = Matrix(Side.Quire, 2, 2, 4, 2); var mNested = Matrix(Side.Mathcad, 2, 2, 4, 2);
        Pointer(Side.Quire, qNested, 0, 1, NestedStringMatrix(Side.Quire));
        Pointer(Side.Mathcad, mNested, 0, 1, NestedStringMatrix(Side.Mathcad));
        Compare("RowDimArray live nested MatVal canonicalizes child and retains child state", "RowDimArray", RowDimArrayToken, qNested, mNested, 0);

        var qStale = Matrix(Side.Quire, 2, 3, 4, 2); var mStale = Matrix(Side.Mathcad, 2, 3, 4, 2);
        Pointer(Side.Quire, qStale, 0, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, mStale, 0, 0, Number(Side.Mathcad, "T"));
        ((double[])Get(qStale, "m_rarr")!)[Index(qStale, 0, 0)] = 7;
        ((double[])Get(mStale, "m_rarr")!)[Index(mStale, 0, 0)] = 7;
        Compare("RowDimArray ignores stale pointer in table", "RowDimArray", RowDimArrayToken, qStale, mStale, 0);

        var qNonScalar = Matrix(Side.Quire); var mNonScalar = Matrix(Side.Mathcad);
        Pointer(Side.Quire, qNonScalar, 0, 0, NonScalar(Side.Quire)); Pointer(Side.Mathcad, mNonScalar, 0, 0, NonScalar(Side.Mathcad));
        Compare("RowDimArray returns null immediately for non-scalar live pointer", "RowDimArray", RowDimArrayToken, qNonScalar, mNonScalar, 0);

        foreach (string malformed in new[] { "missing", "null-value", "null-table", "non-DimVal" })
        {
            var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
            Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
            MalformLivePointer(Side.Quire, q, 0, 0, malformed); MalformLivePointer(Side.Mathcad, m, 0, 0, malformed);
            Compare("RowDimArray live pointer " + malformed + " exception", "RowDimArray", RowDimArrayToken, q, m, 0);
        }

        var qZero = Matrix(Side.Quire, 2, 0, 3, 1); var mZero = Matrix(Side.Mathcad, 2, 0, 3, 1);
        Compare("RowDimArray zero columns returns empty without reading storage", "RowDimArray", RowDimArrayToken, qZero, mZero, 1);
        var qOut = Matrix(Side.Quire, 2, 2, 3, 1); var mOut = Matrix(Side.Mathcad, 2, 2, 3, 1);
        Compare("RowDimArray physically safe row beyond logical shape follows offset arithmetic", "RowDimArray", RowDimArrayToken, qOut, mOut, 2);
    }
}
