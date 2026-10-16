using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class FunctionClosureDirectDifferentialTests
{
    private const string Contract = "Mpl.Run.Function";
    private const int ClosureToken = 0x060001a3;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private sealed class Recorder
    {
        public int Calls;
        public string Seen = "";

        private object Record(params object?[] args)
        {
            Calls++;
            Seen = string.Join("|", args.Select(value => value?.ToString() ?? "null"));
            return Seen;
        }

        public object P1(object captured) => Record(captured);
        public object P2(object captured, object a) => Record(captured, a);
        public object P3(object captured, object a, object b) => Record(captured, a, b);
        public object P4(object captured, object a, object b, object c) => Record(captured, a, b, c);
        public object P5(object captured, object a, object b, object c, object d) => Record(captured, a, b, c, d);
        public object P6(object captured, object a, object b, object c, object d, object e) => Record(captured, a, b, c, d, e);
    }

    private sealed class Capture(string label)
    {
        public override string ToString() => label;
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static Type FunctionType(Side side) => side == Side.Quire
        ? typeof(Function)
        : Mathcad.Type("mplrun", "Mpl.Run.Function");

    private static MethodInfo Closure(Side side)
    {
        var method = FunctionType(side).GetMethods(Static).Single(candidate => candidate.Name == "closure" &&
            candidate.IsGenericMethodDefinition && candidate.GetGenericArguments().Length == 1 &&
            candidate.GetParameters() is [{ ParameterType: var first }, { }, { ParameterType: var third }] &&
            first == typeof(Delegate) && third == typeof(int));
        return method.MakeGenericMethod(typeof(object));
    }

    private static Delegate TypedDelegate(Side side, Recorder recorder, int purity, out string methodName)
    {
        int remaining = purity - 1;
        methodName = $"P{purity}";
        int genericArity = purity + 1; // return type, captured string, and remaining object arguments
        Type definition = side == Side.Quire
            ? typeof(Function).Assembly.GetType($"Quire.Mpl.Run.F`{genericArity}", throwOnError: true)!
            : Mathcad.Type("mplrun", $"Mpl.Run.F`{genericArity}");
        Type closed = definition.MakeGenericType([typeof(object), typeof(object), .. Enumerable.Repeat(typeof(object), remaining)]);
        MethodInfo callback = typeof(Recorder).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new HarnessException($"Missing callback {methodName}");
        return Delegate.CreateDelegate(closed, recorder, callback);
    }

    private static Delegate FallbackDelegate(Recorder recorder) =>
        (Func<object, object, object>)recorder.P2;

    private static bool HoldsReference(object? value, object expected, HashSet<object>? seen = null, int depth = 0)
    {
        if (ReferenceEquals(value, expected)) return true;
        if (value == null || depth > 16) return false;
        Type type = value.GetType();
        if (type.IsPrimitive || type.IsEnum || value is string || value is Type) return false;
        seen ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
        if (!seen.Add(value)) return false;
        if (value is Delegate callback && HoldsReference(callback.Target, expected, seen, depth + 1)) return true;
        for (Type? current = type; current != null; current = current.BaseType)
        {
            foreach (FieldInfo field in current.GetFields(Instance | BindingFlags.DeclaredOnly))
            {
                try
                {
                    if (HoldsReference(field.GetValue(value), expected, seen, depth + 1)) return true;
                }
                catch (FieldAccessException) { }
            }
        }
        return false;
    }

    private static string FunctionText(object? value)
    {
        if (value == null) return "null";
        return $"{value.GetType().Name}:purity={FieldOrNull(value, "m_purity")}:minargs={FieldOrNull(value, "m_minargs")}" +
            $":varargs={FieldOrNull(value, "m_varargs")}:m_clo={MplRunSides.Describe(FieldOrNull(value, "m_clo"))}";
    }

    private static Recorder? FindRecorder(object? value, HashSet<object>? seen = null, int depth = 0)
    {
        if (value == null || depth > 12) return null;
        if (value is Recorder recorder) return recorder;
        seen ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
        if (!seen.Add(value)) return null;
        if (value is Delegate function)
            return FindRecorder(function.Target, seen, depth + 1);
        Type type = value.GetType();
        if (type.IsPrimitive || type.IsEnum || value is string || value is Type) return null;
        for (Type? current = type; current != null; current = current.BaseType)
        {
            foreach (FieldInfo field in current.GetFields(Instance | BindingFlags.DeclaredOnly))
            {
                object? child;
                try { child = field.GetValue(value); }
                catch { continue; }
                Recorder? found = FindRecorder(child, seen, depth + 1);
                if (found != null) return found;
            }
        }
        return null;
    }

    private static string FunctionState(object target)
    {
        Recorder? recorder = FindRecorder(target);
        return $"{FunctionText(target)}:recorder={recorder?.Calls ?? 0}/{recorder?.Seen ?? "none"}";
    }

    private static MethodInfo Apply(Side side) => FunctionType(side).GetMethod("Apply", Instance, null, [typeof(object[])], null)
        ?? throw new HarnessException($"Missing {side.Name} Function.Apply(object[])");

    private static void CompareClosure(string id, int purity, bool fallback = false)
    {
        Runtime.Sync();
        var qRecorder = new Recorder();
        var mRecorder = new Recorder();
        string methodName = "";
        Delegate qDelegate, mDelegate;
        if (fallback)
        {
            qDelegate = FallbackDelegate(qRecorder);
            mDelegate = FallbackDelegate(mRecorder);
        }
        else
        {
            qDelegate = TypedDelegate(Side.Quire, qRecorder, purity, out methodName);
            mDelegate = TypedDelegate(Side.Mathcad, mRecorder, purity, out _);
        }
        var qCapture = new Capture("captured-value");
        var mCapture = new Capture("captured-value");
        object?[] qFactoryArgs = [qDelegate, qCapture, purity];
        object?[] mFactoryArgs = [mDelegate, mCapture, purity];
        object? qFunction = null, mFunction = null;
        string ShowClosure(Side side, object? value)
        {
            if (value != null && FunctionType(side).IsInstanceOfType(value))
            {
                if (side == Side.Quire) qFunction = value;
                else mFunction = value;
            }
            return FunctionText(value);
        }
        MethodInfo qClosure = Closure(Side.Quire), mClosure = Closure(Side.Mathcad);
        Assert.Equal(ClosureToken, mClosure.MetadataToken);
        MethodComparison.Compare(id, Contract, qClosure, null, qFactoryArgs, mClosure, null, mFactoryArgs,
            value => ShowClosure(Side.Quire, value), value => ShowClosure(Side.Mathcad, value));
        if (qFunction == null) throw new HarnessException("Quire closure result renderer did not capture the result");
        if (mFunction == null) throw new HarnessException("Mathcad closure result renderer did not capture the result");
        Assert.True(HoldsReference(qFunction, qCapture), "Quire closure must retain the supplied captured object by reference");
        Assert.True(HoldsReference(mFunction, mCapture), "Mathcad closure must retain the supplied captured object by reference");

        int remaining = purity - 1;
        object[] qInputs = Enumerable.Range(0, remaining).Select(index => (object)$"arg-{index}").ToArray();
        object[] mInputs = (object[])qInputs.Clone();
        var qApplyArgs = new object?[] { qInputs };
        var mApplyArgs = new object?[] { mInputs };
        MethodInfo qApply = Apply(Side.Quire), mApply = Apply(Side.Mathcad);
        MethodComparison.Compare($"{id} apply remaining arguments", Contract, qApply, qFunction!, qApplyArgs,
            mApply, mFunction!, mApplyArgs, value => MplRunSides.Describe(value), value => MplRunSides.Describe(value),
            FunctionState, FunctionState);
        Assert.Equal(1, qRecorder.Calls);
        Assert.Equal(qRecorder.Seen, mRecorder.Seen);
        Assert.Equal(remaining == 0 ? "captured-value" : "captured-value|" + string.Join("|", qInputs), qRecorder.Seen);
    }

    [SkippableFact]
    public void Function_closure_dispatches_special_purities_default_and_fallback()
    {
        CompareClosure("Function.closure default purity 1 below helper range", 1);
        foreach (int purity in new[] { 2, 3, 4, 5 })
            CompareClosure($"Function.closure typed special purity {purity}", purity);
        CompareClosure("Function.closure default purity 6", 6);
        CompareClosure("Function.closure purity 2 with non-F delegate exercises typed-helper fallback", 2, fallback: true);
    }
}
