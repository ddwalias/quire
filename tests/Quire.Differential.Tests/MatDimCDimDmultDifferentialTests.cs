using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimCDimDmultDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const int DMult1Token = 0x060007e1;
    private const int DMult2RowToken = 0x060007e2;
    private const int DMult2ColToken = 0x060007e3;

    private sealed record AxisSnapshot(object? Dimension, object? RowDim, object? ColDim, object? Uniform,
        object?[]? RowItems, object?[]? ColItems);

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Axis(Side side, string kind, string[] names, int invariant = 4)
    {
        var nested = Nested(side.T("Mcd.MatDim"), kind);
        return CallSig(nested, "make", ["Dim[]", "Int32"], Dims(side, names), invariant)!;
    }

    private static object Uniform(Side side, string name = "L") =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(name), 3, 4)!;

    private static object RC(Side side, string[] rows, string[] cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "rc_dim"), "make_rc", Dims(side, rows), Dims(side, cols))!;

    private static MethodInfo Method(Side side, string nestedName, string name, string parameterName)
    {
        var parameter = parameterName == "MatDim" ? side.T("Mcd.MatDim") : Nested(side.T("Mcd.MatDim"), parameterName);
        var method = Nested(side.T("Mcd.MatDim"), nestedName).GetMethod(name, DeclaredInstance, null, [parameter], null)
            ?? throw new HarnessException($"Missing {side.Name} MatDim+{nestedName}.{name}");
        return method;
    }

    private static AxisSnapshot Capture(object value) => new(TryGet(value, "m_dim"),
        TryGet(value, "m_rowdim"), TryGet(value, "m_coldim"), TryGet(value, "UniformDim"),
        Items(TryGet(value, "m_rowdim")), Items(TryGet(value, "m_coldim")));

    private static object? TryGet(object value, string name)
    {
        try { return Get(value, name); }
        catch (HarnessException) { return null; }
    }

    private static object?[]? Items(object? value) => value is Array array ? array.Cast<object?>().ToArray() : null;
    private static string DimArray(Side side, object? value) => value is not Array array
        ? "null" : $"{array.GetType().Name}[{string.Join(",", array.Cast<object?>().Select(side.DimText))}]";

    private static string State(Side side, AxisSnapshot before, object current)
    {
        var type = current.GetType().Name;
        var row = TryGet(current, "m_rowdim"); var col = TryGet(current, "m_coldim");
        string Alias(object? old, object? now) => ReferenceEquals(old, now) ? "same" : "changed";
        string ItemAlias(object?[]? old, object? now) => old != null && now is Array array &&
            old.Length == array.Length && old.Select((item, i) => ReferenceEquals(item, array.GetValue(i))).All(x => x) ? "same-items" : "changed-items";
        return $"{type} {Get(current, "Rows")}x{Get(current, "Cols")}" +
            $" dim={side.DimText(TryGet(current, "m_dim"))}:{Alias(before.Dimension, TryGet(current, "m_dim"))}" +
            $" row={DimArray(side, row)}:{Alias(before.RowDim, row)}:{ItemAlias(before.RowItems, row)}" +
            $" col={DimArray(side, col)}:{Alias(before.ColDim, col)}:{ItemAlias(before.ColItems, col)}" +
            $" uniform={side.DimText(TryGet(current, "UniformDim"))}:{Alias(before.Uniform, TryGet(current, "UniformDim"))}";
    }

    private static string Argument(Side side, AxisSnapshot before, object? value) =>
        value == null ? "null" : State(side, before, value);

    private static string ResultOrArgument(Side side, AxisSnapshot? argumentBefore, object? expected, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().Name is "uniform_dim" or "r_dim" or "c_dim" or "rc_dim" or "crazy_dim")
            return argumentBefore == null ? "matrix-dimension" : Argument(side, argumentBefore, value);
        return $"{side.DimText(value)} expected-ref={ReferenceEquals(value, expected)}";
    }

    private static object? Product(Side side, object? left, object? right) =>
        CallSig(side.T("Mcd.MatDim"), "Mult", ["Dim[]", "Dim[]"], left, right);

    private static void Compare(string caseId, string nested, string methodName, int token,
        object qLeft, object? qRight, object mLeft, object? mRight, string argumentType,
        object? qExpected, object? mExpected)
    {
        var qBefore = Capture(qLeft); var mBefore = Capture(mLeft);
        var qRightBefore = qRight == null ? null : Capture(qRight);
        var mRightBefore = mRight == null ? null : Capture(mRight);
        var qRightStateBefore = qRightBefore == null ? null : Argument(Side.Quire, qRightBefore, qRight);
        var mRightStateBefore = mRightBefore == null ? null : Argument(Side.Mathcad, mRightBefore, mRight);
        var qMethod = Method(Side.Quire, nested, methodName, argumentType);
        var mMethod = Method(Side.Mathcad, nested, methodName, argumentType);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => ResultOrArgument(Side.Quire, qRightBefore, qExpected, value),
            value => ResultOrArgument(Side.Mathcad, mRightBefore, mExpected, value),
            target => State(Side.Quire, qBefore, target), target => State(Side.Mathcad, mBefore, target));
        if (qRight != null)
            Assert.Equal(qRightStateBefore, Argument(Side.Quire, qRightBefore!, qRight));
        if (mRight != null)
            Assert.Equal(mRightStateBefore, Argument(Side.Mathcad, mRightBefore!, mRight));
    }

    [SkippableFact]
    public void Direct_c_dim_dmult_dispatch_and_array_products_match_pinned_IL()
    {
        Runtime.Sync();

        // c_dim.dmult1 forwards to the RHS virtual dmult2(c_dim): r_dim and c_dim select formulas;
        // uniform_dim and rc_dim use the inherited null fallback.
        var qC = Axis(Side.Quire, "c_dim", ["T", "T"]); var mC = Axis(Side.Mathcad, "c_dim", ["T", "T"]);
        var qR = Axis(Side.Quire, "r_dim", ["L", "L"]); var mR = Axis(Side.Mathcad, "r_dim", ["L", "L"]);
        Compare("c_dim.dmult1 dispatches to r_dim.dmult2(c_dim), compatible products preserve helper result",
            "c_dim", "dmult1", DMult1Token, qC, qR, mC, mR, "MatDim",
            Product(Side.Quire, Get(qC, "m_coldim")!, Get(qR, "m_rowdim")!),
            Product(Side.Mathcad, Get(mC, "m_coldim")!, Get(mR, "m_rowdim")!));

        var qRBad = Axis(Side.Quire, "r_dim", ["L", "N"]); var mRBad = Axis(Side.Mathcad, "r_dim", ["L", "N"]);
        Compare("c_dim.dmult1 r_dim dispatch returns null for inconsistent products",
            "c_dim", "dmult1", DMult1Token, qC, qRBad, mC, mRBad, "MatDim", null, null);
        var qREmpty = Axis(Side.Quire, "r_dim", []); var mREmpty = Axis(Side.Mathcad, "r_dim", []);
        Compare("c_dim.dmult1 r_dim dispatch propagates empty-factor indexing exception",
            "c_dim", "dmult1", DMult1Token, qC, qREmpty, mC, mREmpty, "MatDim", null, null);

        var qCArg = Axis(Side.Quire, "c_dim", ["L", "L"]); var mCArg = Axis(Side.Mathcad, "c_dim", ["L", "L"]);
        Compare("c_dim.dmult1 dispatches to c_dim.dmult2(c_dim)", "c_dim", "dmult1", DMult1Token,
            qC, qCArg, mC, mCArg, "MatDim",
            Product(Side.Quire, Get(qC, "m_coldim")!, Get(qCArg, "m_coldim")!),
            Product(Side.Mathcad, Get(mC, "m_coldim")!, Get(mCArg, "m_coldim")!));

        var qUniform = Uniform(Side.Quire); var mUniform = Uniform(Side.Mathcad);
        Compare("c_dim.dmult1 uniform RHS uses inherited null fallback", "c_dim", "dmult1", DMult1Token,
            qC, qUniform, mC, mUniform, "MatDim", null, null);
        var qRC = RC(Side.Quire, ["L", "T"], ["T", "N"]); var mRC = RC(Side.Mathcad, ["L", "T"], ["T", "N"]);
        Compare("c_dim.dmult1 rc_dim RHS uses inherited null fallback", "c_dim", "dmult1", DMult1Token,
            qC, qRC, mC, mRC, "MatDim", null, null);
        Compare("c_dim.dmult1 null RHS throws after virtual call", "c_dim", "dmult1", DMult1Token,
            qC, null, mC, null, "MatDim", null, null);

        // c_dim.dmult2(r_dim) multiplies argument row factors by receiver column factors.
        var qRows = Axis(Side.Quire, "r_dim", ["L", "L"]); var mRows = Axis(Side.Mathcad, "r_dim", ["L", "L"]);
        Compare("c_dim.dmult2(r_dim) returns exact common row-times-column product",
            "c_dim", "dmult2", DMult2RowToken, qC, qRows, mC, mRows, "r_dim",
            Product(Side.Quire, Get(qRows, "m_rowdim")!, Get(qC, "m_coldim")!),
            Product(Side.Mathcad, Get(mRows, "m_rowdim")!, Get(mC, "m_coldim")!));
        var qRowsBad = Axis(Side.Quire, "r_dim", ["L", "N"]); var mRowsBad = Axis(Side.Mathcad, "r_dim", ["L", "N"]);
        Compare("c_dim.dmult2(r_dim) rejects inconsistent factor products",
            "c_dim", "dmult2", DMult2RowToken, qC, qRowsBad, mC, mRowsBad, "r_dim", null, null);
        Compare("c_dim.dmult2(r_dim) null argument exception", "c_dim", "dmult2", DMult2RowToken,
            qC, null, mC, null, "r_dim", null, null);
        var qRowsEmpty = Axis(Side.Quire, "r_dim", []); var mRowsEmpty = Axis(Side.Mathcad, "r_dim", []);
        Compare("c_dim.dmult2(r_dim) empty RHS row factors throw from Mult",
            "c_dim", "dmult2", DMult2RowToken, qC, qRowsEmpty, mC, mRowsEmpty, "r_dim", null, null);
        var qCEmpty = Axis(Side.Quire, "c_dim", []); var mCEmpty = Axis(Side.Mathcad, "c_dim", []);
        var qRowsForEmptyCol = Axis(Side.Quire, "r_dim", ["L", "L"]);
        var mRowsForEmptyCol = Axis(Side.Mathcad, "r_dim", ["L", "L"]);
        Compare("c_dim.dmult2(r_dim) empty receiver column factors throw from Mult",
            "c_dim", "dmult2", DMult2RowToken, qCEmpty, qRowsForEmptyCol, mCEmpty, mRowsForEmptyCol, "r_dim", null, null);

        // c_dim.dmult2(c_dim) multiplies argument column factors by receiver column factors.
        var qCols = Axis(Side.Quire, "c_dim", ["L", "L"]); var mCols = Axis(Side.Mathcad, "c_dim", ["L", "L"]);
        Compare("c_dim.dmult2(c_dim) returns exact common column-times-column product",
            "c_dim", "dmult2", DMult2ColToken, qC, qCols, mC, mCols, "c_dim",
            Product(Side.Quire, Get(qCols, "m_coldim")!, Get(qC, "m_coldim")!),
            Product(Side.Mathcad, Get(mCols, "m_coldim")!, Get(mC, "m_coldim")!));
        var qColsBad = Axis(Side.Quire, "c_dim", ["L", "N"]); var mColsBad = Axis(Side.Mathcad, "c_dim", ["L", "N"]);
        Compare("c_dim.dmult2(c_dim) rejects inconsistent factor products",
            "c_dim", "dmult2", DMult2ColToken, qC, qColsBad, mC, mColsBad, "c_dim", null, null);
        Compare("c_dim.dmult2(c_dim) null argument exception", "c_dim", "dmult2", DMult2ColToken,
            qC, null, mC, null, "c_dim", null, null);
        var qColsEmpty = Axis(Side.Quire, "c_dim", []); var mColsEmpty = Axis(Side.Mathcad, "c_dim", []);
        Compare("c_dim.dmult2(c_dim) empty RHS column factors throw from Mult",
            "c_dim", "dmult2", DMult2ColToken, qC, qColsEmpty, mC, mColsEmpty, "c_dim", null, null);
        var qColsForEmptyCol = Axis(Side.Quire, "c_dim", ["L", "L"]);
        var mColsForEmptyCol = Axis(Side.Mathcad, "c_dim", ["L", "L"]);
        Compare("c_dim.dmult2(c_dim) empty receiver column factors throw from Mult",
            "c_dim", "dmult2", DMult2ColToken, qCEmpty, qColsForEmptyCol, mCEmpty, mColsForEmptyCol, "c_dim", null, null);
    }
}
