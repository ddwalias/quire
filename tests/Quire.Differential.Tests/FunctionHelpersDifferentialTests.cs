using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FunctionHelpersDifferentialTests
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

    private static object MakeFnCloTarget(Side side, Func<object[], object> del, int purity, string? name = null)
        => MakeFnCloTarget(side, del, purity, purity, name);

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

    private sealed record FunctionHelpersSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        bool NoVectorize,
        string? Name,
        bool HasInverse,
        string? InnerType = null,
        int? InnerPurity = null);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? inv = FieldOrNull(target, "m_inverse_function");
        object? inner = FieldOrNull(target, "m_f");
        var snapshot = new FunctionHelpersSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (bool)(FieldOrNull(target, "m_no_vectorize") ?? false),
            (string?)FieldOrNull(target, "m_name"),
            inv != null,
            inner?.GetType().Name,
            (int?)FieldOrNull(inner, "m_purity"));
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Function_InverseFunction_getter_and_setter()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "f";
        object qTarget = MakeFnCloTarget(Side.Quire, del, 1);
        object mTarget = MakeFnCloTarget(Side.Mathcad, del, 1);

        object qInv = MakeFnCloTarget(Side.Quire, del, 1, "inv");
        object mInv = MakeFnCloTarget(Side.Mathcad, del, 1, "inv");

        var qGet = FunctionType(Side.Quire).GetProperty("InverseFunction", Instance)!.GetGetMethod()!;
        var mGet = FunctionType(Side.Mathcad).GetProperty("InverseFunction", Instance)!.GetGetMethod()!;
        Assert.Equal(0x06000157, mGet.MetadataToken);
        Assert.Equal(MathcadMvid, mGet.Module.ModuleVersionId);

        var qSet = FunctionType(Side.Quire).GetProperty("InverseFunction", Instance)!.GetSetMethod()!;
        var mSet = FunctionType(Side.Mathcad).GetProperty("InverseFunction", Instance)!.GetSetMethod()!;
        Assert.Equal(0x06000158, mSet.MetadataToken);
        Assert.Equal(MathcadMvid, mSet.Module.ModuleVersionId);

        // Case 1: get throws when unset
        MethodComparison.Compare("Function.get_InverseFunction_unset", Contract,
            qGet, qTarget, [],
            mGet, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Case 2: set inverse function
        MethodComparison.Compare("Function.set_InverseFunction", Contract,
            qSet, qTarget, [qInv],
            mSet, mTarget, [mInv],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Case 3: get returns inverse after set
        MethodComparison.Compare("Function.get_InverseFunction_set", Contract,
            qGet, qTarget, [],
            mGet, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_IgnoreVectorization_and_Premium()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "f";
        object qTarget = MakeFnCloTarget(Side.Quire, del, 1);
        object mTarget = MakeFnCloTarget(Side.Mathcad, del, 1);

        // IgnoreVectorization() -> 0x06000154
        var qIgnore = FunctionType(Side.Quire).GetMethod("IgnoreVectorization", Instance, Type.EmptyTypes)!;
        var mIgnore = FunctionType(Side.Mathcad).GetMethod("IgnoreVectorization", Instance, Type.EmptyTypes)!;
        Assert.Equal(0x06000154, mIgnore.MetadataToken);
        Assert.Equal(MathcadMvid, mIgnore.Module.ModuleVersionId);

        MethodComparison.Compare("Function.IgnoreVectorization", Contract,
            qIgnore, qTarget, [],
            mIgnore, mTarget, [],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Premium() -> 0x0600015f
        var qPrem = FunctionType(Side.Quire).GetMethod("Premium", Instance, Type.EmptyTypes)!;
        var mPrem = FunctionType(Side.Mathcad).GetMethod("Premium", Instance, Type.EmptyTypes)!;
        Assert.Equal(0x0600015f, mPrem.MetadataToken);
        Assert.Equal(MathcadMvid, mPrem.Module.ModuleVersionId);

        MethodComparison.Compare("Function.Premium", Contract,
            qPrem, qTarget, [],
            mPrem, mTarget, [],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_asFunction_and_IsFunctionN()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "f";
        object qTarget = MakeFnCloTarget(Side.Quire, del, 2);
        object mTarget = MakeFnCloTarget(Side.Mathcad, del, 2);

        // asFunction(string, uint) -> 0x0600015b
        var qAsFunc = FunctionType(Side.Quire).GetMethod("asFunction", Instance, [typeof(string), typeof(uint)])!;
        var mAsFunc = FunctionType(Side.Mathcad).GetMethod("asFunction", Instance, [typeof(string), typeof(uint)])!;
        Assert.Equal(0x0600015b, mAsFunc.MetadataToken);
        Assert.Equal(MathcadMvid, mAsFunc.Module.ModuleVersionId);

        MethodComparison.Compare("Function.asFunction", Contract,
            qAsFunc, qTarget, ["msg", 0u],
            mAsFunc, mTarget, ["msg", 0u],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // IsFunctionN(int) -> 0x0600015c
        var qIsFuncN = FunctionType(Side.Quire).GetMethod("IsFunctionN", Instance, [typeof(int)])!;
        var mIsFuncN = FunctionType(Side.Mathcad).GetMethod("IsFunctionN", Instance, [typeof(int)])!;
        Assert.Equal(0x0600015c, mIsFuncN.MetadataToken);
        Assert.Equal(MathcadMvid, mIsFuncN.Module.ModuleVersionId);

        // Matching arity: returns this
        MethodComparison.Compare("Function.IsFunctionN_matching", Contract,
            qIsFuncN, qTarget, [2],
            mIsFuncN, mTarget, [2],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Non-matching arity: returns null (kills brfalse.s -> brtrue.s mutant!)
        MethodComparison.Compare("Function.IsFunctionN_mismatch", Contract,
            qIsFuncN, qTarget, [1],
            mIsFuncN, mTarget, [1],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // asFunctionN(int, uint) -> 0x0600015d
        var qAsFuncN = FunctionType(Side.Quire).GetMethod("asFunctionN", Instance, [typeof(int), typeof(uint)])!;
        var mAsFuncN = FunctionType(Side.Mathcad).GetMethod("asFunctionN", Instance, [typeof(int), typeof(uint)])!;
        Assert.Equal(0x0600015d, mAsFuncN.MetadataToken);
        Assert.Equal(MathcadMvid, mAsFuncN.Module.ModuleVersionId);

        MethodComparison.Compare("Function.asFunctionN", Contract,
            qAsFuncN, qTarget, [2, 0u],
            mAsFuncN, mTarget, [2, 0u],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // AssertFullApp(int, uint) -> 0x060001a8
        // Target with minargs = 2 and purity = 4 to distinguish argc < m_minargs vs argc > m_purity!
        object qAssertTarget = MakeFnCloTarget(Side.Quire, del, purity: 4, minArgs: 2);
        object mAssertTarget = MakeFnCloTarget(Side.Mathcad, del, purity: 4, minArgs: 2);

        var qAssertFull = FunctionType(Side.Quire).GetMethod("AssertFullApp", Instance, [typeof(int), typeof(uint)])!;
        var mAssertFull = FunctionType(Side.Mathcad).GetMethod("AssertFullApp", Instance, [typeof(int), typeof(uint)])!;
        Assert.Equal(0x060001a8, mAssertFull.MetadataToken);
        Assert.Equal(MathcadMvid, mAssertFull.Module.ModuleVersionId);

        // Success: matching arity in range [minargs, purity]
        MethodComparison.Compare("Function.AssertFullApp_success_2", Contract,
            qAssertFull, qAssertTarget, [2, 0u],
            mAssertFull, mAssertTarget, [2, 0u],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        MethodComparison.Compare("Function.AssertFullApp_success_3", Contract,
            qAssertFull, qAssertTarget, [3, 0u],
            mAssertFull, mAssertTarget, [3, 0u],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Failure: under-arity (1 < 2) with errpos 0 (kills blt.s -> bge.s mutant: argc < minargs selects minargs=2; mutant selects purity=4)
        MethodComparison.Compare("Function.AssertFullApp_under_0", Contract,
            qAssertFull, qAssertTarget, [1, 0u],
            mAssertFull, mAssertTarget, [1, 0u],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Failure: under-arity (1 < 2) with errpos 2 (kills errpos != 0 mutant)
        MethodComparison.Compare("Function.AssertFullApp_under_pos", Contract,
            qAssertFull, qAssertTarget, [1, 2u],
            mAssertFull, mAssertTarget, [1, 2u],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Failure: over-arity (5 > 4) (kills blt.s -> bge.s mutant: argc < minargs is false -> selects purity=4; mutant selects minargs=2)
        MethodComparison.Compare("Function.AssertFullApp_over", Contract,
            qAssertFull, qAssertTarget, [5, 0u],
            mAssertFull, mAssertTarget, [5, 0u],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_CheckPremium()
    {
        Runtime.Sync();
        var qCheck = FunctionType(Side.Quire).GetMethod("CheckPremium", Static, Type.EmptyTypes)!;
        var mCheck = FunctionType(Side.Mathcad).GetMethod("CheckPremium", Static, Type.EmptyTypes)!;
        Assert.Equal(0x0600015e, mCheck.MetadataToken);
        Assert.Equal(MathcadMvid, mCheck.Module.ModuleVersionId);

        // Kills brfalse.s -> brtrue.s mutant!
        MethodComparison.Compare("Function.CheckPremium", Contract,
            qCheck, null, [],
            mCheck, null, [],
            Show, Show);
    }

    [SkippableFact]
    public void Function_Call_0()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "zero_res";
        object qTarget = MakeFnCloTarget(Side.Quire, del, 0);
        object mTarget = MakeFnCloTarget(Side.Mathcad, del, 0);

        var qCall0Def = FunctionType(Side.Quire).GetMethods(Instance).Single(m => m.Name == "Call_0" && m.IsGenericMethodDefinition);
        var mCall0Def = FunctionType(Side.Mathcad).GetMethods(Instance).Single(m => m.Name == "Call_0" && m.IsGenericMethodDefinition);
        Assert.Equal(0x06000182, mCall0Def.MetadataToken);
        Assert.Equal(MathcadMvid, mCall0Def.Module.ModuleVersionId);

        var qCall0 = qCall0Def.MakeGenericMethod(typeof(string));
        var mCall0 = mCall0Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Function.Call_0", Contract,
            qCall0, qTarget, [],
            mCall0, mTarget, [],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_DynApply()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "dyn:" + string.Join(",", arr);
        object qTarget = MakeFnCloTarget(Side.Quire, del, 2);
        object mTarget = MakeFnCloTarget(Side.Mathcad, del, 2);

        var qDynApply = FunctionType(Side.Quire).GetMethod("DynApply", Instance, [typeof(object[])])!;
        var mDynApply = FunctionType(Side.Mathcad).GetMethod("DynApply", Instance, [typeof(object[])])!;
        Assert.Equal(0x060001d1, mDynApply.MetadataToken);
        Assert.Equal(MathcadMvid, mDynApply.Module.ModuleVersionId);

        // Case 1: single argument
        object[] args1 = ["first"];
        MethodComparison.Compare("Function.DynApply_1", Contract,
            qDynApply, qTarget, [args1],
            mDynApply, mTarget, [args1],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // Case 2: two arguments (kills cgt.un / brfalse mutants)
        object[] args2 = ["first", "second"];
        MethodComparison.Compare("Function.DynApply_2", Contract,
            qDynApply, qTarget, [args2],
            mDynApply, mTarget, [args2],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_CurryMatVal_and_UnCurryMatVal()
    {
        Runtime.Sync();
        Func<object[], object> del1 = arr => "curried_rows:" + (arr.Length > 1 ? Get(arr[1], "Rows") : null);
        object qTarget1 = MakeFnCloTarget(Side.Quire, del1, 1);
        object mTarget1 = MakeFnCloTarget(Side.Mathcad, del1, 1);

        // CurryMatVal() -> 0x060001d5
        var qCurry = FunctionType(Side.Quire).GetMethod("CurryMatVal", Instance, Type.EmptyTypes)!;
        var mCurry = FunctionType(Side.Mathcad).GetMethod("CurryMatVal", Instance, Type.EmptyTypes)!;
        Assert.Equal(0x060001d5, mCurry.MetadataToken);
        Assert.Equal(MathcadMvid, mCurry.Module.ModuleVersionId);

        MethodComparison.Compare("Function.CurryMatVal", Contract,
            qCurry, qTarget1, [],
            mCurry, mTarget1, [],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        object qCurried = qCurry.Invoke(qTarget1, [])!;
        object mCurried = mCurry.Invoke(mTarget1, [])!;
        var qDynCall2 = qCurried.GetType().GetMethod("DynCall", Instance, [typeof(object), typeof(object)])!;
        var mDynCall2 = mCurried.GetType().GetMethod("DynCall", Instance, [typeof(object), typeof(object)])!;

        // DynCall with 2 arguments: creates 2x1 MatVal and passes to target1
        MethodComparison.Compare("Function.CurryMatVal_invoke_2", "Mpl.Run.Fn_varargs",
            qDynCall2, qCurried, ["a", "b"],
            mDynCall2, mCurried, ["a", "b"],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // UnCurryMatVal() -> 0x060001d6
        Func<object[], object> del2 = arr => "uncurried:" + string.Join(",", arr);
        object qTarget2 = MakeFnCloTarget(Side.Quire, del2, 2);
        object mTarget2 = MakeFnCloTarget(Side.Mathcad, del2, 2);

        // Incompatible arity: AsDelegate_1(1u) calls AssertFullApp(1, 1u), throwing AppException with errpos = 1u.
        // Mutant ldc.i4.1 -> ldc.i4.0 calls AsDelegate_1(0u), throwing AppException with errpos = 0u!
        MethodComparison.Compare("Function.CurryMatVal_incompatible_arity", Contract,
            qCurry, qTarget2, [],
            mCurry, mTarget2, [],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        var qUnCurry = FunctionType(Side.Quire).GetMethod("UnCurryMatVal", Instance, Type.EmptyTypes)!;
        var mUnCurry = FunctionType(Side.Mathcad).GetMethod("UnCurryMatVal", Instance, Type.EmptyTypes)!;
        Assert.Equal(0x060001d6, mUnCurry.MetadataToken);
        Assert.Equal(MathcadMvid, mUnCurry.Module.ModuleVersionId);

        MethodComparison.Compare("Function.UnCurryMatVal", Contract,
            qUnCurry, qTarget2, [],
            mUnCurry, mTarget2, [],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res),
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        object qUnCurried = qUnCurry.Invoke(qTarget2, [])!;
        object mUnCurried = mUnCurry.Invoke(mTarget2, [])!;
        var qDynCall1 = qUnCurried.GetType().GetMethod("DynCall", Instance, [typeof(object)])!;
        var mDynCall1 = mUnCurried.GetType().GetMethod("DynCall", Instance, [typeof(object)])!;

        // 1. Invoke with valid matrix: calls (m as DimVal).asMatrix(1u).PackObjects()
        object qMat = New(Side.Quire.T("Mcd.MatVal"), (object)new object[] { "x", "y" });
        object mMat = New(Side.Mathcad.T("Mcd.MatVal"), (object)new object[] { "x", "y" });
        MethodComparison.Compare("Function.UnCurryMatVal_invoke_mat", Contract,
            qDynCall1, qUnCurried, [qMat],
            mDynCall1, mUnCurried, [mMat],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));

        // 2. Invoke with scalar NumVal (non-matrix DimVal): (m as DimVal).asMatrix(1u) throws AppException!
        // Mutant ldc.i4.1 -> ldc.i4.0 changes errpos from 1 to 0, which is captured in the AppException comparison!
        object qNum = Side.Quire.Num(1.0, 0.0, "identity");
        object mNum = Side.Mathcad.Num(1.0, 0.0, "identity");
        MethodComparison.Compare("Function.UnCurryMatVal_invoke_scalar_throws", Contract,
            qDynCall1, qUnCurried, [qNum],
            mDynCall1, mUnCurried, [mNum],
            Show, Show,
            t => RenderSnapshot(Side.Quire, t),
            t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Function_factories_Constant_Fixpoint_Compose()
    {
        Runtime.Sync();
        Func<object[], object> del = arr => "f";
        object qF = MakeFnCloTarget(Side.Quire, del, 1, "F");
        object mF = MakeFnCloTarget(Side.Mathcad, del, 1, "F");
        object qG = MakeFnCloTarget(Side.Quire, del, 1, "G");
        object mG = MakeFnCloTarget(Side.Mathcad, del, 1, "G");

        // ConstantFunction(object) -> 0x060001d7
        var qConst = FunctionType(Side.Quire).GetMethod("ConstantFunction", Static, [typeof(object)])!;
        var mConst = FunctionType(Side.Mathcad).GetMethod("ConstantFunction", Static, [typeof(object)])!;
        Assert.Equal(0x060001d7, mConst.MetadataToken);
        Assert.Equal(MathcadMvid, mConst.Module.ModuleVersionId);

        MethodComparison.Compare("Function.ConstantFunction", Contract,
            qConst, null, ["const_val"],
            mConst, null, ["const_val"],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res));

        // Fixpoint(Function) -> 0x060001e5
        var qFix = FunctionType(Side.Quire).GetMethod("Fixpoint", Static, [FunctionType(Side.Quire)])!;
        var mFix = FunctionType(Side.Mathcad).GetMethod("Fixpoint", Static, [FunctionType(Side.Mathcad)])!;
        Assert.Equal(0x060001e5, mFix.MetadataToken);
        Assert.Equal(MathcadMvid, mFix.Module.ModuleVersionId);

        MethodComparison.Compare("Function.Fixpoint", Contract,
            qFix, null, [qF],
            mFix, null, [mF],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res));

        // Compose(Function, Function) -> 0x060001e6
        var qComp = FunctionType(Side.Quire).GetMethod("Compose", Static, [FunctionType(Side.Quire), FunctionType(Side.Quire)])!;
        var mComp = FunctionType(Side.Mathcad).GetMethod("Compose", Static, [FunctionType(Side.Mathcad), FunctionType(Side.Mathcad)])!;
        Assert.Equal(0x060001e6, mComp.MetadataToken);
        Assert.Equal(MathcadMvid, mComp.Module.ModuleVersionId);

        MethodComparison.Compare("Function.Compose", Contract,
            qComp, null, [qF, qG],
            mComp, null, [mF, mG],
            res => RenderSnapshot(Side.Quire, res),
            res => RenderSnapshot(Side.Mathcad, res));
    }
}
