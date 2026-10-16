using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimEqFactoriesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int EqMatrixToken = 0x06000617;
    private const int EqDimensionToken = 0x06000618;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name, System.Type second) =>
        side.T("Mcd.MatDim").GetMethod(name, StaticDeclared, null,
            [side.T("Mcd.MatDim"), second, typeof(uint)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{name}");

    private static Array DimArray(Side side, params string[] names)
    {
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) values.SetValue(side.D(names[i]), i);
        return values;
    }

    private static object RowDimension(Side side, string[] names, int cols)
    {
        var rowType = Nested(side.T("Mcd.MatDim"), "r_dim");
        var make = rowType.GetMethod("make", StaticDeclared, null, [side.T("Mcd.Dim[]"), typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} r_dim.make(Dim[], int)");
        return make.Invoke(null, [DimArray(side, names), cols])!;
    }

    private static object UniformDimension(Side side, string name, int rows, int cols)
    {
        var uniformType = Nested(side.T("Mcd.MatDim"), "uniform_dim");
        var make = uniformType.GetMethod("make", StaticDeclared, null,
            [side.T("Mcd.Dim"), typeof(int), typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} uniform_dim.make(Dim, int, int)");
        return make.Invoke(null, [side.D(name), rows, cols])!;
    }

    private static object RcDimension(Side side, string[] rows, string[] cols)
    {
        var rcType = Nested(side.T("Mcd.MatDim"), "rc_dim");
        var make = rcType.GetMethod("make_rc", StaticDeclared, null,
            [side.T("Mcd.Dim[]"), side.T("Mcd.Dim[]")], null)
            ?? throw new HarnessException($"Missing {side.Name} rc_dim.make_rc(Dim[], Dim[])");
        return make.Invoke(null, [DimArray(side, rows), DimArray(side, cols)])!;
    }

    private static string DimensionArray(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : value == null ? "null" : side.Show(value);

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (side.T("Mcd.MatDim").IsInstanceOfType(value))
        {
            string result = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
                $" uniform={side.DimText(Get(value, "UniformDim"))}";
            foreach (string member in new[] { "RowDimensions", "ColDimensions" })
                result += $" {member}={DimensionArray(side, Get(value, member))}";
            int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!;
            if ((long)rows * cols <= 16)
                for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++) result += $" ({i},{j})={side.DimText(Call(value, "dim", i, j))}";
            return result;
        }
        if (side.Dim.IsInstanceOfType(value)) return side.DimText(value);
        if (value is IFormattable scalar) return scalar.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
        return side.Show(value);
    }

    private sealed record MatDimSnapshot(int Rows, int Cols, object? Uniform, object? RowArray,
        object? ColArray, object?[]? RowValues, object?[]? ColValues, object?[,] Cells);

    private static MatDimSnapshot Snapshot(object target)
    {
        int rows = (int)Get(target, "Rows")!, cols = (int)Get(target, "Cols")!;
        object? rowArray = Get(target, "RowDimensions"), colArray = Get(target, "ColDimensions");
        var cells = new object?[rows, cols];
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++) cells[i, j] = Call(target, "dim", i, j);
        return new MatDimSnapshot(rows, cols, Get(target, "UniformDim"), rowArray, colArray,
            rowArray is Array ra ? ra.Cast<object?>().ToArray() : null,
            colArray is Array ca ? ca.Cast<object?>().ToArray() : null,
            cells);
    }

    private static void AssertUnchanged(object target, MatDimSnapshot before)
    {
        Assert.Equal(before.Rows, Get(target, "Rows"));
        Assert.Equal(before.Cols, Get(target, "Cols"));
        Assert.Same(before.Uniform, Get(target, "UniformDim"));
        object? rowArray = Get(target, "RowDimensions"), colArray = Get(target, "ColDimensions");
        Assert.Same(before.RowArray, rowArray);
        Assert.Same(before.ColArray, colArray);
        if (before.RowValues != null)
        {
            Assert.IsAssignableFrom<Array>(rowArray);
            var values = (Array)rowArray!;
            Assert.Equal(before.RowValues.Length, values.Length);
            for (int i = 0; i < values.Length; i++) Assert.Same(before.RowValues[i], values.GetValue(i));
        }
        if (before.ColValues != null)
        {
            Assert.IsAssignableFrom<Array>(colArray);
            var values = (Array)colArray!;
            Assert.Equal(before.ColValues.Length, values.Length);
            for (int i = 0; i < values.Length; i++) Assert.Same(before.ColValues[i], values.GetValue(i));
        }
        for (int i = 0; i < before.Rows; i++)
        for (int j = 0; j < before.Cols; j++) Assert.Same(before.Cells[i, j], Call(target, "dim", i, j));
    }

    private static void Compare(string caseId, string secondType, object? qFirst, object? qSecond,
        object? mFirst, object? mSecond, uint param, int token)
    {
        MatDimSnapshot? qFirstState = qFirst == null ? null : Snapshot(qFirst);
        MatDimSnapshot? qSecondState = qSecond != null && Side.Quire.T("Mcd.MatDim").IsInstanceOfType(qSecond) ? Snapshot(qSecond) : null;
        MatDimSnapshot? mFirstState = mFirst == null ? null : Snapshot(mFirst);
        MatDimSnapshot? mSecondState = mSecond != null && Side.Mathcad.T("Mcd.MatDim").IsInstanceOfType(mSecond) ? Snapshot(mSecond) : null;
        var qMethod = Method(Side.Quire, "Eq", Side.Quire.T(secondType));
        var mMethod = Method(Side.Mathcad, "Eq", Side.Mathcad.T(secondType));
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qFirst, qSecond, param],
            mMethod, null, [mFirst, mSecond, param],
            quireShow: value => Show(Side.Quire, value), originalShow: value => Show(Side.Mathcad, value));
        if (qFirstState != null) AssertUnchanged(qFirst!, qFirstState);
        if (qSecondState != null) AssertUnchanged(qSecond!, qSecondState);
        if (mFirstState != null) AssertUnchanged(mFirst!, mFirstState);
        if (mSecondState != null) AssertUnchanged(mSecond!, mSecondState);
    }

    [SkippableFact]
    public void Eq_factories_return_or_report_original_diagnostics_without_mutating_inputs()
    {
        Runtime.Sync();
        Assert.Equal(EqMatrixToken, Method(Side.Mathcad, "Eq", Side.Mathcad.T("Mcd.MatDim")).MetadataToken);
        Assert.Equal(EqDimensionToken, Method(Side.Mathcad, "Eq", Side.Mathcad.T("Mcd.Dim")).MetadataToken);

        Compare("Eq(MatDim,MatDim) equal row dimensions return normally", "Mcd.MatDim",
            RowDimension(Side.Quire, ["L", "T"], 2), RowDimension(Side.Quire, ["L", "T"], 2),
            RowDimension(Side.Mathcad, ["L", "T"], 2), RowDimension(Side.Mathcad, ["L", "T"], 2),
            31u, EqMatrixToken);
        Compare("Eq reports the first differing vector row and winner dimension", "Mcd.MatDim",
            RowDimension(Side.Quire, ["L", "T"], 1), RowDimension(Side.Quire, ["L", "N"], 1),
            RowDimension(Side.Mathcad, ["L", "T"], 1), RowDimension(Side.Mathcad, ["L", "N"], 1),
            47u, EqMatrixToken);
        Compare("Eq reports the first differing matrix coordinate and winner dimension", "Mcd.MatDim",
            RowDimension(Side.Quire, ["L", "T"], 2), RowDimension(Side.Quire, ["L", "N"], 2),
            RowDimension(Side.Mathcad, ["L", "T"], 2), RowDimension(Side.Mathcad, ["L", "N"], 2),
            53u, EqMatrixToken);
        Compare("Eq reports array unit mismatch when structures differ but cells agree", "Mcd.MatDim",
            RowDimension(Side.Quire, ["L", "L"], 2), UniformDimension(Side.Quire, "L", 2, 2),
            RowDimension(Side.Mathcad, ["L", "L"], 2), UniformDimension(Side.Mathcad, "L", 2, 2),
            61u, EqMatrixToken);
        var qRcLoser = RcDimension(Side.Quire, ["L", "T"], ["L", "T"]);
        var qRcWinner = RcDimension(Side.Quire, ["L", "N"], ["L", "v"]);
        var mRcLoser = RcDimension(Side.Mathcad, ["L", "T"], ["L", "T"]);
        var mRcWinner = RcDimension(Side.Mathcad, ["L", "N"], ["L", "v"]);
        Compare("Eq scans row-major: (0,0) matches, then (0,1) wins over (1,0)", "Mcd.MatDim",
            qRcLoser, qRcWinner, mRcLoser, mRcWinner, 67u, EqMatrixToken);
        AssertOriginalMatrixDiagnostic(mRcLoser, mRcWinner, 67u);
        Compare("Eq(MatDim,Dim) equal units return normally", "Mcd.Dim",
            UniformDimension(Side.Quire, "L", 2, 2), Side.Quire.D("L"),
            UniformDimension(Side.Mathcad, "L", 2, 2), Side.Mathcad.D("L"),
            71u, EqDimensionToken);
        Compare("Eq(MatDim,Dim) mismatch reports the supplied unit and argument", "Mcd.Dim",
            UniformDimension(Side.Quire, "L", 2, 2), Side.Quire.D("T"),
            UniformDimension(Side.Mathcad, "L", 2, 2), Side.Mathcad.D("T"),
            83u, EqDimensionToken);
    }

    private static void AssertOriginalMatrixDiagnostic(object loser, object winner, uint argument)
    {
        var method = Method(Side.Mathcad, "Eq", Side.Mathcad.T("Mcd.MatDim"));
        Exception? failure = null;
        try { method.Invoke(null, [loser, winner, argument]); }
        catch (TargetInvocationException e) { failure = e.InnerException ?? e; }
        Assert.NotNull(failure);
        Assert.Equal("AppException", failure!.GetType().Name);
        Assert.Equal(argument, Get(failure, "Argument"));
        var winnerRows = (Array)Get(winner, "m_rowdim")!;
        var winnerCols = (Array)Get(winner, "m_coldim")!;
        object expectedUnit = CallSig(Side.Mathcad.T("Mcd.Dim"), "Mult", ["Dim", "Dim"],
            winnerRows.GetValue(0), winnerCols.GetValue(1))!;
        object fixedUnit = Call(Side.Mathcad.T("Mcd.Dim"), "FixDimension", expectedUnit)!;
        string expectedMessage = $"bad matrix element dimension\t{Call(fixedUnit, "Format")}\t0\t1";
        Assert.Equal(expectedMessage, Get(failure, "Message"));
    }
}
