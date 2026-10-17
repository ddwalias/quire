using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValRealClassifiersDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}()");

    private static object Matrix(Side side, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = true)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            real[offset + row + stride * col] = row + col + 1;
        double[]? imag = imaginary ? new double[length] : null;
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static void StalePointer(Side side, object matrix, int row = 0, int col = 0, object? value = null)
    {
        int index = (int)Call(matrix, "Index", row, col)!;
        ((Hashtable)Get(matrix, "m_parr")!)[index] = value ?? "stale";
        ((double[])Get(matrix, "m_rarr")!)[index] = 17;
    }

    private sealed record RealClassifiersPointerSnapshot(object? Value, object? Dimension, double? Real, double? Imag);

    private static IReadOnlyDictionary<int, RealClassifiersPointerSnapshot> Snapshot(object? table) => table is Hashtable pointers
        ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry =>
        {
            object? value = entry.Value;
            if (value?.GetType().Name != "NumVal") return new RealClassifiersPointerSnapshot(value, null, null, null);
            object? data = Get(value, "Data");
            return new RealClassifiersPointerSnapshot(value, Get(value, "m_dim"),
                data == null ? null : (double)Get(data, "rp")!, data == null ? null : (double)Get(data, "ip")!);
        })
        : new Dictionary<int, RealClassifiersPointerSnapshot>();

    private static string State(Side side, object? beforeTable, object? beforeReal, object? beforeImag,
        IReadOnlyDictionary<int, RealClassifiersPointerSnapshot> beforePointers, object target)
    {
        object? afterTable = Get(target, "m_parr");
        string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";
        string Describe(int index, object? value, RealClassifiersPointerSnapshot? before)
        {
            if (value == null) return $"{index}:null";
            string identity = before != null && ReferenceEquals(before.Value, value) ? "same-object" : "new-object";
            if (value.GetType().Name != "NumVal") return $"{index}:{value.GetType().Name}:{identity}";
            object? data = Get(value, "Data");
            object? dimension = Get(value, "m_dim");
            string scalar = data == null ? "no-data" : $"rp={Get(data, "rp")},ip={Get(data, "ip")}";
            string dimRef = before == null ? "new-dim" : Ref(before.Dimension, dimension) + "-dim-ref";
            string values = before == null ? "new-values" :
                $"values={(before.Real == (double)Get(data!, "rp")! && before.Imag == (double)Get(data!, "ip")! ? "same" : "changed")}";
            return $"{index}:NumVal:{identity}:{scalar}:dim={side.DimText(dimension)}:{dimRef}:{values}";
        }
        string tableAlias = string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair =>
            Describe(pair.Key, pair.Value.Value, pair.Value)));
        string pointerRefs = afterTable is Hashtable table
            ? string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
                Describe((int)entry.Key, entry.Value,
                    beforePointers.TryGetValue((int)entry.Key, out var before) ? before : null)))
            : string.Join(";", beforePointers.Keys.Order().Select(index => $"{index}:absent"));
        return $"{MatValDifferentialTests.Storage(side, target)} table-ref {Ref(beforeTable, afterTable)}" +
            $" prior-table [{tableAlias}]" +
            $" real-ref {Ref(beforeReal, Get(target, "m_rarr"))} imag-ref {Ref(beforeImag, Get(target, "m_iarr"))}" +
            $" pointer-refs [{pointerRefs}]";
    }

    private static void Compare(string caseId, string methodName, int originalToken, object qTarget, object mTarget)
    {
        var qMethod = Method(Side.Quire, methodName); var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(originalToken, mMethod.MetadataToken);
        var qTable = Get(qTarget, "m_parr"); var mTable = Get(mTarget, "m_parr");
        var qReal = Get(qTarget, "m_rarr"); var mReal = Get(mTarget, "m_rarr");
        var qImag = Get(qTarget, "m_iarr"); var mImag = Get(mTarget, "m_iarr");
        var qPointers = Snapshot(qTable); var mPointers = Snapshot(mTable);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [], mMethod, mTarget, [],
            quireState: target => State(Side.Quire, qTable, qReal, qImag, qPointers, target),
            originalState: target => State(Side.Mathcad, mTable, mReal, mImag, mPointers, target));
    }

    private static void CompareBoth(string caseId, string methodName, int token,
        Action<Side, object> setup, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = true)
    {
        var q = Matrix(Side.Quire, rows, cols, stride, offset, imaginary);
        var m = Matrix(Side.Mathcad, rows, cols, stride, offset, imaginary);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        Compare(caseId, methodName, token, q, m);
    }

    [SkippableFact]
    public void IsVector_uses_logical_column_count()
    {
        Runtime.Sync();
        const int token = 0x0600045b;
        CompareBoth("IsVector one logical column with padded offset", "get_IsVector", token, (_, _) => { }, rows: 3, cols: 1, stride: 5, offset: 2);
        CompareBoth("IsVector two logical columns", "get_IsVector", token, (_, _) => { }, rows: 1, cols: 2, stride: 3, offset: 1);
        CompareBoth("IsVector one row and one column", "get_IsVector", token, (_, _) => { }, rows: 1, cols: 1, stride: 2, offset: 1);
        CompareBoth("IsVector zero logical rows with one column", "get_IsVector", token, (_, _) => { }, rows: 0, cols: 1, stride: 3, offset: 1);
        CompareBoth("IsVector zero logical columns", "get_IsVector", token, (_, _) => { }, rows: 2, cols: 0, stride: 3, offset: 0);
        CompareBoth("IsVector ignores pointers and imaginary values", "get_IsVector", token, (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, "text");
            ((double[])Get(matrix, "m_iarr")!)[1] = 4;
        }, rows: 2, cols: 1);
    }

    [SkippableFact]
    public void IsScalarMatrix_checks_only_live_pointer_value_types()
    {
        Runtime.Sync();
        const int token = 0x0600045d;
        CompareBoth("IsScalarMatrix null table", "get_IsScalarMatrix", token, (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("IsScalarMatrix empty table", "get_IsScalarMatrix", token, (_, _) => { });
        CompareBoth("IsScalarMatrix accepts live NumVal pointer", "get_IsScalarMatrix", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(5, 3, "T")));
        CompareBoth("IsScalarMatrix rejects live string pointer", "get_IsScalarMatrix", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, "text"));
        CompareBoth("IsScalarMatrix ignores stale non-NumVal entry", "get_IsScalarMatrix", token,
            (side, matrix) => StalePointer(side, matrix));
        CompareBoth("IsScalarMatrix rejects live nested matrix", "get_IsScalarMatrix", token, (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), new double[] { 5 });
            Pointer(side, matrix, 0, 0, nested);
        });
        CompareBoth("IsScalarMatrix accepts live NumVal alongside stale non-NumVal", "get_IsScalarMatrix", token, (side, matrix) =>
        {
            StalePointer(side, matrix, 0, 0);
            Pointer(side, matrix, 1, 1, side.Num(7, 0, "L"));
        });
        CompareBoth("IsScalarMatrix preserves the original null-backing exception for a live pointer", "get_IsScalarMatrix", token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(7, 0, "L"));
                Set(matrix, "m_rarr", null);
            });
    }

    [SkippableFact]
    public void IsRealMatrix_checks_uniformity_and_all_imaginary_backing_storage()
    {
        Runtime.Sync();
        const int token = 0x0600045c;
        CompareBoth("IsRealMatrix without imaginary array or pointers", "get_IsRealMatrix", token,
            (_, matrix) => Set(matrix, "m_parr", null), imaginary: false);
        CompareBoth("IsRealMatrix clears zero imaginary backing", "get_IsRealMatrix", token,
            (_, _) => { });
        CompareBoth("IsRealMatrix rejects nonzero logical imaginary value", "get_IsRealMatrix", token, (_, matrix) =>
            ((double[])Get(matrix, "m_iarr")!)[(int)Call(matrix, "Index", 0, 0)!] = 2);
        CompareBoth("IsRealMatrix rejects nonzero padding imaginary value", "get_IsRealMatrix", token, (_, matrix) =>
            ((double[])Get(matrix, "m_iarr")!)[0] = 2);
        CompareBoth("IsRealMatrix retains NaN in padding imaginary storage", "get_IsRealMatrix", token, (_, matrix) =>
            ((double[])Get(matrix, "m_iarr")!)[0] = double.NaN);
        CompareBoth("IsRealMatrix canonicalizes partial compatible NumVal pointer", "get_IsRealMatrix", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "L")), imaginary: false);
        CompareBoth("IsRealMatrix reports false for incompatible live dimension", "get_IsRealMatrix", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "T")), imaginary: false);
        CompareBoth("IsRealMatrix reports false for non-NumVal pointer", "get_IsRealMatrix", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"), imaginary: false);
        CompareBoth("IsRealMatrix still clears all-zero imaginary backing when pointer uniformity fails", "get_IsRealMatrix", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"));
        CompareBoth("IsRealMatrix clears stale-only pointer table", "get_IsRealMatrix", token,
            (side, matrix) => StalePointer(side, matrix), imaginary: false);
        CompareBoth("IsRealMatrix canonicalizes full compatible pointers", "get_IsRealMatrix", token, (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "L"));
        }, imaginary: false);
        CompareBoth("IsRealMatrix preserves the original null-backing exception for a live pointer", "get_IsRealMatrix", token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(7, 0, "L"));
                Set(matrix, "m_rarr", null);
                Set(matrix, "m_iarr", null);
            });
    }

    [SkippableFact]
    public void IsRealMatrixMixed_checks_imaginary_values_and_live_pointer_kinds()
    {
        Runtime.Sync();
        const int token = 0x0600045e;
        CompareBoth("IsRealMatrixMixed no table and no imaginary array", "get_IsRealMatrixMixed", token,
            (_, matrix) => Set(matrix, "m_parr", null), imaginary: false);
        CompareBoth("IsRealMatrixMixed clears all-zero imaginary array", "get_IsRealMatrixMixed", token,
            (_, _) => { });
        CompareBoth("IsRealMatrixMixed rejects imaginary value in padding", "get_IsRealMatrixMixed", token, (_, matrix) =>
            ((double[])Get(matrix, "m_iarr")!)[0] = 1);
        CompareBoth("IsRealMatrixMixed rejects NaN imaginary padding", "get_IsRealMatrixMixed", token, (_, matrix) =>
            ((double[])Get(matrix, "m_iarr")!)[0] = double.NaN);
        CompareBoth("IsRealMatrixMixed accepts real-valued NumVal pointer", "get_IsRealMatrixMixed", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(3, 0, "T")), imaginary: false);
        CompareBoth("IsRealMatrixMixed rejects complex NumVal pointer", "get_IsRealMatrixMixed", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(3, 2, "T")), imaginary: false);
        CompareBoth("IsRealMatrixMixed rejects NaN imaginary NumVal pointer", "get_IsRealMatrixMixed", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(3, double.NaN, "T")), imaginary: false);
        CompareBoth("IsRealMatrixMixed rejects non-NumVal pointer", "get_IsRealMatrixMixed", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, "text"), imaginary: false);
        CompareBoth("IsRealMatrixMixed ignores stale non-NumVal entry without cleanup", "get_IsRealMatrixMixed", token,
            (side, matrix) => StalePointer(side, matrix), imaginary: false);
        CompareBoth("IsRealMatrixMixed rejects nested matrix pointer", "get_IsRealMatrixMixed", token, (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), new double[] { 5 });
            Pointer(side, matrix, 1, 1, nested);
        }, imaginary: false);
        CompareBoth("IsRealMatrixMixed short-circuits live pointer scan when imaginary backing is nonzero", "get_IsRealMatrixMixed", token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, "text");
                ((double[])Get(matrix, "m_iarr")!)[(int)Call(matrix, "Index", 0, 0)!] = 2;
            });
        CompareBoth("IsRealMatrixMixed preserves the original null-backing exception for a live pointer", "get_IsRealMatrixMixed", token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(7, 0, "L"));
                Set(matrix, "m_rarr", null);
            }, imaginary: false);
    }

    [SkippableFact]
    public void IsRealVector_combines_logical_vector_shape_with_real_matrix_status()
    {
        Runtime.Sync();
        const int token = 0x0600045f;
        CompareBoth("IsRealVector for real one-column matrix", "get_IsRealVector", token,
            (_, matrix) => Set(matrix, "m_parr", null), rows: 3, cols: 1, stride: 4, offset: 1, imaginary: false);
        CompareBoth("IsRealVector false for two logical columns", "get_IsRealVector", token,
            (_, matrix) => Set(matrix, "m_parr", null), rows: 1, cols: 2, stride: 2, offset: 1, imaginary: false);
        CompareBoth("IsRealVector short-circuits multi-column zero imaginary backing", "get_IsRealVector", token,
            (_, matrix) => Set(matrix, "m_parr", null), rows: 1, cols: 2, stride: 2, offset: 1);
        CompareBoth("IsRealVector false for vector with nonzero padded imaginary value", "get_IsRealVector", token,
            (_, matrix) => ((double[])Get(matrix, "m_iarr")!)[0] = 1, rows: 2, cols: 1, stride: 3, offset: 1);
        CompareBoth("IsRealVector canonicalizes uniform scalar pointer", "get_IsRealVector", token,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(4, 0, "L")), rows: 2, cols: 1, stride: 3, offset: 1, imaginary: false);
        CompareBoth("IsRealVector false for vector with nonuniform pointer", "get_IsRealVector", token,
            (side, matrix) => Pointer(side, matrix, 1, 0, side.Num(4, 0, "T")), rows: 2, cols: 1, stride: 3, offset: 1, imaginary: false);
        CompareBoth("IsRealVector preserves the original null-backing exception for a one-column live pointer", "get_IsRealVector", token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(7, 0, "L"));
                Set(matrix, "m_rarr", null);
                Set(matrix, "m_iarr", null);
            }, rows: 2, cols: 1, stride: 3, offset: 1);
        CompareBoth("IsRealVector short-circuits malformed pointers for multiple columns", "get_IsRealVector", token,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, "text");
                Set(matrix, "m_rarr", null);
                Set(matrix, "m_iarr", null);
            }, rows: 1, cols: 2, stride: 2, offset: 1);
    }
}
