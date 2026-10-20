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

    private static void Compare(string label, string name, string[] signature, bool isStatic,
        object? qTarget, object?[] qArgs, object? mTarget, object?[] mArgs)
    {
        Func<object?, string> qs = v => Show(Side.Quire, v);
        Func<object?, string> ms = v => Show(Side.Mathcad, v);
        MethodComparison.Compare(label, Contract, Method(Side.Quire, isStatic, name, signature), qTarget, qArgs,
            Method(Side.Mathcad, isStatic, name, signature), mTarget, mArgs,
            qs, ms,
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
            Compare($"{name}({dimension}, kind {kind})", name, ["MatVal"], true, null, [q], null, [m]);
        }

        foreach (var dimension in Side.DimNames)
        foreach (int kind in Enumerable.Range(0, 4))
        {
            var q = Matrix(Side.Quire, dimension, kind); var m = Matrix(Side.Mathcad, dimension, kind);
            Compare($"SetDim({dimension}, kind {kind})", "SetDim", ["Dim"], false, q, [Side.Quire.D("L")], m, [Side.Mathcad.D("L")]);
        }

        foreach (int kind in Enumerable.Range(0, 4))
        foreach (bool uniform in new[] { false, true })
        foreach (var operation in new[] { "ApplyLinearDimension", "ApplyDimension", "ApplyCrazyDimension" })
        {
            var q = Matrix(Side.Quire, "L", kind); var m = Matrix(Side.Mathcad, "L", kind);
            var qd = Descriptor(Side.Quire, "ROW", ["identity", uniform ? "identity" : "T"], 2);
            var md = Descriptor(Side.Mathcad, "ROW", ["identity", uniform ? "identity" : "T"], 2);
            Compare($"{operation}(kind {kind}, uniform {uniform})", operation, ["MatDim"], false, q, [qd], m, [md]);
        }

        foreach (string operation in new[] { "ApplyRowDimensions", "ApplyColDimensions" })
        foreach (int kind in new[] { 0, 2 })
        {
            var q = Matrix(Side.Quire, "L", kind); var m = Matrix(Side.Mathcad, "L", kind);
            Array qd = Array.CreateInstance(Side.Quire.Dim, 2); qd.SetValue(Side.Quire.D("L"), 0); qd.SetValue(Side.Quire.D("T"), 1);
            Array md = Array.CreateInstance(Side.Mathcad.Dim, 2); md.SetValue(Side.Mathcad.D("L"), 0); md.SetValue(Side.Mathcad.D("T"), 1);
            Compare($"{operation}(array, kind {kind})", operation, ["Dim[]"], false, q, [qd], m, [md]);
            Compare($"{operation}(pair, kind {kind})", operation, ["Dim", "Dim"], false, q, [Side.Quire.D("L"), Side.Quire.D("T")],
                m, [Side.Mathcad.D("L"), Side.Mathcad.D("T")]);
        }

        Compare("ApplyRowDimensions(pair, 3-row matrix)", "ApplyRowDimensions", ["Dim", "Dim"], false,
            RectangularMatrix(Side.Quire, "L", 3, 2), [Side.Quire.D("L"), Side.Quire.D("T")],
            RectangularMatrix(Side.Mathcad, "L", 3, 2), [Side.Mathcad.D("L"), Side.Mathcad.D("T")]);
        Compare("ApplyColDimensions(pair, 3-column matrix)", "ApplyColDimensions", ["Dim", "Dim"], false,
            RectangularMatrix(Side.Quire, "L", 2, 3), [Side.Quire.D("L"), Side.Quire.D("T")],
            RectangularMatrix(Side.Mathcad, "L", 2, 3), [Side.Mathcad.D("L"), Side.Mathcad.D("T")]);

        Compare("DivideByDimension skips stale pointer-table entries", "DivideByDimension", ["Dim"], false,
            MatrixWithStalePointerEntry(Side.Quire), [Side.Quire.D("T")],
            MatrixWithStalePointerEntry(Side.Mathcad), [Side.Mathcad.D("T")]);

        foreach (int kind in Enumerable.Range(0, 4))
        foreach (var dimension in Side.DimNames)
        {
            var q = Matrix(Side.Quire, "L", kind); var m = Matrix(Side.Mathcad, "L", kind);
            Compare($"DivideByDimension({dimension}, kind {kind})", "DivideByDimension", ["Dim"], false,
                q, [Side.Quire.D(dimension)], m, [Side.Mathcad.D(dimension)]);
            q = Matrix(Side.Quire, dimension, kind); m = Matrix(Side.Mathcad, dimension, kind);
            Compare($"DivideByDefaultDimension({dimension}, kind {kind})", "DivideByDefaultDimension", [], false, q, [], m, []);
        }

        foreach (object? value in new object?[] { null, "text", Side.Quire.Num(1, 0, "poly"), Matrix(Side.Quire, "L", 1) })
        {
            object? mathValue = value == null ? null : value is string ? "text" : value.GetType().Name == "MatVal"
                ? Matrix(Side.Mathcad, "L", 1) : Side.Mathcad.Num(1, 0, "poly");
            var qd = Descriptor(Side.Quire, "ROW", ["L", "T"], 2);
            var md = Descriptor(Side.Mathcad, "ROW", ["L", "T"], 2);
            Compare("MDimCast(value, descriptor)", "MDimCast", ["Object", "MatDim"], true, null, [value, qd], null, [mathValue, md]);
        }
        var qMatrix = Matrix(Side.Quire, "L", 1); var mMatrix = Matrix(Side.Mathcad, "L", 1);
        Compare("MDimCast(matrix, null)", "MDimCast", ["Object", "MatDim"], true, null, [qMatrix, null], null, [mMatrix, null]);

        // Real matrix (m_iarr == null) with non-uniform dimension
        var qReal = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 1, 2, 3, 4 }, null, 2, 2, 2, null,
            Enum.ToObject(Side.Quire.T("Mcd.Flags"), 2146697216), 0);
        var mReal = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 1, 2, 3, 4 }, null, 2, 2, 2, null,
            Enum.ToObject(Side.Mathcad.T("Mcd.Flags"), 2146697216), 0);
        var qdReal = Descriptor(Side.Quire, "ROW", ["identity", "T"], 2);
        var mdReal = Descriptor(Side.Mathcad, "ROW", ["identity", "T"], 2);
        Compare("ApplyLinearDimension(real matrix, non-uniform)", "ApplyLinearDimension", ["MatDim"], false,
            qReal, [qdReal], mReal, [mdReal]);

        foreach (int kind in Enumerable.Range(0, 4))
        {
            var qUniform = Matrix(Side.Quire, "L", kind);
            var mUniform = Matrix(Side.Mathcad, "L", kind);
            var qdUniform = Reflect.Call(Side.Quire.T("Mcd.MatDim"), "make", Side.Quire.D("T"));
            var mdUniform = Reflect.Call(Side.Mathcad.T("Mcd.MatDim"), "make", Side.Mathcad.D("T"));
            Compare($"ApplyLinearDimension(uniform MatDim, kind {kind})", "ApplyLinearDimension", ["MatDim"], false,
                qUniform, [qdUniform], mUniform, [mdUniform]);
        }

        Compare("MatCreateDim(order matrix, numeric point and value)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 2 }), Side.Quire.Num(1, 0, "T"), Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 })], null,
            [Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 2 }), Side.Mathcad.Num(1, 0, "T"), Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 })]);
        Compare("MatCreateDim(nonmatrix order)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            ["other", Side.Quire.Num(1, 0, "T"), "value"], null, ["other", Side.Mathcad.Num(1, 0, "T"), "value"]);
        Compare("MatCreateDim(null point)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 2 }), null, Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 })], null,
            [Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 2 }), null, Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 })]);

        var qValPower = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        var mValPower = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        Reflect.Set(qValPower, "m_dim", Side.Quire.D("L"));
        Reflect.Set(mValPower, "m_dim", Side.Mathcad.D("L"));
        for (int i = 1; i < 3; i++)
        {
            Reflect.CallSig(qValPower, "set_1", ["Int32", "Object"], i, Side.Quire.Num(i + 2, 0, "L"));
            Reflect.CallSig(mValPower, "set_1", ["Int32", "Object"], i, Side.Mathcad.Num(i + 2, 0, "L"));
        }
        var qOrderPower = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 0 });
        var mOrderPower = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 0 });
        Compare("MatCreateDim(power expansion)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrderPower, Side.Quire.Num(1, 0, "T"), qValPower], null,
            [mOrderPower, Side.Mathcad.Num(1, 0, "T"), mValPower]);

        var qOrderNegative = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { -1, 0, 0 });
        var mOrderNegative = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { -1, 0, 0 });
        Compare("MatCreateDim(negative order[0])", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrderNegative, Side.Quire.Num(1, 0, "T"), qValPower], null,
            [mOrderNegative, Side.Mathcad.Num(1, 0, "T"), mValPower]);

        var qValIncompat = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        var mValIncompat = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        Reflect.Set(qValIncompat, "m_dim", Side.Quire.D("L"));
        Reflect.Set(mValIncompat, "m_dim", Side.Mathcad.D("L"));
        Reflect.CallSig(qValIncompat, "set_1", ["Int32", "Object"], 1, Side.Quire.Num(3, 0, "T"));
        Reflect.CallSig(mValIncompat, "set_1", ["Int32", "Object"], 1, Side.Mathcad.Num(3, 0, "T"));
        Reflect.CallSig(qValIncompat, "set_1", ["Int32", "Object"], 2, Side.Quire.Num(4, 0, "L"));
        Reflect.CallSig(mValIncompat, "set_1", ["Int32", "Object"], 2, Side.Mathcad.Num(4, 0, "L"));
        var qOrderIncompat = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 1, 0 });
        var mOrderIncompat = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 1, 0 });
        Compare("MatCreateDim(incompatible reset)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrderIncompat, Side.Quire.Num(1, 0, "T"), qValIncompat], null,
            [mOrderIncompat, Side.Mathcad.Num(1, 0, "T"), mValIncompat]);

        var qOrderDimNull = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 2 });
        var mOrderDimNull = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 2 });
        var qValDimNull = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        var mValDimNull = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        Compare("MatCreateDim(polymorphic point)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrderDimNull, Side.Quire.Num(1, 0, "poly"), qValDimNull], null,
            [mOrderDimNull, Side.Mathcad.Num(1, 0, "poly"), mValDimNull]);

        var qValNullDim2 = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 1, 2, 3 });
        var mValNullDim2 = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 1, 2, 3 });
        Reflect.Call(qValNullDim2, "set_ptrs_2", 0, 0, Side.Quire.Num(1, 0, "poly"));
        Reflect.Call(mValNullDim2, "set_ptrs_2", 0, 0, Side.Mathcad.Num(1, 0, "poly"));
        Reflect.Call(qValNullDim2, "set_ptrs_2", 1, 0, Side.Quire.Num(2, 0, "T"));
        Reflect.Call(mValNullDim2, "set_ptrs_2", 1, 0, Side.Mathcad.Num(2, 0, "T"));
        Reflect.Call(qValNullDim2, "set_ptrs_2", 2, 0, Side.Quire.Num(3, 0, "L"));
        Reflect.Call(mValNullDim2, "set_ptrs_2", 2, 0, Side.Mathcad.Num(3, 0, "L"));
        var qOrderNullDim2 = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 2 });
        var mOrderNullDim2 = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 2 });
        Compare("MatCreateDim(null first column dim)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrderNullDim2, Side.Quire.Num(1, 0, "T"), qValNullDim2], null,
            [mOrderNullDim2, Side.Mathcad.Num(1, 0, "T"), mValNullDim2]);

        var qValMismatch = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        var mValMismatch = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 2, 3, 4 });
        Reflect.Set(qValMismatch, "m_dim", Side.Quire.D("L"));
        Reflect.Set(mValMismatch, "m_dim", Side.Mathcad.D("L"));
        Reflect.CallSig(qValMismatch, "set_1", ["Int32", "Object"], 1, Side.Quire.Num(3, 0, "T"));
        Reflect.CallSig(mValMismatch, "set_1", ["Int32", "Object"], 1, Side.Mathcad.Num(3, 0, "T"));
        Reflect.CallSig(qValMismatch, "set_1", ["Int32", "Object"], 2, Side.Quire.Num(4, 0, "L"));
        Reflect.CallSig(mValMismatch, "set_1", ["Int32", "Object"], 2, Side.Mathcad.Num(4, 0, "L"));
        var qOrderMismatch = Reflect.New(Side.Quire.T("Mcd.MatVal"), new double[] { 0, 0, 1 });
        var mOrderMismatch = Reflect.New(Side.Mathcad.T("Mcd.MatVal"), new double[] { 0, 0, 1 });
        Compare("MatCreateDim(mismatched row units)", "MatCreateDim", ["Object", "Object", "Object"], true, null,
            [qOrderMismatch, Side.Quire.Num(1, 0, "T"), qValMismatch], null,
            [mOrderMismatch, Side.Mathcad.Num(1, 0, "T"), mValMismatch]);

        foreach (int kind in Enumerable.Range(0, 4))
        {
            var qSource = Matrix(Side.Quire, "L", kind);
            var mSource = Matrix(Side.Mathcad, "L", kind);
            var qTarget = Matrix(Side.Quire, "identity", 0);
            var mTarget = Matrix(Side.Mathcad, "identity", 0);
            Compare($"CastPtrs(source kind {kind}, T)", "CastPtrs", ["MatVal", "Dim"], false,
                qTarget, [qSource, Side.Quire.D("T")],
                mTarget, [mSource, Side.Mathcad.D("T")]);
        }

        var qCtor = Side.Quire.T("Mcd.MatVal").GetConstructor(Instance, null, [Side.Quire.T("Mcd.MatVal"), Side.Quire.Dim], null)!;
        var mCtor = Side.Mathcad.T("Mcd.MatVal").GetConstructor(Instance, null, [Side.Mathcad.T("Mcd.MatVal"), Side.Mathcad.Dim], null)!;
        foreach (int kind in Enumerable.Range(0, 4))
        {
            var qInput = Matrix(Side.Quire, "L", kind);
            var mInput = Matrix(Side.Mathcad, "L", kind);
            MethodComparison.Compare($"MatVal.ctor(MatVal, Dim) kind {kind}", Contract,
                qCtor, null, [qInput, Side.Quire.D("T")],
                mCtor, null, [mInput, Side.Mathcad.D("T")],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));
        }
    }
}
