using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCDimMmultDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int Mmult1Token = 0x060007de;
    private const int MmultUniformToken = 0x060007df;
    private const int MmultRowToken = 0x060007e0;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(string Field, object Value, object?[] Items, int Rows, int Cols);

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");
    private static Type RcDim(Side side) => Nested(MatDim(side), "rc_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static Array Dims(Side side, string?[] values)
    {
        var array = Array.CreateInstance(side.Dim, values.Length);
        for (int i = 0; i < values.Length; i++) array.SetValue(values[i] == null ? null : side.D(values[i]!), i);
        return array;
    }

    private static object C(Side side, string?[] factors, int rows) => Call(CDim(side), "make", Dims(side, factors), rows)!;
    private static object R(Side side, string?[] factors, int cols) => Call(RDim(side), "make", Dims(side, factors), cols)!;
    private static object U(Side side, string? factor, int rows, int cols) => Call(UniformDim(side), "make", factor == null ? null : side.D(factor), rows, cols)!;

    private static Snapshot Capture(Side side, object matrix)
    {
        string field = CDim(side).IsInstanceOfType(matrix) || RcDim(side).IsInstanceOfType(matrix)
            ? "m_coldim"
            : RDim(side).IsInstanceOfType(matrix) ? "m_rowdim" : "m_dim";
        object value = Get(matrix, field)!;
        object?[] items = value is Array array ? array.Cast<object?>().ToArray() : [];
        return new(field, value, items, (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!);
    }

    private static string ArrayText(Side side, object? value) => value is not Array array
        ? "null"
        : "[" + string.Join(",", array.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d))) + "]";

    private static string MatrixText(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is bool) return value.ToString()!;
        if (side.Dim.IsInstanceOfType(value)) return "Dim " + side.DimText(value);
        if (!MatDim(side).IsInstanceOfType(value)) return Text(value);
        string axes = CDim(side).IsInstanceOfType(value)
            ? $" coldim={ArrayText(side, Get(value, "m_coldim"))}"
            : RDim(side).IsInstanceOfType(value)
                ? $" rowdim={ArrayText(side, Get(value, "m_rowdim"))}"
                : RcDim(side).IsInstanceOfType(value)
                    ? $" rowdim={ArrayText(side, Get(value, "m_rowdim"))} coldim={ArrayText(side, Get(value, "m_coldim"))}"
                    : $" dim={side.DimText(Get(value, "m_dim"))}";
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}{axes}";
    }

    private static string State(Side side, object value, Snapshot before) =>
        MatrixText(side, value) + $" {before.Field}-same={ReferenceEquals(before.Value, Get(value, before.Field))}";

    private static void AssertUnchanged(Side side, object matrix, Snapshot before)
    {
        Assert.Equal(before.Rows, Get(matrix, "Rows"));
        Assert.Equal(before.Cols, Get(matrix, "Cols"));
        Assert.Same(before.Value, Get(matrix, before.Field));
        if (before.Value is Array)
        {
            var current = (Array)Get(matrix, before.Field)!;
            Assert.Equal(before.Items.Length, current.Length);
            for (int i = 0; i < before.Items.Length; i++) Assert.Same(before.Items[i], current.GetValue(i));
        }
    }

    private static MethodInfo Method(Side side, string name, Type parameter) =>
        CDim(side).GetMethod(name, DeclaredInstance, null, [parameter], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.{name}");

    private static object? Compare(string caseId, string methodName, Func<Side, Type> parameter, object qReceiver,
        object? qArgument, object mReceiver, object? mArgument, int token, out object? mResult)
    {
        MethodInfo qMethod = Method(Side.Quire, methodName, parameter(Side.Quire));
        MethodInfo mMethod = Method(Side.Mathcad, methodName, parameter(Side.Mathcad));
        Assert.Equal(token, mMethod.MetadataToken);
        Snapshot qBefore = Capture(Side.Quire, qReceiver), mBefore = Capture(Side.Mathcad, mReceiver);
        Snapshot? qArgBefore = qArgument == null ? null : Capture(Side.Quire, qArgument);
        Snapshot? mArgBefore = mArgument == null ? null : Capture(Side.Mathcad, mArgument);
        object? qResult = null, mathcadResult = null;
        mResult = null;
        string Show(Side side, object? receiver, object? argument, object? value, bool quire)
        {
            if (value != null && MatDim(side).IsInstanceOfType(value) && !ReferenceEquals(value, receiver) && !ReferenceEquals(value, argument))
            {
                if (quire) qResult = value; else mathcadResult = value;
            }
            return MatrixText(side, value);
        }
        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [qArgument], mMethod, mReceiver, [mArgument],
            value => Show(Side.Quire, qReceiver, qArgument, value, true),
            value => Show(Side.Mathcad, mReceiver, mArgument, value, false),
            target => State(Side.Quire, target, qBefore), target => State(Side.Mathcad, target, mBefore));
        mResult = mathcadResult;
        AssertUnchanged(Side.Quire, qReceiver, qBefore); AssertUnchanged(Side.Mathcad, mReceiver, mBefore);
        if (qArgument != null) AssertUnchanged(Side.Quire, qArgument, qArgBefore!);
        if (mArgument != null) AssertUnchanged(Side.Mathcad, mArgument, mArgBefore!);
        return qResult;
    }

    private static void AssertScaled(Side side, object uniform, object receiver, object result)
    {
        var input = (Array)Get(receiver, "m_coldim")!;
        var output = (Array)Get(result, "m_coldim")!;
        Assert.NotSame(input, output);
        Assert.Equal(input.Length, output.Length);
        for (int i = 0; i < input.Length; i++)
        {
            object? expected = CallSig(side.Dim, "Mult", ["Dim", "Dim"], Get(uniform, "m_dim"), input.GetValue(i));
            Assert.Same(expected, output.GetValue(i));
        }
    }

    [SkippableFact]
    public void C_dim_mmult1_uses_rhs_virtual_dispatch()
    {
        Runtime.Sync();
        Assert.Equal(Mmult1Token, Method(Side.Mathcad, "mmult1", MatDim(Side.Mathcad)).MetadataToken);
        object qLeft = C(Side.Quire, ["L", "T"], 3), mLeft = C(Side.Mathcad, ["L", "T"], 3);
        object qUniform = U(Side.Quire, "L", 2, 4), mUniform = U(Side.Mathcad, "L", 2, 4);
        object? qResult = Compare("c_dim mmult1 uniform RHS dispatch", "mmult1", MatDim, qLeft, qUniform, mLeft, mUniform, Mmult1Token, out object? mResult);
        Assert.Null(qResult); Assert.Null(mResult); // uniform_dim inherits the base mmult2(c_dim) null fallback.

        object qRow = R(Side.Quire, ["T", "L"], 8), mRow = R(Side.Mathcad, ["T", "L"], 8);
        qResult = Compare("c_dim mmult1 row RHS dispatch", "mmult1", MatDim, qLeft, qRow, mLeft, mRow, Mmult1Token, out mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(UniformDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(UniformDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(3, Get(qResult!, "Rows")); Assert.Equal(8, Get(qResult!, "Cols"));
        Assert.Equal(3, Get(mResult!, "Rows")); Assert.Equal(8, Get(mResult!, "Cols"));
        object? qExpected = CallSig(MatDim(Side.Quire), "Mult", ["Dim[]", "Dim[]"], Get(qLeft, "m_coldim"), Get(qRow, "m_rowdim"));
        object? mExpected = CallSig(MatDim(Side.Mathcad), "Mult", ["Dim[]", "Dim[]"], Get(mLeft, "m_coldim"), Get(mRow, "m_rowdim"));
        Assert.Same(qExpected, Get(qResult!, "m_dim")); Assert.Same(mExpected, Get(mResult!, "m_dim"));

        object qIncompatible = R(Side.Quire, ["L", "N"], 8), mIncompatible = R(Side.Mathcad, ["L", "N"], 8);
        qResult = Compare("c_dim mmult1 row RHS incompatible factors", "mmult1", MatDim, qLeft, qIncompatible, mLeft, mIncompatible, Mmult1Token, out mResult);
        Assert.Null(qResult); Assert.Null(mResult);

        object qColumn = C(Side.Quire, ["L", "T"], 6), mColumn = C(Side.Mathcad, ["L", "T"], 6);
        qResult = Compare("c_dim mmult1 c_dim RHS base fallback", "mmult1", MatDim, qLeft, qColumn, mLeft, mColumn, Mmult1Token, out mResult);
        Assert.Null(qResult); Assert.Null(mResult);
        qResult = Compare("c_dim mmult1 null RHS throws", "mmult1", MatDim, qLeft, null, mLeft, null, Mmult1Token, out mResult);
        Assert.Null(qResult); Assert.Null(mResult);
    }

    [SkippableFact]
    public void C_dim_mmult2_uniform_scales_column_factors()
    {
        Runtime.Sync();
        Assert.Equal(MmultUniformToken, Method(Side.Mathcad, "mmult2", UniformDim(Side.Mathcad)).MetadataToken);
        object qLeft = C(Side.Quire, ["T", "N"], 3), mLeft = C(Side.Mathcad, ["T", "N"], 3);
        object qUniform = U(Side.Quire, "L", 2, 4), mUniform = U(Side.Mathcad, "L", 2, 4);
        object? qResult = Compare("c_dim mmult2 uniform L", "mmult2", UniformDim, qLeft, qUniform, mLeft, mUniform, MmultUniformToken, out object? mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(CDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(CDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(2, Get(qResult!, "Rows")); Assert.Equal(2, Get(qResult!, "Cols"));
        Assert.Equal(2, Get(mResult!, "Rows")); Assert.Equal(2, Get(mResult!, "Cols"));
        AssertScaled(Side.Quire, qUniform, qLeft, qResult!); AssertScaled(Side.Mathcad, mUniform, mLeft, mResult!);

        object qIdentity = U(Side.Quire, null, 5, 1), mIdentity = U(Side.Mathcad, null, 5, 1);
        Assert.Same(Get(Side.Quire.Dim, "identity"), Get(qIdentity, "m_dim"));
        Assert.Same(Get(Side.Mathcad.Dim, "identity"), Get(mIdentity, "m_dim"));
        qResult = Compare("c_dim mmult2 null factory factor normalizes to identity", "mmult2", UniformDim, qLeft, qIdentity, mLeft, mIdentity, MmultUniformToken, out mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.Equal(5, Get(qResult!, "Rows")); Assert.Equal(2, Get(qResult!, "Cols"));
        Assert.Equal(5, Get(mResult!, "Rows")); Assert.Equal(2, Get(mResult!, "Cols"));
        AssertScaled(Side.Quire, qIdentity, qLeft, qResult!); AssertScaled(Side.Mathcad, mIdentity, mLeft, mResult!);

        object qPolyLeft = C(Side.Quire, ["L", null], 1), mPolyLeft = C(Side.Mathcad, ["L", null], 1);
        qResult = Compare("c_dim mmult2 uniform preserves polymorphic null factor", "mmult2", UniformDim, qPolyLeft, qUniform, mPolyLeft, mUniform, MmultUniformToken, out mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.Equal(2, Get(qResult!, "Rows")); Assert.Equal(2, Get(qResult!, "Cols"));
        Assert.Equal(2, Get(mResult!, "Rows")); Assert.Equal(2, Get(mResult!, "Cols"));
        AssertScaled(Side.Quire, qUniform, qPolyLeft, qResult!); AssertScaled(Side.Mathcad, mUniform, mPolyLeft, mResult!);

        object qEmptyLeft = C(Side.Quire, [], 4), mEmptyLeft = C(Side.Mathcad, [], 4);
        qResult = Compare("c_dim mmult2 uniform with empty column axis", "mmult2", UniformDim, qEmptyLeft, qUniform, mEmptyLeft, mUniform, MmultUniformToken, out mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.Equal(2, Get(qResult!, "Rows")); Assert.Equal(0, Get(qResult!, "Cols"));
        Assert.Equal(2, Get(mResult!, "Rows")); Assert.Equal(0, Get(mResult!, "Cols"));
        AssertScaled(Side.Quire, qUniform, qEmptyLeft, qResult!); AssertScaled(Side.Mathcad, mUniform, mEmptyLeft, mResult!);

        qResult = Compare("c_dim mmult2 null uniform throws", "mmult2", UniformDim, qLeft, null, mLeft, null, MmultUniformToken, out mResult);
        Assert.Null(qResult); Assert.Null(mResult);
    }

    [SkippableFact]
    public void C_dim_mmult2_row_rhs_builds_rc_dim_with_axis_aliases()
    {
        Runtime.Sync();
        Assert.Equal(MmultRowToken, Method(Side.Mathcad, "mmult2", RDim(Side.Mathcad)).MetadataToken);
        object qLeft = C(Side.Quire, ["T", "N"], 3), mLeft = C(Side.Mathcad, ["T", "N"], 3);
        object qRow = R(Side.Quire, ["L", "N", "T"], 8), mRow = R(Side.Mathcad, ["L", "N", "T"], 8);
        object? qResult = Compare("c_dim mmult2 row RHS asymmetric axes", "mmult2", RDim, qLeft, qRow, mLeft, mRow, MmultRowToken, out object? mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(RcDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(RcDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(3, Get(qResult!, "Rows")); Assert.Equal(2, Get(qResult!, "Cols"));
        Assert.Equal(3, Get(mResult!, "Rows")); Assert.Equal(2, Get(mResult!, "Cols"));
        Assert.Same(Get(qRow, "m_rowdim"), Get(qResult!, "m_rowdim")); Assert.Same(Get(qLeft, "m_coldim"), Get(qResult!, "m_coldim"));
        Assert.Same(Get(mRow, "m_rowdim"), Get(mResult!, "m_rowdim")); Assert.Same(Get(mLeft, "m_coldim"), Get(mResult!, "m_coldim"));

        object qEmptyRow = R(Side.Quire, [], 5), mEmptyRow = R(Side.Mathcad, [], 5);
        qResult = Compare("c_dim mmult2 row RHS empty row axis", "mmult2", RDim, qLeft, qEmptyRow, mLeft, mEmptyRow, MmultRowToken, out mResult);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.Equal(0, Get(qResult!, "Rows")); Assert.Equal(2, Get(qResult!, "Cols"));
        Assert.Equal(0, Get(mResult!, "Rows")); Assert.Equal(2, Get(mResult!, "Cols"));
        Assert.Same(Get(qEmptyRow, "m_rowdim"), Get(qResult!, "m_rowdim"));
        Assert.Same(Get(mEmptyRow, "m_rowdim"), Get(mResult!, "m_rowdim"));

        qResult = Compare("c_dim mmult2 null row RHS throws", "mmult2", RDim, qLeft, null, mLeft, null, MmultRowToken, out mResult);
        Assert.Null(qResult); Assert.Null(mResult);
    }
}
