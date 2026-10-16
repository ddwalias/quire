using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimPickLoserDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x06000645;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side) =>
        side.T("Mcd.MatDim").GetMethod("PickLoser", StaticDeclared, null,
            [side.T("Mcd.MatDim"), side.T("Mcd.MatDim"), typeof(uint)], null)
        ?? throw new HarnessException($"Missing {side.Name} private MatDim.PickLoser");

    private static Array DimArray(Side side, params string[] names)
    {
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) values.SetValue(side.D(names[i]), i);
        return values;
    }

    private static object Row(Side side, string[] dimensions, int cols)
    {
        var type = Nested(side.T("Mcd.MatDim"), "r_dim");
        var make = type.GetMethod("make", StaticDeclared, null, [side.T("Mcd.Dim[]"), typeof(int)], null)!;
        return make.Invoke(null, [DimArray(side, dimensions), cols])!;
    }

    private static object Rc(Side side, string[] rows, string[] cols)
    {
        var type = Nested(side.T("Mcd.MatDim"), "rc_dim");
        var make = type.GetMethod("make_rc", StaticDeclared, null,
            [side.T("Mcd.Dim[]"), side.T("Mcd.Dim[]")], null)!;
        return make.Invoke(null, [DimArray(side, rows), DimArray(side, cols)])!;
    }

    private static object Uniform(Side side, string dimension, int rows, int cols)
    {
        var type = Nested(side.T("Mcd.MatDim"), "uniform_dim");
        var make = type.GetMethod("make", StaticDeclared, null,
            [side.T("Mcd.Dim"), typeof(int), typeof(int)], null)!;
        return make.Invoke(null, [side.D(dimension), rows, cols])!;
    }

    private static object? TryGet(object target, string member)
    {
        try { return Get(target, member); }
        catch (HarnessException) { return null; }
    }

    private sealed record SnapshotState(int Rows, int Cols, object? Uniform, object? RowArray, object? ColArray,
        object?[]? RowValues, object?[]? ColValues, object?[,] Cells);

    private static SnapshotState Snapshot(object target)
    {
        int rows = (int)Get(target, "Rows")!, cols = (int)Get(target, "Cols")!;
        object? rowArray = TryGet(target, "m_rowdim") ?? TryGet(target, "RowDimensions");
        object? colArray = TryGet(target, "m_coldim") ?? TryGet(target, "ColDimensions");
        var cells = new object?[rows, cols];
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++) cells[i, j] = Call(target, "dim", i, j);
        return new SnapshotState(rows, cols, TryGet(target, "UniformDim") ?? TryGet(target, "m_dim"), rowArray, colArray,
            rowArray is Array ra ? ra.Cast<object?>().ToArray() : null,
            colArray is Array ca ? ca.Cast<object?>().ToArray() : null, cells);
    }

    private static void AssertUnchanged(object target, SnapshotState before)
    {
        Assert.Equal(before.Rows, Get(target, "Rows")); Assert.Equal(before.Cols, Get(target, "Cols"));
        Assert.Same(before.Uniform, TryGet(target, "UniformDim") ?? TryGet(target, "m_dim"));
        object? rowArray = TryGet(target, "m_rowdim") ?? TryGet(target, "RowDimensions");
        object? colArray = TryGet(target, "m_coldim") ?? TryGet(target, "ColDimensions");
        Assert.Same(before.RowArray, rowArray); Assert.Same(before.ColArray, colArray);
        AssertArrayUnchanged(before.RowValues, rowArray); AssertArrayUnchanged(before.ColValues, colArray);
        for (int i = 0; i < before.Rows; i++)
        for (int j = 0; j < before.Cols; j++) Assert.Same(before.Cells[i, j], Call(target, "dim", i, j));
    }

    private static void AssertArrayUnchanged(object?[]? before, object? current)
    {
        if (before == null) return;
        Assert.IsAssignableFrom<Array>(current);
        var values = (Array)current!;
        Assert.Equal(before.Length, values.Length);
        for (int i = 0; i < before.Length; i++) Assert.Same(before[i], values.GetValue(i));
    }

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : value == null ? "null" : side.DimText(value);

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (side.T("Mcd.MatDim").IsInstanceOfType(value))
        {
            string result = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
                $" uniform={side.DimText(TryGet(value, "UniformDim") ?? TryGet(value, "m_dim"))}" +
                $" rows={ArrayText(side, TryGet(value, "m_rowdim") ?? TryGet(value, "RowDimensions"))}" +
                $" cols={ArrayText(side, TryGet(value, "m_coldim") ?? TryGet(value, "ColDimensions"))}";
            int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!;
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++) result += $" ({i},{j})={side.DimText(Call(value, "dim", i, j))}";
            return result;
        }
        if (side.Dim.IsInstanceOfType(value)) return side.DimText(value);
        if (value is IFormattable scalar) return scalar.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
        return side.Show(value);
    }

    private static void Compare(string caseId, object qLoser, object qWinner, object mLoser, object mWinner,
        uint argument, string? expectedMessage, int[]? expectedIndices)
    {
        var qLoserBefore = Snapshot(qLoser); var qWinnerBefore = Snapshot(qWinner);
        var mLoserBefore = Snapshot(mLoser); var mWinnerBefore = Snapshot(mWinner);
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        object?[] qArgs = [qLoser, qWinner, argument], mArgs = [mLoser, mWinner, argument];
        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => Show(Side.Quire, value), originalShow: value => Show(Side.Mathcad, value));
        AssertUnchanged(qLoser, qLoserBefore); AssertUnchanged(qWinner, qWinnerBefore);
        AssertUnchanged(mLoser, mLoserBefore); AssertUnchanged(mWinner, mWinnerBefore);
        if (expectedMessage != null) AssertOriginalDiagnostic(mLoser, mWinner, argument, expectedMessage, expectedIndices ?? []);
    }

    private static void AssertOriginalDiagnostic(object loser, object winner, uint argument,
        string messagePrefix, int[] indices)
    {
        var method = Method(Side.Mathcad);
        Exception? failure = null;
        try { method.Invoke(null, [loser, winner, argument]); }
        catch (TargetInvocationException e) { failure = e.InnerException ?? e; }
        Assert.NotNull(failure);
        Assert.Equal("AppException", failure!.GetType().Name);
        Assert.Equal(argument, Get(failure, "Argument"));
        Assert.Equal(0u, Get(failure, "Handle"));
        string expected = messagePrefix;
        if (indices.Length > 0)
        {
            int row = indices[0], col = indices.Length > 1 ? indices[1] : 0;
            object? winningDimension = Call(winner, "dim", row, col);
            object fixedDimension = Call(Side.Mathcad.T("Mcd.Dim"), "FixDimension", winningDimension)!;
            string format = (string)Call(fixedDimension, "Format")!;
            expected += $"\t{format}";
            foreach (int index in indices) expected += $"\t{index}";
        }
        Assert.Equal(expected, Get(failure, "Message"));
    }

    [SkippableFact]
    public void PickLoser_reports_vector_and_row_major_matrix_failures_or_structure_mismatch()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("PickLoser vector scans rows and reports the first differing row",
            Row(Side.Quire, ["L", "T"], 1), Row(Side.Quire, ["L", "N"], 1),
            Row(Side.Mathcad, ["L", "T"], 1), Row(Side.Mathcad, ["L", "N"], 1),
            11u, "bad vector element dimension", [1]);

        var qLoser = Rc(Side.Quire, ["L", "T"], ["L", "T"]);
        var qWinner = Rc(Side.Quire, ["L", "N"], ["L", "v"]);
        var mLoser = Rc(Side.Mathcad, ["L", "T"], ["L", "T"]);
        var mWinner = Rc(Side.Mathcad, ["L", "N"], ["L", "v"]);
        Compare("PickLoser matrix scans row-major: first difference is (0,1)", qLoser, qWinner, mLoser, mWinner,
            23u, "bad matrix element dimension", [0, 1]);

        Compare("PickLoser raises array unit mismatch when all loser cells agree",
            Row(Side.Quire, ["L", "L"], 2), Uniform(Side.Quire, "L", 2, 2),
            Row(Side.Mathcad, ["L", "L"], 2), Uniform(Side.Mathcad, "L", 2, 2),
            37u, "array unit mismatch", []);
    }
}
