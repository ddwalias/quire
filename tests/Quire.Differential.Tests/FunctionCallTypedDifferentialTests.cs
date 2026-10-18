using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionCallTypedDifferentialTests
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

    private sealed record FunctionCallTypedSnapshot(
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
        var snapshot = new FunctionCallTypedSnapshot(
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
    public void Function_Call_typed_generic_overloads_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        for (int k = 1; k <= 16; k++)
        {
            var qCallDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == $"Call_{k}" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k);
            var mCallDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == $"Call_{k}" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k);

            Assert.Equal(0x06000182 + k, mCallDef.MetadataToken);
            Assert.Equal(MathcadMvid, mCallDef.Module.ModuleVersionId);

            Type[] typeArgs = new Type[k + 1];
            typeArgs[0] = typeof(string);
            for (int i = 1; i <= k; i++) typeArgs[i] = typeof(int);

            var qCall = qCallDef.MakeGenericMethod(typeArgs);
            var mCall = mCallDef.MakeGenericMethod(typeArgs);

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
    public void Function_Call_vec_and_Apply1_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        // Call_vec<string>(object[] args)
        var qCallVecDef = FunctionType(Side.Quire).GetMethods(Instance)
            .Single(m => m.Name == "Call_vec" && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(object[]));
        var mCallVecDef = FunctionType(Side.Mathcad).GetMethods(Instance)
            .Single(m => m.Name == "Call_vec" && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(object[]));

        Assert.Equal(0x060001d2, mCallVecDef.MetadataToken);
        Assert.Equal(MathcadMvid, mCallVecDef.Module.ModuleVersionId);

        var qCallVec = qCallVecDef.MakeGenericMethod(typeof(string));
        var mCallVec = mCallVecDef.MakeGenericMethod(typeof(string));

        object[] vecArgs = [10, 20, 30];
        MethodComparison.Compare("Function.Call_vec", Contract,
            qCallVec, qTarget, [vecArgs],
            mCallVec, mTarget, [vecArgs],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Apply1(object)
        var qApplyObj = FunctionType(Side.Quire).GetMethod("Apply1", Instance, [typeof(object)])!;
        var mApplyObj = FunctionType(Side.Mathcad).GetMethod("Apply1", Instance, [typeof(object)])!;
        Assert.Equal(0x060001e7, mApplyObj.MetadataToken);
        MethodComparison.Compare("Function.Apply1_object", Contract,
            qApplyObj, qTarget, ["hello"],
            mApplyObj, mTarget, ["hello"],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Apply1(int)
        var qApplyInt = FunctionType(Side.Quire).GetMethod("Apply1", Instance, [typeof(int)])!;
        var mApplyInt = FunctionType(Side.Mathcad).GetMethod("Apply1", Instance, [typeof(int)])!;
        Assert.Equal(0x060001e8, mApplyInt.MetadataToken);
        MethodComparison.Compare("Function.Apply1_int", Contract,
            qApplyInt, qTarget, [42],
            mApplyInt, mTarget, [42],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Apply1(double)
        var qApplyDbl = FunctionType(Side.Quire).GetMethod("Apply1", Instance, [typeof(double)])!;
        var mApplyDbl = FunctionType(Side.Mathcad).GetMethod("Apply1", Instance, [typeof(double)])!;
        Assert.Equal(0x060001e9, mApplyDbl.MetadataToken);
        MethodComparison.Compare("Function.Apply1_double", Contract,
            qApplyDbl, qTarget, [3.14],
            mApplyDbl, mTarget, [3.14],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_AsDelegate_and_AsF_direct_projections()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        // AsDelegate_1(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 1);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 1);
            var qM = FunctionType(Side.Quire).GetMethod("AsDelegate_1", Instance, [typeof(uint)])!;
            var mM = FunctionType(Side.Mathcad).GetMethod("AsDelegate_1", Instance, [typeof(uint)])!;
            Assert.Equal(0x060001e1, mM.MetadataToken);
            MethodComparison.Compare("Function.AsDelegate_1_uint", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsDelegate_2(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 2);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 2);
            var qM = FunctionType(Side.Quire).GetMethod("AsDelegate_2", Instance, [typeof(uint)])!;
            var mM = FunctionType(Side.Mathcad).GetMethod("AsDelegate_2", Instance, [typeof(uint)])!;
            Assert.Equal(0x060001e2, mM.MetadataToken);
            MethodComparison.Compare("Function.AsDelegate_2_uint", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsDelegate_3()
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 3);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 3);
            var qM = FunctionType(Side.Quire).GetMethod("AsDelegate_3", Instance, Type.EmptyTypes)!;
            var mM = FunctionType(Side.Mathcad).GetMethod("AsDelegate_3", Instance, Type.EmptyTypes)!;
            Assert.Equal(0x060001df, mM.MetadataToken);
            MethodComparison.Compare("Function.AsDelegate_3", Contract,
                qM, qT, [],
                mM, mT, [],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // Failure case on purity 2 (kills 0u errpos mutant)
            object qFail = MakeFnCloPurityTarget(Side.Quire, del, 2);
            object mFail = MakeFnCloPurityTarget(Side.Mathcad, del, 2);
            MethodComparison.Compare("Function.AsDelegate_3_too_few_args", Contract,
                qM, qFail, [],
                mM, mFail, [],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsDelegate_3(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 3);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 3);
            var qM = FunctionType(Side.Quire).GetMethod("AsDelegate_3", Instance, [typeof(uint)])!;
            var mM = FunctionType(Side.Mathcad).GetMethod("AsDelegate_3", Instance, [typeof(uint)])!;
            Assert.Equal(0x060001e3, mM.MetadataToken);
            MethodComparison.Compare("Function.AsDelegate_3_uint", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsDelegate_4()
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 4);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 4);
            var qM = FunctionType(Side.Quire).GetMethod("AsDelegate_4", Instance, Type.EmptyTypes)!;
            var mM = FunctionType(Side.Mathcad).GetMethod("AsDelegate_4", Instance, Type.EmptyTypes)!;
            Assert.Equal(0x060001e0, mM.MetadataToken);
            MethodComparison.Compare("Function.AsDelegate_4", Contract,
                qM, qT, [],
                mM, mT, [],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // Failure case on purity 2 (kills 0u errpos mutant)
            object qFail = MakeFnCloPurityTarget(Side.Quire, del, 2);
            object mFail = MakeFnCloPurityTarget(Side.Mathcad, del, 2);
            MethodComparison.Compare("Function.AsDelegate_4_too_few_args", Contract,
                qM, qFail, [],
                mM, mFail, [],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsDelegate_4(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 4);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 4);
            var qM = FunctionType(Side.Quire).GetMethod("AsDelegate_4", Instance, [typeof(uint)])!;
            var mM = FunctionType(Side.Mathcad).GetMethod("AsDelegate_4", Instance, [typeof(uint)])!;
            Assert.Equal(0x060001e4, mM.MetadataToken);
            MethodComparison.Compare("Function.AsDelegate_4_uint", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsF<R, A>(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 1);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 1);
            var qMDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);
            var mMDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);
            Assert.Equal(0x060001d9, mMDef.MetadataToken);
            var qM = qMDef.MakeGenericMethod(typeof(object), typeof(object));
            var mM = mMDef.MakeGenericMethod(typeof(object), typeof(object));
            MethodComparison.Compare("Function.AsF_2", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsF<R, A, B>(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 2);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 2);
            var qMDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 3);
            var mMDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 3);
            Assert.Equal(0x060001da, mMDef.MetadataToken);
            var qM = qMDef.MakeGenericMethod(typeof(object), typeof(object), typeof(object));
            var mM = mMDef.MakeGenericMethod(typeof(object), typeof(object), typeof(object));
            MethodComparison.Compare("Function.AsF_3", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsF<R, A, B, C>(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 3);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 3);
            var qMDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 4);
            var mMDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 4);
            Assert.Equal(0x060001db, mMDef.MetadataToken);
            var qM = qMDef.MakeGenericMethod(typeof(object), typeof(object), typeof(object), typeof(object));
            var mM = mMDef.MakeGenericMethod(typeof(object), typeof(object), typeof(object), typeof(object));
            MethodComparison.Compare("Function.AsF_4", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // AsF<R, A, B, C, D>(uint)
        {
            object qT = MakeFnCloPurityTarget(Side.Quire, del, 4);
            object mT = MakeFnCloPurityTarget(Side.Mathcad, del, 4);
            var qMDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 5);
            var mMDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "AsF" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 5);
            Assert.Equal(0x060001dc, mMDef.MetadataToken);
            var qM = qMDef.MakeGenericMethod(typeof(object), typeof(object), typeof(object), typeof(object), typeof(object));
            var mM = mMDef.MakeGenericMethod(typeof(object), typeof(object), typeof(object), typeof(object), typeof(object));
            MethodComparison.Compare("Function.AsF_5", Contract,
                qM, qT, [10u],
                mM, mT, [10u],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }
}
