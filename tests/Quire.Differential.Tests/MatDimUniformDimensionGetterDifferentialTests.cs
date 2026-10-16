using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimUniformDimensionGetterDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int GetterToken = 0x060007a0;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static System.Type UniformType(Side side) => Nested(side.T("Mcd.MatDim"), "uniform_dim");

    private static MethodInfo Getter(Side side) =>
        UniformType(side).GetMethod("get_UniformDim", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.get_UniformDim()");

    private static object Uniform(Side side, string? unit, int rows, int cols) =>
        Call(UniformType(side), "make", unit == null ? null : side.D(unit), rows, cols)!;

    private static string State(Side side, object target, object? retainedUnit)
    {
        object? stored = Get(target, "m_dim");
        return $"uniform_dim {Get(target, "Rows")}x{Get(target, "Cols")} " +
            $"m_dim={side.DimText(stored)} matches-retained={ReferenceEquals(stored, retainedUnit)} " +
            $"is-identity={ReferenceEquals(stored, side.Marker("identity"))}";
    }

    private static string Show(Side side, object target, object? retainedUnit, object? result)
    {
        if (result == null) return "null";
        if (result.GetType() != side.Dim) return side.Show(result);
        return $"Dim {side.DimText(result)} same-stored={ReferenceEquals(result, Get(target, "m_dim"))} " +
            $"same-retained={ReferenceEquals(result, retainedUnit)} is-identity={ReferenceEquals(result, side.Marker("identity"))}";
    }

    private static void Compare(string caseId, string? unit, int rows, int cols)
    {
        object qTarget = Uniform(Side.Quire, unit, rows, cols);
        object mTarget = Uniform(Side.Mathcad, unit, rows, cols);
        object? qUnit = Get(qTarget, "m_dim");
        object? mUnit = Get(mTarget, "m_dim");
        var qGetter = Getter(Side.Quire);
        var mGetter = Getter(Side.Mathcad);
        Assert.Equal(GetterToken, mGetter.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mGetter.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qUnit);
        string mBefore = State(Side.Mathcad, mTarget, mUnit);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qGetter, qTarget, [], mGetter, mTarget, [],
            result => Show(Side.Quire, qTarget, qUnit, result),
            result => Show(Side.Mathcad, mTarget, mUnit, result),
            target => State(Side.Quire, target, qUnit),
            target => State(Side.Mathcad, target, mUnit));

        Assert.Same(qUnit, Get(qTarget, "m_dim"));
        Assert.Same(mUnit, Get(mTarget, "m_dim"));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qUnit));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mUnit));
    }

    [SkippableFact]
    public void UniformDim_returns_the_stored_dimension_reference()
    {
        Runtime.Sync();
        Assert.Equal(GetterToken, Getter(Side.Mathcad).MetadataToken);

        Compare("uniform_dim.UniformDim returns existing length dimension", "L", 2, 3);
        Compare("uniform_dim.UniformDim returns existing velocity dimension", "v", 1, 1);
        Compare("uniform_dim.UniformDim returns existing identity dimension", "identity", 0, 2);
        Compare("uniform_dim.UniformDim returns identity normalized from null", null, 2, 0);
    }
}
