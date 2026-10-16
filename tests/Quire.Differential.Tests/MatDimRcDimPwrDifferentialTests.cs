using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimPwrDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007c2;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object RowArray, object ColArray, object[] Rows, object[] Cols, int Height, int Width);

    private static MethodInfo Method(Side side) =>
        Nested(side.T("Mcd.MatDim"), "rc_dim").GetMethod("pwr", DeclaredInstance, null,
            [typeof(uint)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.pwr(uint)");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Rc(Side side, string[] rowNames, string[] colNames) =>
        Call(Nested(side.T("Mcd.MatDim"), "rc_dim"), "make_rc", Dims(side, rowNames), Dims(side, colNames))!;

    private static object[] Elements(object array) => ((Array)array).Cast<object>().ToArray();

    private static Snapshot Capture(object target)
    {
        object rowArray = Get(target, "m_rowdim")!;
        object colArray = Get(target, "m_coldim")!;
        return new(rowArray, colArray, Elements(rowArray), Elements(colArray),
            (int)Get(target, "Rows")!, (int)Get(target, "Cols")!);
    }

    private static object? ExpectedFactor(Side side, Snapshot before) =>
        CallSig(side.T("Mcd.MatDim"), "Mult", ["Dim[]", "Dim[]"], before.ColArray, before.RowArray);

    private static object? ExpectedPoweredRows(Side side, Snapshot before, uint exponent)
    {
        object? factor = ExpectedFactor(side, before);
        if (factor == null) return null;
        double wrappedExponent = (double)unchecked(exponent - 1u);
        object poweredFactor = CallSig(side.T("Mcd.Dim"), "Pwr", ["Dim", "Double"], factor, wrappedExponent)!;
        return CallSig(side.T("Mcd.MatDim"), "Mult", ["Dim", "Dim[]"], poweredFactor, before.RowArray);
    }

    private static void AssertUnchanged(object target, Snapshot before)
    {
        Assert.Same(before.RowArray, Get(target, "m_rowdim"));
        Assert.Same(before.ColArray, Get(target, "m_coldim"));
        Assert.Equal(before.Rows.Length, ((Array)Get(target, "m_rowdim")!).Length);
        Assert.Equal(before.Cols.Length, ((Array)Get(target, "m_coldim")!).Length);
        for (int i = 0; i < before.Rows.Length; i++) Assert.Same(before.Rows[i], ((Array)Get(target, "m_rowdim")!).GetValue(i));
        for (int i = 0; i < before.Cols.Length; i++) Assert.Same(before.Cols[i], ((Array)Get(target, "m_coldim")!).GetValue(i));
        Assert.Equal(before.Height, Get(target, "Rows"));
        Assert.Equal(before.Width, Get(target, "Cols"));
    }

    private static string Render(Side side, object? value, Snapshot before, object? expectedRows,
        Action<object>? retainResult)
    {
        if (value == null) return "null";
        if (!side.T("Mcd.MatDim").IsInstanceOfType(value)) return side.Show(value);
        retainResult?.Invoke(value);
        object rowArray = Get(value, "m_rowdim")!;
        object colArray = Get(value, "m_coldim")!;
        var rowValues = Elements(rowArray);
        var colValues = Elements(colArray);
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" row-alias={ReferenceEquals(rowArray, before.RowArray)} col-alias={ReferenceEquals(colArray, before.ColArray)}" +
            $" expected-rows={expectedRows != null && rowValues.Length == ((Array)expectedRows).Length && rowValues.Zip(Elements(expectedRows)).All(p => ReferenceEquals(p.First, p.Second))}" +
            $" rows=[{string.Join(",", rowValues.Select(side.DimText))}] cols=[{string.Join(",", colValues.Select(side.DimText))}]";
    }

    private static object? Compare(string caseId, string[] rowNames, string[] colNames, uint exponent)
    {
        object qReceiver = Rc(Side.Quire, rowNames, colNames);
        object mReceiver = Rc(Side.Mathcad, rowNames, colNames);
        var qBefore = Capture(qReceiver);
        var mBefore = Capture(mReceiver);
        object? qFactor = ExpectedFactor(Side.Quire, qBefore);
        object? mFactor = ExpectedFactor(Side.Mathcad, mBefore);
        object? qExpectedRows = ExpectedPoweredRows(Side.Quire, qBefore, exponent);
        object? mExpectedRows = ExpectedPoweredRows(Side.Mathcad, mBefore, exponent);
        object? qResult = null, mResult = null;
        var qMethod = Method(Side.Quire);
        var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qReceiver, [exponent], mMethod, mReceiver, [exponent],
            value => Render(Side.Quire, value, qBefore, qExpectedRows, result => qResult = result),
            value => Render(Side.Mathcad, value, mBefore, mExpectedRows, result => mResult = result),
            target => $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}" +
                $" row-array-unchanged={ReferenceEquals(Get(target, "m_rowdim"), qBefore.RowArray)}" +
                $" col-array-unchanged={ReferenceEquals(Get(target, "m_coldim"), qBefore.ColArray)}",
            target => $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}" +
                $" row-array-unchanged={ReferenceEquals(Get(target, "m_rowdim"), mBefore.RowArray)}" +
                $" col-array-unchanged={ReferenceEquals(Get(target, "m_coldim"), mBefore.ColArray)}");

        AssertUnchanged(qReceiver, qBefore);
        AssertUnchanged(mReceiver, mBefore);
        if (qFactor == null || mFactor == null)
        {
            Assert.Null(qFactor); Assert.Null(mFactor);
            Assert.Null(qResult); Assert.Null(mResult);
            return null;
        }

        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(Nested(Side.Quire.T("Mcd.MatDim"), "rc_dim").IsInstanceOfType(qResult));
        Assert.True(Nested(Side.Mathcad.T("Mcd.MatDim"), "rc_dim").IsInstanceOfType(mResult));
        Assert.Equal(qBefore.Height, Get(qResult!, "Rows")); Assert.Equal(qBefore.Width, Get(qResult!, "Cols"));
        Assert.Equal(mBefore.Height, Get(mResult!, "Rows")); Assert.Equal(mBefore.Width, Get(mResult!, "Cols"));
        Assert.NotSame(qBefore.RowArray, Get(qResult!, "m_rowdim"));
        Assert.NotSame(mBefore.RowArray, Get(mResult!, "m_rowdim"));
        Assert.Same(qBefore.ColArray, Get(qResult!, "m_coldim"));
        Assert.Same(mBefore.ColArray, Get(mResult!, "m_coldim"));
        var qExpectedItems = Elements(qExpectedRows!);
        var mExpectedItems = Elements(mExpectedRows!);
        var qResultItems = Elements(Get(qResult!, "m_rowdim")!);
        var mResultItems = Elements(Get(mResult!, "m_rowdim")!);
        Assert.Equal(qExpectedItems.Length, qResultItems.Length);
        Assert.Equal(mExpectedItems.Length, mResultItems.Length);
        for (int i = 0; i < qExpectedItems.Length; i++) Assert.Same(qExpectedItems[i], qResultItems[i]);
        for (int i = 0; i < mExpectedItems.Length; i++) Assert.Same(mExpectedItems[i], mResultItems[i]);
        return qResult;
    }

    [SkippableFact]
    public void Rc_dim_pwr_uses_compatible_factors_unsigned_exponents_and_original_axis_aliases()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("rc_dim pwr exponent zero uses uint wraparound", ["identity", "identity"], ["identity", "identity"], 0);
        Compare("rc_dim pwr exponent zero wraps a nonidentity factor", ["L", "L"], ["identity", "identity"], 0);
        Compare("rc_dim pwr exponent one preserves row dimensions", ["L", "L"], ["identity", "identity"], 1);
        Compare("rc_dim pwr exponent two", ["L", "L"], ["T", "T"], 2);
        Compare("rc_dim pwr uint maximum wraps before conversion", ["identity", "identity"], ["identity", "identity"], uint.MaxValue);
        Compare("rc_dim pwr rejects nonuniform factor", ["L", "T"], ["identity", "identity"], 2);
    }
}
