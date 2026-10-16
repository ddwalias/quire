using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class IfKeywordDifferentialTests
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
        var qType = typeof(if_keyword);
        var mType = Mathcad.Type("mplrun", "Mpl.Run.if_keyword");

        MethodComparison.Compare("if_keyword.cctor", "Mpl.Run.if_keyword",
            qType.TypeInitializer!, null, [], mType.TypeInitializer!, null, [],
            _ => Describe(Reflect.Get(qType, "singleton")), _ => Describe(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = mType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare("if_keyword.ctor", "Mpl.Run.if_keyword",
            qCtor, null, [], mCtor, null, [], o => Describe(o), o => Describe(o));

        var qSingleton = (if_keyword)Reflect.Get(qType, "singleton")!;
        var mSingleton = Reflect.Get(mType, "singleton")!;
        var qNewTok = qType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        var mNewTok = mType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 9u, uint.MaxValue })
        {
            MethodComparison.Compare($"if_keyword.NewTok({handle})", "Mpl.Run.if_keyword",
                qNewTok, null, [handle], mNewTok, null, [handle], o => Describe(o), o => Describe(o));
        }

        var qMakeForms = qType.GetMethod("MakeIf", DeclaredStaticPublic, [typeof(uint), typeof(Sexpr)])!;
        var mMakeForms = mType.GetMethod("MakeIf", DeclaredStaticPublic,
            [typeof(uint), Mathcad.Type("mplrun", "Mpl.Run.Sexpr")])!;
        var qMakeParts = qType.GetMethod("MakeIf", DeclaredStaticPublic,
            [typeof(uint), typeof(Sexpr), typeof(Sexpr), typeof(Sexpr)])!;
        var mMakeParts = mType.GetMethod("MakeIf", DeclaredStaticPublic,
            [typeof(uint), Mathcad.Type("mplrun", "Mpl.Run.Sexpr"), Mathcad.Type("mplrun", "Mpl.Run.Sexpr"), Mathcad.Type("mplrun", "Mpl.Run.Sexpr")])!;

        var qCond = new Tok(new int_constant(1), 30u);
        var qThen = new Tok(new int_constant(2), 31u);
        var qElse = new Tok(new int_constant(3), 32u);
        var qForms = SCons.list3(qCond, qThen, qElse);
        var mCond = ToMathcad(qCond)!;
        var mThen = ToMathcad(qThen)!;
        var mElse = ToMathcad(qElse)!;
        var mForms = MathcadList(mCond, mThen, mElse);
        foreach (uint handle in new[] { 0u, 9u, uint.MaxValue })
        {
            MethodComparison.Compare($"if_keyword.MakeIf(forms,{handle})", "Mpl.Run.if_keyword",
                qMakeForms, null, [handle, qForms], mMakeForms, null, [handle, mForms], o => Describe(o), o => Describe(o));
            MethodComparison.Compare($"if_keyword.MakeIf(parts,{handle})", "Mpl.Run.if_keyword",
                qMakeParts, null, [handle, qCond, qThen, qElse], mMakeParts, null, [handle, mCond, mThen, mElse],
                o => Describe(o), o => Describe(o));
        }

        var qWalk = qType.GetMethod("walk_expression_as", DeclaredInstancePublic,
            [typeof(MplEnv), typeof(SCons)])!;
        var mWalk = mType.GetMethod("walk_expression_as", DeclaredInstancePublic,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qEnv = new LocalEnv();
        var mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        var qHead = new Tok(qSingleton, 33u);
        var qForm = SCons.list3(qHead, qCond, qThen);
        var mTok = Mathcad.Type("mplrun", "Mpl.Run.Tok");
        var mAtom = Mathcad.Type("mplrun", "Mpl.Run.Atom");
        var mTokCtor = mTok.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            null, [mAtom, typeof(uint)], null)!;
        var mHead = mTokCtor.Invoke([mSingleton, 33u]);
        var mForm = MathcadList(mHead, mCond, mThen);
        MethodComparison.Compare("if_keyword.walk_expression_as", "Mpl.Run.if_keyword",
            qWalk, qSingleton, [qEnv, qForm], mWalk, mSingleton, [mEnv, mForm],
            o => ReferenceEquals(o, qForm) ? "reused input form" : Describe(o),
            o => ReferenceEquals(o, mForm) ? "reused input form" : Describe(o),
            o => Describe(o), o => Describe(o));
    }
}
