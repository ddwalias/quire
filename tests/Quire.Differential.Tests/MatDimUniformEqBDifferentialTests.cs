using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimUniformEqBDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x060007a2;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixSnapshot(object Matrix, int Rows, int Cols, int Offset, object? Anr, object? Flags,
        object? Dimension, object? RealArray, object? ImagArray, object? Table, double[]? RealValues,
        double[]? ImagValues, Dictionary<int, object?>? Pointers);
    private sealed record StateSnapshot(int Rows, int Cols, object? Uniform, object? RowArray, object? ColArray,
        object?[]? RowValues, object?[]? ColValues, MatrixSnapshot? Matrix);

    private static MethodInfo Method(Side side)
    {
        var type = Nested(side.T("Mcd.MatDim"), "uniform_dim");
        return type.GetMethod("eq_b", DeclaredInstance, null, [side.T("Mcd.MatDim")], null)
            ?? throw new HarnessException($"Missing {side.Name} uniform_dim.eq_b(MatDim)");
    }

    private static Array DimArray(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static object Uniform(Side side, string dimension, int rows, int cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dimension), rows, cols)!;

    private static object Axis(Side side, string kind, string[] dimensions, int invariantSize) =>
        Call(Nested(side.T("Mcd.MatDim"), kind), "make", DimArray(side, dimensions), invariantSize)!;

    private static object Rc(Side side, string[] rows, string[] cols) =>
        Call(Nested(side.T("Mcd.MatDim"), "rc_dim"), "make_rc", DimArray(side, rows), DimArray(side, cols))!;

    private static object Crazy(Side side)
    {
        object matrix = New(side.T("Mcd.MatVal"), 2, 3);
        Set(matrix, "m_dim", side.D("L"));
        var type = Nested(side.T("Mcd.MatDim"), "crazy_dim");
        var make = type.GetMethod("make", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            null, [side.T("Mcd.MatVal")], null) ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");
        return make.Invoke(null, [matrix])!;
    }

    private static object? TryGet(object target, string name)
    {
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static object? UniformValue(object target) => TryGet(target, "m_dim") ?? TryGet(target, "UniformDim");
    private static object? RowArray(object target) => TryGet(target, "m_rowdim") ?? TryGet(target, "RowDimensions");
    private static object? ColArray(object target) => TryGet(target, "m_coldim") ?? TryGet(target, "ColDimensions");

    private static MatrixSnapshot? SnapshotMatrix(object? matrix)
    {
        if (matrix == null) return null;
        object data = Get(matrix, "Data")!;
        object? real = TryGet(matrix, "m_rarr"), imag = TryGet(matrix, "m_iarr"), table = TryGet(matrix, "m_parr");
        return new MatrixSnapshot(matrix, (int)Get(matrix, "Rows")!, (int)Get(matrix, "Cols")!,
            (int)Get(matrix, "m_offset")!, TryGet(data, "m_anr"), TryGet(data, "m_flags"), TryGet(matrix, "m_dim"),
            real, imag, table, real is double[] r ? r.ToArray() : null, imag is double[] i ? i.ToArray() : null,
            table is System.Collections.Hashtable pointers
                ? pointers.Cast<System.Collections.DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => entry.Value)
                : null);
    }

    private static StateSnapshot Snapshot(object value)
    {
        object? rowArray = RowArray(value), colArray = ColArray(value);
        MatrixSnapshot? matrix = SnapshotMatrix(TryGet(value, "m_matrix"));
        return new StateSnapshot((int)Get(value, "Rows")!, (int)Get(value, "Cols")!, UniformValue(value),
            rowArray, colArray, rowArray is Array rows ? rows.Cast<object?>().ToArray() : null,
            colArray is Array cols ? cols.Cast<object?>().ToArray() : null, matrix);
    }

    private static void AssertUnchanged(object value, StateSnapshot before)
    {
        Assert.Equal(before.Rows, Get(value, "Rows"));
        Assert.Equal(before.Cols, Get(value, "Cols"));
        Assert.Same(before.Uniform, UniformValue(value));
        object? rowArray = RowArray(value), colArray = ColArray(value);
        Assert.Same(before.RowArray, rowArray);
        Assert.Same(before.ColArray, colArray);
        AssertArrayUnchanged(before.RowValues, rowArray);
        AssertArrayUnchanged(before.ColValues, colArray);
        AssertMatrixUnchanged(before.Matrix, TryGet(value, "m_matrix"));
    }

    private static void AssertMatrixUnchanged(MatrixSnapshot? before, object? current)
    {
        if (before == null)
        {
            Assert.Null(current);
            return;
        }
        Assert.Same(before.Matrix, current);
        Assert.Equal(before.Rows, Get(current!, "Rows")); Assert.Equal(before.Cols, Get(current!, "Cols"));
        Assert.Equal(before.Offset, Get(current!, "m_offset"));
        object data = Get(current!, "Data")!;
        Assert.Equal(before.Anr, TryGet(data, "m_anr")); Assert.Equal(before.Flags, TryGet(data, "m_flags"));
        Assert.Same(before.Dimension, TryGet(current!, "m_dim"));
        Assert.Same(before.RealArray, TryGet(current!, "m_rarr")); Assert.Same(before.ImagArray, TryGet(current!, "m_iarr"));
        Assert.Same(before.Table, TryGet(current!, "m_parr"));
        AssertDoubleArrayUnchanged(before.RealValues, TryGet(current!, "m_rarr"));
        AssertDoubleArrayUnchanged(before.ImagValues, TryGet(current!, "m_iarr"));
        var table = TryGet(current!, "m_parr") as System.Collections.Hashtable;
        if (before.Pointers == null) Assert.Null(table);
        else
        {
            Assert.NotNull(table);
            Assert.Equal(before.Pointers.Count, table!.Count);
            foreach (var pair in before.Pointers) Assert.Same(pair.Value, table[pair.Key]);
        }
    }

    private static void AssertDoubleArrayUnchanged(double[]? before, object? current)
    {
        if (before == null)
        {
            Assert.Null(current);
            return;
        }
        Assert.IsAssignableFrom<double[]>(current);
        var values = (double[])current!;
        Assert.Equal(before.Length, values.Length);
        for (int i = 0; i < before.Length; i++) Assert.Equal(before[i], values[i]);
    }

    private static void AssertArrayUnchanged(object?[]? before, object? after)
    {
        if (before == null) return;
        Assert.IsAssignableFrom<Array>(after);
        var current = (Array)after!;
        Assert.Equal(before.Length, current.Length);
        for (int i = 0; i < before.Length; i++) Assert.Same(before[i], current.GetValue(i));
    }

    private static string ArrayText(Side side, object? array) => array is Array values
        ? "[" + string.Join(",", values.Cast<object?>().Select(side.DimText)) + "]"
        : array == null ? "null" : side.DimText(array);

    private static string MatrixText(Side side, object? matrix)
    {
        if (matrix == null) return "null";
        object data = Get(matrix, "Data")!;
        object? anr = data.GetType().GetField("m_anr")?.GetValue(data);
        string Reals(object? value) => value is double[] values
            ? "[" + string.Join(",", values.Select(v => v.ToString("R", System.Globalization.CultureInfo.InvariantCulture))) + "]"
            : "null";
        return $"{Get(matrix, "Rows")}x{Get(matrix, "Cols")} offset={Get(matrix, "m_offset")}" +
            $" anr={anr}" +
            $" dim={side.DimText(TryGet(matrix, "m_dim"))}" +
            $" real={Reals(TryGet(matrix, "m_rarr"))} imag={Reals(TryGet(matrix, "m_iarr"))}" +
            $" table-null={TryGet(matrix, "m_parr") == null}";
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (!side.T("Mcd.MatDim").IsInstanceOfType(value)) return side.Show(value);
        string text = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" unit={side.DimText(UniformValue(value))}" +
            $" rowdim={ArrayText(side, RowArray(value))}" +
            $" coldim={ArrayText(side, ColArray(value))}" +
            $" matrix={MatrixText(side, TryGet(value, "m_matrix"))}";
        int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!;
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++) text += $" ({r},{c})={side.DimText(Call(value, "dim", r, c))}";
        return text;
    }

    private static string State(Side side, object target)
    {
        object? uniform = UniformValue(target), rows = RowArray(target), cols = ColArray(target), matrix = TryGet(target, "m_matrix");
        return $"{target.GetType().Name} {Get(target, "Rows")}x{Get(target, "Cols")}" +
            $" unit={side.DimText(uniform)} unit-self-ref={ReferenceEquals(uniform, UniformValue(target))}" +
            $" rows-ref-self={ReferenceEquals(rows, RowArray(target))}:{ArrayText(side, rows)}" +
            $" cols-ref-self={ReferenceEquals(cols, ColArray(target))}:{ArrayText(side, cols)}" +
            $" matrix-present={matrix != null}";
    }

    private static void Compare(string caseId, object qTarget, object? qOther, object mTarget, object? mOther)
    {
        var qTargetBefore = Snapshot(qTarget); var mTargetBefore = Snapshot(mTarget);
        var qOtherBefore = qOther == null ? null : Snapshot(qOther);
        var mOtherBefore = mOther == null ? null : Snapshot(mOther);
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [qOther], mMethod, mTarget, [mOther],
            value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
            target => State(Side.Quire, target), target => State(Side.Mathcad, target));
        AssertUnchanged(qTarget, qTargetBefore); AssertUnchanged(mTarget, mTargetBefore);
        if (qOther != null) AssertUnchanged(qOther, qOtherBefore!);
        if (mOther != null) AssertUnchanged(mOther, mOtherBefore!);
    }

    [SkippableFact]
    public void Uniform_eq_b_virtual_dispatches_to_the_rhs_eq2_b_overload()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("uniform equality uses equal stored units and ignores both shapes",
            Uniform(Side.Quire, "L", 2, 3), Uniform(Side.Quire, "L", 1, 1),
            Uniform(Side.Mathcad, "L", 2, 3), Uniform(Side.Mathcad, "L", 1, 1));
        Compare("uniform equality rejects different stored units",
            Uniform(Side.Quire, "L", 2, 3), Uniform(Side.Quire, "T", 2, 3),
            Uniform(Side.Mathcad, "L", 2, 3), Uniform(Side.Mathcad, "T", 2, 3));
        Compare("uniform equality delegates to row RHS eq2_b(Dim)",
            Uniform(Side.Quire, "L", 2, 2), Axis(Side.Quire, "r_dim", ["L", "L"], 2),
            Uniform(Side.Mathcad, "L", 2, 2), Axis(Side.Mathcad, "r_dim", ["L", "L"], 2));
        Compare("uniform equality delegates to column RHS eq2_b(Dim)",
            Uniform(Side.Quire, "L", 2, 2), Axis(Side.Quire, "c_dim", ["L", "L"], 2),
            Uniform(Side.Mathcad, "L", 2, 2), Axis(Side.Mathcad, "c_dim", ["L", "L"], 2));
        Compare("uniform equality delegates to row-column RHS eq2_b(Dim)",
            Uniform(Side.Quire, "L", 2, 2), Rc(Side.Quire, ["L", "L"], ["L", "L"]),
            Uniform(Side.Mathcad, "L", 2, 2), Rc(Side.Mathcad, ["L", "L"], ["L", "L"]));
        Compare("uniform equality delegates to crazy RHS eq2_b(Dim)",
            Uniform(Side.Quire, "L", 2, 3), Crazy(Side.Quire),
            Uniform(Side.Mathcad, "L", 2, 3), Crazy(Side.Mathcad));
        Compare("uniform equality throws on null RHS through virtual call",
            Uniform(Side.Quire, "L", 2, 3), null,
            Uniform(Side.Mathcad, "L", 2, 3), null);
    }
}
