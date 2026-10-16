using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimEqualityDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int EqToken = 0x060007ca;
    private const int Eq2Token = 0x060007cb;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object? AxisArray, object?[] Items, int Rows, int Cols);

    private static Type RDim(Side side) => Nested(side.T("Mcd.MatDim"), "r_dim");
    private static Type CDim(Side side) => Nested(side.T("Mcd.MatDim"), "c_dim");

    private static MethodInfo EqMethod(Side side) =>
        RDim(side).GetMethod("eq_b", DeclaredInstance, null, [side.T("Mcd.MatDim")], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.eq_b(MatDim)");

    private static MethodInfo Eq2Method(Side side) =>
        RDim(side).GetMethod("eq2_b", DeclaredInstance, null, [RDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.eq2_b(r_dim)");

    private static Array Dims(Side side, params string?[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(names[i] == null ? null : side.D(names[i]!), i);
        return result;
    }

    private static object Row(Side side, string?[] names, int cols) =>
        Call(RDim(side), "make", Dims(side, names), cols)!;

    private static object Column(Side side, string?[] names, int rows) =>
        Call(CDim(side), "make", Dims(side, names), rows)!;

    private static Snapshot Capture(object target)
    {
        string axisField = RDimFrom(target) ? "m_rowdim" : "m_coldim";
        object? axis = Get(target, axisField);
        return new(axis, axis is Array a ? a.Cast<object?>().ToArray() : [],
            (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);
    }

    private static bool RDimFrom(object target) => target.GetType().Name == "r_dim";

    private static string ArrayText(Side side, object? array) => array is Array values
        ? "[" + string.Join(",", values.Cast<object?>().Select(side.DimText)) + "]"
        : "null";

    private static string ShapeText(Side side, object target, Snapshot before) =>
        $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}" +
        $" axis-ref-unchanged={ReferenceEquals(Get(target, RDimFrom(target) ? "m_rowdim" : "m_coldim"), before.AxisArray)}" +
        $" axis={ArrayText(side, Get(target, RDimFrom(target) ? "m_rowdim" : "m_coldim"))}";

    private static string ArgumentText(Side side, object? value)
    {
        if (value == null) return "null";
        string field = RDimFrom(value) ? "m_rowdim" : "m_coldim";
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" axis={ArrayText(side, Get(value, field))}";
    }

    private static void AssertUnchanged(object target, Snapshot before)
    {
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
        object? after = Get(target, RDimFrom(target) ? "m_rowdim" : "m_coldim");
        Assert.Same(before.AxisArray, after);
        Assert.Equal(before.Items.Length, ((Array)after!).Length);
        for (int i = 0; i < before.Items.Length; i++) Assert.Same(before.Items[i], ((Array)after).GetValue(i));
    }

    private static bool CompareEq(string caseId, string?[] left, int leftCols, string?[]? right, int rightCols, bool columnRight = false)
    {
        object qLeft = Row(Side.Quire, left, leftCols), mLeft = Row(Side.Mathcad, left, leftCols);
        object? qRight = right == null ? null : columnRight ? Column(Side.Quire, right, rightCols) : Row(Side.Quire, right, rightCols);
        object? mRight = right == null ? null : columnRight ? Column(Side.Mathcad, right, rightCols) : Row(Side.Mathcad, right, rightCols);
        Snapshot qLeftBefore = Capture(qLeft), mLeftBefore = Capture(mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(qRight);
        Snapshot? mRightBefore = mRight == null ? null : Capture(mRight);
        MethodInfo qMethod = EqMethod(Side.Quire), mMethod = EqMethod(Side.Mathcad);
        Assert.Equal(EqToken, mMethod.MetadataToken);
        bool? result = null;

        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => { if (value is bool b) result = b; return value is bool ? Text(value) : ArgumentText(Side.Quire, value); },
            value => value is bool ? Text(value) : ArgumentText(Side.Mathcad, value),
            target => ShapeText(Side.Quire, target, qLeftBefore), target => ShapeText(Side.Mathcad, target, mLeftBefore));

        AssertUnchanged(qLeft, qLeftBefore); AssertUnchanged(mLeft, mLeftBefore);
        if (qRight != null) AssertUnchanged(qRight, qRightBefore!);
        if (mRight != null) AssertUnchanged(mRight, mRightBefore!);
        if (right == null) { Assert.Null(result); return false; }
        return result ?? throw new HarnessException("eq_b comparison did not return a Boolean");
    }

    private static bool CompareEq2(string caseId, string?[] left, int leftCols, string?[]? right, int rightCols)
    {
        object qLeft = Row(Side.Quire, left, leftCols), mLeft = Row(Side.Mathcad, left, leftCols);
        object? qRight = right == null ? null : Row(Side.Quire, right, rightCols);
        object? mRight = right == null ? null : Row(Side.Mathcad, right, rightCols);
        Snapshot qLeftBefore = Capture(qLeft), mLeftBefore = Capture(mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(qRight);
        Snapshot? mRightBefore = mRight == null ? null : Capture(mRight);
        MethodInfo qMethod = Eq2Method(Side.Quire), mMethod = Eq2Method(Side.Mathcad);
        Assert.Equal(Eq2Token, mMethod.MetadataToken);
        bool? result = null;

        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => { if (value is bool b) result = b; return value is bool ? Text(value) : ArgumentText(Side.Quire, value); },
            value => value is bool ? Text(value) : ArgumentText(Side.Mathcad, value),
            target => ShapeText(Side.Quire, target, qLeftBefore), target => ShapeText(Side.Mathcad, target, mLeftBefore));

        AssertUnchanged(qLeft, qLeftBefore); AssertUnchanged(mLeft, mLeftBefore);
        if (qRight != null) AssertUnchanged(qRight, qRightBefore!);
        if (mRight != null) AssertUnchanged(mRight, mRightBefore!);
        if (right == null) { Assert.Null(result); return false; }
        return result ?? throw new HarnessException("eq2_b comparison did not return a Boolean");
    }

    [SkippableFact]
    public void R_dim_eq_b_dispatches_to_the_rhs_and_ignores_columns()
    {
        Runtime.Sync();
        Assert.Equal(EqToken, EqMethod(Side.Mathcad).MetadataToken);

        Assert.True(CompareEq("r_dim eq_b equal row factors but different columns", ["L", "T"], 2, ["L", "T"], 9));
        Assert.False(CompareEq("r_dim eq_b later row factor differs", ["L", "T"], 2, ["L", "N"], 2));
        Assert.False(CompareEq("r_dim eq_b row lengths differ", ["L", "T"], 2, ["L"], 2));
        Assert.False(CompareEq("r_dim eq_b dispatch to c_dim default is false", ["L", "T"], 2, ["L", "T"], 2, columnRight: true));
        _ = CompareEq("r_dim eq_b null rhs throws", ["L"], 1, null, 0);
    }

    [SkippableFact]
    public void R_dim_eq2_b_compares_only_row_factor_arrays()
    {
        Runtime.Sync();
        Assert.Equal(Eq2Token, Eq2Method(Side.Mathcad).MetadataToken);

        Assert.True(CompareEq2("r_dim eq2_b equal row factors but different columns", ["L", "T"], 2, ["L", "T"], 9));
        Assert.False(CompareEq2("r_dim eq2_b later row factor differs", ["L", "T"], 2, ["L", "N"], 2));
        Assert.False(CompareEq2("r_dim eq2_b row lengths differ", ["L", "T"], 2, ["L"], 2));
        _ = CompareEq2("r_dim eq2_b null argument throws", ["L"], 1, null, 0);
    }
}
