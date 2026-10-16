using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimMmultDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int Mmult1Token = 0x060007cc;
    private const int Mmult2ColumnToken = 0x060007cd;
    private const int Mmult2RcToken = 0x060007ce;
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

    private static string MatrixText(Side side, object? value)
    {
        if (value == null) return "null";
        if (!MatDim(side).IsInstanceOfType(value)) return side.Dim.IsInstanceOfType(value) ? side.DimText(value) : Text(value);
        string text = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" rows={ArrayText(side, RowAxis(value))} cols={ArrayText(side, ColAxis(value))}";
        if (Nested(side, "uniform_dim").IsInstanceOfType(value)) text += $" uniform={side.DimText(Get(value, "m_dim"))}";
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

    private static MethodInfo MethodFor(Side side, string name, string parameterType) =>
        Method(side, name, parameterType == "MatDim" ? MatDim(side) : Nested(side, parameterType));

    private static object? Invoke(MethodInfo method, object target, object? argument) => method.Invoke(target, [argument]);

    private static void Compare(string caseId, string methodName, string parameterType, int token,
        Func<Side, object> leftFactory, Func<Side, object?> rightFactory,
        Action<Side, object, object?, object?>? assertResult = null)
    {
        object qLeft = leftFactory(Side.Quire), mLeft = leftFactory(Side.Mathcad);
        object? qRight = rightFactory(Side.Quire), mRight = rightFactory(Side.Mathcad);
        MethodInfo qMethod = MethodFor(Side.Quire, methodName, parameterType);
        MethodInfo mMethod = MethodFor(Side.Mathcad, methodName, parameterType);
        Assert.Equal(token, mMethod.MetadataToken);
        Snapshot qLeftBefore = Capture(qLeft), mLeftBefore = Capture(mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(qRight);
        Snapshot? mRightBefore = mRight == null ? null : Capture(mRight);

        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => MatrixText(Side.Quire, value), value => MatrixText(Side.Mathcad, value),
            target => MatrixText(Side.Quire, target), target => MatrixText(Side.Mathcad, target));

        if (assertResult != null)
        {
            assertResult(Side.Quire, qLeft, qRight, Invoke(qMethod, qLeft, qRight));
            assertResult(Side.Mathcad, mLeft, mRight, Invoke(mMethod, mLeft, mRight));
        }
        AssertUnchanged(qLeftBefore); AssertUnchanged(mLeftBefore);
        if (qRightBefore != null) AssertUnchanged(qRightBefore);
        if (mRightBefore != null) AssertUnchanged(mRightBefore);
    }

    private static object? ExpectedFactor(Side side, object left, object right) =>
        CallSig(MatDim(side), "Mult", ["Dim[]", "Dim[]"], ColAxis(right), RowAxis(left));

    private static void AssertRhsDispatch(Side side, object left, object? right, object? result)
    {
        Assert.NotNull(right);
        Assert.NotNull(result);
        Assert.True(Nested(side, "rc_dim").IsInstanceOfType(result));
        Assert.Equal(Get(left, "Rows"), Get(result!, "Rows"));
        Assert.Equal(Get(right!, "Cols"), Get(result!, "Cols"));
        Assert.Same(RowAxis(left), RowAxis(result!));
        Assert.Same(ColAxis(right!), ColAxis(result!));
    }

    private static void AssertColumnProduct(Side side, object left, object? right, object? result)
    {
        Assert.NotNull(right);
        object? factor = ExpectedFactor(side, left, right!);
        if (factor == null) { Assert.Null(result); return; }
        Assert.NotNull(result);
        Assert.True(Nested(side, "uniform_dim").IsInstanceOfType(result));
        Assert.Equal(Get(right, "Rows"), Get(result!, "Rows"));
        Assert.Equal(Get(left, "Cols"), Get(result!, "Cols"));
        Assert.Same(factor, Get(result!, "m_dim"));
    }

    private static void AssertRcProduct(Side side, object left, object? right, object? result)
    {
        Assert.NotNull(right);
        object? factor = ExpectedFactor(side, left, right!);
        if (factor == null) { Assert.Null(result); return; }
        Assert.NotNull(result);
        Assert.True(Nested(side, "r_dim").IsInstanceOfType(result));
        Assert.Equal(Get(right, "Rows"), Get(result!, "Rows"));
        Assert.Equal(Get(left, "Cols"), Get(result!, "Cols"));
        object expectedRows = CallSig(MatDim(side), "Mult", ["Dim", "Dim[]"], factor, RowAxis(right))!;
        object actualRows = RowAxis(result!)!;
        var expected = (Array)expectedRows; var actual = (Array)actualRows;
        Assert.NotSame(RowAxis(right), actualRows);
        Assert.Equal(expected.Length, actual.Length);
        for (int i = 0; i < expected.Length; i++) Assert.Same(expected.GetValue(i), actual.GetValue(i));
    }

    [SkippableFact]
    public void R_dim_mmult1_forwards_to_the_right_operand_overload()
    {
        Runtime.Sync();
        Assert.Equal(Mmult1Token, MethodFor(Side.Mathcad, "mmult1", "MatDim").MetadataToken);

        Compare("r_dim mmult1 dispatches to c_dim and preserves its axes", "mmult1", "MatDim", Mmult1Token,
            side => MakeRow(side, ["L", "T"], 3), side => MakeColumn(side, ["N", "v", "identity"], 4),
            AssertRhsDispatch);
        Compare("r_dim mmult1 dispatches to rc_dim base null fallback", "mmult1", "MatDim", Mmult1Token,
            side => MakeRow(side, ["L", "T"], 3), side => MakeRc(side, ["N", "v"], ["identity", "identity"]),
            (_, _, _, result) => Assert.Null(result));
        Compare("r_dim mmult1 null RHS throws", "mmult1", "MatDim", Mmult1Token,
            side => MakeRow(side, ["L"], 2), _ => null);
    }

    [SkippableFact]
    public void R_dim_mmult2_c_dim_uses_uniform_compatibility_factor()
    {
        Runtime.Sync();
        Assert.Equal(Mmult2ColumnToken, MethodFor(Side.Mathcad, "mmult2", "c_dim").MetadataToken);

        Compare("r_dim mmult2 c_dim compatible asymmetric factor", "mmult2", "c_dim", Mmult2ColumnToken,
            side => MakeRow(side, ["L", "L"], 3), side => MakeColumn(side, ["identity", "identity"], 4),
            AssertColumnProduct);
        Compare("r_dim mmult2 c_dim incompatible factors return null", "mmult2", "c_dim", Mmult2ColumnToken,
            side => MakeRow(side, ["L", "T"], 3), side => MakeColumn(side, ["identity", "identity"], 4),
            AssertColumnProduct);
        Compare("r_dim mmult2 c_dim null argument throws", "mmult2", "c_dim", Mmult2ColumnToken,
            side => MakeRow(side, ["L"], 2), _ => null);
    }

    [SkippableFact]
    public void R_dim_mmult2_rc_dim_builds_row_factor_products()
    {
        Runtime.Sync();
        Assert.Equal(Mmult2RcToken, MethodFor(Side.Mathcad, "mmult2", "rc_dim").MetadataToken);

        Compare("r_dim mmult2 rc_dim compatible asymmetric factor", "mmult2", "rc_dim", Mmult2RcToken,
            side => MakeRow(side, ["L", "L"], 3), side => MakeRc(side, ["T", "N"], ["identity", "identity"]),
            AssertRcProduct);
        Compare("r_dim mmult2 rc_dim incompatible factors return null", "mmult2", "rc_dim", Mmult2RcToken,
            side => MakeRow(side, ["L", "T"], 3), side => MakeRc(side, ["T", "N"], ["identity", "identity"]),
            AssertRcProduct);
        Compare("r_dim mmult2 rc_dim null argument throws", "mmult2", "rc_dim", Mmult2RcToken,
            side => MakeRow(side, ["L"], 2), _ => null);
    }
}
