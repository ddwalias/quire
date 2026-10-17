using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnVarargsDifferentialTests
{
    private const string Contract = "Mpl.Run.Fn_varargs";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FnVarargsType(Side side) => side == Side.Quire
        ? typeof(Fn_varargs)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_varargs");

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

    private sealed record FnVarargsDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        string? InnerName);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? f = FieldOrNull(target, "m_f");
        var snapshot = new FnVarargsDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (string?)FieldOrNull(f, "m_name"));
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Fn_varargs_constructor()
    {
        Runtime.Sync();
        Type qType = FnVarargsType(Side.Quire);
        Type mType = FnVarargsType(Side.Mathcad);
        Type qFuncType = FunctionType(Side.Quire);
        Type mFuncType = FunctionType(Side.Mathcad);

        var qCtor = qType.GetConstructor(Instance, null, [qFuncType], null)!;
        var mCtor = mType.GetConstructor(Instance, null, [mFuncType], null)!;
        Assert.Equal(0x06000263, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join(",", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "InnerFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "InnerFunc");

        MethodComparison.Compare("Fn_varargs.ctor", Contract,
            qCtor, null, [qF],
            mCtor, null, [mF],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void Fn_varargs_Name()
    {
        Runtime.Sync();
        Type qType = FnVarargsType(Side.Quire);
        Type mType = FnVarargsType(Side.Mathcad);

        var qName = qType.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = mType.GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x06000264, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);

        Func<object[], object> del = arr => "ok";
        object qF1 = MakeDelegateFunction(Side.Quire, del, 1, name: "MyVarargsInner");
        object mF1 = MakeDelegateFunction(Side.Mathcad, del, 1, name: "MyVarargsInner");

        object qTarget1 = Activator.CreateInstance(qType, [qF1])!;
        object mTarget1 = Activator.CreateInstance(mType, [mF1])!;

        MethodComparison.Compare("Fn_varargs.Name", Contract,
            qName, qTarget1, [],
            mName, mTarget1, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_varargs_invVec()
    {
        Runtime.Sync();
        Type qType = FnVarargsType(Side.Quire);
        Type mType = FnVarargsType(Side.Mathcad);

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        Assert.Equal(0x06000265, mInvVecDef.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVecDef.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join(";", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "InnerFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "InnerFunc");

        object qTarget = Activator.CreateInstance(qType, [qF])!;
        object mTarget = Activator.CreateInstance(mType, [mF])!;

        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(object));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(object));

        object?[] qArgs1 = [null, new object[] { "x", "y", "z" }];
        object?[] mArgs1 = [null, new object[] { "x", "y", "z" }];
        MethodComparison.Compare("Fn_varargs.invVec.nonempty", Contract,
            qInvVec, qTarget, (object?[])qArgs1.Clone(),
            mInvVec, mTarget, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        object?[] qArgs2 = [null, Array.Empty<object>()];
        object?[] mArgs2 = [null, Array.Empty<object>()];
        MethodComparison.Compare("Fn_varargs.invVec.empty", Contract,
            qInvVec, qTarget, (object?[])qArgs2.Clone(),
            mInvVec, mTarget, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_varargs_Apply()
    {
        Runtime.Sync();
        Type qType = FnVarargsType(Side.Quire);
        Type mType = FnVarargsType(Side.Mathcad);

        var qApply = qType.GetMethod("Apply", Instance, [typeof(object[])])!;
        var mApply = mType.GetMethod("Apply", Instance, [typeof(object[])])!;
        Assert.Equal(0x06000266, mApply.MetadataToken);
        Assert.Equal(MathcadMvid, mApply.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join("-", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "ApplyInner");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "ApplyInner");

        object qTarget = Activator.CreateInstance(qType, [qF])!;
        object mTarget = Activator.CreateInstance(mType, [mF])!;

        object?[] qArgs1 = [new object[] { 10, 20 }];
        object?[] mArgs1 = [new object[] { 10, 20 }];
        MethodComparison.Compare("Fn_varargs.Apply.nonempty", Contract,
            qApply, qTarget, (object?[])qArgs1.Clone(),
            mApply, mTarget, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        object?[] qArgs2 = [Array.Empty<object>()];
        object?[] mArgs2 = [Array.Empty<object>()];
        MethodComparison.Compare("Fn_varargs.Apply.empty", Contract,
            qApply, qTarget, (object?[])qArgs2.Clone(),
            mApply, mTarget, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_varargs_DynApply()
    {
        Runtime.Sync();
        Type qType = FnVarargsType(Side.Quire);
        Type mType = FnVarargsType(Side.Mathcad);

        var qDynApply = qType.GetMethod("DynApply", Instance, [typeof(object[])])!;
        var mDynApply = mType.GetMethod("DynApply", Instance, [typeof(object[])])!;
        Assert.Equal(0x06000267, mDynApply.MetadataToken);
        Assert.Equal(MathcadMvid, mDynApply.Module.ModuleVersionId);

        Func<object[], object> del = arr => string.Join("+", arr.Select(x => x?.ToString() ?? "null"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "DynApplyInner");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "DynApplyInner");

        object qTarget = Activator.CreateInstance(qType, [qF])!;
        object mTarget = Activator.CreateInstance(mType, [mF])!;

        object?[] qArgs1 = [new object[] { "p", "q" }];
        object?[] mArgs1 = [new object[] { "p", "q" }];
        MethodComparison.Compare("Fn_varargs.DynApply.nonempty", Contract,
            qDynApply, qTarget, (object?[])qArgs1.Clone(),
            mDynApply, mTarget, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        object?[] qArgs2 = [Array.Empty<object>()];
        object?[] mArgs2 = [Array.Empty<object>()];
        MethodComparison.Compare("Fn_varargs.DynApply.empty", Contract,
            qDynApply, qTarget, (object?[])qArgs2.Clone(),
            mDynApply, mTarget, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Fn_varargs_DynCall_1_to_4()
    {
        Runtime.Sync();
        Type qType = FnVarargsType(Side.Quire);
        Type mType = FnVarargsType(Side.Mathcad);

        Func<object[], object> del = arr => string.Join(",", arr.Select((x, idx) => $"[{idx}]={x}"));
        object qF = MakeDelegateFunction(Side.Quire, del, 1, name: "DynCallInner");
        object mF = MakeDelegateFunction(Side.Mathcad, del, 1, name: "DynCallInner");

        object qTarget = Activator.CreateInstance(qType, [qF])!;
        object mTarget = Activator.CreateInstance(mType, [mF])!;

        // 1. DynCall(object) -> 0x06000268
        var qDynCall1 = qType.GetMethod("DynCall", Instance, [typeof(object)])!;
        var mDynCall1 = mType.GetMethod("DynCall", Instance, [typeof(object)])!;
        Assert.Equal(0x06000268, mDynCall1.MetadataToken);
        Assert.Equal(MathcadMvid, mDynCall1.Module.ModuleVersionId);
        MethodComparison.Compare("Fn_varargs.DynCall1", Contract,
            qDynCall1, qTarget, ["first"],
            mDynCall1, mTarget, ["first"],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 2. DynCall(object, object) -> 0x06000269
        var qDynCall2 = qType.GetMethod("DynCall", Instance, [typeof(object), typeof(object)])!;
        var mDynCall2 = mType.GetMethod("DynCall", Instance, [typeof(object), typeof(object)])!;
        Assert.Equal(0x06000269, mDynCall2.MetadataToken);
        Assert.Equal(MathcadMvid, mDynCall2.Module.ModuleVersionId);
        MethodComparison.Compare("Fn_varargs.DynCall2", Contract,
            qDynCall2, qTarget, ["first", "second"],
            mDynCall2, mTarget, ["first", "second"],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 3. DynCall(object, object, object) -> 0x0600026a
        var qDynCall3 = qType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object)])!;
        var mDynCall3 = mType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object)])!;
        Assert.Equal(0x0600026a, mDynCall3.MetadataToken);
        Assert.Equal(MathcadMvid, mDynCall3.Module.ModuleVersionId);
        MethodComparison.Compare("Fn_varargs.DynCall3", Contract,
            qDynCall3, qTarget, ["first", "second", "third"],
            mDynCall3, mTarget, ["first", "second", "third"],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 4. DynCall(object, object, object, object) -> 0x0600026b
        var qDynCall4 = qType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object), typeof(object)])!;
        var mDynCall4 = mType.GetMethod("DynCall", Instance, [typeof(object), typeof(object), typeof(object), typeof(object)])!;
        Assert.Equal(0x0600026b, mDynCall4.MetadataToken);
        Assert.Equal(MathcadMvid, mDynCall4.Module.ModuleVersionId);
        MethodComparison.Compare("Fn_varargs.DynCall4", Contract,
            qDynCall4, qTarget, ["first", "second", "third", "fourth"],
            mDynCall4, mTarget, ["first", "second", "third", "fourth"],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
