using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCrazyDimMakeDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int MakeToken = 0x060007b2;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixSnapshot(object Matrix, object? Real, object? Imag, object? Table,
        object? Dimension, object? Flags, int Rows, int Cols, int Stride, int Offset,
        double[]? RealValues, double[]? ImagValues, IReadOnlyList<(object? Key, object? Value)> Entries);

    private static System.Type Crazy(Side side) => Nested(side.T("Mcd.MatDim"), "crazy_dim");

    private static MethodInfo Method(Side side) =>
        Crazy(side).GetMethod("make", DeclaredStatic, null, [side.T("Mcd.MatVal")], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");

    private static object Matrix(Side side, int rows, int cols, int stride, int offset, double[] real,
        double[]? imag = null, string dimension = "L")
    {
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static string MatrixState(Side side, object matrix)
    {
        string Values(object? array) => array is double[] values
            ? "[" + string.Join(",", values.Select(Side.Bits)) + "]"
            : "null";
        object? tableValue = Get(matrix, "m_parr");
        object data = Get(matrix, "Data")!;
        string Table() => tableValue is Hashtable table
            ? string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => Convert.ToString(entry.Key), StringComparer.Ordinal)
                .Select(entry => $"{entry.Key}={entry.Value?.GetType().Name ?? "null"}"))
            : "null";
        return $"MatVal({Get(matrix, "Rows")}x{Get(matrix, "Cols")},stride={Get(matrix, "Anr")},offset={Get(matrix, "Offset")}," +
            $"flags={Convert.ToInt64(Get(data, "m_flags"))},dim={side.DimText(Get(matrix, "m_dim"))},real={Values(Get(matrix, "m_rarr"))}," +
            $"imag={Values(Get(matrix, "m_iarr"))},table=[{Table()}])";
    }

    private static string Show(Side side, object? value, object? input)
    {
        if (value == null) return "null";
        if (value.GetType().Name != "crazy_dim") return MatrixState(side, value);
        object? backing = Get(value, "m_matrix");
        Assert.Equal(Crazy(side), value.GetType());
        Assert.Equal(Get(input!, "Rows"), Get(value, "Rows"));
        Assert.Equal(Get(input!, "Cols"), Get(value, "Cols"));
        Assert.Same(input, backing);
        return $"{value.GetType().Name}({Get(value, "Rows")}x{Get(value, "Cols")},backing-is-input={ReferenceEquals(backing, input)};" +
            $"{MatrixState(side, backing!)})";
    }

    private static MatrixSnapshot Capture(object matrix)
    {
        object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr");
        IReadOnlyList<(object? Key, object? Value)> entries = table is Hashtable hash
            ? hash.Cast<DictionaryEntry>().Select(entry => (Key: (object?)entry.Key, Value: entry.Value)).ToArray()
            : Array.Empty<(object? Key, object? Value)>();
        return new MatrixSnapshot(matrix, real, imag, table, Get(matrix, "m_dim"),
            Get(Get(matrix, "Data")!, "m_flags"),
            (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!, (int)Get(matrix, "Anr")!,
            (int)Get(matrix, "Offset")!, real is double[] ra ? ra.ToArray() : null,
            imag is double[] ia ? ia.ToArray() : null, entries);
    }

    private static void AssertUnchanged(MatrixSnapshot before)
    {
        object matrix = before.Matrix;
        Assert.Same(before.Real, Get(matrix, "m_rarr"));
        Assert.Same(before.Imag, Get(matrix, "m_iarr"));
        Assert.Same(before.Table, Get(matrix, "m_parr"));
        Assert.Same(before.Dimension, Get(matrix, "m_dim"));
        Assert.Equal(before.Flags, Get(Get(matrix, "Data")!, "m_flags"));
        Assert.Equal(before.Rows, Get(matrix, "Rows"));
        Assert.Equal(before.Cols, Get(matrix, "Cols"));
        Assert.Equal(before.Stride, Get(matrix, "Anr"));
        Assert.Equal(before.Offset, Get(matrix, "Offset"));
        if (before.RealValues != null)
            Assert.Equal(before.RealValues.Select(Side.Bits), ((double[])Get(matrix, "m_rarr")!).Select(Side.Bits));
        if (before.ImagValues != null)
            Assert.Equal(before.ImagValues.Select(Side.Bits), ((double[])Get(matrix, "m_iarr")!).Select(Side.Bits));
        if (before.Table is Hashtable oldTable)
        {
            var current = (Hashtable)Get(matrix, "m_parr")!;
            Assert.Equal(oldTable.Count, current.Count);
            foreach (var entry in before.Entries)
            {
                Assert.True(current.ContainsKey(entry.Key!));
                Assert.Same(entry.Value, current[entry.Key!]);
            }
        }
    }

    private static void Compare(string caseId, object? qInput, object? mInput)
    {
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(MakeToken, mMethod.MetadataToken);
        MatrixSnapshot? qBefore = qInput == null ? null : Capture(qInput);
        MatrixSnapshot? mBefore = mInput == null ? null : Capture(mInput);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qInput], mMethod, null, [mInput],
            quireShow: value => Show(Side.Quire, value, qInput),
            originalShow: value => Show(Side.Mathcad, value, mInput));
        if (qBefore != null)
        {
            AssertUnchanged(qBefore);
            AssertUnchanged(mBefore!);
        }
    }

    [SkippableFact]
    public void Crazy_dimension_factory_wraps_input_identity_shape_and_null_failure()
    {
        Runtime.Sync();

        var qPadded = Matrix(Side.Quire, 2, 3, 4, 1, [99, 1, 2, 99, 3, 4, 99, 5, 6, 99, 7, 8, 99],
            [88, 0.5, 1.5, 88, 2.5, 3.5, 88, 4.5, 5.5, 88, 6.5, 7.5, 88], "T");
        var mPadded = Matrix(Side.Mathcad, 2, 3, 4, 1, [99, 1, 2, 99, 3, 4, 99, 5, 6, 99, 7, 8, 99],
            [88, 0.5, 1.5, 88, 2.5, 3.5, 88, 4.5, 5.5, 88, 6.5, 7.5, 88], "T");
        Compare("crazy_dim.make wraps non-default padded complex matrix by exact backing identity", qPadded, mPadded);

        var qEmpty = Matrix(Side.Quire, 0, 0, 0, 0, [], [], "identity");
        var mEmpty = Matrix(Side.Mathcad, 0, 0, 0, 0, [], [], "identity");
        Compare("crazy_dim.make wraps zero-area matrix and preserves zero shape", qEmpty, mEmpty);
        Compare("crazy_dim.make null follows constructor Rows dereference", null, null);
    }
}
