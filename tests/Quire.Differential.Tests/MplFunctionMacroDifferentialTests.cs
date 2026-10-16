using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class MplFunctionMacroDifferentialTests
{
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static string MacroState(object macro)
    {
        object? expander = Get(macro, "m_expander");
        if (expander == null) return Describe(Get(macro, "m_name")) + ";expander=null";
        return Describe(Get(macro, "m_name")) + ";expander=" + Describe(Get(expander, "Purity")) + "," +
            Describe(Get(expander, "MinArgs")) + "," + Describe(Get(expander, "Varargs"));
    }

    private static object? OriginalFunction(Delegate function, int purity) =>
        New(M("Mpl.Run.DelegateFunction"), ["Delegate", "Int32"], function, purity);

    private static object ThrowQuire(object env, object form) =>
        throw new AppException("inner", 6u, 0x00070008u);

    private static object ThrowOriginal(object env, object form) =>
        throw (Exception)New(M("Mpl.Run.AppException"), ["String", "UInt32", "UInt32"],
            "inner", 6u, 0x00070008u)!;

    [SkippableFact]
    public void String_constructor_and_factory()
    {
        Runtime.Sync();
        var qCtor = typeof(MplFunctionMacro).GetConstructor(DeclaredInstancePublic, null,
            [typeof(string), typeof(Function)], null)!;
        var mType = M("Mpl.Run.MplFunctionMacro");
        var mCtor = mType.GetConstructor(DeclaredInstancePublic, null,
            [typeof(string), M("Mpl.Run.Function")], null)!;
        var qMake = typeof(MplFunctionMacro).GetMethod("make", DeclaredStaticPublic,
            [typeof(string), typeof(Function)])!;
        var mMake = mType.GetMethod("make", DeclaredStaticPublic,
            [typeof(string), M("Mpl.Run.Function")])!;
        Func<object?, string> show = value => Describe(value);
        Func<object, string> state = MacroState;

        foreach (string? name in new[] { "macro", "", null })
        {
            MethodComparison.Compare($"MplFunctionMacro(string={Describe(name)}, null)", "Mpl.Run.MplFunctionMacro",
                qCtor, null, [name, null], mCtor, null, [name, null], show, show);

            MethodComparison.Compare($"MplFunctionMacro.make({Describe(name)}, null)", "Mpl.Run.MplFunctionMacro",
                qMake, null, [name, null], mMake, null, [name, null], show, show);
        }
    }

    [SkippableFact]
    public void ExpandCall_returns_the_result_and_wraps_AppException()
    {
        Runtime.Sync();
        var qMethod = typeof(MplFunctionMacro).GetMethod(nameof(MplFunctionMacro.ExpandCall), DeclaredInstancePublic,
            [typeof(MplEnv), typeof(Sexpr)])!;
        var mMethod = M("Mpl.Run.MplFunctionMacro").GetMethod(nameof(MplFunctionMacro.ExpandCall), DeclaredInstancePublic,
            [M("Mpl.Run.MplEnv"), M("Mpl.Run.Sexpr")])!;
        Func<object?, string> show = value => Describe(value);
        Func<object, string> state = MacroState;
        Sexpr qForm = int_constant.NewTok(17, 0x00030004u);
        object mForm = ToMathcad(qForm)!;

        Func<object, object, object> identity = (_, form) => form;
        Function qSuccessFunction = DelegateFunction.make(identity, 2);
        object mSuccessFunction = OriginalFunction(identity, 2)!;
        var qSuccess = new MplFunctionMacro("id", qSuccessFunction);
        object mSuccess = New(M("Mpl.Run.MplFunctionMacro"), ["String", "Function"], "id", mSuccessFunction)!;
        MethodComparison.Compare("ExpandCall returns the expander result", "Mpl.Run.MplFunctionMacro",
            qMethod, qSuccess, [null, qForm], mMethod, mSuccess, [null, mForm], show, show, state, state);

        F<object, object, object> qThrow = ThrowQuire;
        System.Type mDelegateType = M("Mpl.Run.F`3").MakeGenericType(typeof(object), typeof(object), typeof(object));
        Delegate mThrow = Delegate.CreateDelegate(mDelegateType,
            typeof(MplFunctionMacroDifferentialTests).GetMethod(nameof(ThrowOriginal),
                BindingFlags.NonPublic | BindingFlags.Static)!);
        Function qThrowFunction = DelegateFunction.make(qThrow, 2);
        object mThrowFunction = OriginalFunction(mThrow, 2)!;
        var qThrowing = new MplFunctionMacro("throw", qThrowFunction);
        object mThrowing = New(M("Mpl.Run.MplFunctionMacro"), ["String", "Function"], "throw", mThrowFunction)!;
        MethodComparison.Compare("ExpandCall wraps the AppException at the form handle", "Mpl.Run.MplFunctionMacro",
            qMethod, qThrowing, [null, qForm], mMethod, mThrowing, [null, mForm], show, show, state, state);
    }
}
