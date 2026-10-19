using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MethFunctionDifferentialTests
{
    private const string Contract = "Mpl.Run.MethFunction";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type MethFunctionType(Side side) => side == Side.Quire
        ? typeof(MethFunction)
        : Mathcad.Type("mplrun", "Mpl.Run.MethFunction");

    public static string SampleStaticMethod(string a, string b) => $"static:{a}+{b}";
    public string SampleInstanceMethod(string a, string b) => $"instance:{a}+{b}";

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        if (target.GetType().Name != "MethFunction") return Show(target);
        var method = (MethodBase?)FieldOrNull(target, "m_method");
        var clo = FieldOrNull(target, "m_clo");
        var args = (object[]?)FieldOrNull(target, "m_args") ?? [];
        var snapshot = new
        {
            Type = target.GetType().Name,
            Purity = (int)(FieldOrNull(target, "m_purity") ?? 0),
            MethodName = method?.Name,
            Clo = clo?.ToString(),
            Args = args.Select(a => a?.ToString()).ToArray(),
        };
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? o) => MplRunSides.Describe(o);

    [SkippableFact]
    public void MethFunction_ctors_and_invVec_static()
    {
        Runtime.Sync();
        Type qType = MethFunctionType(Side.Quire);
        Type mType = MethFunctionType(Side.Mathcad);

        var mi = typeof(MethFunctionDifferentialTests).GetMethod("SampleStaticMethod", BindingFlags.Public | BindingFlags.Static)!;
        RuntimeMethodHandle handle = mi.MethodHandle;

        // Constructor 1: (RuntimeMethodHandle, int)
        var qCtor1 = qType.GetConstructor([typeof(RuntimeMethodHandle), typeof(int)])!;
        var mCtor1 = mType.GetConstructor([typeof(RuntimeMethodHandle), typeof(int)])!;
        Assert.Equal(0x06000271, mCtor1.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor1.Module.ModuleVersionId);

        MethodComparison.Compare("MethFunction.ctor(mh, purity)", Contract,
            qCtor1, null, [handle, 1],
            mCtor1, null, [handle, 1],
            quireShow: t => RenderSnapshot(Side.Quire, t),
            originalShow: t => RenderSnapshot(Side.Mathcad, t));

        // Constructor 2: (RuntimeMethodHandle, int, object) with static method -> args prepended
        var qCtor2 = qType.GetConstructor([typeof(RuntimeMethodHandle), typeof(int), typeof(object)])!;
        var mCtor2 = mType.GetConstructor([typeof(RuntimeMethodHandle), typeof(int), typeof(object)])!;
        Assert.Equal(0x06000272, mCtor2.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor2.Module.ModuleVersionId);

        MethodComparison.Compare("MethFunction.ctor(mh, purity, clo) static", Contract,
            qCtor2, null, [handle, 1, "boundA"],
            mCtor2, null, [handle, 1, "boundA"],
            quireShow: t => RenderSnapshot(Side.Quire, t),
            originalShow: t => RenderSnapshot(Side.Mathcad, t));

        // invVec on static MethFunction
        object qTarget = Activator.CreateInstance(qType, [handle, 1, "boundA"])!;
        object mTarget = Activator.CreateInstance(mType, [handle, 1, "boundA"])!;

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));
        Assert.Equal(0x06000274, mInvVec.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVec.Module.ModuleVersionId);

        object?[] qArgs = [null, new object[] { "argB" }];
        object?[] mArgs = [null, new object[] { "argB" }];

        MethodComparison.Compare("MethFunction.invVec static", Contract,
            qInvVec, qTarget, (object?[])qArgs.Clone(),
            mInvVec, mTarget, (object?[])mArgs.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void MethFunction_ctors_and_invVec_instance()
    {
        Runtime.Sync();
        Type qType = MethFunctionType(Side.Quire);
        Type mType = MethFunctionType(Side.Mathcad);

        var mi = typeof(MethFunctionDifferentialTests).GetMethod("SampleInstanceMethod", BindingFlags.Public | BindingFlags.Instance)!;
        RuntimeMethodHandle handle = mi.MethodHandle;
        var instance = new MethFunctionDifferentialTests();

        // Constructor 2: (RuntimeMethodHandle, int, object) with instance method -> m_clo set
        var qCtor2 = qType.GetConstructor([typeof(RuntimeMethodHandle), typeof(int), typeof(object)])!;
        var mCtor2 = mType.GetConstructor([typeof(RuntimeMethodHandle), typeof(int), typeof(object)])!;

        MethodComparison.Compare("MethFunction.ctor(mh, purity, clo) instance", Contract,
            qCtor2, null, [handle, 2, instance],
            mCtor2, null, [handle, 2, instance],
            quireShow: t => RenderSnapshot(Side.Quire, t),
            originalShow: t => RenderSnapshot(Side.Mathcad, t));

        // invVec on bound instance MethFunction
        object qTarget = Activator.CreateInstance(qType, [handle, 2, instance])!;
        object mTarget = Activator.CreateInstance(mType, [handle, 2, instance])!;

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));

        object?[] qArgs = [null, new object[] { "arg1", "arg2" }];
        object?[] mArgs = [null, new object[] { "arg1", "arg2" }];

        MethodComparison.Compare("MethFunction.invVec bound instance", Contract,
            qInvVec, qTarget, (object?[])qArgs.Clone(),
            mInvVec, mTarget, (object?[])mArgs.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // invVec on unbound instance MethFunction (m_clo == null, target in args[0])
        object qUnbound = Activator.CreateInstance(qType, [handle, 2])!;
        object mUnbound = Activator.CreateInstance(mType, [handle, 2])!;

        object?[] qArgsUnbound = [null, new object[] { instance, "arg1", "arg2" }];
        object?[] mArgsUnbound = [null, new object[] { instance, "arg1", "arg2" }];

        MethodComparison.Compare("MethFunction.invVec unbound instance", Contract,
            qInvVec, qUnbound, (object?[])qArgsUnbound.Clone(),
            mInvVec, mUnbound, (object?[])mArgsUnbound.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
