using System.Reflection;

namespace Quire.Differential.Tests;

public class DimValApplyMatValDifferentialTests
{
    private static MethodInfo ApplyMatVal(Side side)
    {
        var type = side.T("Mcd.DimVal");
        var dimVal = side.T("Mcd.DimVal");
        return type.GetMethod("ApplyMatVal", MplRunSides.All, null, [dimVal, dimVal], null)!;
    }

    private static object Matrix(Side side, double[] real, int rows, int cols) =>
        Reflect.New(side.T("Mcd.MatVal"), real, null, rows, cols)!;

    private static object Function(Side side) =>
        Reflect.New(side.T("Mpl.Run.DelegateFunction"),
            new Func<object, object, object, object, object>((a, b, c, d) => $"{a}|{b}|{c}|{d}"), 4)!;

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().Name == "MatVal")
            return MatValDifferentialTests.Storage(side, value);
        return side.Show(value);
    }

    [SkippableFact]
    public void ApplyMatVal_packs_matrix_arguments_and_applies_function()
    {
        Runtime.Sync();
        var qFunction = Function(Side.Quire);
        var mFunction = Function(Side.Mathcad);
        var qMatrix = Matrix(Side.Quire, [1.0, 2.0, 3.0, 4.0], 2, 2);
        var mMatrix = Matrix(Side.Mathcad, [1.0, 2.0, 3.0, 4.0], 2, 2);

        MethodComparison.Compare("four argument function receives packed matrix elements in row-major order", "Mcd.DimVal",
            ApplyMatVal(Side.Quire), null, [qFunction, qMatrix],
            ApplyMatVal(Side.Mathcad), null, [mFunction, mMatrix],
            quireShow: value => Show(Side.Quire, value),
            originalShow: value => Show(Side.Mathcad, value));
    }

    [SkippableFact]
    public void ApplyMatVal_rejects_a_non_matrix_argument()
    {
        Runtime.Sync();
        var qFunction = Function(Side.Quire);
        var mFunction = Function(Side.Mathcad);
        var qScalar = Side.Quire.Num(1, 0, "identity");
        var mScalar = Side.Mathcad.Num(1, 0, "identity");

        MethodComparison.Compare("scalar second argument fails the asMatrix conversion", "Mcd.DimVal",
            ApplyMatVal(Side.Quire), null, [qFunction, qScalar],
            ApplyMatVal(Side.Mathcad), null, [mFunction, mScalar],
            quireShow: value => Show(Side.Quire, value),
            originalShow: value => Show(Side.Mathcad, value));
    }
}
