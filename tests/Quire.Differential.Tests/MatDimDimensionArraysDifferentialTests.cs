using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimDimensionArraysDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int RowDimensionArrayToken = 0x06000613;
    private const int ColDimensionArrayToken = 0x06000614;
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private sealed record TargetSnapshot(object Target, object? UniformDimension, object? Matrix,
        object? MatrixDimension, object? Real, object? Imaginary, object? Table,
        IReadOnlyDictionary<int, object?> Entries, IReadOnlyDictionary<int, object?> PointerDimensions);

    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatDim").GetMethod(name, DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{name}(int)");

    private static object Uniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static object Matrix(Side side, int rows = 2, int cols = 3, int stride = 4, int offset = 2,
        string dimension = "L")
    {
        var real = new double[offset + stride * cols + 8];
        var imag = new double[real.Length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            real[offset + col * stride + row] = 10 * col + row + 1;
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static object Crazy(Side side, object matrix)
    {
        var method = Nested(side.T("Mcd.MatDim"), "crazy_dim")
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(candidate => candidate.Name == "make" && candidate.GetParameters() is [{ ParameterType: var parameter }] && parameter == matrix.GetType());
        return method.Invoke(null, [matrix])!;
    }

    private static void Pointer(Side side, object matrix, int row, int col, string dimension) =>
        Call(matrix, "set_ptrs_2", row, col, side.Num(2 + row + col, 0, dimension));

    private static TargetSnapshot Capture(object target)
    {
        object? matrix = target.GetType().Name == "crazy_dim" ? Get(target, "m_matrix") : null;
        var table = matrix == null ? null : Get(matrix, "m_parr");
        var entries = table is Hashtable pointers
            ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
            : new Dictionary<int, object?>();
        var pointerDimensions = entries.Where(pair => pair.Value?.GetType().Name == "NumVal")
            .ToDictionary(pair => pair.Key, pair => Get(pair.Value!, "m_dim"));
        return new(target, target.GetType().Name == "uniform_dim" ? Get(target, "m_dim") : null,
            matrix, matrix == null ? null : Get(matrix, "m_dim"), matrix == null ? null : Get(matrix, "m_rarr"),
            matrix == null ? null : Get(matrix, "m_iarr"), table, entries, pointerDimensions);
    }

    private static string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";

    private static string ArrayValues(object? value, Side side) => value is double[] array
        ? "[" + string.Join(",", array.Select(item => side.Show(item))) + "]"
        : value == null ? "null" : side.Show(value);

    private static string Pointer(object? value, int index, TargetSnapshot before, Side side)
    {
        if (value == null) return "null";
        string identity = before.Entries.TryGetValue(index, out var old) && ReferenceEquals(old, value) ? "retained" : "object";
        if (value.GetType().Name != "NumVal") return $"{value.GetType().Name}:{identity}";
        object? dimension = Get(value, "m_dim");
        string dimRef = before.PointerDimensions.TryGetValue(index, out var oldDim) && ReferenceEquals(oldDim, dimension)
            ? "same-dim-ref" : "changed-dim-ref";
        return $"NumVal:{identity}:real={Get(value, "Real")}:imag={Get(value, "Imag")}:dim={side.DimText(dimension)}:{dimRef}";
    }

    private static string State(Side side, TargetSnapshot before, object target)
    {
        string common = $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")}";
        if (before.Matrix == null)
            return common + $" dimension={side.DimText(Get(target, "m_dim"))}:dim-ref={Ref(before.UniformDimension, Get(target, "m_dim"))}";

        object matrix = before.Matrix;
        var table = Get(matrix, "m_parr") as Hashtable;
        string Entries(IEnumerable<DictionaryEntry> entries) => string.Join(";", entries.OrderBy(entry => (int)entry.Key)
            .Select(entry => $"{entry.Key}={Pointer(entry.Value, (int)entry.Key, before, side)}"));
        return common + $" matrix-retained={ReferenceEquals(Get(target, "m_matrix"), matrix)} " +
            $"matrix={MatValDifferentialTests.Storage(side, matrix)}" +
            $" matrix-dim-ref={Ref(before.MatrixDimension, Get(matrix, "m_dim"))}" +
            $" real-ref={Ref(before.Real, Get(matrix, "m_rarr"))}:real={ArrayValues(Get(matrix, "m_rarr"), side)}:retained-real={ArrayValues(before.Real, side)}" +
            $" imag-ref={Ref(before.Imaginary, Get(matrix, "m_iarr"))}:imag={ArrayValues(Get(matrix, "m_iarr"), side)}:retained-imag={ArrayValues(before.Imaginary, side)}" +
            $" table-ref={Ref(before.Table, table)}:table-after={(table == null ? "null" : Entries(table.Cast<DictionaryEntry>()))}" +
            $" retained-table-after={(before.Table is Hashtable oldTable ? Entries(oldTable.Cast<DictionaryEntry>()) : "null")}";
    }

    private static string Result(Side side, object? result, TargetSnapshot before)
    {
        if (result is not Array array) return Text(result);
        string Alias(object? dimension)
        {
            if (ReferenceEquals(dimension, before.UniformDimension)) return "uniform-dim";
            if (ReferenceEquals(dimension, before.MatrixDimension)) return "matrix-dim";
            foreach (var pair in before.PointerDimensions)
                if (ReferenceEquals(dimension, pair.Value)) return $"pointer-dim@{pair.Key}";
            return "other-ref";
        }
        var values = array.Cast<object?>().Select((dimension, index) =>
            $"{index}:{side.DimText(dimension)}:{Alias(dimension)}");
        return $"{result!.GetType().Name}:[{string.Join(";", values)}]";
    }

    private static void Compare(string label, string methodName, int token, object qTarget, object mTarget, int index)
    {
        var qMethod = Method(Side.Quire, methodName);
        var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(token, mMethod.MetadataToken);
        var qBefore = Capture(qTarget);
        var mBefore = Capture(mTarget);
        MethodComparison.Compare(label, Contract, qMethod, qTarget, [index], mMethod, mTarget, [index],
            result => Result(Side.Quire, result, qBefore), result => Result(Side.Mathcad, result, mBefore),
            target => State(Side.Quire, qBefore, target), target => State(Side.Mathcad, mBefore, target));
    }

    [SkippableFact]
    public void RowDimensionArray_direct_base_method_matches_original()
    {
        Runtime.Sync();
        Assert.Equal(RowDimensionArrayToken, Method(Side.Mathcad, "RowDimensionArray").MetadataToken);

        Compare("RowDimensionArray uniform row returns fresh repeated dimension array", "RowDimensionArray", RowDimensionArrayToken,
            Uniform(Side.Quire, "T", 2, 3), Uniform(Side.Mathcad, "T", 2, 3), 1);
        Compare("RowDimensionArray zero columns returns empty array", "RowDimensionArray", RowDimensionArrayToken,
            Uniform(Side.Quire, "L", 2, 0), Uniform(Side.Mathcad, "L", 2, 0), 1);

        var qMatrix = Matrix(Side.Quire); var mMatrix = Matrix(Side.Mathcad);
        Pointer(Side.Quire, qMatrix, 1, 0, "T"); Pointer(Side.Mathcad, mMatrix, 1, 0, "T");
        Pointer(Side.Quire, qMatrix, 1, 2, "N"); Pointer(Side.Mathcad, mMatrix, 1, 2, "N");
        Compare("RowDimensionArray crazy matrix reads padded row cells and aliases pointer dimensions", "RowDimensionArray", RowDimensionArrayToken,
            Crazy(Side.Quire, qMatrix), Crazy(Side.Mathcad, mMatrix), 1);

        Compare("RowDimensionArray crazy matrix propagates physically invalid row lookup exception", "RowDimensionArray", RowDimensionArrayToken,
            Crazy(Side.Quire, Matrix(Side.Quire)), Crazy(Side.Mathcad, Matrix(Side.Mathcad)), 100);
    }

    [SkippableFact]
    public void ColDimensionArray_direct_base_method_matches_original()
    {
        Runtime.Sync();
        Assert.Equal(ColDimensionArrayToken, Method(Side.Mathcad, "ColDimensionArray").MetadataToken);

        Compare("ColDimensionArray uniform column returns fresh repeated dimension array", "ColDimensionArray", ColDimensionArrayToken,
            Uniform(Side.Quire, "T", 3, 2), Uniform(Side.Mathcad, "T", 3, 2), 1);
        Compare("ColDimensionArray zero rows returns empty array", "ColDimensionArray", ColDimensionArrayToken,
            Uniform(Side.Quire, "L", 0, 3), Uniform(Side.Mathcad, "L", 0, 3), 1);

        var qMatrix = Matrix(Side.Quire); var mMatrix = Matrix(Side.Mathcad);
        Pointer(Side.Quire, qMatrix, 0, 2, "T"); Pointer(Side.Mathcad, mMatrix, 0, 2, "T");
        Pointer(Side.Quire, qMatrix, 1, 2, "N"); Pointer(Side.Mathcad, mMatrix, 1, 2, "N");
        Compare("ColDimensionArray crazy matrix reads padded column cells and aliases pointer dimensions", "ColDimensionArray", ColDimensionArrayToken,
            Crazy(Side.Quire, qMatrix), Crazy(Side.Mathcad, mMatrix), 2);

        Compare("ColDimensionArray crazy matrix propagates physically invalid column lookup exception", "ColDimensionArray", ColDimensionArrayToken,
            Crazy(Side.Quire, Matrix(Side.Quire)), Crazy(Side.Mathcad, Matrix(Side.Mathcad)), 100);
    }
}
