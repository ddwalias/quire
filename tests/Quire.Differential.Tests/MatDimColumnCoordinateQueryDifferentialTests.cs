using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimColumnCoordinateQueryDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007d7;
    private const int MatDimToken = 0x060007d8;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static System.Type ColumnType(Side side) => Nested(side.T("Mcd.MatDim"), "c_dim");

    private static MethodInfo Method(Side side, string name) =>
        ColumnType(side).GetMethod(name, DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.{name}(int,int)");

    private static Array Dimensions(Side side, params string?[] units)
    {
        var result = Array.CreateInstance(side.Dim, units.Length);
        for (int i = 0; i < units.Length; i++)
            result.SetValue(units[i] == null ? null : side.D(units[i]!), i);
        return result;
    }

    private static object Column(Side side, Array dimensions, int rows) =>
        Call(ColumnType(side), "make", dimensions, rows)!;

    private static string ArrayText(Side side, Array dimensions) =>
        "[" + string.Join(",", dimensions.Cast<object?>().Select(side.DimText)) + "]";

    private static string State(Side side, object target, Array retained)
    {
        var cols = (Array)Get(target, "m_coldim")!;
        return $"c_dim {Get(target, "Rows")}x{Get(target, "Cols")} cols-ref={ReferenceEquals(cols, retained)} " +
            $"stored={ArrayText(side, cols)} retained-now={ArrayText(side, retained)}";
    }

    private static string ShowDim(Side side, object? expected, object? result)
    {
        if (result == null) return "null";
        if (result.GetType() != side.Dim) return side.Show(result);
        return $"Dim {side.DimText(result)} same-selected-column-unit={ReferenceEquals(result, expected)}";
    }

    private static string ShowMatDim(Side side, object? expected, object? result)
    {
        if (result == null) return "null";
        if (!side.T("Mcd.MatDim").IsAssignableFrom(result.GetType())) return side.Show(result);
        object? unit = Get(result, "UniformDim");
        return $"{result.GetType().Name} {Get(result, "Rows")}x{Get(result, "Cols")} " +
            $"unit={side.DimText(unit)} unit-is-identity={ReferenceEquals(unit, side.Marker("identity"))} " +
            $"unit-same-selected-column={ReferenceEquals(unit, expected)}";
    }

    private static void CompareDim(string caseId, string?[] units, int rows, int i, int j)
    {
        Array qCols = Dimensions(Side.Quire, units);
        Array mCols = Dimensions(Side.Mathcad, units);
        object?[] qColSnapshot = qCols.Cast<object?>().ToArray();
        object?[] mColSnapshot = mCols.Cast<object?>().ToArray();
        object qTarget = Column(Side.Quire, qCols, rows);
        object mTarget = Column(Side.Mathcad, mCols, rows);
        object? qSelected = (uint)j < (uint)qCols.Length ? qCols.GetValue(j) : null;
        object? mSelected = (uint)j < (uint)mCols.Length ? mCols.GetValue(j) : null;
        var qMethod = Method(Side.Quire, "dim");
        var mMethod = Method(Side.Mathcad, "dim");
        Assert.Equal(DimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qCols);
        string mBefore = State(Side.Mathcad, mTarget, mCols);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowDim(Side.Quire, qSelected, result),
            result => ShowDim(Side.Mathcad, mSelected, result),
            target => State(Side.Quire, target, qCols),
            target => State(Side.Mathcad, target, mCols));

        Assert.Same(qCols, Get(qTarget, "m_coldim"));
        Assert.Same(mCols, Get(mTarget, "m_coldim"));
        for (int k = 0; k < qCols.Length; k++) Assert.Same(qColSnapshot[k], qCols.GetValue(k));
        for (int k = 0; k < mCols.Length; k++) Assert.Same(mColSnapshot[k], mCols.GetValue(k));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qCols));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mCols));
    }

    private static void CompareMatDim(string caseId, string?[] units, int rows, int i, int j)
    {
        Array qCols = Dimensions(Side.Quire, units);
        Array mCols = Dimensions(Side.Mathcad, units);
        object?[] qColSnapshot = qCols.Cast<object?>().ToArray();
        object?[] mColSnapshot = mCols.Cast<object?>().ToArray();
        object qTarget = Column(Side.Quire, qCols, rows);
        object mTarget = Column(Side.Mathcad, mCols, rows);
        object? qSelected = (uint)j < (uint)qCols.Length ? qCols.GetValue(j) : null;
        object? mSelected = (uint)j < (uint)mCols.Length ? mCols.GetValue(j) : null;
        var qMethod = Method(Side.Quire, "matdim");
        var mMethod = Method(Side.Mathcad, "matdim");
        Assert.Equal(MatDimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qCols);
        string mBefore = State(Side.Mathcad, mTarget, mCols);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowMatDim(Side.Quire, qSelected, result),
            result => ShowMatDim(Side.Mathcad, mSelected, result),
            target => State(Side.Quire, target, qCols),
            target => State(Side.Mathcad, target, mCols));

        Assert.Same(qCols, Get(qTarget, "m_coldim"));
        Assert.Same(mCols, Get(mTarget, "m_coldim"));
        for (int k = 0; k < qCols.Length; k++) Assert.Same(qColSnapshot[k], qCols.GetValue(k));
        for (int k = 0; k < mCols.Length; k++) Assert.Same(mColSnapshot[k], mCols.GetValue(k));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qCols));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mCols));
    }

    [SkippableFact]
    public void C_dim_dim_selects_by_column_only_and_propagates_invalid_columns()
    {
        Runtime.Sync();
        Assert.Equal(DimToken, Method(Side.Mathcad, "dim").MetadataToken);

        var units = new string?[] { "L", null, "T" };
        CompareDim("c_dim.dim column 0 ignores normal, negative, and extreme rows", units, 4, 0, 0);
        CompareDim("c_dim.dim column 0 ignores negative row", units, 4, -1, 0);
        CompareDim("c_dim.dim column 0 ignores high row", units, 4, 200, 0);
        CompareDim("c_dim.dim column 0 ignores extreme row", units, 4, int.MinValue, 0);
        CompareDim("c_dim.dim column 1 returns null slot", units, 4, int.MaxValue, 1);
        CompareDim("c_dim.dim column 2 selects time unit", units, 4, -100, 2);
        CompareDim("c_dim.dim negative column throws", units, 4, 0, -1);
        CompareDim("c_dim.dim column at length throws", units, 4, 0, 3);
        CompareDim("c_dim.dim empty column array throws", Array.Empty<string?>(), 4, 0, 0);
    }

    [SkippableFact]
    public void C_dim_matdim_builds_one_by_one_uniform_dimensions_and_propagates_invalid_columns()
    {
        Runtime.Sync();
        Assert.Equal(MatDimToken, Method(Side.Mathcad, "matdim").MetadataToken);

        var units = new string?[] { "L", null, "T" };
        CompareMatDim("c_dim.matdim column 0 ignores normal, negative, and extreme rows", units, 4, 0, 0);
        CompareMatDim("c_dim.matdim column 0 ignores negative row", units, 4, -1, 0);
        CompareMatDim("c_dim.matdim column 0 ignores high row", units, 4, 200, 0);
        CompareMatDim("c_dim.matdim column 0 ignores extreme row", units, 4, int.MinValue, 0);
        CompareMatDim("c_dim.matdim null column normalizes to identity", units, 4, int.MaxValue, 1);
        CompareMatDim("c_dim.matdim column 2 wraps selected unit in 1x1", units, 4, -100, 2);
        CompareMatDim("c_dim.matdim negative column throws", units, 4, 0, -1);
        CompareMatDim("c_dim.matdim column at length throws", units, 4, 0, 3);
        CompareMatDim("c_dim.matdim empty column array throws", Array.Empty<string?>(), 4, 0, 0);
    }
}
