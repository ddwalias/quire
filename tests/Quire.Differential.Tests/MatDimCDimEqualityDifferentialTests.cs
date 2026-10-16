using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCDimEqualityDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int EqToken = 0x060007dc;
    private const int Eq2Token = 0x060007dd;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(string AxisField, object Array, object?[] Items, int Rows, int Cols);

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");

    private static Array Dims(Side side, string?[] values)
    {
        var array = Array.CreateInstance(side.Dim, values.Length);
        for (int i = 0; i < values.Length; i++) array.SetValue(values[i] == null ? null : side.D(values[i]!), i);
        return array;
    }

    private static object C(Side side, string?[] factors, int rows) => Call(CDim(side), "make", Dims(side, factors), rows)!;

    private static object R(Side side, string?[] factors, int cols) => Call(RDim(side), "make", Dims(side, factors), cols)!;

    private static Snapshot Capture(Side side, object matrix)
    {
        string axisField = CDim(side).IsInstanceOfType(matrix) ? "m_coldim" : "m_rowdim";
        object array = Get(matrix, axisField)!;
        return new(axisField, array, ((Array)array).Cast<object?>().ToArray(), (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!);
    }

    private static string DimArrayText(Side side, object? value) => value is not Array array
        ? "null"
        : "[" + string.Join(",", array.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d))) + "]";

    private static string MatrixText(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is bool) return value.ToString()!;
        if (side.Dim.IsInstanceOfType(value)) return "Dim " + side.DimText(value);
        if (!MatDim(side).IsInstanceOfType(value)) return Text(value);
        string axes = CDim(side).IsInstanceOfType(value)
            ? $" coldim={DimArrayText(side, Get(value, "m_coldim"))}"
            : RDim(side).IsInstanceOfType(value)
                ? $" rowdim={DimArrayText(side, Get(value, "m_rowdim"))}"
                : "";
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}{axes}";
    }

    private static string State(Side side, object value, Snapshot before) =>
        MatrixText(side, value) + $" axis-array-same={ReferenceEquals(before.Array, Get(value, before.AxisField))}";

    private static void AssertUnchanged(object matrix, Snapshot before)
    {
        Assert.Equal(before.Rows, Get(matrix, "Rows"));
        Assert.Equal(before.Cols, Get(matrix, "Cols"));
        Assert.Same(before.Array, Get(matrix, before.AxisField));
        var current = (Array)Get(matrix, before.AxisField)!;
        Assert.Equal(before.Items.Length, current.Length);
        for (int i = 0; i < before.Items.Length; i++) Assert.Same(before.Items[i], current.GetValue(i));
    }

    private static MethodInfo Method(Side side, string name, Type parameter) =>
        CDim(side).GetMethod(name, DeclaredInstance, null, [parameter], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.{name}");

    private static void CompareEq(string caseId, string?[] leftFactors, int leftRows, string?[]? rightFactors, int rightRows, bool? expected, bool rightIsRow = false)
    {
        object qLeft = C(Side.Quire, leftFactors, leftRows), mLeft = C(Side.Mathcad, leftFactors, leftRows);
        object? qRight = rightFactors == null ? null : rightIsRow ? R(Side.Quire, rightFactors, 7) : C(Side.Quire, rightFactors, rightRows);
        object? mRight = rightFactors == null ? null : rightIsRow ? R(Side.Mathcad, rightFactors, 7) : C(Side.Mathcad, rightFactors, rightRows);
        Snapshot qLeftBefore = Capture(Side.Quire, qLeft), mLeftBefore = Capture(Side.Mathcad, mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(Side.Quire, qRight), mRightBefore = mRight == null ? null : Capture(Side.Mathcad, mRight);
        MethodInfo qMethod = Method(Side.Quire, "eq_b", MatDim(Side.Quire));
        MethodInfo mMethod = Method(Side.Mathcad, "eq_b", MatDim(Side.Mathcad));
        Assert.Equal(EqToken, mMethod.MetadataToken);
        bool? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => { if (value is bool b) qResult = b; return MatrixText(Side.Quire, value); },
            value => { if (value is bool b) mResult = b; return MatrixText(Side.Mathcad, value); },
            target => State(Side.Quire, target, qLeftBefore), target => State(Side.Mathcad, target, mLeftBefore));
        Assert.Equal(expected, qResult); Assert.Equal(expected, mResult);
        AssertUnchanged(qLeft, qLeftBefore); AssertUnchanged(mLeft, mLeftBefore);
        if (qRight != null) AssertUnchanged(qRight, qRightBefore!);
        if (mRight != null) AssertUnchanged(mRight, mRightBefore!);
    }

    private static void CompareEq2(string caseId, string?[] leftFactors, int leftRows, string?[]? rightFactors, int rightRows, bool? expected)
    {
        object qLeft = C(Side.Quire, leftFactors, leftRows), mLeft = C(Side.Mathcad, leftFactors, leftRows);
        object? qRight = rightFactors == null ? null : C(Side.Quire, rightFactors, rightRows);
        object? mRight = rightFactors == null ? null : C(Side.Mathcad, rightFactors, rightRows);
        Snapshot qLeftBefore = Capture(Side.Quire, qLeft), mLeftBefore = Capture(Side.Mathcad, mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(Side.Quire, qRight), mRightBefore = mRight == null ? null : Capture(Side.Mathcad, mRight);
        MethodInfo qMethod = Method(Side.Quire, "eq2_b", CDim(Side.Quire));
        MethodInfo mMethod = Method(Side.Mathcad, "eq2_b", CDim(Side.Mathcad));
        Assert.Equal(Eq2Token, mMethod.MetadataToken);
        bool? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => { if (value is bool b) qResult = b; return MatrixText(Side.Quire, value); },
            value => { if (value is bool b) mResult = b; return MatrixText(Side.Mathcad, value); },
            target => State(Side.Quire, target, qLeftBefore), target => State(Side.Mathcad, target, mLeftBefore));
        Assert.Equal(expected, qResult); Assert.Equal(expected, mResult);
        AssertUnchanged(qLeft, qLeftBefore); AssertUnchanged(mLeft, mLeftBefore);
        if (qRight != null) AssertUnchanged(qRight, qRightBefore!);
        if (mRight != null) AssertUnchanged(mRight, mRightBefore!);
    }

    [SkippableFact]
    public void C_dim_eq_b_dispatches_to_rhs_and_ignores_rows()
    {
        Runtime.Sync();
        Assert.Equal(EqToken, Method(Side.Mathcad, "eq_b", MatDim(Side.Mathcad)).MetadataToken);
        CompareEq("c_dim eq_b equal values distinct dimension refs", ["L", "T"], 2, ["L", "T"], 9, true);
        CompareEq("c_dim eq_b ignores different Rows", ["L", "T"], 2, ["L", "T"], -4, true);
        CompareEq("c_dim eq_b differing column factor", ["L", "T"], 2, ["L", "N"], 2, false);
        CompareEq("c_dim eq_b differing column length", ["L"], 2, ["L", "T"], 2, false);
        CompareEq("c_dim eq_b empty arrays", [], 0, [], -1, true);
        CompareEq("c_dim eq_b rhs r_dim dispatch fallback", ["L", "T"], 2, ["L", "T"], 7, false, rightIsRow: true);
        CompareEq("c_dim eq_b null rhs throws", ["L"], 2, null, 0, null);
    }

    [SkippableFact]
    public void C_dim_eq2_b_compares_only_column_factor_arrays()
    {
        Runtime.Sync();
        Assert.Equal(Eq2Token, Method(Side.Mathcad, "eq2_b", CDim(Side.Mathcad)).MetadataToken);
        CompareEq2("c_dim eq2_b equal values distinct dimension refs", ["L", "T"], 2, ["L", "T"], 8, true);
        CompareEq2("c_dim eq2_b ignores different Rows", ["L", "T"], 2, ["L", "T"], -5, true);
        CompareEq2("c_dim eq2_b differing column factor", ["L", "T"], 2, ["L", "N"], 2, false);
        CompareEq2("c_dim eq2_b differing column length", ["L"], 2, ["L", "T"], 2, false);
        CompareEq2("c_dim eq2_b empty arrays", [], 0, [], 0, true);
        CompareEq2("c_dim eq2_b null rhs throws", ["L"], 2, null, 0, null);
    }
}
