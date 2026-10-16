using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimArrayDimDirectDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int MakeArrayDimToken = 0x0600064b;

    private static object Matrix(Side side, int rows = 2, int cols = 2)
    {
        int stride = rows + 1, offset = 1;
        var real = new double[offset + stride * cols + 4];
        var imag = new double[real.Length];
        var matrix = New(side.T("Mcd.MatVal"), real, imag, rows, cols, stride, new Hashtable(),
            Enum.ToObject(side.T("Mcd.Flags"), 2146697216), offset);
        Set(matrix, "m_dim", side.D("identity"));
        return matrix;
    }

    private static void Pointer(Side side, object matrix, int row, int col, object value) =>
        Call(matrix, "set_ptrs_2", row, col, value);

    private static object Number(Side side, string dimension) => side.Num(1, 0, dimension);

    private static object NonScalarNumber(Side side) =>
        New(side.T("Mcd.NumVal"), 1.0, 0.0, side.D("function_dim"));

    private static string DescribeValue(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().Name == "NumVal")
            return $"number:dim={side.DimText(Get(value, "m_dim"))}:real={Get(value, "Real")}:imag={Get(value, "Imag")}";
        return $"{value.GetType().Name}:{Text(value)}";
    }

    private static string ShowInput(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) != true) return DescribeValue(side, value);
        var table = Get(value, "m_parr") as Hashtable;
        string entries = table == null ? "null" : string.Join(";", table.Cast<DictionaryEntry>().OrderBy(entry => (int)entry.Key)
            .Select(entry => $"{entry.Key}:{DescribeValue(side, entry.Value)}"));
        string real = ShowArray(Get(value, "m_rarr") as double[]);
        string imag = ShowArray(Get(value, "m_iarr") as double[]);
        var data = Get(value, "Data");
        return $"MatVal:{Get(value, "Rows")}x{Get(value, "Cols")}:stride={Get(value, "Anr")}:offset={Get(value, "Offset")}" +
            $":flags={Get(data!, "m_flags")}:immutable={Get(value, "Immutable")}" +
            $":dim={side.DimText(Get(value, "m_dim"))}:real=[{real}]:imag=[{imag}]:ptrs=[{entries}]";
    }

    private static string ShowArray(double[]? values) => values == null ? "null" :
        string.Join(",", values.Select(value => BitConverter.DoubleToInt64Bits(value).ToString("x16")));

    private static string ShowValue(Side side, object? value, object? input)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true) return ShowInput(side, value);
        int rows = (int)Get(value, "Rows")!; int cols = (int)Get(value, "Cols")!;
        var cells = new List<string>();
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
            cells.Add($"({row},{col})={side.DimText(Call(value, "dim", row, col))}");
        string retainedMatrix = value.GetType().Name == "crazy_dim"
            ? $":retainsInput={ReferenceEquals(Get(value, "m_matrix"), input)}"
            : string.Empty;
        return $"{value.GetType().Name}:{rows}x{cols}:uniform={side.DimText(Get(value, "UniformDim"))}:dims=[{string.Join(";", cells)}]{retainedMatrix}";
    }

    private static MethodInfo Method(Side side)
    {
        var matValType = side.T("Mcd.MatVal");
        return side.T("Mcd.MatDim").GetMethod("makeArrayDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [matValType], modifiers: null)
            ?? throw new HarnessException($"Missing {side.Name} MatDim.makeArrayDim(MatVal)");
    }

    private static void Compare(string caseId, object q, object m)
    {
        var qMethod = Method(Side.Quire); var mMethod = Method(Side.Mathcad);
        Assert.Equal(MakeArrayDimToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [q], mMethod, null, [m],
            value => ShowValue(Side.Quire, value, q), value => ShowValue(Side.Mathcad, value, m));
    }

    private static (object Q, object M) ColumnUniform()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "L"));
        return (q, m);
    }

    private static (object Q, object M) RowUniform()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "T"));
        return (q, m);
    }

    private static (object Q, object M) NonuniformScalarFactorPair()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        var qLFactor = Call(Side.Quire.Dim, "Mult", Side.Quire.D("L"), Side.Quire.D("v"))!;
        var mLFactor = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("L"), Side.Mathcad.D("v"))!;
        var qTFactor = Call(Side.Quire.Dim, "Mult", Side.Quire.D("T"), Side.Quire.D("v"))!;
        var mTFactor = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("T"), Side.Mathcad.D("v"))!;
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 0, 1, SideNumber(Side.Quire, qLFactor)); Pointer(Side.Mathcad, m, 0, 1, SideNumber(Side.Mathcad, mLFactor));
        Pointer(Side.Quire, q, 1, 1, SideNumber(Side.Quire, qTFactor)); Pointer(Side.Mathcad, m, 1, 1, SideNumber(Side.Mathcad, mTFactor));
        return (q, m);
    }

    private static (object Q, object M) NonuniformNonScalarFactorPair()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "L"));
        return (q, m);
    }

    private static (object Q, object M) FirstColumnUniformLaterColumnNonuniform()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "T"));
        return (q, m);
    }

    private static (object Q, object M) FirstColumnNonuniformFirstRowUniformLaterRowNonuniform()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "L"));
        return (q, m);
    }

    private static (object Q, object M) NullRowsWithNonuniformFirstRow()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, NonScalarNumber(Side.Quire)); Pointer(Side.Mathcad, m, 1, 0, NonScalarNumber(Side.Mathcad));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "L"));
        return (q, m);
    }

    private static (object Q, object M) NullColsWithNonuniformFirstColumn()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 0, 1, NonScalarNumber(Side.Quire)); Pointer(Side.Mathcad, m, 0, 1, NonScalarNumber(Side.Mathcad));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "L"));
        return (q, m);
    }

    private static object SideNumber(Side side, object dimension) => New(side.T("Mcd.NumVal"), 1.0, 0.0, dimension);

    [SkippableFact]
    public void Direct_makeArrayDim_uniform_columns_returns_column_dimension()
    {
        Runtime.Sync();
        var (q, m) = ColumnUniform();
        Compare("makeArrayDim uniform columns returns their dimension", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_uniform_rows_returns_row_dimension()
    {
        Runtime.Sync();
        var (q, m) = RowUniform();
        Compare("makeArrayDim uniform rows returns their dimension", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_nonuniform_axes_scalar_factorization_returns_rc_dim()
    {
        Runtime.Sync();
        var (q, m) = NonuniformScalarFactorPair();
        Compare("makeArrayDim nonuniform first axes with proportional columns returns rc_dim dimensions", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_nonuniform_axes_inconsistent_factorization_returns_crazy_dim()
    {
        Runtime.Sync();
        var (q, m) = NonuniformNonScalarFactorPair();
        Compare("makeArrayDim nonuniform first axes with inconsistent columns returns crazy_dim", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_nonuniform_later_column_returns_crazy_dim()
    {
        Runtime.Sync();
        var (q, m) = FirstColumnUniformLaterColumnNonuniform();
        Compare("makeArrayDim first column uniform and later column nonuniform returns crazy_dim", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_nonuniform_later_row_returns_crazy_dim()
    {
        Runtime.Sync();
        var (q, m) = FirstColumnNonuniformFirstRowUniformLaterRowNonuniform();
        Compare("makeArrayDim first column nonuniform and later row nonuniform returns crazy_dim", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_null_rows_short_circuit_returns_crazy_dim()
    {
        Runtime.Sync();
        var (q, m) = NullRowsWithNonuniformFirstRow();
        Compare("makeArrayDim nonscalar pointer makes rows null and nonuniform first row reaches null guard", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_null_cols_right_operand_returns_crazy_dim()
    {
        Runtime.Sync();
        var (q, m) = NullColsWithNonuniformFirstColumn();
        Compare("makeArrayDim nonscalar pointer makes cols null after nonuniform first column", q, m);
    }
}
