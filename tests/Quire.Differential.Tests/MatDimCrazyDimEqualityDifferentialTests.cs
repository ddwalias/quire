using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCrazyDimEqualityDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int EqToken = 0x060007b0;
    private const int Eq2Token = 0x060007b1;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private sealed record CrazyDimEqualitySnapshot(object Matrix, object? Real, object? Imag, object? Table,
        object? Dimension, object? Flags, int Rows, int Cols, int Stride, int Offset, double[]? RealValues,
        double[]? ImagValues, IReadOnlyList<(object? Key, object? Value)> Entries,
        IReadOnlyList<CrazyDimEqualitySnapshot> Children);

    private static System.Type Crazy(Side side) => Nested(side.T("Mcd.MatDim"), "crazy_dim");

    private static object Matrix(Side side, int rows, int cols, string dimension = "L", double valueStart = 1)
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

    private static object CrazyMatrix(Side side, int rows, int cols, string dimension = "L", double valueStart = 1) =>
        MakeCrazy(side, Matrix(side, rows, cols, dimension, valueStart));

    private static void SetNestedMatrix(Side side, object outer, int row, int col, string dimension, double value)
    {
        var child = Matrix(side, 1, 1, dimension);
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

    private static object MakeUniform(Side side, string dimension, int rows = 1, int cols = 1)
    {
        var type = Nested(side.T("Mcd.MatDim"), "uniform_dim");
        var method = type.GetMethod("make", BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.DeclaredOnly, null,
            [side.Dim, typeof(int), typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} uniform_dim.make(Dim,int,int)");
        return method.Invoke(null, [side.D(dimension), rows, cols])!;
    }

    private static string CrazyState(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().Name != "crazy_dim") return side.Show(value);
        var matrix = Get(value, "m_matrix")!;
        return $"{value.GetType().Name}({Get(value, "Rows")}x{Get(value, "Cols")};{MatValState(side, matrix)})";
    }

    private static CrazyDimEqualitySnapshot CaptureMatrix(object matrix)
    {
        object? real = Get(matrix, "m_rarr"), imag = Get(matrix, "m_iarr"), table = Get(matrix, "m_parr");
        IReadOnlyList<(object? Key, object? Value)> entries = table is Hashtable hash
            ? hash.Cast<DictionaryEntry>().Select(entry => (Key: (object?)entry.Key, Value: entry.Value)).ToArray()
            : Array.Empty<(object? Key, object? Value)>();
        var children = entries.Select(entry => entry.Value).Where(value => value?.GetType().Name == "MatVal")
            .Select(value => CaptureMatrix(value!)).ToArray();
        return new CrazyDimEqualitySnapshot(matrix, real, imag, table, Get(matrix, "m_dim"),
            Get(Get(matrix, "Data")!, "m_flags"),
            (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!, (int)Get(matrix, "Anr")!,
            (int)Get(matrix, "Offset")!, real is double[] ra ? ra.ToArray() : null,
            imag is double[] ia ? ia.ToArray() : null, entries, children);
    }

    private static void AssertUnchanged(CrazyDimEqualitySnapshot before)
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
        foreach (var child in before.Children) AssertUnchanged(child);
    }

    private static MethodInfo Method(Side side, string name, System.Type parameter)
    {
        var type = Crazy(side);
        var mapped = name == "eq_b" ? side.T("Mcd.MatDim") : Crazy(side);
        return type.GetMethod(name, DeclaredInstance, null, [mapped], null)
            ?? throw new HarnessException($"Missing {side.Name} crazy_dim.{name}");
    }

    private static void Compare(string caseId, string name, int token, object? qLeft, object? qRight,
        object? mLeft, object? mRight)
    {
        System.Type parameter = name == "eq_b" ? typeof(global::Quire.Mcd.MatDim) : Crazy(Side.Quire);
        object? qTarget = name == "eq_b" ? qLeft : qRight;
        object? qArgument = name == "eq_b" ? qRight : qLeft;
        object? mTarget = name == "eq_b" ? mLeft : mRight;
        object? mArgument = name == "eq_b" ? mRight : mLeft;
        var qLeftBefore = qLeft != null && qLeft.GetType().Name == "crazy_dim" ? CaptureMatrix(Get(qLeft, "m_matrix")!) : null;
        var qRightBefore = qRight != null && qRight.GetType().Name == "crazy_dim" ? CaptureMatrix(Get(qRight, "m_matrix")!) : null;
        var mLeftBefore = mLeft != null && mLeft.GetType().Name == "crazy_dim" ? CaptureMatrix(Get(mLeft, "m_matrix")!) : null;
        var mRightBefore = mRight != null && mRight.GetType().Name == "crazy_dim" ? CaptureMatrix(Get(mRight, "m_matrix")!) : null;
        var qMethod = Method(Side.Quire, name, parameter);
        var mMethod = Method(Side.Mathcad, name, parameter);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [qArgument], mMethod, mTarget, [mArgument],
            quireShow: value => value == null ? "null" : CrazyState(Side.Quire, value),
            originalShow: value => value == null ? "null" : CrazyState(Side.Mathcad, value),
            quireState: target => CrazyState(Side.Quire, target),
            originalState: target => CrazyState(Side.Mathcad, target));
        if (qLeftBefore != null)
        {
            Assert.Same(Get(qLeft!, "m_matrix"), qLeftBefore.Matrix);
            AssertUnchanged(qLeftBefore);
        }
        if (qRightBefore != null)
        {
            Assert.Same(Get(qRight!, "m_matrix"), qRightBefore.Matrix);
            AssertUnchanged(qRightBefore);
        }
        if (mLeftBefore != null)
        {
            Assert.Same(Get(mLeft!, "m_matrix"), mLeftBefore.Matrix);
            AssertUnchanged(mLeftBefore);
        }
        if (mRightBefore != null)
        {
            Assert.Same(Get(mRight!, "m_matrix"), mRightBefore.Matrix);
            AssertUnchanged(mRightBefore);
        }
    }

    [SkippableFact]
    public void Crazy_dimension_double_dispatch_compares_cell_dimensions_without_mutating_matrices()
    {
        Runtime.Sync();

        void Both(string caseId, Func<Side, object> left, Func<Side, object> right)
        {
            object qLeft = left(Side.Quire), qRight = right(Side.Quire);
            object mLeft = left(Side.Mathcad), mRight = right(Side.Mathcad);
            Assert.NotSame(Get(qLeft, "m_matrix"), Get(qRight, "m_matrix"));
            Assert.NotSame(Get(mLeft, "m_matrix"), Get(mRight, "m_matrix"));
            Compare(caseId + " via eq_b", "eq_b", EqToken, qLeft, qRight, mLeft, mRight);
            Compare(caseId + " via eq2_b", "eq2_b", Eq2Token, qLeft, qRight, mLeft, mRight);
        }

        Both("equal shape and cell dimensions despite distinct backing values and references",
            side => CrazyMatrix(side, 2, 2, "L", 1), side => CrazyMatrix(side, 2, 2, "L", 10));
        Both("unequal shape returns false before cell comparison",
            side => CrazyMatrix(side, 1, 2, "L"), side => CrazyMatrix(side, 2, 1, "L"));
        Both("unequal rows same columns returns false before cell comparison",
            side => CrazyMatrix(side, 2, 2, "L"), side => CrazyMatrix(side, 3, 2, "L"));
        Both("same rows unequal columns returns false before cell comparison",
            side => CrazyMatrix(side, 2, 2, "L"), side => CrazyMatrix(side, 2, 3, "L"));

        Both("first cell dimension mismatch returns false",
            side =>
            {
                var outer = Matrix(side, 2, 2, "identity");
                SetNestedMatrix(side, outer, 0, 0, "L", 1);
                return MakeCrazy(side, outer);
            },
            side =>
            {
                var outer = Matrix(side, 2, 2, "identity");
                SetNestedMatrix(side, outer, 0, 0, "T", 1);
                return MakeCrazy(side, outer);
            });

        Both("later row-major cell dimension mismatch returns false after equal first cell",
            side =>
            {
                var outer = Matrix(side, 2, 2, "identity");
                SetNestedMatrix(side, outer, 0, 0, "L", 1);
                SetNestedMatrix(side, outer, 1, 1, "L", 2);
                return MakeCrazy(side, outer);
            },
            side =>
            {
                var outer = Matrix(side, 2, 2, "identity");
                SetNestedMatrix(side, outer, 0, 0, "L", 9);
                SetNestedMatrix(side, outer, 1, 1, "T", 2);
                return MakeCrazy(side, outer);
            });

        Both("equal zero-area shapes have no cell dimensions to compare",
            side => CrazyMatrix(side, 0, 0), side => CrazyMatrix(side, 0, 0));

        var qCrazy = CrazyMatrix(Side.Quire, 2, 2, "L");
        var mCrazy = CrazyMatrix(Side.Mathcad, 2, 2, "L");
        Compare("non-crazy uniform RHS false double-dispatch", "eq_b", EqToken,
            qCrazy, MakeUniform(Side.Quire, "L", 2, 2), mCrazy, MakeUniform(Side.Mathcad, "L", 2, 2));
        Compare("null RHS throws NullReferenceException on eq_b", "eq_b", EqToken,
            qCrazy, null, mCrazy, null);
    }
}
