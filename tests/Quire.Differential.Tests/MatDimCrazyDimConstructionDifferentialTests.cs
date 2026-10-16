using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCrazyDimConstructionDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int ConstructorToken = 0x060007ad;
    private const int FactoryToken = 0x060007b2;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record CrazyDimMatrixSnapshot(int Rows, int Cols, int Stride, int Offset,
        object? Dimension, object? Real, double[]? RealValues, object? Imag, double[]? ImagValues,
        object? Table, Dictionary<object, object?>? Entries, long Flags);

    private static Type CrazyType(Side side) => Nested(side.T("Mcd.MatDim"), "crazy_dim");

    private static ConstructorInfo Constructor(Side side) =>
        CrazyType(side).GetConstructor(DeclaredInstance, null, [side.T("Mcd.MatVal")], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim..ctor(MatVal)");

    private static MethodInfo Factory(Side side) =>
        CrazyType(side).GetMethod("make", DeclaredStatic, null, [side.T("Mcd.MatVal")], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");

    private static object MakeMatrix(Side side, int rows, int cols, int stride, int offset,
        double[] real, double[]? imag, Hashtable pointers, string dimension)
    {
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, pointers,
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static CrazyDimMatrixSnapshot CaptureMatrix(object matrix)
    {
        object data = Get(matrix, "Data")!;
        object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr");
        var entries = table is Hashtable pointers
            ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key, entry => entry.Value)
            : null;
        return new CrazyDimMatrixSnapshot((int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!,
            (int)Get(matrix, "Anr")!, (int)Get(matrix, "Offset")!, Get(matrix, "m_dim"),
            real, real is double[] r ? r.ToArray() : null,
            imag, imag is double[] i ? i.ToArray() : null,
            table, entries, Convert.ToInt64(Get(data, "m_flags")));
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.Show)) + "]"
        : "null";

    private static string TableText(Side side, object? value) => value is Hashtable table
        ? "[" + string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => Convert.ToString(entry.Key), StringComparer.Ordinal).Select(entry =>
            $"{entry.Key}:{side.Show(entry.Value)}")) + "]"
        : "null";

    private static string MatrixText(Side side, object matrix, CrazyDimMatrixSnapshot? before)
    {
        string text = $"MatVal {Get(matrix, "Rows")}x{Get(matrix, "Cols")} stride={Get(matrix, "Anr")} offset={Get(matrix, "Offset")}" +
            $" flags={Convert.ToInt64(Get(Get(matrix, "Data")!, "m_flags"))} dim={side.DimText(Get(matrix, "m_dim"))}" +
            $" real={ArrayText(side, Get(matrix, "m_rarr"))} imag={ArrayText(side, Get(matrix, "m_iarr"))}" +
            $" pointers={TableText(side, Get(matrix, "m_parr"))}";
        if (before != null)
            text += $" dim-ref={ReferenceEquals(before.Dimension, Get(matrix, "m_dim"))}" +
                $" real-ref={ReferenceEquals(before.Real, Get(matrix, "m_rarr"))}" +
                $" imag-ref={ReferenceEquals(before.Imag, Get(matrix, "m_iarr"))}" +
                $" table-ref={ReferenceEquals(before.Table, Get(matrix, "m_parr"))}";
        return text;
    }

    private static string Show(Side side, object? value, object? input, CrazyDimMatrixSnapshot? before)
    {
        if (value == null) return "null";
        if (side.T("Mcd.MatVal").IsInstanceOfType(value)) return MatrixText(side, value, before);
        if (CrazyType(side).IsInstanceOfType(value))
            return $"crazy_dim {Get(value, "Rows")}x{Get(value, "Cols")}" +
                $" m_matrix-same-input={ReferenceEquals(Get(value, "m_matrix"), input)}";
        return Text(value);
    }

    private static void AssertUnchanged(object matrix, CrazyDimMatrixSnapshot before)
    {
        Assert.Equal(before.Rows, Get(matrix, "Rows")); Assert.Equal(before.Cols, Get(matrix, "Cols"));
        Assert.Equal(before.Stride, Get(matrix, "Anr")); Assert.Equal(before.Offset, Get(matrix, "Offset"));
        Assert.Same(before.Dimension, Get(matrix, "m_dim"));
        Assert.Same(before.Real, Get(matrix, "m_rarr")); AssertArrayUnchanged(before.RealValues, Get(matrix, "m_rarr"));
        Assert.Same(before.Imag, Get(matrix, "m_iarr")); AssertArrayUnchanged(before.ImagValues, Get(matrix, "m_iarr"));
        Assert.Same(before.Table, Get(matrix, "m_parr"));
        Assert.Equal(before.Flags, Convert.ToInt64(Get(Get(matrix, "Data")!, "m_flags")));
        if (before.Entries == null) Assert.Null(Get(matrix, "m_parr"));
        else
        {
            var current = Assert.IsType<Hashtable>(Get(matrix, "m_parr"));
            Assert.Equal(before.Entries.Count, current.Count);
            foreach (var entry in before.Entries) Assert.Same(entry.Value, current[entry.Key]);
        }
    }

    private static void AssertArrayUnchanged(double[]? before, object? after)
    {
        if (before == null)
        {
            Assert.Null(after);
            return;
        }
        var current = Assert.IsType<double[]>(after);
        Assert.Equal(before.Length, current.Length);
        for (int i = 0; i < before.Length; i++) Assert.Equal(before[i], current[i]);
    }

    private static void CompareOne(string caseId, object? qMatrix, object? mMatrix,
        MethodBase qMethod, MethodBase mMethod, int expectedToken)
    {
        CrazyDimMatrixSnapshot? qBefore = qMatrix == null ? null : CaptureMatrix(qMatrix);
        CrazyDimMatrixSnapshot? mBefore = mMatrix == null ? null : CaptureMatrix(mMatrix);
        Assert.Equal(expectedToken, mMethod.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qMethod, null, [qMatrix], mMethod, null, [mMatrix],
            value =>
            {
                if (value != null && CrazyType(Side.Quire).IsInstanceOfType(value)) qResult = value;
                return Show(Side.Quire, value, qMatrix, qBefore);
            },
            value =>
            {
                if (value != null && CrazyType(Side.Mathcad).IsInstanceOfType(value)) mResult = value;
                return Show(Side.Mathcad, value, mMatrix, mBefore);
            });

        if (qMatrix != null)
        {
            AssertUnchanged(qMatrix, qBefore!);
            Assert.NotNull(qResult);
            Assert.Equal(qBefore!.Rows, Get(qResult!, "Rows"));
            Assert.Equal(qBefore.Cols, Get(qResult!, "Cols"));
            Assert.Same(qMatrix, Get(qResult!, "m_matrix"));
        }
        if (mMatrix != null)
        {
            AssertUnchanged(mMatrix, mBefore!);
            Assert.NotNull(mResult);
            Assert.Equal(mBefore!.Rows, Get(mResult!, "Rows"));
            Assert.Equal(mBefore.Cols, Get(mResult!, "Cols"));
            Assert.Same(mMatrix, Get(mResult!, "m_matrix"));
        }
    }

    private static void ComparePair(string caseId,
        Func<Side, object?> createMatrix)
    {
        object? qMatrix = createMatrix(Side.Quire);
        object? mMatrix = createMatrix(Side.Mathcad);

        CompareOne(caseId + " private constructor", qMatrix, mMatrix,
            Constructor(Side.Quire), Constructor(Side.Mathcad), ConstructorToken);
        CompareOne(caseId + " public make factory", qMatrix, mMatrix,
            Factory(Side.Quire), Factory(Side.Mathcad), FactoryToken);
    }

    [SkippableFact]
    public void Crazy_dimension_construction_preserves_shape_backing_matrix_and_null_failures()
    {
        Runtime.Sync();

        ComparePair("padded non-square complex matrix with pointers and dimension", side =>
        {
            const int rows = 2, cols = 3, stride = 4, offset = 1;
            int length = offset + stride * cols;
            var real = Enumerable.Range(0, length).Select(i => 10.0 + i).ToArray();
            var imag = Enumerable.Range(0, length).Select(i => 20.0 + i).ToArray();
            var pointers = new Hashtable { [7] = side.D("T") };
            return MakeMatrix(side, rows, cols, stride, offset, real, imag, pointers, "L");
        });

        ComparePair("simple 1x1 scalar real matrix", side =>
        {
            return MakeMatrix(side, 1, 1, 1, 0, [42.0], null, new Hashtable(), "T");
        });

        ComparePair("zero-area 0x0 matrix", side =>
        {
            return MakeMatrix(side, 0, 0, 0, 0, [], [], new Hashtable(), "identity");
        });

        ComparePair("rectangular column matrix", side =>
        {
            return MakeMatrix(side, 3, 1, 3, 0, [1.0, 2.0, 3.0], null, new Hashtable(), "L");
        });

        ComparePair("rectangular row matrix", side =>
        {
            return MakeMatrix(side, 1, 4, 1, 0, [10.0, 20.0, 30.0, 40.0], null, new Hashtable(), "v");
        });

        ComparePair("null matrix argument throws NullReferenceException on Rows dereference", _ => null);
    }
}
