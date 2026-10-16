using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionAppVecDirectDifferentialTests
{
    private const string Contract = "Mpl.Run.Function.Matrix";
    private const int AppVecToken = 0x060001a9;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private sealed class Recorder
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

        public object Capture1(object a0) => Record(a0);
        public object Capture0() => Record();
        public object Capture2(object a0, object a1) => Record(a0, a1);
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

    private static object ConstantFunctionValue(Side side, object value)
    {
        Type type = side == Side.Quire ? typeof(FnConstant) : Mathcad.Type("mplrun", "Mpl.Run.FnConstant");
        object result = Activator.CreateInstance(type, value)!;
        Set(result, "m_purity", 2);
        Set(result, "m_minargs", 2);
        Set(result, "m_varargs", false);
        return result;
    }

    private static Delegate RecorderDelegate(Side side, Recorder recorder, int inputCount, string methodName)
    {
        int genericArity = inputCount + 1;
        Type definition = side == Side.Quire
            ? typeof(Function).Assembly.GetType($"Quire.Mpl.Run.F`{genericArity}", throwOnError: true)!
            : Mathcad.Type("mplrun", $"Mpl.Run.F`{genericArity}");
        Type closed = definition.MakeGenericType(Enumerable.Repeat(typeof(object), genericArity).ToArray());
        MethodInfo method = typeof(Recorder).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
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

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static Recorder? FindRecorder(object? function, HashSet<object>? seen = null)
    {
        if (function == null) return null;
        seen ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
        if (!seen.Add(function)) return null;
        if (FieldOrNull(function, "m_f") is Delegate wrapped && wrapped.Target is Recorder recorder) return recorder;
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
    public void AppVec_zero_arguments_dispatches_through_the_default_branch()
    {
        Runtime.Sync();
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 0, nameof(Recorder.Capture0));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 0, nameof(Recorder.Capture0));
        Compare("Function.appVec count 0 with purity 0 dispatches through invVec", FunctionValue(Side.Quire, qDelegate, 0),
            FunctionValue(Side.Mathcad, mDelegate, 0), qDelegate, mDelegate, [], []);
        Assert.Equal(1, qRecorder.Calls);
        Assert.Equal(1, mRecorder.Calls);
    }

    [SkippableFact]
    public void AppVec_boundary_sixteen_dispatches_exact_sixteen_arguments()
    {
        Runtime.Sync();
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(Recorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(Recorder.Capture16));
        Compare("Function.appVec boundary count 16 dispatches the exact high-arity overload",
            FunctionValue(Side.Quire, qDelegate, 16), FunctionValue(Side.Mathcad, mDelegate, 16), qDelegate, mDelegate,
            Inputs(16), Inputs(16));
    }

    [SkippableFact]
    public void AppVec_boundary_seventeen_dispatches_exact_vector_arity()
    {
        Runtime.Sync();
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 17, nameof(Recorder.Capture17));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 17, nameof(Recorder.Capture17));
        Compare("Function.appVec boundary count 17 with exact purity invokes invVec directly",
            FunctionValue(Side.Quire, qDelegate, 17), FunctionValue(Side.Mathcad, mDelegate, 17), qDelegate, mDelegate,
            Inputs(17), Inputs(17));
    }

    [SkippableFact]
    public void AppVec_varargs_precedes_the_arity_switch()
    {
        Runtime.Sync();
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 17, nameof(Recorder.Capture17));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 17, nameof(Recorder.Capture17));
        Compare("Function.appVec varargs invokes the 17-value delegate before count dispatch",
            FunctionValue(Side.Quire, qDelegate, 16, varargs: true), FunctionValue(Side.Mathcad, mDelegate, 16, varargs: true),
            qDelegate, mDelegate, Inputs(17), Inputs(17));
    }

    [SkippableFact]
    public void AppVec_overapplication_splits_at_purity_and_calls_tail_function()
    {
        Runtime.Sync();
        var qTailRecorder = new Recorder(); var mTailRecorder = new Recorder();
        var qTailDelegate = RecorderDelegate(Side.Quire, qTailRecorder, 1, nameof(Recorder.Capture1));
        var mTailDelegate = RecorderDelegate(Side.Mathcad, mTailRecorder, 1, nameof(Recorder.Capture1));
        var qTail = FunctionValue(Side.Quire, qTailDelegate, 1);
        var mTail = FunctionValue(Side.Mathcad, mTailDelegate, 1);
        var qRecorder = new Recorder { NextFunction = qTail }; var mRecorder = new Recorder { NextFunction = mTail };
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(Recorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(Recorder.Capture16));
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
        var qTailRecorder = new Recorder(); var mTailRecorder = new Recorder();
        var qTailDelegate = RecorderDelegate(Side.Quire, qTailRecorder, 2, nameof(Recorder.Capture2));
        var mTailDelegate = RecorderDelegate(Side.Mathcad, mTailRecorder, 2, nameof(Recorder.Capture2));
        var qTail = FunctionValue(Side.Quire, qTailDelegate, 2);
        var mTail = FunctionValue(Side.Mathcad, mTailDelegate, 2);
        var qRecorder = new Recorder { NextFunction = qTail }; var mRecorder = new Recorder { NextFunction = mTail };
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(Recorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(Recorder.Capture16));
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
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 16, nameof(Recorder.Capture16));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 16, nameof(Recorder.Capture16));
        Compare("Function.appVec count 15 with purity 16 and optional arity pads final delegate argument",
            FunctionValue(Side.Quire, qDelegate, 16, minArgs: 0), FunctionValue(Side.Mathcad, mDelegate, 16, minArgs: 0),
            qDelegate, mDelegate, Inputs(15), Inputs(15));
    }

    [SkippableFact]
    public void AppVec_below_minimum_returns_a_closure_without_calling_delegate()
    {
        Runtime.Sync();
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 2, nameof(Recorder.Capture2));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 2, nameof(Recorder.Capture2));
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
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 1, nameof(Recorder.Capture1));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 1, nameof(Recorder.Capture1));
        Compare("Function.appVec null args throws while reading vector length", FunctionValue(Side.Quire, qDelegate, 1),
            FunctionValue(Side.Mathcad, mDelegate, 1), qDelegate, mDelegate, null, null);
    }

    [SkippableFact]
    public void AppVec_delegate_exception_preserves_out_slot_and_records_call()
    {
        Runtime.Sync();
        var qRecorder = new Recorder(); var mRecorder = new Recorder();
        var qDelegate = RecorderDelegate(Side.Quire, qRecorder, 1, nameof(Recorder.Throw1));
        var mDelegate = RecorderDelegate(Side.Mathcad, mRecorder, 1, nameof(Recorder.Throw1));
        Compare("Function.appVec one-argument delegate exception", FunctionValue(Side.Quire, qDelegate, 1),
            FunctionValue(Side.Mathcad, mDelegate, 1), qDelegate, mDelegate, ["throw-me"], ["throw-me"]);
    }
}
