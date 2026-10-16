using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimSpecializedAxisArraysDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private static readonly Guid Mvid = new("17a0933b-4988-4c18-963d-563998321cc8");
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Target, int Rows, int Cols, object? RowDimensions, object? ColDimensions,
        object?[] RowValues, object?[] ColValues);

    private static Type Nested(Side side, string name) => side.T($"Mcd.MatDim+{name}");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.D("identity")!.GetType(), names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Row(Side side, string[] dimensions, int columns) =>
        Call(Nested(side, "r_dim"), "make", Dims(side, dimensions), columns)!;

    private static object Column(Side side, string[] dimensions, int rows) =>
        Call(Nested(side, "c_dim"), "make", Dims(side, dimensions), rows)!;

    private static object RowColumn(Side side, string[] rows, string[] columns) =>
        Call(Nested(side, "rc_dim"), "make_rc", Dims(side, rows), Dims(side, columns))!;

    private static MethodInfo Method(Side side, string kind, string name) =>
        Nested(side, kind).GetMethod(name, DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{kind}.{name}(int)");

    private static Snapshot Capture(object target)
    {
        object? rows = FieldOrNull(target, "m_rowdim");
        object? cols = FieldOrNull(target, "m_coldim");
        return new(target, (int)Get(target, "Rows")!, (int)Get(target, "Cols")!, rows, cols,
            Values(rows), Values(cols));
    }

    private static object? FieldOrNull(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object?[] Values(object? array) => array is Array values
        ? values.Cast<object?>().ToArray()
        : [];

    private static string Ref(object? left, object? right) => ReferenceEquals(left, right) ? "same" : "changed";

    private static string DimText(Side side, object? dimension) => dimension == null ? "null" : side.DimText(dimension);

    private static string AxisValues(Side side, object?[] values) =>
        "[" + string.Join(",", values.Select(value => DimText(side, value))) + "]";

    private static string State(Side side, Snapshot before, object target)
    {
        object? rowDimensions = FieldOrNull(target, "m_rowdim");
        object? colDimensions = FieldOrNull(target, "m_coldim");
        return $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}" +
            $":row-array-ref={Ref(before.RowDimensions, rowDimensions)}:rows={AxisValues(side, Values(rowDimensions))}" +
            $":retained-rows={AxisValues(side, before.RowValues)}" +
            $":col-array-ref={Ref(before.ColDimensions, colDimensions)}:cols={AxisValues(side, Values(colDimensions))}" +
            $":retained-cols={AxisValues(side, before.ColValues)}";
    }

    private static string Result(Side side, object? result, Snapshot before)
    {
        if (result is not Array array) return Text(result);
        string alias = ReferenceEquals(result, before.RowDimensions) ? "row-axis" :
            ReferenceEquals(result, before.ColDimensions) ? "col-axis" : "neither-axis-field";
        return $"{result.GetType().Name}:alias={alias}:" + "[" + string.Join(";", array.Cast<object?>().Select((dimension, index) =>
            $"{index}={DimText(side, dimension)}:row-ref={before.RowValues.Any(value => ReferenceEquals(value, dimension))}:" +
            $"col-ref={before.ColValues.Any(value => ReferenceEquals(value, dimension))}")) + "]";
    }

    private static void Compare(string caseId, string kind, string name, int token, object qTarget, object mTarget, int index)
    {
        MethodInfo qMethod = Method(Side.Quire, kind, name), mMethod = Method(Side.Mathcad, kind, name);
        Assert.Equal(token, mMethod.MetadataToken);
        Assert.Equal(Mvid, mMethod.Module.ModuleVersionId);
        Snapshot qBefore = Capture(qTarget), mBefore = Capture(mTarget);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [index], mMethod, mTarget, [index],
            result => Result(Side.Quire, result, qBefore), result => Result(Side.Mathcad, result, mBefore),
            target => State(Side.Quire, qBefore, target), target => State(Side.Mathcad, mBefore, target));
    }

    [SkippableFact]
    public void R_dim_ColDimensionArray_returns_the_stored_row_array_and_ignores_index()
    {
        Runtime.Sync();
        Compare("r_dim.ColDimensionArray valid index aliases stored row dimensions", "r_dim", "ColDimensionArray", 0x060007c9,
            Row(Side.Quire, ["L", "T"], 3), Row(Side.Mathcad, ["L", "T"], 3), 1);
        Compare("r_dim.ColDimensionArray negative index is ignored", "r_dim", "ColDimensionArray", 0x060007c9,
            Row(Side.Quire, ["L", "T"], 3), Row(Side.Mathcad, ["L", "T"], 3), -1);
    }

    [SkippableFact]
    public void C_dim_RowDimensionArray_returns_the_stored_column_array_and_ignores_index()
    {
        Runtime.Sync();
        Compare("c_dim.RowDimensionArray valid index aliases stored column dimensions", "c_dim", "RowDimensionArray", 0x060007db,
            Column(Side.Quire, ["L", "T"], 3), Column(Side.Mathcad, ["L", "T"], 3), 1);
        Compare("c_dim.RowDimensionArray above-range index is ignored", "c_dim", "RowDimensionArray", 0x060007db,
            Column(Side.Quire, ["L", "T"], 3), Column(Side.Mathcad, ["L", "T"], 3), 99);
    }

    [SkippableFact]
    public void Rc_dim_RowDimensionArray_multiplies_selected_row_factor_and_checks_index()
    {
        Runtime.Sync();
        Compare("rc_dim.RowDimensionArray multiplies selected row factor by all column factors", "rc_dim", "RowDimensionArray", 0x060007b8,
            RowColumn(Side.Quire, ["L", "T"], ["T", "N"]),
            RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), 1);
        Compare("rc_dim.RowDimensionArray rejects negative row index", "rc_dim", "RowDimensionArray", 0x060007b8,
            RowColumn(Side.Quire, ["L", "T"], ["T", "N"]),
            RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), -1);
    }

    [SkippableFact]
    public void Rc_dim_ColDimensionArray_multiplies_selected_column_factor_and_checks_index()
    {
        Runtime.Sync();
        Compare("rc_dim.ColDimensionArray multiplies selected column factor by all row factors", "rc_dim", "ColDimensionArray", 0x060007b9,
            RowColumn(Side.Quire, ["L", "T"], ["T", "N"]),
            RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), 1);
        Compare("rc_dim.ColDimensionArray rejects above-range column index", "rc_dim", "ColDimensionArray", 0x060007b9,
            RowColumn(Side.Quire, ["L", "T"], ["T", "N"]),
            RowColumn(Side.Mathcad, ["L", "T"], ["T", "N"]), 2);
    }
}
