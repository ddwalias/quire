using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValPointerHelpersDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly System.Type QMat = typeof(global::Quire.Mcd.MatVal);
    private static System.Type MMat => Mathcad.Type("mplrun", "Mcd.MatVal");

    private static MethodInfo Method(Side side, string name, params System.Type[] parameters)
    {
        System.Type Map(System.Type type) => side == Side.Quire ? type
            : type == QMat ? MMat
            : type == typeof(global::Quire.Mcd.Dim) ? side.Dim
            : type;
        return side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, parameters.Select(Map).ToArray(), null)
            ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}");
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

    private static object OneByOneMatrix(Side side)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[] { 1 }, new double[1], 1, 1, 1,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private sealed record PointerSnapshot(int Index, object? Value, object? Dimension);

    private static Dictionary<int, PointerSnapshot> PointerSnapshots(object? table) => table is Hashtable pointers
        ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry =>
        {
            object? value = entry.Value;
            object? dimension = value?.GetType().Name == "NumVal" ? Get(value, "m_dim") : null;
            return new PointerSnapshot((int)entry.Key, value, dimension);
        })
        : new Dictionary<int, PointerSnapshot>();

    private static string State(Side side, object? priorTable, object? priorReal, object? priorImag,
        IReadOnlyDictionary<int, PointerSnapshot> priorPointers, object target)
    {
        string changed(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";
        string PointerText(int index, object? value, PointerSnapshot? prior)
        {
            if (value == null) return $"{index}:null";
            string identity = prior != null && ReferenceEquals(prior.Value, value) ? "same-object" : "new-object";
            if (value.GetType().Name != "NumVal") return $"{index}:{value.GetType().Name}:{identity}";
            object? dimension = Get(value, "m_dim");
            string dimRef = prior == null ? "new-dimension" : changed(prior.Dimension, dimension) + "-dimension-ref";
            return $"{index}:NumVal:{identity}:dim={side.DimText(dimension)}:{dimRef}";
        }
        string beforePointers = string.Join(";", priorPointers.OrderBy(pair => pair.Key).Select(pair =>
            PointerText(pair.Key, pair.Value.Value, pair.Value)));
        string afterPointers = Get(target, "m_parr") is Hashtable table
            ? string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
                PointerText((int)entry.Key, entry.Value,
                    priorPointers.TryGetValue((int)entry.Key, out var prior) ? prior : null)))
            : "null";
        return $"{MatValDifferentialTests.Storage(side, target)} table-ref {changed(priorTable, Get(target, "m_parr"))}" +
            $" real-ref {changed(priorReal, Get(target, "m_rarr"))} imag-ref {changed(priorImag, Get(target, "m_iarr"))}" +
            $" pointer-before [{beforePointers}] pointer-after [{afterPointers}]";
    }

    private static void Compare(string caseId, string name, System.Type[] parameters,
        object qTarget, object mTarget, object?[] qArgs, object?[] mArgs,
        bool canonicalResult = false)
    {
        var qTable = Get(qTarget, "m_parr"); var mTable = Get(mTarget, "m_parr");
        var qReal = Get(qTarget, "m_rarr"); var mReal = Get(mTarget, "m_rarr");
        var qImag = Get(qTarget, "m_iarr"); var mImag = Get(mTarget, "m_iarr");
        var qPointers = PointerSnapshots(qTable); var mPointers = PointerSnapshots(mTable);
        Func<Side, object?, string> show = (side, value) =>
        {
            if (canonicalResult)
            {
                object target = side == Side.Quire ? qTarget : mTarget;
                return $"same-target {ReferenceEquals(value, target)} " +
                    (value == null ? "null" : MatValDifferentialTests.Storage(side, value));
            }
            return side.Show(value);
        };
        MethodComparison.Compare(caseId, Contract,
            Method(Side.Quire, name, parameters), qTarget, qArgs,
            Method(Side.Mathcad, name, parameters), mTarget, mArgs,
            value => show(Side.Quire, value), value => show(Side.Mathcad, value),
            target => State(Side.Quire, qTable, qReal, qImag, qPointers, target),
            target => State(Side.Mathcad, mTable, mReal, mImag, mPointers, target));
    }

    private static void CompareBoth(string caseId, string name, System.Type[] parameters,
        Action<Side, object> setup, object?[]? qArgs = null, object?[]? mArgs = null, bool canonicalResult = false)
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        setup(Side.Quire, q); setup(Side.Mathcad, m);
        Compare(caseId, name, parameters, q, m, qArgs ?? [], mArgs ?? [], canonicalResult);
    }

    private static void AssertOriginalToken(string name, int token, params System.Type[] parameters) =>
        Assert.Equal(token, Method(Side.Mathcad, name, parameters).MetadataToken);

    [SkippableFact]
    public void Canonicalize_returns_identity_and_reinserts_or_retains_live_pointers()
    {
        Runtime.Sync();
        AssertOriginalToken("Canonicalize", 0x0600043b);

        CompareBoth("Canonicalize leaves a null pointer table alone", "Canonicalize", [], (_, matrix) => Set(matrix, "m_parr", null), canonicalResult: true);
        CompareBoth("Canonicalize replaces the special string matrix dimension", "Canonicalize", [], (side, matrix) =>
        {
            Set(matrix, "m_dim", side.D("string_dim"));
        }, canonicalResult: true);
        CompareBoth("Canonicalize drops stale table entries", "Canonicalize", [], (side, matrix) =>
        {
            ((double[])Get(matrix, "m_rarr")!)[(int)Call(matrix, "Index", 0, 0)!] = 8;
            ((Hashtable)Get(matrix, "m_parr")!)[(int)Call(matrix, "Index", 0, 0)!] = side.Num(5, 0, "L");
        }, canonicalResult: true);
        CompareBoth("Canonicalize reinserts a partial compatible scalar pointer", "Canonicalize", [], (side, matrix) =>
            Pointer(side, matrix, 0, 1, side.Num(7, 2, "L")), canonicalResult: true);
        CompareBoth("Canonicalize retains an incompatible scalar pointer", "Canonicalize", [], (side, matrix) =>
            Pointer(side, matrix, 0, 1, side.Num(7, 2, "T")), canonicalResult: true);
        CompareBoth("Canonicalize promotes full nonzero pointer coverage", "Canonicalize", [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(row + col + 1, 0, "T"));
        }, canonicalResult: true);
        CompareBoth("Canonicalize handles full zero pointer coverage", "Canonicalize", [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(0, 0, "mcd_zero"));
        }, canonicalResult: true);
        CompareBoth("Canonicalize merges ordinary and zero pointer dimensions", "Canonicalize", [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, row == 0 && col == 0
                    ? side.Num(0, 0, "mcd_zero")
                    : side.Num(1 + row + col, 0, "L"));
        }, canonicalResult: true);
        CompareBoth("Canonicalize retains an incompatible nonnumeric pointer", "Canonicalize", [], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, "text");
        }, canonicalResult: true);
        CompareBoth("Canonicalize repacks a Shared compatible pointer", "Canonicalize", [], (side, matrix) =>
        {
            Set(matrix, "Shared", true);
            Pointer(side, matrix, 0, 1, side.Num(4, 0, "L"));
        }, canonicalResult: true);
    }

    [SkippableFact]
    public void TidyPtrs_direct_counts_dimensions_and_shared_repacking()
    {
        Runtime.Sync();
        AssertOriginalToken("TidyPtrs", 0x06000442, typeof(int), typeof(int), typeof(global::Quire.Mcd.Dim), typeof(bool));
        System.Type[] signature = [typeof(int), typeof(int), typeof(global::Quire.Mcd.Dim), typeof(bool)];

        CompareBoth("TidyPtrs zero count clears a table", "TidyPtrs", signature,
            (_, _) => { }, [0, 0, null, false], [0, 0, null, false]);
        CompareBoth("TidyPtrs zero count accepts an already null table", "TidyPtrs", signature,
            (_, matrix) => Set(matrix, "m_parr", null), [0, 0, null, false], [0, 0, null, false]);
        CompareBoth("TidyPtrs zero count rejects another live pointer", "TidyPtrs", signature,
            (side, matrix) => Pointer(side, matrix, 0, 0, "other"), [0, 0, null, true], [0, 0, null, true]);
        CompareBoth("TidyPtrs retains an incompatible partial pointer set", "TidyPtrs", signature,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(2, 0, "L")),
            [1, 0, Side.Quire.D("T"), false], [1, 0, Side.Mathcad.D("T"), false]);
        CompareBoth("TidyPtrs reinserts a compatible partial pointer set", "TidyPtrs", signature,
            (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(2, 0, "L")),
            [1, 0, Side.Quire.D("L"), false], [1, 0, Side.Mathcad.D("L"), false]);
        CompareBoth("TidyPtrs partial compatible path keeps other pointers", "TidyPtrs", signature,
            (side, matrix) => Pointer(side, matrix, 0, 0, "outside"),
            [1, 0, Side.Quire.D("L"), true], [1, 0, Side.Mathcad.D("L"), true]);
        CompareBoth("TidyPtrs partial compatible zero mutates the retained NumVal", "TidyPtrs", signature,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
                Pointer(side, matrix, 1, 0, "outside");
            }, [1, 1, Side.Quire.D("L"), true], [1, 1, Side.Mathcad.D("L"), true]);
        CompareBoth("TidyPtrs fully covered nonzero pointers use regular reinsertion", "TidyPtrs", signature,
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(3 + row + col, 0, "T"));
            }, [4, 0, Side.Quire.D("T"), false], [4, 0, Side.Mathcad.D("T"), false]);
        CompareBoth("TidyPtrs fully covered zero pointers use zero-aware reinsertion", "TidyPtrs", signature,
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(0, 0, "mcd_zero"));
            }, [4, 4, Side.Quire.D("L"), false], [4, 4, Side.Mathcad.D("L"), false]);
        CompareBoth("TidyPtrs full coverage with one zero NumVal selects regular reinsertion", "TidyPtrs", signature,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
                Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
                Pointer(side, matrix, 1, 0, side.Num(3, 0, "L"));
                Pointer(side, matrix, 1, 1, side.Num(4, 0, "L"));
            }, [4, 1, Side.Quire.D("L"), false], [4, 1, Side.Mathcad.D("L"), false]);
        CompareBoth("TidyPtrs full coverage with multiple zero NumVals selects regular reinsertion", "TidyPtrs", signature,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
                Pointer(side, matrix, 0, 1, side.Num(0, 0, "mcd_zero"));
                Pointer(side, matrix, 1, 0, side.Num(3, 0, "L"));
                Pointer(side, matrix, 1, 1, side.Num(4, 0, "L"));
            }, [4, 2, Side.Quire.D("L"), false], [4, 2, Side.Mathcad.D("L"), false]);
        CompareBoth("TidyPtrs partial compatible count with a zero and ordinary NumVal", "TidyPtrs", signature,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
                Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
            }, [2, 1, Side.Quire.D("L"), false], [2, 1, Side.Mathcad.D("L"), false]);
        CompareBoth("TidyPtrs partial mixed dimensions rejects without reinsertion", "TidyPtrs", signature,
            (side, matrix) =>
            {
                Pointer(side, matrix, 0, 0, side.Num(2, 0, "L"));
                Pointer(side, matrix, 0, 1, side.Num(3, 0, "T"));
            }, [2, 0, Side.Quire.D("T"), false], [2, 0, Side.Mathcad.D("T"), false]);
        CompareBoth("TidyPtrs full coverage with a null dimension keeps the existing dimension", "TidyPtrs", signature,
            (side, matrix) =>
            {
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(3 + row + col, 0, "T"));
            }, [4, 0, null, false], [4, 0, null, false]);
        CompareBoth("TidyPtrs repacks a Shared fully covered matrix", "TidyPtrs", signature,
            (side, matrix) =>
            {
                Set(matrix, "Shared", true);
                for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                    Pointer(side, matrix, row, col, side.Num(3 + row + col, 0, "T"));
            }, [4, 0, Side.Quire.D("T"), false], [4, 0, Side.Mathcad.D("T"), false]);
    }

    [SkippableFact]
    public void IsUniformPtrs_repairs_live_uniform_pointers_and_reports_other_dimensions()
    {
        Runtime.Sync();
        AssertOriginalToken("IsUniformPtrs", 0x06000458);

        CompareBoth("IsUniformPtrs on a null table raises the original exception", "IsUniformPtrs", [], (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("IsUniformPtrs ignores stale entries", "IsUniformPtrs", [], (side, matrix) =>
        {
            int index = (int)Call(matrix, "Index", 0, 0)!;
            ((Hashtable)Get(matrix, "m_parr")!)[index] = side.Num(1, 0, "T");
        });
        CompareBoth("IsUniformPtrs clears a stale-only table without unpacking Shared storage", "IsUniformPtrs", [], (side, matrix) =>
        {
            Set(matrix, "Shared", true);
            int index = (int)Call(matrix, "Index", 0, 0)!;
            ((Hashtable)Get(matrix, "m_parr")!)[index] = side.Num(1, 0, "T");
        });
        CompareBoth("IsUniformPtrs clears an empty Shared pointer table without unpacking", "IsUniformPtrs", [], (_, matrix) =>
            Set(matrix, "Shared", true));
        CompareBoth("IsUniformPtrs accepts compatible scalar entries", "IsUniformPtrs", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, side.Num(1, 0, "L")));
        CompareBoth("IsUniformPtrs accepts zero-dimension entries", "IsUniformPtrs", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero")));
        var qOne = OneByOneMatrix(Side.Quire); var mOne = OneByOneMatrix(Side.Mathcad);
        Pointer(Side.Quire, qOne, 0, 0, Side.Quire.Num(0, 0, "mcd_zero"));
        Pointer(Side.Mathcad, mOne, 0, 0, Side.Mathcad.Num(0, 0, "mcd_zero"));
        Compare("IsUniformPtrs full-coverage 1x1 zero NumVal", "IsUniformPtrs", [],
            qOne, mOne, [], []);
        CompareBoth("IsUniformPtrs accepts one zero and one compatible NumVal", "IsUniformPtrs", [], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
            Pointer(side, matrix, 0, 1, side.Num(2, 0, "L"));
        });
        CompareBoth("IsUniformPtrs accepts multiple zeros with matching ordinary coverage", "IsUniformPtrs", [], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
            Pointer(side, matrix, 0, 1, side.Num(0, 0, "mcd_zero"));
            Pointer(side, matrix, 1, 0, side.Num(3, 0, "L"));
            Pointer(side, matrix, 1, 1, side.Num(4, 0, "L"));
        });
        CompareBoth("IsUniformPtrs rejects incompatible dimensions", "IsUniformPtrs", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, side.Num(1, 0, "T")));
        CompareBoth("IsUniformPtrs rejects a mixed pair of live NumVal dimensions", "IsUniformPtrs", [], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(1, 0, "L"));
            Pointer(side, matrix, 0, 1, side.Num(2, 0, "T"));
        });
        CompareBoth("IsUniformPtrs rejects mixed dimensions after a zero pointer", "IsUniformPtrs", [], (side, matrix) =>
        {
            Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero"));
            Pointer(side, matrix, 0, 1, side.Num(1, 0, "L"));
            Pointer(side, matrix, 1, 0, side.Num(2, 0, "T"));
        });
        CompareBoth("IsUniformPtrs recognizes a nonnumeric pointer dimension", "IsUniformPtrs", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, "text"));
        CompareBoth("IsUniformPtrs handles full pointer coverage", "IsUniformPtrs", [], (side, matrix) =>
        {
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "L"));
        });
        CompareBoth("IsUniformPtrs full coverage adopts pointer dimension over incompatible matrix dimension", "IsUniformPtrs", [], (side, matrix) =>
        {
            Set(matrix, "m_dim", side.D("T"));
            for (int col = 0; col < 2; col++) for (int row = 0; row < 2; row++)
                Pointer(side, matrix, row, col, side.Num(1 + row + col, 0, "L"));
        });
    }

    [SkippableFact]
    public void IsUniformPtrsNoChange_checks_without_mutating_storage()
    {
        Runtime.Sync();
        AssertOriginalToken("IsUniformPtrsNoChange", 0x06000459);

        CompareBoth("IsUniformPtrsNoChange accepts a null table", "IsUniformPtrsNoChange", [], (_, matrix) => Set(matrix, "m_parr", null));
        CompareBoth("IsUniformPtrsNoChange ignores stale pointers", "IsUniformPtrsNoChange", [], (side, matrix) =>
        {
            int index = (int)Call(matrix, "Index", 0, 0)!;
            ((Hashtable)Get(matrix, "m_parr")!)[index] = side.Num(2, 0, "T");
        });
        CompareBoth("IsUniformPtrsNoChange accepts a matching scalar dimension", "IsUniformPtrsNoChange", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, side.Num(2, 0, "L")));
        CompareBoth("IsUniformPtrsNoChange rejects a different ordinary dimension", "IsUniformPtrsNoChange", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, side.Num(2, 0, "T")));
        CompareBoth("IsUniformPtrsNoChange accepts special zero dimensions", "IsUniformPtrsNoChange", [], (side, matrix) =>
            Pointer(side, matrix, 0, 0, side.Num(0, 0, "mcd_zero")));
        CompareBoth("IsUniformPtrsNoChange rejects a nested matrix with string_dim", "IsUniformPtrsNoChange", [], (side, matrix) =>
        {
            var nested = New(side.T("Mcd.MatVal"), 2, 1);
            Set(nested, "m_dim", side.D("string_dim"));
            Pointer(side, matrix, 0, 0, nested);
        });
    }
}
