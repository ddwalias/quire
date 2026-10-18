using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionDynCallCallDifferentialTests
{
    private const string Contract = "Mpl.Run.Function.Matrix";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FunctionType(Side side) => side == Side.Quire
        ? typeof(Function)
        : Mathcad.Type("mplrun", "Mpl.Run.Function");

    private static Type FnVarargsType(Side side) => side == Side.Quire
        ? typeof(Fn_varargs)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_varargs");

    private static Type DelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(DelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");

    private static Type FnCloType(Side side) => (side == Side.Quire
        ? typeof(Fn_clo<>)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_clo`1")).MakeGenericType(typeof(object));

    private static object MakeVarargsTarget(Side side, Func<object[], object> del)
    {
        var delegateType = DelegateFunctionType(side);
        var makeMethod = delegateType.GetMethod("make", Static, [typeof(Delegate), typeof(int)])
            ?? throw new HarnessException($"Missing {side.Name} DelegateFunction.make");
        object innerFunc = makeMethod.Invoke(null, [del, 1])!;

        var varargsCtor = FnVarargsType(side).GetConstructor(Instance, null, [FunctionType(side)], null)
            ?? throw new HarnessException($"Missing {side.Name} Fn_varargs constructor");
        return varargsCtor.Invoke([innerFunc])!;
    }

    private static object MakeFnCloPurityTarget(Side side, Func<object[], object> del, int purity)
    {
        object innerVarargs = MakeVarargsTarget(side, del);
        var fnCloCtor = FnCloType(side).GetConstructor(Instance, null, [FunctionType(side), typeof(object)], null)
            ?? throw new HarnessException($"Missing {side.Name} Fn_clo constructor");
        object fnClo = fnCloCtor.Invoke([innerVarargs, "clo0"])!;
        Set(fnClo, "m_purity", purity);
        Set(fnClo, "m_minargs", purity);
        Set(fnClo, "m_varargs", false);
        return fnClo;
    }

    private sealed record FunctionDynCallCallSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        bool NoVectorize);

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        var snapshot = new FunctionDynCallCallSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (bool)(FieldOrNull(target, "m_no_vectorize") ?? false));
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Function_Call_overloads_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        for (int k = 1; k <= 16; k++)
        {
            var qCall = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "Call" && !m.IsGenericMethod
                    && m.GetParameters().Length == k
                    && m.GetParameters().All(p => p.ParameterType == typeof(object)));
            var mCall = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "Call" && !m.IsGenericMethod
                    && m.GetParameters().Length == k
                    && m.GetParameters().All(p => p.ParameterType == typeof(object)));

            Assert.Equal(0x06000171 + k, mCall.MetadataToken);
            Assert.Equal(MathcadMvid, mCall.Module.ModuleVersionId);

            object?[] qArgs = new object?[k];
            object?[] mArgs = new object?[k];
            for (int i = 0; i < k; i++)
            {
                qArgs[i] = (i + 1) * 10;
                mArgs[i] = (i + 1) * 10;
            }

            MethodComparison.Compare($"Function.Call_{k}", Contract,
                qCall, qTarget, qArgs,
                mCall, mTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }

    [SkippableFact]
    public void Function_DynCall_overloads_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        for (int k = 1; k <= 16; k++)
        {
            object qTarget = MakeFnCloPurityTarget(Side.Quire, del, k);
            object mTarget = MakeFnCloPurityTarget(Side.Mathcad, del, k);

            var qDynCall = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "DynCall" && !m.IsGenericMethod
                    && m.GetParameters().Length == k
                    && m.GetParameters().All(p => p.ParameterType == typeof(object)));
            var mDynCall = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "DynCall" && !m.IsGenericMethod
                    && m.GetParameters().Length == k
                    && m.GetParameters().All(p => p.ParameterType == typeof(object)));

            Assert.Equal(0x06000160 + k, mDynCall.MetadataToken);
            Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);

            object?[] qArgs = new object?[k];
            object?[] mArgs = new object?[k];
            for (int i = 0; i < k; i++)
            {
                qArgs[i] = (i + 1) * 10;
                mArgs[i] = (i + 1) * 10;
            }

            MethodComparison.Compare($"Function.DynCall_{k}", Contract,
                qDynCall, qTarget, qArgs,
                mDynCall, mTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }
}
