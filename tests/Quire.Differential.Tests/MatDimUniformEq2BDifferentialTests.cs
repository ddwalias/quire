using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformEq2BDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a3;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "uniform_dim").GetMethod("eq2_b", DeclaredInstance, null, [side.Dim], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.eq2_b(Dim)");

    private static object Uniform(Side side, string? dimension)
    {
        var type = Nested(side.T("Mcd.MatDim"), "uniform_dim");
        return Call(type, "make", side.D(dimension ?? "poly"), 2, 3)!;
    }

    private static string Show(Side side, object? value) => value == null ? "null"
        : side.Dim.IsInstanceOfType(value) ? side.DimText(value)
        : Text(value);

    private static string State(Side side, object target, object? initialStoredDimension)
    {
        object? stored = Get(target, "m_dim");
        return $"{Get(target, "Rows")}x{Get(target, "Cols")} {side.DimText(stored)}" +
            $" stored-identity-unchanged={ReferenceEquals(stored, initialStoredDimension)}";
    }

    private static void Compare(string caseId, string? receiverDimension, string? argumentDimension)
    {
        var qTarget = Uniform(Side.Quire, receiverDimension);
        var mTarget = Uniform(Side.Mathcad, receiverDimension);
        object? qStored = Get(qTarget, "m_dim"), mStored = Get(mTarget, "m_dim");
        object? qArgument = Side.Quire.D(argumentDimension ?? "poly");
        object? mArgument = Side.Mathcad.D(argumentDimension ?? "poly");
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [qArgument], mMethod, mTarget, [mArgument],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            target => State(Side.Quire, target, qStored), target => State(Side.Mathcad, target, mStored));

        Assert.Same(qStored, Get(qTarget, "m_dim"));
        Assert.Same(mStored, Get(mTarget, "m_dim"));
        Assert.Equal(2, Get(qTarget, "Rows")); Assert.Equal(3, Get(qTarget, "Cols"));
        Assert.Equal(2, Get(mTarget, "Rows")); Assert.Equal(3, Get(mTarget, "Cols"));
    }

    [SkippableFact]
    public void Uniform_eq2_b_uses_Dim_Eq_b_without_mutating_the_receiver()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform eq2_b matches ordinary units", "L", "L");
        Compare("uniform eq2_b rejects different ordinary units", "L", "T");
        Compare("uniform eq2_b treats mcd_zero as equal to identity", "identity", "mcd_zero");
        Compare("uniform eq2_b distinguishes special marker dimensions", "string_dim", "function_dim");
        Compare("uniform eq2_b accepts polymorphic null argument", "L", null);
    }
}
