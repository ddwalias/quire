using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimUniformCoordinateQueryDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x0600079e;
    private const int MatDimToken = 0x0600079f;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static System.Type UniformType(Side side) => Nested(side.T("Mcd.MatDim"), "uniform_dim");

    private static MethodInfo Method(Side side, string name) =>
        UniformType(side).GetMethod(name, DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.{name}(int,int)");

    private static object Uniform(Side side, string? unit, int rows, int cols) =>
        Call(UniformType(side), "make", unit == null ? null : side.D(unit), rows, cols)!;

    private static string State(Side side, object target, object? retainedUnit)
    {
        var unit = Get(target, "m_dim");
        return $"{target.GetType().Name} {Get(target, "Rows")}x{Get(target, "Cols")} unit={side.DimText(unit)} " +
            $"unit-retained={ReferenceEquals(unit, retainedUnit)}";
    }

    private static string ShowDim(Side side, object target, object? retainedUnit, object? result)
    {
        if (result == null) return "null";
        if (result.GetType() != side.Dim) return side.Show(result);
        return $"Dim {side.DimText(result)} same-stored-unit={ReferenceEquals(result, Get(target, "m_dim"))} " +
            $"same-retained-unit={ReferenceEquals(result, retainedUnit)}";
    }

    private static string ShowMatDim(Side side, object target, object? result)
    {
        if (result == null) return "null";
        if (!side.T("Mcd.MatDim").IsAssignableFrom(result.GetType())) return side.Show(result);
        return $"{result.GetType().Name} {Get(result, "Rows")}x{Get(result, "Cols")} same-receiver={ReferenceEquals(result, target)}";
    }

    private static void CompareDim(string caseId, string? unit, int rows, int cols, int i, int j)
    {
        object qTarget = Uniform(Side.Quire, unit, rows, cols);
        object mTarget = Uniform(Side.Mathcad, unit, rows, cols);
        object? qUnit = Get(qTarget, "m_dim"), mUnit = Get(mTarget, "m_dim");
        var qMethod = Method(Side.Quire, "dim");
        var mMethod = Method(Side.Mathcad, "dim");
        Assert.Equal(DimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qUnit);
        string mBefore = State(Side.Mathcad, mTarget, mUnit);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowDim(Side.Quire, qTarget, qUnit, result),
            result => ShowDim(Side.Mathcad, mTarget, mUnit, result),
            target => State(Side.Quire, target, qUnit),
            target => State(Side.Mathcad, target, mUnit));

        Assert.Same(qUnit, Get(qTarget, "m_dim"));
        Assert.Same(mUnit, Get(mTarget, "m_dim"));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qUnit));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mUnit));
    }

    private static void CompareMatDim(string caseId, string? unit, int rows, int cols, int i, int j)
    {
        object qTarget = Uniform(Side.Quire, unit, rows, cols);
        object mTarget = Uniform(Side.Mathcad, unit, rows, cols);
        object? qUnit = Get(qTarget, "m_dim"), mUnit = Get(mTarget, "m_dim");
        var qMethod = Method(Side.Quire, "matdim");
        var mMethod = Method(Side.Mathcad, "matdim");
        Assert.Equal(MatDimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qUnit);
        string mBefore = State(Side.Mathcad, mTarget, mUnit);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowMatDim(Side.Quire, qTarget, result),
            result => ShowMatDim(Side.Mathcad, mTarget, result),
            target => State(Side.Quire, target, qUnit),
            target => State(Side.Mathcad, target, mUnit));

        Assert.Equal(qBefore, State(Side.Quire, qTarget, qUnit));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mUnit));
    }

    private static readonly (int Rows, int Cols, string? Unit)[] Shapes =
    [
        (2, 3, "L"),
        (1, 1, "T"),
        (0, 3, "poly"),
        (3, 0, "poly"),
        (0, 0, "L"),
        (2, 3, null),
    ];

    private static readonly (int I, int J)[] Coordinates =
    [
        (0, 0),
        (-1, -1),
        (100, 200),
        (int.MaxValue, int.MinValue),
    ];

    [SkippableFact]
    public void Uniform_dim_returns_stored_unit_for_every_coordinate()
    {
        Runtime.Sync();
        Assert.Equal(DimToken, Method(Side.Mathcad, "dim").MetadataToken);
        foreach (var (rows, cols, unit) in Shapes)
        foreach (var (i, j) in Coordinates)
            CompareDim($"uniform_dim.dim {rows}x{cols} {unit ?? "null-normalized"} at ({i},{j})", unit, rows, cols, i, j);
    }

    [SkippableFact]
    public void Uniform_matdim_returns_receiver_for_every_coordinate()
    {
        Runtime.Sync();
        Assert.Equal(MatDimToken, Method(Side.Mathcad, "matdim").MetadataToken);
        foreach (var (rows, cols, unit) in Shapes)
        foreach (var (i, j) in Coordinates)
            CompareMatDim($"uniform_dim.matdim {rows}x{cols} {unit ?? "null-normalized"} at ({i},{j})", unit, rows, cols, i, j);
    }
}
