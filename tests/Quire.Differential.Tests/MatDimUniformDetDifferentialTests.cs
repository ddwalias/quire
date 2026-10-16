using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformDetDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007aa;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixState(int Rows, int Cols, object? Dimension);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("det", DeclaredInstance, null,
            [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.det(int)");

    private static object Uniform(Side side, string? dimension, int rows = 2, int cols = 5) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make",
            dimension == null ? null : side.D(dimension), rows, cols)!;

    private static MatrixState Snapshot(object matrix) => new(
        (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!, Get(matrix, "m_dim"));

    private static string Show(Side side, object? value) => value switch
    {
        null => "null",
        _ when side.Dim.IsInstanceOfType(value) => side.DimText(value),
        _ when side.T("Mcd.MatDim").IsInstanceOfType(value) =>
            $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")} dim={side.DimText(Get(value, "m_dim"))}",
        _ => Text(value)
    };

    private static string State(Side side, object target, MatrixState before) =>
        $"{Get(target, "Rows")}x{Get(target, "Cols")}" +
        $" dim={side.DimText(Get(target, "m_dim"))}" +
        $" dim-ref={ReferenceEquals(before.Dimension, Get(target, "m_dim"))}";

    private static void AssertUnchanged(object matrix, MatrixState before)
    {
        Assert.Equal(before.Rows, Get(matrix, "Rows"));
        Assert.Equal(before.Cols, Get(matrix, "Cols"));
        Assert.Same(before.Dimension, Get(matrix, "m_dim"));
    }

    private static object Expected(Side side, object matrix, int exponent) =>
        CallSig(side.T("Mcd.Dim"), "Pwr", ["Dim", "Double"], Get(matrix, "m_dim"), (double)exponent)!;

    private static void Compare(string caseId, string? dimension, int exponent)
    {
        var qMatrix = Uniform(Side.Quire, dimension);
        var mMatrix = Uniform(Side.Mathcad, dimension);
        var qBefore = Snapshot(qMatrix); var mBefore = Snapshot(mMatrix);
        object qExpected = Expected(Side.Quire, qMatrix, exponent);
        object mExpected = Expected(Side.Mathcad, mMatrix, exponent);
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        object? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, qMethod, qMatrix, [exponent], mMethod, mMatrix, [exponent],
            value =>
            {
                if (value != null && Side.Quire.Dim.IsInstanceOfType(value)) qResult = value;
                return Show(Side.Quire, value);
            },
            value =>
            {
                if (value != null && Side.Mathcad.Dim.IsInstanceOfType(value)) mResult = value;
                return Show(Side.Mathcad, value);
            },
            target => State(Side.Quire, target, qBefore),
            target => State(Side.Mathcad, target, mBefore));

        AssertUnchanged(qMatrix, qBefore); AssertUnchanged(mMatrix, mBefore);
        if (dimension == null)
        {
            Assert.Same(Side.Quire.Marker("identity"), Get(qMatrix, "m_dim"));
            Assert.Same(Side.Mathcad.Marker("identity"), Get(mMatrix, "m_dim"));
        }
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.Same(qExpected, qResult);
        Assert.Same(mExpected, mResult);
    }

    [SkippableFact]
    public void Uniform_det_returns_the_stored_unit_to_the_requested_power_without_mutation()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        foreach (int exponent in new[] { -2, 0, 1, 3 })
            Compare($"uniform L determinant exponent {exponent}", "L", exponent);
        foreach (int exponent in new[] { -2, 0 })
            Compare($"factory-null uniform determinant exponent {exponent}", null, exponent);
    }
}
