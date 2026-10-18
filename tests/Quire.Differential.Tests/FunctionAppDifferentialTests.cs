using System.Reflection;
using System.Text.Json;
using Type = System.Type;
using Quire.Mcd;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionAppDifferentialTests
{
    private const string ContractMatrix = "Mpl.Run.Function.Matrix";
    private const string ContractFunction = "Mpl.Run.Function";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FunctionType(Side side) => side == Side.Quire
        ? typeof(Function)
        : Mathcad.Type("mplrun", "Mpl.Run.Function");

    private static Type FnTupleType(Side side) => side == Side.Quire
        ? typeof(Fn_tuple)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_tuple");

    private static Type DelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(DelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");

    private static Type FnVarargsType(Side side) => side == Side.Quire
        ? typeof(Fn_varargs)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_varargs");

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

    private static object MakeFnCloTarget(Side side, Func<object[], object> del, int purity, int minArgs, string? name = null)
    {
        object innerVarargs = MakeVarargsTarget(side, del);
        var fnCloCtor = FnCloType(side).GetConstructor(Instance, null, [FunctionType(side), typeof(object)], null)
            ?? throw new HarnessException($"Missing {side.Name} Fn_clo constructor");
        object fnClo = fnCloCtor.Invoke([innerVarargs, "clo0"])!;
        Set(fnClo, "m_purity", purity);
        Set(fnClo, "m_minargs", minArgs);
        Set(fnClo, "m_varargs", false);
        if (name != null) Set(fnClo, "m_name", name);
        return fnClo;
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record FunctionAppDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        bool NoVectorize,
        string? Name,
        bool HasInverse);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? inv = FieldOrNull(target, "m_inverse_function");
        var snapshot = new FunctionAppDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (bool)(FieldOrNull(target, "m_no_vectorize") ?? false),
            (string?)FieldOrNull(target, "m_name"),
            inv != null);
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Function_app_overloads_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        for (int k = 0; k <= 16; k++)
        {
            var qAppDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);
            var mAppDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);

            Assert.Equal(0x060001aa + k, mAppDef.MetadataToken);
            Assert.Equal(MathcadMvid, mAppDef.Module.ModuleVersionId);

            Type[] typeArgs = new Type[k + 1];
            typeArgs[0] = typeof(string);
            for (int i = 1; i <= k; i++) typeArgs[i] = typeof(int);

            var qApp = qAppDef.MakeGenericMethod(typeArgs);
            var mApp = mAppDef.MakeGenericMethod(typeArgs);

            object?[] qArgs = new object?[k + 1];
            object?[] mArgs = new object?[k + 1];
            for (int i = 1; i <= k; i++)
            {
                qArgs[i] = i * 10;
                mArgs[i] = i * 10;
            }

            MethodComparison.Compare($"Function.app_{k}", ContractMatrix,
                qApp, qTarget, qArgs,
                mApp, mTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }

    [SkippableFact]
    public void Function_app_partial_and_null_application()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        // Case 1: app<R> (0 args) throws null_application when minargs > 0
        object qTarget0 = MakeFnCloTarget(Side.Quire, del, purity: 2, minArgs: 2);
        object mTarget0 = MakeFnCloTarget(Side.Mathcad, del, purity: 2, minArgs: 2);

        var qApp0 = FunctionType(Side.Quire).GetMethods(Instance)
            .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].IsOut)
            .MakeGenericMethod(typeof(string));
        var mApp0 = FunctionType(Side.Mathcad).GetMethods(Instance)
            .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].IsOut)
            .MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Function.app_0_null_app", ContractMatrix,
            qApp0, qTarget0, [null],
            mApp0, mTarget0, [null],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Case 2: app<R> (0 args) with minargs == 0 pads with OptArg and then raises null_application
        object qTarget0Pad = MakeFnCloTarget(Side.Quire, del, purity: 1, minArgs: 0);
        object mTarget0Pad = MakeFnCloTarget(Side.Mathcad, del, purity: 1, minArgs: 0);
        MethodComparison.Compare("Function.app_0_pad_optarg", ContractMatrix,
            qApp0, qTarget0Pad, [null],
            mApp0, mTarget0Pad, [null],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Case 3: Partial application closure creation for all k = 1..16
        object qPartialTarget = MakeFnCloTarget(Side.Quire, del, purity: 20, minArgs: 20);
        object mPartialTarget = MakeFnCloTarget(Side.Mathcad, del, purity: 20, minArgs: 20);

        for (int k = 1; k <= 16; k++)
        {
            var qAppDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);
            var mAppDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);

            Type[] typeArgs = new Type[k + 1];
            typeArgs[0] = typeof(object);
            for (int i = 1; i <= k; i++) typeArgs[i] = typeof(int);

            var qApp = qAppDef.MakeGenericMethod(typeArgs);
            var mApp = mAppDef.MakeGenericMethod(typeArgs);

            object?[] qArgs = new object?[k + 1];
            object?[] mArgs = new object?[k + 1];
            for (int i = 1; i <= k; i++)
            {
                qArgs[i] = i * 10;
                mArgs[i] = i * 10;
            }

            MethodComparison.Compare($"Function.app_partial_{k}", ContractMatrix,
                qApp, qPartialTarget, qArgs,
                mApp, mPartialTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // Case 4: OptArg padding for all k = 1..16
        for (int k = 1; k <= 16; k++)
        {
            object qPadTarget = MakeFnCloTarget(Side.Quire, del, purity: k + 1, minArgs: k);
            object mPadTarget = MakeFnCloTarget(Side.Mathcad, del, purity: k + 1, minArgs: k);

            var qAppDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);
            var mAppDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "app" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);

            Type[] typeArgs = new Type[k + 1];
            typeArgs[0] = typeof(string);
            for (int i = 1; i <= k; i++) typeArgs[i] = typeof(int);

            var qApp = qAppDef.MakeGenericMethod(typeArgs);
            var mApp = mAppDef.MakeGenericMethod(typeArgs);

            object?[] qArgs = new object?[k + 1];
            object?[] mArgs = new object?[k + 1];
            for (int i = 1; i <= k; i++)
            {
                qArgs[i] = i * 10;
                mArgs[i] = i * 10;
            }

            MethodComparison.Compare($"Function.app_pad_{k}", ContractMatrix,
                qApp, qPadTarget, qArgs,
                mApp, mPadTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }

    [SkippableFact]
    public void Function_Call_and_DynCall_zero_args()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "zero";
        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        var qCall = FunctionType(Side.Quire).GetMethod("Call", Instance, Type.EmptyTypes)!;
        var mCall = FunctionType(Side.Mathcad).GetMethod("Call", Instance, Type.EmptyTypes)!;
        Assert.Equal(0x06000171, mCall.MetadataToken);
        Assert.Equal(MathcadMvid, mCall.Module.ModuleVersionId);

        MethodComparison.Compare("Function.Call_zero", ContractFunction,
            qCall, qTarget, [],
            mCall, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        var qDynCall = qTarget.GetType().GetMethod("DynCall", Instance, Type.EmptyTypes)!;
        var mDynCall = mTarget.GetType().GetMethod("DynCall", Instance, Type.EmptyTypes)!;
        Assert.Equal(0x06000160, mDynCall.MetadataToken);
        Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);

        MethodComparison.Compare("Function.DynCall_zero", ContractFunction,
            qDynCall, qTarget, [],
            mDynCall, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_get_Name()
    {
        Runtime.Sync();
        Type qType = FnTupleType(Side.Quire);
        Type mType = FnTupleType(Side.Mathcad);

        var qCtor = qType.GetConstructor(Instance, null, [typeof(object[]), typeof(int)], null)!;
        var mCtor = mType.GetConstructor(Instance, null, [typeof(object[]), typeof(int)], null)!;

        object qTarget = qCtor.Invoke([new object[] { "arg0" }, 1])!;
        object mTarget = mCtor.Invoke([new object[] { "arg0" }, 1])!;

        var qGetName = qTarget.GetType().GetMethod("get_Name", Instance)!;
        var mGetName = mTarget.GetType().GetMethod("get_Name", Instance)!;
        Assert.Equal(0x0600014c, mGetName.MetadataToken);
        Assert.Equal(MathcadMvid, mGetName.Module.ModuleVersionId);

        // Case 1: uninitialized name returns "<anonymous>"
        MethodComparison.Compare("Function.get_Name_anonymous", ContractFunction,
            qGetName, qTarget, [],
            mGetName, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Case 2: custom name returns stored name
        Set(qTarget, "m_name", "custom_fn");
        Set(mTarget, "m_name", "custom_fn");
        MethodComparison.Compare("Function.get_Name_custom", ContractFunction,
            qGetName, qTarget, [],
            mGetName, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }
}

