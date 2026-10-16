using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformMMult2DifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a5;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixSnapshot(object? Uniform, int Rows, int Cols);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("mmult2", DeclaredInstance, null,
            [Nested(side.T("Mcd.MatDim"), "uniform_dim")], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.mmult2(uniform_dim)");

    private static object Uniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static object? TryGet(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object? UniformValue(object target) => TryGet(target, "m_dim") ?? TryGet(target, "UniformDim");

    private static MatrixSnapshot Snapshot(object target) =>
        new(UniformValue(target), (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);

    private static void AssertUnchanged(object target, MatrixSnapshot before)
    {
        Assert.Same(before.Uniform, UniformValue(target));
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
    }

    private static string MatrixText(Side side, object value) =>
        $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
        $" unit={side.DimText(UniformValue(value))}" +
        $" unit-field-alias={ReferenceEquals(UniformValue(value), TryGet(value, "m_dim"))}";

    private static string ResultText(Side side, object value, object target, object? left)
    {
        string sameTarget = ReferenceEquals(value, target) ? "same" : "new";
        string sameLeft = ReferenceEquals(value, left) ? "same" : "new";
        object? dimension = UniformValue(value);
        return $"{MatrixText(side, value)} result-target={sameTarget} result-left={sameLeft}" +
            $" unit-target={ReferenceEquals(dimension, UniformValue(target))}" +
            $" unit-left={left != null && ReferenceEquals(dimension, UniformValue(left))}";
    }

    private static string Show(Side side, object? value, object target, object? left, Action<object>? retainResult)
    {
        if (value == null) return "null";
        if (!side.T("Mcd.MatDim").IsInstanceOfType(value)) return side.Show(value);
        if (!ReferenceEquals(value, target) && !ReferenceEquals(value, left)) retainResult?.Invoke(value);
        return ResultText(side, value, target, left);
    }

    private static string State(Side side, object target, MatrixSnapshot before) =>
        $"{MatrixText(side, target)} unit-ref={ReferenceEquals(before.Uniform, UniformValue(target))}" +
        $" rows-before={before.Rows} cols-before={before.Cols}";

    private static void Compare(string caseId, object qRight, object? qLeft, object mRight, object? mLeft,
        int? expectedRows, int? expectedCols)
    {
        var qBefore = Snapshot(qRight); var mBefore = Snapshot(mRight);
        MatrixSnapshot? qLeftBefore = qLeft == null ? null : Snapshot(qLeft);
        MatrixSnapshot? mLeftBefore = mLeft == null ? null : Snapshot(mLeft);
        object? qExpectedProduct = expectedRows != null
            ? CallSig(Side.Quire.T("Mcd.Dim"), "Mult", ["Dim", "Dim"], UniformValue(qLeft!), UniformValue(qRight))
            : null;
        object? mExpectedProduct = expectedRows != null
            ? CallSig(Side.Mathcad.T("Mcd.Dim"), "Mult", ["Dim", "Dim"], UniformValue(mLeft!), UniformValue(mRight))
            : null;
        object? qResult = null, mResult = null;
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qRight, [qLeft], mMethod, mRight, [mLeft],
            value => Show(Side.Quire, value, qRight, qLeft, result => qResult = result),
            value => Show(Side.Mathcad, value, mRight, mLeft, result => mResult = result),
            target => State(Side.Quire, target, qBefore), target => State(Side.Mathcad, target, mBefore));
        AssertUnchanged(qRight, qBefore); AssertUnchanged(mRight, mBefore);
        if (qLeft != null) AssertUnchanged(qLeft, qLeftBefore!);
        if (mLeft != null) AssertUnchanged(mLeft, mLeftBefore!);
        if (expectedRows != null)
        {
            Assert.NotNull(qResult); Assert.NotNull(mResult);
            Assert.NotSame(qRight, qResult); Assert.NotSame(qLeft, qResult);
            Assert.NotSame(mRight, mResult); Assert.NotSame(mLeft, mResult);
            Assert.Equal(expectedRows, Get(qResult!, "Rows")); Assert.Equal(expectedCols, Get(qResult!, "Cols"));
            Assert.Equal(expectedRows, Get(mResult!, "Rows")); Assert.Equal(expectedCols, Get(mResult!, "Cols"));
            Assert.Same(UniformValue(qResult!), Get(qResult!, "m_dim"));
            Assert.Same(UniformValue(mResult!), Get(mResult!, "m_dim"));
            Assert.Same(qExpectedProduct, Get(qResult!, "m_dim"));
            Assert.Same(mExpectedProduct, Get(mResult!, "m_dim"));
        }
        else
        {
            Assert.Null(qResult); Assert.Null(mResult);
        }
    }

    [SkippableFact]
    public void Uniform_mmult2_multiplies_units_and_uses_left_rows_and_right_columns()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform mmult2 multiplies distinct units and returns left rows by right columns",
            Uniform(Side.Quire, "T", 3, 4), Uniform(Side.Quire, "L", 2, 3),
            Uniform(Side.Mathcad, "T", 3, 4), Uniform(Side.Mathcad, "L", 2, 3), 2, 4);
        Compare("uniform mmult2 preserves zero left row extent",
            Uniform(Side.Quire, "T", 3, 4), Uniform(Side.Quire, "L", 0, 3),
            Uniform(Side.Mathcad, "T", 3, 4), Uniform(Side.Mathcad, "L", 0, 3), 0, 4);
        Compare("uniform mmult2 preserves zero right column extent",
            Uniform(Side.Quire, "T", 3, 0), Uniform(Side.Quire, "L", 2, 3),
            Uniform(Side.Mathcad, "T", 3, 0), Uniform(Side.Mathcad, "L", 2, 3), 2, 0);
        Compare("uniform mmult2 dereferences null left argument",
            Uniform(Side.Quire, "T", 3, 4), null,
            Uniform(Side.Mathcad, "T", 3, 4), null, null, null);
    }
}
