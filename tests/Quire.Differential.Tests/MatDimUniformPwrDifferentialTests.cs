using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformPwrDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007ac;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Dimension, int Rows, int Cols);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("pwr", DeclaredInstance, null,
            [typeof(uint)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.pwr(uint)");

    private static object Uniform(Side side, string? dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make",
            [dimension == null ? null : side.D(dimension), rows, cols])!;

    private static Snapshot Capture(object target) =>
        new(Get(target, "m_dim")!, (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);

    private static object ExpectedPower(Side side, object target, uint exponent) =>
        CallSig(side.T("Mcd.Dim"), "Pwr", ["Dim", "Double"], Get(target, "m_dim"), (double)exponent)!;

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
        if (!side.T("Mcd.MatDim").IsInstanceOfType(value)) return side.Show(value);
        retainResult?.Invoke(value);
        object? dimension = Get(value, "m_dim");
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" same-receiver={ReferenceEquals(value, receiver)}" +
            $" unit={side.DimText(dimension)}" +
            $" unit-same-expected={ReferenceEquals(dimension, expectedDimension)}" +
            $" receiver-unit-unchanged={ReferenceEquals(Get(receiver, "m_dim"), before.Dimension)}";
    }

    private static void Compare(string caseId, string? dimension, int rows, int cols, uint exponent)
    {
        object qReceiver = Uniform(Side.Quire, dimension, rows, cols);
        object mReceiver = Uniform(Side.Mathcad, dimension, rows, cols);
        if (dimension == null)
        {
            Assert.Same(Get(Side.Quire.T("Mcd.Dim"), "identity"), Get(qReceiver, "m_dim"));
            Assert.Same(Get(Side.Mathcad.T("Mcd.Dim"), "identity"), Get(mReceiver, "m_dim"));
        }
        var qBefore = Capture(qReceiver);
        var mBefore = Capture(mReceiver);
        object qExpectedDimension = ExpectedPower(Side.Quire, qReceiver, exponent);
        object mExpectedDimension = ExpectedPower(Side.Mathcad, mReceiver, exponent);
        object? qResult = null, mResult = null;
        var qMethod = Method(Side.Quire);
        var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [exponent], mMethod, mReceiver, [exponent],
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
        if (exponent == 1)
        {
            Assert.Same(qBefore.Dimension, qExpectedDimension);
            Assert.Same(mBefore.Dimension, mExpectedDimension);
        }
        AssertUnchanged(qReceiver, qBefore);
        AssertUnchanged(mReceiver, mBefore);
    }

    [SkippableFact]
    public void Uniform_pwr_computes_unsigned_exponents_and_preserves_receiver_shape_and_unit()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform pwr exponent zero", "L", 2, 5, 0);
        Compare("uniform pwr exponent one retains input dimension", "T", 3, 2, 1);
        Compare("uniform pwr exponent two", "N", 1, 1, 2);
        Compare("uniform pwr exponent three", "L", 2, 4, 3);
        Compare("uniform pwr normalizes null factory dimension before exponentiation", null, 3, 5, 2);
    }
}
