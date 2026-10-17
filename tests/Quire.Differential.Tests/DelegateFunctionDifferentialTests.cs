using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class DelegateFunctionDifferentialTests
{
    private const string Contract = "Mpl.Run.Function";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static Type QType => typeof(DelegateFunction);
    private static Type MType => Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");
    private static Type MDelegatesType => Mathcad.Type("mplrun", "Mpl.Run.Delegates");
    private static Type[] MGtypes => (Type[])MDelegatesType.GetField("gtypes", Static)!.GetValue(null)!;
    private static Type QFDefinition(int typeArgCount) => typeArgCount switch
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
        _ => throw new ArgumentOutOfRangeException(nameof(typeArgCount))
    };

    private sealed record DelegateFunctionSnapshot(string? Name, int Purity, int MinArgs, bool Varargs);

    private static DelegateFunctionSnapshot Snapshot(object target)
    {
        var type = target.GetType();
        string? name = (string?)type.GetProperty("Name", Instance)?.GetValue(target);
        int purity = (int)type.GetProperty("Purity", Instance)!.GetValue(target)!;
        int minArgs = (int)type.GetProperty("MinArgs", Instance)!.GetValue(target)!;
        bool varargs = (bool)type.GetProperty("Varargs", Instance)!.GetValue(target)!;
        return new DelegateFunctionSnapshot(name, purity, minArgs, varargs);
    }

    private static string RenderSnapshot(object target) => JsonSerializer.Serialize(Snapshot(target));
    private static string RenderNameSnapshot(object target)
    {
        for (var t = target.GetType(); t != null; t = t.BaseType)
        {
            var field = t.GetField("m_name", Instance);
            if (field != null)
            {
                string? name = (string?)field.GetValue(target);
                return JsonSerializer.Serialize(new { name });
            }
        }
        return "{}";
    }
    private static string Show(object? value) => MplRunSides.Describe(value);
    private static string ShowAsF(object? value) => (value is Delegate d) ? $"{d.GetType().Name}:{d.Method.Name}" : Show(value);

    public static class DelegateTargets
    {
        public static string M0() => "m0";
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

        public static object Obj1(object a) => a;
        public static object Obj2(object a, object b) => a;
        public static object Obj3(object a, object b, object c) => a;
        public static object Obj4(object a, object b, object c, object d) => a;
    }

    [SkippableFact]
    public void DelegateFunction_inv_overloads_direct_and_fallback_dispatch()
    {
        for (int k = 0; k <= 16; k++)
        {
            var targetMethod = typeof(DelegateTargets).GetMethod($"M{k}", Static)!;

            // Prepare generic type arguments: R = string, A1..Ak = int
            Type[] typeArgs = new Type[k + 1];
            typeArgs[0] = typeof(string);
            for (int i = 1; i <= k; i++) typeArgs[i] = typeof(int);

            // Lookup inv method definition with (k + 1) type parameters and (k + 1) method parameters (out R, A1..Ak)
            var qInvDef = QType.GetMethods(Instance)
                .First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == k + 1 && m.GetParameters().Length == k + 1);
            var mInvDef = MType.GetMethods(Instance)
                .First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == k + 1 && m.GetParameters().Length == k + 1);

            var qInv = qInvDef.MakeGenericMethod(typeArgs);
            var mInv = mInvDef.MakeGenericMethod(typeArgs);

            // Arguments array: [null, 1, 2, ..., k]
            object?[] qArgs = new object[k + 1];
            object?[] mArgs = new object[k + 1];
            for (int i = 1; i <= k; i++)
            {
                qArgs[i] = i * 10;
                mArgs[i] = i * 10;
            }

            // Case A: F<...> delegate (direct branch)
            Type qFType = QFDefinition(k + 1).MakeGenericType(typeArgs);
            Type mFType = MGtypes[k + 1].MakeGenericType(typeArgs);
            Delegate qDelA = Delegate.CreateDelegate(qFType, targetMethod);
            Delegate mDelA = Delegate.CreateDelegate(mFType, targetMethod);

            var qTargetA = Activator.CreateInstance(QType, [qDelA, k])!;
            var mTargetA = Activator.CreateInstance(MType, [mDelA, k])!;

            MethodComparison.Compare($"inv_{k}_direct", Contract,
                qInv, qTargetA, (object?[])qArgs.Clone(),
                mInv, mTargetA, (object?[])mArgs.Clone(),
                Show, Show, RenderSnapshot, RenderSnapshot);

            // Case B: non-F delegate (fallback DynamicInvoke branch)
            Type[] funcTypeArgs = new Type[k + 1];
            for (int i = 0; i < k; i++) funcTypeArgs[i] = typeof(int);
            funcTypeArgs[k] = typeof(string);
            Type funcType = Expression.GetFuncType(funcTypeArgs);
            Delegate delB = Delegate.CreateDelegate(funcType, targetMethod);

            var qTargetB = Activator.CreateInstance(QType, [delB, k])!;
            var mTargetB = Activator.CreateInstance(MType, [delB, k])!;

            MethodComparison.Compare($"inv_{k}_fallback", Contract,
                qInv, qTargetB, (object?[])qArgs.Clone(),
                mInv, mTargetB, (object?[])mArgs.Clone(),
                Show, Show, RenderSnapshot, RenderSnapshot);
        }
    }

    [SkippableFact]
    public void DelegateFunction_invVec_and_app0_and_clo()
    {
        // invVec
        Func<object[], string> vecFunc = args => $"vec:{args?.Length ?? 0}";
        var qTargetVec = Activator.CreateInstance(QType, [vecFunc, 1])!;
        var mTargetVec = Activator.CreateInstance(MType, [vecFunc, 1])!;

        var qInvVecDef = QType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = MType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));

        object?[] vecArgs = [null, new object[] { 1, "two", 3.0 }];
        MethodComparison.Compare("invVec", Contract,
            qInvVec, qTargetVec, (object?[])vecArgs.Clone(),
            mInvVec, mTargetVec, (object?[])vecArgs.Clone(),
            Show, Show, RenderSnapshot, RenderSnapshot);

        // app0 with purity 0 (invokes)
        Func<string> p0Func = () => "zero_res";
        var qTarget0 = Activator.CreateInstance(QType, [p0Func, 0])!;
        var mTarget0 = Activator.CreateInstance(MType, [p0Func, 0])!;
        var qApp0 = QType.GetMethod("app0", Instance)!;
        var mApp0 = MType.GetMethod("app0", Instance)!;
        MethodComparison.Compare("app0_purity0", Contract, qApp0, qTarget0, [], mApp0, mTarget0, [], Show, Show, RenderSnapshot, RenderSnapshot);

        // app0 with purity 1 (returns this)
        var qTarget1 = Activator.CreateInstance(QType, [p0Func, 1])!;
        var mTarget1 = Activator.CreateInstance(MType, [p0Func, 1])!;
        MethodComparison.Compare("app0_purity1", Contract, qApp0, qTarget1, [], mApp0, mTarget1, [], Show, Show, RenderSnapshot, RenderSnapshot);

        // clo<CLO>
        var qCloDef = QType.GetMethods(Instance).First(m => m.Name == "clo" && m.IsGenericMethodDefinition);
        var mCloDef = MType.GetMethods(Instance).First(m => m.Name == "clo" && m.IsGenericMethodDefinition);
        var qClo = qCloDef.MakeGenericMethod(typeof(string));
        var mClo = mCloDef.MakeGenericMethod(typeof(string));
        MethodComparison.Compare("clo", Contract, qClo, qTarget1, ["bound_val"], mClo, mTarget1, ["bound_val"], Show, Show, RenderSnapshot, RenderSnapshot);
    }

    [SkippableFact]
    public void DelegateFunction_remove_module_suffix()
    {
        var qFunc = Activator.CreateInstance(QType, [(Func<string>)DelegateTargets.M0, 0])!;
        var mFunc = Activator.CreateInstance(MType, [(Func<string>)DelegateTargets.M0, 0])!;
        var qMethod = QType.GetMethod("remove_module_suffix", Instance)!;
        var mMethod = MType.GetMethod("remove_module_suffix", Instance)!;

        string[] testNames = ["MathEngineR.dll", "DebugEngineD.dll", "Standard.dll", "NoSuffix", ""];
        foreach (var name in testNames)
        {
            MethodComparison.Compare($"remove_module_suffix_{name}", Contract,
                qMethod, qFunc, [name],
                mMethod, mFunc, [name],
                Show, Show, RenderSnapshot, RenderSnapshot);
        }
    }

    [SkippableFact]
    public void DelegateFunction_construction_and_factories()
    {
        // .ctor(Delegate, int)
        var qCtor = QType.GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)!;
        var mCtor = MType.GetConstructor(Instance, null, [typeof(Delegate), typeof(int)], null)!;
        Func<int, int> fn = x => x * 2;
        MethodComparison.Compare(".ctor", Contract, qCtor, null, [fn, 1], mCtor, null, [fn, 1], Show, Show);

        // makeF overloads (1..4)
        for (int p = 1; p <= 4; p++)
        {
            Type[] typeArgs = Enumerable.Repeat(typeof(object), p + 1).ToArray();
            Type qFType = QFDefinition(p + 1).MakeGenericType(typeArgs);
            Type mFType = MGtypes[p + 1].MakeGenericType(typeArgs);

            var qMakeF = QType.GetMethod("makeF", Static, null, [qFType, typeof(string)], null)!;
            var mMakeF = MType.GetMethod("makeF", Static, null, [mFType, typeof(string)], null)!;

            MethodInfo targetMethod = typeof(DelegateTargets).GetMethod($"Obj{p}", Static)!;
            Delegate qDel = Delegate.CreateDelegate(qFType, targetMethod);
            Delegate mDel = Delegate.CreateDelegate(mFType, targetMethod);

            MethodComparison.Compare($"makeF_{p}", Contract,
                qMakeF, null, [qDel, $"my_func_{p}"],
                mMakeF, null, [mDel, $"my_func_{p}"],
                Show, Show);
        }
    }

    [SkippableFact]
    public void DelegateFunction_make()
    {
        // make(Delegate, int)
        var qMake = QType.GetMethod("make", Static, null, [typeof(Delegate), typeof(int)], null)!;
        var mMake = MType.GetMethod("make", Static, null, [typeof(Delegate), typeof(int)], null)!;

        // Case 1: delegates that convert to F (via toFDelegate)
        Delegate[] matchingDelegates = [
            (Func<object, object>)(a => a),
            (Func<object, object, object>)((a, b) => a),
            (Func<object, object, object, object>)((a, b, c) => a),
            (Func<object, object, object, object, object>)((a, b, c, d) => a)
        ];
        for (int p = 1; p <= 4; p++)
        {
            var del = matchingDelegates[p - 1];
            MethodComparison.Compare($"make_matching_purity_{p}", Contract,
                qMake, null, [del, p],
                mMake, null, [del, p],
                Show, Show);
        }

        // Case 2: non-matching delegates (toFDelegate returns null, falls through to new DelegateFunction)
        var nonMatching = (Func<int, string>)DelegateTargets.M1;
        for (int p = 1; p <= 5; p++)
        {
            MethodComparison.Compare($"make_fallback_purity_{p}", Contract,
                qMake, null, [nonMatching, p],
                mMake, null, [nonMatching, p],
                Show, Show);
        }
    }

    [SkippableFact]
    public void DelegateFunction_AsF_projections()
    {
        for (int p = 1; p <= 4; p++)
        {
            // p=1 -> AsF<A,B>; p=2 -> AsF<A,B,C>; etc.
            Type[] typeArgs = Enumerable.Repeat(typeof(object), p + 1).ToArray();

            var qAsFDef = QType.GetMethods(Instance).First(m => m.Name == "AsF" && m.GetGenericArguments().Length == p + 1);
            var mAsFDef = MType.GetMethods(Instance).First(m => m.Name == "AsF" && m.GetGenericArguments().Length == p + 1);

            var qAsF = qAsFDef.MakeGenericMethod(typeArgs);
            var mAsF = mAsFDef.MakeGenericMethod(typeArgs);

            // Case A: matching F delegate
            Type qFType = QFDefinition(p + 1).MakeGenericType(typeArgs);
            Type mFType = MGtypes[p + 1].MakeGenericType(typeArgs);
            MethodInfo targetMethod = typeof(DelegateTargets).GetMethod($"Obj{p}", Static)!;
            Delegate qDelA = Delegate.CreateDelegate(qFType, targetMethod);
            Delegate mDelA = Delegate.CreateDelegate(mFType, targetMethod);

            var qTargetA = Activator.CreateInstance(QType, [qDelA, p])!;
            var mTargetA = Activator.CreateInstance(MType, [mDelA, p])!;

            MethodComparison.Compare($"AsF_{p}_match", Contract,
                qAsF, qTargetA, [0u],
                mAsF, mTargetA, [0u],
                ShowAsF, ShowAsF, RenderSnapshot, RenderSnapshot);

            Assert.Same(qDelA, qAsF.Invoke(qTargetA, [0u]));
            Assert.Same(mDelA, mAsF.Invoke(mTargetA, [0u]));

            // Case B: non-matching delegate (fallback to base)
            Delegate delB = (Func<string>)DelegateTargets.M0;
            var qTargetB = Activator.CreateInstance(QType, [delB, p])!;
            var mTargetB = Activator.CreateInstance(MType, [delB, p])!;

            MethodComparison.Compare($"AsF_{p}_fallback", Contract,
                qAsF, qTargetB, [1u],
                mAsF, mTargetB, [1u],
                Show, Show, RenderSnapshot, RenderSnapshot);

            Assert.NotSame(delB, qAsF.Invoke(qTargetB, [1u]));
        }
    }

    [SkippableFact]
    public void DelegateFunction_get_Name()
    {
        var qGetName = QType.GetProperty("Name", Instance)!.GetGetMethod(true)!;
        var mGetName = MType.GetProperty("Name", Instance)!.GetGetMethod(true)!;
        var qFunc = Activator.CreateInstance(QType, [(Func<string>)DelegateTargets.M0, 0])!;
        var mFunc = Activator.CreateInstance(MType, [(Func<string>)DelegateTargets.M0, 0])!;
        MethodComparison.Compare("DelegateFunction.get_Name canonical fallback", Contract,
            qGetName, qFunc, [],
            mGetName, mFunc, [],
            Show, Show, RenderNameSnapshot, RenderNameSnapshot);

        var qSetName = typeof(Function).GetProperty("Name", Instance)!.GetSetMethod(true)!;
        var mSetName = Mathcad.Type("mplrun", "Mpl.Run.Function").GetProperty("Name", Instance)!.GetSetMethod(true)!;
        qSetName.Invoke(qFunc, ["explicit_name"]);
        mSetName.Invoke(mFunc, ["explicit_name"]);
        MethodComparison.Compare("DelegateFunction.get_Name explicit value", Contract,
            qGetName, qFunc, [],
            mGetName, mFunc, [],
            Show, Show, RenderNameSnapshot, RenderNameSnapshot);
    }
}
