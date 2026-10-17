using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValAssertionsAndCastsDifferentialTests
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
            real[offset + row + stride * col] = row + col + 1;
        double[]? imag = imaginary ? new double[length] : null;
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride,
            new Hashtable(), Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("L"));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object? value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static string Show(Side side, object target, object? value)
    {
        if (value == null) return "null";
        if (ReferenceEquals(target, value)) return "same-instance";
        if (value.GetType().Name == "MatVal") return MatValDifferentialTests.Storage(side, value);
        return Reflect.Text(value);
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
    public void asVector_validates_column_count_or_raises_exception()
    {
        Runtime.Sync();
        const int token = 0x06000469;
        var types = new[] { typeof(string), typeof(uint) };

        CompareBoth("asVector returns this for one column vector", "asVector", types, token,
            ["not vector", 0u], rows: 3, cols: 1, stride: 4, offset: 1);

        CompareBoth("asVector returns this for 1x1 vector", "asVector", types, token,
            ["not vector", 2u], rows: 1, cols: 1, stride: 2, offset: 0);

        CompareBoth("asVector raises exception for 2 columns", "asVector", types, token,
            ["expected vector", 5u], rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("asVector raises exception for 0 columns", "asVector", types, token,
            ["expected vector", 1u], rows: 2, cols: 0, stride: 2, offset: 0);

        CompareBoth("asVector returns this even with imaginary values if 1 column", "asVector", types, token,
            ["not vector", 0u], (side, matrix) => ((double[])Get(matrix, "m_iarr")!)[1] = 5.0,
            rows: 2, cols: 1, stride: 3, offset: 1, imaginary: true);
    }

    [SkippableFact]
    public void asRealVector_validates_real_vector_status_or_raises_exception()
    {
        Runtime.Sync();
        const int token = 0x0600046a;
        var types = new[] { typeof(string), typeof(uint) };

        CompareBoth("asRealVector returns this for real vector", "asRealVector", types, token,
            ["not real vector", 0u], rows: 3, cols: 1, stride: 4, offset: 1);

        CompareBoth("asRealVector raises for multi-column matrix", "asRealVector", types, token,
            ["must be real vector", 3u], rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("asRealVector raises for nonzero imaginary value", "asRealVector", types, token,
            ["must be real vector", 7u], (side, matrix) => ((double[])Get(matrix, "m_iarr")!)[1] = 2.0,
            rows: 2, cols: 1, stride: 3, offset: 1, imaginary: true);

        CompareBoth("asRealVector raises for non-NumVal pointer", "asRealVector", types, token,
            ["must be real vector", 1u], (side, matrix) => Pointer(side, matrix, 0, 0, "string_ptr"),
            rows: 2, cols: 1, stride: 3, offset: 1);

        CompareBoth("asRealVector returns this for compatible NumVal pointer", "asRealVector", types, token,
            ["not real vector", 0u], (side, matrix) => Pointer(side, matrix, 0, 0, side.Num(5, 0, "L")),
            rows: 2, cols: 1, stride: 3, offset: 1);
    }

    [SkippableFact]
    public void asMatrix_returns_this_unconditionally()
    {
        Runtime.Sync();
        const int token = 0x0600046b;
        var types = new[] { typeof(string), typeof(uint) };

        CompareBoth("asMatrix returns this for 2x2 matrix", "asMatrix", types, token,
            ["message", 0u], rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("asMatrix returns this for 1 column vector", "asMatrix", types, token,
            ["message", 4u], rows: 3, cols: 1, stride: 4, offset: 1);

        CompareBoth("asMatrix returns this for empty matrix", "asMatrix", types, token,
            ["message", 1u], rows: 0, cols: 0, stride: 1, offset: 0);

        CompareBoth("asMatrix returns this for matrix with imaginary values", "asMatrix", types, token,
            ["message", 9u], rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);
    }

    [SkippableFact]
    public void asRealMatrix_validates_real_matrix_status_or_raises_exception()
    {
        Runtime.Sync();
        const int token = 0x0600046c;
        var types = new[] { typeof(string), typeof(uint) };

        CompareBoth("asRealMatrix returns this for real matrix", "asRealMatrix", types, token,
            ["not real matrix", 0u], rows: 2, cols: 3, stride: 4, offset: 1);

        CompareBoth("asRealMatrix returns this for real vector", "asRealMatrix", types, token,
            ["not real matrix", 0u], rows: 3, cols: 1, stride: 4, offset: 1);

        CompareBoth("asRealMatrix raises for nonzero imaginary value", "asRealMatrix", types, token,
            ["must be real matrix", 2u], (side, matrix) => ((double[])Get(matrix, "m_iarr")!)[1] = 4.0,
            rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);

        CompareBoth("asRealMatrix raises for non-NumVal pointer", "asRealMatrix", types, token,
            ["must be real matrix", 6u], (side, matrix) => Pointer(side, matrix, 0, 0, "text"),
            rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("asRealMatrix clears zero imaginary array and returns this", "asRealMatrix", types, token,
            ["not real matrix", 0u], rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);
    }

    [SkippableFact]
    public void assertRealVector_no_args_asserts_real_vector()
    {
        Runtime.Sync();
        const int token = 0x0600046e;
        var types = Type.EmptyTypes;

        CompareBoth("assertRealVector no-arg succeeds for real vector", "assertRealVector", types, token,
            [], rows: 3, cols: 1, stride: 4, offset: 1);

        CompareBoth("assertRealVector no-arg raises for multi-column matrix", "assertRealVector", types, token,
            [], rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("assertRealVector no-arg raises for imaginary vector", "assertRealVector", types, token,
            [], (side, matrix) => ((double[])Get(matrix, "m_iarr")!)[1] = 3.0,
            rows: 2, cols: 1, stride: 3, offset: 1, imaginary: true);
    }

    [SkippableFact]
    public void assertRealVector_with_param_asserts_real_vector_with_handle()
    {
        Runtime.Sync();
        const int token = 0x0600046f;
        var types = new[] { typeof(uint) };

        CompareBoth("assertRealVector with param succeeds for real vector", "assertRealVector", types, token,
            [42u], rows: 3, cols: 1, stride: 4, offset: 1);

        CompareBoth("assertRealVector with param raises for multi-column matrix", "assertRealVector", types, token,
            [7u], rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("assertRealVector with param raises for imaginary vector", "assertRealVector", types, token,
            [12u], (side, matrix) => ((double[])Get(matrix, "m_iarr")!)[1] = 1.5,
            rows: 2, cols: 1, stride: 3, offset: 1, imaginary: true);

        CompareBoth("assertRealVector with param raises for non-NumVal pointer", "assertRealVector", types, token,
            [3u], (side, matrix) => Pointer(side, matrix, 0, 0, "stale"),
            rows: 2, cols: 1, stride: 3, offset: 1);
    }

    [SkippableFact]
    public void assertRealMatrix_with_param_asserts_real_matrix_with_handle()
    {
        Runtime.Sync();
        const int token = 0x06000470;
        var types = new[] { typeof(uint) };

        CompareBoth("assertRealMatrix with param succeeds for real matrix", "assertRealMatrix", types, token,
            [5u], rows: 2, cols: 3, stride: 4, offset: 1);

        CompareBoth("assertRealMatrix with param raises for imaginary matrix", "assertRealMatrix", types, token,
            [9u], (side, matrix) => ((double[])Get(matrix, "m_iarr")!)[1] = 2.5,
            rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);

        CompareBoth("assertRealMatrix with param raises for non-NumVal pointer", "assertRealMatrix", types, token,
            [1u], (side, matrix) => Pointer(side, matrix, 0, 0, "text"),
            rows: 2, cols: 2, stride: 3, offset: 1);

        CompareBoth("assertRealMatrix with param clears zero imaginary array and succeeds", "assertRealMatrix", types, token,
            [0u], rows: 2, cols: 2, stride: 3, offset: 1, imaginary: true);
    }
}
