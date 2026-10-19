using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnPappDifferentialTests
{
    private const string Contract = "Mpl.Run.Fn_papp";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FnPappType(Side side) => side == Side.Quire
        ? typeof(Fn_papp)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_papp");

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

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        if (target.GetType().Name != "Fn_papp") return Show(target);
        var f = FieldOrNull(target, "m_f");
        var args = (object[]?)FieldOrNull(target, "m_args") ?? [];
        var snapshot = new
        {
            Type = target.GetType().Name,
            Purity = (int)(FieldOrNull(target, "m_purity") ?? 0),
            MinArgs = (int)(FieldOrNull(target, "m_minargs") ?? 0),
            Varargs = (bool)(FieldOrNull(target, "m_varargs") ?? false),
            FName = (string?)FieldOrNull(f, "m_name"),
            Args = args.Select(a => a?.ToString()).ToArray(),
        };
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? o) => MplRunSides.Describe(o);

    [SkippableFact]
    public void FnPapp_ctor_and_Name()
    {
        Runtime.Sync();
        Type qType = FnPappType(Side.Quire);
        Type mType = FnPappType(Side.Mathcad);

        Func<object, object, object> binDel = (a, b) => $"{a}:{b}";
        object qF = MakeDelegateFunction(Side.Quire, binDel, 2, name: "BinFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, binDel, 2, name: "BinFunc");

        var qCtor = qType.GetConstructor([FunctionType(Side.Quire), typeof(object[])])!;
        var mCtor = mType.GetConstructor([FunctionType(Side.Mathcad), typeof(object[])])!;
        Assert.Equal(0x0600025e, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        object[] pArgs = ["bound1"];
        MethodComparison.Compare("Fn_papp.ctor", Contract,
            qCtor, null, [qF, (object[])pArgs.Clone()],
            mCtor, null, [mF, (object[])pArgs.Clone()],
            quireShow: t => RenderSnapshot(Side.Quire, t),
            originalShow: t => RenderSnapshot(Side.Mathcad, t));

        object qTarget = Activator.CreateInstance(qType, [qF, (object[])pArgs.Clone()])!;
        object mTarget = Activator.CreateInstance(mType, [mF, (object[])pArgs.Clone()])!;

        var qName = qType.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = mType.GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x06000260, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);

        MethodComparison.Compare("Fn_papp.Name", Contract,
            qName, qTarget, [],
            mName, mTarget, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnPapp_invVec()
    {
        Runtime.Sync();
        Type qType = FnPappType(Side.Quire);
        Type mType = FnPappType(Side.Mathcad);

        Func<object, object, object> binDel = (a, b) => $"{a}:{b}";
        object qF = MakeDelegateFunction(Side.Quire, binDel, 2, name: "BinFunc");
        object mF = MakeDelegateFunction(Side.Mathcad, binDel, 2, name: "BinFunc");

        object[] pArgs = ["bound1"];
        object qTarget = Activator.CreateInstance(qType, [qF, (object[])pArgs.Clone()])!;
        object mTarget = Activator.CreateInstance(mType, [mF, (object[])pArgs.Clone()])!;

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));
        Assert.Equal(0x0600025f, mInvVec.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVec.Module.ModuleVersionId);

        object?[] qArgs = [null, new object[] { "arg2" }];
        object?[] mArgs = [null, new object[] { "arg2" }];

        MethodComparison.Compare("Fn_papp.invVec", Contract,
            qInvVec, qTarget, (object?[])qArgs.Clone(),
            mInvVec, mTarget, (object?[])mArgs.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
