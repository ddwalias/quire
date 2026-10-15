using System.Collections;
using System.Reflection;

namespace Quire.Differential.Tests;

public class MatValIndexedDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, bool isStatic, string name, params string[] parameters)
    {
        var type = side.T("Mcd.MatVal");
        return (isStatic ? type.GetMethods(Static) : type.GetMethods(Instance))
            .Single(m => m.Name == name && m.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(parameters));
    }

    private static object Matrix(Side side, int rows = 3, int cols = 2)
    {
        var matrix = Reflect.New(side.T("Mcd.MatVal"), new double[rows * cols], new double[rows * cols], rows, cols, rows,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Reflect.Set(matrix, "m_dim", side.D("identity"));
        for (int c = 0; c < cols; c++)
        for (int r = 0; r < rows; r++)
            Reflect.CallSig(matrix, "set_2", ["Int32", "Int32", "Double"], r, c, (double)(r + c * rows + 1));
        Reflect.Call(matrix, "set_ptrs_2", 0, 1, side.Num(8, 2, "T"));
        Reflect.Call(matrix, "set_ptrs_2", 1, 1, "text");
        return matrix;
    }

    private static object Vector(Side side)
    {
        var vector = Reflect.New(side.T("Mcd.MatVal"), new double[] { 1, 2, 3 });
        Reflect.Set(vector, "m_dim", side.D("identity"));
        return vector;
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName!.EndsWith("Mcd.MatVal", StringComparison.Ordinal))
            return MatValDifferentialTests.Storage(side, value);
        return side.Show(value);
    }

    private static string State(Side side, object value) => MatValDifferentialTests.Storage(side, value);

    private static void Compare(string label, string name, string[] signature, bool isStatic,
        object? qTarget, object?[] qArgs, object? mTarget, object?[] mArgs)
    {
        var qs = (Func<object?, string>)(v => Show(Side.Quire, v));
        var ms = (Func<object?, string>)(v => Show(Side.Mathcad, v));
        MethodComparison.Compare(label, Contract, Method(Side.Quire, isStatic, name, signature), qTarget, qArgs,
            Method(Side.Mathcad, isStatic, name, signature), mTarget, mArgs,
            qs, ms,
            isStatic ? null : v => State(Side.Quire, v),
            isStatic ? null : v => State(Side.Mathcad, v));
    }

    [SkippableFact]
    public void Indexed_access_and_classification()
    {
        Runtime.Sync();
        foreach (int index in new[] { -1, 0, 1, 2, 3 })
        {
            var qv = Vector(Side.Quire); var mv = Vector(Side.Mathcad);
            var qm = Matrix(Side.Quire); var mm = Matrix(Side.Mathcad);
            var objectIndexQ = Side.Quire.Num(index, 0, "identity");
            var objectIndexM = Side.Mathcad.Num(index, 0, "identity");
            Compare($"matrix_get_1({index})", "matrix_get_1", ["Int32"], false, qv, [index], mv, [index]);
            Compare($"sym_matrix_get_1({index})", "sym_matrix_get_1", ["Int32"], false, qv, [index], mv, [index]);
            Compare($"aref_1({index})", "aref_1", ["Int32"], false, qv, [index], mv, [index]);
            Compare($"mcd_get_1(Int32 {index})", "mcd_get_1", ["Int32"], false, qv, [index], mv, [index]);
            Compare($"mcd_get_1(UInt32 {index})", "mcd_get_1", ["UInt32"], false, qv, [unchecked((uint)index)], mv, [unchecked((uint)index)]);
            Compare($"mcd_get_1(Object, Object {index})", "mcd_get_1", ["Object", "Object"], true, null, [qv, objectIndexQ], null, [mv, objectIndexM]);
            Compare($"mcd_getm_1({index})", "mcd_getm_1", ["Object", "Int32"], true, null, [qv, index], null, [mv, index]);
            Compare($"DynCall({index})", "DynCall", ["Object"], false, qv, [objectIndexQ], mv, [objectIndexM]);
            Compare($"iterator_get({index})", "iterator_get", ["Int32"], false, qm, [index], mm, [index]);

            foreach (int column in new[] { -1, 0, 1, 2 })
            {
                Compare($"matrix_get_2({index}, {column})", "matrix_get_2", ["Int32", "Int32"], false, qm, [index, column], mm, [index, column]);
                Compare($"sym_matrix_get_2({index}, {column})", "sym_matrix_get_2", ["Int32", "Int32"], false, qm, [index, column], mm, [index, column]);
                Compare($"aref_2({index}, {column})", "aref_2", ["Int32", "Int32"], false, qm, [index, column], mm, [index, column]);
                Compare($"mcd_get_2(Int32 {index}, {column})", "mcd_get_2", ["Int32", "Int32"], false, qm, [index, column], mm, [index, column]);
                Compare($"mcd_get_2(UInt32 {index}, {column})", "mcd_get_2", ["UInt32", "UInt32"], false, qm, [unchecked((uint)index), unchecked((uint)column)], mm, [unchecked((uint)index), unchecked((uint)column)]);
                Compare($"mcd_getm_2({index}, {column})", "mcd_getm_2", ["Object", "Int32", "Int32"], true, null, [qm, index, column], null, [mm, index, column]);
                Compare($"DynCall({index}, {column})", "DynCall", ["Object", "Object"], false, qm,
                    [Side.Quire.Num(index, 0, "identity"), Side.Quire.Num(column, 0, "identity")], mm,
                    [Side.Mathcad.Num(index, 0, "identity"), Side.Mathcad.Num(column, 0, "identity")]);
                foreach (double threshold in new[] { double.NaN, 0, 0.1, 0.2, double.PositiveInfinity })
                    Compare($"get_type_2({index}, {column}, {threshold})", "get_type_2", ["Int32", "Int32", "Double"], false,
                        qm, [index, column, threshold], mm, [index, column, threshold]);
            }
        }

        Compare("mcd_get_1(nonmatrix, number)", "mcd_get_1", ["Object", "Object"], true, null,
            ["not a matrix", Side.Quire.Num(0, 0, "identity")], null,
            ["not a matrix", Side.Mathcad.Num(0, 0, "identity")]);
        Compare("mcd_get_1(vector, nonnumber)", "mcd_get_1", ["Object", "Object"], true, null,
            [Vector(Side.Quire), "not a number"], null, [Vector(Side.Mathcad), "not a number"]);
        Compare("mcd_get_1(vector, noninteger number)", "mcd_get_1", ["Object", "Object"], true, null,
            [Vector(Side.Quire), Side.Quire.Num(1.5, 0, "identity")], null,
            [Vector(Side.Mathcad), Side.Mathcad.Num(1.5, 0, "identity")]);
        Compare("mcd_getm_1(nonmatrix)", "mcd_getm_1", ["Object", "Int32"], true, null,
            ["not a matrix", 0], null, ["not a matrix", 0]);
        Compare("mcd_getm_2(nonmatrix)", "mcd_getm_2", ["Object", "Int32", "Int32"], true, null,
            ["not a matrix", 0, 0], null, ["not a matrix", 0, 0]);

        var qvForDynamic = Vector(Side.Quire); var mvForDynamic = Vector(Side.Mathcad);
        var qIndexNoninteger = Side.Quire.Num(1.5, 0, "identity"); var mIndexNoninteger = Side.Mathcad.Num(1.5, 0, "identity");
        Compare("DynCall(noninteger vector index)", "DynCall", ["Object"], false, qvForDynamic,
            [qIndexNoninteger], mvForDynamic, [mIndexNoninteger]);
        var qmForDynamic = Matrix(Side.Quire); var mmForDynamic = Matrix(Side.Mathcad);
        Compare("DynCall(noninteger matrix row)", "DynCall", ["Object", "Object"], false, qmForDynamic,
            [Side.Quire.Num(1.5, 0, "identity"), Side.Quire.Num(0, 0, "identity")], mmForDynamic,
            [Side.Mathcad.Num(1.5, 0, "identity"), Side.Mathcad.Num(0, 0, "identity")]);
        Compare("DynCall(noninteger matrix column)", "DynCall", ["Object", "Object"], false, qmForDynamic,
            [Side.Quire.Num(0, 0, "identity"), Side.Quire.Num(1.5, 0, "identity")], mmForDynamic,
            [Side.Mathcad.Num(0, 0, "identity"), Side.Mathcad.Num(1.5, 0, "identity")]);

        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        foreach (var axisName in new[] { "Row", "Col" })
        foreach (int index in new[] { -1, 0, 1, 2 })
        {
            var qa = Enum.Parse(Side.Quire.T("Mcd.MatAxis"), axisName);
            var ma = Enum.Parse(Side.Mathcad.T("Mcd.MatAxis"), axisName);
            foreach (double threshold in new[] { double.NaN, 0, 0.1, 0.2 })
                Compare($"element_types({axisName}, {index}, {threshold})", "element_types", ["MatAxis", "Int32", "Double"], false,
                    q, [qa, index, threshold], m, [ma, index, threshold]);
            Compare($"get_ptrs({axisName}, {index})", "get_ptrs", ["MatAxis", "Int32"], false, q, [qa, index], m, [ma, index]);
            foreach (double threshold in new[] { double.NaN, 0, 0.1, 0.2 })
            foreach (double scale in new[] { 0, 2 })
            {
                Compare($"get_doubles({axisName}, {index}, {threshold}, {scale})", "get_doubles", ["MatAxis", "Int32", "Double", "Double"], false,
                    q, [qa, index, threshold, scale], m, [ma, index, threshold, scale]);
                Compare($"get_data({axisName}, {index}, {threshold}, {scale})", "get_data", ["MatAxis", "Int32", "Double", "Double"], false,
                    q, [qa, index, threshold, scale], m, [ma, index, threshold, scale]);
            }
        }

        foreach (int offset in new[] { 0, 2 })
            Compare($"PackObjects({offset})", "PackObjects", ["Int32"], false, q, [offset], m, [offset]);
        Compare("PackObjects()", "PackObjects", [], false, q, [], m, []);
    }
}
