using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimDetDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007c0;
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

    private static MethodInfo Method(Side side) =>
        Rc(side).GetMethod("det", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.det(int)");

    private static Array Rows(object value) => (Array)Get(value, "m_rowdim")!;
    private static Array Cols(object value) => (Array)Get(value, "m_coldim")!;

    private static Snapshot Capture(object value)
    {
        Array rows = Rows(value), cols = Cols(value);
        return new Snapshot(value, rows, cols, rows.Cast<object?>().ToArray(), cols.Cast<object?>().ToArray(),
            (int)Get(value, "Rows")!, (int)Get(value, "Cols")!);
    }

    private static void AssertUnchanged(Snapshot before)
    {
        Assert.Equal(before.RowCount, Get(before.Value, "Rows"));
        Assert.Equal(before.ColCount, Get(before.Value, "Cols"));
        Assert.Same(before.Rows, Rows(before.Value));
        Assert.Same(before.Cols, Cols(before.Value));
        Assert.Equal(before.RowItems.Length, Rows(before.Value).Length);
        Assert.Equal(before.ColItems.Length, Cols(before.Value).Length);
        for (int i = 0; i < before.RowItems.Length; i++) Assert.Same(before.RowItems[i], Rows(before.Value).GetValue(i));
        for (int i = 0; i < before.ColItems.Length; i++) Assert.Same(before.ColItems[i], Cols(before.Value).GetValue(i));
    }

    private static string MatrixText(Side side, object? value) => value == null ? "null" :
        $"{value.GetType().Name} {Get(value, "Rows")}x{Get(value, "Cols")}; rows=[{string.Join(",", Rows(value).Cast<object?>().Select(side.DimText))}]; cols=[{string.Join(",", Cols(value).Cast<object?>().Select(side.DimText))}]";

    private static string Show(Side side, object? value) => value == null ? "null" :
        side.Dim.IsInstanceOfType(value) ? side.DimText(value) : Text(value);

    private static object? Product(Side side, string[] rows, string[] cols)
    {
        object? product = side.D("identity");
        foreach (string name in rows) product = Call(side.Dim, "Mult", product, side.D(name));
        foreach (string name in cols) product = Call(side.Dim, "Mult", product, side.D(name));
        return product;
    }

    private static void Compare(string caseId, string[] rows, string[] cols, int n)
    {
        object qMatrix = MakeRc(Side.Quire, rows, cols), mMatrix = MakeRc(Side.Mathcad, rows, cols);
        Snapshot qBefore = Capture(qMatrix), mBefore = Capture(mMatrix);
        MethodInfo qMethod = Method(Side.Quire), mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);

        MethodComparison.Compare(caseId, Contract, qMethod, qMatrix, [n], mMethod, mMatrix, [n],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            target => MatrixText(Side.Quire, target), target => MatrixText(Side.Mathcad, target));

        object? qResult = qMethod.Invoke(qMatrix, [n]), mResult = mMethod.Invoke(mMatrix, [n]);
        Assert.Same(Product(Side.Quire, rows, cols), qResult);
        Assert.Same(Product(Side.Mathcad, rows, cols), mResult);
        AssertUnchanged(qBefore); AssertUnchanged(mBefore);
    }

    [SkippableFact]
    public void Rc_det_folds_rows_then_columns_from_identity_ignoring_n_and_preserving_inputs()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("det on asymmetric row and column factors with int.MinValue", ["L", "T"], ["N", "v"], int.MinValue);
        Compare("det on identical factors ignores zero n", ["L", "T"], ["N", "v"], 0);
        Compare("det on identical factors ignores positive n", ["L", "T"], ["N", "v"], int.MaxValue);
        Compare("polymorphic row factor makes the determinant polymorphic", ["L", "poly"], ["T"], 1);
        Compare("empty row axis leaves column product", [], ["T"], 9);
        Compare("empty column axis leaves row product", ["L", "T"], [], -9);
        Compare("both axes empty return canonical identity", [], [], 42);
    }
}
