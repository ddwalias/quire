using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DelegateSubclassesDifferentialTests
{
    private const string Contract = "Mpl.Run.Function";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FunctionType(Side side) => side == Side.Quire
        ? typeof(Function)
        : Mathcad.Type("mplrun", "Mpl.Run.Function");

    private static Type DelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(DelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");

    private static Type UnaryDelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(UnaryDelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.UnaryDelegateFunction");

    private static Type BinaryDelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(BinaryDelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.BinaryDelegateFunction");

    private static Type TernaryDelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(TernaryDelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.TernaryDelegateFunction");

    private static Type QuartnaryDelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(QuartnaryDelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.QuartnaryDelegateFunction");

    private static Type PremiumFunctionType(Side side) => side == Side.Quire
        ? typeof(PremiumFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.PremiumFunction");

    private static Type FnVarargsType(Side side) => side == Side.Quire
        ? typeof(Fn_varargs)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_varargs");

    private static Type FnCloType(Side side) => (side == Side.Quire
        ? typeof(Fn_clo<>)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_clo`1")).MakeGenericType(typeof(object));

    private static Type MDelegatesType => Mathcad.Type("mplrun", "Mpl.Run.Delegates");
    private static Type[] MGtypes => (Type[])MDelegatesType.GetField("gtypes", Static)!.GetValue(null)!;

    public static object Target1(object a) => $"u({a})";
    public static object Target2(object a, object b) => $"b({a},{b})";
    public static object Target3(object a, object b, object c) => $"t({a},{b},{c})";
    public static object Target4(object a, object b, object c, object d) => $"q({a},{b},{c},{d})";

    private static Delegate CreateFDelegate(Side side, int arity)
    {
        MethodInfo mi = typeof(DelegateSubclassesDifferentialTests).GetMethod($"Target{arity}", Static)
            ?? throw new HarnessException($"Missing Target{arity}");
        if (side == Side.Quire)
        {
            Type qFType = arity switch
            {
                1 => typeof(F<object, object>),
                2 => typeof(F<object, object, object>),
                3 => typeof(F<object, object, object, object>),
                4 => typeof(F<object, object, object, object, object>),
                _ => throw new ArgumentOutOfRangeException(nameof(arity))
            };
            return Delegate.CreateDelegate(qFType, mi);
        }
        else
        {
            Type[] typeArgs = new Type[arity + 1];
            Array.Fill(typeArgs, typeof(object));
            Type mFType = MGtypes[arity + 1].MakeGenericType(typeArgs);
            return Delegate.CreateDelegate(mFType, mi);
        }
    }

    private static object MakeTarget(Side side, int arity, int minArgs = 0, bool noVectorize = false, string? name = null)
    {
        Delegate del = CreateFDelegate(side, arity);
        Type type = arity switch
        {
            1 => UnaryDelegateFunctionType(side),
            2 => BinaryDelegateFunctionType(side),
            3 => TernaryDelegateFunctionType(side),
            4 => QuartnaryDelegateFunctionType(side),
            _ => throw new ArgumentOutOfRangeException(nameof(arity))
        };
        object target = Activator.CreateInstance(type, [del])!;
        if (minArgs != 0) Set(target, "m_minargs", minArgs);
        if (noVectorize) Set(target, "m_no_vectorize", true);
        if (name != null) Set(target, "m_name", name);
        return target;
    }

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

    private static object MakeFnCloPurityTarget(Side side, Func<object[], object> del, int purity, string? name = null)
    {
        object innerVarargs = MakeVarargsTarget(side, del);
        var fnCloCtor = FnCloType(side).GetConstructor(Instance, null, [FunctionType(side), typeof(object)], null)
            ?? throw new HarnessException($"Missing {side.Name} Fn_clo constructor");
        object fnClo = fnCloCtor.Invoke([innerVarargs, "clo0"])!;
        Set(fnClo, "m_purity", purity);
        Set(fnClo, "m_minargs", purity);
        Set(fnClo, "m_varargs", false);
        if (name != null) Set(fnClo, "m_name", name);
        return fnClo;
    }

    private static object MakePremiumTarget(Side side, Func<object[], object> del, int purity, string? name = null)
    {
        object inner = MakeFnCloPurityTarget(side, del, purity, name);
        Type pType = PremiumFunctionType(side);
        var ctor = pType.GetConstructor(Instance, null, [FunctionType(side)], null)
            ?? throw new HarnessException($"Missing {side.Name} PremiumFunction constructor");
        return ctor.Invoke([inner])!;
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record DelegateSubclassesSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        bool NoVectorize,
        string? Name,
        bool HasDelegate,
        bool HasInnerFunc);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? innerFunc = FieldOrNull(target, "m_func");
        object? del = FieldOrNull(target, "m_F");
        var snapshot = new DelegateSubclassesSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (bool)(FieldOrNull(target, "m_no_vectorize") ?? false),
            (string?)FieldOrNull(target, "m_name"),
            del != null,
            innerFunc != null);
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Specialized_DelegateFunction_constructors()
    {
        Runtime.Sync();
        // 1. Unary .ctor (0x0600020f)
        {
            var qType = UnaryDelegateFunctionType(Side.Quire);
            var mType = UnaryDelegateFunctionType(Side.Mathcad);
            var qDel = CreateFDelegate(Side.Quire, 1);
            var mDel = CreateFDelegate(Side.Mathcad, 1);
            var qCtor = qType.GetConstructor(Instance, null, [qDel.GetType()], null)!;
            var mCtor = mType.GetConstructor(Instance, null, [mDel.GetType()], null)!;
            Assert.Equal(0x0600020f, mCtor.MetadataToken);
            Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);
            MethodComparison.Compare("UnaryDelegateFunction.ctor", Contract,
                qCtor, null, [qDel],
                mCtor, null, [mDel],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res));
        }

        // 2. Binary .ctor (0x06000214)
        {
            var qType = BinaryDelegateFunctionType(Side.Quire);
            var mType = BinaryDelegateFunctionType(Side.Mathcad);
            var qDel = CreateFDelegate(Side.Quire, 2);
            var mDel = CreateFDelegate(Side.Mathcad, 2);
            var qCtor = qType.GetConstructor(Instance, null, [qDel.GetType()], null)!;
            var mCtor = mType.GetConstructor(Instance, null, [mDel.GetType()], null)!;
            Assert.Equal(0x06000214, mCtor.MetadataToken);
            Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);
            MethodComparison.Compare("BinaryDelegateFunction.ctor", Contract,
                qCtor, null, [qDel],
                mCtor, null, [mDel],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res));
        }

        // 3. Ternary .ctor (0x06000219)
        {
            var qType = TernaryDelegateFunctionType(Side.Quire);
            var mType = TernaryDelegateFunctionType(Side.Mathcad);
            var qDel = CreateFDelegate(Side.Quire, 3);
            var mDel = CreateFDelegate(Side.Mathcad, 3);
            var qCtor = qType.GetConstructor(Instance, null, [qDel.GetType()], null)!;
            var mCtor = mType.GetConstructor(Instance, null, [mDel.GetType()], null)!;
            Assert.Equal(0x06000219, mCtor.MetadataToken);
            Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);
            MethodComparison.Compare("TernaryDelegateFunction.ctor", Contract,
                qCtor, null, [qDel],
                mCtor, null, [mDel],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res));
        }

        // 4. Quartnary .ctor (0x0600021e)
        {
            var qType = QuartnaryDelegateFunctionType(Side.Quire);
            var mType = QuartnaryDelegateFunctionType(Side.Mathcad);
            var qDel = CreateFDelegate(Side.Quire, 4);
            var mDel = CreateFDelegate(Side.Mathcad, 4);
            var qCtor = qType.GetConstructor(Instance, null, [qDel.GetType()], null)!;
            var mCtor = mType.GetConstructor(Instance, null, [mDel.GetType()], null)!;
            Assert.Equal(0x0600021e, mCtor.MetadataToken);
            Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);
            MethodComparison.Compare("QuartnaryDelegateFunction.ctor", Contract,
                qCtor, null, [qDel],
                mCtor, null, [mDel],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res));
        }
    }

    [SkippableFact]
    public void Specialized_DelegateFunction_inv_and_app()
    {
        Runtime.Sync();
        // 1. Unary
        {
            var qType = UnaryDelegateFunctionType(Side.Quire);
            var mType = UnaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 1);
            object mTarget = MakeTarget(Side.Mathcad, 1);

            // inv<R, A>(out R, A) -> 0x06000210
            var qInvDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "inv" && m.IsGenericMethodDefinition);
            var mInvDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "inv" && m.IsGenericMethodDefinition);
            Assert.Equal(0x06000210, mInvDef.MetadataToken);
            Assert.Equal(MathcadMvid, mInvDef.Module.ModuleVersionId);
            var qInv = qInvDef.MakeGenericMethod(typeof(string), typeof(int));
            var mInv = mInvDef.MakeGenericMethod(typeof(string), typeof(int));
            object?[] qArgs = [null, 11];
            object?[] mArgs = [null, 11];
            MethodComparison.Compare("UnaryDelegateFunction.inv", Contract,
                qInv, qTarget, (object?[])qArgs.Clone(),
                mInv, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // app<R, A>(out R, A) -> 0x06000211
            var qAppDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "app" && m.IsGenericMethodDefinition);
            var mAppDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "app" && m.IsGenericMethodDefinition);
            Assert.Equal(0x06000211, mAppDef.MetadataToken);
            Assert.Equal(MathcadMvid, mAppDef.Module.ModuleVersionId);
            var qApp = qAppDef.MakeGenericMethod(typeof(string), typeof(int));
            var mApp = mAppDef.MakeGenericMethod(typeof(string), typeof(int));
            MethodComparison.Compare("UnaryDelegateFunction.app", Contract,
                qApp, qTarget, (object?[])qArgs.Clone(),
                mApp, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // 2. Binary
        {
            var qType = BinaryDelegateFunctionType(Side.Quire);
            var mType = BinaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 2);
            object mTarget = MakeTarget(Side.Mathcad, 2);

            // inv<R, A, B>(out R, A, B) -> 0x06000215
            var qInvDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "inv" && m.IsGenericMethodDefinition);
            var mInvDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "inv" && m.IsGenericMethodDefinition);
            Assert.Equal(0x06000215, mInvDef.MetadataToken);
            Assert.Equal(MathcadMvid, mInvDef.Module.ModuleVersionId);
            var qInv = qInvDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int));
            var mInv = mInvDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int));
            object?[] qArgs = [null, 11, 22];
            object?[] mArgs = [null, 11, 22];
            MethodComparison.Compare("BinaryDelegateFunction.inv", Contract,
                qInv, qTarget, (object?[])qArgs.Clone(),
                mInv, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // app<R, A, B>(out R, A, B) -> 0x06000216
            var qAppDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "app" && m.IsGenericMethodDefinition);
            var mAppDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "app" && m.IsGenericMethodDefinition);
            Assert.Equal(0x06000216, mAppDef.MetadataToken);
            Assert.Equal(MathcadMvid, mAppDef.Module.ModuleVersionId);
            var qApp = qAppDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int));
            var mApp = mAppDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int));
            MethodComparison.Compare("BinaryDelegateFunction.app", Contract,
                qApp, qTarget, (object?[])qArgs.Clone(),
                mApp, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // 3. Ternary
        {
            var qType = TernaryDelegateFunctionType(Side.Quire);
            var mType = TernaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 3);
            object mTarget = MakeTarget(Side.Mathcad, 3);

            // inv<R, A, B, C>(out R, A, B, C) -> 0x0600021a
            var qInvDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "inv" && m.IsGenericMethodDefinition);
            var mInvDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "inv" && m.IsGenericMethodDefinition);
            Assert.Equal(0x0600021a, mInvDef.MetadataToken);
            Assert.Equal(MathcadMvid, mInvDef.Module.ModuleVersionId);
            var qInv = qInvDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int));
            var mInv = mInvDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int));
            object?[] qArgs = [null, 11, 22, 33];
            object?[] mArgs = [null, 11, 22, 33];
            MethodComparison.Compare("TernaryDelegateFunction.inv", Contract,
                qInv, qTarget, (object?[])qArgs.Clone(),
                mInv, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // app<R, A, B, C>(out R, A, B, C) -> 0x0600021b
            var qAppDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "app" && m.IsGenericMethodDefinition);
            var mAppDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "app" && m.IsGenericMethodDefinition);
            Assert.Equal(0x0600021b, mAppDef.MetadataToken);
            Assert.Equal(MathcadMvid, mAppDef.Module.ModuleVersionId);
            var qApp = qAppDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int));
            var mApp = mAppDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int));
            MethodComparison.Compare("TernaryDelegateFunction.app", Contract,
                qApp, qTarget, (object?[])qArgs.Clone(),
                mApp, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }

        // 4. Quartnary
        {
            var qType = QuartnaryDelegateFunctionType(Side.Quire);
            var mType = QuartnaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 4);
            object mTarget = MakeTarget(Side.Mathcad, 4);

            // inv<R, A, B, C, D>(out R, A, B, C, D) -> 0x0600021f
            var qInvDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "inv" && m.IsGenericMethodDefinition);
            var mInvDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "inv" && m.IsGenericMethodDefinition);
            Assert.Equal(0x0600021f, mInvDef.MetadataToken);
            Assert.Equal(MathcadMvid, mInvDef.Module.ModuleVersionId);
            var qInv = qInvDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int), typeof(int));
            var mInv = mInvDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int), typeof(int));
            object?[] qArgs = [null, 11, 22, 33, 44];
            object?[] mArgs = [null, 11, 22, 33, 44];
            MethodComparison.Compare("QuartnaryDelegateFunction.inv", Contract,
                qInv, qTarget, (object?[])qArgs.Clone(),
                mInv, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // app<R, A, B, C, D>(out R, A, B, C, D) -> 0x06000220
            var qAppDef = qType.GetMethods(Instance).Single(m => m.DeclaringType == qType && m.Name == "app" && m.IsGenericMethodDefinition);
            var mAppDef = mType.GetMethods(Instance).Single(m => m.DeclaringType == mType && m.Name == "app" && m.IsGenericMethodDefinition);
            Assert.Equal(0x06000220, mAppDef.MetadataToken);
            Assert.Equal(MathcadMvid, mAppDef.Module.ModuleVersionId);
            var qApp = qAppDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int), typeof(int));
            var mApp = mAppDef.MakeGenericMethod(typeof(string), typeof(int), typeof(int), typeof(int), typeof(int));
            MethodComparison.Compare("QuartnaryDelegateFunction.app", Contract,
                qApp, qTarget, (object?[])qArgs.Clone(),
                mApp, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }

    [SkippableFact]
    public void Specialized_DelegateFunction_DynCall_and_Premium()
    {
        Runtime.Sync();
        // 1. Unary
        {
            var qType = UnaryDelegateFunctionType(Side.Quire);
            var mType = UnaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 1, minArgs: 1, noVectorize: true, name: "UName");
            object mTarget = MakeTarget(Side.Mathcad, 1, minArgs: 1, noVectorize: true, name: "UName");

            // DynCall(object) -> 0x06000212
            var qDynCall = qType.GetMethod("DynCall", Instance, [typeof(object)])!;
            var mDynCall = mType.GetMethod("DynCall", Instance, [typeof(object)])!;
            Assert.Equal(0x06000212, mDynCall.MetadataToken);
            Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);
            MethodComparison.Compare("UnaryDelegateFunction.DynCall", Contract,
                qDynCall, qTarget, ["u_in"],
                mDynCall, mTarget, ["u_in"],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // Premium() -> 0x06000213
            var qPrem = qType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            var mPrem = mType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            Assert.Equal(0x06000213, mPrem.MetadataToken);
            Assert.Equal(MathcadMvid, mPrem.Module.ModuleVersionId);
            MethodComparison.Compare("UnaryDelegateFunction.Premium", Contract,
                qPrem, qTarget, [],
                mPrem, mTarget, [],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res),
                quireState: t => RenderSnapshot(Side.Quire, t),
                originalState: t => RenderSnapshot(Side.Mathcad, t));
        }

        // 2. Binary
        {
            var qType = BinaryDelegateFunctionType(Side.Quire);
            var mType = BinaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 2, minArgs: 2, noVectorize: true, name: "BName");
            object mTarget = MakeTarget(Side.Mathcad, 2, minArgs: 2, noVectorize: true, name: "BName");

            // DynCall(object, object) -> 0x06000217
            var qDynCall = qType.GetMethod("DynCall", Instance, [typeof(object), typeof(object)])!;
            var mDynCall = mType.GetMethod("DynCall", Instance, [typeof(object), typeof(object)])!;
            Assert.Equal(0x06000217, mDynCall.MetadataToken);
            Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);
            MethodComparison.Compare("BinaryDelegateFunction.DynCall", Contract,
                qDynCall, qTarget, ["b_in1", "b_in2"],
                mDynCall, mTarget, ["b_in1", "b_in2"],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // Premium() -> 0x06000218
            var qPrem = qType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            var mPrem = mType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            Assert.Equal(0x06000218, mPrem.MetadataToken);
            Assert.Equal(MathcadMvid, mPrem.Module.ModuleVersionId);
            MethodComparison.Compare("BinaryDelegateFunction.Premium", Contract,
                qPrem, qTarget, [],
                mPrem, mTarget, [],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res),
                quireState: t => RenderSnapshot(Side.Quire, t),
                originalState: t => RenderSnapshot(Side.Mathcad, t));
        }

        // 3. Ternary
        {
            var qType = TernaryDelegateFunctionType(Side.Quire);
            var mType = TernaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 3, minArgs: 3, noVectorize: true, name: "TName");
            object mTarget = MakeTarget(Side.Mathcad, 3, minArgs: 3, noVectorize: true, name: "TName");

            // DynCall(object, object, object) -> 0x0600021c
            var qDynCall = qType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object)])!;
            var mDynCall = mType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object)])!;
            Assert.Equal(0x0600021c, mDynCall.MetadataToken);
            Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);
            MethodComparison.Compare("TernaryDelegateFunction.DynCall", Contract,
                qDynCall, qTarget, ["t_in1", "t_in2", "t_in3"],
                mDynCall, mTarget, ["t_in1", "t_in2", "t_in3"],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // Premium() -> 0x0600021d
            var qPrem = qType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            var mPrem = mType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            Assert.Equal(0x0600021d, mPrem.MetadataToken);
            Assert.Equal(MathcadMvid, mPrem.Module.ModuleVersionId);
            MethodComparison.Compare("TernaryDelegateFunction.Premium", Contract,
                qPrem, qTarget, [],
                mPrem, mTarget, [],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res),
                quireState: t => RenderSnapshot(Side.Quire, t),
                originalState: t => RenderSnapshot(Side.Mathcad, t));
        }

        // 4. Quartnary
        {
            var qType = QuartnaryDelegateFunctionType(Side.Quire);
            var mType = QuartnaryDelegateFunctionType(Side.Mathcad);
            object qTarget = MakeTarget(Side.Quire, 4, minArgs: 4, noVectorize: true, name: "QName");
            object mTarget = MakeTarget(Side.Mathcad, 4, minArgs: 4, noVectorize: true, name: "QName");

            // DynCall(object, object, object, object) -> 0x06000221
            var qDynCall = qType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object), typeof(object)])!;
            var mDynCall = mType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object), typeof(object)])!;
            Assert.Equal(0x06000221, mDynCall.MetadataToken);
            Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);
            MethodComparison.Compare("QuartnaryDelegateFunction.DynCall", Contract,
                qDynCall, qTarget, ["q_in1", "q_in2", "q_in3", "q_in4"],
                mDynCall, mTarget, ["q_in1", "q_in2", "q_in3", "q_in4"],
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));

            // Premium() -> 0x06000222
            var qPrem = qType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            var mPrem = mType.GetMethod("Premium", Instance, Type.EmptyTypes)!;
            Assert.Equal(0x06000222, mPrem.MetadataToken);
            Assert.Equal(MathcadMvid, mPrem.Module.ModuleVersionId);
            MethodComparison.Compare("QuartnaryDelegateFunction.Premium", Contract,
                qPrem, qTarget, [],
                mPrem, mTarget, [],
                quireShow: res => RenderSnapshot(Side.Quire, res),
                originalShow: res => RenderSnapshot(Side.Mathcad, res),
                quireState: t => RenderSnapshot(Side.Quire, t),
                originalState: t => RenderSnapshot(Side.Mathcad, t));
        }
    }

    [SkippableFact]
    public void PremiumFunction_constructor_Name_and_invVec()
    {
        Runtime.Sync();

        Func<object[], object> del = arr => "res:" + string.Join(",", arr);
        object qInner = MakeFnCloPurityTarget(Side.Quire, del, 2, "InnerFunc");
        object mInner = MakeFnCloPurityTarget(Side.Mathcad, del, 2, "InnerFunc");

        // .ctor(Function) -> 0x06000238
        var qCtor = PremiumFunctionType(Side.Quire).GetConstructor(Instance, null, [FunctionType(Side.Quire)], null)!;
        var mCtor = PremiumFunctionType(Side.Mathcad).GetConstructor(Instance, null, [FunctionType(Side.Mathcad)], null)!;
        Assert.Equal(0x06000238, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);
        MethodComparison.Compare("PremiumFunction.ctor", Contract,
            qCtor, null, [qInner],
            mCtor, null, [mInner],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));

        object qTarget = MakePremiumTarget(Side.Quire, del, 2, "TestPremiumFunc");
        object mTarget = MakePremiumTarget(Side.Mathcad, del, 2, "TestPremiumFunc");

        // get_Name() -> 0x06000239
        var qName = PremiumFunctionType(Side.Quire).GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = PremiumFunctionType(Side.Mathcad).GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x06000239, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);
        MethodComparison.Compare("PremiumFunction.Name", Contract,
            qName, qTarget, [],
            mName, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // invVec<R>(out R, object[]) -> 0x0600023a
        var qPType = PremiumFunctionType(Side.Quire);
        var mPType = PremiumFunctionType(Side.Mathcad);
        var qInvVecDef = qPType.GetMethods(Instance).Single(m => m.DeclaringType == qPType && m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mPType.GetMethods(Instance).Single(m => m.DeclaringType == mPType && m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));
        Assert.Equal(0x0600023a, mInvVec.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVec.Module.ModuleVersionId);

        object?[] qArgs = [null, new object[] { "alpha", "beta" }];
        object?[] mArgs = [null, new object[] { "alpha", "beta" }];
        MethodComparison.Compare("PremiumFunction.invVec", Contract,
            qInvVec, qTarget, (object?[])qArgs.Clone(),
            mInvVec, mTarget, (object?[])mArgs.Clone(),
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void PremiumFunction_DynCall_overloads_1_to_16()
    {
        Runtime.Sync();

        Func<object[], object> del = arr =>
            $"res:{arr.Length}:" + string.Join(",", arr);

        var qPType = PremiumFunctionType(Side.Quire);
        var mPType = PremiumFunctionType(Side.Mathcad);

        for (int k = 1; k <= 16; k++)
        {
            object qTarget = MakePremiumTarget(Side.Quire, del, k);
            object mTarget = MakePremiumTarget(Side.Mathcad, del, k);

            var qDynCall = qPType.GetMethods(Instance)
                .Single(m => m.DeclaringType == qPType && m.Name == "DynCall" && !m.IsGenericMethod
                    && m.GetParameters().Length == k
                    && m.GetParameters().All(p => p.ParameterType == typeof(object)));
            var mDynCall = mPType.GetMethods(Instance)
                .Single(m => m.DeclaringType == mPType && m.Name == "DynCall" && !m.IsGenericMethod
                    && m.GetParameters().Length == k
                    && m.GetParameters().All(p => p.ParameterType == typeof(object)));

            Assert.Equal(0x0600023a + k, mDynCall.MetadataToken);
            Assert.Equal(MathcadMvid, mDynCall.Module.ModuleVersionId);

            object?[] qArgs = new object?[k];
            object?[] mArgs = new object?[k];
            for (int i = 0; i < k; i++)
            {
                qArgs[i] = (i + 1) * 10;
                mArgs[i] = (i + 1) * 10;
            }

            MethodComparison.Compare($"PremiumFunction.DynCall_{k}", Contract,
                qDynCall, qTarget, (object?[])qArgs.Clone(),
                mDynCall, mTarget, (object?[])mArgs.Clone(),
                Show, Show,
                t => RenderSnapshot(Side.Quire, t),
                t => RenderSnapshot(Side.Mathcad, t));
        }
    }
}
