using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValDimensionPropertiesDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly System.Type QMat = typeof(global::Quire.Mcd.MatVal);

    private static System.Type MapType(Side side, System.Type type) => side == Side.Quire ? type
        : type == QMat ? side.T("Mcd.MatVal")
        : type == typeof(global::Quire.Mcd.Dim) ? side.Dim
        : type;

    private static MethodInfo Method(Side side, string name, params System.Type[] parameters) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null,
            parameters.Select(parameter => MapType(side, parameter)).ToArray(), null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}");

    private static object Matrix(Side side, string dimension = "L", bool shared = false)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[] { 91, 1, 2, 92, 3, 4, 93 }, new double[7],
            2, 2, 3, new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
        Set(matrix, "m_dim", side.D(dimension));
        Set(matrix, "Shared", shared);
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static void StalePointer(Side side, object matrix, int row = 0, int col = 0)
    {
        int index = (int)Call(matrix, "Index", row, col)!;
        ((Hashtable)Get(matrix, "m_parr")!)[index] = side.Num(8, 0, "T");
        ((double[])Get(matrix, "m_rarr")!)[index] = 8;
    }

    private static IReadOnlyDictionary<int, object?> Snapshot(object? table) => table is Hashtable pointers
        ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
        : new Dictionary<int, object?>();

    private static string State(Side side, object? beforeTable, object? beforeReal, object? beforeImag,
        IReadOnlyDictionary<int, object?> beforePointers, object target)
    {
        object? afterTable = Get(target, "m_parr");
        string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";
        string tableAlias = beforeTable is Hashtable previous
            ? string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{(previous.ContainsKey(pair.Key) ? Ref(pair.Value, previous[pair.Key]) : "cleared")}"))
            : "none";
        string pointerRefs = afterTable is Hashtable table
            ? string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{(table.ContainsKey(pair.Key) ? Ref(pair.Value, table[pair.Key]) : "absent")}"))
            : string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}:absent"));
        return $"{MatValDifferentialTests.Storage(side, target)} table-ref {Ref(beforeTable, afterTable)}" +
            $" prior-table [{tableAlias}]" +
            $" real-ref {Ref(beforeReal, Get(target, "m_rarr"))} imag-ref {Ref(beforeImag, Get(target, "m_iarr"))}" +
            $" pointer-refs [{pointerRefs}]";
    }

    private static string DimensionResult(Side side, object target, object? value)
    {
        object? targetDimension = Get(target, "m_dim");
        object? mixed = Get(side.Dim, "mixed_matrix_dim");
        return $"{side.DimText(value)} same-matrix-dim {ReferenceEquals(value, targetDimension)}" +
            $" mixed-singleton {ReferenceEquals(value, mixed)}";
    }

    private static void Compare(string caseId, string name, int originalToken, System.Type[] parameters,
        object qTarget, object mTarget, object?[] qArgs, object?[] mArgs, bool dimensionResult = false)
    {
        var qMethod = Method(Side.Quire, name, parameters);
        var mMethod = Method(Side.Mathcad, name, parameters);
        Assert.Equal(originalToken, mMethod.MetadataToken);
        var qTable = Get(qTarget, "m_parr"); var mTable = Get(mTarget, "m_parr");
        var qReal = Get(qTarget, "m_rarr"); var mReal = Get(mTarget, "m_rarr");
        var qImag = Get(qTarget, "m_iarr"); var mImag = Get(mTarget, "m_iarr");
        var qPointers = Snapshot(qTable); var mPointers = Snapshot(mTable);
        Func<Side, object, object?, string> show = (side, target, value) => dimensionResult
            ? DimensionResult(side, target, value)
            : side.Show(value);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs,
            value => show(Side.Quire, qTarget, value), value => show(Side.Mathcad, mTarget, value),
            target => State(Side.Quire, qTable, qReal, qImag, qPointers, target) +
                (name == "IsUniformDimOf" ? $" requested-dim-ref {(ReferenceEquals(qArgs[0], Get(target, "m_dim")) ? "same" : "changed")}" : ""),
            target => State(Side.Mathcad, mTable, mReal, mImag, mPointers, target) +
                (name == "IsUniformDimOf" ? $" requested-dim-ref {(ReferenceEquals(mArgs[0], Get(target, "m_dim")) ? "same" : "changed")}" : ""));
    }

    private static void CompareBoth(string caseId, string name, int token, System.Type[] parameters,
        Action<Side, object> setup, object?[]? qArgs = null, object?[]? mArgs = null,
        string dimension = "L", bool shared = false, bool dimensionResult = false)
    {
        var q = Matrix(Side.Quire, dimension, shared); var m = Matrix(Side.Mathcad, dimension, shared);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        Compare(caseId, name, token, parameters, q, m, qArgs ?? [], mArgs ?? [], dimensionResult);
    }

    [SkippableFact]
    public void IsDimless_tracks_uniform_scalar_dimension_and_pointer_cleanup()
    {
        Runtime.Sync();
        const int token = 0x0600043a;
        CompareBoth("IsDimless with null dimension and no table", "get_IsDimless", token, [],
            (_, matrix) => Set(matrix, "m_parr", null), dimension: "poly");
        CompareBoth("IsDimless with identity dimension", "get_IsDimless", token, [],
            (_, _) => { }, dimension: "identity");
        CompareBoth("IsDimless with physical dimension", "get_IsDimless", token, [],
            (_, _) => { }, dimension: "L");
        CompareBoth("IsDimless clears stale-only pointers before checking dimension", "get_IsDimless", token, [],
            (side, matrix) => StalePointer(side, matrix), dimension: "identity");
        CompareBoth("IsDimless canonicalizes full uniform identity pointers", "get_IsDimless", token, [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(row + col + 1, 0, "identity"));
        }, dimension: "L");
        CompareBoth("IsDimless remains false for mixed live dimensions", "get_IsDimless", token, [], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(3, 0, "T"));
        }, dimension: "identity");
        CompareBoth("IsDimless returns false for a live non-NumVal pointer", "get_IsDimless", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"), dimension: "identity");
    }

    [SkippableFact]
    public void IsUniformDimOf_checks_live_pointer_dimensions_and_full_coverage()
    {
        Runtime.Sync();
        const int token = 0x06000437;
        System.Type[] signature = [typeof(global::Quire.Mcd.Dim)];
        CompareBoth("IsUniformDimOf matches without a table", "IsUniformDimOf", token, signature,
            (_, matrix) => Set(matrix, "m_parr", null), [Side.Quire.D("L")], [Side.Mathcad.D("L")]);
        CompareBoth("IsUniformDimOf rejects a different base dimension", "IsUniformDimOf", token, signature,
            (_, _) => { }, [Side.Quire.D("T")], [Side.Mathcad.D("T")]);
        CompareBoth("IsUniformDimOf leaves stale-only table and falls back to matrix dimension", "IsUniformDimOf", token, signature,
            (side, matrix) => StalePointer(side, matrix), [Side.Quire.D("L")], [Side.Mathcad.D("L")]);
        CompareBoth("IsUniformDimOf accepts a partial pointer matching the matrix dimension", "IsUniformDimOf", token, signature,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "L")), [Side.Quire.D("L")], [Side.Mathcad.D("L")]);
        CompareBoth("IsUniformDimOf rejects a partial live dimension mismatch", "IsUniformDimOf", token, signature,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "T")), [Side.Quire.D("T")], [Side.Mathcad.D("T")]);
        CompareBoth("IsUniformDimOf changes matrix dimension for full matching pointer coverage", "IsUniformDimOf", token, signature,
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(row + col + 2, 0, "L"));
            }, [Side.Quire.D("T")], [Side.Mathcad.D("T")]);
        CompareBoth("IsUniformDimOf assigns null requested dimension for full pointer coverage", "IsUniformDimOf", token, signature,
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(row + col + 2, 0, "L"));
            }, [null], [null]);
        CompareBoth("IsUniformDimOf rejects full mixed pointer dimensions without changing matrix dimension", "IsUniformDimOf", token, signature,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(1, 0, "L"));
                Pointer(side, matrix, 0, 1, side.Num(2, 0, "T"));
                Pointer(side, matrix, 1, 0, side.Num(3, 0, "L"));
                Pointer(side, matrix, 1, 1, side.Num(4, 0, "L"));
            }, [Side.Quire.D("T")], [Side.Mathcad.D("T")]);
        CompareBoth("IsUniformDimOf rejects a live non-NumVal pointer", "IsUniformDimOf", token, signature,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"), [Side.Quire.D("L")], [Side.Mathcad.D("L")]);
        var qEmpty = New(Side.Quire.T("Mcd.MatVal"), Array.Empty<double>(), Array.Empty<double>(),
            0, 0, 0, new Hashtable(), Enum.ToObject(Side.Quire.T("Mcd.Flags"), 2146697216), 0);
        var mEmpty = New(Side.Mathcad.T("Mcd.MatVal"), Array.Empty<double>(), Array.Empty<double>(),
            0, 0, 0, new Hashtable(), Enum.ToObject(Side.Mathcad.T("Mcd.Flags"), 2146697216), 0);
        Set(qEmpty, "m_dim", Side.Quire.D("L"));
        Set(mEmpty, "m_dim", Side.Mathcad.D("L"));
        Compare("IsUniformDimOf assigns requested dimension for zero-area matrix", "IsUniformDimOf", token,
            signature, qEmpty, mEmpty, [Side.Quire.D("T")], [Side.Mathcad.D("T")]);
    }

    [SkippableFact]
    public void PhysicalDimension_returns_uniform_or_mixed_dimension_with_identity()
    {
        Runtime.Sync();
        const int token = 0x06000452;
        CompareBoth("PhysicalDimension without pointer table", "get_PhysicalDimension", token, [],
            (_, matrix) => Set(matrix, "m_parr", null), dimensionResult: true);
        CompareBoth("PhysicalDimension clears stale-only entries and returns matrix dimension", "get_PhysicalDimension", token, [],
            (side, matrix) => StalePointer(side, matrix), dimensionResult: true);
        CompareBoth("PhysicalDimension returns matching uniform live dimension", "get_PhysicalDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(4, 0, "L")), dimensionResult: true);
        CompareBoth("PhysicalDimension returns mixed marker for incompatible dimensions", "get_PhysicalDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(4, 0, "T")), dimensionResult: true);
        CompareBoth("PhysicalDimension returns mixed marker for full mixed pointers", "get_PhysicalDimension", token, [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, row == 0 ? "L" : "T"));
        }, dimensionResult: true);
        CompareBoth("PhysicalDimension observes nonscalar pointer dimension", "get_PhysicalDimension", token, [], (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), 2, 1);
            Set(nested, "m_dim", side.D("v"));
            Pointer(side, matrix, 0, 1, nested);
        }, dimensionResult: true);
    }

    [SkippableFact]
    public void UniformDimension_canonicalizes_uniform_pointers_and_reports_mixed_storage()
    {
        Runtime.Sync();
        const int token = 0x06000453;
        CompareBoth("UniformDimension without pointer table", "get_UniformDimension", token, [],
            (_, matrix) => Set(matrix, "m_parr", null), dimensionResult: true);
        CompareBoth("UniformDimension clears stale-only entries", "get_UniformDimension", token, [],
            (side, matrix) => StalePointer(side, matrix), dimensionResult: true);
        CompareBoth("UniformDimension canonicalizes partial matching pointers", "get_UniformDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 1, "L")), dimensionResult: true);
        CompareBoth("UniformDimension canonicalizes full matching pointers", "get_UniformDimension", token, [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "L"));
        }, dimensionResult: true);
        CompareBoth("UniformDimension accepts zero pointer on polymorphic matrix", "get_UniformDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(0, 0, "mcd_zero")),
            dimension: "poly", dimensionResult: true);
        CompareBoth("UniformDimension reports mixed marker for incompatible partial pointer", "get_UniformDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(8, 0, "T")), dimensionResult: true);
        CompareBoth("UniformDimension reports mixed marker for non-NumVal pointer", "get_UniformDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"), dimensionResult: true);
        CompareBoth("UniformDimension repacks Shared padded uniform storage", "get_UniformDimension", token, [],
            (side, matrix) => Pointer(side, matrix, 1, 1, side.Num(8, 0, "L")), shared: true, dimensionResult: true);
    }
}
