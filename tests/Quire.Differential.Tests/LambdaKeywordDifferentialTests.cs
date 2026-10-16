using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class LambdaKeywordDifferentialTests
{
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static object MathcadList(params object[] items)
    {
        var mSCons = Mathcad.Type("mplrun", "Mpl.Run.SCons");
        var mSexpr = Mathcad.Type("mplrun", "Mpl.Run.Sexpr");
        var ctor = mSCons.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            null, [mSexpr, mSexpr], null)!;
        object tail = Reflect.Get(Mathcad.Type("mplrun", "Mpl.Run.Nul"), "singleton")!;
        for (int i = items.Length - 1; i >= 0; i--)
        {
            tail = ctor.Invoke([items[i], tail]);
        }
        return tail;
    }

    [SkippableFact]
    public void Singleton_factories_and_walk()
    {
        Runtime.Sync();
        var qType = typeof(lambda_keyword);
        var mType = Mathcad.Type("mplrun", "Mpl.Run.lambda_keyword");

        MethodComparison.Compare("lambda_keyword.cctor", "Mpl.Run.lambda_keyword",
            qType.TypeInitializer!, null, [], mType.TypeInitializer!, null, [],
            _ => Describe(Reflect.Get(qType, "singleton")), _ => Describe(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = mType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare("lambda_keyword.ctor", "Mpl.Run.lambda_keyword",
            qCtor, null, [], mCtor, null, [], o => Describe(o), o => Describe(o));

        var qSingleton = (lambda_keyword)Reflect.Get(qType, "singleton")!;
        var mSingleton = Reflect.Get(mType, "singleton")!;
        var qNewTok = qType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        var mNewTok = mType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 6u, uint.MaxValue })
        {
            MethodComparison.Compare($"lambda_keyword.NewTok({handle})", "Mpl.Run.lambda_keyword",
                qNewTok, null, [handle], mNewTok, null, [handle], o => Describe(o), o => Describe(o));
        }

        var qMakeForms = qType.GetMethod("MakeLambda", DeclaredStaticPublic, [typeof(uint), typeof(Sexpr)])!;
        var mSexpr = Mathcad.Type("mplrun", "Mpl.Run.Sexpr");
        var mMakeForms = mType.GetMethod("MakeLambda", DeclaredStaticPublic, [typeof(uint), mSexpr])!;
        var qMakeParts = qType.GetMethod("MakeLambda", DeclaredStaticPublic,
            [typeof(uint), typeof(Sexpr), typeof(Sexpr)])!;
        var mMakeParts = mType.GetMethod("MakeLambda", DeclaredStaticPublic, [typeof(uint), mSexpr, mSexpr])!;
        var qSymbol = new Tok(new Sym("x", null), 61u);
        var qPats = SCons.list1(qSymbol);
        var qBody = new Tok(new Sym("x", null), 62u);
        var mPats = ToMathcad(qPats)!;
        var mBody = ToMathcad(qBody)!;
        foreach (uint handle in new[] { 0u, 6u, uint.MaxValue })
        {
            var qForms = SCons.list2(qPats, qBody);
            var mForms = MathcadList(mPats, mBody);
            MethodComparison.Compare($"lambda_keyword.MakeLambda(forms,{handle})", "Mpl.Run.lambda_keyword",
                qMakeForms, null, [handle, qForms], mMakeForms, null, [handle, mForms], o => Describe(o), o => Describe(o));
            MethodComparison.Compare($"lambda_keyword.MakeLambda(parts,{handle})", "Mpl.Run.lambda_keyword",
                qMakeParts, null, [handle, qPats, qBody], mMakeParts, null, [handle, mPats, mBody],
                o => Describe(o), o => Describe(o));
        }

        var qWalk = qType.GetMethod("walk_expression_as", DeclaredInstancePublic,
            [typeof(MplEnv), typeof(SCons)])!;
        var mWalk = mType.GetMethod("walk_expression_as", DeclaredInstancePublic,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qEnv = new LocalEnv();
        var mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        var qForm = new SCons(new Tok(qSingleton, 70u), SCons.list2(qPats, qBody));
        var mTok = Mathcad.Type("mplrun", "Mpl.Run.Tok");
        var mAtom = Mathcad.Type("mplrun", "Mpl.Run.Atom");
        var mTokCtor = mTok.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            null, [mAtom, typeof(uint)], null)!;
        var mForm = MathcadList(mTokCtor.Invoke([mSingleton, 70u]), mPats, mBody);
        MethodComparison.Compare("lambda_keyword.walk_expression_as", "Mpl.Run.lambda_keyword",
            qWalk, qSingleton, [qEnv, qForm], mWalk, mSingleton, [mEnv, mForm],
            o => ReferenceEquals(o, qForm) ? "reused input form" : Describe(o),
            o => ReferenceEquals(o, mForm) ? "reused input form" : Describe(o),
            o => Describe(o), o => Describe(o));
    }
}
