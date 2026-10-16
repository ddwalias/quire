using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class LocallyKeywordDifferentialTests
{
    private const BindingFlags StaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static System.Type MathcadType(System.Type type) =>
        Mathcad.Type("mplrun", type.FullName![6..]);

    private static System.Type MathcadParameter(System.Type type) =>
        type.FullName?.StartsWith("Quire.", StringComparison.Ordinal) == true ? Mathcad.Type("mplrun", type.FullName[6..]) : type;

    private static (MethodInfo q, MethodInfo m) Method(string name, params System.Type[] parameters)
    {
        var q = typeof(locally_keyword).GetMethod(name, StaticPublic, parameters)!;
        var m = MathcadType(typeof(locally_keyword)).GetMethod(name, StaticPublic, parameters.Select(MathcadParameter).ToArray())!;
        return (q, m);
    }

    [SkippableFact]
    public void singleton_construction_factories_and_forms()
    {
        Runtime.Sync();

        MethodComparison.Compare("locally_keyword.cctor", "Mpl.Run.locally_keyword",
            typeof(locally_keyword).TypeInitializer!, null, [],
            MathcadType(typeof(locally_keyword)).TypeInitializer!, null, [],
            o => Describe(o), o => Describe(o));

        var qSingleton = locally_keyword.singleton;
        var mSingleton = Get(MathcadType(typeof(locally_keyword)), "singleton")!;
        Assert.Equal(Describe(mSingleton), Describe(qSingleton));

        var (qNewTok, mNewTok) = Method("NewTok", typeof(uint));
        foreach (uint handle in new[] { 0u, 1u, 42u, uint.MaxValue })
            Compare($"locally_keyword.NewTok({handle})", qNewTok, [handle], mNewTok, [handle]);

        var (qMakeForms, mMakeForms) = Method("MakeLocally", typeof(uint), typeof(Sexpr));
        Sexpr forms = SCons.list1(new Tok(new int_constant(4), 12u));
        Compare("locally_keyword.MakeLocally(forms)", qMakeForms, [13u, forms], mMakeForms, [13u, ToMathcad(forms)]);
        Compare("locally_keyword.MakeLocally(null)", qMakeForms, [14u, null], mMakeForms, [14u, null]);

        var (qMakeDefsBody, mMakeDefsBody) = Method("MakeLocally", typeof(uint), typeof(Sexpr), typeof(Sexpr));
        Sexpr defs = SCons.list1(new Tok(new Sym("x", null), 15u));
        Sexpr body = new Tok(new int_constant(9), 16u);
        Compare("locally_keyword.MakeLocally(defs, body)", qMakeDefsBody, [17u, defs, body],
            mMakeDefsBody, [17u, ToMathcad(defs), ToMathcad(body)]);

        var (qMakeConsBody, mMakeConsBody) = Method("MakeLocally", typeof(uint), typeof(Cons), typeof(Sexpr));
        Cons cons = new Cons(new Tok(new int_constant(5), 18u), null);
        Compare("locally_keyword.MakeLocally(cons, body)", qMakeConsBody, [19u, cons, body],
            mMakeConsBody, [19u, ToMathcad(cons), ToMathcad(body)]);

        var qEnv = new LocalEnv();
        var mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        System.Type mSCons = Mathcad.Type("mplrun", "Mpl.Run.SCons");
        object mNul = Get(Mathcad.Type("mplrun", "Mpl.Run.Nul"), "singleton")!;
        object mBody = ToMathcad(body)!;
        var (qWalk, mWalk) = (typeof(locally_keyword).GetMethod("walk_expression_as", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!,
            MathcadType(typeof(locally_keyword)).GetMethod("walk_expression_as", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!);
        object? mSingletonObject = mSingleton;

        SCons form = SCons.list3(locally_keyword.NewTok(20u), Nul.singleton, body);
        object mTok = Call(MathcadType(typeof(locally_keyword)), null, "NewTok", ["UInt32"], [20u])!;
        object mForm = Call(mSCons, null, "list3", ["Sexpr", "Sexpr", "Sexpr"], [mTok, mNul, mBody])!;
        Compare("locally_keyword.walk_expression_as empty block", qWalk, qSingleton, [qEnv, form], mWalk,
            mSingletonObject, [mEnv, mForm]);

        SCons malformed = SCons.list2(locally_keyword.NewTok(21u), body);
        object mMalformedTok = Call(MathcadType(typeof(locally_keyword)), null, "NewTok", ["UInt32"], [21u])!;
        object mMalformed = Call(mSCons, null, "list2", ["Sexpr", "Sexpr"], [mMalformedTok, mBody])!;
        Compare("locally_keyword.walk_expression_as malformed form", qWalk, qSingleton, [qEnv, malformed], mWalk,
            mSingletonObject, [mEnv, mMalformed]);
    }

    private static void Compare(string caseId, MethodBase q, object?[] qArgs, MethodBase m, object?[] mArgs) =>
        Compare(caseId, q, null, qArgs, m, null, mArgs);

    private static void Compare(string caseId, MethodBase q, object? qTarget, object?[] qArgs, MethodBase m, object? mTarget, object?[] mArgs) =>
        MethodComparison.Compare(caseId, "Mpl.Run.locally_keyword", q, qTarget, qArgs, m, mTarget, mArgs,
            o => Describe(o), o => Describe(o), o => Describe(o), o => Describe(o));
}
