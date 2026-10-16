using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformMMult1DifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a4;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixSnapshot(object? Uniform, object? RowArray, object? ColArray,
        object?[]? RowValues, object?[]? ColValues, object?[,] Cells, int Rows, int Cols);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("mmult1", DeclaredInstance, null, [side.T("Mcd.MatDim")], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.mmult1(MatDim)");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Uniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static object Axis(Side side, string kind, string[] dimensions, int invariantSize) =>
        Call(Nested(side.T("Mcd.MatDim"), kind), "make", Dims(side, dimensions), invariantSize)!;

    private static object? TryGet(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object? UniformValue(object target) => TryGet(target, "m_dim") ?? TryGet(target, "UniformDim");
    private static object? RowArray(object target) => TryGet(target, "m_rowdim") ?? TryGet(target, "RowDimensions");
    private static object? ColArray(object target) => TryGet(target, "m_coldim") ?? TryGet(target, "ColDimensions");

    private static MatrixSnapshot Snapshot(object target)
    {
        int rows = (int)Get(target, "Rows")!, cols = (int)Get(target, "Cols")!;
        object? rowArray = RowArray(target), colArray = ColArray(target);
        var cells = new object?[rows, cols];
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++) cells[r, c] = Call(target, "dim", r, c);
        return new MatrixSnapshot(UniformValue(target), rowArray, colArray,
            rowArray is Array ra ? ra.Cast<object?>().ToArray() : null,
            colArray is Array ca ? ca.Cast<object?>().ToArray() : null, cells, rows, cols);
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : value == null ? "null" : side.DimText(value);

    private static string MatrixText(Side side, object? value)
    {
        if (value == null) return "null";
        string text = $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" uniform={side.DimText(UniformValue(value))}" +
            $" row={ArrayText(side, RowArray(value))} col={ArrayText(side, ColArray(value))}";
        int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!;
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++) text += $" ({r},{c})={side.DimText(Call(value, "dim", r, c))}";
        return text;
    }

    private static string Show(Side side, object? value) => value switch
    {
        null => "null",
        _ when side.T("Mcd.MatDim").IsInstanceOfType(value) => MatrixText(side, value),
        _ when side.Dim.IsInstanceOfType(value) => side.DimText(value),
        _ => Text(value)
    };

    private static string State(Side side, object target, MatrixSnapshot before)
    {
        object? uniform = UniformValue(target), rows = RowArray(target), cols = ColArray(target);
        string text = $"{Get(target, "Rows")}x{Get(target, "Cols")} uniform={side.DimText(uniform)}" +
            $" uniform-ref={ReferenceEquals(uniform, before.Uniform)}" +
            $" row={ArrayText(side, rows)} row-ref={ReferenceEquals(rows, before.RowArray)}" +
            $" col={ArrayText(side, cols)} col-ref={ReferenceEquals(cols, before.ColArray)}";
        int rowCount = (int)Get(target, "Rows")!, colCount = (int)Get(target, "Cols")!;
        for (int r = 0; r < rowCount; r++)
        for (int c = 0; c < colCount; c++) text += $" ({r},{c})={side.DimText(Call(target, "dim", r, c))}";
        return text;
    }

    private static void AssertUnchanged(Side side, object target, MatrixSnapshot before)
    {
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
        Assert.Same(before.Uniform, UniformValue(target));
        object? rowArray = RowArray(target), colArray = ColArray(target);
        Assert.Same(before.RowArray, rowArray); Assert.Same(before.ColArray, colArray);
        AssertArrayUnchanged(before.RowValues, rowArray); AssertArrayUnchanged(before.ColValues, colArray);
        for (int r = 0; r < before.Rows; r++)
        for (int c = 0; c < before.Cols; c++) Assert.Same(before.Cells[r, c], Call(target, "dim", r, c));
    }

    private static void AssertArrayUnchanged(object?[]? before, object? after)
    {
        if (before == null) return;
        Assert.IsAssignableFrom<Array>(after);
        var current = (Array)after!;
        Assert.Equal(before.Length, current.Length);
        for (int i = 0; i < before.Length; i++) Assert.Same(before[i], current.GetValue(i));
    }

    private static void Compare(string caseId, Func<Side, object> rightFactory)
    {
        var qTarget = Uniform(Side.Quire, "L", 2, 3);
        var mTarget = Uniform(Side.Mathcad, "L", 2, 3);
        object? qRight = rightFactory(Side.Quire), mRight = rightFactory(Side.Mathcad);
        var qTargetBefore = Snapshot(qTarget); var mTargetBefore = Snapshot(mTarget);
        MatrixSnapshot? qRightBefore = qRight == null ? null : Snapshot(qRight);
        MatrixSnapshot? mRightBefore = mRight == null ? null : Snapshot(mRight);
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [qRight], mMethod, mTarget, [mRight],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            target => State(Side.Quire, target, qTargetBefore), target => State(Side.Mathcad, target, mTargetBefore));

        AssertUnchanged(Side.Quire, qTarget, qTargetBefore);
        AssertUnchanged(Side.Mathcad, mTarget, mTargetBefore);
        if (qRight != null) AssertUnchanged(Side.Quire, qRight, qRightBefore!);
        if (mRight != null) AssertUnchanged(Side.Mathcad, mRight, mRightBefore!);
    }

    [SkippableFact]
    public void Uniform_mmult1_dispatches_to_the_right_operand_and_preserves_both_inputs()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform mmult1 dispatches to uniform mmult2",
            side => Uniform(side, "T", 3, 4));
        Compare("uniform mmult1 dispatches to specialized column mmult2",
            side => Axis(side, "c_dim", ["T", "N"], 3));
        Compare("uniform mmult1 uses base null fallback for row mmult2",
            side => Axis(side, "r_dim", ["T", "N"], 4));
        Compare("uniform mmult1 dereferences a null right operand", _ => null!);
    }
}
