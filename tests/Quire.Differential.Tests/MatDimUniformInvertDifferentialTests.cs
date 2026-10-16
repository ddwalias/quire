using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformInvertDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007ab;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Dimension, int Rows, int Cols);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("invert", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.invert()");

    private static object Uniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static Snapshot Capture(object target) =>
        new(Get(target, "m_dim")!, (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);

    private static object ExpectedPower(Side side, object target) =>
        CallSig(side.T("Mcd.Dim"), "Pwr", ["Dim", "Double"], Get(target, "m_dim"), -1.0)!;

    private static void AssertUnchanged(object target, Snapshot before)
    {
        Assert.Same(before.Dimension, Get(target, "m_dim"));
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
    }

    private static string Render(Side side, object? value, object receiver, Snapshot before,
        object expectedDimension, Action<object>? retainResult)
    {
        if (value == null) return "null";
        retainResult?.Invoke(value);
        object? dimension = Get(value, "m_dim");
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" same-receiver={ReferenceEquals(value, receiver)}" +
            $" unit={side.DimText(dimension)}" +
            $" unit-same-expected={ReferenceEquals(dimension, expectedDimension)}" +
            $" receiver-unit-unchanged={ReferenceEquals(Get(receiver, "m_dim"), before.Dimension)}";
    }

    private static void Compare(string caseId, int rows, int cols)
    {
        object qReceiver = Uniform(Side.Quire, "L", rows, cols);
        object mReceiver = Uniform(Side.Mathcad, "L", rows, cols);
        var qBefore = Capture(qReceiver);
        var mBefore = Capture(mReceiver);
        object qExpectedDimension = ExpectedPower(Side.Quire, qReceiver);
        object mExpectedDimension = ExpectedPower(Side.Mathcad, mReceiver);
        object? qResult = null, mResult = null;
        var qMethod = Method(Side.Quire);
        var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [], mMethod, mReceiver, [],
            value => Render(Side.Quire, value, qReceiver, qBefore, qExpectedDimension, result => qResult = result),
            value => Render(Side.Mathcad, value, mReceiver, mBefore, mExpectedDimension, result => mResult = result),
            target => $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}" +
                $" receiver-dim-unchanged={ReferenceEquals(Get(target, "m_dim"), qBefore.Dimension)}",
            target => $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}" +
                $" receiver-dim-unchanged={ReferenceEquals(Get(target, "m_dim"), mBefore.Dimension)}");

        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.NotSame(qReceiver, qResult); Assert.NotSame(mReceiver, mResult);
        Assert.True(Nested(Side.Quire.T("Mcd.MatDim"), "uniform_dim").IsInstanceOfType(qResult));
        Assert.True(Nested(Side.Mathcad.T("Mcd.MatDim"), "uniform_dim").IsInstanceOfType(mResult));
        Assert.Equal(rows, Get(qResult!, "Rows")); Assert.Equal(cols, Get(qResult!, "Cols"));
        Assert.Equal(rows, Get(mResult!, "Rows")); Assert.Equal(cols, Get(mResult!, "Cols"));
        Assert.Same(qExpectedDimension, Get(qResult!, "m_dim"));
        Assert.Same(mExpectedDimension, Get(mResult!, "m_dim"));
        AssertUnchanged(qReceiver, qBefore);
        AssertUnchanged(mReceiver, mBefore);
    }

    [SkippableFact]
    public void Uniform_invert_raises_the_dimension_to_minus_one_and_preserves_shape()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform invert preserves 1x1 shape and computes inverse unit", 1, 1);
        Compare("uniform invert preserves non-square shape and computes inverse unit", 2, 5);
    }
}
