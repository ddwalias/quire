using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimMmultDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int Mmult1Token = 0x060007bc;
    private const int Mmult2ColumnToken = 0x060007bd;
    private const int Mmult2RcToken = 0x060007be;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Value, object? Rows, object? Cols, object?[]? RowItems, object?[]? ColItems, int RowCount, int ColCount);

    private static System.Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static System.Type Nested(Side side, string name) => Reflect.Nested(MatDim(side), name);

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object MakeRc(Side side, string[] rows, string[] cols) =>
        Call(Nested(side, "rc_dim"), "make_rc", Dims(side, rows), Dims(side, cols))!;

    private static object MakeColumn(Side side, string[] cols, int rows) =>
        Call(Nested(side, "c_dim"), "make", Dims(side, cols), rows)!;

    private static object MakeUniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side, "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static MethodInfo Method(Side side, string name, System.Type parameter)
    {
        return Nested(side, "rc_dim").GetMethod(name, DeclaredInstance, null, [parameter], null)
            ?? throw new HarnessException($"Missing {side.Name} rc_dim.{name}({parameter.Name})");
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : value == null ? "null" : side.DimText(value);

    private static object? TryGet(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object? Rows(object target) => TryGet(target, "m_rowdim");
    private static object? Cols(object target) => TryGet(target, "m_coldim");

    private static Snapshot Capture(object value)
    {
        object? rows = Rows(value), cols = Cols(value);
        return new Snapshot(value, rows, cols,
            rows is Array ra ? ra.Cast<object?>().ToArray() : null,
            cols is Array ca ? ca.Cast<object?>().ToArray() : null,
            (int)Get(value, "Rows")!, (int)Get(value, "Cols")!);
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
        Assert.Same(before.Rows, Rows(before.Value));
        Assert.Same(before.Cols, Cols(before.Value));
        AssertArrayUnchanged(before.RowItems, Rows(before.Value));
        AssertArrayUnchanged(before.ColItems, Cols(before.Value));
    }

    private static string MatrixText(Side side, object? value)
    {
        if (value == null) return "null";
        string text = $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" rows={ArrayText(side, Rows(value))} cols={ArrayText(side, Cols(value))}";
        return text;
    }

    private static string Show(Side side, object? value) => value switch
    {
        null => "null",
        _ when MatDim(side).IsInstanceOfType(value) => MatrixText(side, value),
        _ when side.Dim.IsInstanceOfType(value) => side.DimText(value),
        _ => Text(value)
    };

    private static MethodInfo GetMethod(Side side, string methodName, string parameterType) =>
        Method(side, methodName, parameterType == "MatDim" ? MatDim(side) : Nested(side, parameterType));

    private static object? Invoke(MethodInfo method, object target, object? argument) =>
        method.Invoke(target, [argument]);

    private static void Compare(string caseId, string methodName, string parameterType, int token,
        Func<Side, object> leftFactory, Func<Side, object?> rightFactory,
        Action<Side, object, object?, object?>? assertResult = null)
    {
        object qLeft = leftFactory(Side.Quire), mLeft = leftFactory(Side.Mathcad);
        object? qRight = rightFactory(Side.Quire), mRight = rightFactory(Side.Mathcad);
        var qMethod = GetMethod(Side.Quire, methodName, parameterType);
        var mMethod = GetMethod(Side.Mathcad, methodName, parameterType);
        Assert.Equal(token, mMethod.MetadataToken);
        Snapshot qLeftBefore = Capture(qLeft), mLeftBefore = Capture(mLeft);
        Snapshot? qRightBefore = qRight == null ? null : Capture(qRight);
        Snapshot? mRightBefore = mRight == null ? null : Capture(mRight);

        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
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

    private static object? InvokeMatDim(Side side, string method, params object?[] args) =>
        Call(MatDim(side), method, args);

    private static void AssertColumnProducts(Side side, object result, object receiver, object right, object? factor)
    {
        Assert.IsType(Nested(side, "c_dim"), result);
        Assert.Equal(Get(right, "Rows"), Get(result, "Rows"));
        object? expected = InvokeMatDim(side, "Mult", factor, Cols(receiver));
        var actual = (Array)Cols(result)!;
        var expectedArray = (Array)expected!;
        Assert.Equal(expectedArray.Length, Get(result, "Cols"));
        Assert.Equal(expectedArray.Length, actual.Length);
        for (int i = 0; i < actual.Length; i++) Assert.Same(expectedArray.GetValue(i), actual.GetValue(i));
    }

    private static void AssertRcProducts(Side side, object result, object receiver, object right, object? factor)
    {
        Assert.IsType(Nested(side, "rc_dim"), result);
        object? expectedRows = InvokeMatDim(side, "Mult", factor, Rows(right));
        var actualRows = (Array)Rows(result)!;
        var expectedArray = (Array)expectedRows!;
        Assert.Equal(expectedArray.Length, Get(result, "Rows"));
        Assert.Equal(expectedArray.Length, actualRows.Length);
        Assert.Equal(((Array)Cols(receiver)!).Length, Get(result, "Cols"));
        for (int i = 0; i < actualRows.Length; i++) Assert.Same(expectedArray.GetValue(i), actualRows.GetValue(i));
        Assert.Same(Cols(receiver), Cols(result));
    }

    private static void AssertRcMmult1Products(Side side, object result, object left, object right, object? factor)
    {
        Assert.IsType(Nested(side, "rc_dim"), result);
        object? expectedRows = InvokeMatDim(side, "Mult", factor, Rows(left));
        var actualRows = (Array)Rows(result)!;
        var expectedArray = (Array)expectedRows!;
        Assert.Equal(expectedArray.Length, Get(result, "Rows"));
        Assert.Equal(expectedArray.Length, actualRows.Length);
        Assert.Equal(((Array)Cols(right)!).Length, Get(result, "Cols"));
        for (int i = 0; i < actualRows.Length; i++) Assert.Same(expectedArray.GetValue(i), actualRows.GetValue(i));
        Assert.Same(Cols(right), Cols(result));
    }

    [SkippableFact]
    public void Rc_multiplication_dispatches_by_operand_type_and_preserves_dimensions()
    {
        Runtime.Sync();
        Assert.Equal(Mmult1Token, GetMethod(Side.Mathcad, "mmult1", "MatDim").MetadataToken);
        Assert.Equal(Mmult2ColumnToken, GetMethod(Side.Mathcad, "mmult2", "c_dim").MetadataToken);
        Assert.Equal(Mmult2RcToken, GetMethod(Side.Mathcad, "mmult2", "rc_dim").MetadataToken);

        Compare("rc mmult1 on c_dim RHS uses inherited base mmult2(rc_dim) null fallback", "mmult1", "MatDim", Mmult1Token,
            side => MakeRc(side, ["L", "T"], ["N", "v", "identity"]),
            side => MakeColumn(side, ["L", "T"], 2));

        Compare("rc mmult1 dispatches to compatible rc_dim", "mmult1", "MatDim", Mmult1Token,
            side => MakeRc(side, ["N", "v"], ["identity", "identity"]),
            side => MakeRc(side, ["L", "L"], ["T", "N", "v"]),
            (side, left, right, result) => AssertRcMmult1Products(side, result!, left, right!, side.D("L")));

        Compare("rc mmult1 uses inherited base null fallback for uniform_dim", "mmult1", "MatDim", Mmult1Token,
            side => MakeRc(side, ["L", "T"], ["N", "v"]),
            side => MakeUniform(side, "identity", 2, 2));

        Compare("rc mmult1 null RHS dispatch throws", "mmult1", "MatDim", Mmult1Token,
            side => MakeRc(side, ["L"], ["T"]), _ => null);

        Compare("rc mmult2(c_dim) compatible asymmetric factors preserve output products", "mmult2", "c_dim", Mmult2ColumnToken,
            side => MakeRc(side, ["L", "L"], ["T", "N", "v"]),
            side => MakeColumn(side, ["identity", "identity"], 3),
            (side, left, right, result) => AssertColumnProducts(side, result!, left, right!, side.D("L")));

        Compare("rc mmult2(c_dim) incompatible inner factors return null", "mmult2", "c_dim", Mmult2ColumnToken,
            side => MakeRc(side, ["identity", "identity"], ["T", "N", "v"]),
            side => MakeColumn(side, ["L", "N"], 3));

        Compare("rc mmult2(rc_dim) compatible asymmetric factors preserve output products", "mmult2", "rc_dim", Mmult2RcToken,
            side => MakeRc(side, ["L", "L"], ["T", "N", "v"]),
            side => MakeRc(side, ["T", "identity", "v"], ["identity", "identity"]),
            (side, left, right, result) => AssertRcProducts(side, result!, left, right!, side.D("L")));

        Compare("rc mmult2(rc_dim) incompatible inner factors return null", "mmult2", "rc_dim", Mmult2RcToken,
            side => MakeRc(side, ["identity", "identity"], ["T", "N", "v"]),
            side => MakeRc(side, ["T", "identity", "v"], ["L", "N"]));
    }
}
