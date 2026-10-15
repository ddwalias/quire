using System.Collections;
using System.Reflection;

namespace Quire.Differential.Tests;

public class MatValDimensionDifferentialTests
{
    private const string Contract = "Mcd.MatVal.Dimensions";
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, bool isStatic, string name, params string[] parameters)
    {
        var type = side.T("Mcd.MatVal");
        return (isStatic ? type.GetMethods(Static) : type.GetMethods(Instance))
            .Single(m => m.Name == name && m.GetParameters().Select(p => p.ParameterType.Name).SequenceEqual(parameters));
    }

    private static object Matrix(Side side, string dimension, int kind)
    {
        var matrix = Reflect.New(side.T("Mcd.MatVal"), new double[] { 99, 1, 2, 99, 3, 4, 99 }, new double[7], 2, 2, 3, null,
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
        Reflect.Set(matrix, "m_dim", side.D(dimension));
        if (kind > 0) Reflect.CallSig(matrix, "set_2", ["Int32", "Int32", "Object"], 1, 1, side.Num(5, 6, "T"));
        if (kind > 1) Reflect.Call(matrix, "set_ptrs_2", 0, 1, Reflect.New(side.T("Mcd.MatVal"), new double[] { 3 }));
        if (kind > 2) Reflect.Call(matrix, "set_ptrs_2", 1, 0, "text");
        return matrix;
    }

    private static object MatrixWithStalePointerEntry(Side side)
    {
        var matrix = Matrix(side, "L", 1);
        ((Hashtable)Reflect.Get(matrix, "m_parr")!)[0] = "stale";
        return matrix;
    }

    private static object RectangularMatrix(Side side, string dimension, int rows, int cols)
    {
        var matrix = Reflect.New(side.T("Mcd.MatVal"), new double[rows * cols], new double[rows * cols], rows, cols, rows,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 0);
        Reflect.Set(matrix, "m_dim", side.D(dimension));
        return matrix;
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName!.EndsWith("Mcd.MatVal", StringComparison.Ordinal))
            return MatValDifferentialTests.Storage(side, value);
        if (value.GetType().FullName!.EndsWith("Mcd.MatDim", StringComparison.Ordinal))
            return MatDimDifferentialTests.DescribeDimension(side, value);
        return side.Show(value);
    }

    private static string State(Side side, object value) => MatValDifferentialTests.Storage(side, value);

    private static string ShowWithSource(Side side, object? value, object? source)
    {
        string result = Show(side, value);
        if (value == null || source == null) return result;
        if (!side.T("Mcd.MatVal").IsInstanceOfType(value) || !side.T("Mcd.MatVal").IsInstanceOfType(source))
            return $"{result} same={ReferenceEquals(value, source)}";
        return $"{result} same={ReferenceEquals(value, source)}"
            + $" real={ReferenceEquals(Reflect.Get(value, "m_rarr"), Reflect.Get(source, "m_rarr"))}"
            + $" imag={ReferenceEquals(Reflect.Get(value, "m_iarr"), Reflect.Get(source, "m_iarr"))}"
            + $" ptrs={ReferenceEquals(Reflect.Get(value, "m_parr"), Reflect.Get(source, "m_parr"))}";
    }

    private static void Compare(string label, string name, string[] signature, bool isStatic,
        object? qTarget, object?[] qArgs, object? mTarget, object?[] mArgs,
        Func<object?, string>? qRenderer = null, Func<object?, string>? mRenderer = null)
    {
        Func<object?, string> qs = v => Show(Side.Quire, v);
        Func<object?, string> ms = v => Show(Side.Mathcad, v);
        MethodComparison.Compare(label, Contract, Method(Side.Quire, isStatic, name, signature), qTarget, qArgs,
            Method(Side.Mathcad, isStatic, name, signature), mTarget, mArgs,
            qRenderer ?? qs, mRenderer ?? ms,
            isStatic ? null : v => State(Side.Quire, v),
            isStatic ? null : v => State(Side.Mathcad, v));
    }

    private static object Descriptor(Side side, string axis, string[] dimensions, int size)
    {
        var descriptor = side.T("Mcd.MatDim");
        var array = Array.CreateInstance(side.Dim, dimensions.Length);
        for (int i = 0; i < dimensions.Length; i++) array.SetValue(side.D(dimensions[i]), i);
        var axisValue = Enum.Parse(Reflect.Nested(descriptor, "RC"), axis);
        return Reflect.Call(descriptor, "make", axisValue, array, size)!;
    }

    [SkippableFact]
    public void Dimension_casts_and_mutations()
    {
        Runtime.Sync();
        foreach (var dimension in Side.DimNames)
        foreach (int kind in Enumerable.Range(0, 4))
        foreach (string name in new[] { "StripDimension", "AsUniform", "AsDimless", "AsPolymorphic" })
        {
            var q = Matrix(Side.Quire, dimension, kind); var m = Matrix(Side.Mathcad, dimension, kind);
            Compare($"{name}({dimension}, kind {kind})", name, ["MatVal"], true, null, [q], null, [m],
                v => ShowWithSource(Side.Quire, v, q), v => ShowWithSource(Side.Mathcad, v, m));
        }

        foreach (var dimension in Side.DimNames)
        foreach (int kind in Enumerable.Range(0, 4))
        {
            var q = Matrix(Side.Quire, dimension, kind); var m = Matrix(Side.Mathcad, dimension, kind);
            Compare($"SetDim({dimension}, kind {kind})", "SetDim", ["Dim"], false, q, [Side.Quire.D("L")], m, [Side.Mathcad.D("L")],
                v => ShowWithSource(Side.Quire, v, q), v => ShowWithSource(Side.Mathcad, v, m));
        }

        foreach (int kind in Enumerable.Range(0, 4))
        foreach (bool uniform in new[] { false, true })
        foreach (var operation in new[] { "ApplyLinearDimension", "ApplyDimension", "ApplyCrazyDimension" })
        {
            var q = Matrix(Side.Quire, "L", kind); var m = Matrix(Side.Mathcad, "L", kind);
            var qd = Descriptor(Side.Quire, "ROW", ["identity", uniform ? "identity" : "T"], 2);
            var md = Descriptor(Side.Mathcad, "ROW", ["identity", uniform ? "identity" : "T"], 2);
            Compare($"{operation}(kind {kind}, uniform {uniform})", operation, ["MatDim"], false, q, [qd], m, [md],
                v => ShowWithSource(Side.Quire, v, q), v => ShowWithSource(Side.Mathcad, v, m));
        }

        foreach (string operation in new[] { "ApplyRowDimensions", "ApplyColDimensions" })
        foreach (int kind in new[] { 0, 2 })
        {
            var q = Matrix(Side.Quire, "L", kind); var m = Matrix(Side.Mathcad, "L", kind);
            Array qd = Array.CreateInstance(Side.Quire.Dim, 2); qd.SetValue(Side.Quire.D("L"), 0); qd.SetValue(Side.Quire.D("T"), 1);
            Array md = Array.CreateInstance(Side.Mathcad.Dim, 2); md.SetValue(Side.Mathcad.D("L"), 0); md.SetValue(Side.Mathcad.D("T"), 1);
            Compare($"{operation}(array, kind {kind})", operation, ["Dim[]"], false, q, [qd], m, [md],
                v => ShowWithSource(Side.Quire, v, q), v => ShowWithSource(Side.Mathcad, v, m));
            Compare($"{operation}(pair, kind {kind})", operation, ["Dim", "Dim"], false, q, [Side.Quire.D("L"), Side.Quire.D("T")],
                m, [Side.Mathcad.D("L"), Side.Mathcad.D("T")],
                v => ShowWithSource(Side.Quire, v, q), v => ShowWithSource(Side.Mathcad, v, m));
        }

        var qThreeRows = RectangularMatrix(Side.Quire, "L", 3, 2); var mThreeRows = RectangularMatrix(Side.Mathcad, "L", 3, 2);
        Compare("ApplyRowDimensions(pair, 3-row matrix)", "ApplyRowDimensions", ["Dim", "Dim"], false,
            qThreeRows, [Side.Quire.D("L"), Side.Quire.D("T")], mThreeRows, [Side.Mathcad.D("L"), Side.Mathcad.D("T")],
            v => ShowWithSource(Side.Quire, v, qThreeRows), v => ShowWithSource(Side.Mathcad, v, mThreeRows));
        var qThreeCols = RectangularMatrix(Side.Quire, "L", 2, 3); var mThreeCols = RectangularMatrix(Side.Mathcad, "L", 2, 3);
        Compare("ApplyColDimensions(pair, 3-column matrix)", "ApplyColDimensions", ["Dim", "Dim"], false,
            qThreeCols, [Side.Quire.D("L"), Side.Quire.D("T")], mThreeCols, [Side.Mathcad.D("L"), Side.Mathcad.D("T")],
            v => ShowWithSource(Side.Quire, v, qThreeCols), v => ShowWithSource(Side.Mathcad, v, mThreeCols));

        Compare("DivideByDimension skips stale pointer-table entries", "DivideByDimension", ["Dim"], false,
            MatrixWithStalePointerEntry(Side.Quire), [Side.Quire.D("T")],
            MatrixWithStalePointerEntry(Side.Mathcad), [Side.Mathcad.D("T")]);

        foreach (int kind in Enumerable.Range(0, 4))
        foreach (var dimension in Side.DimNames)
        {
            var q = Matrix(Side.Quire, "L", kind); var m = Matrix(Side.Mathcad, "L", kind);
            var qDivisor = Side.Quire.D(dimension); var mDivisor = Side.Mathcad.D(dimension);
            Compare($"DivideByDimension({dimension}, kind {kind})", "DivideByDimension", ["Dim"], false,
                q, [qDivisor], m, [mDivisor],
                v => ShowWithSource(Side.Quire, v, qDivisor), v => ShowWithSource(Side.Mathcad, v, mDivisor));
            q = Matrix(Side.Quire, dimension, kind); m = Matrix(Side.Mathcad, dimension, kind);
            var qDefault = Reflect.Get(q, "m_dim"); var mDefault = Reflect.Get(m, "m_dim");
            Compare($"DivideByDefaultDimension({dimension}, kind {kind})", "DivideByDefaultDimension", [], false, q, [], m, [],
                v => ShowWithSource(Side.Quire, v, qDefault), v => ShowWithSource(Side.Mathcad, v, mDefault));
        }

        foreach (object? value in new object?[] { null, "text", Side.Quire.Num(1, 0, "poly"), Matrix(Side.Quire, "L", 1) })
        {
            object? mathValue = value == null ? null : value is string ? "text" : value.GetType().Name == "MatVal"
                ? Matrix(Side.Mathcad, "L", 1) : Side.Mathcad.Num(1, 0, "poly");
            var qd = Descriptor(Side.Quire, "ROW", ["L", "T"], 2);
            var md = Descriptor(Side.Mathcad, "ROW", ["L", "T"], 2);
            Compare("MDimCast(value, descriptor)", "MDimCast", ["Object", "MatDim"], true, null, [value, qd], null, [mathValue, md],
                v => ShowWithSource(Side.Quire, v, value), v => ShowWithSource(Side.Mathcad, v, mathValue));
        }
        var qMatrix = Matrix(Side.Quire, "L", 1); var mMatrix = Matrix(Side.Mathcad, "L", 1);
        Compare("MDimCast(matrix, null)", "MDimCast", ["Object", "MatDim"], true, null, [qMatrix, null], null, [mMatrix, null],
            v => ShowWithSource(Side.Quire, v, qMatrix), v => ShowWithSource(Side.Mathcad, v, mMatrix));

        object qOrder = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 2 });
        object qValue = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        object mOrder = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 2 });
        object mValue = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        Compare("MatCreateDim(order matrix, numeric point and value)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrder, Side.Quire.Num(1, 0, "T"), qValue], null,
            [mOrder, Side.Mathcad.Num(1, 0, "T"), mValue],
            v => ShowWithSource(Side.Quire, v, qOrder), v => ShowWithSource(Side.Mathcad, v, mOrder));
        Compare("MatCreateDim(nonmatrix order)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            ["other", Side.Quire.Num(1, 0, "T"), "value"], null, ["other", Side.Mathcad.Num(1, 0, "T"), "value"],
            v => ShowWithSource(Side.Quire, v, "other"), v => ShowWithSource(Side.Mathcad, v, "other"));
    }
}
