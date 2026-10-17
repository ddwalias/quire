using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCrazyDimCoordinatesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007ae;
    private const int MatDimToken = 0x060007af;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record CrazyDimCoordinatesSnapshot(object Matrix, object? Real, object? Imag, object? Table,
        object? Dimension, object? Flags, int Rows, int Cols, int Stride, int Offset, double[]? RealValues,
        double[]? ImagValues, IReadOnlyList<(object? Key, object? Value)> Entries,
        IReadOnlyList<CrazyDimCoordinatesSnapshot> Children);

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type Crazy(Side side) => Nested(MatDim(side), "crazy_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static MethodInfo DimMethod(Side side) =>
        Crazy(side).GetMethod("dim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim.dim(int, int)");

    private static MethodInfo MatDimMethod(Side side) =>
        Crazy(side).GetMethod("matdim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim.matdim(int, int)");

    private static object CreateMatrix(Side side, int rows, int cols, string dimension = "L", double valueStart = 1)
    {
        int count = rows * cols;
        var matrix = New(side.T("Mcd.MatVal"), Enumerable.Range(0, count).Select(i => valueStart + i).ToArray(),
            new double[count], rows, cols, rows, null, Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static object MakeCrazy(Side side, object matrix)
    {
        var method = Crazy(side).GetMethod("make", BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.DeclaredOnly, null, [side.T("Mcd.MatVal")], null)
            ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");
        return method.Invoke(null, [matrix])!;
    }

    private static void SetNestedMatrix(Side side, object outer, int row, int col, string dimension, double value)
    {
        var child = CreateMatrix(side, 1, 1, dimension);
        ((double[])Get(child, "m_rarr")!)[0] = value;
        Call(outer, "set_ptrs_2", row, col, child);
    }

    private static string MatValState(Side side, object value, HashSet<object>? seen = null)
    {
        seen ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
        if (!seen.Add(value)) return "MatVal(cycle)";
        var real = Get(value, "m_rarr") as double[];
        var imag = Get(value, "m_iarr") as double[];
        var table = Get(value, "m_parr") as Hashtable;
        string Entries() => table == null ? "null" : string.Join(";", table.Cast<DictionaryEntry>()
            .OrderBy(entry => Convert.ToInt32(entry.Key))
            .Select(entry => $"{entry.Key}=" + (entry.Value?.GetType().Name == "MatVal"
                ? MatValState(side, entry.Value, seen)
                : entry.Value?.GetType().Name == "NumVal" ? side.Show(entry.Value) : entry.Value?.GetType().Name ?? "null")));
        object data = Get(value, "Data")!;
        return $"MatVal({Get(value, "Rows")}x{Get(value, "Cols")},stride={Get(value, "Anr")},offset={Get(value, "Offset")}," +
            $"flags={Convert.ToInt64(Get(data, "m_flags"))},dim={side.DimText(Get(value, "m_dim"))},real=[{string.Join(",", real?.Select(Side.Bits) ?? [])}]," +
            $"imag=[{string.Join(",", imag?.Select(Side.Bits) ?? [])}],ptrs=[{Entries()}])";
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is int i) return i.ToString();
        if (value.GetType() == side.Dim) return $"Dim({side.DimText(value)})";
        if (value.GetType() == UniformDim(side))
        {
            return $"uniform_dim({Get(value, "Rows")}x{Get(value, "Cols")},dim={side.DimText(Get(value, "m_dim"))})";
        }
        if (value.GetType().Name == "crazy_dim")
        {
            var matrix = Get(value, "m_matrix")!;
            return $"crazy_dim({Get(value, "Rows")}x{Get(value, "Cols")};{MatValState(side, matrix)})";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) => Show(side, target);

    private static CrazyDimCoordinatesSnapshot CaptureMatrix(object matrix)
    {
        object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr");
        IReadOnlyList<(object? Key, object? Value)> entries = table is Hashtable hash
            ? hash.Cast<DictionaryEntry>().Select(entry => (Key: (object?)entry.Key, Value: entry.Value)).ToArray()
            : Array.Empty<(object? Key, object? Value)>();
        var children = entries.Select(entry => entry.Value).Where(value => value?.GetType().Name == "MatVal")
            .Select(value => CaptureMatrix(value!)).ToArray();
        return new CrazyDimCoordinatesSnapshot(matrix, real, imag, table, Get(matrix, "m_dim"),
            Get(Get(matrix, "Data")!, "m_flags"),
            (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!, (int)Get(matrix, "Anr")!,
            (int)Get(matrix, "Offset")!, real is double[] ra ? ra.ToArray() : null,
            imag is double[] ia ? ia.ToArray() : null, entries, children);
    }

    private static void AssertUnchanged(CrazyDimCoordinatesSnapshot before)
    {
        Assert.Same(before.Real, Get(before.Matrix, "m_rarr"));
        Assert.Same(before.Imag, Get(before.Matrix, "m_iarr"));
        Assert.Same(before.Table, Get(before.Matrix, "m_parr"));
        Assert.Equal(before.Rows, (int)Get(before.Matrix, "Rows")!);
        Assert.Equal(before.Cols, (int)Get(before.Matrix, "Cols")!);
        Assert.Equal(before.Stride, (int)Get(before.Matrix, "Anr")!);
        Assert.Equal(before.Offset, (int)Get(before.Matrix, "Offset")!);
        Assert.Equal(before.Dimension, Get(before.Matrix, "m_dim"));
        Assert.Equal(before.Flags, Get(Get(before.Matrix, "Data")!, "m_flags"));

        if (before.RealValues != null && Get(before.Matrix, "m_rarr") is double[] currentReal)
            Assert.Equal(before.RealValues, currentReal);
        if (before.ImagValues != null && Get(before.Matrix, "m_iarr") is double[] currentImag)
            Assert.Equal(before.ImagValues, currentImag);

        var currentTable = Get(before.Matrix, "m_parr") as Hashtable;
        if (before.Entries.Count == 0)
        {
            Assert.True(currentTable == null || currentTable.Count == 0);
        }
        else
        {
            Assert.NotNull(currentTable);
            Assert.Equal(before.Entries.Count, currentTable!.Count);
            foreach (var (key, value) in before.Entries)
            {
                Assert.True(currentTable.ContainsKey(key!));
                Assert.Same(value, currentTable[key!]);
            }
        }

        foreach (var child in before.Children)
            AssertUnchanged(child);
    }

    private static void CompareDim(string caseId, Func<Side, object> makeTarget, int i, int j)
    {
        object qTarget = makeTarget(Side.Quire);
        object mTarget = makeTarget(Side.Mathcad);
        var qSnap = CaptureMatrix(Get(qTarget, "m_matrix")!);
        var mSnap = CaptureMatrix(Get(mTarget, "m_matrix")!);

        MethodComparison.Compare(caseId, Contract,
            DimMethod(Side.Quire), qTarget, [i, j],
            DimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        AssertUnchanged(qSnap);
        AssertUnchanged(mSnap);
    }

    private static void CompareMatDim(string caseId, Func<Side, object> makeTarget, int i, int j)
    {
        object qTarget = makeTarget(Side.Quire);
        object mTarget = makeTarget(Side.Mathcad);
        var qSnap = CaptureMatrix(Get(qTarget, "m_matrix")!);
        var mSnap = CaptureMatrix(Get(mTarget, "m_matrix")!);

        MethodComparison.Compare(caseId, Contract,
            MatDimMethod(Side.Quire), qTarget, [i, j],
            MatDimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        AssertUnchanged(qSnap);
        AssertUnchanged(mSnap);
    }

    [SkippableFact]
    public void Crazy_dim_coordinate_access_matches_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(DimToken, DimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(MatDimToken, MatDimMethod(Side.Mathcad).MetadataToken);

        // 1. Uniform underlying matrix (2x3 with dim="L")
        object Make2x3Uniform(Side side) => MakeCrazy(side, CreateMatrix(side, 2, 3, "L"));
        CompareDim("dim at (0, 0) on uniform matrix", Make2x3Uniform, 0, 0);
        CompareDim("dim at (1, 1) on uniform matrix", Make2x3Uniform, 1, 1);
        CompareDim("dim at (1, 2) on uniform matrix", Make2x3Uniform, 1, 2);
        CompareDim("dim out of bounds row throws IndexOutOfRangeException", Make2x3Uniform, 2, 0);
        CompareDim("dim out of bounds col throws IndexOutOfRangeException", Make2x3Uniform, 0, 3);
        CompareDim("dim negative row throws IndexOutOfRangeException", Make2x3Uniform, -1, 0);
        CompareDim("dim negative col throws IndexOutOfRangeException", Make2x3Uniform, 0, -1);

        CompareMatDim("matdim at (0, 0) on uniform matrix", Make2x3Uniform, 0, 0);
        CompareMatDim("matdim at (1, 1) on uniform matrix", Make2x3Uniform, 1, 1);
        CompareMatDim("matdim at (1, 2) on uniform matrix", Make2x3Uniform, 1, 2);
        CompareMatDim("matdim out of bounds row throws IndexOutOfRangeException", Make2x3Uniform, 2, 0);
        CompareMatDim("matdim out of bounds col throws IndexOutOfRangeException", Make2x3Uniform, 0, 3);
        CompareMatDim("matdim negative row throws IndexOutOfRangeException", Make2x3Uniform, -1, 0);
        CompareMatDim("matdim negative col throws IndexOutOfRangeException", Make2x3Uniform, 0, -1);

        // 2. Underlying matrix with nested matrix pointers (heterogeneous dimensions)
        object Make2x2Nested(Side side)
        {
            var mat = CreateMatrix(side, 2, 2, "L");
            SetNestedMatrix(side, mat, 0, 1, "T", 42.0);
            return MakeCrazy(side, mat);
        }
        CompareDim("dim at base cell (0, 0)", Make2x2Nested, 0, 0);
        CompareDim("dim at nested matrix pointer cell (0, 1)", Make2x2Nested, 0, 1);
        CompareDim("dim at second row base cell (1, 1)", Make2x2Nested, 1, 1);

        CompareMatDim("matdim at base cell (0, 0)", Make2x2Nested, 0, 0);
        CompareMatDim("matdim at nested matrix pointer cell (0, 1)", Make2x2Nested, 0, 1);
        CompareMatDim("matdim at second row base cell (1, 1)", Make2x2Nested, 1, 1);

        // 3. Singleton (1x1 matrix)
        object Make1x1(Side side) => MakeCrazy(side, CreateMatrix(side, 1, 1, "N"));
        CompareDim("dim on 1x1 matrix", Make1x1, 0, 0);
        CompareMatDim("matdim on 1x1 matrix", Make1x1, 0, 0);

        // 4. Empty (0x0 matrix)
        object Make0x0(Side side) => MakeCrazy(side, CreateMatrix(side, 0, 0, "identity"));
        CompareDim("dim on 0x0 matrix throws IndexOutOfRangeException", Make0x0, 0, 0);
        CompareMatDim("matdim on 0x0 matrix throws IndexOutOfRangeException", Make0x0, 0, 0);
    }
}
