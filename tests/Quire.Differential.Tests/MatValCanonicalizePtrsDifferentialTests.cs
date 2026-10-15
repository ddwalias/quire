using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValCanonicalizePtrsDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const int Token = 0x06000441;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side) =>
        side.T("Mcd.MatVal").GetMethod("CanonicalizePtrs", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.CanonicalizePtrs()");

    private static object Matrix(Side side, string dimension = "L", bool shared = false,
        int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = true)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        var imag = imaginary ? new double[length] : null;
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
        {
            int index = offset + row + stride * col;
            real[index] = row + col + 1;
        }
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
        string Describe(int index, object? value, PointerSnapshot? before)
        {
            if (value == null) return $"{index}:null";
            string identity = before != null && ReferenceEquals(before.Value, value) ? "same-object" : "new-object";
            if (value.GetType().Name != "NumVal") return $"{index}:{value.GetType().Name}:{identity}";
            object? data = Get(value, "Data");
            object? dimension = Get(value, "m_dim");
            string numeric = data == null ? "no-data" : $"rp={Get(data, "rp")},ip={Get(data, "ip")}";
            string dimRef = before == null ? "new-dim" : Ref(before.Dimension, dimension) + "-dim-ref";
            string numericRef = before == null ? "new-values" :
                $"values={(before.Real == (double)Get(data!, "rp")! && before.Imag == (double)Get(data!, "ip")! ? "same" : "changed")}";
            return $"{index}:NumVal:{identity}:{numeric}:dim={side.DimText(dimension)}:{dimRef}:{numericRef}";
        }
        string prior = string.Join(";", beforePointers.OrderBy(pair => pair.Key)
            .Select(pair => Describe(pair.Key, pair.Value.Value, pair.Value)));
        string after = afterTable is Hashtable table
            ? string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
                Describe((int)entry.Key, entry.Value,
                    beforePointers.TryGetValue((int)entry.Key, out var before) ? before : null)))
            : string.Join(";", beforePointers.Keys.Order().Select(index => $"{index}:absent"));
        return $"{MatValDifferentialTests.Storage(side, target)} matrix-dim-ref {Ref(beforeDim, Get(target, "m_dim"))}" +
            $" table-ref {Ref(beforeTable, afterTable)} prior-table [{prior}] after-table [{after}]" +
            $" real-ref {Ref(beforeReal, Get(target, "m_rarr"))} imag-ref {Ref(beforeImag, Get(target, "m_iarr"))}";
    }

    private static void Compare(string caseId, object qTarget, object mTarget)
    {
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(Token, mMethod.MetadataToken);
        var qTable = Get(qTarget, "m_parr"); var mTable = Get(mTarget, "m_parr");
        var qReal = Get(qTarget, "m_rarr"); var mReal = Get(mTarget, "m_rarr");
        var qImag = Get(qTarget, "m_iarr"); var mImag = Get(mTarget, "m_iarr");
        var qDim = Get(qTarget, "m_dim"); var mDim = Get(mTarget, "m_dim");
        var qPointers = Snapshot(qTable); var mPointers = Snapshot(mTable);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [], mMethod, mTarget, [],
            quireState: target => State(Side.Quire, qTable, qReal, qImag, qDim, qPointers, target),
            originalState: target => State(Side.Mathcad, mTable, mReal, mImag, mDim, mPointers, target));
    }

    private static void CompareBoth(string caseId, Action<Side, object> setup,
        string dimension = "L", bool shared = false, int rows = 2, int cols = 2,
        int stride = 3, int offset = 1, bool imaginary = true)
    {
        var q = Matrix(Side.Quire, dimension, shared, rows, cols, stride, offset, imaginary);
        var m = Matrix(Side.Mathcad, dimension, shared, rows, cols, stride, offset, imaginary);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        Compare(caseId, q, m);
    }

    [SkippableFact]
    public void CanonicalizePtrs_reconciles_dimensions_and_live_pointer_tables()
    {
        Runtime.Sync();
        Assert.Equal(Token, Method(Side.Mathcad).MetadataToken);

        CompareBoth("CanonicalizePtrs replaces string_dim and clears empty table",
            (side, matrix) => Set(matrix, "m_dim", side.D("string_dim")));
        CompareBoth("CanonicalizePtrs clears stale-only pointers",
            (side, matrix) => StalePointer(side, matrix));
        CompareBoth("CanonicalizePtrs keeps a compatible partial pointer when numeric backing establishes dimension",
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 2, "L")));
        CompareBoth("CanonicalizePtrs rejects a partial pointer with incompatible dimension",
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 0, "T")));
        CompareBoth("CanonicalizePtrs canonicalizes all compatible full-coverage pointers",
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "L"));
            });
        CompareBoth("CanonicalizePtrs leaves full pointer-only dimensions without a numeric dimension anchor",
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "T"));
            });
        CompareBoth("CanonicalizePtrs treats a full mixed zero and ordinary pointer set",
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
                Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
                Pointer(side, matrix, 1, 0, side.Num(3, 0, "L"));
                Pointer(side, matrix, 1, 1, side.Num(4, 0, "L"));
            });
        CompareBoth("CanonicalizePtrs reinserts full zero-dimension pointer coverage",
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(0, 0, "mcd_zero"));
            });
        CompareBoth("CanonicalizePtrs retains a live nonnumeric entry with compatible NumVal",
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(5, 0, "L"));
                Pointer(side, matrix, 0, 1, "text");
            });
        CompareBoth("CanonicalizePtrs retains incompatible nonnumeric-only live entries",
            (side, matrix) => Pointer(side, matrix, 1, 1, "text"));
        CompareBoth("CanonicalizePtrs repacks Shared padded storage for compatible partial pointer",
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(8, 0, "L")), shared: true);
        CompareBoth("CanonicalizePtrs preserves Shared storage when pointer dimension is incompatible",
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(8, 0, "T")), shared: true);
        CompareBoth("CanonicalizePtrs clears a zero-count table for zero logical rows",
            (_, _) => { }, rows: 0, cols: 2, stride: 3, offset: 1);
        CompareBoth("CanonicalizePtrs clears a zero-count table for zero logical columns",
            (_, _) => { }, rows: 2, cols: 0, stride: 3, offset: 0);
        CompareBoth("CanonicalizePtrs clears zero-count stale entries",
            (side, matrix) => StalePointer(side, matrix, 1, 1));
        CompareBoth("CanonicalizePtrs keeps a live zero pointer in partial coverage",
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero")));
        CompareBoth("CanonicalizePtrs exposes string-dimension mutation before null-table exception",
            (side, matrix) =>
            {
                Set(matrix, "m_dim", side.D("string_dim"));
                Set(matrix, "m_parr", null);
            });
        CompareBoth("CanonicalizePtrs preserves null-real-backing exception in DimActual",
            (_, matrix) => Set(matrix, "m_rarr", null));
    }
}
