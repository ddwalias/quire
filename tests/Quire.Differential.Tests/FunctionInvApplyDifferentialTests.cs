using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionInvApplyDifferentialTests
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

    private sealed record FunctionInvApplySnapshot(
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
        var snapshot = new FunctionInvApplySnapshot(
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
    public void Function_inv_overloads_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        for (int k = 0; k <= 16; k++)
        {
            var qInvDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "inv" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);
            var mInvDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "inv" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k + 1
                    && m.GetParameters().Length == k + 1
                    && m.GetParameters()[0].IsOut);

            Assert.Equal(0x060001bc + k, mInvDef.MetadataToken);
            Assert.Equal(MathcadMvid, mInvDef.Module.ModuleVersionId);

            Type[] typeArgs = new Type[k + 1];
            typeArgs[0] = typeof(string);
            for (int i = 1; i <= k; i++) typeArgs[i] = typeof(int);

            var qInv = qInvDef.MakeGenericMethod(typeArgs);
            var mInv = mInvDef.MakeGenericMethod(typeArgs);

            object?[] qArgs = new object?[k + 1];
            object?[] mArgs = new object?[k + 1];
            for (int i = 1; i <= k; i++)
            {
                qArgs[i] = i * 10;
                mArgs[i] = i * 10;
            }

            MethodComparison.Compare($"Function.inv_{k}", Contract,
                qInv, qTarget, qArgs,
                mInv, mTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }

    [SkippableFact]
    public void Function_apply_overloads_direct_dispatch()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr.Select(x => x?.ToString() ?? "null"));

        object qTarget = MakeVarargsTarget(Side.Quire, del);
        object mTarget = MakeVarargsTarget(Side.Mathcad, del);

        for (int k = 1; k <= 16; k++)
        {
            var qApplyDef = FunctionType(Side.Quire).GetMethods(Instance)
                .Single(m => m.Name == "apply" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k
                    && m.GetParameters().Length == k);
            var mApplyDef = FunctionType(Side.Mathcad).GetMethods(Instance)
                .Single(m => m.Name == "apply" && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == k
                    && m.GetParameters().Length == k);

            Assert.Equal(0x06000193 + (k - 1), mApplyDef.MetadataToken);
            Assert.Equal(MathcadMvid, mApplyDef.Module.ModuleVersionId);

            Type[] typeArgs = new Type[k];
            for (int i = 0; i < k; i++) typeArgs[i] = typeof(int);

            var qApply = qApplyDef.MakeGenericMethod(typeArgs);
            var mApply = mApplyDef.MakeGenericMethod(typeArgs);

            object?[] qArgs = new object?[k];
            object?[] mArgs = new object?[k];
            for (int i = 0; i < k; i++)
            {
                qArgs[i] = (i + 1) * 10;
                mArgs[i] = (i + 1) * 10;
            }

            MethodComparison.Compare($"Function.apply_{k}", Contract,
                qApply, qTarget, qArgs,
                mApply, mTarget, mArgs,
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }
}
