using System.Reflection;
using System.Text.Json;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class FnComposeDifferentialTests
{
    private const string Contract = "Mpl.Run.FnCompose";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type FnComposeType(Side side) => side == Side.Quire
        ? typeof(FnCompose)
        : Mathcad.Type("mplrun", "Mpl.Run.FnCompose");

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

    private static object ConstantFunction(Side side, object value, int purity = 2, int minArgs = 0, string? name = null)
    {
        Type type = side == Side.Quire ? typeof(FnConstant) : Mathcad.Type("mplrun", "Mpl.Run.FnConstant");
        object result = Activator.CreateInstance(type, value)!;
        Set(result, "m_purity", purity);
        Set(result, "m_minargs", minArgs);
        if (name != null) Set(result, "m_name", name);
        return result;
    }

    private static object? FieldOrNull(object? target, string name)
    {
        if (target == null) return null;
        try { return Get(target, name); }
        catch (HarnessException) { return null; }
    }

    private sealed record FnComposeDirectSnapshot(
        string Type,
        int Purity,
        int MinArgs,
        int N,
        bool Varargs,
        string? FName,
        string? GName);

    private static string RenderSnapshot(Side side, object? target)
    {
        if (target == null) return "null";
        object? f = FieldOrNull(target, "m_f");
        object? g = FieldOrNull(target, "m_g");
        var snapshot = new FnComposeDirectSnapshot(
            target.GetType().Name,
            (int)(FieldOrNull(target, "m_purity") ?? 0),
            (int)(FieldOrNull(target, "m_minargs") ?? 0),
            (int)(FieldOrNull(target, "m_n") ?? 0),
            (bool)(FieldOrNull(target, "m_varargs") ?? false),
            (string?)FieldOrNull(f, "m_name"),
            (string?)FieldOrNull(g, "m_name"));
        return JsonSerializer.Serialize(snapshot);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void FnCompose_constructor()
    {
        Runtime.Sync();
        Type qType = FnComposeType(Side.Quire);
        Type mType = FnComposeType(Side.Mathcad);
        Type qFuncType = FunctionType(Side.Quire);
        Type mFuncType = FunctionType(Side.Mathcad);

        var qCtor = qType.GetConstructor(Instance, null, [qFuncType, qFuncType], null)!;
        var mCtor = mType.GetConstructor(Instance, null, [mFuncType, mFuncType], null)!;
        Assert.Equal(0x0600024b, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        object qF = ConstantFunction(Side.Quire, "outer", purity: 1, name: "OuterFunc");
        object mF = ConstantFunction(Side.Mathcad, "outer", purity: 1, name: "OuterFunc");
        object qG = ConstantFunction(Side.Quire, "inner", purity: 3, name: "InnerFunc");
        object mG = ConstantFunction(Side.Mathcad, "inner", purity: 3, name: "InnerFunc");

        MethodComparison.Compare("FnCompose.ctor", Contract,
            qCtor, null, [qF, qG],
            mCtor, null, [mF, mG],
            quireShow: res => RenderSnapshot(Side.Quire, res),
            originalShow: res => RenderSnapshot(Side.Mathcad, res));
    }

    [SkippableFact]
    public void FnCompose_Name()
    {
        Runtime.Sync();
        Type qType = FnComposeType(Side.Quire);
        Type mType = FnComposeType(Side.Mathcad);

        var qName = qType.GetProperty("Name", Instance)!.GetGetMethod()!;
        var mName = mType.GetProperty("Name", Instance)!.GetGetMethod()!;
        Assert.Equal(0x0600024c, mName.MetadataToken);
        Assert.Equal(MathcadMvid, mName.Module.ModuleVersionId);

        object qF = ConstantFunction(Side.Quire, "f_res", purity: 1, name: "Alpha");
        object mF = ConstantFunction(Side.Mathcad, "f_res", purity: 1, name: "Alpha");
        object qG = ConstantFunction(Side.Quire, "g_res", purity: 2, name: "Beta");
        object mG = ConstantFunction(Side.Mathcad, "g_res", purity: 2, name: "Beta");

        object qTarget = Activator.CreateInstance(qType, [qF, qG])!;
        object mTarget = Activator.CreateInstance(mType, [mF, mG])!;

        MethodComparison.Compare("FnCompose.Name", Contract,
            qName, qTarget, [],
            mName, mTarget, [],
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnCompose_invVec()
    {
        Runtime.Sync();
        Type qType = FnComposeType(Side.Quire);
        Type mType = FnComposeType(Side.Mathcad);

        var qInvVecDef = qType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var mInvVecDef = mType.GetMethods(Instance).First(m => m.Name == "invVec" && m.IsGenericMethodDefinition);
        var qInvVec = qInvVecDef.MakeGenericMethod(typeof(string));
        var mInvVec = mInvVecDef.MakeGenericMethod(typeof(string));
        Assert.Equal(0x0600024d, mInvVec.MetadataToken);
        Assert.Equal(MathcadMvid, mInvVec.Module.ModuleVersionId);

        Func<string, string> fDel = s => $"out({s})";
        Func<string, string, string> gDel = (a, b) => $"in({a},{b})";

        object qF = MakeDelegateFunction(Side.Quire, fDel, 1, name: "OutF");
        object mF = MakeDelegateFunction(Side.Mathcad, fDel, 1, name: "OutF");
        object qG = MakeDelegateFunction(Side.Quire, gDel, 2, name: "InG");
        object mG = MakeDelegateFunction(Side.Mathcad, gDel, 2, name: "InG");

        object qTarget = Activator.CreateInstance(qType, [qF, qG])!;
        object mTarget = Activator.CreateInstance(mType, [mF, mG])!;

        object?[] qArgs = [null, new object[] { "arg1", "arg2" }];
        object?[] mArgs = [null, new object[] { "arg1", "arg2" }];

        MethodComparison.Compare("FnCompose.invVec", Contract,
            qInvVec, qTarget, (object?[])qArgs.Clone(),
            mInvVec, mTarget, (object?[])mArgs.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }

    [SkippableFact]
    public void FnCompose_inv_overloads_1_to_4()
    {
        Runtime.Sync();
        Type qType = FnComposeType(Side.Quire);
        Type mType = FnComposeType(Side.Mathcad);

        Func<string, string> fDel = s => $"out({s})";
        object qF = MakeDelegateFunction(Side.Quire, fDel, 1, name: "OutF");
        object mF = MakeDelegateFunction(Side.Mathcad, fDel, 1, name: "OutF");

        // 1. inv<R, A>(out R, A) -> 0x0600024e
        Func<string, string> g1 = a => $"g1({a})";
        object qG1 = MakeDelegateFunction(Side.Quire, g1, 1, name: "G1");
        object mG1 = MakeDelegateFunction(Side.Mathcad, g1, 1, name: "G1");
        object qTarget1 = Activator.CreateInstance(qType, [qF, qG1])!;
        object mTarget1 = Activator.CreateInstance(mType, [mF, mG1])!;

        var qInv1Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);
        var mInv1Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);
        Assert.Equal(0x0600024e, mInv1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv1Def.Module.ModuleVersionId);
        var qInv1 = qInv1Def.MakeGenericMethod(typeof(string), typeof(string));
        var mInv1 = mInv1Def.MakeGenericMethod(typeof(string), typeof(string));

        object?[] qArgs1 = [null, "x1"];
        object?[] mArgs1 = [null, "x1"];
        MethodComparison.Compare("FnCompose.inv1", Contract,
            qInv1, qTarget1, (object?[])qArgs1.Clone(),
            mInv1, mTarget1, (object?[])mArgs1.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 2. inv<R, A, B>(out R, A, B) -> 0x0600024f
        Func<string, string, string> g2 = (a, b) => $"g2({a},{b})";
        object qG2 = MakeDelegateFunction(Side.Quire, g2, 2, name: "G2");
        object mG2 = MakeDelegateFunction(Side.Mathcad, g2, 2, name: "G2");
        object qTarget2 = Activator.CreateInstance(qType, [qF, qG2])!;
        object mTarget2 = Activator.CreateInstance(mType, [mF, mG2])!;

        var qInv2Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 3);
        var mInv2Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 3);
        Assert.Equal(0x0600024f, mInv2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv2Def.Module.ModuleVersionId);
        var qInv2 = qInv2Def.MakeGenericMethod(typeof(string), typeof(string), typeof(string));
        var mInv2 = mInv2Def.MakeGenericMethod(typeof(string), typeof(string), typeof(string));

        object?[] qArgs2 = [null, "x1", "x2"];
        object?[] mArgs2 = [null, "x1", "x2"];
        MethodComparison.Compare("FnCompose.inv2", Contract,
            qInv2, qTarget2, (object?[])qArgs2.Clone(),
            mInv2, mTarget2, (object?[])mArgs2.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 3. inv<R, A, B, C>(out R, A, B, C) -> 0x06000250
        Func<string, string, string, string> g3 = (a, b, c) => $"g3({a},{b},{c})";
        object qG3 = MakeDelegateFunction(Side.Quire, g3, 3, name: "G3");
        object mG3 = MakeDelegateFunction(Side.Mathcad, g3, 3, name: "G3");
        object qTarget3 = Activator.CreateInstance(qType, [qF, qG3])!;
        object mTarget3 = Activator.CreateInstance(mType, [mF, mG3])!;

        var qInv3Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 4);
        var mInv3Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 4);
        Assert.Equal(0x06000250, mInv3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv3Def.Module.ModuleVersionId);
        var qInv3 = qInv3Def.MakeGenericMethod(typeof(string), typeof(string), typeof(string), typeof(string));
        var mInv3 = mInv3Def.MakeGenericMethod(typeof(string), typeof(string), typeof(string), typeof(string));

        object?[] qArgs3 = [null, "x1", "x2", "x3"];
        object?[] mArgs3 = [null, "x1", "x2", "x3"];
        MethodComparison.Compare("FnCompose.inv3", Contract,
            qInv3, qTarget3, (object?[])qArgs3.Clone(),
            mInv3, mTarget3, (object?[])mArgs3.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));

        // 4. inv<R, A, B, C, D>(out R, A, B, C, D) -> 0x06000251
        Func<string, string, string, string, string> g4 = (a, b, c, d) => $"g4({a},{b},{c},{d})";
        object qG4 = MakeDelegateFunction(Side.Quire, g4, 4, name: "G4");
        object mG4 = MakeDelegateFunction(Side.Mathcad, g4, 4, name: "G4");
        object qTarget4 = Activator.CreateInstance(qType, [qF, qG4])!;
        object mTarget4 = Activator.CreateInstance(mType, [mF, mG4])!;

        var qInv4Def = qType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 5);
        var mInv4Def = mType.GetMethods(Instance).First(m => m.Name == "inv" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 5);
        Assert.Equal(0x06000251, mInv4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv4Def.Module.ModuleVersionId);
        var qInv4 = qInv4Def.MakeGenericMethod(typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
        var mInv4 = mInv4Def.MakeGenericMethod(typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

        object?[] qArgs4 = [null, "x1", "x2", "x3", "x4"];
        object?[] mArgs4 = [null, "x1", "x2", "x3", "x4"];
        MethodComparison.Compare("FnCompose.inv4", Contract,
            qInv4, qTarget4, (object?[])qArgs4.Clone(),
            mInv4, mTarget4, (object?[])mArgs4.Clone(),
            quireShow: Show, originalShow: Show,
            quireState: t => RenderSnapshot(Side.Quire, t),
            originalState: t => RenderSnapshot(Side.Mathcad, t));
    }
}
