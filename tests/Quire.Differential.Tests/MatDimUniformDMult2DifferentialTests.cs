using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformDMult2DifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a8;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixState(int Rows, int Cols, object? Dimension);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("dmult2", DeclaredInstance, null,
            [Nested(side.T("Mcd.MatDim"), "uniform_dim")], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.dmult2(uniform_dim)");

    private static object Uniform(Side side, string? dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make",
            dimension == null ? null : side.D(dimension), rows, cols)!;

    private static MatrixState Snapshot(object matrix) => new(
        (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!, Get(matrix, "m_dim"));

    private static string MatrixStateText(Side side, object target, MatrixState before) =>
        $"{Get(target, "Rows")}x{Get(target, "Cols")}" +
        $" dim={side.DimText(Get(target, "m_dim"))}" +
        $" dim-ref={ReferenceEquals(before.Dimension, Get(target, "m_dim"))}";

    private static string Show(Side side, object? value) => value switch
    {
        null => "null",
        _ when side.Dim.IsInstanceOfType(value) => side.DimText(value),
        _ when side.T("Mcd.MatDim").IsInstanceOfType(value) =>
            $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")} dim={side.DimText(Get(value, "m_dim"))}",
        _ => Text(value)
    };

    private static void AssertUnchanged(object matrix, MatrixState before)
    {
        Assert.Equal(before.Rows, Get(matrix, "Rows"));
        Assert.Equal(before.Cols, Get(matrix, "Cols"));
        Assert.Same(before.Dimension, Get(matrix, "m_dim"));
    }

    private static object? Compare(string caseId, string? receiverDimension, string? argumentDimension)
    {
        // The receiver corresponds to the right-hand uniform dimension in the
        // original DMult double-dispatch path; the argument supplies m1.m_dim.
        var qReceiver = Uniform(Side.Quire, receiverDimension, 2, 5);
        var mReceiver = Uniform(Side.Mathcad, receiverDimension, 2, 5);
        var qArgument = Uniform(Side.Quire, argumentDimension, 3, 4);
        var mArgument = Uniform(Side.Mathcad, argumentDimension, 3, 4);
        var qBefore = Snapshot(qReceiver); var mBefore = Snapshot(mReceiver);
        var qArgumentBefore = Snapshot(qArgument); var mArgumentBefore = Snapshot(mArgument);
        object qExpected = Expected(Side.Quire, qArgument, qReceiver);
        object mExpected = Expected(Side.Mathcad, mArgument, mReceiver);
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        object? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [qArgument], mMethod, mReceiver, [mArgument],
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
            target => MatrixStateText(Side.Quire, target, qBefore),
            target => MatrixStateText(Side.Mathcad, target, mBefore));

        AssertUnchanged(qReceiver, qBefore); AssertUnchanged(mReceiver, mBefore);
        AssertUnchanged(qArgument, qArgumentBefore); AssertUnchanged(mArgument, mArgumentBefore);
        if (receiverDimension == null)
        {
            Assert.Same(Side.Quire.Marker("identity"), Get(qReceiver, "m_dim"));
            Assert.Same(Side.Mathcad.Marker("identity"), Get(mReceiver, "m_dim"));
        }
        if (argumentDimension == null)
        {
            Assert.Same(Side.Quire.Marker("identity"), Get(qArgument, "m_dim"));
            Assert.Same(Side.Mathcad.Marker("identity"), Get(mArgument, "m_dim"));
        }

        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.IsAssignableFrom<global::Quire.Mcd.Dim>(qResult);
        Assert.Same(qExpected, qResult);
        Assert.Same(mExpected, mResult);
        return qResult;
    }

    private static object Expected(Side side, object argument, object receiver) =>
        CallSig(side.T("Mcd.Dim"), "Mult", ["Dim", "Dim"], Get(argument, "m_dim"), Get(receiver, "m_dim"))!;

    [SkippableFact]
    public void Uniform_dmult2_multiplies_stored_dimensions_and_leaves_shapes_untouched()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform dmult2 computes L times T", "T", "L");
        Compare("uniform dmult2 computes T times L", "L", "T");
        Compare("uniform dmult2 normalizes a null receiver dimension to identity", null, "L");
        Compare("uniform dmult2 normalizes a null argument dimension to identity", "T", null);
    }
}
