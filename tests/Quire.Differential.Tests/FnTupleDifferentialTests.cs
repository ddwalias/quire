using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnTupleDifferentialTests
{
    private const string Contract = "Mpl.Run.Fn_tuple";
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FnTupleType(Side side) => side == Side.Quire
        ? typeof(Fn_tuple)
        : Mathcad.Type("mplrun", "Mpl.Run.Fn_tuple");

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record FnTupleDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        string[]? Args);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object?[]? rawArgs = (object?[]?)FieldOrNull(target, "m_args");
        string[]? args = rawArgs?.Select(x => x?.ToString() ?? "null").ToArray();
        var snapshot = new FnTupleDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            args);
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Fn_tuple_constructor()
    {
        Runtime.Sync();
        Type qType = FnTupleType(Side.Quire);
        Type mType = FnTupleType(Side.Mathcad);

        var qCtor = qType.GetConstructor(Instance, null, [typeof(object[]), typeof(int)], null)!;
        var mCtor = mType.GetConstructor(Instance, null, [typeof(object[]), typeof(int)], null)!;
        Assert.Equal(0x06000261, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        // Non-empty array, purity 1
        object[] tup1 = ["alpha", 42];
        MethodComparison.Compare("Fn_tuple.ctor.nonempty", Contract,
            qCtor, null, [(object?[])tup1.Clone(), 1],
            mCtor, null, [(object?[])tup1.Clone(), 1],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));

        // Empty array, purity 0
        object[] tup2 = Array.Empty<object>();
        MethodComparison.Compare("Fn_tuple.ctor.empty", Contract,
            qCtor, null, [(object?[])tup2.Clone(), 0],
            mCtor, null, [(object?[])tup2.Clone(), 0],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void Fn_tuple_invVec()
    {
        Runtime.Sync();
        Type qType = FnTupleType(Side.Quire);
        Type mType = FnTupleType(Side.Mathcad);

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        Assert.Equal(0x06000262, mInvVecDef.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVecDef.Module.ModuleVersionId);

        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(object[]));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(object[]));

        // 1. Stored ["x", "y"], incoming ["z", "w"]
        object[] stored1 = ["x", "y"];
        object qTarget1 = Activator.CreateInstance(qType, [stored1, 2])!;
        object mTarget1 = Activator.CreateInstance(mType, [stored1, 2])!;

        object?[] qArgs1 = [null, new object[] { "z", "w" }];
        object?[] mArgs1 = [null, new object[] { "z", "w" }];
        MethodComparison.Compare("Fn_tuple.invVec.both_nonempty", Contract,
            qInvVec, qTarget1, (object?[])qArgs1.Clone(),
            mInvVec, mTarget1, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 2. Stored empty, incoming ["a", "b"]
        object[] stored2 = Array.Empty<object>();
        object qTarget2 = Activator.CreateInstance(qType, [stored2, 1])!;
        object mTarget2 = Activator.CreateInstance(mType, [stored2, 1])!;

        object?[] qArgs2 = [null, new object[] { "a", "b" }];
        object?[] mArgs2 = [null, new object[] { "a", "b" }];
        MethodComparison.Compare("Fn_tuple.invVec.stored_empty", Contract,
            qInvVec, qTarget2, (object?[])qArgs2.Clone(),
            mInvVec, mTarget2, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 3. Stored ["c"], incoming empty
        object[] stored3 = ["c"];
        object qTarget3 = Activator.CreateInstance(qType, [stored3, 3])!;
        object mTarget3 = Activator.CreateInstance(mType, [stored3, 3])!;

        object?[] qArgs3 = [null, Array.Empty<object>()];
        object?[] mArgs3 = [null, Array.Empty<object>()];
        MethodComparison.Compare("Fn_tuple.invVec.incoming_empty", Contract,
            qInvVec, qTarget3, (object?[])qArgs3.Clone(),
            mInvVec, mTarget3, (object?[])mArgs3.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
