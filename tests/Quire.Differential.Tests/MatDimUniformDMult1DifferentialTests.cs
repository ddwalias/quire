using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformDMult1DifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a7;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixState(int Rows, int Cols, object? Uniform, object? RowArray, object? ColArray,
        object?[]? RowValues, object?[]? ColValues, MatValState? Nested);
    private sealed record MatValState(object Matrix, int Rows, int Cols, int Offset, object? Dimension,
        object? Real, object? Imag, object? Table, double[]? RealValues, double[]? ImagValues,
        Dictionary<int, object?>? Pointers);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("dmult1", DeclaredInstance, null,
            [side.T("Mcd.MatDim")], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.dmult1(MatDim)");

    private static Array DimArray(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Uniform(Side side, string dimension, int rows = 2, int cols = 3) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static object Axis(Side side, string kind, string[] dimensions, int invariantSize) =>
        Call(Nested(side.T("Mcd.MatDim"), kind), "make", DimArray(side, dimensions), invariantSize)!;

    private static object Rc(Side side, string[] rows, string[] cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "rc_dim"), "make_rc", DimArray(side, rows), DimArray(side, cols))!;

    private static object Crazy(Side side)
    {
        object matrix = New(side.T("Mcd.MatVal"), 2, 2);
        Set(matrix, "m_dim", side.D("L"));
        var type = Nested(side.T("Mcd.MatDim"), "crazy_dim");
        var make = type.GetMethod("make", DeclaredStatic, null, [side.T("Mcd.MatVal")], null)
            ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");
        return make.Invoke(null, [matrix])!;
    }

    private static object? TryGet(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object? UniformValue(object target) => TryGet(target, "m_dim") ?? TryGet(target, "UniformDim");
    private static object? RowArray(object target) => TryGet(target, "m_rowdim") ?? TryGet(target, "RowDimensions");
    private static object? ColArray(object target) => TryGet(target, "m_coldim") ?? TryGet(target, "ColDimensions");

    private static MatValState? SnapshotMatVal(object? matrix)
    {
        if (matrix == null) return null;
        object? real = TryGet(matrix, "m_rarr"), imag = TryGet(matrix, "m_iarr"), table = TryGet(matrix, "m_parr");
        return new MatValState(matrix, (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!,
            (int)Get(matrix, "m_offset")!, TryGet(matrix, "m_dim"), real, imag, table,
            real is double[] r ? r.ToArray() : null, imag is double[] i ? i.ToArray() : null,
            table is Hashtable pointers
                ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
                : null);
    }

    private static MatrixState Snapshot(object value)
    {
        object? row = RowArray(value), col = ColArray(value);
        var nested = SnapshotMatVal(TryGet(value, "m_matrix"));
        return new MatrixState((int)Get(value, "Rows")!, (int)Get(value, "Cols")!, UniformValue(value), row, col,
            row is Array rows ? rows.Cast<object?>().ToArray() : null,
            col is Array cols ? cols.Cast<object?>().ToArray() : null,
            nested);
    }

    private static void AssertUnchanged(object value, MatrixState before)
    {
        Assert.Equal(before.Rows, Get(value, "Rows")); Assert.Equal(before.Cols, Get(value, "Cols"));
        Assert.Same(before.Uniform, UniformValue(value));
        object? row = RowArray(value), col = ColArray(value);
        Assert.Same(before.RowArray, row); Assert.Same(before.ColArray, col);
        AssertArrayUnchanged(before.RowValues, row); AssertArrayUnchanged(before.ColValues, col);
        AssertMatValUnchanged(before.Nested, SnapshotMatVal(TryGet(value, "m_matrix")));
    }

    private static void AssertMatValUnchanged(MatValState? before, MatValState? after)
    {
        if (before == null)
        {
            Assert.Null(after);
            return;
        }
        Assert.NotNull(after);
        Assert.Same(before.Matrix, after!.Matrix);
        Assert.Equal(before.Rows, after.Rows); Assert.Equal(before.Cols, after.Cols); Assert.Equal(before.Offset, after.Offset);
        Assert.Same(before.Dimension, after.Dimension); Assert.Same(before.Real, after.Real); Assert.Same(before.Imag, after.Imag);
        Assert.Same(before.Table, after.Table);
        AssertDoubleArrayUnchanged(before.RealValues, after.RealValues); AssertDoubleArrayUnchanged(before.ImagValues, after.ImagValues);
        if (before.Pointers == null) Assert.Null(after.Pointers);
        else
        {
            Assert.NotNull(after.Pointers); Assert.Equal(before.Pointers.Count, after.Pointers!.Count);
            foreach (var pair in before.Pointers) Assert.Same(pair.Value, after.Pointers[pair.Key]);
        }
    }

    private static void AssertDoubleArrayUnchanged(double[]? before, double[]? after)
    {
        if (before == null)
        {
            Assert.Null(after);
            return;
        }
        Assert.NotNull(after); Assert.Equal(before.Length, after!.Length);
        for (int i = 0; i < before.Length; i++) Assert.Equal(before[i], after[i]);
    }

    private static void AssertArrayUnchanged(object?[]? before, object? after)
    {
        if (before == null) return;
        Assert.IsAssignableFrom<Array>(after);
        var current = (Array)after!;
        Assert.Equal(before.Length, current.Length);
        for (int i = 0; i < before.Length; i++) Assert.Same(before[i], current.GetValue(i));
    }

    private static string MatValText(MatValState state) =>
        $"{state.Rows}x{state.Cols} off={state.Offset} dim={state.Dimension != null}" +
        $" real=[{(state.RealValues == null ? "null" : string.Join(",", state.RealValues.Select(x => x.ToString("R", System.Globalization.CultureInfo.InvariantCulture))))}]" +
        $" imag=[{(state.ImagValues == null ? "null" : string.Join(",", state.ImagValues.Select(x => x.ToString("R", System.Globalization.CultureInfo.InvariantCulture))))}]" +
        $" ptrs={(state.Pointers == null ? "null" : string.Join(";", state.Pointers.OrderBy(p => p.Key).Select(p => p.Key)))}";

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : value == null ? "null" : side.DimText(value);

    private static string MatrixText(Side side, object value)
    {
        object? matrix = TryGet(value, "m_matrix");
        string text = $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" uniform={side.DimText(UniformValue(value))} row={ArrayText(side, RowArray(value))}" +
            $" col={ArrayText(side, ColArray(value))} matrix={(matrix == null ? "null" : $"{Get(matrix, "Rows")}x{Get(matrix, "Cols")}")}";
        int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!;
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++) text += $" ({r},{c})={side.DimText(Call(value, "dim", r, c))}";
        return text;
    }

    private static string Show(Side side, object? value, object left, object? right, Action<object>? retainResult)
    {
        if (value == null) return "null";
        if (side.Dim.IsInstanceOfType(value))
        {
            retainResult?.Invoke(value);
            return $"{side.DimText(value)} left-unit={ReferenceEquals(value, UniformValue(left))}" +
                $" right-unit={right != null && ReferenceEquals(value, UniformValue(right))}";
        }
        if (side.T("Mcd.MatDim").IsInstanceOfType(value)) return MatrixText(side, value);
        return side.Show(value);
    }

    private static string State(Side side, object target, MatrixState before) =>
        $"{MatrixText(side, target)} uniform-ref={ReferenceEquals(before.Uniform, UniformValue(target))}" +
        $" row-ref={ReferenceEquals(before.RowArray, RowArray(target))} col-ref={ReferenceEquals(before.ColArray, ColArray(target))}" +
        $" nested-state={(before.Nested == null ? "null" : MatValText(before.Nested))}";

    private static void Compare(string caseId, object qLeft, object? qRight, object mLeft, object? mRight,
        bool expectDimension)
    {
        var qBefore = Snapshot(qLeft); var mBefore = Snapshot(mLeft);
        MatrixState? qRightBefore = qRight == null ? null : Snapshot(qRight);
        MatrixState? mRightBefore = mRight == null ? null : Snapshot(mRight);
        object? qExpectedProduct = expectDimension
            ? CallSig(Side.Quire.T("Mcd.Dim"), "Mult", ["Dim", "Dim"], UniformValue(qLeft), UniformValue(qRight!))
            : null;
        object? mExpectedProduct = expectDimension
            ? CallSig(Side.Mathcad.T("Mcd.Dim"), "Mult", ["Dim", "Dim"], UniformValue(mLeft), UniformValue(mRight!))
            : null;
        object? qResult = null, mResult = null;
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => Show(Side.Quire, value, qLeft, qRight, result => qResult = result),
            value => Show(Side.Mathcad, value, mLeft, mRight, result => mResult = result),
            target => State(Side.Quire, target, qBefore), target => State(Side.Mathcad, target, mBefore));
        AssertUnchanged(qLeft, qBefore); AssertUnchanged(mLeft, mBefore);
        if (qRight != null) AssertUnchanged(qRight, qRightBefore!);
        if (mRight != null) AssertUnchanged(mRight, mRightBefore!);
        if (expectDimension)
        {
            Assert.NotNull(qResult); Assert.NotNull(mResult);
            Assert.IsAssignableFrom<global::Quire.Mcd.Dim>(qResult);
            Assert.True(Side.Mathcad.Dim.IsInstanceOfType(mResult));
            Assert.Same(qExpectedProduct, qResult);
            Assert.Same(mExpectedProduct, mResult);
            Assert.Equal(Side.Quire.DimText(qExpectedProduct), Side.Quire.DimText(qResult));
            Assert.Equal(Side.Mathcad.DimText(mExpectedProduct), Side.Mathcad.DimText(mResult));
            Assert.NotSame(UniformValue(qLeft), qResult); Assert.NotSame(UniformValue(qRight!), qResult);
            Assert.NotSame(UniformValue(mLeft), mResult); Assert.NotSame(UniformValue(mRight!), mResult);
        }
        else
        {
            Assert.Null(qResult); Assert.Null(mResult);
        }
    }

    [SkippableFact]
    public void Uniform_dmult1_delegates_to_the_rhs_uniform_dot_product_overload()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform dmult1 dispatches to uniform dmult2 and multiplies units",
            Uniform(Side.Quire, "L", 2, 3), Uniform(Side.Quire, "T", 3, 4),
            Uniform(Side.Mathcad, "L", 2, 3), Uniform(Side.Mathcad, "T", 3, 4), true);
        Compare("uniform dmult1 dispatches to row RHS base-null fallback",
            Uniform(Side.Quire, "L"), Axis(Side.Quire, "r_dim", ["T", "T"], 3),
            Uniform(Side.Mathcad, "L"), Axis(Side.Mathcad, "r_dim", ["T", "T"], 3), false);
        Compare("uniform dmult1 dispatches to column RHS base-null fallback",
            Uniform(Side.Quire, "L"), Axis(Side.Quire, "c_dim", ["T", "T"], 3),
            Uniform(Side.Mathcad, "L"), Axis(Side.Mathcad, "c_dim", ["T", "T"], 3), false);
        Compare("uniform dmult1 dispatches to row-column RHS base-null fallback",
            Uniform(Side.Quire, "L"), Rc(Side.Quire, ["T", "T"], ["N", "N"]),
            Uniform(Side.Mathcad, "L"), Rc(Side.Mathcad, ["T", "T"], ["N", "N"]), false);
        Compare("uniform dmult1 dispatches to crazy RHS base-null fallback",
            Uniform(Side.Quire, "L"), Crazy(Side.Quire),
            Uniform(Side.Mathcad, "L"), Crazy(Side.Mathcad), false);
        Compare("uniform dmult1 calls virtual dispatch on null RHS",
            Uniform(Side.Quire, "L"), null,
            Uniform(Side.Mathcad, "L"), null, false);
    }
}
