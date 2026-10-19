using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValAccessDifferentialTests
{
    private const string Contract = "Mcd.MatVal";
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name, params Type[] parameterTypes) =>
        side.T("Mcd.MatVal").GetMethod(name, DeclaredInstance, null, parameterTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatVal.{name}({string.Join(", ", parameterTypes.Select(t => t.Name))})");

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

    private static string Show(Side side, object target, object? value)
    {
        if (value == null) return "null";
        if (ReferenceEquals(target, value)) return "same-instance";
        if (side.T("Mcd.MatVal").IsInstanceOfType(value)) return MatValDifferentialTests.Storage(side, value);
        if (side.Dim.IsInstanceOfType(value)) return (string)Reflect.Call(value, "ToString")!;
        if (side.T("Mcd.MatDim").IsInstanceOfType(value)) return MatDimDifferentialTests.DescribeDimension(side, value);
        return side.Show(value);
    }

    private static string State(Side side, object target) => MatValDifferentialTests.Storage(side, target);

    private static void Compare(string caseId, MethodInfo qMethod, object qTarget, object?[] qArgs,
        MethodInfo mMethod, object mTarget, object?[] mArgs, int expectedToken)
    {
        Assert.Equal(expectedToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, qArgs, mMethod, mTarget, mArgs,
            quireShow: value => Show(Side.Quire, qTarget, value),
            originalShow: value => Show(Side.Mathcad, mTarget, value),
            quireState: target => State(Side.Quire, target),
            originalState: target => State(Side.Mathcad, target));
    }

    private static void CompareBoth(string caseId, string methodName, Type[] parameterTypes, int token,
        object?[] args, Action<Side, object>? setup = null, int rows = 2, int cols = 2, int stride = 3, int offset = 1, bool imaginary = false)
    {
        var qTarget = Matrix(Side.Quire, rows, cols, stride, offset, imaginary);
        var mTarget = Matrix(Side.Mathcad, rows, cols, stride, offset, imaginary);
        setup?.Invoke(Side.Quire, qTarget);
        setup?.Invoke(Side.Mathcad, mTarget);
        var qMethod = Method(Side.Quire, methodName, parameterTypes);
        var mMethod = Method(Side.Mathcad, methodName, parameterTypes);
        Compare(caseId, qMethod, qTarget, args, mMethod, mTarget, args, token);
    }

    [SkippableFact]
    public void Direct_element_getters_match_mathcad()
    {
        // 0x060004d0: get_1(int)
        CompareBoth("get_1.vector.index0", "get_1", [typeof(int)], 0x060004d0, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_1.vector.index1", "get_1", [typeof(int)], 0x060004d0, [1], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_1.matrix.index_offset", "get_1", [typeof(int)], 0x060004d0, [2], rows: 2, cols: 2, stride: 3, offset: 1);
        CompareBoth("get_1.pointer", "get_1", [typeof(int)], 0x060004d0, [0],
            setup: (s, m) => Call(m, "set_ptrs_1", 0, s.Num(5, 6, "T")), rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x060004d1: get_2(int, int)
        CompareBoth("get_2.0_0", "get_2", [typeof(int), typeof(int)], 0x060004d1, [0, 0]);
        CompareBoth("get_2.1_1", "get_2", [typeof(int), typeof(int)], 0x060004d1, [1, 1]);
        CompareBoth("get_2.0_1", "get_2", [typeof(int), typeof(int)], 0x060004d1, [0, 1]);
        CompareBoth("get_2.pointer", "get_2", [typeof(int), typeof(int)], 0x060004d1, [1, 0],
            setup: (s, m) => Call(m, "set_ptrs_2", 1, 0, s.Str("hello")));

        // 0x060004d2: get_1_dim(int)
        CompareBoth("get_1_dim.default", "get_1_dim", [typeof(int)], 0x060004d2, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_1_dim.pointer", "get_1_dim", [typeof(int)], 0x060004d2, [0],
            setup: (s, m) => Call(m, "set_ptrs_1", 0, s.Num(1, 0, "T")), rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x060004d3: get_2_dim(int, int)
        CompareBoth("get_2_dim.default", "get_2_dim", [typeof(int), typeof(int)], 0x060004d3, [0, 0]);
        CompareBoth("get_2_dim.pointer", "get_2_dim", [typeof(int), typeof(int)], 0x060004d3, [0, 1],
            setup: (s, m) => Call(m, "set_ptrs_2", 0, 1, s.Num(1, 0, "T")));

        // 0x060004d4: get_1_matdim(int)
        CompareBoth("get_1_matdim.default", "get_1_matdim", [typeof(int)], 0x060004d4, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_1_matdim.pointer", "get_1_matdim", [typeof(int)], 0x060004d4, [1],
            setup: (s, m) => Call(m, "set_ptrs_1", 1, s.Num(2, 0, "T")), rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x060004d5: get_2_matdim(int, int)
        CompareBoth("get_2_matdim.default", "get_2_matdim", [typeof(int), typeof(int)], 0x060004d5, [0, 0]);
        CompareBoth("get_2_matdim.pointer", "get_2_matdim", [typeof(int), typeof(int)], 0x060004d5, [1, 1],
            setup: (s, m) => Call(m, "set_ptrs_2", 1, 1, s.Num(2, 0, "T")));

        // 0x060004d6: get_1_ptr(int)
        CompareBoth("get_1_ptr.none", "get_1_ptr", [typeof(int)], 0x060004d6, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_1_ptr.present", "get_1_ptr", [typeof(int)], 0x060004d6, [0],
            setup: (s, m) => Call(m, "set_ptrs_1", 0, s.Str("ptr")), rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x060004d7: get_2_ptr(int, int)
        CompareBoth("get_2_ptr.none", "get_2_ptr", [typeof(int), typeof(int)], 0x060004d7, [0, 0]);
        CompareBoth("get_2_ptr.present", "get_2_ptr", [typeof(int), typeof(int)], 0x060004d7, [0, 1],
            setup: (s, m) => Call(m, "set_ptrs_2", 0, 1, s.Str("ptr2")));
    }

    [SkippableFact]
    public void Direct_real_imag_and_mixed_getters_match_mathcad()
    {
        // 0x060004e8: get_real(int)
        CompareBoth("get_real.vector", "get_real", [typeof(int)], 0x060004e8, [1], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_real.matrix_offset", "get_real", [typeof(int)], 0x060004e8, [1], rows: 2, cols: 2, stride: 3, offset: 1);

        // 0x060004e9: get_real(int, int)
        CompareBoth("get_real.2d.0_0", "get_real", [typeof(int), typeof(int)], 0x060004e9, [0, 0]);
        CompareBoth("get_real.2d.1_1", "get_real", [typeof(int), typeof(int)], 0x060004e9, [1, 1]);

        // 0x060004ea: get_imag(int)
        CompareBoth("get_imag.vector", "get_imag", [typeof(int)], 0x060004ea, [0], rows: 2, cols: 1, stride: 2, offset: 0, imaginary: true);

        // 0x060004eb: get_imag(int, int)
        CompareBoth("get_imag.2d", "get_imag", [typeof(int), typeof(int)], 0x060004eb, [1, 0], imaginary: true);

        // 0x060004ec: get_real_raw(int)
        CompareBoth("get_real_raw", "get_real_raw", [typeof(int)], 0x060004ec, [2]);

        // 0x060004ed: get_imag_raw(int)
        CompareBoth("get_imag_raw", "get_imag_raw", [typeof(int)], 0x060004ed, [2], imaginary: true);

        // 0x060004ee: get_real_mixed(int)
        CompareBoth("get_real_mixed.real", "get_real_mixed", [typeof(int)], 0x060004ee, [0], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_real_mixed.pointer_num", "get_real_mixed", [typeof(int)], 0x060004ee, [0],
            setup: (s, m) => Call(m, "set_ptrs_1", 0, s.Num(42, 0, "L")), rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_real_mixed.pointer_non_num", "get_real_mixed", [typeof(int)], 0x060004ee, [0],
            setup: (s, m) => Call(m, "set_ptrs_1", 0, s.Str("fail")), rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x060004ef: get_real_mixed(int, int)
        CompareBoth("get_real_mixed.2d.real", "get_real_mixed", [typeof(int), typeof(int)], 0x060004ef, [1, 1]);
        CompareBoth("get_real_mixed.2d.pointer_num", "get_real_mixed", [typeof(int), typeof(int)], 0x060004ef, [0, 1],
            setup: (s, m) => Call(m, "set_ptrs_2", 0, 1, s.Num(99, 0, "L")));
        CompareBoth("get_real_mixed.2d.pointer_non_num", "get_real_mixed", [typeof(int), typeof(int)], 0x060004ef, [0, 1],
            setup: (s, m) => Call(m, "set_ptrs_2", 0, 1, s.Str("fail")));

        // 0x060004f0: get_real_mixed_handle(int, uint)
        CompareBoth("get_real_mixed_handle.real", "get_real_mixed_handle", [typeof(int), typeof(uint)], 0x060004f0, [0, 42u], rows: 2, cols: 1, stride: 2, offset: 0);
        CompareBoth("get_real_mixed_handle.pointer_non_num", "get_real_mixed_handle", [typeof(int), typeof(uint)], 0x060004f0, [0, 42u],
            setup: (s, m) => Call(m, "set_ptrs_1", 0, s.Str("fail")), rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x060004f1: get_real_mixed_handle(int, int, uint)
        CompareBoth("get_real_mixed_handle.2d.real", "get_real_mixed_handle", [typeof(int), typeof(int), typeof(uint)], 0x060004f1, [0, 0, 99u]);
        CompareBoth("get_real_mixed_handle.2d.pointer_non_num", "get_real_mixed_handle", [typeof(int), typeof(int), typeof(uint)], 0x060004f1, [0, 0, 99u],
            setup: (s, m) => Call(m, "set_ptrs_2", 0, 0, s.Str("fail")));
    }

    [SkippableFact]
    public void Direct_setters_match_mathcad()
    {
        // 0x06000509: set_1(int, object)
        var qTarget1 = Matrix(Side.Quire, rows: 2, cols: 1, stride: 2, offset: 0);
        var mTarget1 = Matrix(Side.Mathcad, rows: 2, cols: 1, stride: 2, offset: 0);
        Compare("set_1.object",
            Method(Side.Quire, "set_1", typeof(int), typeof(object)), qTarget1, [0, Side.Quire.Num(7, 8, "L")],
            Method(Side.Mathcad, "set_1", typeof(int), typeof(object)), mTarget1, [0, Side.Mathcad.Num(7, 8, "L")],
            0x06000509);

        // 0x0600050a: set_2(int, int, object)
        var qTarget2 = Matrix(Side.Quire);
        var mTarget2 = Matrix(Side.Mathcad);
        Compare("set_2.object",
            Method(Side.Quire, "set_2", typeof(int), typeof(int), typeof(object)), qTarget2, [1, 1, Side.Quire.Num(9, 10, "L")],
            Method(Side.Mathcad, "set_2", typeof(int), typeof(int), typeof(object)), mTarget2, [1, 1, Side.Mathcad.Num(9, 10, "L")],
            0x0600050a);

        // 0x0600050e: set_1(int, double, Dim)
        var qTarget3 = Matrix(Side.Quire, rows: 2, cols: 1, stride: 2, offset: 0);
        var mTarget3 = Matrix(Side.Mathcad, rows: 2, cols: 1, stride: 2, offset: 0);
        Compare("set_1.double_dim",
            Method(Side.Quire, "set_1", typeof(int), typeof(double), Side.Quire.T("Mcd.Dim")), qTarget3, [0, 3.14, Side.Quire.D("L")],
            Method(Side.Mathcad, "set_1", typeof(int), typeof(double), Side.Mathcad.T("Mcd.Dim")), mTarget3, [0, 3.14, Side.Mathcad.D("L")],
            0x0600050e);

        // 0x0600050f: set_2(int, int, double, Dim)
        var qTarget4 = Matrix(Side.Quire);
        var mTarget4 = Matrix(Side.Mathcad);
        Compare("set_2.double_dim",
            Method(Side.Quire, "set_2", typeof(int), typeof(int), typeof(double), Side.Quire.T("Mcd.Dim")), qTarget4, [0, 1, 2.71, Side.Quire.D("L")],
            Method(Side.Mathcad, "set_2", typeof(int), typeof(int), typeof(double), Side.Mathcad.T("Mcd.Dim")), mTarget4, [0, 1, 2.71, Side.Mathcad.D("L")],
            0x0600050f);

        // 0x06000510: set_1(int, double, double, Dim)
        var qTarget5 = Matrix(Side.Quire, rows: 2, cols: 1, stride: 2, offset: 0);
        var mTarget5 = Matrix(Side.Mathcad, rows: 2, cols: 1, stride: 2, offset: 0);
        Compare("set_1.double_double_dim",
            Method(Side.Quire, "set_1", typeof(int), typeof(double), typeof(double), Side.Quire.T("Mcd.Dim")), qTarget5, [1, 1.0, 2.0, Side.Quire.D("L")],
            Method(Side.Mathcad, "set_1", typeof(int), typeof(double), typeof(double), Side.Mathcad.T("Mcd.Dim")), mTarget5, [1, 1.0, 2.0, Side.Mathcad.D("L")],
            0x06000510);

        // 0x06000511: set_2(int, int, double, double, Dim)
        var qTarget6 = Matrix(Side.Quire);
        var mTarget6 = Matrix(Side.Mathcad);
        Compare("set_2.double_double_dim",
            Method(Side.Quire, "set_2", typeof(int), typeof(int), typeof(double), typeof(double), Side.Quire.T("Mcd.Dim")), qTarget6, [1, 0, 3.0, 4.0, Side.Quire.D("L")],
            Method(Side.Mathcad, "set_2", typeof(int), typeof(int), typeof(double), typeof(double), Side.Mathcad.T("Mcd.Dim")), mTarget6, [1, 0, 3.0, 4.0, Side.Mathcad.D("L")],
            0x06000511);

        // 0x06000517: set_ptrs_1(int, object)
        var qTarget7 = Matrix(Side.Quire, rows: 2, cols: 1, stride: 2, offset: 0);
        var mTarget7 = Matrix(Side.Mathcad, rows: 2, cols: 1, stride: 2, offset: 0);
        Compare("set_ptrs_1",
            Method(Side.Quire, "set_ptrs_1", typeof(int), typeof(object)), qTarget7, [0, Side.Quire.Str("p1")],
            Method(Side.Mathcad, "set_ptrs_1", typeof(int), typeof(object)), mTarget7, [0, Side.Mathcad.Str("p1")],
            0x06000517);

        // 0x06000518: set_ptrs_2(int, int, object)
        var qTarget8 = Matrix(Side.Quire);
        var mTarget8 = Matrix(Side.Mathcad);
        Compare("set_ptrs_2",
            Method(Side.Quire, "set_ptrs_2", typeof(int), typeof(int), typeof(object)), qTarget8, [0, 1, Side.Quire.Str("p2")],
            Method(Side.Mathcad, "set_ptrs_2", typeof(int), typeof(int), typeof(object)), mTarget8, [0, 1, Side.Mathcad.Str("p2")],
            0x06000518);

        // 0x0600051c: set_2i(int, int, double)
        CompareBoth("set_2i", "set_2i", [typeof(int), typeof(int), typeof(double)], 0x0600051c, [0, 1, 5.5], imaginary: true);

        // 0x0600051d: set_1(int, double)
        CompareBoth("set_1.double", "set_1", [typeof(int), typeof(double)], 0x0600051d, [1, 8.8], rows: 2, cols: 1, stride: 2, offset: 0);

        // 0x0600051e: set_2(int, int, double)
        CompareBoth("set_2.double", "set_2", [typeof(int), typeof(int), typeof(double)], 0x0600051e, [1, 0, 7.7]);
    }
}
