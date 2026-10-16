using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformTransposeDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a9;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object? Dimension, int Rows, int Cols);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("transpose", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.transpose()");

    private static object Uniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static Snapshot Capture(object target) =>
        new(Get(target, "m_dim"), (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);

    private static void AssertUnchanged(object target, Snapshot before)
    {
        Assert.Same(before.Dimension, Get(target, "m_dim"));
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
    }

    private static string Render(Side side, object? value, object receiver, Snapshot before, Action<object>? retainResult)
    {
        if (value == null) return "null";
        retainResult?.Invoke(value);
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" same-receiver={ReferenceEquals(value, receiver)}" +
            $" unit={side.DimText(Get(value, "m_dim"))}" +
            $" unit-same-before={ReferenceEquals(Get(value, "m_dim"), before.Dimension)}";
    }

    private static void Compare(string caseId, int rows, int cols)
    {
        object qReceiver = Uniform(Side.Quire, "L", rows, cols);
        object mReceiver = Uniform(Side.Mathcad, "L", rows, cols);
        var qBefore = Capture(qReceiver);
        var mBefore = Capture(mReceiver);
        object? qResult = null, mResult = null;
        var qMethod = Method(Side.Quire);
        var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [], mMethod, mReceiver, [],
            value => Render(Side.Quire, value, qReceiver, qBefore, result => qResult = result),
            value => Render(Side.Mathcad, value, mReceiver, mBefore, result => mResult = result),
            target => $"{target.GetType().Name} {Get(target, "Rows")}x{Get(target, "Cols")}" +
                $" unit-same-before={ReferenceEquals(Get(target, "m_dim"), qBefore.Dimension)}",
            target => $"{target.GetType().Name} {Get(target, "Rows")}x{Get(target, "Cols")}" +
                $" unit-same-before={ReferenceEquals(Get(target, "m_dim"), mBefore.Dimension)}");

        Assert.Same(qReceiver, qResult);
        Assert.Same(mReceiver, mResult);
        Assert.IsAssignableFrom<global::Quire.Mcd.MatDim>(qResult);
        Assert.True(Side.Mathcad.T("Mcd.MatDim").IsInstanceOfType(mResult));
        AssertUnchanged(qReceiver, qBefore);
        AssertUnchanged(mReceiver, mBefore);
    }

    [SkippableFact]
    public void Uniform_transpose_returns_the_same_receiver_without_changing_its_state()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform transpose retains non-square receiver and unit reference", 2, 5);
        Compare("uniform transpose retains square receiver and unit reference", 4, 4);
    }
}
