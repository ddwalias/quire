using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DefineKeywordDifferentialTests
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
    public void Singleton_factories_and_walks()
    {
        Runtime.Sync();
        var qType = typeof(define_keyword);
        var mType = Mathcad.Type("mplrun", "Mpl.Run.define_keyword");

        var qCctor = qType.TypeInitializer!;
        var mCctor = mType.TypeInitializer!;
        MethodComparison.Compare("define_keyword.cctor", "Mpl.Run.define_keyword",
            qCctor, null, [], mCctor, null, [],
            _ => Describe(Reflect.Get(qType, "singleton")), _ => Describe(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = mType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare("define_keyword.ctor", "Mpl.Run.define_keyword",
            qCtor, null, [], mCtor, null, [], o => Describe(o), o => Describe(o));

        var qSingleton = (define_keyword)Reflect.Get(qType, "singleton")!;
        var mSingleton = Reflect.Get(mType, "singleton")!;
        var qNewTok = qType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        var mNewTok = mType.GetMethod("NewTok", DeclaredStaticPublic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 17u, uint.MaxValue })
        {
            MethodComparison.Compare($"define_keyword.NewTok({handle})", "Mpl.Run.define_keyword",
                qNewTok, null, [handle], mNewTok, null, [handle], o => Describe(o), o => Describe(o));
        }

        var qMakeForms = qType.GetMethod("MakeDefine", DeclaredStaticPublic, [typeof(uint), typeof(Sexpr)])!;
        var mMakeForms = mType.GetMethod("MakeDefine", DeclaredStaticPublic,
            [typeof(uint), Mathcad.Type("mplrun", "Mpl.Run.Sexpr")])!;
        var qMakeParts = qType.GetMethod("MakeDefine", DeclaredStaticPublic,
            [typeof(uint), typeof(Sexpr), typeof(Sexpr)])!;
        var mMakeParts = mType.GetMethod("MakeDefine", DeclaredStaticPublic,
            [typeof(uint), Mathcad.Type("mplrun", "Mpl.Run.Sexpr"), Mathcad.Type("mplrun", "Mpl.Run.Sexpr")])!;
        var qPatternPart = new Tok(new Sym("x", null), 2u);
        var qDefinitionPart = new Tok(new int_constant(4), 3u);
        var qForms = SCons.list2(qPatternPart, qDefinitionPart);
        var mForms = ToMathcad(qForms)!;
        foreach (uint handle in new[] { 0u, 17u, uint.MaxValue })
        {
            MethodComparison.Compare($"define_keyword.MakeDefine(forms,{handle})", "Mpl.Run.define_keyword",
                qMakeForms, null, [handle, qForms], mMakeForms, null, [handle, mForms], o => Describe(o), o => Describe(o));
            MethodComparison.Compare($"define_keyword.MakeDefine(pat,def,{handle})", "Mpl.Run.define_keyword",
                qMakeParts, null, [handle, qForms.head, qForms.second], mMakeParts, null, [handle, ToMathcad(qForms.head), ToMathcad(qForms.second)],
                o => Describe(o), o => Describe(o));
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
        var qHead = new Tok(qSingleton, 10u);
        var qPattern = new Tok(new Sym("x", null), 11u);
        var mPattern = ToMathcad(qPattern)!;
        var qDefinition = new Tok(new int_constant(9), 12u);
        var mDefinition = ToMathcad(qDefinition)!;
        var qForm = SCons.list3(qHead, qPattern, qDefinition);
        var mTok = Mathcad.Type("mplrun", "Mpl.Run.Tok");
        var mAtom = Mathcad.Type("mplrun", "Mpl.Run.Atom");
        var mTokCtor = mTok.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            null, [mAtom, typeof(uint)], null)!;
        var mHead = mTokCtor.Invoke([mSingleton, 10u]);
        var mForm = MathcadList(mHead, mPattern, mDefinition);
        MethodComparison.Compare("define_keyword.walk_declaration_as reused pattern", "Mpl.Run.define_keyword",
            qWalkDecl, qSingleton, [qEnv, qForm], mWalkDecl, mSingleton, [mEnv, mForm],
            o => o is Cons cons && ReferenceEquals(cons.head, qForm) ? "reused form" : Describe(o),
            o => o?.GetType().Name == "Cons" && ReferenceEquals(Reflect.Get(o!, "head"), mForm) ? "reused form" : Describe(o),
            o => Describe(o), o => Describe(o));

        var qChangedPattern = SCons.list1(new Tok(new Sym("Pattern", null), 13u));
        var mChangedPattern = ToMathcad(qChangedPattern)!;
        var qChangedForm = SCons.list3(qHead, qChangedPattern, qDefinition);
        var mChangedForm = MathcadList(mHead, mChangedPattern, mDefinition);
        MethodComparison.Compare("define_keyword.walk_declaration_as changed pattern", "Mpl.Run.define_keyword",
            qWalkDecl, qSingleton, [qEnv, qChangedForm], mWalkDecl, mSingleton, [mEnv, mChangedForm],
            o => Describe(o), o => Describe(o), o => Describe(o), o => Describe(o));

        var qBadForm = SCons.list2(qHead, qPattern);
        var mBadForm = MathcadList(mHead, mPattern);
        MethodComparison.Compare("define_keyword.walk_declaration_as invalid length", "Mpl.Run.define_keyword",
            qWalkDecl, qSingleton, [qEnv, qBadForm], mWalkDecl, mSingleton, [mEnv, mBadForm],
            o => Describe(o), o => Describe(o), o => Describe(o), o => Describe(o));

        var qWalkDefEnv = new LocalEnv();
        var mWalkDefEnv = New(Mathcad.Type("mplrun", "Mpl.Run.LocalEnv"), [])!;
        var qUnchangedDef = new Tok(new int_constant(8), 14u);
        var qUnchangedDefForm = SCons.list3(qHead, qPattern, qUnchangedDef);
        var mUnchangedDefForm = MathcadList(mHead, mPattern, ToMathcad(qUnchangedDef)!);
        MethodComparison.Compare("define_keyword.walk_definition_as reused expression", "Mpl.Run.define_keyword",
            qWalkDef, qSingleton, [qWalkDefEnv, qUnchangedDefForm], mWalkDef, mSingleton, [mWalkDefEnv, mUnchangedDefForm],
            o => ReferenceEquals(o, qUnchangedDefForm) ? "reused definition form" : Describe(o),
            o => ReferenceEquals(o, mUnchangedDefForm) ? "reused definition form" : Describe(o),
            o => Describe(o), o => Describe(o));

        var qExpanded = SCons.list2(new Tok(new int_constant(1), 15u), new Tok(new int_constant(2), 16u));
        var mExpanded = ToMathcad(qExpanded)!;
        var qChangedDefForm = SCons.list3(qHead, qPattern, qExpanded);
        var mChangedDefForm = MathcadList(mHead, mPattern, mExpanded);
        MethodComparison.Compare("define_keyword.walk_definition_as changed expression", "Mpl.Run.define_keyword",
            qWalkDef, qSingleton, [qWalkDefEnv, qChangedDefForm], mWalkDef, mSingleton, [mWalkDefEnv, mChangedDefForm],
            o => Describe(o), o => Describe(o), o => Describe(o), o => Describe(o));
    }
}
