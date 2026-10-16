using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCrazyDimConstructorDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007ad;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixSnapshot(int Rows, int Cols, int Stride, int Offset,
        object? Dimension, object? Real, double[]? RealValues, object? Imag, double[]? ImagValues,
        object? Table, Dictionary<object, object?>? Entries, long Flags);

    private static Type CrazyType(Side side) => Nested(side.T("Mcd.MatDim"), "crazy_dim");

    private static ConstructorInfo Constructor(Side side) =>
        CrazyType(side).GetConstructor(DeclaredInstance, null, [side.T("Mcd.MatVal")], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim..ctor(MatVal)");

    private static object Matrix(Side side)
    {
        const int rows = 2, cols = 3, stride = 4, offset = 1;
        int length = offset + stride * cols;
        var real = Enumerable.Range(0, length).Select(i => 10.0 + i).ToArray();
        var imag = Enumerable.Range(0, length).Select(i => 20.0 + i).ToArray();
        var pointers = new Hashtable { [7] = side.D("T") };
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, pointers,
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static MatrixSnapshot Snapshot(object matrix)
    {
        object data = Get(matrix, "Data")!;
        object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr");
        var entries = table is Hashtable pointers
            ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key, entry => entry.Value)
            : null;
        return new MatrixSnapshot((int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!,
            (int)Get(matrix, "Anr")!, (int)Get(matrix, "Offset")!, Get(matrix, "m_dim"),
            real, real is double[] r ? r.ToArray() : null,
            imag, imag is double[] i ? i.ToArray() : null,
            table, entries, Convert.ToInt64(Get(data, "m_flags")));
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.Show)) + "]"
        : "null";

    private static string TableText(Side side, object? value) => value is Hashtable table
        ? "[" + string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => entry.Key).Select(entry =>
            $"{entry.Key}:{side.Show(entry.Value)}")) + "]"
        : "null";

    private static string MatrixText(Side side, object matrix, MatrixSnapshot? before)
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

    private static string Show(Side side, object? value, object? input, MatrixSnapshot? before)
    {
        if (value == null) return "null";
        if (side.T("Mcd.MatVal").IsInstanceOfType(value)) return MatrixText(side, value, before);
        if (CrazyType(side).IsInstanceOfType(value))
            return $"crazy_dim {Get(value, "Rows")}x{Get(value, "Cols")}" +
                $" m_matrix-same-input={ReferenceEquals(Get(value, "m_matrix"), input)}";
        return Text(value);
    }

    private static void AssertUnchanged(object matrix, MatrixSnapshot before)
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

    private static void Compare(string caseId, object? qMatrix, object? mMatrix)
    {
        MatrixSnapshot? qBefore = qMatrix == null ? null : Snapshot(qMatrix);
        MatrixSnapshot? mBefore = mMatrix == null ? null : Snapshot(mMatrix);
        var qCtor = Constructor(Side.Quire); var mCtor = Constructor(Side.Mathcad);
        Assert.Equal(OriginalToken, mCtor.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qCtor, null, [qMatrix], mCtor, null, [mMatrix],
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
            Assert.Equal(qBefore!.Rows, Get(qResult!, "Rows")); Assert.Equal(qBefore.Cols, Get(qResult!, "Cols"));
            Assert.Same(qMatrix, Get(qResult!, "m_matrix"));
        }
        if (mMatrix != null)
        {
            AssertUnchanged(mMatrix, mBefore!);
            Assert.NotNull(mResult);
            Assert.Equal(mBefore!.Rows, Get(mResult!, "Rows")); Assert.Equal(mBefore.Cols, Get(mResult!, "Cols"));
            Assert.Same(mMatrix, Get(mResult!, "m_matrix"));
        }
    }

    [SkippableFact]
    public void Crazy_dimension_constructor_copies_shape_and_retains_the_source_matrix()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Constructor(Side.Mathcad).MetadataToken);

        Compare("crazy_dim constructor retains a padded non-square matrix", Matrix(Side.Quire), Matrix(Side.Mathcad));
        Compare("crazy_dim constructor reads Rows from null before constructing", null, null);
    }
}
