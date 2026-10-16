using System.Reflection;
using Quire.Mpl.Run;
using Xunit;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class CaseKeywordDifferentialTests
{
    private const string Contract = "Mpl.Run.case_keyword";
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type MathcadType => Mathcad.Type("mplrun", "Mpl.Run.case_keyword");

    private static string View(object? value) => MplRunSides.Describe(value);

    [SkippableFact]
    public void Singleton_constructor_tokens_and_unimplemented_walk()
    {
        Runtime.Sync();
        var qType = typeof(case_keyword);
        var mType = MathcadType;

        var qCctor = qType.TypeInitializer!;
        var mCctor = mType.TypeInitializer!;
        MethodComparison.Compare("case_keyword.cctor", Contract,
            qCctor, null, [], mCctor, null, [],
            quireShow: _ => View(Reflect.Get(qType, "singleton")), originalShow: _ => View(Reflect.Get(mType, "singleton")),
            quireState: _ => View(Reflect.Get(qType, "singleton")), originalState: _ => View(Reflect.Get(mType, "singleton")));

        var qCtor = qType.GetConstructor(PublicInstance, null, [], null)!;
        var mCtor = mType.GetConstructor(PublicInstance, null, [], null)!;
        MethodComparison.Compare("case_keyword()", Contract, qCtor, null, [], mCtor, null, [],
            quireShow: View, originalShow: View);

        var qNewTok = qType.GetMethod("NewTok", PublicStatic, [typeof(uint)])!;
        var mNewTok = mType.GetMethod("NewTok", PublicStatic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 17u, uint.MaxValue })
            MethodComparison.Compare($"NewTok({handle})", Contract, qNewTok, null, [handle], mNewTok, null, [handle],
                quireShow: View, originalShow: View, quireState: View, originalState: View);

        var qWalk = qType.GetMethod("walk_expression_as", PublicInstance, [typeof(MplEnv), typeof(SCons)])!;
        var mWalk = mType.GetMethod("walk_expression_as", PublicInstance,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qForm = new SCons(new Tok(new Sym("form", null), 3u), Nul.singleton);
        var mForm = ToMathcad(qForm)!;
        MethodComparison.Compare("walk_expression_as throws not implemented", Contract,
            qWalk, new case_keyword(), [null, qForm], mWalk, Activator.CreateInstance(mType), [null, mForm],
            quireShow: View, originalShow: View, quireState: View, originalState: View);
    }
}
