using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValReinsertPtrsDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const int ReinsertPtrsToken = 0x06000443;
    private const int ReinsertPtrs1Token = 0x06000444;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}()");

    private static object Matrix(Side side, bool shared = false, string dimension = "L", bool imaginary = true,
        int rows = 2, int cols = 2, int stride = 3, int offset = 1)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        var imag = imaginary ? new double[length] : null;
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            real[offset + row + stride * col] = 1 + row + col;
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        Set(matrix, "Shared", shared);
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static void StalePointer(Side side, object matrix, int row = 0, int col = 0, object? value = null)
    {
        int index = (int)Call(matrix, "Index", row, col)!;
        ((Hashtable)Get(matrix, "m_parr")!)[index] = value ?? side.Num(9, 0, "T");
        ((double[])Get(matrix, "m_rarr")!)[index] = 9;
    }

    private sealed record PointerSnapshot(object? Value, object? Dimension, double? Real, double? Imag);

    private static IReadOnlyDictionary<int, PointerSnapshot> Snapshot(object? table) => table is Hashtable pointers
        ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry =>
        {
            object? value = entry.Value;
            if (value?.GetType().Name != "NumVal") return new PointerSnapshot(value, null, null, null);
            object? data = Get(value, "Data");
            return new PointerSnapshot(value, Get(value, "m_dim"),
                data == null ? null : (double)Get(data, "rp")!, data == null ? null : (double)Get(data, "ip")!);
        })
        : new Dictionary<int, PointerSnapshot>();

    private static string State(Side side, object? beforeTable, object? beforeReal, object? beforeImag,
        object? beforeDim, IReadOnlyDictionary<int, PointerSnapshot> beforePointers, object target)
    {
        object? afterTable = Get(target, "m_parr");
        string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";
        string ArrayContents(object? value) => value is double[] array
            ? $"[{string.Join(",", array.Select(item => side.Show(item)))}]"
            : value == null ? "null" : side.Show(value);
        string Describe(int index, object? value, PointerSnapshot? before)
        {
            if (value == null) return $"{index}:null";
            string identity = before != null && ReferenceEquals(before.Value, value) ? "same-object" : "new-object";
            if (value.GetType().Name != "NumVal") return $"{index}:{value.GetType().Name}:{identity}";
            object? data = Get(value, "Data");
            object? dimension = Get(value, "m_dim");
            string numeric = data == null ? "no-data" : $"rp={Get(data, "rp")},ip={Get(data, "ip")}";
            string dimRef = before == null ? "new-dim" : Ref(before.Dimension, dimension) + "-dim-ref";
            string valueState = before == null ? "new-values" :
                $"values={(before.Real == (double)Get(data!, "rp")! && before.Imag == (double)Get(data!, "ip")! ? "same" : "changed")}";
            return $"{index}:NumVal:{identity}:{numeric}:dim={side.DimText(dimension)}:{dimRef}:{valueState}";
        }
        string prior = string.Join(";", beforePointers.OrderBy(pair => pair.Key)
            .Select(pair => Describe(pair.Key, pair.Value.Value, pair.Value)));
        string retainedTable = beforeTable is Hashtable retained
            ? string.Join(";", retained.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
                Describe((int)entry.Key, entry.Value,
                    beforePointers.TryGetValue((int)entry.Key, out var before) ? before : null)))
            : "null";
        string after = afterTable is Hashtable table
            ? string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
                Describe((int)entry.Key, entry.Value,
                    beforePointers.TryGetValue((int)entry.Key, out var before) ? before : null)))
            : string.Join(";", beforePointers.Keys.Order().Select(index => $"{index}:absent"));
        return $"{MatValDifferentialTests.Storage(side, target)} matrix-dim-ref {Ref(beforeDim, Get(target, "m_dim"))}" +
            $" table-ref {Ref(beforeTable, afterTable)} prior-table-before [{prior}] retained-table-after [{retainedTable}] after-table [{after}]" +
            $" real-ref {Ref(beforeReal, Get(target, "m_rarr"))} imag-ref {Ref(beforeImag, Get(target, "m_iarr"))}" +
            $" retained-real-after {ArrayContents(beforeReal)} retained-imag-after {ArrayContents(beforeImag)}";
    }

    private static void Compare(string caseId, string methodName, int token, object qTarget, object mTarget)
    {
        var qMethod = Method(Side.Quire, methodName); var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(token, mMethod.MetadataToken);
        var qTable = Get(qTarget, "m_parr"); var mTable = Get(mTarget, "m_parr");
        var qReal = Get(qTarget, "m_rarr"); var mReal = Get(mTarget, "m_rarr");
        var qImag = Get(qTarget, "m_iarr"); var mImag = Get(mTarget, "m_iarr");
        var qDim = Get(qTarget, "m_dim"); var mDim = Get(mTarget, "m_dim");
        var qPointers = Snapshot(qTable); var mPointers = Snapshot(mTable);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [], mMethod, mTarget, [],
            quireState: target => State(Side.Quire, qTable, qReal, qImag, qDim, qPointers, target),
            originalState: target => State(Side.Mathcad, mTable, mReal, mImag, mDim, mPointers, target));
    }

    private static void CompareBoth(string caseId, string methodName, int token, Action<Side, object> setup,
        bool shared = false, string dimension = "L", bool imaginary = true,
        int rows = 2, int cols = 2, int stride = 3, int offset = 1)
    {
        var q = Matrix(Side.Quire, shared, dimension, imaginary, rows, cols, stride, offset);
        var m = Matrix(Side.Mathcad, shared, dimension, imaginary, rows, cols, stride, offset);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        Compare(caseId, methodName, token, q, m);
    }

    [SkippableFact]
    public void ReinsertPtrs_reinserts_live_entries_and_repackages_shared_storage()
    {
        Runtime.Sync();
        Assert.Equal(ReinsertPtrsToken, Method(Side.Mathcad, "ReinsertPtrs").MetadataToken);

        CompareBoth("ReinsertPtrs clears an empty table", "ReinsertPtrs", ReinsertPtrsToken, (_, _) => { });
        CompareBoth("ReinsertPtrs clears stale-only table entries", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) => StalePointer(side, matrix));
        CompareBoth("ReinsertPtrs reinserts compatible scalar pointer and preserves object identity", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 2, "L")));
        CompareBoth("ReinsertPtrs reinserts incompatible NumVal pointer", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 2, "T")));
        CompareBoth("ReinsertPtrs reinserts live nonnumeric pointer", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) => Pointer(side, matrix, 1, 0, "text"));
        CompareBoth("ReinsertPtrs preserves mcd_zero dimension on the same NumVal", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero")));
        CompareBoth("ReinsertPtrs reinserts complex pointer with imaginary backing", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) =>
            {
                ((double[])Get(matrix, "m_iarr")!)[0] = 6;
                Pointer(side, matrix, 0, 0, side.Num(4, 5, "L"));
            });
        CompareBoth("ReinsertPtrs remaps Shared padded pointer indices and packs imaginary storage", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) =>
            {
                ((double[])Get(matrix, "m_iarr")!)[(int)Call(matrix, "Index", 0, 1)!] = 6;
                Pointer(side, matrix, 0, 1, side.Num(4, 5, "T"));
                StalePointer(side, matrix, 1, 0);
            }, shared: true);
        CompareBoth("ReinsertPtrs exposes null-table enumeration exception", "ReinsertPtrs", ReinsertPtrsToken,
            (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("ReinsertPtrs exposes null-backing access exception after detaching table", "ReinsertPtrs", ReinsertPtrsToken,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(3, 0, "L"));
                Set(matrix, "m_rarr", null);
            }, imaginary: false);
        CompareBoth("ReinsertPtrs exposes Shared null-table exception after repacking", "ReinsertPtrs", ReinsertPtrsToken,
            (_, matrix) => Set(matrix, "m_parr", null), shared: true);
    }

    [SkippableFact]
    public void ReinsertPtrs1_converts_zero_dimensions_and_repackages_shared_storage()
    {
        Runtime.Sync();
        Assert.Equal(ReinsertPtrs1Token, Method(Side.Mathcad, "ReinsertPtrs1").MetadataToken);

        CompareBoth("ReinsertPtrs1 clears an empty table", "ReinsertPtrs1", ReinsertPtrs1Token, (_, _) => { });
        CompareBoth("ReinsertPtrs1 clears stale-only table entries", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) => StalePointer(side, matrix));
        CompareBoth("ReinsertPtrs1 mutates the same zero NumVal dimension to identity", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero")));
        CompareBoth("ReinsertPtrs1 preserves ordinary NumVal dimension and identity", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 2, "T")));
        CompareBoth("ReinsertPtrs1 reinserts live nonnumeric pointer", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) => Pointer(side, matrix, 1, 0, "text"));
        CompareBoth("ReinsertPtrs1 reinserts incompatible NumVal pointer", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 2, "T")), dimension: "L");
        CompareBoth("ReinsertPtrs1 remaps Shared zero pointer and mutates its dimension in place", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) =>
            {
                var zero = side.Num(0, 0, "mcd_zero");
                Pointer(side, matrix, 0, 1, zero);
                ((double[])Get(matrix, "m_iarr")!)[(int)Call(matrix, "Index", 0, 1)!] = 3;
            }, shared: true);
        CompareBoth("ReinsertPtrs1 remaps Shared nonnumeric and ordinary pointers", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 1, side.Num(4, 6, "T"));
                Pointer(side, matrix, 1, 0, "text");
                StalePointer(side, matrix, 1, 1);
            }, shared: true);
        CompareBoth("ReinsertPtrs1 keeps zero dimension on a stale entry", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) => StalePointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero")));
        CompareBoth("ReinsertPtrs1 exposes null-table enumeration exception", "ReinsertPtrs1", ReinsertPtrs1Token,
            (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("ReinsertPtrs1 exposes null-backing access exception after detaching table", "ReinsertPtrs1", ReinsertPtrs1Token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(3, 0, "L"));
                Set(matrix, "m_rarr", null);
            }, imaginary: false);
        CompareBoth("ReinsertPtrs1 exposes Shared null-table exception after repacking", "ReinsertPtrs1", ReinsertPtrs1Token,
            (_, matrix) => Set(matrix, "m_parr", null), shared: true);
    }
}
