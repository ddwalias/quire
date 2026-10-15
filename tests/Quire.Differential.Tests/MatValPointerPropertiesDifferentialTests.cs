using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValPointerPropertiesDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static MethodInfo Method(Side side, string name)
    {
        var method = side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, Type.EmptyTypes, null);
        return method ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}()");
    }

    private static object Matrix(Side side, string dimension = "L", bool shared = false)
    {
        var matrix = New(side.T("Mcd.MatVal"),
            new double[] { 91, 1, 2, 92, 3, 4, 93 }, new double[7], 2, 2, 3,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
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

    private static string State(Side side, object? beforeTable, object? beforeReal, object? beforeImag,
        IReadOnlyDictionary<int, object?> beforePointers, object target)
    {
        object? afterTable = Get(target, "m_parr");
        string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";
        string tableAlias = beforeTable is Hashtable previous
            ? string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{(previous.ContainsKey(pair.Key) ? Ref(pair.Value, previous[pair.Key]) : "cleared")}"))
            : "none";
        string pointers = afterTable is Hashtable table
            ? string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair =>
                $"{pair.Key}:{(table.ContainsKey(pair.Key) ? Ref(pair.Value, table[pair.Key]) : "absent")}"))
            : string.Join(";", beforePointers.OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}:absent"));
        return $"{MatValDifferentialTests.Storage(side, target)} table-ref {Ref(beforeTable, afterTable)}" +
            $" prior-table [{tableAlias}]" +
            $" real-ref {Ref(beforeReal, Get(target, "m_rarr"))} imag-ref {Ref(beforeImag, Get(target, "m_iarr"))}" +
            $" pointer-refs [{pointers}]";
    }

    private static void Compare(string caseId, string methodName, int originalToken, object qTarget, object mTarget)
    {
        var qMethod = Method(Side.Quire, methodName);
        var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(originalToken, mMethod.MetadataToken);
        var qTable = Get(qTarget, "m_parr"); var mTable = Get(mTarget, "m_parr");
        var qReal = Get(qTarget, "m_rarr"); var mReal = Get(mTarget, "m_rarr");
        var qImag = Get(qTarget, "m_iarr"); var mImag = Get(mTarget, "m_iarr");
        var qPointers = Snapshot(qTable); var mPointers = Snapshot(mTable);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [], mMethod, mTarget, [],
            quireState: target => State(Side.Quire, qTable, qReal, qImag, qPointers, target),
            originalState: target => State(Side.Mathcad, mTable, mReal, mImag, mPointers, target));
    }

    private static IReadOnlyDictionary<int, object?> Snapshot(object? table) => table is Hashtable pointers
        ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
        : new Dictionary<int, object?>();

    private static void CompareBoth(string caseId, string methodName, int token,
        Action<Side, object> setup, string dimension = "L", bool shared = false)
    {
        var q = Matrix(Side.Quire, dimension, shared); var m = Matrix(Side.Mathcad, dimension, shared);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        Compare(caseId, methodName, token, q, m);
    }

    [SkippableFact]
    public void HasPtrs_reports_only_nonuniform_live_pointer_representations()
    {
        Runtime.Sync();
        const int token = 0x06000434;
        CompareBoth("HasPtrs with null table", "get_HasPtrs", token, (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("HasPtrs clears stale-only table", "get_HasPtrs", token, (side, matrix) => StalePointer(side, matrix));
        CompareBoth("HasPtrs canonicalizes partial compatible NumVal pointer", "get_HasPtrs", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 2, "L")));
        CompareBoth("HasPtrs canonicalizes partial mcd_zero pointer", "get_HasPtrs", token,
            (side, matrix) => Pointer(side, matrix, 1, 0, side.Num(0, 0, "mcd_zero")));
        CompareBoth("HasPtrs reports incompatible dimension pointer", "get_HasPtrs", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "T")));
        CompareBoth("HasPtrs reports non-NumVal pointer", "get_HasPtrs", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"));
        CompareBoth("HasPtrs reports nonscalar matrix pointer", "get_HasPtrs", token, (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), 2, 1);
            Set(nested, "m_dim", side.D("v"));
            Pointer(side, matrix, 0, 1, nested);
        });
        CompareBoth("HasPtrs canonicalizes full compatible coverage", "get_HasPtrs", token, (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(row + col + 1, 0, "L"));
        });
        CompareBoth("HasPtrs retains Shared padded storage while reporting incompatible pointer", "get_HasPtrs", token,
            (side, matrix) => Pointer(side, matrix, 1, 1, side.Num(9, 0, "T")), shared: true);
    }

    [SkippableFact]
    public void RepHasPtrs_scans_live_markers_and_clears_stale_tables()
    {
        Runtime.Sync();
        const int token = 0x06000435;
        CompareBoth("RepHasPtrs with null table", "get_RepHasPtrs", token, (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("RepHasPtrs clears stale-only entries", "get_RepHasPtrs", token, (side, matrix) => StalePointer(side, matrix));
        CompareBoth("RepHasPtrs preserves live pointer in padded Shared storage", "get_RepHasPtrs", token,
            (side, matrix) => Pointer(side, matrix, 1, 0, "text"), shared: true);
        CompareBoth("RepHasPtrs stops at live entry and retains stale entries", "get_RepHasPtrs", token, (side, matrix) =>
        {
            StalePointer(side, matrix, 0, 0);
            Pointer(side, matrix, 1, 1, side.Num(4, 0, "L"));
        }, shared: true);
    }

    [SkippableFact]
    public void IsUniformScalar_property_mutates_only_uniform_pointer_tables()
    {
        Runtime.Sync();
        const int token = 0x0600043e;
        CompareBoth("IsUniformScalar with null table", "get_IsUniformScalar", token, (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("IsUniformScalar clears stale-only table", "get_IsUniformScalar", token, (side, matrix) => StalePointer(side, matrix));
        CompareBoth("IsUniformScalar reinserts partial matching scalar pointer", "get_IsUniformScalar", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 2, "L")));
        CompareBoth("IsUniformScalar accepts partial zero pointer", "get_IsUniformScalar", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(0, 0, "mcd_zero")));
        CompareBoth("IsUniformScalar rejects incompatible dimensions", "get_IsUniformScalar", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "T")));
        CompareBoth("IsUniformScalar rejects non-NumVal live pointer", "get_IsUniformScalar", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"));
        CompareBoth("IsUniformScalar handles full matching coverage", "get_IsUniformScalar", token, (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "L"));
        });
        CompareBoth("IsUniformScalar repacks Shared offset storage", "get_IsUniformScalar", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 0, "L")), shared: true);
    }

    [SkippableFact]
    public void IsUniformScalarPtrs_directly_compares_private_pointer_logic()
    {
        Runtime.Sync();
        const int token = 0x0600043f;
        CompareBoth("IsUniformScalarPtrs null table exception", "IsUniformScalarPtrs", token,
            (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("IsUniformScalarPtrs clears stale-only table", "IsUniformScalarPtrs", token,
            (side, matrix) => StalePointer(side, matrix));
        CompareBoth("IsUniformScalarPtrs folds compatible scalar pointer", "IsUniformScalarPtrs", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(6, 1, "L")));
        CompareBoth("IsUniformScalarPtrs allows mcd_zero with matching scalar", "IsUniformScalarPtrs", token, (side, matrix) =>
        {
            Pointer(side, matrix, 0, 1, side.Num(6, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(0, 0, "mcd_zero"));
        });
        CompareBoth("IsUniformScalarPtrs rejects incompatible dimension", "IsUniformScalarPtrs", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(6, 0, "T")));
        CompareBoth("IsUniformScalarPtrs rejects non-NumVal live value", "IsUniformScalarPtrs", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"));
        CompareBoth("IsUniformScalarPtrs rejects nonscalar pointer", "IsUniformScalarPtrs", token, (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), 2, 1);
            Set(nested, "m_dim", side.D("v"));
            Pointer(side, matrix, 0, 1, nested);
        });
        CompareBoth("IsUniformScalarPtrs full zero coverage", "IsUniformScalarPtrs", token, (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(0, 0, "mcd_zero"));
        });
        CompareBoth("IsUniformScalarPtrs repacks Shared padded storage", "IsUniformScalarPtrs", token,
            (side, matrix) => Pointer(side, matrix, 1, 1, side.Num(3, 0, "L")), shared: true);
    }

    [SkippableFact]
    public void IsUniform_property_compares_pointer_uniformity_and_canonicalization()
    {
        Runtime.Sync();
        const int token = 0x06000457;
        CompareBoth("IsUniform with null table", "get_IsUniform", token, (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("IsUniform clears stale-only table", "get_IsUniform", token, (side, matrix) => StalePointer(side, matrix));
        CompareBoth("IsUniform reinserts compatible partial pointer", "get_IsUniform", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(5, 1, "L")));
        CompareBoth("IsUniform accepts zero pointer on polymorphic matrix", "get_IsUniform", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(0, 0, "mcd_zero")), dimension: "poly");
        CompareBoth("IsUniform canonicalizes full matching coverage", "get_IsUniform", token, (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(2 + row + col, 0, "L"));
        });
        CompareBoth("IsUniform allows zero and matching scalar pointers together", "get_IsUniform", token, (side, matrix) =>
        {
            Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(0, 0, "mcd_zero"));
        });
        CompareBoth("IsUniform rejects incompatible pointer dimension", "get_IsUniform", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(2, 0, "T")));
        CompareBoth("IsUniform observes non-NumVal scalar-like pointer", "get_IsUniform", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, "text"));
        CompareBoth("IsUniform rejects nonscalar nested matrix pointer", "get_IsUniform", token, (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), 2, 1);
            Set(nested, "m_dim", side.D("v"));
            Pointer(side, matrix, 0, 1, nested);
        });
        CompareBoth("IsUniform repacks Shared offset storage", "get_IsUniform", token,
            (side, matrix) => Pointer(side, matrix, 0, 1, side.Num(7, 0, "L")), shared: true);
    }
}
