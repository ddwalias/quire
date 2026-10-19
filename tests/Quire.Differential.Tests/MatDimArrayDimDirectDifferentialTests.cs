using System.Collections;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimArrayDimDirectDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private const int MakeMatValBoolToken = 0x06000620;
    private const int MakeMatValToken = 0x06000621;
    private const int MakeArrayDimBoolToken = 0x0600064a;
    private const int MakeArrayDimToken = 0x0600064b;
    private const int MakeColumnVectorDimToken = 0x0600064c;
    private const int MakeRowVectorDimToken = 0x0600064d;

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

    private static object SideNumber(Side side, object dimension) => New(side.T("Mcd.NumVal"), 1.0, 0.0, dimension);

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
        if (value is bool b) return b ? "true" : "false";
        if (value is int i) return i.ToString();
        if (value is string s) return $"string({s})";
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

    private static (object Q, object M) ColumnUniform()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "T"));
        Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "T"));
        Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "T"));
        return (q, m);
    }

    private static (object Q, object M) RowUniform()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T"));
        Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "T"));
        Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "T"));
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

    [SkippableFact]
    public void Direct_makeArrayDim_matches_mathcad()
    {
        Runtime.Sync();

        var qMethod = sideT("Mcd.MatDim", Side.Quire).GetMethod("makeArrayDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Quire)], modifiers: null)!;
        var mMethod = sideT("Mcd.MatDim", Side.Mathcad).GetMethod("makeArrayDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Mathcad)], modifiers: null)!;

        Assert.Equal(MakeArrayDimToken, mMethod.MetadataToken);
        Assert.Equal(MathcadMvid, mMethod.Module.ModuleVersionId);

        void Compare(string caseId, object q, object m) =>
            MethodComparison.Compare(caseId, Contract, qMethod, null, [q], mMethod, null, [m],
                value => ShowValue(Side.Quire, value, q), value => ShowValue(Side.Mathcad, value, m));

        var (qCol, mCol) = ColumnUniform();
        Compare("makeArrayDim uniform columns returns c_dim", qCol, mCol);

        var (qRow, mRow) = RowUniform();
        Compare("makeArrayDim uniform rows returns r_dim", qRow, mRow);

        var (qProp, mProp) = NonuniformScalarFactorPair();
        Compare("makeArrayDim proportional columns returns rc_dim", qProp, mProp);

        var (qCrazy, mCrazy) = NonuniformNonScalarFactorPair();
        Compare("makeArrayDim inconsistent columns returns crazy_dim", qCrazy, mCrazy);

        var (qColNonunif, mColNonunif) = FirstColumnUniformLaterColumnNonuniform();
        Compare("makeArrayDim first column uniform later nonuniform returns crazy_dim", qColNonunif, mColNonunif);

        var (qRowNonunif, mRowNonunif) = FirstColumnNonuniformFirstRowUniformLaterRowNonuniform();
        Compare("makeArrayDim first column nonuniform later row nonuniform returns crazy_dim", qRowNonunif, mRowNonunif);

        var (qNullRows, mNullRows) = NullRowsWithNonuniformFirstRow();
        Compare("makeArrayDim null rows returns crazy_dim", qNullRows, mNullRows);

        var (qNullCols, mNullCols) = NullColsWithNonuniformFirstColumn();
        Compare("makeArrayDim null cols returns crazy_dim", qNullCols, mNullCols);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_with_bool_matches_mathcad()
    {
        Runtime.Sync();

        var qMethod = sideT("Mcd.MatDim", Side.Quire).GetMethod("makeArrayDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Quire), typeof(bool)], modifiers: null)!;
        var mMethod = sideT("Mcd.MatDim", Side.Mathcad).GetMethod("makeArrayDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Mathcad), typeof(bool)], modifiers: null)!;

        Assert.Equal(MakeArrayDimBoolToken, mMethod.MetadataToken);
        Assert.Equal(MathcadMvid, mMethod.Module.ModuleVersionId);

        void Compare(string caseId, object q, object m, bool linearOnly) =>
            MethodComparison.Compare(caseId, Contract, qMethod, null, [q, linearOnly], mMethod, null, [m, linearOnly],
                value => ShowValue(Side.Quire, value, q), value => ShowValue(Side.Mathcad, value, m));

        var (qProp, mProp) = NonuniformScalarFactorPair();
        Compare("makeArrayDim(m, false) proportional returns rc_dim", qProp, mProp, false);
        Compare("makeArrayDim(m, true) proportional returns rc_dim", qProp, mProp, true);

        var (qCrazy, mCrazy) = NonuniformNonScalarFactorPair();
        Compare("makeArrayDim(m, false) crazy returns crazy_dim", qCrazy, mCrazy, false);
        Compare("makeArrayDim(m, true) crazy returns null", qCrazy, mCrazy, true);

        var (qCol, mCol) = ColumnUniform();
        Compare("makeArrayDim(m, true) uniform columns returns c_dim", qCol, mCol, true);

        var (qRow, mRow) = RowUniform();
        Compare("makeArrayDim(m, true) uniform rows returns r_dim", qRow, mRow, true);
    }

    [SkippableFact]
    public void Direct_vector_and_matrix_factories_match_mathcad()
    {
        Runtime.Sync();

        var qColVecMethod = sideT("Mcd.MatDim", Side.Quire).GetMethod("makeColumnVectorDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Quire)], modifiers: null)!;
        var mColVecMethod = sideT("Mcd.MatDim", Side.Mathcad).GetMethod("makeColumnVectorDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Mathcad)], modifiers: null)!;
        Assert.Equal(MakeColumnVectorDimToken, mColVecMethod.MetadataToken);
        Assert.Equal(MathcadMvid, mColVecMethod.Module.ModuleVersionId);

        var qRowVecMethod = sideT("Mcd.MatDim", Side.Quire).GetMethod("makeRowVectorDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Quire)], modifiers: null)!;
        var mRowVecMethod = sideT("Mcd.MatDim", Side.Mathcad).GetMethod("makeRowVectorDim", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Mathcad)], modifiers: null)!;
        Assert.Equal(MakeRowVectorDimToken, mRowVecMethod.MetadataToken);
        Assert.Equal(MathcadMvid, mRowVecMethod.Module.ModuleVersionId);

        var qMakeMatValBool = sideT("Mcd.MatDim", Side.Quire).GetMethod("make", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Quire), typeof(bool)], modifiers: null)!;
        var mMakeMatValBool = sideT("Mcd.MatDim", Side.Mathcad).GetMethod("make", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Mathcad), typeof(bool)], modifiers: null)!;
        Assert.Equal(MakeMatValBoolToken, mMakeMatValBool.MetadataToken);
        Assert.Equal(MathcadMvid, mMakeMatValBool.Module.ModuleVersionId);

        var qMakeMatVal = sideT("Mcd.MatDim", Side.Quire).GetMethod("make", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Quire)], modifiers: null)!;
        var mMakeMatVal = sideT("Mcd.MatDim", Side.Mathcad).GetMethod("make", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [sideT("Mcd.MatVal", Side.Mathcad)], modifiers: null)!;
        Assert.Equal(MakeMatValToken, mMakeMatVal.MetadataToken);
        Assert.Equal(MathcadMvid, mMakeMatVal.Module.ModuleVersionId);

        // 1. Column vector (2x1)
        var qColVec = Matrix(Side.Quire, 2, 1); var mColVec = Matrix(Side.Mathcad, 2, 1);
        Pointer(Side.Quire, qColVec, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, mColVec, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, qColVec, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, mColVec, 1, 0, Number(Side.Mathcad, "T"));

        MethodComparison.Compare("makeColumnVectorDim valid", Contract,
            qColVecMethod, null, [qColVec], mColVecMethod, null, [mColVec],
            v => ShowValue(Side.Quire, v, qColVec), v => ShowValue(Side.Mathcad, v, mColVec));

        // Column vector with nonscalar (null ColDimArray) -> crazy_dim
        var qColVecCrazy = Matrix(Side.Quire, 2, 1); var mColVecCrazy = Matrix(Side.Mathcad, 2, 1);
        Pointer(Side.Quire, qColVecCrazy, 0, 0, NonScalarNumber(Side.Quire)); Pointer(Side.Mathcad, mColVecCrazy, 0, 0, NonScalarNumber(Side.Mathcad));
        Pointer(Side.Quire, qColVecCrazy, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, mColVecCrazy, 1, 0, Number(Side.Mathcad, "T"));

        MethodComparison.Compare("makeColumnVectorDim nonscalar crazy", Contract,
            qColVecMethod, null, [qColVecCrazy], mColVecMethod, null, [mColVecCrazy],
            v => ShowValue(Side.Quire, v, qColVecCrazy), v => ShowValue(Side.Mathcad, v, mColVecCrazy));

        // 2. Row vector (1x2)
        var qRowVec = Matrix(Side.Quire, 1, 2); var mRowVec = Matrix(Side.Mathcad, 1, 2);
        Pointer(Side.Quire, qRowVec, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, mRowVec, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, qRowVec, 0, 1, Number(Side.Quire, "T")); Pointer(Side.Mathcad, mRowVec, 0, 1, Number(Side.Mathcad, "T"));

        MethodComparison.Compare("makeRowVectorDim valid", Contract,
            qRowVecMethod, null, [qRowVec], mRowVecMethod, null, [mRowVec],
            v => ShowValue(Side.Quire, v, qRowVec), v => ShowValue(Side.Mathcad, v, mRowVec));

        // Row vector with nonscalar (null RowDimArray) -> crazy_dim
        var qRowVecCrazy = Matrix(Side.Quire, 1, 2); var mRowVecCrazy = Matrix(Side.Mathcad, 1, 2);
        Pointer(Side.Quire, qRowVecCrazy, 0, 0, NonScalarNumber(Side.Quire)); Pointer(Side.Mathcad, mRowVecCrazy, 0, 0, NonScalarNumber(Side.Mathcad));
        Pointer(Side.Quire, qRowVecCrazy, 0, 1, Number(Side.Quire, "T")); Pointer(Side.Mathcad, mRowVecCrazy, 0, 1, Number(Side.Mathcad, "T"));

        MethodComparison.Compare("makeRowVectorDim nonscalar crazy", Contract,
            qRowVecMethod, null, [qRowVecCrazy], mRowVecMethod, null, [mRowVecCrazy],
            v => ShowValue(Side.Quire, v, qRowVecCrazy), v => ShowValue(Side.Mathcad, v, mRowVecCrazy));

        // 3. MatDim.make(MatVal) and MatDim.make(MatVal, bool) entry points
        var (qProp, mProp) = NonuniformScalarFactorPair();
        var (qCrazy, mCrazy) = NonuniformNonScalarFactorPair();
        var qUniform = Matrix(Side.Quire, 2, 2); var mUniform = Matrix(Side.Mathcad, 2, 2);

        // Uniform 2x2 matrix routes to uniform_dim via make(MatVal, bool)
        MethodComparison.Compare("MatDim.make(MatVal, false) uniform matrix", Contract,
            qMakeMatValBool, null, [qUniform, false], mMakeMatValBool, null, [mUniform, false],
            v => ShowValue(Side.Quire, v, qUniform), v => ShowValue(Side.Mathcad, v, mUniform));

        MethodComparison.Compare("MatDim.make(MatVal, true) uniform matrix", Contract,
            qMakeMatValBool, null, [qUniform, true], mMakeMatValBool, null, [mUniform, true],
            v => ShowValue(Side.Quire, v, qUniform), v => ShowValue(Side.Mathcad, v, mUniform));

        // Single column routes to makeColumnVectorDim
        MethodComparison.Compare("MatDim.make(MatVal) col vector", Contract,
            qMakeMatVal, null, [qColVec], mMakeMatVal, null, [mColVec],
            v => ShowValue(Side.Quire, v, qColVec), v => ShowValue(Side.Mathcad, v, mColVec));

        MethodComparison.Compare("MatDim.make(MatVal, false) col vector", Contract,
            qMakeMatValBool, null, [qColVec, false], mMakeMatValBool, null, [mColVec, false],
            v => ShowValue(Side.Quire, v, qColVec), v => ShowValue(Side.Mathcad, v, mColVec));

        MethodComparison.Compare("MatDim.make(MatVal, true) col vector crazy kills mutant", Contract,
            qMakeMatValBool, null, [qColVecCrazy, true], mMakeMatValBool, null, [mColVecCrazy, true],
            v => ShowValue(Side.Quire, v, qColVecCrazy), v => ShowValue(Side.Mathcad, v, mColVecCrazy));

        // Single row routes to makeRowVectorDim
        MethodComparison.Compare("MatDim.make(MatVal) row vector", Contract,
            qMakeMatVal, null, [qRowVec], mMakeMatVal, null, [mRowVec],
            v => ShowValue(Side.Quire, v, qRowVec), v => ShowValue(Side.Mathcad, v, mRowVec));

        MethodComparison.Compare("MatDim.make(MatVal, false) row vector", Contract,
            qMakeMatValBool, null, [qRowVec, false], mMakeMatValBool, null, [mRowVec, false],
            v => ShowValue(Side.Quire, v, qRowVec), v => ShowValue(Side.Mathcad, v, mRowVec));

        MethodComparison.Compare("MatDim.make(MatVal, true) row vector crazy kills mutant", Contract,
            qMakeMatValBool, null, [qRowVecCrazy, true], mMakeMatValBool, null, [mRowVecCrazy, true],
            v => ShowValue(Side.Quire, v, qRowVecCrazy), v => ShowValue(Side.Mathcad, v, mRowVecCrazy));

        // General matrix routes to makeArrayDim
        MethodComparison.Compare("MatDim.make(MatVal) 2x2 matrix", Contract,
            qMakeMatVal, null, [qProp], mMakeMatVal, null, [mProp],
            v => ShowValue(Side.Quire, v, qProp), v => ShowValue(Side.Mathcad, v, mProp));

        MethodComparison.Compare("MatDim.make(MatVal) crazy returns crazy_dim kills mutant", Contract,
            qMakeMatVal, null, [qCrazy], mMakeMatVal, null, [mCrazy],
            v => ShowValue(Side.Quire, v, qCrazy), v => ShowValue(Side.Mathcad, v, mCrazy));

        MethodComparison.Compare("MatDim.make(MatVal, false) proportional", Contract,
            qMakeMatValBool, null, [qProp, false], mMakeMatValBool, null, [mProp, false],
            v => ShowValue(Side.Quire, v, qProp), v => ShowValue(Side.Mathcad, v, mProp));

        MethodComparison.Compare("MatDim.make(MatVal, true) proportional", Contract,
            qMakeMatValBool, null, [qProp, true], mMakeMatValBool, null, [mProp, true],
            v => ShowValue(Side.Quire, v, qProp), v => ShowValue(Side.Mathcad, v, mProp));

        MethodComparison.Compare("MatDim.make(MatVal, false) crazy returns crazy_dim", Contract,
            qMakeMatValBool, null, [qCrazy, false], mMakeMatValBool, null, [mCrazy, false],
            v => ShowValue(Side.Quire, v, qCrazy), v => ShowValue(Side.Mathcad, v, mCrazy));

        MethodComparison.Compare("MatDim.make(MatVal, true) crazy returns null", Contract,
            qMakeMatValBool, null, [qCrazy, true], mMakeMatValBool, null, [mCrazy, true],
            v => ShowValue(Side.Quire, v, qCrazy), v => ShowValue(Side.Mathcad, v, mCrazy));
    }

    private static System.Type sideT(string name, Side side) => side.T(name);
}
