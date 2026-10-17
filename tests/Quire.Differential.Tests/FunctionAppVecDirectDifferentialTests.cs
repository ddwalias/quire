using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionAppVecDirectDifferentialTests
{
    private const string Contract = "Mpl.Run.Function.Matrix";
    private const int AppVecToken = 0x060001a9;
    private const int CloToken = 0x060001a4;
    private const int ClovecToken = 0x060001a5;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private sealed class FunctionAppVecDirectRecorder
    {
        public int Calls;
        public string Seen = "";
        public object? NextFunction;

        private object Record(params object?[] args)
        {
            Calls++;
            Seen = $"call{Calls}:[{string.Join(",", args.Select(value => value == null ? "null" : Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)))}]";
            return NextFunction ?? Seen;
        }

        public object Capture0() => Record();
        public object Capture1(object a0) => Record(a0);
        public object Capture2(object a0, object a1) => Record(a0, a1);
        public object Capture3(object a0, object a1, object a2) => Record(a0, a1, a2);
        public object Capture4(object a0, object a1, object a2, object a3) => Record(a0, a1, a2, a3);
        public object Capture5(object a0, object a1, object a2, object a3, object a4) => Record(a0, a1, a2, a3, a4);
        public object Capture6(object a0, object a1, object a2, object a3, object a4, object a5) => Record(a0, a1, a2, a3, a4, a5);
        public object Capture7(object a0, object a1, object a2, object a3, object a4, object a5, object a6) => Record(a0, a1, a2, a3, a4, a5, a6);
        public object Capture8(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7) => Record(a0, a1, a2, a3, a4, a5, a6, a7);
        public object Capture9(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8);
        public object Capture10(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
        public object Capture11(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
        public object Capture12(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
        public object Capture13(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
        public object Capture14(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
        public object Capture15(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14) => Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
        public object Capture16(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7,
            object a8, object a9, object a10, object a11, object a12, object a13, object a14, object a15) =>
            Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
        public object Capture17(object a0, object a1, object a2, object a3, object a4, object a5, object a6, object a7,
            object a8, object a9, object a10, object a11, object a12, object a13, object a14, object a15, object a16) =>
            Record(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
        public object Throw1(object value)
        {
            Calls++;
            Seen = $"throw:[{value ?? "null"}]";
            throw new InvalidOperationException("recorded delegate failure");
        }
    }

    private static Type FunctionType(Side side) => side == Side.Quire
        ? typeof(Function)
        : Mathcad.Type("mplrun", "Mpl.Run.Function");

    private static object FunctionValue(Side side, Delegate function, int purity, int? minArgs = null, bool varargs = false)
    {
        var delegateType = side == Side.Quire ? typeof(DelegateFunction) : Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");
        var constructor = delegateType.GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} DelegateFunction constructor");
        object result = constructor.Invoke([function, purity]);
        if (minArgs.HasValue) Set(result, "m_minargs", minArgs.Value);
        if (varargs) Set(result, "m_varargs", true);
        return result;
    }

    private static object ConstantFunctionValue(Side side, object value, int purity = 2, int minArgs = 2, bool varargs = false)
    {
        Type type = side == Side.Quire ? typeof(FnConstant) : Mathcad.Type("mplrun", "Mpl.Run.FnConstant");
        object result = Activator.CreateInstance(type, value)!;
        Set(result, "m_purity", purity);
        Set(result, "m_minargs", minArgs);
        Set(result, "m_varargs", varargs);
        return result;
    }

    private static Type QFDefinition(int genericArity) => genericArity switch
    {
        1 => typeof(F<>),
        2 => typeof(F<,>),
        3 => typeof(F<,,>),
        4 => typeof(F<,,,>),
        5 => typeof(F<,,,,>),
        6 => typeof(F<,,,,,>),
        7 => typeof(F<,,,,,,>),
        8 => typeof(F<,,,,,,,>),
        9 => typeof(F<,,,,,,,,>),
        10 => typeof(F<,,,,,,,,,>),
        11 => typeof(F<,,,,,,,,,,>),
        12 => typeof(F<,,,,,,,,,,,>),
        13 => typeof(F<,,,,,,,,,,,,>),
        14 => typeof(F<,,,,,,,,,,,,,>),
        15 => typeof(F<,,,,,,,,,,,,,,>),
        16 => typeof(F<,,,,,,,,,,,,,,,>),
        17 => typeof(F<,,,,,,,,,,,,,,,,>),
        18 => typeof(F<,,,,,,,,,,,,,,,,,>),
        19 => typeof(F<,,,,,,,,,,,,,,,,,,>),
        _ => throw new ArgumentOutOfRangeException(nameof(genericArity))
    };

    private static Delegate RecorderDelegate(Side side, FunctionAppVecDirectRecorder recorder, int inputCount, string methodName)
    {
        int genericArity = inputCount + 1;
        Type definition = side == Side.Quire
            ? QFDefinition(genericArity)
            : Mathcad.Type("mplrun", $"Mpl.Run.F`{genericArity}");
        Type closed = definition.MakeGenericType(Enumerable.Repeat(typeof(object), genericArity).ToArray());
        MethodInfo method = typeof(FunctionAppVecDirectRecorder).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new HarnessException($"Missing recorder method {methodName}");
        return Delegate.CreateDelegate(closed, recorder, method);
    }

    private static MethodInfo AppVec(Side side)
    {
        var method = FunctionType(side).GetMethods(Instance).Single(candidate => candidate.Name == "appVec" &&
            candidate.IsGenericMethodDefinition && candidate.GetGenericArguments().Length == 1 &&
            candidate.GetParameters().Length == 2 && candidate.GetParameters()[0].IsOut);
        return method.MakeGenericMethod(typeof(object));
    }

    private static MethodInfo Clo(Side side)
    {
        var method = FunctionType(side).GetMethods(Instance).Single(candidate => candidate.Name == "clo" &&
            candidate.IsGenericMethodDefinition && candidate.GetGenericArguments().Length == 1 &&
            candidate.GetParameters().Length == 1);
        return method.MakeGenericMethod(typeof(object));
    }

    private static MethodInfo Clovec(Side side)
    {
        var method = FunctionType(side).GetMethods(Instance).Single(candidate => candidate.Name == "clovec" &&
            candidate.IsGenericMethodDefinition && candidate.GetGenericArguments().Length == 1 &&
            candidate.GetParameters().Length == 1 && candidate.GetParameters()[0].ParameterType.IsArray);
        return method.MakeGenericMethod(typeof(object));
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static FunctionAppVecDirectRecorder? FindRecorder(object? function, HashSet<object>? seen = null)
    {
        if (function == null) return null;
        seen ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
        if (!seen.Add(function)) return null;
        if (FieldOrNull(function, "m_f") is Delegate wrapped && wrapped.Target is FunctionAppVecDirectRecorder recorder) return recorder;
        return FindRecorder(FieldOrNull(function, "m_func"), seen);
    }

    private static string RecorderState(object? function, bool includeNext = true)
    {
        if (function == null) return "null";
        var recorder = FindRecorder(function);
        if (recorder == null) return "no-recorder";
        string next = recorder.NextFunction == null ? "null" : recorder.NextFunction.GetType().Name;
        if (includeNext && recorder.NextFunction != null) next += $"({RecorderState(recorder.NextFunction, includeNext: false)})";
        return $"calls={recorder.Calls}:seen={recorder.Seen}:next={next}";
    }

    private static string State(object? sourceDelegate, object target)
    {
        object? current = FieldOrNull(target, "m_f");
        object? capture = FieldOrNull(target, "m_clo");
        string delegateRelation = sourceDelegate == null ? "n/a" : ReferenceEquals(sourceDelegate, current).ToString();
        return $"type={target.GetType().Name}:purity={FieldOrNull(target, "m_purity")}:minargs={FieldOrNull(target, "m_minargs")}:n={FieldOrNull(target, "m_n")}" +
            $":varargs={FieldOrNull(target, "m_varargs")}:delegate-same={delegateRelation}:capture={MplRunSides.Describe(capture)}:{RecorderState(target)}";
    }

    private static (object? QResult, object? MResult) Compare(string caseId, object q, object m, Delegate? qDelegate, Delegate? mDelegate, object?[]? qArgs, object?[]? mArgs, bool showClosureReceiver = false)
    {
        MethodInfo qMethod = AppVec(Side.Quire), mMethod = AppVec(Side.Mathcad);
        Assert.Equal(AppVecToken, mMethod.MetadataToken);
        Assert.Equal(new Guid("17a0933b-4988-4c18-963d-563998321cc8"), mMethod.Module.ModuleVersionId);
        object?[]? qBefore = qArgs == null ? null : (object?[])qArgs.Clone();
        object?[]? mBefore = mArgs == null ? null : (object?[])mArgs.Clone();
        Func<object?, string> show = value => MplRunSides.Describe(value);
        Func<object?, string> qShow = showClosureReceiver
            ? value => $"{show(value)}:m_func-same-receiver={ReferenceEquals(FieldOrNull(value, "m_func"), q)}"
            : show;
        Func<object?, string> mShow = showClosureReceiver
            ? value => $"{show(value)}:m_func-same-receiver={ReferenceEquals(FieldOrNull(value, "m_func"), m)}"
            : show;
        object?[] qInvokeArgs = [null, qArgs], mInvokeArgs = [null, mArgs];
        MethodComparison.Compare(caseId, Contract, qMethod, q, qInvokeArgs, mMethod, m, mInvokeArgs, qShow, mShow,
            target => State(qDelegate, target), target => State(mDelegate, target));
        Assert.Equal(qBefore, qArgs);
        Assert.Equal(mBefore, mArgs);
        return (qInvokeArgs[0], mInvokeArgs[0]);
    }

    private static object[] Inputs(int count) => Enumerable.Range(0, count).Select(index => (object)$"a{index}").ToArray();

    [SkippableFact]
    public void Clo_and_Clovec_return_closures_wrapping_arguments()
    {
        Runtime.Sync();
        object qFunction = ConstantFunctionValue(Side.Quire, "constant-val");
        object mFunction = ConstantFunctionValue(Side.Mathcad, "constant-val");

        MethodInfo qClo = Clo(Side.Quire), mClo = Clo(Side.Mathcad);
        Assert.Equal(CloToken, mClo.MetadataToken);
        Assert.Equal(new Guid("17a0933b-4988-4c18-963d-563998321cc8"), mClo.Module.ModuleVersionId);
        Func<object?, string> show = value => MplRunSides.Describe(value);
        MethodComparison.Compare("Function.clo wraps receiver and argument in Fn_clo", Contract,
            qClo, qFunction, ["arg1"], mClo, mFunction, ["arg1"],
            show, show,
            target => State(null, target), target => State(null, target));

        MethodInfo qClovec = Clovec(Side.Quire), mClovec = Clovec(Side.Mathcad);
        Assert.Equal(ClovecToken, mClovec.MetadataToken);
        Assert.Equal(new Guid("17a0933b-4988-4c18-963d-563998321cc8"), mClovec.Module.ModuleVersionId);
        MethodComparison.Compare("Function.clovec iteratively wraps arguments in Fn_clo", Contract,
            qClovec, qFunction, [new object[] { "arg1", "arg2" }], mClovec, mFunction, [new object[] { "arg1", "arg2" }],
            show, show,
            target => State(null, target), target => State(null, target));
    }

    [SkippableFact]
    public void AppVec_zero_arguments_dispatches_through_the_default_branch()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 0, nameof(FunctionAppVecDirectRecorder.Capture0));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 0, nameof(FunctionAppVecDirectRecorder.Capture0));
        Compare("Function.appVec count 0 with purity 0 dispatches through invVec", FunctionValue(Side.Quire, qDelegate, 0),
            FunctionValue(Side.Mathcad, mDelegate, 0), qDelegate, mDelegate, [], []);
        Assert.Equal(1, qRecorder.Calls);
        Assert.Equal(1, mRecorder.Calls);
    }

    [SkippableTheory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    public void AppVec_dispatches_each_switch_arity_1_to_16(int count)
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        string name = $"Capture{count}";
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, count, name);
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, count, name);
        Compare($"Function.appVec count {count} dispatches exact overload",
            FunctionValue(Side.Quire, qDelegate, count), FunctionValue(Side.Mathcad, mDelegate, count),
            qDelegate, mDelegate, Inputs(count), Inputs(count));
        Assert.Equal(1, qRecorder.Calls);
        Assert.Equal(1, mRecorder.Calls);
    }

    [SkippableFact]
    public void AppVec_boundary_seventeen_dispatches_exact_vector_arity()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 17, nameof(FunctionAppVecDirectRecorder.Capture17));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 17, nameof(FunctionAppVecDirectRecorder.Capture17));
        Compare("Function.appVec boundary count 17 with exact purity invokes invVec directly",
            FunctionValue(Side.Quire, qDelegate, 17), FunctionValue(Side.Mathcad, mDelegate, 17), qDelegate, mDelegate,
            Inputs(17), Inputs(17));
    }

    [SkippableFact]
    public void AppVec_varargs_precedes_the_arity_switch()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 17, nameof(FunctionAppVecDirectRecorder.Capture17));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 17, nameof(FunctionAppVecDirectRecorder.Capture17));
        Compare("Function.appVec varargs invokes the 17-value delegate before count dispatch",
            FunctionValue(Side.Quire, qDelegate, 16, varargs: true), FunctionValue(Side.Mathcad, mDelegate, 16, varargs: true),
            qDelegate, mDelegate, Inputs(17), Inputs(17));
    }

    [SkippableFact]
    public void AppVec_overapplication_splits_at_purity_and_calls_tail_function()
    {
        Runtime.Sync();
        var qTailRecorder = new FunctionAppVecDirectRecorder(); var mTailRecorder = new FunctionAppVecDirectRecorder();
        var qTailDelegate = RecorderDelegate(Side.Quire, qTailRecorder, 1, nameof(FunctionAppVecDirectRecorder.Capture1));
        var mTailDelegate = RecorderDelegate(Side.Mathcad, mTailRecorder, 1, nameof(FunctionAppVecDirectRecorder.Capture1));
        var qTail = FunctionValue(Side.Quire, qTailDelegate, 1);
        var mTail = FunctionValue(Side.Mathcad, mTailDelegate, 1);
        var qRecorder = new FunctionAppVecDirectRecorder { NextFunction = qTail }; var mRecorder = new FunctionAppVecDirectRecorder { NextFunction = mTail };
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(FunctionAppVecDirectRecorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(FunctionAppVecDirectRecorder.Capture16));
        Compare("Function.appVec count 17 overapplies at purity 16 then invokes the tail function",
            FunctionValue(Side.Quire, qDelegate, 16), FunctionValue(Side.Mathcad, mDelegate, 16), qDelegate, mDelegate,
            Inputs(17), Inputs(17));
        Assert.Equal(1, qTailRecorder.Calls);
        Assert.Equal(1, mTailRecorder.Calls);
    }

    [SkippableFact]
    public void AppVec_overapplication_preserves_multiple_remainder_arguments()
    {
        Runtime.Sync();
        var qTailRecorder = new FunctionAppVecDirectRecorder(); var mTailRecorder = new FunctionAppVecDirectRecorder();
        var qTailDelegate = RecorderDelegate(Side.Quire, qTailRecorder, 2, nameof(FunctionAppVecDirectRecorder.Capture2));
        var mTailDelegate = RecorderDelegate(Side.Mathcad, mTailRecorder, 2, nameof(FunctionAppVecDirectRecorder.Capture2));
        var qTail = FunctionValue(Side.Quire, qTailDelegate, 2);
        var mTail = FunctionValue(Side.Mathcad, mTailDelegate, 2);
        var qRecorder = new FunctionAppVecDirectRecorder { NextFunction = qTail }; var mRecorder = new FunctionAppVecDirectRecorder { NextFunction = mTail };
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(FunctionAppVecDirectRecorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(FunctionAppVecDirectRecorder.Capture16));
        Compare("Function.appVec count 18 overapplies at purity 16 and forwards both tail arguments",
            FunctionValue(Side.Quire, qDelegate, 16), FunctionValue(Side.Mathcad, mDelegate, 16), qDelegate, mDelegate,
            Inputs(18), Inputs(18));
        Assert.Equal("call1:[a16,a17]", qTailRecorder.Seen);
        Assert.Equal("call1:[a16,a17]", mTailRecorder.Seen);
    }

    [SkippableFact]
    public void AppVec_high_arity_optional_padding_appends_optional_argument()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(FunctionAppVecDirectRecorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(FunctionAppVecDirectRecorder.Capture16));
        Compare("Function.appVec count 15 with purity 16 and optional arity pads final delegate argument",
            FunctionValue(Side.Quire, qDelegate, 16, minArgs: 0), FunctionValue(Side.Mathcad, mDelegate, 16, minArgs: 0),
            qDelegate, mDelegate, Inputs(15), Inputs(15));
    }

    [SkippableFact]
    public void AppVec_above_sixteen_optional_padding_pads_array()
    {
        Runtime.Sync();
        object qFunction = ConstantFunctionValue(Side.Quire, "constant-padded", purity: 18, minArgs: 16);
        object mFunction = ConstantFunctionValue(Side.Mathcad, "constant-padded", purity: 18, minArgs: 16);
        Compare("Function.appVec count 17 with purity 18 and minargs 16 pads array to purity",
            qFunction, mFunction, null, null, Inputs(17), Inputs(17));
    }

    [SkippableFact]
    public void AppVec_above_sixteen_below_minargs_returns_clovec()
    {
        Runtime.Sync();
        object qFunction = ConstantFunctionValue(Side.Quire, "constant-clovec", purity: 18, minArgs: 18);
        object mFunction = ConstantFunctionValue(Side.Mathcad, "constant-clovec", purity: 18, minArgs: 18);
        Compare("Function.appVec count 17 with purity 18 and minargs 18 returns clovec",
            qFunction, mFunction, null, null, Inputs(17), Inputs(17));
    }

    [SkippableFact]
    public void AppVec_below_minimum_returns_a_closure_without_calling_delegate()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 2, nameof(FunctionAppVecDirectRecorder.Capture2));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 2, nameof(FunctionAppVecDirectRecorder.Capture2));
        object qFunction = FunctionValue(Side.Quire, qDelegate, 2);
        object mFunction = FunctionValue(Side.Mathcad, mDelegate, 2);
        var (qClosure, mClosure) = Compare("Function.appVec below minimum with one argument returns a delegate closure",
            qFunction, mFunction, qDelegate, mDelegate, ["captured"], ["captured"]);
        Assert.Equal(0, qRecorder.Calls);
        Assert.Equal(0, mRecorder.Calls);
        Compare("returned one-argument closure appVec supplies the missing argument",
            qClosure!, mClosure!, null, null, ["missing"], ["missing"]);
        Assert.Equal("call1:[captured,missing]", qRecorder.Seen);
        Assert.Equal("call1:[captured,missing]", mRecorder.Seen);

        object qConstantFunction = ConstantFunctionValue(Side.Quire, "constant-result");
        object mConstantFunction = ConstantFunctionValue(Side.Mathcad, "constant-result");
        var (qReceiverClosure, mReceiverClosure) = Compare(
            "Function.appVec closure retains the original ordinary Function receiver",
            qConstantFunction, mConstantFunction, null, null, ["captured"], ["captured"], showClosureReceiver: true);
        Assert.Same(qConstantFunction, FieldOrNull(qReceiverClosure, "m_func"));
        Assert.Same(mConstantFunction, FieldOrNull(mReceiverClosure, "m_func"));
    }

    [SkippableFact]
    public void AppVec_null_arguments_raise_before_dispatch()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 1, nameof(FunctionAppVecDirectRecorder.Capture1));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 1, nameof(FunctionAppVecDirectRecorder.Capture1));
        Compare("Function.appVec null args throws while reading vector length", FunctionValue(Side.Quire, qDelegate, 1),
            FunctionValue(Side.Mathcad, mDelegate, 1), qDelegate, mDelegate, null, null);
    }

    [SkippableFact]
    public void AppVec_delegate_exception_preserves_out_slot_and_records_call()
    {
        Runtime.Sync();
        var qRecorder = new FunctionAppVecDirectRecorder(); var mRecorder = new FunctionAppVecDirectRecorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 1, nameof(FunctionAppVecDirectRecorder.Throw1));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 1, nameof(FunctionAppVecDirectRecorder.Throw1));
        Compare("Function.appVec one-argument delegate exception", FunctionValue(Side.Quire, qDelegate, 1),
            FunctionValue(Side.Mathcad, mDelegate, 1), qDelegate, mDelegate, ["throw-me"], ["throw-me"]);
    }
}
