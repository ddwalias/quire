using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DefdataKeywordDifferentialTests
{
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void Singleton_tokens_and_declaration_walks()
    {
        Runtime.Sync();
        var qType = typeof(defdata_keyword);
        var mType = Mathcad.Type("mplrun", "Mpl.Run.defdata_keyword");

        var qCctor = qType.TypeInitializer!;
        var mCctor = mType.TypeInitializer!;
        MethodComparison.Compare("defdata_keyword.cctor", "Mpl.Run.defdata_keyword",
            qCctor, null, [], mCctor, null, [],
            _ => Describe(Reflect.Get(qType, "singleton")), _ => Describe(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(Type.EmptyTypes)!;
        var mCtor = mType.GetConstructor(Type.EmptyTypes)!;
        MethodComparison.Compare("defdata_keyword.ctor", "Mpl.Run.defdata_keyword",
            qCtor, null, [], mCtor, null, [], o => Describe(o), o => Describe(o));

        var qNewTok = qType.GetMethod("NewTok", BindingFlags.Public | BindingFlags.Static)!;
        var mNewTok = mType.GetMethod("NewTok", BindingFlags.Public | BindingFlags.Static)!;
        foreach (uint handle in new[] { 0u, 1u, 37u, uint.MaxValue })
        {
            MethodComparison.Compare($"defdata_keyword.NewTok({handle})", "Mpl.Run.defdata_keyword",
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

        var qSingleton = (defdata_keyword)Reflect.Get(qType, "singleton")!;
        var mSingleton = Reflect.Get(mType, "singleton")!;
        var qForm = SCons.list2(new Tok(new int_constant(42), 12u), new Tok(new Sym("name", null), 13u));
        var mForm = ToMathcad(qForm)!;
        MethodComparison.Compare("defdata_keyword.walk_declaration_as", "Mpl.Run.defdata_keyword",
            qWalkDecl, qSingleton, [null, qForm], mWalkDecl, mSingleton, [null, mForm], o => Describe(o), o => Describe(o),
            o => Describe(o), o => Describe(o));
        MethodComparison.Compare("defdata_keyword.walk_declaration_as null", "Mpl.Run.defdata_keyword",
            qWalkDecl, qSingleton, [null, null], mWalkDecl, mSingleton, [null, null], o => Describe(o), o => Describe(o),
            o => Describe(o), o => Describe(o));
        MethodComparison.Compare("defdata_keyword.walk_definition_as", "Mpl.Run.defdata_keyword",
            qWalkDef, qSingleton, [null, qForm], mWalkDef, mSingleton, [null, mForm], o => Describe(o), o => Describe(o),
            o => Describe(o), o => Describe(o));
        MethodComparison.Compare("defdata_keyword.walk_definition_as null", "Mpl.Run.defdata_keyword",
            qWalkDef, qSingleton, [null, null], mWalkDef, mSingleton, [null, null], o => Describe(o), o => Describe(o),
            o => Describe(o), o => Describe(o));
    }
}
