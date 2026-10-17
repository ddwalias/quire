using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class MplForeignMacroDifferentialTests
{
    private static readonly BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic
        | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static readonly Func<object?, string> Show = value => Describe(value);
    private static readonly Func<object, string> State = value =>
        $"MplForeignMacro(name={Describe(Get(value, "m_name"))},sig={Describe(Get(value, "m_sig"))},macro={Describe(Get(value, "m_macro"))})";

    private static MethodInfo Method(System.Type type, string name, params System.Type[] arguments) =>
        type.GetMethod(name, DeclaredInstance, null, arguments, null)
        ?? throw new HarnessException($"Method not found: {type.FullName}.{name}");

    private static MethodInfo Getter(System.Type type) =>
        type.GetProperty("Macro", DeclaredInstance)?.GetMethod
        ?? throw new HarnessException($"Property getter not found: {type.FullName}.Macro");

    [SkippableFact]
    public void MplForeignMacro_members()
    {
        Runtime.Sync();

        System.Type qType = typeof(MplForeignMacro);
        System.Type mType = M("Mpl.Run.MplForeignMacro");
        System.Type qEnvType = typeof(MplEnv);
        System.Type mEnvType = M("Mpl.Run.MplEnv");
        var qCtor = qType.GetConstructor(DeclaredInstance, null, [qEnvType, typeof(Sym), typeof(string)], null)!;
        var mCtor = mType.GetConstructor(DeclaredInstance, null, [mEnvType, M("Mpl.Run.Sym"), typeof(string)], null)!;
        var qGetter = Getter(qType);
        var mGetter = Getter(mType);
        var qExpandCall = Method(qType, "ExpandCall", qEnvType, typeof(Sexpr));
        var mExpandCall = Method(mType, "ExpandCall", mEnvType, M("Mpl.Run.Sexpr"));
        var qExpandRef = Method(qType, "ExpandRef", qEnvType, typeof(Tok));
        var mExpandRef = Method(mType, "ExpandRef", mEnvType, M("Mpl.Run.Tok"));

        var qName = new Sym("foreign", null);
        var mName = ToMathcad(qName)!;
        MethodComparison.Compare("MplForeignMacro.ctor", "Mpl.Run.MplForeignMacro",
            qCtor, null, [null, qName, "foreign.sig"], mCtor, null, [null, mName, "foreign.sig"], Show, Show);

        var qReplacement = int_constant.NewTok(42, 0u);
        var mReplacement = ToMathcad(qReplacement)!;
        var qMacro = new MplSymbolMacro(qName, qReplacement);
        var mMacro = New(M("Mpl.Run.MplSymbolMacro"), ["Sym", "Sexpr"], mName, mReplacement)!;
        var qCall = SCons.list2(new Tok(qName, 9u), int_constant.NewTok(7, 0u));
        var mCall = ToMathcad(qCall)!;
        var qCallTarget = new MplForeignMacro(null, qName, "foreign.sig");
        var mCallTarget = New(mType, ["MplEnv", "Sym", "String"], null, mName, "foreign.sig")!;
        Set(qCallTarget, "m_macro", qMacro);
        Set(mCallTarget, "m_macro", mMacro);
        MethodComparison.Compare("MplForeignMacro.ExpandCall cached macro", "Mpl.Run.MplForeignMacro",
            qExpandCall, qCallTarget, [null, qCall], mExpandCall, mCallTarget, [null, mCall], Show, Show, State, State);

        var qReference = new Tok(qName, 13u);
        var mReference = ToMathcad(qReference)!;
        var qRefTarget = new MplForeignMacro(null, qName, "foreign.sig");
        var mRefTarget = New(mType, ["MplEnv", "Sym", "String"], null, mName, "foreign.sig")!;
        Set(qRefTarget, "m_macro", qMacro);
        Set(mRefTarget, "m_macro", mMacro);
        MethodComparison.Compare("MplForeignMacro.ExpandRef cached macro", "Mpl.Run.MplForeignMacro",
            qExpandRef, qRefTarget, [null, qReference], mExpandRef, mRefTarget, [null, mReference], Show, Show, State, State);

        var qCachedTarget = new MplForeignMacro(null, qName, "ignored.sig");
        var mCachedTarget = New(mType, ["MplEnv", "Sym", "String"], null, mName, "ignored.sig")!;
        Set(qCachedTarget, "m_macro", qMacro);
        Set(mCachedTarget, "m_macro", mMacro);
        MethodComparison.Compare("MplForeignMacro.Macro cache hit", "Mpl.Run.MplForeignMacro",
            qGetter, qCachedTarget, [], mGetter, mCachedTarget, [], Show, Show, State, State);

        var qMissingTarget = new MplForeignMacro(null, qName, "missing.sig");
        var mMissingTarget = New(mType, ["MplEnv", "Sym", "String"], null, mName, "missing.sig")!;
        MethodComparison.Compare("MplForeignMacro.Macro null environment", "Mpl.Run.MplForeignMacro",
            qGetter, qMissingTarget, [], mGetter, mMissingTarget, [], Show, Show, State, State);

        var qCallMissing = new MplForeignMacro(null, qName, "missing.sig");
        var mCallMissing = New(mType, ["MplEnv", "Sym", "String"], null, mName, "missing.sig")!;
        MethodComparison.Compare("MplForeignMacro.ExpandCall null environment", "Mpl.Run.MplForeignMacro",
            qExpandCall, qCallMissing, [null, qCall], mExpandCall, mCallMissing, [null, mCall], Show, Show, State, State);

        var qRefMissing = new MplForeignMacro(null, qName, "missing.sig");
        var mRefMissing = New(mType, ["MplEnv", "Sym", "String"], null, mName, "missing.sig")!;
        MethodComparison.Compare("MplForeignMacro.ExpandRef null environment", "Mpl.Run.MplForeignMacro",
            qExpandRef, qRefMissing, [null, qReference], mExpandRef, mRefMissing, [null, mReference], Show, Show, State, State);
    }

}
