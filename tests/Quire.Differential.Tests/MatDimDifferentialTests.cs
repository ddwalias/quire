using System.Reflection;

namespace Quire.Differential.Tests;

public class MatDimDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type Type(Side side, string name) => side.T(name);

    private static System.Type ParameterType(Side side, string name) => name switch
    {
        "Boolean" => typeof(bool), "Int32" => typeof(int), "UInt32" => typeof(uint), "Double" => typeof(double),
        "Dim[]" => side.Dim.MakeArrayType(), "MatDim" => side.T("Mcd.MatDim"), "Dim" => side.Dim,
        "RC" => Reflect.Nested(side.T("Mcd.MatDim"), "RC"),
        _ => side.T("Mcd." + name),
    };

    private static MethodInfo Method(Side side, string owner, string name, params string[] parameters)
    {
        var type = Type(side, owner);
        var types = parameters.Select(p => ParameterType(side, p)).ToArray();
        return type.GetMethod(name, All, types) ?? throw new HarnessException($"Missing exact method {owner}.{name}({string.Join(",", parameters)})");
    }

    private static Array Dims(Side side, params string[] names)
    {
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) values.SetValue(side.D(names[i]), i);
        return values;
    }

    private static Array DimValues(Side side, params object?[] dimensions)
    {
        var values = Array.CreateInstance(side.Dim, dimensions.Length);
        for (int i = 0; i < dimensions.Length; i++) values.SetValue(dimensions[i], i);
        return values;
    }

    public static string DescribeDimension(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is string or int or uint or double or bool or long || value.GetType().IsEnum) return Reflect.Text(value);
        if (value is Array array)
            return "[" + string.Join(",", array.Cast<object?>().Select(v => side.DimText(v))) + "]";
        if (value.GetType().FullName!.EndsWith("Mcd.Dim", StringComparison.Ordinal)) return side.DimText(value);
        string result = $"{value.GetType().Name} {Reflect.Get(value, "Rows")}x{Reflect.Get(value, "Cols")} uniform {side.DimText(Reflect.Get(value, "UniformDim"))} linear {ReferenceEquals(Reflect.Get(value, "IsLinear"), value)}";
        int rows = (int)Reflect.Get(value, "Rows")!, cols = (int)Reflect.Get(value, "Cols")!;
        if (rows * cols > 36) return result;
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            result += $" ({r},{c})={side.DimText(Reflect.Call(value, "dim", r, c))}";
        return result;
    }

    private static string Show(Side side, object? value) => DescribeDimension(side, value);

    private static string ShowWithScratch(Side side, object? value)
    {
        var scratch = (int[])Reflect.Get(side.Dim, "s_vec")!;
        string scratchText = string.Join(',', scratch);
        string description = value != null && side.T("Mcd.MatDim").IsInstanceOfType(value)
            ? $"{value.GetType().Name} {Reflect.Get(value, "Rows")}x{Reflect.Get(value, "Cols")}" : Show(side, value);
        return $"{description} scratch [{scratchText}]";
    }

    private static Func<object?, string> PowerScratchRenderer(Side side)
    {
        int inputCount = 0;
        return value =>
        {
            var scratch = (int[])Reflect.Get(side.Dim, "s_vec")!;
            string scratchText = string.Join(',', scratch);
            int internedCount = ((System.Collections.IDictionary)Reflect.Get(side.Dim, "s_map")!).Count;
            string description = value != null && side.T("Mcd.MatDim").IsInstanceOfType(value)
                ? $"{value.GetType().Name} {Reflect.Get(value, "Rows")}x{Reflect.Get(value, "Cols")}" : Show(side, value);
            if (++inputCount <= 2) return description;
            return $"{description} scratch [{scratchText}] interned {internedCount}";
        };
    }

    private static void Compare(string label, string owner, string name, string[] signature, bool isStatic,
        object? qTarget, object?[] qArgs, object? mTarget, object?[] mArgs,
        Func<object?, string>? qRenderer = null, Func<object?, string>? mRenderer = null)
    {
        Func<object?, string> qs = value => Show(Side.Quire, value);
        Func<object?, string> ms = value => Show(Side.Mathcad, value);
        MethodInfo Resolve(Side side, object? target)
        {
            if (!isStatic && target != null)
            {
                var exact = target.GetType().GetMethod(name, All, signature.Select(p => ParameterType(side, p)).ToArray());
                if (exact != null) return exact;
            }
            return Method(side, owner, name, signature);
        }
        MethodComparison.Compare(label, Contract, Resolve(Side.Quire, qTarget), qTarget, qArgs,
            Resolve(Side.Mathcad, mTarget), mTarget, mArgs, qRenderer ?? qs, mRenderer ?? ms,
            isStatic ? null : v => Show(Side.Quire, v), isStatic ? null : v => Show(Side.Mathcad, v));
    }

    private static object Uniform(Side side, string dim, int rows, int cols) =>
        Reflect.Call(Reflect.Nested(side.T("Mcd.MatDim"), "uniform_dim"), "make", side.D(dim), rows, cols)!;

    private static object Row(Side side, params string[] dims) =>
        Reflect.Call(side.T("Mcd.MatDim"), "make", Enum.Parse(Reflect.Nested(side.T("Mcd.MatDim"), "RC"), "ROW"), Dims(side, dims), 3)!;

    private static object Col(Side side, params string[] dims) =>
        Reflect.Call(side.T("Mcd.MatDim"), "make", Enum.Parse(Reflect.Nested(side.T("Mcd.MatDim"), "RC"), "COL"), Dims(side, dims), 3)!;

    private static object Rc(Side side, string[] rows, string[] cols) =>
        Reflect.Call(side.T("Mcd.MatDim"), "make", Dims(side, rows), Dims(side, cols))!;

    [SkippableFact]
    public void Dimension_array_helpers()
    {
        Runtime.Sync();
        foreach (var names in new[] { Array.Empty<string>(), new[] { "L" }, new[] { "L", "L" }, new[] { "L", "T" }, new[] { "poly", "poly" } })
        {
            var q = Dims(Side.Quire, names); var m = Dims(Side.Mathcad, names);
            Compare($"Uniform({string.Join(',', names)})", "Mcd.MatDim", "Uniform", ["Dim[]"], true, null, [q], null, [m]);
            Compare($"Invert({string.Join(',', names)})", "Mcd.MatDim", "Invert", ["Dim[]"], true, null, [q], null, [m]);
            Compare($"Mult(L, {string.Join(',', names)})", "Mcd.MatDim", "Mult", ["Dim", "Dim[]"], true, null, [Side.Quire.D("L"), q], null, [Side.Mathcad.D("L"), m]);
            Compare($"Product(T, {string.Join(',', names)})", "Mcd.MatDim", "Product", ["Dim", "Dim[]"], true, null, [Side.Quire.D("T"), q], null, [Side.Mathcad.D("T"), m]);
            Compare($"Push(L, {string.Join(',', names)})", "Mcd.MatDim", "Push", ["Dim", "Dim[]"], true, null, [Side.Quire.D("L"), q], null, [Side.Mathcad.D("L"), m]);
            foreach (var rhs in new[] { Array.Empty<string>(), new[] { "L" }, new[] { "L", "T" }, new[] { "L", "L" } })
            {
                var q2 = Dims(Side.Quire, rhs); var m2 = Dims(Side.Mathcad, rhs);
                Compare("Eq_b(Dim[], Dim[])", "Mcd.MatDim", "Eq_b", ["Dim[]", "Dim[]"], true, null, [q, q2], null, [m, m2]);
                Compare("IsScalarMultiple(Dim[], Dim[])", "Mcd.MatDim", "IsScalarMultiple", ["Dim[]", "Dim[]"], true, null, [q, q2], null, [m, m2]);
                Compare("Mult(Dim[], Dim[])", "Mcd.MatDim", "Mult", ["Dim[]", "Dim[]"], true, null, [q, q2], null, [m, m2]);
            }
        }

        var equalLeftQ = Dims(Side.Quire, "L", "L"); var equalLeftM = Dims(Side.Mathcad, "L", "L");
        var firstMismatchQ = Dims(Side.Quire, "T", "L"); var firstMismatchM = Dims(Side.Mathcad, "T", "L");
        Compare("Eq_b(Dim[], Dim[]) first element differs, equal lengths", "Mcd.MatDim", "Eq_b", ["Dim[]", "Dim[]"], true,
            null, [equalLeftQ, firstMismatchQ], null, [equalLeftM, firstMismatchM]);
        Compare("IsScalarMultiple(Dim[], Dim[]) later ratio differs", "Mcd.MatDim", "IsScalarMultiple", ["Dim[]", "Dim[]"], true,
            null, [equalLeftQ, firstMismatchQ], null, [equalLeftM, firstMismatchM]);
        Compare("Mult(Dim[], Dim[]) later product differs", "Mcd.MatDim", "Mult", ["Dim[]", "Dim[]"], true,
            null, [equalLeftQ, firstMismatchQ], null, [equalLeftM, firstMismatchM]);
        var qScratchLeft = Side.Quire.Parse("V{Length 123456789}")!;
        var qScratchRight = Side.Quire.Parse("V{Time 987654321}")!;
        var mScratchLeft = Side.Mathcad.Parse("V{Length 123456789}")!;
        var mScratchRight = Side.Mathcad.Parse("V{Time 987654321}")!;
        Compare("IsScalarMultiple(Dim[], Dim[]) one item leaves scratch", "Mcd.MatDim", "IsScalarMultiple", ["Dim[]", "Dim[]"], true,
            null, [DimValues(Side.Quire, qScratchLeft), DimValues(Side.Quire, qScratchRight)], null,
            [DimValues(Side.Mathcad, mScratchLeft), DimValues(Side.Mathcad, mScratchRight)],
            v => ShowWithScratch(Side.Quire, v), v => ShowWithScratch(Side.Mathcad, v));
        Compare("Mult(Dim[], Dim[]) one item leaves scratch", "Mcd.MatDim", "Mult", ["Dim[]", "Dim[]"], true,
            null, [DimValues(Side.Quire, qScratchLeft), DimValues(Side.Quire, qScratchRight)], null,
            [DimValues(Side.Mathcad, mScratchLeft), DimValues(Side.Mathcad, mScratchRight)],
            v => ShowWithScratch(Side.Quire, v), v => ShowWithScratch(Side.Mathcad, v));
        Compare("Uniform(null)", "Mcd.MatDim", "Uniform", ["Dim[]"], true, null, [null], null, [null]);
        Compare("CMult(null, null)", "Mcd.MatDim", "CMult", ["MatDim", "MatDim"], true, null, [null, null], null, [null, null]);
    }

    [SkippableFact]
    public void Matrix_dimension_dispatch()
    {
        Runtime.Sync();
        var qDims = new[] { Uniform(Side.Quire, "L", 2, 3), Uniform(Side.Quire, "T", 2, 3), Row(Side.Quire, "L", "T"),
            Col(Side.Quire, "L", "T"), Rc(Side.Quire, ["L", "T"], ["L", "T"]), Rc(Side.Quire, ["L", "T"], ["T", "L"]) };
        var mDims = new[] { Uniform(Side.Mathcad, "L", 2, 3), Uniform(Side.Mathcad, "T", 2, 3), Row(Side.Mathcad, "L", "T"),
            Col(Side.Mathcad, "L", "T"), Rc(Side.Mathcad, ["L", "T"], ["L", "T"]), Rc(Side.Mathcad, ["L", "T"], ["T", "L"]) };

        for (int i = 0; i < qDims.Length; i++)
        {
            var q = qDims[i]; var m = mDims[i];
            Compare($"Rows[{i}]", "Mcd.MatDim", "get_Rows", [], false, q, [], m, []);
            Compare($"Cols[{i}]", "Mcd.MatDim", "get_Cols", [], false, q, [], m, []);
            Compare($"UniformDim[{i}]", "Mcd.MatDim", "get_UniformDim", [], false, q, [], m, []);
            Compare($"RowDimensions[{i}]", "Mcd.MatDim", "get_RowDimensions", [], false, q, [], m, []);
            Compare($"ColDimensions[{i}]", "Mcd.MatDim", "get_ColDimensions", [], false, q, [], m, []);
            Compare($"IsLinear[{i}]", "Mcd.MatDim", "get_IsLinear", [], false, q, [], m, []);
            foreach (int r in new[] { 0, 1 })
            foreach (int c in new[] { 0, 1, 2 })
            {
                Compare($"dim[{i}]({r},{c})", q.GetType().FullName![6..].Replace('+', '+'), "dim", ["Int32", "Int32"], false, q, [r, c], m, [r, c]);
                Compare($"matdim[{i}]({r},{c})", q.GetType().FullName![6..], "matdim", ["Int32", "Int32"], false, q, [r, c], m, [r, c]);
            }
            Compare($"RowDimensionArray[{i}](0)", "Mcd.MatDim", "RowDimensionArray", ["Int32"], false, q, [0], m, [0]);
            Compare($"ColDimensionArray[{i}](0)", "Mcd.MatDim", "ColDimensionArray", ["Int32"], false, q, [0], m, [0]);
            Compare($"Transpose[{i}]", "Mcd.MatDim", "Transpose", ["MatDim"], true, null, [q], null, [m]);
            Compare($"Det[{i}](2)", "Mcd.MatDim", "Det", ["MatDim", "Int32"], true, null, [q, 2], null, [m, 2]);
            foreach (int power in new[] { -2, -1, 0, 1, 2, 3 })
                Compare($"Pwr[{i}]({power})", "Mcd.MatDim", "Pwr", ["MatDim", "Int32"], true, null, [q, power], null, [m, power]);
            foreach (int j in Enumerable.Range(0, qDims.Length))
            {
                Compare($"Eq_b[{i},{j}]", "Mcd.MatDim", "Eq_b", ["MatDim", "MatDim"], true, null, [q, qDims[j]], null, [m, mDims[j]]);
                Compare($"MMult[{i},{j}]", "Mcd.MatDim", "MMult", ["MatDim", "MatDim"], true, null, [q, qDims[j]], null, [m, mDims[j]]);
                Compare($"DMult[{i},{j}]", "Mcd.MatDim", "DMult", ["MatDim", "MatDim"], true, null, [q, qDims[j]], null, [m, mDims[j]]);
                Compare($"Eq[{i},{j}]", "Mcd.MatDim", "Eq", ["MatDim", "MatDim", "UInt32"], true, null, [q, qDims[j], 7u], null, [m, mDims[j], 7u]);
            }
            foreach (string dim in Side.DimNames)
            {
                Compare($"Eq_b[{i}, {dim}]", "Mcd.MatDim", "Eq_b", ["MatDim", "Dim"], true, null, [q, Side.Quire.D(dim)], null, [m, Side.Mathcad.D(dim)]);
                Compare($"Eq[{i}, {dim}]", "Mcd.MatDim", "Eq", ["MatDim", "Dim", "UInt32"], true, null, [q, Side.Quire.D(dim), 5u], null, [m, Side.Mathcad.D(dim), 5u]);
            }
        }

        var qPowerLeft = Side.Quire.Parse("V{Length 123456789}")!;
        var qPowerRight = Side.Quire.Parse("V{Time 987654321}")!;
        var mPowerLeft = Side.Mathcad.Parse("V{Length 123456789}")!;
        var mPowerRight = Side.Mathcad.Parse("V{Time 987654321}")!;
        var qPowerRows = DimValues(Side.Quire, qPowerLeft, qPowerRight);
        var qPowerCols = DimValues(Side.Quire, qPowerRight, qPowerLeft);
        var mPowerRows = DimValues(Side.Mathcad, mPowerLeft, mPowerRight);
        var mPowerCols = DimValues(Side.Mathcad, mPowerRight, mPowerLeft);
        var qPowerMatrix = Reflect.Call(Side.Quire.T("Mcd.MatDim"), "make", qPowerRows, qPowerCols)!;
        var mPowerMatrix = Reflect.Call(Side.Mathcad.T("Mcd.MatDim"), "make", mPowerRows, mPowerCols)!;
        Compare("Pwr(row-column, 0) leaves dimension scratch", "Mcd.MatDim", "Pwr", ["MatDim", "Int32"], true,
            null, [qPowerMatrix, 0], null, [mPowerMatrix, 0],
            PowerScratchRenderer(Side.Quire), PowerScratchRenderer(Side.Mathcad));

        foreach (var names in new[] { new[] { "L", "L" }, new[] { "L", "T" }, new[] { "poly", "poly" } })
        foreach (int size in new[] { 0, 1, 3 })
        foreach (string axis in new[] { "ROW", "COL" })
        {
            var qaxis = Enum.Parse(Reflect.Nested(Side.Quire.T("Mcd.MatDim"), "RC"), axis);
            var maxis = Enum.Parse(Reflect.Nested(Side.Mathcad.T("Mcd.MatDim"), "RC"), axis);
            var qarray = Dims(Side.Quire, names); var marray = Dims(Side.Mathcad, names);
            Compare($"make({axis}, dims, {size})", "Mcd.MatDim", "make", ["RC", "Dim[]", "Int32"], true, null, [qaxis, qarray, size], null, [maxis, marray, size]);
            Compare($"makeRowDimension({size})", "Mcd.MatDim", "makeRowDimension", ["Dim", "Dim[]", "Int32"], true, null,
                [Side.Quire.D("L"), qarray, size], null, [Side.Mathcad.D("L"), marray, size]);
            Compare($"makeColDimension({size})", "Mcd.MatDim", "makeColDimension", ["Dim", "Dim[]", "Int32"], true, null,
                [Side.Quire.D("L"), qarray, size], null, [Side.Mathcad.D("L"), marray, size]);
        }
        Compare("make(Dim)", "Mcd.MatDim", "make", ["Dim"], true, null, [Side.Quire.D("L")], null, [Side.Mathcad.D("L")]);
        Compare("make(row dims, col dims)", "Mcd.MatDim", "make", ["Dim[]", "Dim[]"], true, null,
            [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "L", "T")], null,
            [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "L", "T")]);
    }
}
