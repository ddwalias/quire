using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnCurryTupleDifferentialTests
{
    private const string Contract = "Mpl.Run.Fn_CurryTuple";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FnCurryTupleType(Side side) => side == Side.Quire
        ? typeof(Fn_CurryTuple)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_CurryTuple");

    private static Type FunctionType(Side side) => side == Side.Quire
        ? typeof(Function)
        : Mathcad.Type("mplrun", "Mpl.Run.Function");

    private static Type DelegateFunctionType(Side side) => side == Side.Quire
        ? typeof(DelegateFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.DelegateFunction");

    private static object MakeDelegateFunction(Side side, Delegate d, int purity, string? name = null)
    {
        var method = DelegateFunctionType(side).GetMethod("make", Static, [typeof(Delegate), typeof(int)])
            ?? throw new HarnessException($"Missing {side.Name} DelegateFunction.make");
        object result = method.Invoke(null, [d, purity])!;
        if (name != null) Set(result, "m_name", name);
        return result;
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record FnCurryTupleDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        string? Name,
        string? InnerName);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? f = FieldOrNull(target, "m_f");
        var snapshot = new FnCurryTupleDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (string?)FieldOrNull(target, "m_name"),
            (string?)FieldOrNull(f, "m_name"));
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Fn_CurryTuple_constructor()
    {
        Runtime.Sync();
        Type qType = FnCurryTupleType(Side.Quire);
        Type mType = FnCurryTupleType(Side.Mathcad);
        Type qFuncType = FunctionType(Side.Quire);
        Type mFuncType = FunctionType(Side.Mathcad);

        var qCtor = qType.GetConstructor(Instance, null, [qFuncType, typeof(int), typeof(object)], null)!;
        var mCtor = mType.GetConstructor(Instance, null, [mFuncType, typeof(int), typeof(object)], null)!;
        Assert.Equal(0x0600026c, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        Func<object[], object> del = arr => "ok";
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "InnerFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "InnerFunc");

        MethodComparison.Compare("Fn_CurryTuple.ctor.string_name", Contract,
            qCtor, null, [qF, 2, "CurryAlpha"],
            mCtor, null, [mF, 2, "CurryAlpha"],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));

        MethodComparison.Compare("Fn_CurryTuple.ctor.int_name", Contract,
            qCtor, null, [qF, 1, 999],
            mCtor, null, [mF, 1, 999],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void Fn_CurryTuple_Name()
    {
        Runtime.Sync();
        Type qType = FnCurryTupleType(Side.Quire);
        Type mType = FnCurryTupleType(Side.Mathcad);

        var qName = qType.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = mType.GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x0600026d, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);

        Func<object[], object> del = arr => "ok";
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "InnerFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "InnerFunc");

        object qTarget = Activator.CreateInstance(qType, [qF, 2, "CurriedTupleName"])!;
        object mTarget = Activator.CreateInstance(mType, [mF, 2, "CurriedTupleName"])!;

        MethodComparison.Compare("Fn_CurryTuple.Name", Contract,
            qName, qTarget, [],
            mName, mTarget, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_CurryTuple_invVec()
    {
        Runtime.Sync();
        Type qType = FnCurryTupleType(Side.Quire);
        Type mType = FnCurryTupleType(Side.Mathcad);

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        Assert.Equal(0x0600026e, mInvVecDef.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVecDef.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join(",", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "InnerFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "InnerFunc");

        // Purity 2 matching 2 args
        object qTarget2 = Activator.CreateInstance(qType, [qF, 2, "Tuple2"])!;
        object mTarget2 = Activator.CreateInstance(mType, [mF, 2, "Tuple2"])!;

        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(object));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(object));

        object?[] qArgs1 = [null, new object[] { "arg1", "arg2" }];
        object?[] mArgs1 = [null, new object[] { "arg1", "arg2" }];
        MethodComparison.Compare("Fn_CurryTuple.invVec.full_app", Contract,
            qInvVec, qTarget2, (object?[])qArgs1.Clone(),
            mInvVec, mTarget2, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // Purity 0 matching 0 args
        object qTarget0 = Activator.CreateInstance(qType, [qF, 0, "Tuple0"])!;
        object mTarget0 = Activator.CreateInstance(mType, [mF, 0, "Tuple0"])!;

        object?[] qArgs2 = [null, Array.Empty<object>()];
        object?[] mArgs2 = [null, Array.Empty<object>()];
        MethodComparison.Compare("Fn_CurryTuple.invVec.zero_app", Contract,
            qInvVec, qTarget0, (object?[])qArgs2.Clone(),
            mInvVec, mTarget0, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_CurryTuple_Apply()
    {
        Runtime.Sync();
        Type qType = FnCurryTupleType(Side.Quire);
        Type mType = FnCurryTupleType(Side.Mathcad);

        var qApply = qType.GetMethod("Apply", Instance, [typeof(object[])])!;
        var mApply = mType.GetMethod("Apply", Instance, [typeof(object[])])!;
        Assert.Equal(0x0600026f, mApply.MetadataToken);
        Assert.Equal(MathcadMvid, mApply.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join("-", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "ApplyInner");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "ApplyInner");

        object qTarget = Activator.CreateInstance(qType, [qF, 2, "ApplyTuple"])!;
        object mTarget = Activator.CreateInstance(mType, [mF, 2, "ApplyTuple"])!;

        object?[] qArgs = [new object[] { "first", "second" }];
        object?[] mArgs = [new object[] { "first", "second" }];
        MethodComparison.Compare("Fn_CurryTuple.Apply", Contract,
            qApply, qTarget, (object?[])qArgs.Clone(),
            mApply, mTarget, (object?[])mArgs.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_CurryTuple_DynApply()
    {
        Runtime.Sync();
        Type qType = FnCurryTupleType(Side.Quire);
        Type mType = FnCurryTupleType(Side.Mathcad);

        var qDynApply = qType.GetMethod("DynApply", Instance, [typeof(object[])])!;
        var mDynApply = mType.GetMethod("DynApply", Instance, [typeof(object[])])!;
        Assert.Equal(0x06000270, mDynApply.MetadataToken);
        Assert.Equal(MathcadMvid, mDynApply.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join("+", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "DynApplyInner");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "DynApplyInner");

        object qTarget = Activator.CreateInstance(qType, [qF, 2, "DynApplyTuple"])!;
        object mTarget = Activator.CreateInstance(mType, [mF, 2, "DynApplyTuple"])!;

        object?[] qArgs = [new object[] { 100, 200 }];
        object?[] mArgs = [new object[] { 100, 200 }];
        MethodComparison.Compare("Fn_CurryTuple.DynApply", Contract,
            qDynApply, qTarget, (object?[])qArgs.Clone(),
            mDynApply, mTarget, (object?[])mArgs.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
