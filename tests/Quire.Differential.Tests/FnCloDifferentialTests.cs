using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnCloDifferentialTests
{
    private const string Contract = "Mpl.Run.Fn_clo";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type GenericFnCloType(Side side) => side == Side.Quire
        ? typeof(Fn_clo<>)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_clo`1");

    private static Type ClosedFnCloType(Side side, Type cloType) =>
        GenericFnCloType(side).MakeGenericType(cloType);

    private static Type DelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(DelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");

    private static object MakeDelegateFunction(Side side, Delegate d, int purity)
    {
        var method = DelegateFunctionType(side).GetMethod("make", Static, [typeof(Delegate), typeof(int)])
            ?? throw new HarnessException($"Missing {side.Name} DelegateFunction.make");
        return method.Invoke(null, [d, purity])!;
    }

    private static object ConstantFunction(Side side, object value, int purity = 2, int minArgs = 2)
    {
        Type type = side == Side.Quire ? typeof(FnConstant) : Mathcad.Type("mplrun", "Mpl.Run.FnConstant");
        object result = Activator.CreateInstance(type, value)!;
        Set(result, "m_purity", purity);
        Set(result, "m_minargs", minArgs);
        return result;
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record FnCloDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        string? FuncType,
        string? CloValue);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? clo = FieldOrNull(target, "m_clo");
        string cloDesc = ReferenceEquals(clo, target) ? "self" : MplRunSides.Describe(clo);
        var snapshot = new FnCloDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            FieldOrNull(target, "m_func")?.GetType().Name,
            cloDesc);
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    private static class FnCloDelegateTargets
    {
        public static string M1(int a) => $"m1({a})";
        public static string M2(int a, int b) => $"m2({a},{b})";
        public static string M3(int a, int b, int c) => $"m3({a},{b},{c})";
        public static string M4(int a, int b, int c, int d) => $"m4({a},{b},{c},{d})";
        public static string M5(int a, int b, int c, int d, int e) => $"m5({a},{b},{c},{d},{e})";
        public static string M6(int a, int b, int c, int d, int e, int f) => $"m6({a},{b},{c},{d},{e},{f})";
        public static string M7(int a, int b, int c, int d, int e, int f, int g) => $"m7({a},{b},{c},{d},{e},{f},{g})";
        public static string M8(int a, int b, int c, int d, int e, int f, int g, int h) => $"m8({a},{b},{c},{d},{e},{f},{g},{h})";
        public static string M9(int a, int b, int c, int d, int e, int f, int g, int h, int i) => $"m9({a},{b},{c},{d},{e},{f},{g},{h},{i})";
        public static string M10(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j) => $"m10({a},{b},{c},{d},{e},{f},{g},{h},{i},{j})";
        public static string M11(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k) => $"m11({a},{b},{c},{d},{e},{f},{g},{h},{i},{j},{k})";
        public static string M12(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l) => $"m12({a},{b},{c},{d},{e},{f},{g},{h},{i},{j},{k},{l})";
        public static string M13(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m) => $"m13({a},{b},{c},{d},{e},{f},{g},{h},{i},{j},{k},{l},{m})";
        public static string M14(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n) => $"m14({a},{b},{c},{d},{e},{f},{g},{h},{i},{j},{k},{l},{m},{n})";
        public static string M15(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o) => $"m15({a},{b},{c},{d},{e},{f},{g},{h},{i},{j},{k},{l},{m},{n},{o})";
        public static string M16(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p) => $"m16({a},{b},{c},{d},{e},{f},{g},{h},{i},{j},{k},{l},{m},{n},{o},{p})";
    }

    [SkippableFact]
    public void FnClo_constructors()
    {
        Runtime.Sync();
        Func<int, string> del1 = x => $"del({x})";
        Type qClosedInt = ClosedFnCloType(Side.Quire, typeof(int));
        Type mClosedInt = ClosedFnCloType(Side.Mathcad, typeof(int));

        // .ctor(Delegate, CLO, int) -> 0x06000223
        var qCtorDel = qClosedInt.GetConstructor(Instance, null, [typeof(Delegate), typeof(int), typeof(int)], null)!;
        var mCtorDel = mClosedInt.GetConstructor(Instance, null, [typeof(Delegate), typeof(int), typeof(int)], null)!;
        Assert.Equal(0x06000223, mCtorDel.MetadataToken);
        Assert.Equal(MathcadMvid, mCtorDel.Module.ModuleVersionId);

        MethodComparison.Compare("Fn_clo.ctor_delegate", Contract,
            qCtorDel, null, [del1, 42, 2],
            mCtorDel, null, [del1, 42, 2],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));

        // .ctor(Function, CLO) -> 0x06000224
        Type qFuncType = typeof(Function);
        Type mFuncType = Mathcad.Type("mplrun", "Mpl.Run.Function");
        var qCtorFunc = qClosedInt.GetConstructor(Instance, null, [qFuncType, typeof(int)], null)!;
        var mCtorFunc = mClosedInt.GetConstructor(Instance, null, [mFuncType, typeof(int)], null)!;
        Assert.Equal(0x06000224, mCtorFunc.MetadataToken);
        Assert.Equal(MathcadMvid, mCtorFunc.Module.ModuleVersionId);

        object qF = ConstantFunction(Side.Quire, "c", purity: 3, minArgs: 2);
        object mF = ConstantFunction(Side.Mathcad, "c", purity: 3, minArgs: 2);

        MethodComparison.Compare("Fn_clo.ctor_function", Contract,
            qCtorFunc, null, [qF, 99],
            mCtorFunc, null, [mF, 99],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void FnClo_Fixpoint()
    {
        Runtime.Sync();
        Type qType = typeof(Fn_clo<object>);
        Type mType = ClosedFnCloType(Side.Mathcad, typeof(object));

        var qFixpoint = qType.GetMethod("Fixpoint", Static, [typeof(Function)])!;
        var mFixpoint = mType.GetMethod("Fixpoint", Static, [Mathcad.Type("mplrun", "Mpl.Run.Function")])!;
        Assert.Equal(0x06000225, mFixpoint.MetadataToken);
        Assert.Equal(MathcadMvid, mFixpoint.Module.ModuleVersionId);

        object qF = ConstantFunction(Side.Quire, "rec", purity: 2);
        object mF = ConstantFunction(Side.Mathcad, "rec", purity: 2);

        MethodComparison.Compare("Fn_clo.Fixpoint", Contract,
            qFixpoint, null, [qF],
            mFixpoint, null, [mF],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void FnClo_Name()
    {
        Runtime.Sync();
        Type qTypeStr = ClosedFnCloType(Side.Quire, typeof(string));
        Type mTypeStr = ClosedFnCloType(Side.Mathcad, typeof(string));

        var qName = qTypeStr.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = mTypeStr.GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x06000226, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);

        object qF1 = ConstantFunction(Side.Quire, "named_fn", purity: 2);
        object mF1 = ConstantFunction(Side.Mathcad, "named_fn", purity: 2);
        Set(qF1, "m_name", "TestFunc");
        Set(mF1, "m_name", "TestFunc");

        object qTarget1 = Activator.CreateInstance(qTypeStr, [qF1, "my_arg"])!;
        object mTarget1 = Activator.CreateInstance(mTypeStr, [mF1, "my_arg"])!;

        MethodComparison.Compare("Fn_clo.Name_string_clo", Contract,
            qName, qTarget1, [],
            mName, mTarget1, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        Type qTypeInt = ClosedFnCloType(Side.Quire, typeof(int));
        Type mTypeInt = ClosedFnCloType(Side.Mathcad, typeof(int));
        var qNameInt = qTypeInt.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mNameInt = mTypeInt.GetProperty("Name", Instance)!.GetGetMethod()!;
        object qTarget2 = Activator.CreateInstance(qTypeInt, [qF1, 12345])!;
        object mTarget2 = Activator.CreateInstance(mTypeInt, [mF1, 12345])!;

        MethodComparison.Compare("Fn_clo.Name_int_clo", Contract,
            qNameInt, qTarget2, [],
            mNameInt, mTarget2, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnClo_invVec()
    {
        Runtime.Sync();
        Type qTypeInt = ClosedFnCloType(Side.Quire, typeof(int));
        Type mTypeInt = ClosedFnCloType(Side.Mathcad, typeof(int));

        var qInvVecDef = qTypeInt.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mTypeInt.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));
        Assert.Equal(0x06000227, mInvVec.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVec.Module.ModuleVersionId);

        // Case 1: 2-arg delegate (M2), clo=10, args=[20] -> "m2(10,20)"
        MethodInfo m2Method = typeof(FnCloDelegateTargets).GetMethod("M2", Static)!;
        Delegate del2 = Delegate.CreateDelegate(typeof(Func<int, int, string>), m2Method);
        object qFn2 = MakeDelegateFunction(Side.Quire, del2, 2);
        object mFn2 = MakeDelegateFunction(Side.Mathcad, del2, 2);
        object qTarget2 = Activator.CreateInstance(qTypeInt, [qFn2, 10])!;
        object mTarget2 = Activator.CreateInstance(mTypeInt, [mFn2, 10])!;

        object?[] qArgs2 = [null, new object[] { 20 }];
        object?[] mArgs2 = [null, new object[] { 20 }];

        MethodComparison.Compare("Fn_clo.invVec_arity2", Contract,
            qInvVec, qTarget2, (object?[])qArgs2.Clone(),
            mInvVec, mTarget2, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // Case 2: 3-arg delegate (M3), clo=1, args=[2, 3] -> "m3(1,2,3)"
        MethodInfo m3Method = typeof(FnCloDelegateTargets).GetMethod("M3", Static)!;
        Delegate del3 = Delegate.CreateDelegate(typeof(Func<int, int, int, string>), m3Method);
        object qFn3 = MakeDelegateFunction(Side.Quire, del3, 3);
        object mFn3 = MakeDelegateFunction(Side.Mathcad, del3, 3);
        object qTarget3 = Activator.CreateInstance(qTypeInt, [qFn3, 1])!;
        object mTarget3 = Activator.CreateInstance(mTypeInt, [mFn3, 1])!;

        object?[] qArgs3 = [null, new object[] { 2, 3 }];
        object?[] mArgs3 = [null, new object[] { 2, 3 }];

        MethodComparison.Compare("Fn_clo.invVec_arity3", Contract,
            qInvVec, qTarget3, (object?[])qArgs3.Clone(),
            mInvVec, mTarget3, (object?[])mArgs3.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnClo_inv_overloads()
    {
        Runtime.Sync();
        Type qTypeInt = ClosedFnCloType(Side.Quire, typeof(int));
        Type mTypeInt = ClosedFnCloType(Side.Mathcad, typeof(int));

        for (int k = 0; k < 16; k++)
        {
            int arity = k + 1; // total delegate arguments
            MethodInfo targetMethod = typeof(FnCloDelegateTargets).GetMethod($"M{arity}", Static)!;

            Type[] funcTypeArgs = new Type[arity + 1];
            for (int i = 0; i < arity; i++) funcTypeArgs[i] = typeof(int);
            funcTypeArgs[arity] = typeof(string);
            Type funcType = Expression.GetFuncType(funcTypeArgs);
            Delegate del = Delegate.CreateDelegate(funcType, targetMethod);

            object qFn = MakeDelegateFunction(Side.Quire, del, arity);
            object mFn = MakeDelegateFunction(Side.Mathcad, del, arity);

            object qTarget = Activator.CreateInstance(qTypeInt, [qFn, 10])!;
            object mTarget = Activator.CreateInstance(mTypeInt, [mFn, 10])!;

            // Prepare inv method generic args: R = string, A1..Ak = int
            Type[] invGenericArgs = new Type[k + 1];
            invGenericArgs[0] = typeof(string);
            for (int i = 1; i <= k; i++) invGenericArgs[i] = typeof(int);

            var qInvDef = qTypeInt.GetMethods(Instance)
                .First(m => m.Name == "inv" && m.IsGenericMethodDefinition &&
                            m.GetGenericArguments().Length == k + 1 &&
                            m.GetParameters().Length == k + 1);
            var mInvDef = mTypeInt.GetMethods(Instance)
                .First(m => m.Name == "inv" && m.IsGenericMethodDefinition &&
                            m.GetGenericArguments().Length == k + 1 &&
                            m.GetParameters().Length == k + 1);

            int expectedToken = 0x06000228 + k;
            Assert.Equal(expectedToken, mInvDef.MetadataToken);
            Assert.Equal(MathcadMvid, mInvDef.Module.ModuleVersionId);

            var qInv = qInvDef.MakeGenericMethod(invGenericArgs);
            var mInv = mInvDef.MakeGenericMethod(invGenericArgs);

            // Method arguments: [out R (null), int arg1, int arg2, ...]
            object?[] qArgs = new object?[k + 1];
            object?[] mArgs = new object?[k + 1];
            for (int i = 1; i <= k; i++)
            {
                qArgs[i] = (i + 1) * 10;
                mArgs[i] = (i + 1) * 10;
            }

            MethodComparison.Compare($"Fn_clo.inv_overload_{k}", Contract,
                qInv, qTarget, (object?[])qArgs.Clone(),
                mInv, mTarget, (object?[])mArgs.Clone(),
                quireShow: Show, originalShow: Show,
                quireState: t => RenderSnapshot(Side.Quire, t),
                originalState: t => RenderSnapshot(Side.Mathcad, t));
        }
    }
}
