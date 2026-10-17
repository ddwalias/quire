using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnConstantDifferentialTests
{
    private const string Contract = "Mpl.Run.FnConstant";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FnConstantType(Side side) => side == Side.Quire
        ? typeof(FnConstant)
        : Mathcad.Type("mplrun", "Mpl.Run.FnConstant");

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record FnConstantDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        string? Val);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? val = FieldOrNull(target, "m_val");
        var snapshot = new FnConstantDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            val?.ToString());
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void FnConstant_constructor()
    {
        Runtime.Sync();
        Type qType = FnConstantType(Side.Quire);
        Type mType = FnConstantType(Side.Mathcad);

        var qCtor = qType.GetConstructor(Instance, null, [typeof(object)], null)!;
        var mCtor = mType.GetConstructor(Instance, null, [typeof(object)], null)!;
        Assert.Equal(0x06000252, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        MethodComparison.Compare("FnConstant.ctor.string", Contract,
            qCtor, null, ["hello"],
            mCtor, null, ["hello"],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));

        MethodComparison.Compare("FnConstant.ctor.int", Contract,
            qCtor, null, [42],
            mCtor, null, [42],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void FnConstant_Name()
    {
        Runtime.Sync();
        Type qType = FnConstantType(Side.Quire);
        Type mType = FnConstantType(Side.Mathcad);

        var qName = qType.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = mType.GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x06000253, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);

        object qTarget1 = Activator.CreateInstance(qType, ["test_constant"])!;
        object mTarget1 = Activator.CreateInstance(mType, ["test_constant"])!;
        MethodComparison.Compare("FnConstant.Name.string", Contract,
            qName, qTarget1, [],
            mName, mTarget1, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        object qTarget2 = Activator.CreateInstance(qType, [99])!;
        object mTarget2 = Activator.CreateInstance(mType, [99])!;
        MethodComparison.Compare("FnConstant.Name.int", Contract,
            qName, qTarget2, [],
            mName, mTarget2, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnConstant_invVec()
    {
        Runtime.Sync();
        Type qType = FnConstantType(Side.Quire);
        Type mType = FnConstantType(Side.Mathcad);

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        Assert.Equal(0x06000254, mInvVecDef.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVecDef.Module.ModuleVersionId);

        // String generic
        var qInvVecStr = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVecStr = mInvVecDef.MakeGenericMethod(typeof(string));
        object qTargetStr = Activator.CreateInstance(qType, ["constant_val"])!;
        object mTargetStr = Activator.CreateInstance(mType, ["constant_val"])!;

        object?[] qArgs1 = [null, new object[] { "arg1", "arg2" }];
        object?[] mArgs1 = [null, new object[] { "arg1", "arg2" }];
        MethodComparison.Compare("FnConstant.invVec.string", Contract,
            qInvVecStr, qTargetStr, (object?[])qArgs1.Clone(),
            mInvVecStr, mTargetStr, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // Int generic
        var qInvVecInt = qInvVecDef.MakeGenericMethod(typeof(int));
        var mInvVecInt = mInvVecDef.MakeGenericMethod(typeof(int));
        object qTargetInt = Activator.CreateInstance(qType, [12345])!;
        object mTargetInt = Activator.CreateInstance(mType, [12345])!;

        object?[] qArgs2 = [0, Array.Empty<object>()];
        object?[] mArgs2 = [0, Array.Empty<object>()];
        MethodComparison.Compare("FnConstant.invVec.int", Contract,
            qInvVecInt, qTargetInt, (object?[])qArgs2.Clone(),
            mInvVecInt, mTargetInt, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnConstant_Apply()
    {
        Runtime.Sync();
        Type qType = FnConstantType(Side.Quire);
        Type mType = FnConstantType(Side.Mathcad);

        var qApply = qType.GetMethod("Apply", Instance, [typeof(object[])])!;
        var mApply = mType.GetMethod("Apply", Instance, [typeof(object[])])!;
        Assert.Equal(0x06000255, mApply.MetadataToken);
        Assert.Equal(MathcadMvid, mApply.Module.ModuleVersionId);

        object qTarget = Activator.CreateInstance(qType, ["applied_result"])!;
        object mTarget = Activator.CreateInstance(mType, ["applied_result"])!;

        object?[] qArgs1 = [new object[] { 1, 2, 3 }];
        object?[] mArgs1 = [new object[] { 1, 2, 3 }];
        MethodComparison.Compare("FnConstant.Apply.nonempty", Contract,
            qApply, qTarget, (object?[])qArgs1.Clone(),
            mApply, mTarget, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        object?[] qArgs2 = [Array.Empty<object>()];
        object?[] mArgs2 = [Array.Empty<object>()];
        MethodComparison.Compare("FnConstant.Apply.empty", Contract,
            qApply, qTarget, (object?[])qArgs2.Clone(),
            mApply, mTarget, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnConstant_DynApply()
    {
        Runtime.Sync();
        Type qType = FnConstantType(Side.Quire);
        Type mType = FnConstantType(Side.Mathcad);

        var qDynApply = qType.GetMethod("DynApply", Instance, [typeof(object[])])!;
        var mDynApply = mType.GetMethod("DynApply", Instance, [typeof(object[])])!;
        Assert.Equal(0x06000256, mDynApply.MetadataToken);
        Assert.Equal(MathcadMvid, mDynApply.Module.ModuleVersionId);

        object qTarget = Activator.CreateInstance(qType, ["dyn_applied_result"])!;
        object mTarget = Activator.CreateInstance(mType, ["dyn_applied_result"])!;

        object?[] qArgs1 = [new object[] { "alpha", "beta" }];
        object?[] mArgs1 = [new object[] { "alpha", "beta" }];
        MethodComparison.Compare("FnConstant.DynApply.nonempty", Contract,
            qDynApply, qTarget, (object?[])qArgs1.Clone(),
            mDynApply, mTarget, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        object?[] qArgs2 = [Array.Empty<object>()];
        object?[] mArgs2 = [Array.Empty<object>()];
        MethodComparison.Compare("FnConstant.DynApply.empty", Contract,
            qDynApply, qTarget, (object?[])qArgs2.Clone(),
            mDynApply, mTarget, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnConstant_inv_overloads_1_to_4()
    {
        Runtime.Sync();
        Type qType = FnConstantType(Side.Quire);
        Type mType = FnConstantType(Side.Mathcad);

        object qTarget = Activator.CreateInstance(qType, ["inv_result"])!;
        object mTarget = Activator.CreateInstance(mType, ["inv_result"])!;

        // 1. inv<R, A>(out R, A) -> 0x06000257
        var qInv1Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);
        var mInv1Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);
        Assert.Equal(0x06000257, mInv1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv1Def.Module.ModuleVersionId);
        var qInv1 = qInv1Def.MakeGenericMethod(typeof(string), typeof(int));
        var mInv1 = mInv1Def.MakeGenericMethod(typeof(string), typeof(int));

        object?[] qArgs1 = [null, 10];
        object?[] mArgs1 = [null, 10];
        MethodComparison.Compare("FnConstant.inv1", Contract,
            qInv1, qTarget, (object?[])qArgs1.Clone(),
            mInv1, mTarget, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 2. inv<R, A, B>(out R, A, B) -> 0x06000258
        var qInv2Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 3);
        var mInv2Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 3);
        Assert.Equal(0x06000258, mInv2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv2Def.Module.ModuleVersionId);
        var qInv2 = qInv2Def.MakeGenericMethod(typeof(string), typeof(int), typeof(double));
        var mInv2 = mInv2Def.MakeGenericMethod(typeof(string), typeof(int), typeof(double));

        object?[] qArgs2 = [null, 10, 2.5];
        object?[] mArgs2 = [null, 10, 2.5];
        MethodComparison.Compare("FnConstant.inv2", Contract,
            qInv2, qTarget, (object?[])qArgs2.Clone(),
            mInv2, mTarget, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 3. inv<R, A, B, C>(out R, A, B, C) -> 0x06000259
        var qInv3Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 4);
        var mInv3Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 4);
        Assert.Equal(0x06000259, mInv3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv3Def.Module.ModuleVersionId);
        var qInv3 = qInv3Def.MakeGenericMethod(typeof(string), typeof(int), typeof(double), typeof(bool));
        var mInv3 = mInv3Def.MakeGenericMethod(typeof(string), typeof(int), typeof(double), typeof(bool));

        object?[] qArgs3 = [null, 10, 2.5, true];
        object?[] mArgs3 = [null, 10, 2.5, true];
        MethodComparison.Compare("FnConstant.inv3", Contract,
            qInv3, qTarget, (object?[])qArgs3.Clone(),
            mInv3, mTarget, (object?[])mArgs3.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 4. inv<R, A, B, C, D>(out R, A, B, C, D) -> 0x0600025a
        var qInv4Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 5);
        var mInv4Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 5);
        Assert.Equal(0x0600025a, mInv4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv4Def.Module.ModuleVersionId);
        var qInv4 = qInv4Def.MakeGenericMethod(typeof(string), typeof(int), typeof(double), typeof(bool), typeof(string));
        var mInv4 = mInv4Def.MakeGenericMethod(typeof(string), typeof(int), typeof(double), typeof(bool), typeof(string));

        object?[] qArgs4 = [null, 10, 2.5, true, "extra"];
        object?[] mArgs4 = [null, 10, 2.5, true, "extra"];
        MethodComparison.Compare("FnConstant.inv4", Contract,
            qInv4, qTarget, (object?[])qArgs4.Clone(),
            mInv4, mTarget, (object?[])mArgs4.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
