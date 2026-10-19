using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValIndexedDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name, params Type[] parameterTypes) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, parameterTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}({string.Join(", ", parameterTypes.Select(t => t.Name))})");

    private static MethodInfo StaticMethod(Side side, string name, params Type[] parameterTypes) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredStatic, null, parameterTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name} (static)({string.Join(", ", parameterTypes.Select(t => t.Name))})");

    private static object Matrix(Side side, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = false)
    {
        int length = offset + stride * cols;
        var real = new double[length];
        for (int col = 0; col < cols; col++)
        for (int row = 0; row < rows; row++)
            real[offset + row + stride * col] = (row + 1) * 10 + (col + 1);
        double[]? imag = imaginary ? new double[length] : null;
        if (imag != null)
        {
            for (int col = 0; col < cols; col++)
            for (int row = 0; row < rows; row++)
                imag[offset + row + stride * col] = (row + 1) * 100 + (col + 1);
        }
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static string Show(Side side, object? target, object? value)
    {
        if (value == null) return "null";
        string prefix = target != null && ReferenceEquals(target, value) ? "same-instance: " : "";
        if (side.T("Mcd.MatVal").IsInstanceOfType(value)) return prefix + MatValDifferentialTests.Storage(side, value);
        if (side.Dim.IsInstanceOfType(value)) return prefix + (string)Reflect.Call(value, "ToString")!;
        if (side.T("Mcd.MatDim").IsInstanceOfType(value)) return prefix + MatDimDifferentialTests.DescribeDimension(side, value);
        return prefix + side.Show(value);
    }

    private static string State(Side side, object target) => MatValDifferentialTests.Storage(side, target);

    private static void Compare(string caseId, MethodInfo qMethod, object? qTarget, object?[] qArgs,
        MethodInfo mMethod, object? mTarget, object?[] mArgs, int expectedToken)
    {
        Assert.Equal(expectedToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs,
            quireShow: value => Show(Side.Quire, qTarget, value),
            originalShow: value => Show(Side.Mathcad, mTarget, value),
            quireState: target => State(Side.Quire, target),
            originalState: target => State(Side.Mathcad, target));
    }

    private static void CompareDivision(string caseId, MethodInfo qMethod, object qMat, double qScalar,
        MethodInfo mMethod, object mMat, double mScalar, int expectedToken)
    {
        Assert.Equal(expectedToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qMat, qScalar], mMethod, null, [mMat, mScalar],
            quireShow: value => Show(Side.Quire, qMat, value),
            originalShow: value => Show(Side.Mathcad, mMat, value));
    }

    private static void CompareBoth(string caseId, string methodName, Type[] parameterTypes, int token,
        Func<Side, object?[]> args, Action<Side, object>? setup = null, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = false)
    {
        var qTarget = Matrix(Side.Quire, rows, cols, stride, offset, imaginary);
        var mTarget = Matrix(Side.Mathcad, rows, cols, stride, offset, imaginary);
        setup?.Invoke(Side.Quire, qTarget);
        setup?.Invoke(Side.Mathcad, mTarget);
        var qArgs = args(Side.Quire);
        var mArgs = args(Side.Mathcad);
        var qMethod = Method(Side.Quire, methodName, parameterTypes);
        var mMethod = Method(Side.Mathcad, methodName, parameterTypes);
        Compare(caseId, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs, token);
    }

    private static void CompareBoth(string caseId, string methodName, Type[] parameterTypes, int token,
        object?[] args, Action<Side, object>? setup = null, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = false) =>
        CompareBoth(caseId, methodName, parameterTypes, token, _ => args, setup, rows, cols, stride, offset, imaginary);

    [SkippableFact]
    public void Checked_and_symbolic_index_getters_match_mathcad()
    {
        // 0x060004bf: mcd_get_1(uint)
        CompareBoth("mcd_get_1.vector.0", "mcd_get_1", [typeof(uint)], 0x060004bf, [0u], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("mcd_get_1.vector.1", "mcd_get_1", [typeof(uint)], 0x060004bf, [1u], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("mcd_get_1.vector.oob", "mcd_get_1", [typeof(uint)], 0x060004bf, [5u], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("mcd_get_1.matrix_fails", "mcd_get_1", [typeof(uint)], 0x060004bf, [0u], rows: 2, cols: 2, stride: 3, offset: 1);

        // 0x060004c0: mcd_sym_get_1(uint)
        CompareBoth("mcd_sym_get_1.vector.0", "mcd_sym_get_1", [typeof(uint)], 0x060004c0, [0u], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("mcd_sym_get_1.vector.oob", "mcd_sym_get_1", [typeof(uint)], 0x060004c0, [5u], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("mcd_sym_get_1.matrix_fails", "mcd_sym_get_1", [typeof(uint)], 0x060004c0, [0u], rows: 2, cols: 2, stride: 3, offset: 1);

        // 0x060004c2: mcd_get_2(uint, uint)
        CompareBoth("mcd_get_2.0_0", "mcd_get_2", [typeof(uint), typeof(uint)], 0x060004c2, [0u, 0u]);
        CompareBoth("mcd_get_2.1_1", "mcd_get_2", [typeof(uint), typeof(uint)], 0x060004c2, [1u, 1u]);
        CompareBoth("mcd_get_2.row_oob", "mcd_get_2", [typeof(uint), typeof(uint)], 0x060004c2, [5u, 0u]);
        CompareBoth("mcd_get_2.col_oob", "mcd_get_2", [typeof(uint), typeof(uint)], 0x060004c2, [0u, 5u]);

        // 0x060004c3: mcd_sym_get_2(uint, uint)
        CompareBoth("mcd_sym_get_2.0_0", "mcd_sym_get_2", [typeof(uint), typeof(uint)], 0x060004c3, [0u, 0u]);
        CompareBoth("mcd_sym_get_2.row_oob", "mcd_sym_get_2", [typeof(uint), typeof(uint)], 0x060004c3, [5u, 0u]);
        CompareBoth("mcd_sym_get_2.col_oob", "mcd_sym_get_2", [typeof(uint), typeof(uint)], 0x060004c3, [0u, 5u]);

        // 0x060004c6: mcd_get_1(int)
        CompareBoth("mcd_get_1_int.0", "mcd_get_1", [typeof(int)], 0x060004c6, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        // 0x060004c7: mcd_sym_get_1(int)
        CompareBoth("mcd_sym_get_1_int.0", "mcd_sym_get_1", [typeof(int)], 0x060004c7, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        // 0x060004c8: mcd_get_2(int, int)
        CompareBoth("mcd_get_2_int.0_0", "mcd_get_2", [typeof(int), typeof(int)], 0x060004c8, [0, 0]);
        // 0x060004c9: mcd_sym_get_2(int, int)
        CompareBoth("mcd_sym_get_2_int.0_0", "mcd_sym_get_2", [typeof(int), typeof(int)], 0x060004c9, [0, 0]);

        // 0x060004b8: matrix_get_1(int)
        CompareBoth("matrix_get_1.0", "matrix_get_1", [typeof(int)], 0x060004b8, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        // 0x060004b9: sym_matrix_get_1(int)
        CompareBoth("sym_matrix_get_1.0", "sym_matrix_get_1", [typeof(int)], 0x060004b9, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        // 0x060004ba: matrix_get_2(int, int)
        CompareBoth("matrix_get_2.0_0", "matrix_get_2", [typeof(int), typeof(int)], 0x060004ba, [0, 0]);
        // 0x060004bb: sym_matrix_get_2(int, int)
        CompareBoth("sym_matrix_get_2.0_0", "sym_matrix_get_2", [typeof(int), typeof(int)], 0x060004bb, [0, 0]);

        // 0x060004ca: aref_1(int)
        CompareBoth("aref_1.0", "aref_1", [typeof(int)], 0x060004ca, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        // 0x060004cb: aref_2(int, int)
        CompareBoth("aref_2.0_0", "aref_2", [typeof(int), typeof(int)], 0x060004cb, [0, 0]);

        // 0x060004bd: iterator_get(int)
        CompareBoth("iterator_get.0", "iterator_get", [typeof(int)], 0x060004bd, [0]);
        CompareBoth("iterator_get.1", "iterator_get", [typeof(int)], 0x060004bd, [1]);
        CompareBoth("iterator_get.2", "iterator_get", [typeof(int)], 0x060004bd, [2]);
        CompareBoth("iterator_get.3", "iterator_get", [typeof(int)], 0x060004bd, [3]);
    }

    [SkippableFact]
    public void Dynamic_and_static_call_dispatch_matches_mathcad()
    {
        // 0x060004b5: DynCall(object)
        CompareBoth("DynCall.vector", "DynCall", [typeof(object)], 0x060004b5, s => [s.Num(0, 0, "identity")],
            rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("DynCall.vector.non_integer", "DynCall", [typeof(object)], 0x060004b5, s => [s.Num(0.5, 0, "identity")],
            rows: 2, cols: 1, stride: 2, offset: 0);
        // 0x060004b6: DynCall(object, object)
        CompareBoth("DynCall.matrix", "DynCall", [typeof(object), typeof(object)], 0x060004b6,
            s => [s.Num(1, 0, "identity"), s.Num(0, 0, "identity")]);
        CompareBoth("DynCall.matrix.non_int_first", "DynCall", [typeof(object), typeof(object)], 0x060004b6,
            s => [s.Num(1.5, 0, "identity"), s.Num(0, 0, "identity")]);
        CompareBoth("DynCall.matrix.non_int_second", "DynCall", [typeof(object), typeof(object)], 0x060004b6,
            s => [s.Num(1, 0, "identity"), s.Num(0.5, 0, "identity")]);

        // 0x060004b7: mcd_get_1(object, object) (static)
        {
            var qVec = Matrix(Side.Quire, rows: 2, cols: 1, stride: 2, offset: 0);
            var mVec = Matrix(Side.Mathcad, rows: 2, cols: 1, stride: 2, offset: 0);
            var qMethod = StaticMethod(Side.Quire, "mcd_get_1", typeof(object), typeof(object));
            var mMethod = StaticMethod(Side.Mathcad, "mcd_get_1", typeof(object), typeof(object));
            Compare("mcd_get_1.static.valid", qMethod, null, [qVec, Side.Quire.Num(0, 0, "identity")],
                mMethod, null, [mVec, Side.Mathcad.Num(0, 0, "identity")], 0x060004b7);
            Compare("mcd_get_1.static.non_vector", qMethod, null, [Matrix(Side.Quire), Side.Quire.Num(0, 0, "identity")],
                mMethod, null, [Matrix(Side.Mathcad), Side.Mathcad.Num(0, 0, "identity")], 0x060004b7);
            Compare("mcd_get_1.static.non_integer", qMethod, null, [qVec, "bad"],
                mMethod, null, [mVec, "bad"], 0x060004b7);
        }

        // 0x060004be: mcd_getm_1(object, int) (static)
        {
            var qVec = Matrix(Side.Quire, rows: 2, cols: 1, stride: 2, offset: 0);
            var mVec = Matrix(Side.Mathcad, rows: 2, cols: 1, stride: 2, offset: 0);
            var qMethod = StaticMethod(Side.Quire, "mcd_getm_1", typeof(object), typeof(int));
            var mMethod = StaticMethod(Side.Mathcad, "mcd_getm_1", typeof(object), typeof(int));
            Compare("mcd_getm_1.static.valid", qMethod, null, [qVec, 0], mMethod, null, [mVec, 0], 0x060004be);
            Compare("mcd_getm_1.static.non_vector", qMethod, null, [Matrix(Side.Quire), 0], mMethod, null, [Matrix(Side.Mathcad), 0], 0x060004be);
        }

        // 0x060004c1: mcd_getm_2(object, int, int) (static)
        {
            var qMat = Matrix(Side.Quire);
            var mMat = Matrix(Side.Mathcad);
            var qMethod = StaticMethod(Side.Quire, "mcd_getm_2", typeof(object), typeof(int), typeof(int));
            var mMethod = StaticMethod(Side.Mathcad, "mcd_getm_2", typeof(object), typeof(int), typeof(int));
            Compare("mcd_getm_2.static.valid", qMethod, null, [qMat, 0, 1], mMethod, null, [mMat, 0, 1], 0x060004c1);
            Compare("mcd_getm_2.static.non_matrix", qMethod, null, ["bad", 0, 0], mMethod, null, ["bad", 0, 0], 0x060004c1);
        }
    }

    [SkippableFact]
    public void Axis_enumeration_and_classification_match_mathcad()
    {
        var rowAxisQ = Enum.ToObject(Side.Quire.T("Mcd.MatAxis"), 0);
        var rowAxisM = Enum.ToObject(Side.Mathcad.T("Mcd.MatAxis"), 0);
        var colAxisQ = Enum.ToObject(Side.Quire.T("Mcd.MatAxis"), 1);
        var colAxisM = Enum.ToObject(Side.Mathcad.T("Mcd.MatAxis"), 1);

        // 0x060004db: get_type_2(int, int, double)
        CompareBoth("get_type_2.finite", "get_type_2", [typeof(int), typeof(int), typeof(double)], 0x060004db, [0, 0, 1e-6]);
        CompareBoth("get_type_2.nan_thresh", "get_type_2", [typeof(int), typeof(int), typeof(double)], 0x060004db, [1, 1, double.NaN]);
        CompareBoth("get_type_2.cell_1_1_nan", "get_type_2", [typeof(int), typeof(int), typeof(double)], 0x060004db,
            [1, 1, 1e-6],
            setup: (side, mat) =>
            {
                var rarr = (double[])Get(mat, "m_rarr")!;
                rarr[5] = double.NaN;
            },
            rows: 2, cols: 2, stride: 3, offset: 1);
        CompareBoth("get_type_2.col2_nan", "get_type_2", [typeof(int), typeof(int), typeof(double)], 0x060004db,
            [0, 2, 1e-6],
            setup: (side, mat) =>
            {
                var rarr = (double[])Get(mat, "m_rarr")!;
                rarr[4] = double.NaN;
            },
            rows: 2, cols: 3, stride: 2, offset: 0);

        // 0x060004de: element_types(MatAxis, int, double)
        {
            var qMat = Matrix(Side.Quire);
            var mMat = Matrix(Side.Mathcad);
            var qMethod = Method(Side.Quire, "element_types", Side.Quire.T("Mcd.MatAxis"), typeof(int), typeof(double));
            var mMethod = Method(Side.Mathcad, "element_types", Side.Mathcad.T("Mcd.MatAxis"), typeof(int), typeof(double));
            Compare("element_types.row0", qMethod, qMat, [rowAxisQ, 0, 1e-6], mMethod, mMat, [rowAxisM, 0, 1e-6], 0x060004de);
            Compare("element_types.col1", qMethod, qMat, [colAxisQ, 1, double.NaN], mMethod, mMat, [colAxisM, 1, double.NaN], 0x060004de);
        }

        // 0x060004df: get_ptrs(MatAxis, int)
        {
            var qMat = Matrix(Side.Quire);
            var mMat = Matrix(Side.Mathcad);
            var qMethod = Method(Side.Quire, "get_ptrs", Side.Quire.T("Mcd.MatAxis"), typeof(int));
            var mMethod = Method(Side.Mathcad, "get_ptrs", Side.Mathcad.T("Mcd.MatAxis"), typeof(int));
            Compare("get_ptrs.row0", qMethod, qMat, [rowAxisQ, 0], mMethod, mMat, [rowAxisM, 0], 0x060004df);
            Compare("get_ptrs.col0", qMethod, qMat, [colAxisQ, 0], mMethod, mMat, [colAxisM, 0], 0x060004df);
        }

        // 0x060004e1: get_doubles(MatAxis, int, double, double)
        {
            var qMat = Matrix(Side.Quire);
            var mMat = Matrix(Side.Mathcad);
            var qMethod = Method(Side.Quire, "get_doubles", Side.Quire.T("Mcd.MatAxis"), typeof(int), typeof(double), typeof(double));
            var mMethod = Method(Side.Mathcad, "get_doubles", Side.Mathcad.T("Mcd.MatAxis"), typeof(int), typeof(double), typeof(double));
            Compare("get_doubles.row0", qMethod, qMat, [rowAxisQ, 0, 1e-6, 2.0], mMethod, mMat, [rowAxisM, 0, 1e-6, 2.0], 0x060004e1);
        }

        // 0x060004e3: get_data(MatAxis, int, double, double)
        {
            var qMat = Matrix(Side.Quire);
            var mMat = Matrix(Side.Mathcad);
            var qMethod = Method(Side.Quire, "get_data", Side.Quire.T("Mcd.MatAxis"), typeof(int), typeof(double), typeof(double));
            var mMethod = Method(Side.Mathcad, "get_data", Side.Mathcad.T("Mcd.MatAxis"), typeof(int), typeof(double), typeof(double));
            Compare("get_data.row0", qMethod, qMat, [rowAxisQ, 0, 1e-6, 2.0], mMethod, mMat, [rowAxisM, 0, 1e-6, 2.0], 0x060004e3);
        }

        // 0x06000539: PackObjects()
        CompareBoth("PackObjects.0", "PackObjects", [], 0x06000539, []);
        // 0x06000538: PackObjects(int)
        CompareBoth("PackObjects.offset2", "PackObjects", [typeof(int)], 0x06000538, [2]);

        // 0x06000556: op_Division(MatVal, double)
        {
            var qDiv = StaticMethod(Side.Quire, "op_Division", Side.Quire.T("Mcd.MatVal"), typeof(double));
            var mDiv = StaticMethod(Side.Mathcad, "op_Division", Side.Mathcad.T("Mcd.MatVal"), typeof(double));

            var qMatUnpacked = Matrix(Side.Quire);
            var mMatUnpacked = Matrix(Side.Mathcad);
            CompareDivision("op_Division.unpacked", qDiv, qMatUnpacked, 2.0, mDiv, mMatUnpacked, 2.0, 0x06000556);

            var qMatPacked = Matrix(Side.Quire, rows: 2, cols: 2, stride: 2, offset: 0);
            var mMatPacked = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 2, offset: 0);
            CompareDivision("op_Division.packed_mutable", qDiv, qMatPacked, 2.0, mDiv, mMatPacked, 2.0, 0x06000556);

            var qMatImmutable = Matrix(Side.Quire, rows: 2, cols: 2, stride: 2, offset: 0);
            var mMatImmutable = Matrix(Side.Mathcad, rows: 2, cols: 2, stride: 2, offset: 0);
            Set(qMatImmutable, "Immutable", true);
            Set(mMatImmutable, "Immutable", true);
            CompareDivision("op_Division.packed_immutable", qDiv, qMatImmutable, 2.0, mDiv, mMatImmutable, 2.0, 0x06000556);
        }
    }
}
