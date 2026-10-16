using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class DoTryKeywordDifferentialTests
{
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private static string View(object? value) => Describe(value);

    private static void CompareKeyword(string name, System.Type quireType, int keynum)
    {
        var mathcadType = Mathcad.Type("mplrun", "Mpl.Run." + name);
        string contract = "Mpl.Run." + name;

        MethodComparison.Compare(name + ".cctor", contract, quireType.TypeInitializer!, null, [],
            mathcadType.TypeInitializer!, null, [],
            quireShow: _ => Describe(Reflect.Get(quireType, "singleton")),
            originalShow: _ => Describe(Reflect.Get(mathcadType, "singleton")),
            quireState: _ => Describe(Reflect.Get(quireType, "singleton")),
            originalState: _ => Describe(Reflect.Get(mathcadType, "singleton")));

        var qCtor = quireType.GetConstructor(PublicInstance, null, [], null)!;
        var mCtor = mathcadType.GetConstructor(PublicInstance, null, [], null)!;
        MethodComparison.Compare(name + "()", contract, qCtor, null, [], mCtor, null, [],
            quireShow: View, originalShow: View);

        var qNewTok = quireType.GetMethod("NewTok", PublicStatic, [typeof(uint)])!;
        var mNewTok = mathcadType.GetMethod("NewTok", PublicStatic, [typeof(uint)])!;
        foreach (uint handle in new[] { 0u, 17u, uint.MaxValue })
            MethodComparison.Compare("NewTok(" + handle + ")", contract, qNewTok, null, [handle], mNewTok, null, [handle],
                quireShow: View, originalShow: View);

        object qSingleton = Reflect.Get(quireType, "singleton")!;
        object mSingleton = Reflect.Get(mathcadType, "singleton")!;
        Assert.Equal(keynum, Reflect.Get(qSingleton, "KeyNum"));
        Assert.Equal(keynum, Reflect.Get(mSingleton, "KeyNum"));

        var qWalk = quireType.GetMethod("walk_expression_as", PublicInstance, [typeof(MplEnv), typeof(SCons)])!;
        var mWalk = mathcadType.GetMethod("walk_expression_as", PublicInstance,
            [Mathcad.Type("mplrun", "Mpl.Run.MplEnv"), Mathcad.Type("mplrun", "Mpl.Run.SCons")])!;
        var qForm = new SCons(new Tok(new Sym("form", null), 3u), Nul.singleton);
        var mForm = ToMathcad(qForm)!;
        MethodComparison.Compare("walk_expression_as throws not implemented", contract,
            qWalk, Activator.CreateInstance(quireType), [null, qForm], mWalk, Activator.CreateInstance(mathcadType), [null, mForm],
            quireShow: View, originalShow: View, quireState: View, originalState: View);
    }

    [SkippableFact]
    public void Singleton_constructors_tokens_and_unimplemented_walks()
    {
        Runtime.Sync();
        CompareKeyword(nameof(do_keyword), typeof(do_keyword), 11);
        CompareKeyword(nameof(try_keyword), typeof(try_keyword), 12);
    }
}
