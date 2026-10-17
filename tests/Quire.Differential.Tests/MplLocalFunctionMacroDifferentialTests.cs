using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class MplLocalFunctionMacroDifferentialTests
{
    private static readonly BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic
        | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static readonly Func<object?, string> Show = value => Describe(value);
    private static readonly Func<object, string> State = value => Describe(value);

    private static MethodInfo Method(System.Type type, string name, params System.Type[] arguments) =>
        type.GetMethod(name, DeclaredInstance, null, arguments, null)
        ?? throw new HarnessException($"Method not found: {type.FullName}.{name}");

    [SkippableFact]
    public void MplLocalFunctionMacro_members()
    {
        Runtime.Sync();

        System.Type qType = typeof(MplLocalFunctionMacro);
        System.Type mType = M("Mpl.Run.MplLocalFunctionMacro");
        System.Type qEnvType = typeof(MplEnv);
        System.Type mEnvType = M("Mpl.Run.MplEnv");
        System.Type qSexprType = typeof(Sexpr);
        System.Type mSexprType = M("Mpl.Run.Sexpr");
        var qCtor = qType.GetConstructor(DeclaredInstance, null,
            [qEnvType, typeof(Sym), typeof(uint), qSexprType], null)!;
        var mCtor = mType.GetConstructor(DeclaredInstance, null,
            [mEnvType, M("Mpl.Run.Sym"), typeof(uint), mSexprType], null)!;
        var qExpandCall = Method(qType, "ExpandCall", qEnvType, qSexprType);
        var mExpandCall = Method(mType, "ExpandCall", mEnvType, mSexprType);
        var qExpandRef = Method(qType, "ExpandRef", qEnvType, typeof(Tok));
        var mExpandRef = Method(mType, "ExpandRef", mEnvType, M("Mpl.Run.Tok"));

        var qName = new Sym("local", null);
        object mName = ToMathcad(qName)!;
        Sexpr qLambda = int_constant.NewTok(23, 0x00030002u);
        object mLambda = ToMathcad(qLambda)!;
        MethodComparison.Compare("MplLocalFunctionMacro.ctor", "Mpl.Run.MplLocalFunctionMacro",
            qCtor, null, [null, qName, 0x12345678u, qLambda],
            mCtor, null, [null, mName, 0x12345678u, mLambda], Show, Show);

        Sexpr qForm = int_constant.NewTok(17, 0x00030004u);
        object mForm = ToMathcad(qForm)!;
        var qCallMacro = new MplLocalFunctionMacro(null, qName, 3u, qLambda);
        object mCallMacro = New(mType, ["MplEnv", "Sym", "UInt32", "Sexpr"], null, mName, 3u, mLambda)!;
        MethodComparison.Compare("MplLocalFunctionMacro.ExpandCall throws at the form handle", "Mpl.Run.MplLocalFunctionMacro",
            qExpandCall, qCallMacro, [null, qForm], mExpandCall, mCallMacro, [null, mForm], Show, Show, State, State);

        var qReference = new Tok(qName, 0x00050006u);
        object mReference = ToMathcad(qReference)!;
        var qRefMacro = new MplLocalFunctionMacro(null, qName, 3u, qLambda);
        object mRefMacro = New(mType, ["MplEnv", "Sym", "UInt32", "Sexpr"], null, mName, 3u, mLambda)!;
        MethodComparison.Compare("MplLocalFunctionMacro.ExpandRef throws at the token handle", "Mpl.Run.MplLocalFunctionMacro",
            qExpandRef, qRefMacro, [null, qReference], mExpandRef, mRefMacro, [null, mReference], Show, Show, State, State);
    }
}
