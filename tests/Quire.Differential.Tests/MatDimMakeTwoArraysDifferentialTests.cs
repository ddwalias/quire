using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimMakeTwoArraysDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x06000627;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Make(Side side) =>
        side.T("Mcd.MatDim").GetMethod("make", StaticDeclared, null,
            [side.T("Mcd.Dim[]"), side.T("Mcd.Dim[]")], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.make(Dim[], Dim[])");

    private static Array? DimArray(Side side, string[]? names)
    {
        if (names == null) return null;
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) values.SetValue(side.D(names[i]), i);
        return values;
    }

    private static object?[] Products(Side side, object scalar, Array dimensions) =>
        dimensions.Cast<object?>().Select(dimension => CallSig(side.T("Mcd.Dim"), "Mult", ["Dim", "Dim"], scalar, dimension)!).ToArray();

    private static bool Uniform(object?[] dimensions) => dimensions.Length > 0 && dimensions.All(dimension => ReferenceEquals(dimension, dimensions[0]));

    private static string FactorArray(Side side, object? actual, object? expected, Array? source)
    {
        if (actual is not Array values) return actual == null ? "null" : side.Show(actual);
        string factors = string.Join(",", values.Cast<object?>().Select((value, index) =>
            $"{side.DimText(value)}:{(expected is object?[] expectedValues && index < expectedValues.Length && ReferenceEquals(value, expectedValues[index]) ? "expected-ref" : "other-ref")}"));
        return $"len={values.Length} source-ref={(ReferenceEquals(actual, source) ? "yes" : "no")} [{factors}]";
    }

    private static string Result(Side side, object? value, Array? rows, Array? cols,
        object?[]? expectedRows, object?[]? expectedCols, object? expectedUniform)
    {
        if (value == null) return "null";
        int rowCount = (int)Get(value, "Rows")!, colCount = (int)Get(value, "Cols")!;
        object? uniform = Get(value, "UniformDim");
        string result = $"{value.GetType().Name}:{rowCount}x{colCount}" +
            $" uniform={side.DimText(uniform)} uniform-ref-expected={(ReferenceEquals(uniform, expectedUniform) ? "yes" : "no")}" +
            $" rowdims={FactorArray(side, Get(value, "RowDimensions"), expectedRows, rows)}" +
            $" coldims={FactorArray(side, Get(value, "ColDimensions"), expectedCols, cols)}";
        if ((long)rowCount * colCount <= 16)
            for (int i = 0; i < rowCount; i++)
            for (int j = 0; j < colCount; j++)
                result += $" ({i},{j})={side.DimText(Call(value, "dim", i, j))}";
        return result;
    }

    private static string Show(Side side, object? value, Array? rows, Array? cols,
        object?[]? expectedRows, object?[]? expectedCols, object? expectedUniform)
    {
        if (value != null && side.T("Mcd.MatDim").IsInstanceOfType(value))
            return Result(side, value, rows, cols, expectedRows, expectedCols, expectedUniform);
        if (value != null && side.T("Mcd.Dim[]").IsInstanceOfType(value))
            return "[" + string.Join(",", ((Array)value).Cast<object?>().Select(side.DimText)) + "]";
        return side.DimText(value);
    }

    private static void Compare(string caseId, string[]? rowNames, string[]? colNames)
    {
        var qRows = DimArray(Side.Quire, rowNames); var qCols = DimArray(Side.Quire, colNames);
        var mRows = DimArray(Side.Mathcad, rowNames); var mCols = DimArray(Side.Mathcad, colNames);
        string qRowsBefore = Show(Side.Quire, qRows, qRows, qCols, null, null, null);
        string qColsBefore = Show(Side.Quire, qCols, qRows, qCols, null, null, null);
        string mRowsBefore = Show(Side.Mathcad, mRows, mRows, mCols, null, null, null);
        string mColsBefore = Show(Side.Mathcad, mCols, mRows, mCols, null, null, null);
        object?[]? qExpectedRows = null, qExpectedCols = null, mExpectedRows = null, mExpectedCols = null;
        object? qUniform = null, mUniform = null;
        if (qRows != null && qCols != null && qRows.Length > 0 && qRows.Cast<object?>().All(d => ReferenceEquals(d, qRows.GetValue(0))))
        {
            qExpectedCols = Products(Side.Quire, qRows.GetValue(0)!, qCols);
            if (Uniform(qExpectedCols)) qUniform = qExpectedCols[0];
        }
        else if (qRows != null && qCols != null && qCols.Length > 0 && qCols.Cast<object?>().All(d => ReferenceEquals(d, qCols.GetValue(0))))
        {
            qExpectedRows = Products(Side.Quire, qCols.GetValue(0)!, qRows);
            if (Uniform(qExpectedRows)) qUniform = qExpectedRows[0];
        }
        else if (qRows != null && qCols != null)
        {
            qExpectedRows = qRows.Cast<object?>().ToArray(); qExpectedCols = qCols.Cast<object?>().ToArray();
        }
        if (mRows != null && mCols != null && mRows.Length > 0 && mRows.Cast<object?>().All(d => ReferenceEquals(d, mRows.GetValue(0))))
        {
            mExpectedCols = Products(Side.Mathcad, mRows.GetValue(0)!, mCols);
            if (Uniform(mExpectedCols)) mUniform = mExpectedCols[0];
        }
        else if (mRows != null && mCols != null && mCols.Length > 0 && mCols.Cast<object?>().All(d => ReferenceEquals(d, mCols.GetValue(0))))
        {
            mExpectedRows = Products(Side.Mathcad, mCols.GetValue(0)!, mRows);
            if (Uniform(mExpectedRows)) mUniform = mExpectedRows[0];
        }
        else if (mRows != null && mCols != null)
        {
            mExpectedRows = mRows.Cast<object?>().ToArray(); mExpectedCols = mCols.Cast<object?>().ToArray();
        }

        var qMethod = Make(Side.Quire); var mMethod = Make(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        object?[] qArgs = [qRows, qCols], mArgs = [mRows, mCols];
        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => Show(Side.Quire, value, qRows, qCols, qExpectedRows, qExpectedCols, qUniform),
            originalShow: value => Show(Side.Mathcad, value, mRows, mCols, mExpectedRows, mExpectedCols, mUniform));

        Assert.Same(qRows, qArgs[0]); Assert.Same(qCols, qArgs[1]);
        Assert.Same(mRows, mArgs[0]); Assert.Same(mCols, mArgs[1]);
        Assert.Equal(qRowsBefore, Show(Side.Quire, qRows, qRows, qCols, null, null, null));
        Assert.Equal(qColsBefore, Show(Side.Quire, qCols, qRows, qCols, null, null, null));
        Assert.Equal(mRowsBefore, Show(Side.Mathcad, mRows, mRows, mCols, null, null, null));
        Assert.Equal(mColsBefore, Show(Side.Mathcad, mCols, mRows, mCols, null, null, null));
    }

    [SkippableFact]
    public void Make_from_two_dimension_arrays_uses_row_then_column_uniformity_before_retaining_both_axes()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Make(Side.Mathcad).MetadataToken);

        Compare("uniform row factors take precedence and use column length for square output", ["L", "L"], ["T", "T", "T"]);
        Compare("row-uniform products with nonuniform columns produce column factors", ["L", "L"], ["T", "N"]);
        Compare("uniform columns produce row factors from nonuniform rows", ["L", "T"], ["L", "L", "L"]);
        Compare("nonuniform rows and columns retain both exact input arrays", ["L", "T"], ["N", "L", "T"]);
        Compare("empty rows throw while Uniform inspects element zero", [], ["L", "T"]);
        Compare("empty columns throw after nonuniform rows", ["L", "T"], []);
    }
}
