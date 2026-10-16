using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimCrazyDimAccessDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007ae;
    private const int MatDimToken = 0x060007af;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record MatrixSnapshot(object? Real, object? Imag, object? Table, object? Dimension,
        Dictionary<int, (object? Value, object? Dimension)> Entries,
        List<(int Index, object Child, MatrixSnapshot Snapshot)> Children);

    private static System.Type CrazyType(Side side) => Nested(side.T("Mcd.MatDim"), "crazy_dim");

    private static MethodInfo Method(Side side, string name) =>
        CrazyType(side).GetMethod(name, DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} crazy_dim.{name}(int,int)");

    private static object Matrix(Side side, string dimension = "L", int rows = 2, int cols = 2,
        int stride = 3, int offset = 1)
    {
        int length = offset + stride * cols;
        var real = Enumerable.Range(0, length).Select(i => 10.0 + i).ToArray();
        var imag = new double[length];
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static MatrixSnapshot Capture(object matrix, int childDepth)
    {
        var table = Get(matrix, "m_parr");
        var entries = table is Hashtable pointers
            ? pointers.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry =>
                (Value: entry.Value, Dimension: entry.Value?.GetType().Name == "NumVal" ? Get(entry.Value, "m_dim") : null))
            : new Dictionary<int, (object? Value, object? Dimension)>();
        var children = new List<(int Index, object Child, MatrixSnapshot Snapshot)>();
        if (childDepth > 0)
            foreach (var (index, item) in entries)
                if (item.Value?.GetType().Name == "MatVal") children.Add((index, item.Value, Capture(item.Value, childDepth - 1)));
        return new MatrixSnapshot(Get(matrix, "m_rarr"), Get(matrix, "m_iarr"), table,
            Get(matrix, "m_dim"), entries, children);
    }

    private static string Ref(object? oldValue, object? newValue) => ReferenceEquals(oldValue, newValue) ? "same" : "changed";

    private static string TableText(Side side, object? value, IReadOnlyDictionary<int, (object? Value, object? Dimension)> before)
    {
        if (value is not Hashtable table) return "null";
        return "[" + string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key).Select(entry =>
        {
            int index = (int)entry.Key;
            object? current = entry.Value;
            before.TryGetValue(index, out var old);
            string same = old.Value == null ? "new" : ReferenceEquals(old.Value, current) ? "same-object" : "changed-object";
            if (current?.GetType().Name == "NumVal")
                return $"{index}:NumVal:{same}:{side.Show(current)}:dim-ref={Ref(old.Dimension, Get(current, "m_dim"))}";
            if (current?.GetType().Name == "MatVal") return $"{index}:MatVal:{same}";
            return $"{index}:{current?.GetType().Name ?? "null"}:{same}:{side.Show(current)}";
        })) + "]";
    }

    private static string MatrixState(Side side, object matrix, MatrixSnapshot prior, bool includeChildren)
    {
        var real = Get(matrix, "m_rarr");
        var imag = Get(matrix, "m_iarr");
        var table = Get(matrix, "m_parr");
        string state = MatValDifferentialTests.Storage(side, matrix) +
            $" immutable={Get(matrix, "Immutable")} shared={Get(matrix, "Shared")} packed={Get(matrix, "Packed")} isize={Get(matrix, "Isize")}" +
            $" real-ref={Ref(prior.Real, real)} prior-real-now={side.Show(prior.Real)}" +
            $" imag-ref={Ref(prior.Imag, imag)} prior-imag-now={side.Show(prior.Imag)}" +
            $" table-ref={Ref(prior.Table, table)} prior-table-now={TableText(side, prior.Table, prior.Entries)}" +
            $" dim-ref={Ref(prior.Dimension, Get(matrix, "m_dim"))}";
        if (includeChildren)
            foreach (var child in prior.Children.OrderBy(child => child.Index))
                state += $" child[{child.Index}]=" + MatrixState(side, child.Child, child.Snapshot, false);
        return state;
    }

    private static string TargetState(Side side, object target, object matrix, MatrixSnapshot snapshot) =>
        $"crazy_dim {Get(target, "Rows")}x{Get(target, "Cols")} source-same={ReferenceEquals(Get(target, "m_matrix"), matrix)} " +
        MatrixState(side, matrix, snapshot, true);

    private static object Crazy(Side side, object matrix)
    {
        var factory = CrazyType(side).GetMethod("make", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
            null, [side.T("Mcd.MatVal")], null)
            ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");
        return factory.Invoke(null, [matrix])!;
    }

    private static string ShowDim(Side side, object? selected, object? expectedDimension, object? result)
    {
        if (result == null) return "null";
        if (result.GetType() != side.Dim) return side.Show(result);
        return $"Dim {side.DimText(result)} same-selected-dimension={ReferenceEquals(result, expectedDimension)} " +
            $"same-selected-m_dim={selected != null && ReferenceEquals(result, Get(selected, "m_dim"))}";
    }

    private static string DescribeDimension(Side side, object? dimension) =>
        dimension == null ? "null" : side.DimText(dimension);

    private static string ShowMatDim(Side side, object? selected, object? expectedDimension, object? result)
    {
        if (result == null) return "null";
        if (!side.T("Mcd.MatDim").IsAssignableFrom(result.GetType())) return side.Show(result);
        string text = $"{result.GetType().Name} {Get(result, "Rows")}x{Get(result, "Cols")} uniform={DescribeDimension(side, Get(result, "UniformDim"))}";
        if (selected != null && selected.GetType().Name == "MatVal")
        {
            text += $" crazy-source-same={result.GetType().Name == "crazy_dim" && ReferenceEquals(Get(result, "m_matrix"), selected)}";
            object? rowDimensions = Get(result, "RowDimensions");
            object? colDimensions = Get(result, "ColDimensions");
            text += $" row-dims-same-child={rowDimensions != null && ReferenceEquals(rowDimensions, Get(selected, "m_rowdim"))}" +
                $" col-dims-same-child={colDimensions != null && ReferenceEquals(colDimensions, Get(selected, "m_coldim"))}";
        }
        if (result.GetType().Name == "uniform_dim")
        {
            object? unit = Get(result, "m_dim");
            text += $" same-selected-dimension={ReferenceEquals(unit, expectedDimension)} " +
                $"unit-is-identity={ReferenceEquals(unit, side.Marker("identity"))}";
        }
        return text;
    }

    private static void Compare(string caseId, string name, int token, object qMatrix, object mMatrix,
        int i, int j, object? qSelected = null, object? mSelected = null)
    {
        object qTarget = Crazy(Side.Quire, qMatrix);
        object mTarget = Crazy(Side.Mathcad, mMatrix);
        var qSnapshot = Capture(qMatrix, 1);
        var mSnapshot = Capture(mMatrix, 1);
        object? qExpectedDimension = qSelected == null ? Get(qMatrix, "m_dim") : Get(qSelected, "m_dim");
        object? mExpectedDimension = mSelected == null ? Get(mMatrix, "m_dim") : Get(mSelected, "m_dim");
        var qMethod = Method(Side.Quire, name);
        var mMethod = Method(Side.Mathcad, name);
        Assert.Equal(token, mMethod.MetadataToken);
        Assert.Equal(Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);
        string qBefore = TargetState(Side.Quire, qTarget, qMatrix, qSnapshot);
        string mBefore = TargetState(Side.Mathcad, mTarget, mMatrix, mSnapshot);
        Assert.Equal(mBefore, qBefore);

        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [i, j], mMethod, mTarget, [i, j],
            result => name == "dim"
                ? ShowDim(Side.Quire, qSelected, qExpectedDimension, result)
                : ShowMatDim(Side.Quire, qSelected, qExpectedDimension, result),
            result => name == "dim"
                ? ShowDim(Side.Mathcad, mSelected, mExpectedDimension, result)
                : ShowMatDim(Side.Mathcad, mSelected, mExpectedDimension, result),
            target => TargetState(Side.Quire, target, qMatrix, qSnapshot),
            target => TargetState(Side.Mathcad, target, mMatrix, mSnapshot));

        Assert.Equal(qBefore, TargetState(Side.Quire, qTarget, qMatrix, qSnapshot));
        Assert.Equal(mBefore, TargetState(Side.Mathcad, mTarget, mMatrix, mSnapshot));
    }

    private static (object Q, object M) Pair(string dimension = "L") =>
        (Matrix(Side.Quire, dimension), Matrix(Side.Mathcad, dimension));

    [SkippableFact]
    public void Crazy_dim_dim_reads_scalar_and_live_pointer_dimensions_and_forwards_boundaries()
    {
        Runtime.Sync();
        Assert.Equal(DimToken, Method(Side.Mathcad, "dim").MetadataToken);

        var (qScalar, mScalar) = Pair("L");
        Compare("crazy_dim.dim scalar-backed first cell", "dim", DimToken, qScalar, mScalar, 0, 0);
        Compare("crazy_dim.dim scalar-backed last logical cell", "dim", DimToken, qScalar, mScalar, 1, 1);
        Compare("crazy_dim.dim physically safe row past logical shape", "dim", DimToken, qScalar, mScalar, 2, 0);

        var (qPointers, mPointers) = Pair("L");
        object qLength = Side.Quire.Num(5, 0, "T"), mLength = Side.Mathcad.Num(5, 0, "T");
        object qTime = Side.Quire.Num(8, 0, "v"), mTime = Side.Mathcad.Num(8, 0, "v");
        Pointer(Side.Quire, qPointers, 0, 1, qLength);
        Pointer(Side.Mathcad, mPointers, 0, 1, mLength);
        Pointer(Side.Quire, qPointers, 1, 0, qTime);
        Pointer(Side.Mathcad, mPointers, 1, 0, mTime);
        Compare("crazy_dim.dim live pointer at first selected coordinate", "dim", DimToken, qPointers, mPointers, 0, 1, qLength, mLength);
        Compare("crazy_dim.dim live pointer at second selected coordinate", "dim", DimToken, qPointers, mPointers, 1, 0, qTime, mTime);

        var (qNested, mNested) = Pair("L");
        object qChild = Matrix(Side.Quire, "T", 1, 1, 1, 0);
        object mChild = Matrix(Side.Mathcad, "T", 1, 1, 1, 0);
        Set(qChild, "m_parr", null);
        Set(mChild, "m_parr", null);
        Pointer(Side.Quire, qNested, 1, 1, qChild);
        Pointer(Side.Mathcad, mNested, 1, 1, mChild);
        Compare("crazy_dim.dim nested MatVal pointer reads child UniformDimension and captures child state", "dim", DimToken, qNested, mNested, 1, 1, qChild, mChild);

        var (qInvalid, mInvalid) = Pair("L");
        Compare("crazy_dim.dim negative backing index throws", "dim", DimToken, qInvalid, mInvalid, -2, 0);
        Compare("crazy_dim.dim high backing index throws", "dim", DimToken, qInvalid, mInvalid, 100, 0);
        Compare("crazy_dim.dim high column backing index throws", "dim", DimToken, qInvalid, mInvalid, 0, 3);
    }

    [SkippableFact]
    public void Crazy_dim_matdim_returns_scalar_pointer_or_nested_matrix_dimensions()
    {
        Runtime.Sync();
        Assert.Equal(MatDimToken, Method(Side.Mathcad, "matdim").MetadataToken);

        var (qScalar, mScalar) = Pair("L");
        Compare("crazy_dim.matdim scalar-backed matrix dimension", "matdim", MatDimToken, qScalar, mScalar, 0, 0);
        Compare("crazy_dim.matdim scalar-backed last cell", "matdim", MatDimToken, qScalar, mScalar, 1, 1);
        Compare("crazy_dim.matdim physically safe row past logical shape", "matdim", MatDimToken, qScalar, mScalar, 2, 0);

        var (qPointers, mPointers) = Pair("L");
        object qLength = Side.Quire.Num(5, 0, "T"), mLength = Side.Mathcad.Num(5, 0, "T");
        object qTime = Side.Quire.Num(8, 0, "v"), mTime = Side.Mathcad.Num(8, 0, "v");
        Pointer(Side.Quire, qPointers, 0, 1, qLength);
        Pointer(Side.Mathcad, mPointers, 0, 1, mLength);
        Pointer(Side.Quire, qPointers, 1, 0, qTime);
        Pointer(Side.Mathcad, mPointers, 1, 0, mTime);
        Compare("crazy_dim.matdim live NumVal pointer produces 1x1 T dimension", "matdim", MatDimToken, qPointers, mPointers, 0, 1, qLength, mLength);
        Compare("crazy_dim.matdim live NumVal pointer produces 1x1 v dimension", "matdim", MatDimToken, qPointers, mPointers, 1, 0, qTime, mTime);

        var (qNested, mNested) = Pair("L");
        object qChild = Matrix(Side.Quire, "T", 1, 1, 1, 0);
        object mChild = Matrix(Side.Mathcad, "T", 1, 1, 1, 0);
        Set(qChild, "m_parr", null);
        Set(mChild, "m_parr", null);
        Pointer(Side.Quire, qNested, 1, 1, qChild);
        Pointer(Side.Mathcad, mNested, 1, 1, mChild);
        Compare("crazy_dim.matdim nested MatVal pointer returns child matrix dimension and captures child state", "matdim", MatDimToken, qNested, mNested, 1, 1, qChild, mChild);

        var (qInvalid, mInvalid) = Pair("L");
        Compare("crazy_dim.matdim negative backing index throws", "matdim", MatDimToken, qInvalid, mInvalid, -2, 0);
        Compare("crazy_dim.matdim high backing index throws", "matdim", MatDimToken, qInvalid, mInvalid, 100, 0);
        Compare("crazy_dim.matdim high column backing index throws", "matdim", MatDimToken, qInvalid, mInvalid, 0, 3);
    }
}
