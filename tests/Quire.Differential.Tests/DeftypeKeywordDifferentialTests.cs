using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DeftypeKeywordDifferentialTests
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
    public void Singleton_factory_and_walks()
    {
        Runtime.Sync();
        var qType = typeof(deftype_keyword);
        var mType = Mathcad.Type("mplrun", "Mpl.Run.deftype_keyword");

        MethodComparison.Compare("deftype_keyword.cctor", "Mpl.Run.deftype_keyword",
            qType.TypeInitializer!, null, [], mType.TypeInitializer!, null, [],
            _ => Describe(Reflect.Get(qType, "singleton")), _ => Describe(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = mType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare("deftype_keyword.ctor", "Mpl.Run.deftype_keyword",
            qCtor, null, [], mCtor, null, [], o => Describe(o), o => Describe(o));

        var qSingleton = (deftype_keyword)Reflect.Get(qType, "singleton")!;
        var mSingleton = Reflect.Get(mType, "singleton")!;
        var qNewTok = qType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        var mNewTok = mType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 2u, uint.MaxValue })
        {
            MethodComparison.Compare($"deftype_keyword.NewTok({handle})", "Mpl.Run.deftype_keyword",
                qNewTok, null, [handle], mNewTok, null, [handle], o => Describe(o), o => Describe(o));
        }

        var qWalkDecl = qType.GetMethod("walk_declaration_as", DeclaredInstancePublic,
            [typeof(MplEnv), typeof(SCons)])!;
        var mWalkDecl = mType.GetMethod("walk_declaration_as", DeclaredInstancePublic,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qWalkDef = qType.GetMethod("walk_definition_as", DeclaredInstancePublic,
            [typeof(MplEnv), typeof(SCons)])!;
        var mWalkDef = mType.GetMethod("walk_definition_as", DeclaredInstancePublic,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;

        var qEnv = new LocalEnv();
        var mEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        var qHead = new Tok(qSingleton, 20u);
        var qValue = new Tok(new int_constant(42), 21u);
        var qForm = SCons.list2(qHead, qValue);
        var mTok = Mathcad.Type("mplrun", "Mpl.Run.Tok");
        var mAtom = Mathcad.Type("mplrun", "Mpl.Run.Atom");
        var mTokCtor = mTok.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            null, [mAtom, typeof(uint)], null)!;
        var mHead = mTokCtor.Invoke([mSingleton, 20u]);
        var mValue = ToMathcad(qValue)!;
        var mForm = MathcadList(mHead, mValue);

        MethodComparison.Compare("deftype_keyword.walk_declaration_as wraps form", "Mpl.Run.deftype_keyword",
            qWalkDecl, qSingleton, [qEnv, qForm], mWalkDecl, mSingleton, [mEnv, mForm],
            o => o is Cons cons && ReferenceEquals(cons.head, qForm) ? "wrapped original form" : Describe(o),
            o => o?.GetType().Name == "Cons" && ReferenceEquals(Reflect.Get(o!, "head"), mForm) ? "wrapped original form" : Describe(o),
            o => Describe(o), o => Describe(o));

        MethodComparison.Compare("deftype_keyword.walk_definition_as returns form", "Mpl.Run.deftype_keyword",
            qWalkDef, qSingleton, [qEnv, qForm], mWalkDef, mSingleton, [mEnv, mForm],
            o => ReferenceEquals(o, qForm) ? "reused form" : Describe(o),
            o => ReferenceEquals(o, mForm) ? "reused form" : Describe(o),
            o => Describe(o), o => Describe(o));
    }
}
