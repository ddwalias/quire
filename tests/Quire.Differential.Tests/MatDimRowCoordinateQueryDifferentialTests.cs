using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimRowCoordinateQueryDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007c5;
    private const int MatDimToken = 0x060007c6;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static System.Type RowType(Side side) => Nested(side.T("Mcd.MatDim"), "r_dim");

    private static MethodInfo Method(Side side, string name) =>
        RowType(side).GetMethod(name, DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.{name}(int,int)");

    private static Array Dimensions(Side side, params string?[] units)
    {
        var result = Array.CreateInstance(side.Dim, units.Length);
        for (int i = 0; i < units.Length; i++)
            result.SetValue(units[i] == null ? null : side.D(units[i]!), i);
        return result;
    }

    private static object Row(Side side, Array dimensions, int cols) =>
        Call(RowType(side), "make", dimensions, cols)!;

    private static string ArrayText(Side side, Array dimensions) =>
        "[" + string.Join(",", dimensions.Cast<object?>().Select(side.DimText)) + "]";

    private static string State(Side side, object target, Array retained)
    {
        var rows = (Array)Get(target, "m_rowdim")!;
        return $"r_dim {Get(target, "Rows")}x{Get(target, "Cols")} rows-ref={ReferenceEquals(rows, retained)} " +
            $"stored={ArrayText(side, rows)} retained-now={ArrayText(side, retained)}";
    }

    private static string ShowDim(Side side, object? expected, object? result)
    {
        if (result == null) return "null";
        if (result.GetType() != side.Dim) return side.Show(result);
        return $"Dim {side.DimText(result)} same-selected-row-unit={ReferenceEquals(result, expected)}";
    }

    private static string ShowMatDim(Side side, object? expected, object? result)
    {
        if (result == null) return "null";
        if (!side.T("Mcd.MatDim").IsAssignableFrom(result.GetType())) return side.Show(result);
        object? unit = Get(result, "UniformDim");
        return $"{result.GetType().Name} {Get(result, "Rows")}x{Get(result, "Cols")} " +
            $"unit={side.DimText(unit)} unit-is-identity={ReferenceEquals(unit, side.Marker("identity"))} " +
            $"unit-same-selected-row={ReferenceEquals(unit, expected)}";
    }

    private static void CompareDim(string caseId, string?[] units, int cols, int i, int j)
    {
        Array qRows = Dimensions(Side.Quire, units);
        Array mRows = Dimensions(Side.Mathcad, units);
        object?[] qRowSnapshot = qRows.Cast<object?>().ToArray();
        object?[] mRowSnapshot = mRows.Cast<object?>().ToArray();
        object qTarget = Row(Side.Quire, qRows, cols);
        object mTarget = Row(Side.Mathcad, mRows, cols);
        object? qSelected = (uint)i < (uint)qRows.Length ? qRows.GetValue(i) : null;
        object? mSelected = (uint)i < (uint)mRows.Length ? mRows.GetValue(i) : null;
        var qMethod = Method(Side.Quire, "dim");
        var mMethod = Method(Side.Mathcad, "dim");
        Assert.Equal(DimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qRows);
        string mBefore = State(Side.Mathcad, mTarget, mRows);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowDim(Side.Quire, qSelected, result),
            result => ShowDim(Side.Mathcad, mSelected, result),
            target => State(Side.Quire, target, qRows),
            target => State(Side.Mathcad, target, mRows));

        Assert.Same(qRows, Get(qTarget, "m_rowdim"));
        Assert.Same(mRows, Get(mTarget, "m_rowdim"));
        for (int k = 0; k < qRows.Length; k++) Assert.Same(qRowSnapshot[k], qRows.GetValue(k));
        for (int k = 0; k < mRows.Length; k++) Assert.Same(mRowSnapshot[k], mRows.GetValue(k));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qRows));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mRows));
    }

    private static void CompareMatDim(string caseId, string?[] units, int cols, int i, int j)
    {
        Array qRows = Dimensions(Side.Quire, units);
        Array mRows = Dimensions(Side.Mathcad, units);
        object?[] qRowSnapshot = qRows.Cast<object?>().ToArray();
        object?[] mRowSnapshot = mRows.Cast<object?>().ToArray();
        object qTarget = Row(Side.Quire, qRows, cols);
        object mTarget = Row(Side.Mathcad, mRows, cols);
        object? qSelected = (uint)i < (uint)qRows.Length ? qRows.GetValue(i) : null;
        object? mSelected = (uint)i < (uint)mRows.Length ? mRows.GetValue(i) : null;
        var qMethod = Method(Side.Quire, "matdim");
        var mMethod = Method(Side.Mathcad, "matdim");
        Assert.Equal(MatDimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qRows);
        string mBefore = State(Side.Mathcad, mTarget, mRows);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowMatDim(Side.Quire, qSelected, result),
            result => ShowMatDim(Side.Mathcad, mSelected, result),
            target => State(Side.Quire, target, qRows),
            target => State(Side.Mathcad, target, mRows));

        Assert.Same(qRows, Get(qTarget, "m_rowdim"));
        Assert.Same(mRows, Get(mTarget, "m_rowdim"));
        for (int k = 0; k < qRows.Length; k++) Assert.Same(qRowSnapshot[k], qRows.GetValue(k));
        for (int k = 0; k < mRows.Length; k++) Assert.Same(mRowSnapshot[k], mRows.GetValue(k));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qRows));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mRows));
    }

    [SkippableFact]
    public void R_dim_dim_selects_by_row_only_and_propagates_invalid_rows()
    {
        Runtime.Sync();
        Assert.Equal(DimToken, Method(Side.Mathcad, "dim").MetadataToken);

        var units = new string?[] { "L", null, "T" };
        CompareDim("r_dim.dim row 0 ignores normal, negative, and extreme columns", units, 4, 0, 0);
        CompareDim("r_dim.dim row 0 ignores negative column", units, 4, 0, -1);
        CompareDim("r_dim.dim row 0 ignores high column", units, 4, 0, 200);
        CompareDim("r_dim.dim row 0 ignores extreme column", units, 4, 0, int.MinValue);
        CompareDim("r_dim.dim row 1 returns null slot", units, 4, 1, int.MaxValue);
        CompareDim("r_dim.dim row 2 selects time unit", units, 4, 2, -100);
        CompareDim("r_dim.dim negative row throws", units, 4, -1, 0);
        CompareDim("r_dim.dim row at length throws", units, 4, 3, 0);
        CompareDim("r_dim.dim empty row array throws", Array.Empty<string?>(), 4, 0, 0);
    }

    [SkippableFact]
    public void R_dim_matdim_builds_one_by_one_uniform_dimensions_and_propagates_invalid_rows()
    {
        Runtime.Sync();
        Assert.Equal(MatDimToken, Method(Side.Mathcad, "matdim").MetadataToken);

        var units = new string?[] { "L", null, "T" };
        CompareMatDim("r_dim.matdim row 0 ignores normal, negative, and extreme columns", units, 4, 0, 0);
        CompareMatDim("r_dim.matdim row 0 ignores negative column", units, 4, 0, -1);
        CompareMatDim("r_dim.matdim row 0 ignores high column", units, 4, 0, 200);
        CompareMatDim("r_dim.matdim row 0 ignores extreme column", units, 4, 0, int.MinValue);
        CompareMatDim("r_dim.matdim null row normalizes to identity", units, 4, 1, int.MaxValue);
        CompareMatDim("r_dim.matdim row 2 wraps selected unit in 1x1", units, 4, 2, -100);
        CompareMatDim("r_dim.matdim negative row throws", units, 4, -1, 0);
        CompareMatDim("r_dim.matdim row at length throws", units, 4, 3, 0);
        CompareMatDim("r_dim.matdim empty row array throws", Array.Empty<string?>(), 4, 0, 0);
    }
}
