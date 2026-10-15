using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimArrayDimLoopDispatchDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int MakeArrayDimToken = 0x0600064b;
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly System.Type QMatVal = typeof(global::Quire.Mcd.MatVal);
    private static readonly System.Type QMatDim = typeof(global::Quire.Mcd.MatDim);

    private sealed class QuireCountingDimVal : global::Quire.Mcd.DimVal
    {
        public int Calls;
        public Action? OnFirstCall;
        public override global::Quire.Mcd.Dim UniformDimension
        {
            get
            {
                Calls++;
                if (Calls == 1) OnFirstCall?.Invoke();
                return m_dim;
            }
        }
    }

    private static readonly Lazy<System.Type> MathcadCountingType = new(CreateMathcadCountingType);

    private static System.Type CreateMathcadCountingType()
    {
        var baseType = Mathcad.Type("mplrun", "Mcd.DimVal");
        var baseConstructor = baseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null, Type.EmptyTypes, modifiers: null)
            ?? throw new HarnessException("Missing Mathcad DimVal parameterless constructor");
        var baseDimension = baseType.GetField("m_dim", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new HarnessException("Missing Mathcad DimVal.m_dim field");
        var baseGetter = baseType.GetMethod("get_UniformDimension", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new HarnessException("Missing Mathcad DimVal.UniformDimension getter");

        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Quire.DimensionArrayCounterFixture"), AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule("CounterFixture");
        var builder = module.DefineType("McdDimensionArrayCounter", TypeAttributes.Public | TypeAttributes.Sealed, baseType);
        var calls = builder.DefineField("Calls", typeof(int), FieldAttributes.Public);
        var callback = builder.DefineField("Callback", typeof(Action), FieldAttributes.Public);
        var constructor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
        var ctorIl = constructor.GetILGenerator();
        ctorIl.Emit(OpCodes.Ldarg_0);
        ctorIl.Emit(OpCodes.Call, baseConstructor);
        ctorIl.Emit(OpCodes.Ret);

        var getter = builder.DefineMethod("get_UniformDimension",
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            baseGetter.ReturnType, Type.EmptyTypes);
        var il = getter.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Dup);
        il.Emit(OpCodes.Ldfld, calls);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Stfld, calls);
        var notFirstCall = il.DefineLabel();
        var hasCallback = il.DefineLabel();
        var afterCallback = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, calls);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Bne_Un_S, notFirstCall);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, callback);
        il.Emit(OpCodes.Dup);
        il.Emit(OpCodes.Brtrue_S, hasCallback);
        il.Emit(OpCodes.Pop);
        il.Emit(OpCodes.Br_S, afterCallback);
        il.MarkLabel(hasCallback);
        il.Emit(OpCodes.Callvirt, typeof(Action).GetMethod(nameof(Action.Invoke))!);
        il.Emit(OpCodes.Br_S, afterCallback);
        il.MarkLabel(notFirstCall);
        il.MarkLabel(afterCallback);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, baseDimension);
        il.Emit(OpCodes.Ret);
        builder.DefineMethodOverride(getter, baseGetter);
        return builder.CreateType()!;
    }

    private static object CountingValue(Side side, string dimension, Action? onFirstCall = null) =>
        CountingValue(side, side.D(dimension), onFirstCall);

    private static object CountingValue(Side side, object dimension, Action? onFirstCall = null)
    {
        object value;
        if (side == Side.Quire)
        {
            value = new QuireCountingDimVal
            {
                m_dim = (global::Quire.Mcd.Dim)dimension,
                OnFirstCall = onFirstCall,
            };
        }
        else
        {
            value = Activator.CreateInstance(MathcadCountingType.Value)
                ?? throw new HarnessException("Could not instantiate emitted Mathcad DimVal counter");
            Set(value, "m_dim", dimension);
            Set(value, "Callback", onFirstCall);
        }
        return value;
    }

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

    private static bool IsCounter(object? value) => value?.GetType().GetField("Calls", BindingFlags.Public | BindingFlags.Instance) != null;

    private static string DescribeValue(Side side, object? value)
    {
        if (value == null) return "null";
        if (IsCounter(value)) return $"counter:{Get(value, "Calls")}:dim={side.DimText(Get(value, "m_dim"))}";
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

    private static string ShowValue(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true) return ShowInput(side, value);
        int rows = (int)Get(value, "Rows")!; int cols = (int)Get(value, "Cols")!;
        var cells = new List<string>();
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
            cells.Add($"({row},{col})={side.DimText(Call(value, "dim", row, col))}");
        return $"{value.GetType().Name}:{rows}x{cols}:uniform={side.DimText(Get(value, "UniformDim"))}:dims=[{string.Join(";", cells)}]";
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
            value => ShowValue(Side.Quire, value), value => ShowValue(Side.Mathcad, value));
    }

    private static (object Q, object M) ColumnUniformWithCounters()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, CountingValue(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 0, CountingValue(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, CountingValue(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 1, 0, CountingValue(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 1, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 1, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 1, 1, Number(Side.Mathcad, "L"));
        return (q, m);
    }

    private static (object Q, object M) RowUniformWithCounters()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, CountingValue(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 0, CountingValue(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 0, 1, CountingValue(Side.Quire, "L"));
        Pointer(Side.Mathcad, m, 0, 1, CountingValue(Side.Mathcad, "L"));
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

    private static (object Q, object M) CountedFirstColumnScalarFactorPair()
    {
        var q = Matrix(Side.Quire); var m = Matrix(Side.Mathcad);
        Pointer(Side.Quire, q, 0, 0, CountingValue(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, CountingValue(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, CountingValue(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, CountingValue(Side.Mathcad, "T"));
        var qLFactor = Call(Side.Quire.Dim, "Mult", Side.Quire.D("L"), Side.Quire.D("v"))!;
        var mLFactor = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("L"), Side.Mathcad.D("v"))!;
        var qTFactor = Call(Side.Quire.Dim, "Mult", Side.Quire.D("T"), Side.Quire.D("v"))!;
        var mTFactor = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("T"), Side.Mathcad.D("v"))!;
        Pointer(Side.Quire, q, 0, 1, SideNumber(Side.Quire, qLFactor)); Pointer(Side.Mathcad, m, 0, 1, SideNumber(Side.Mathcad, mLFactor));
        Pointer(Side.Quire, q, 1, 1, SideNumber(Side.Quire, qTFactor)); Pointer(Side.Mathcad, m, 1, 1, SideNumber(Side.Mathcad, mTFactor));
        return (q, m);
    }

    private static (object Q, object M) ChangingColumnCountFactorPair()
    {
        var q = Matrix(Side.Quire, 2, 3); var m = Matrix(Side.Mathcad, 2, 3);
        Action qShrink = () => SetCols(q, 2);
        Action mShrink = () => SetCols(m, 2);

        Pointer(Side.Quire, q, 0, 0, Number(Side.Quire, "L")); Pointer(Side.Mathcad, m, 0, 0, Number(Side.Mathcad, "L"));
        Pointer(Side.Quire, q, 1, 0, Number(Side.Quire, "T")); Pointer(Side.Mathcad, m, 1, 0, Number(Side.Mathcad, "T"));
        var qLV = Call(Side.Quire.Dim, "Mult", Side.Quire.D("L"), Side.Quire.D("v"))!;
        var mLV = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("L"), Side.Mathcad.D("v"))!;
        var qTV = Call(Side.Quire.Dim, "Mult", Side.Quire.D("T"), Side.Quire.D("v"))!;
        var mTV = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("T"), Side.Mathcad.D("v"))!;
        Pointer(Side.Quire, q, 0, 1, SideNumber(Side.Quire, qLV)); Pointer(Side.Mathcad, m, 0, 1, SideNumber(Side.Mathcad, mLV));
        Pointer(Side.Quire, q, 1, 1, CountingValue(Side.Quire, qTV, qShrink));
        Pointer(Side.Mathcad, m, 1, 1, CountingValue(Side.Mathcad, mTV, mShrink));

        var qLL = Call(Side.Quire.Dim, "Mult", Side.Quire.D("L"), Side.Quire.D("L"))!;
        var mLL = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("L"), Side.Mathcad.D("L"))!;
        var qTL = Call(Side.Quire.Dim, "Mult", Side.Quire.D("T"), Side.Quire.D("L"))!;
        var mTL = Call(Side.Mathcad.Dim, "Mult", Side.Mathcad.D("T"), Side.Mathcad.D("L"))!;
        Pointer(Side.Quire, q, 0, 2, SideNumber(Side.Quire, qLL)); Pointer(Side.Mathcad, m, 0, 2, SideNumber(Side.Mathcad, mLL));
        Pointer(Side.Quire, q, 1, 2, SideNumber(Side.Quire, qTL)); Pointer(Side.Mathcad, m, 1, 2, SideNumber(Side.Mathcad, mTL));
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

    private static void SetCols(object matrix, int cols)
    {
        var data = Get(matrix, "Data")!;
        Set(data, "m_cols", cols);
        Set(matrix, "Data", data);
    }

    private static object SideNumber(Side side, object dimension) => New(side.T("Mcd.NumVal"), 1.0, 0.0, dimension);

    [SkippableFact]
    public void Direct_makeArrayDim_column_loop_uses_original_start_one()
    {
        Runtime.Sync();
        var (q, m) = ColumnUniformWithCounters();
        Compare("makeArrayDim column loop start one avoids repeated virtual UniformDimension query", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_row_loop_uses_original_start_one()
    {
        Runtime.Sync();
        var (q, m) = RowUniformWithCounters();
        Compare("makeArrayDim row loop start one avoids repeated virtual UniformDimension query", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_nonuniform_axes_scalar_factorization_returns_rc_dim()
    {
        Runtime.Sync();
        var (q, m) = NonuniformScalarFactorPair();
        Compare("makeArrayDim nonuniform first axes with proportional columns returns rc_dim dimensions", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_factor_loop_skips_first_column()
    {
        Runtime.Sync();
        var (q, m) = CountedFirstColumnScalarFactorPair();
        Compare("makeArrayDim factor loop starts at column one without re-querying counted first-column dimensions", q, m);
    }

    [SkippableFact]
    public void Direct_makeArrayDim_loop_condition_reads_mutated_parent_column_count()
    {
        Runtime.Sync();
        var (q, m) = ChangingColumnCountFactorPair();
        Compare("makeArrayDim factor loop re-reads parent Cols after first virtual dimension query", q, m);
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
