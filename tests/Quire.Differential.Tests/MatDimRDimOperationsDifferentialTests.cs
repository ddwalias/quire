using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimOperationsDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int TransposeToken = 0x060007d2;
    private const int DetToken = 0x060007d3;
    private const int InvertToken = 0x060007d4;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object RowsArray, object?[] RowItems, int Rows, int Cols);

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");

    private static Array Dims(Side side, params string?[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(names[i] == null ? null : side.D(names[i]!), i);
        return result;
    }

    private static object Row(Side side, string?[] factors, int cols) =>
        Call(RDim(side), "make", Dims(side, factors), cols)!;

    private static MethodInfo Method(Side side, string name, Type[] parameters) =>
        RDim(side).GetMethod(name, DeclaredInstance, null, parameters, null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.{name}");

    private static Snapshot Capture(object target)
    {
        object rows = Get(target, "m_rowdim")!;
        return new(rows, ((Array)rows).Cast<object?>().ToArray(), (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);
    }

    private static string ArrayText(Side side, object? array) => array is Array values
        ? "[" + string.Join(",", values.Cast<object?>().Select(side.DimText)) + "]"
        : "null";

    private static string MatrixText(Side side, object? value)
    {
        if (value == null) return "null";
        if (!MatDim(side).IsInstanceOfType(value)) return side.Dim.IsInstanceOfType(value) ? side.DimText(value) : Text(value);
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            (CDim(side).IsInstanceOfType(value)
                ? $" coldim={ArrayText(side, Get(value, "m_coldim"))}"
                : $" rowdim={ArrayText(side, Get(value, "m_rowdim"))}");
    }

    private static string TargetState(Side side, object target, Snapshot before) =>
        MatrixText(side, target) + $" row-array-same={ReferenceEquals(before.RowsArray, Get(target, "m_rowdim"))}";

    private static void AssertUnchanged(object target, Snapshot before)
    {
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
        Assert.Same(before.RowsArray, Get(target, "m_rowdim"));
        var current = (Array)Get(target, "m_rowdim")!;
        Assert.Equal(before.RowItems.Length, current.Length);
        for (int i = 0; i < before.RowItems.Length; i++) Assert.Same(before.RowItems[i], current.GetValue(i));
    }

    private static void CompareTranspose(string caseId, string?[] factors, int cols)
    {
        object qReceiver = Row(Side.Quire, factors, cols), mReceiver = Row(Side.Mathcad, factors, cols);
        Snapshot qBefore = Capture(qReceiver), mBefore = Capture(mReceiver);
        MethodInfo qMethod = Method(Side.Quire, "transpose", Type.EmptyTypes);
        MethodInfo mMethod = Method(Side.Mathcad, "transpose", Type.EmptyTypes);
        Assert.Equal(TransposeToken, mMethod.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [], mMethod, mReceiver, [],
            value => { if (value != null) qResult = value; return MatrixText(Side.Quire, value); },
            value => { if (value != null) mResult = value; return MatrixText(Side.Mathcad, value); },
            target => TargetState(Side.Quire, target, qBefore), target => TargetState(Side.Mathcad, target, mBefore));

        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(CDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(CDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(qBefore.Cols, Get(qResult!, "Rows")); Assert.Equal(mBefore.Cols, Get(mResult!, "Rows"));
        Assert.Equal(qBefore.RowItems.Length, Get(qResult!, "Cols")); Assert.Equal(mBefore.RowItems.Length, Get(mResult!, "Cols"));
        Assert.Same(qBefore.RowsArray, Get(qResult!, "m_coldim")); Assert.Same(mBefore.RowsArray, Get(mResult!, "m_coldim"));
        AssertUnchanged(qReceiver, qBefore); AssertUnchanged(mReceiver, mBefore);
    }

    private static void CompareDet(string caseId, string?[] factors, int cols, int n)
    {
        object qReceiver = Row(Side.Quire, factors, cols), mReceiver = Row(Side.Mathcad, factors, cols);
        Snapshot qBefore = Capture(qReceiver), mBefore = Capture(mReceiver);
        MethodInfo qMethod = Method(Side.Quire, "det", [typeof(int)]);
        MethodInfo mMethod = Method(Side.Mathcad, "det", [typeof(int)]);
        Assert.Equal(DetToken, mMethod.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [n], mMethod, mReceiver, [n],
            value =>
            {
                if (value is not int && value != null) qResult = value;
                return value is int ? Text(value) : MatrixText(Side.Quire, value);
            },
            value =>
            {
                if (value is not int && value != null) mResult = value;
                return value is int ? Text(value) : MatrixText(Side.Mathcad, value);
            },
            target => TargetState(Side.Quire, target, qBefore), target => TargetState(Side.Mathcad, target, mBefore));

        object qExpected = CallSig(MatDim(Side.Quire), "Product", ["Dim", "Dim[]"], Get(Side.Quire.Dim, "identity"), Get(qReceiver, "m_rowdim"))!;
        object mExpected = CallSig(MatDim(Side.Mathcad), "Product", ["Dim", "Dim[]"], Get(Side.Mathcad.Dim, "identity"), Get(mReceiver, "m_rowdim"))!;
        Assert.Same(qExpected, qResult); Assert.Same(mExpected, mResult);
        AssertUnchanged(qReceiver, qBefore); AssertUnchanged(mReceiver, mBefore);
    }

    private static void CompareInvert(string caseId, string?[] factors, int cols)
    {
        object qReceiver = Row(Side.Quire, factors, cols), mReceiver = Row(Side.Mathcad, factors, cols);
        Snapshot qBefore = Capture(qReceiver), mBefore = Capture(mReceiver);
        MethodInfo qMethod = Method(Side.Quire, "invert", Type.EmptyTypes);
        MethodInfo mMethod = Method(Side.Mathcad, "invert", Type.EmptyTypes);
        Assert.Equal(InvertToken, mMethod.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [], mMethod, mReceiver, [],
            value => { if (value != null) qResult = value; return MatrixText(Side.Quire, value); },
            value => { if (value != null) mResult = value; return MatrixText(Side.Mathcad, value); },
            target => TargetState(Side.Quire, target, qBefore), target => TargetState(Side.Mathcad, target, mBefore));

        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(CDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(CDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(qBefore.Rows, Get(qResult!, "Rows")); Assert.Equal(mBefore.Rows, Get(mResult!, "Rows"));
        Assert.Equal(qBefore.RowItems.Length, Get(qResult!, "Cols")); Assert.Equal(mBefore.RowItems.Length, Get(mResult!, "Cols"));
        object qArray = Get(qResult!, "m_coldim")!; object mArray = Get(mResult!, "m_coldim")!;
        Assert.NotSame(qBefore.RowsArray, qArray); Assert.NotSame(mBefore.RowsArray, mArray);
        Assert.Equal(qBefore.RowItems.Length, ((Array)qArray).Length); Assert.Equal(mBefore.RowItems.Length, ((Array)mArray).Length);
        for (int i = 0; i < qBefore.RowItems.Length; i++)
        {
            object qExpected = CallSig(Side.Quire.Dim, "Pwr", ["Dim", "Double"], qBefore.RowItems[i], -1.0)!;
            object mExpected = CallSig(Side.Mathcad.Dim, "Pwr", ["Dim", "Double"], mBefore.RowItems[i], -1.0)!;
            Assert.Same(qExpected, ((Array)qArray).GetValue(i));
            Assert.Same(mExpected, ((Array)mArray).GetValue(i));
        }
        AssertUnchanged(qReceiver, qBefore); AssertUnchanged(mReceiver, mBefore);
    }

    [SkippableFact]
    public void R_dim_transpose_returns_c_dim_with_original_row_array()
    {
        Runtime.Sync();
        Assert.Equal(TransposeToken, Method(Side.Mathcad, "transpose", Type.EmptyTypes).MetadataToken);
        CompareTranspose("r_dim transpose asymmetric axes", ["L", "T", "N"], 4);
        CompareTranspose("r_dim transpose empty row axis", [], 3);
    }

    [SkippableFact]
    public void R_dim_det_folds_row_factors_from_identity_and_ignores_n()
    {
        Runtime.Sync();
        Assert.Equal(DetToken, Method(Side.Mathcad, "det", [typeof(int)]).MetadataToken);
        CompareDet("r_dim det ignores int.MinValue", ["L", "T", "N"], 3, int.MinValue);
        CompareDet("r_dim det ignores zero", ["L", "T", "N"], 3, 0);
        CompareDet("r_dim det ignores int.MaxValue", ["L", "T", "N"], 3, int.MaxValue);
        CompareDet("r_dim det empty row axis returns identity", [], 3, 7);
    }

    [SkippableFact]
    public void R_dim_invert_returns_c_dim_with_fresh_inverted_row_factors()
    {
        Runtime.Sync();
        Assert.Equal(InvertToken, Method(Side.Mathcad, "invert", Type.EmptyTypes).MetadataToken);
        CompareInvert("r_dim invert asymmetric factors", ["L", "T", "N"], 3);
        CompareInvert("r_dim invert identity factor", ["identity"], 2);
        CompareInvert("r_dim invert empty row axis", [], 4);
    }
}
