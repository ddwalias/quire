using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimRowColumnCoordinateQueryDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007b5;
    private const int MatDimToken = 0x060007b6;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static System.Type RowColumnType(Side side) => Nested(side.T("Mcd.MatDim"), "rc_dim");

    private static MethodInfo Method(Side side, string name) =>
        RowColumnType(side).GetMethod(name, DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.{name}(int,int)");

    private static Array Dimensions(Side side, params string?[] units)
    {
        var result = Array.CreateInstance(side.Dim, units.Length);
        for (int i = 0; i < units.Length; i++)
            result.SetValue(units[i] == null ? null : side.D(units[i]!), i);
        return result;
    }

    private static object MatrixDimension(Side side, Array rows, Array cols) =>
        Call(RowColumnType(side), "make_rc", rows, cols)!;

    private static string ArrayText(Side side, Array dimensions) =>
        "[" + string.Join(",", dimensions.Cast<object?>().Select(side.DimText)) + "]";

    private static string State(Side side, object target, Array retainedRows, Array retainedCols)
    {
        var rows = (Array)Get(target, "m_rowdim")!;
        var cols = (Array)Get(target, "m_coldim")!;
        return $"rc_dim {Get(target, "Rows")}x{Get(target, "Cols")} " +
            $"row-ref={ReferenceEquals(rows, retainedRows)} rows={ArrayText(side, rows)} retained-rows={ArrayText(side, retainedRows)} " +
            $"col-ref={ReferenceEquals(cols, retainedCols)} cols={ArrayText(side, cols)} retained-cols={ArrayText(side, retainedCols)}";
    }

    private static string ShowDim(Side side, object? expectedProduct, object? result)
    {
        if (result == null) return "null";
        if (result.GetType() != side.Dim) return side.Show(result);
        return $"Dim {side.DimText(result)} same-product={ReferenceEquals(result, expectedProduct)}";
    }

    private static string ShowMatDim(Side side, object? expectedProduct, object? result)
    {
        if (result == null) return "null";
        if (!side.T("Mcd.MatDim").IsAssignableFrom(result.GetType())) return side.Show(result);
        object? unit = Get(result, "UniformDim");
        return $"{result.GetType().Name} {Get(result, "Rows")}x{Get(result, "Cols")} " +
            $"unit={side.DimText(unit)} unit-is-identity={ReferenceEquals(unit, side.Marker("identity"))} " +
            $"unit-same-product={ReferenceEquals(unit, expectedProduct)}";
    }

    private static void CompareDim(string caseId, string?[] rowUnits, string?[] colUnits, int i, int j)
    {
        Array qRows = Dimensions(Side.Quire, rowUnits);
        Array mRows = Dimensions(Side.Mathcad, rowUnits);
        Array qCols = Dimensions(Side.Quire, colUnits);
        Array mCols = Dimensions(Side.Mathcad, colUnits);
        object?[] qRowSnapshot = qRows.Cast<object?>().ToArray();
        object?[] mRowSnapshot = mRows.Cast<object?>().ToArray();
        object?[] qColSnapshot = qCols.Cast<object?>().ToArray();
        object?[] mColSnapshot = mCols.Cast<object?>().ToArray();
        object qTarget = MatrixDimension(Side.Quire, qRows, qCols);
        object mTarget = MatrixDimension(Side.Mathcad, mRows, mCols);
        object? qExpected = (uint)i < (uint)qRows.Length && (uint)j < (uint)qCols.Length
            ? Call(Side.Quire.T("Mcd.Dim"), "Mult", qRows.GetValue(i), qCols.GetValue(j)) : null;
        object? mExpected = (uint)i < (uint)mRows.Length && (uint)j < (uint)mCols.Length
            ? Call(Side.Mathcad.T("Mcd.Dim"), "Mult", mRows.GetValue(i), mCols.GetValue(j)) : null;
        var qMethod = Method(Side.Quire, "dim");
        var mMethod = Method(Side.Mathcad, "dim");
        Assert.Equal(DimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qRows, qCols);
        string mBefore = State(Side.Mathcad, mTarget, mRows, mCols);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowDim(Side.Quire, qExpected, result),
            result => ShowDim(Side.Mathcad, mExpected, result),
            target => State(Side.Quire, target, qRows, qCols),
            target => State(Side.Mathcad, target, mRows, mCols));

        Assert.Same(qRows, Get(qTarget, "m_rowdim"));
        Assert.Same(mRows, Get(mTarget, "m_rowdim"));
        Assert.Same(qCols, Get(qTarget, "m_coldim"));
        Assert.Same(mCols, Get(mTarget, "m_coldim"));
        for (int k = 0; k < qRows.Length; k++) Assert.Same(qRowSnapshot[k], qRows.GetValue(k));
        for (int k = 0; k < mRows.Length; k++) Assert.Same(mRowSnapshot[k], mRows.GetValue(k));
        for (int k = 0; k < qCols.Length; k++) Assert.Same(qColSnapshot[k], qCols.GetValue(k));
        for (int k = 0; k < mCols.Length; k++) Assert.Same(mColSnapshot[k], mCols.GetValue(k));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qRows, qCols));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mRows, mCols));
    }

    private static void CompareMatDim(string caseId, string?[] rowUnits, string?[] colUnits, int i, int j)
    {
        Array qRows = Dimensions(Side.Quire, rowUnits);
        Array mRows = Dimensions(Side.Mathcad, rowUnits);
        Array qCols = Dimensions(Side.Quire, colUnits);
        Array mCols = Dimensions(Side.Mathcad, colUnits);
        object?[] qRowSnapshot = qRows.Cast<object?>().ToArray();
        object?[] mRowSnapshot = mRows.Cast<object?>().ToArray();
        object?[] qColSnapshot = qCols.Cast<object?>().ToArray();
        object?[] mColSnapshot = mCols.Cast<object?>().ToArray();
        object qTarget = MatrixDimension(Side.Quire, qRows, qCols);
        object mTarget = MatrixDimension(Side.Mathcad, mRows, mCols);
        object? qExpected = (uint)i < (uint)qRows.Length && (uint)j < (uint)qCols.Length
            ? Call(Side.Quire.T("Mcd.Dim"), "Mult", qRows.GetValue(i), qCols.GetValue(j)) : null;
        object? mExpected = (uint)i < (uint)mRows.Length && (uint)j < (uint)mCols.Length
            ? Call(Side.Mathcad.T("Mcd.Dim"), "Mult", mRows.GetValue(i), mCols.GetValue(j)) : null;
        var qMethod = Method(Side.Quire, "matdim");
        var mMethod = Method(Side.Mathcad, "matdim");
        Assert.Equal(MatDimToken, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);

        string qBefore = State(Side.Quire, qTarget, qRows, qCols);
        string mBefore = State(Side.Mathcad, mTarget, mRows, mCols);
        Assert.Equal(mBefore, qBefore);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => ShowMatDim(Side.Quire, qExpected, result),
            result => ShowMatDim(Side.Mathcad, mExpected, result),
            target => State(Side.Quire, target, qRows, qCols),
            target => State(Side.Mathcad, target, mRows, mCols));

        Assert.Same(qRows, Get(qTarget, "m_rowdim"));
        Assert.Same(mRows, Get(mTarget, "m_rowdim"));
        Assert.Same(qCols, Get(qTarget, "m_coldim"));
        Assert.Same(mCols, Get(mTarget, "m_coldim"));
        for (int k = 0; k < qRows.Length; k++) Assert.Same(qRowSnapshot[k], qRows.GetValue(k));
        for (int k = 0; k < mRows.Length; k++) Assert.Same(mRowSnapshot[k], mRows.GetValue(k));
        for (int k = 0; k < qCols.Length; k++) Assert.Same(qColSnapshot[k], qCols.GetValue(k));
        for (int k = 0; k < mCols.Length; k++) Assert.Same(mColSnapshot[k], mCols.GetValue(k));
        Assert.Equal(qBefore, State(Side.Quire, qTarget, qRows, qCols));
        Assert.Equal(mBefore, State(Side.Mathcad, mTarget, mRows, mCols));
    }

    [SkippableFact]
    public void Rc_dim_dim_multiplies_selected_row_and_column_factors()
    {
        Runtime.Sync();
        Assert.Equal(DimToken, Method(Side.Mathcad, "dim").MetadataToken);

        var rows = new string?[] { "L", "identity", "T", null };
        var cols = new string?[] { "T", "L", "identity", null };
        CompareDim("rc_dim.dim length times time", rows, cols, 0, 0);
        CompareDim("rc_dim.dim identity row returns selected column unit", rows, cols, 1, 1);
        CompareDim("rc_dim.dim identity column returns selected row unit", rows, cols, 2, 2);
        CompareDim("rc_dim.dim null row and identity column returns null", rows, cols, 3, 2);
        CompareDim("rc_dim.dim negative row throws", rows, cols, -1, 0);
        CompareDim("rc_dim.dim row beyond length throws", rows, cols, 4, 0);
        CompareDim("rc_dim.dim negative column throws", rows, cols, 0, -1);
        CompareDim("rc_dim.dim column beyond length throws", rows, cols, 0, 4);
    }

    [SkippableFact]
    public void Rc_dim_matdim_wraps_each_product_in_one_by_one_uniform_dimension()
    {
        Runtime.Sync();
        Assert.Equal(MatDimToken, Method(Side.Mathcad, "matdim").MetadataToken);

        var rows = new string?[] { "L", "identity", "T", null };
        var cols = new string?[] { "T", "L", "identity", null };
        CompareMatDim("rc_dim.matdim length times time", rows, cols, 0, 0);
        CompareMatDim("rc_dim.matdim identity row normalizes to selected column unit", rows, cols, 1, 1);
        CompareMatDim("rc_dim.matdim identity column returns selected row unit", rows, cols, 2, 2);
        CompareMatDim("rc_dim.matdim null product normalizes to identity", rows, cols, 3, 2);
        CompareMatDim("rc_dim.matdim negative row throws", rows, cols, -1, 0);
        CompareMatDim("rc_dim.matdim row beyond length throws", rows, cols, 4, 0);
        CompareMatDim("rc_dim.matdim negative column throws", rows, cols, 0, -1);
        CompareMatDim("rc_dim.matdim column beyond length throws", rows, cols, 0, 4);
    }
}
