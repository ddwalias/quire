using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimDmultDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int Dmult1Token = 0x060007cf;
    private const int Dmult2RowToken = 0x060007d0;
    private const int Dmult2ColumnToken = 0x060007d1;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Value, object? Rows, object? Cols, object?[]? RowItems, object?[]? ColItems, int RowCount, int ColCount);

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type Nested(Side side, string name) => Reflect.Nested(MatDim(side), name);

    private static Array Dims(Side side, params string?[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(names[i] == null ? null : side.D(names[i]!), i);
        return result;
    }

    private static object MakeRow(Side side, string?[] rows, int cols) =>
        Call(Nested(side, "r_dim"), "make", Dims(side, rows), cols)!;

    private static object MakeColumn(Side side, string?[] cols, int rows) =>
        Call(Nested(side, "c_dim"), "make", Dims(side, cols), rows)!;

    private static object MakeRc(Side side, string?[] rows, string?[] cols) =>
        Call(Nested(side, "rc_dim"), "make_rc", Dims(side, rows), Dims(side, cols))!;

    private static MethodInfo Method(Side side, string name, Type parameter) =>
        Nested(side, "r_dim").GetMethod(name, DeclaredInstance, null, [parameter], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.{name}({parameter.Name})");

    private static object? TryGet(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object? RowAxis(object target) => TryGet(target, "m_rowdim");
    private static object? ColAxis(object target) => TryGet(target, "m_coldim");

    private static Snapshot Capture(object value)
    {
        object? rows = RowAxis(value), cols = ColAxis(value);
        return new(value, rows, cols,
            rows is Array ra ? ra.Cast<object?>().ToArray() : null,
            cols is Array ca ? ca.Cast<object?>().ToArray() : null,
            (int)Get(value, "Rows")!, (int)Get(value, "Cols")!);
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : value == null ? "null" : side.DimText(value);

    private static string MatrixText(Side side, object? value, Snapshot? before = null)
    {
        if (value == null) return "null";
        if (!MatDim(side).IsInstanceOfType(value)) return side.Dim.IsInstanceOfType(value) ? side.DimText(value) : Text(value);
        string text = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" rows={ArrayText(side, RowAxis(value))} cols={ArrayText(side, ColAxis(value))}";
        if (before != null)
            text += $" rows-ref-unchanged={ReferenceEquals(RowAxis(value), before.Rows)}" +
                $" cols-ref-unchanged={ReferenceEquals(ColAxis(value), before.Cols)}";
        return text;
    }

    private static void AssertArrayUnchanged(object?[]? before, object? after)
    {
        if (before == null) { Assert.Null(after); return; }
        Assert.IsAssignableFrom<Array>(after);
        var current = (Array)after!;
        Assert.Equal(before.Length, current.Length);
        for (int i = 0; i < before.Length; i++) Assert.Same(before[i], current.GetValue(i));
    }

    private static void AssertUnchanged(Snapshot before)
    {
        Assert.Equal(before.RowCount, Get(before.Value, "Rows"));
        Assert.Equal(before.ColCount, Get(before.Value, "Cols"));
        Assert.Same(before.Rows, RowAxis(before.Value));
        Assert.Same(before.Cols, ColAxis(before.Value));
        AssertArrayUnchanged(before.RowItems, RowAxis(before.Value));
        AssertArrayUnchanged(before.ColItems, ColAxis(before.Value));
    }

    private static object? Factor(Side side, object left, object right, string formula)
    {
        object? first = formula switch
        {
            "left-row-right-row" => RowAxis(left),
            "right-row-left-row" => RowAxis(right),
            "left-row-right-col" => RowAxis(left),
            "right-col-left-row" => ColAxis(right),
            _ => throw new ArgumentOutOfRangeException(nameof(formula))
        };
        object? second = formula switch
        {
            "left-row-right-row" => RowAxis(right),
            "right-row-left-row" => RowAxis(left),
            "left-row-right-col" => ColAxis(right),
            "right-col-left-row" => RowAxis(left),
            _ => throw new ArgumentOutOfRangeException(nameof(formula))
        };
        return CallSig(MatDim(side), "Mult", ["Dim[]", "Dim[]"], first, second);
    }

    private static void Compare(string caseId, string methodName, string parameterType, int token,
        Func<Side, object> leftFactory, Func<Side, object?> rightFactory, string formula,
        bool expectsBaseNull = false)
    {
        object qLeft = leftFactory(Side.Quire), mLeft = leftFactory(Side.Mathcad);
        object? qRight = rightFactory(Side.Quire), mRight = rightFactory(Side.Mathcad);
        MethodInfo qMethod = Method(Side.Quire, methodName, parameterType == "MatDim" ? MatDim(Side.Quire) : Nested(Side.Quire, parameterType));
        MethodInfo mMethod = Method(Side.Mathcad, methodName, parameterType == "MatDim" ? MatDim(Side.Mathcad) : Nested(Side.Mathcad, parameterType));
        Assert.Equal(token, mMethod.MetadataToken);
        Snapshot qLeftBefore = Capture(qLeft), mLeftBefore = Capture(mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(qRight);
        Snapshot? mRightBefore = mRight == null ? null : Capture(mRight);

        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => MatrixText(Side.Quire, value), value => MatrixText(Side.Mathcad, value),
            target => MatrixText(Side.Quire, target, qLeftBefore), target => MatrixText(Side.Mathcad, target, mLeftBefore));

        if (qRight != null && !expectsBaseNull)
        {
            object? qResult = mMethod.ReturnType == typeof(void) ? null : qMethod.Invoke(qLeft, [qRight]);
            object? mResult = mMethod.Invoke(mLeft, [mRight]);
            object? qExpected = Factor(Side.Quire, qLeft, qRight, formula);
            object? mExpected = Factor(Side.Mathcad, mLeft, mRight!, formula);
            Assert.Equal(qExpected == null, qResult == null);
            Assert.Equal(mExpected == null, mResult == null);
            if (qExpected != null) Assert.Same(qExpected, qResult);
            if (mExpected != null) Assert.Same(mExpected, mResult);
        }
        else if (expectsBaseNull)
        {
            Assert.Null(qMethod.Invoke(qLeft, [qRight]));
            Assert.Null(mMethod.Invoke(mLeft, [mRight]));
        }

        AssertUnchanged(qLeftBefore); AssertUnchanged(mLeftBefore);
        if (qRightBefore != null) AssertUnchanged(qRightBefore);
        if (mRightBefore != null) AssertUnchanged(mRightBefore);
    }

    [SkippableFact]
    public void R_dim_dmult1_dispatches_to_the_rhs_dimension_product_overload()
    {
        Runtime.Sync();
        Assert.Equal(Dmult1Token, Method(Side.Mathcad, "dmult1", MatDim(Side.Mathcad)).MetadataToken);

        Compare("r_dim dmult1 dispatches to r_dim compatible factors", "dmult1", "MatDim", Dmult1Token,
            side => MakeRow(side, ["L", "L"], 3), side => MakeRow(side, ["T", "T"], 2), "left-row-right-row");
        Compare("r_dim dmult1 dispatches to c_dim compatible factors", "dmult1", "MatDim", Dmult1Token,
            side => MakeRow(side, ["L", "L"], 3), side => MakeColumn(side, ["T", "T"], 4), "left-row-right-col");
        Compare("r_dim dmult1 dispatches to rc_dim base null fallback", "dmult1", "MatDim", Dmult1Token,
            side => MakeRow(side, ["L", "L"], 3), side => MakeRc(side, ["T", "T"], ["N", "N"]), "", expectsBaseNull: true);
        Compare("r_dim dmult1 incompatible factors return null", "dmult1", "MatDim", Dmult1Token,
            side => MakeRow(side, ["L", "T"], 3), side => MakeRow(side, ["identity", "identity"], 2), "left-row-right-row");
        Compare("r_dim dmult1 null argument throws", "dmult1", "MatDim", Dmult1Token,
            side => MakeRow(side, ["L"], 2), _ => null, "");
    }

    [SkippableFact]
    public void R_dim_dmult2_r_dim_multiplies_the_row_factor_arrays()
    {
        Runtime.Sync();
        Assert.Equal(Dmult2RowToken, Method(Side.Mathcad, "dmult2", Nested(Side.Mathcad, "r_dim")).MetadataToken);

        Compare("r_dim dmult2 r_dim compatible factors", "dmult2", "r_dim", Dmult2RowToken,
            side => MakeRow(side, ["L", "L"], 3), side => MakeRow(side, ["T", "T"], 2), "right-row-left-row");
        Compare("r_dim dmult2 r_dim incompatible factors return null", "dmult2", "r_dim", Dmult2RowToken,
            side => MakeRow(side, ["L", "T"], 3), side => MakeRow(side, ["identity", "identity"], 2), "right-row-left-row");
        Compare("r_dim dmult2 r_dim null argument throws", "dmult2", "r_dim", Dmult2RowToken,
            side => MakeRow(side, ["L"], 2), _ => null, "");
    }

    [SkippableFact]
    public void R_dim_dmult2_c_dim_multiplies_column_and_row_factor_arrays()
    {
        Runtime.Sync();
        Assert.Equal(Dmult2ColumnToken, Method(Side.Mathcad, "dmult2", Nested(Side.Mathcad, "c_dim")).MetadataToken);

        Compare("r_dim dmult2 c_dim compatible factors", "dmult2", "c_dim", Dmult2ColumnToken,
            side => MakeRow(side, ["L", "L"], 3), side => MakeColumn(side, ["T", "T"], 4), "right-col-left-row");
        Compare("r_dim dmult2 c_dim incompatible factors return null", "dmult2", "c_dim", Dmult2ColumnToken,
            side => MakeRow(side, ["L", "T"], 3), side => MakeColumn(side, ["identity", "identity"], 4), "right-col-left-row");
        Compare("r_dim dmult2 c_dim null argument throws", "dmult2", "c_dim", Dmult2ColumnToken,
            side => MakeRow(side, ["L"], 2), _ => null, "");
    }
}
