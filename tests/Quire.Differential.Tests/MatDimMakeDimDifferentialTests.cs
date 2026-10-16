using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimMakeDimDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x06000628;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Make(Side side) =>
        side.T("Mcd.MatDim").GetMethod("make", StaticDeclared, null, [side.T("Mcd.Dim")], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.make(Dim)");

    private static string DimensionResult(Side side, object? result, object? expectedDimension)
    {
        if (result == null) return "null";
        object? uniform = Get(result, "UniformDim");
        object? stored = Get(result, "m_dim");
        return $"{result.GetType().Name}:{Get(result, "Rows")}x{Get(result, "Cols")}" +
            $" uniform={side.DimText(uniform)} uniform-ref={(ReferenceEquals(uniform, expectedDimension) ? "expected" : "other")}" +
            $" stored-ref-uniform={ReferenceEquals(stored, uniform)}";
    }

    private static string Show(Side side, object? value, object? expectedDimension) =>
        value != null && side.T("Mcd.MatDim").IsInstanceOfType(value)
            ? DimensionResult(side, value, expectedDimension)
            : side.DimText(value);

    private static void Compare(string caseId, object? qDimension, object? mDimension,
        object? qExpected, object? mExpected)
    {
        var qMethod = Make(Side.Quire);
        var mMethod = Make(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qDimension], mMethod, null, [mDimension],
            quireShow: value => Show(Side.Quire, value, qExpected),
            originalShow: value => Show(Side.Mathcad, value, mExpected),
            quireState: null, originalState: null);
    }

    [SkippableFact]
    public void Make_from_Dim_forwards_unit_and_normalizes_null()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Make(Side.Mathcad).MetadataToken);

        var qLength = Side.Quire.D("L");
        var mLength = Side.Mathcad.D("L");
        Compare("make(Dim) retains the supplied nonnull dimension reference", qLength, mLength, qLength, mLength);

        var qIdentity = Side.Quire.D("identity");
        var mIdentity = Side.Mathcad.D("identity");
        Compare("make(Dim) normalizes null to Dim.identity", null, null, qIdentity, mIdentity);
    }
}
