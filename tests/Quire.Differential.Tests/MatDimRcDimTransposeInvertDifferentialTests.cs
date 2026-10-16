using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimTransposeInvertDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int TransposeToken = 0x060007bf;
    private const int InvertToken = 0x060007c1;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Value, Array Rows, Array Cols, object?[] RowItems,
        object?[] ColItems, int RowCount, int ColCount);

    private static System.Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static System.Type Rc(Side side) => Reflect.Nested(MatDim(side), "rc_dim");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object MakeRc(Side side, string[] rows, string[] cols) =>
        Call(Rc(side), "make_rc", Dims(side, rows), Dims(side, cols))!;

    private static MethodInfo Method(Side side, string name) =>
        Rc(side).GetMethod(name, DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.{name}()");

    private static Array Rows(object value) => (Array)Get(value, "m_rowdim")!;
    private static Array Cols(object value) => (Array)Get(value, "m_coldim")!;

    private static Snapshot Capture(object value)
    {
        Array rows = Rows(value), cols = Cols(value);
        return new Snapshot(value, rows, cols, rows.Cast<object?>().ToArray(), cols.Cast<object?>().ToArray(),
            (int)Get(value, "Rows")!, (int)Get(value, "Cols")!);
    }

    private static void AssertArrayContents(Array array, IReadOnlyList<object?> items)
    {
        Assert.Equal(items.Count, array.Length);
        for (int i = 0; i < items.Count; i++) Assert.Same(items[i], array.GetValue(i));
    }

    private static void AssertUnchanged(Snapshot before)
    {
        Assert.Equal(before.RowCount, Get(before.Value, "Rows"));
        Assert.Equal(before.ColCount, Get(before.Value, "Cols"));
        Assert.Same(before.Rows, Rows(before.Value));
        Assert.Same(before.Cols, Cols(before.Value));
        AssertArrayContents(Rows(before.Value), before.RowItems);
        AssertArrayContents(Cols(before.Value), before.ColItems);
    }

    private static string DimArray(Side side, Array array) =>
        "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]";

    private static string MatrixText(Side side, object? value) => value == null ? "null" :
        $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")}; rows={DimArray(side, Rows(value))}; cols={DimArray(side, Cols(value))}";

    private static string Show(Side side, object? value) => value == null ? "null" :
        MatDim(side).IsInstanceOfType(value) ? MatrixText(side, value) : Text(value);

    private static void Compare(string caseId, string name, int token, string[] rows, string[] cols,
        Action<Side, object, object> assertResult)
    {
        object qInput = MakeRc(Side.Quire, rows, cols), mInput = MakeRc(Side.Mathcad, rows, cols);
        Snapshot qBefore = Capture(qInput), mBefore = Capture(mInput);
        MethodInfo qMethod = Method(Side.Quire, name), mMethod = Method(Side.Mathcad, name);
        Assert.Equal(token, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qInput, [], mMethod, mInput, [],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            target => MatrixText(Side.Quire, target), target => MatrixText(Side.Mathcad, target));

        object? qResult = qMethod.Invoke(qInput, null), mResult = mMethod.Invoke(mInput, null);
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        assertResult(Side.Quire, qInput, qResult!);
        assertResult(Side.Mathcad, mInput, mResult!);
        AssertUnchanged(qBefore); AssertUnchanged(mBefore);
    }

    private static void AssertTranspose(Side side, object input, object result)
    {
        Assert.IsType(Rc(side), result);
        Assert.Same(Cols(input), Rows(result));
        Assert.Same(Rows(input), Cols(result));
        Assert.Equal(Get(input, "Cols"), Get(result, "Rows"));
        Assert.Equal(Get(input, "Rows"), Get(result, "Cols"));
    }

    private static object? Inverse(Side side, object? dimension) => Call(side.Dim, "Pwr", dimension, -1.0);

    private static void AssertInverseArray(Side side, Array input, Array output)
    {
        Assert.NotSame(input, output);
        Assert.Equal(input.Length, output.Length);
        for (int i = 0; i < input.Length; i++)
            Assert.Same(Inverse(side, input.GetValue(i)), output.GetValue(i));
    }

    private static void AssertInvert(Side side, object input, object result)
    {
        Assert.IsType(Rc(side), result);
        Assert.Equal(Cols(input).Length, Get(result, "Rows"));
        Assert.Equal(Rows(input).Length, Get(result, "Cols"));
        AssertInverseArray(side, Cols(input), Rows(result));
        AssertInverseArray(side, Rows(input), Cols(result));
    }

    [SkippableFact]
    public void Rc_transpose_swaps_axis_arrays_and_invert_returns_fresh_inverted_axes()
    {
        Runtime.Sync();
        Assert.Equal(TransposeToken, Method(Side.Mathcad, "transpose").MetadataToken);
        Assert.Equal(InvertToken, Method(Side.Mathcad, "invert").MetadataToken);

        Compare("transpose asymmetric rc_dim swaps exact row and column array references", "transpose", TransposeToken,
            ["L", "T"], ["N", "v", "identity"], AssertTranspose);
        Compare("transpose with empty row axis swaps empty array identity", "transpose", TransposeToken,
            [], ["L", "T"], AssertTranspose);
        Compare("transpose with empty column axis swaps empty array identity", "transpose", TransposeToken,
            ["N", "v"], [], AssertTranspose);

        Compare("invert asymmetric rc_dim creates fresh arrays with exact inverse dimensions", "invert", InvertToken,
            ["L", "T"], ["N", "v", "identity"], AssertInvert);
        Compare("invert with an empty row axis returns fresh empty inverse axis", "invert", InvertToken,
            [], ["L", "T"], AssertInvert);
        Compare("invert with an empty column axis returns fresh empty inverse axis", "invert", InvertToken,
            ["N", "v"], [], AssertInvert);
    }
}
