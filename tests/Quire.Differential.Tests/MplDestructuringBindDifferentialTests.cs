using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class MplDestructuringBindDifferentialTests
{
    private const string Contract = "Mpl.Run.MplDestructuringBind";
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void as_pattern_matches_destructuring_conversion()
    {
        Runtime.Sync();
        System.Type quireType = typeof(MplDestructuringBind);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("as_pattern", PublicStatic, [typeof(Sexpr)])!;
        MethodInfo originalMethod = originalType.GetMethod("as_pattern", PublicStatic, [M("Mpl.Run.Sexpr")])!;
        Tok symbol = new(new Sym("x", null), 1u);
        Sexpr[] inputs =
        [
            Nul.singleton,
            new Cm(symbol),
            new Ca(symbol),
            SCons.list2(symbol, new Ca(SCons.list1(symbol))),
            SCons.list2(symbol, SCons.list1(symbol)),
            new Qt(symbol),
            (Sexpr)null!
        ];

        foreach (Sexpr input in inputs)
        {
            object?[] quireArgs = [input];
            object?[] originalArgs = [ToMathcad(input)];
            MethodComparison.Compare("MplDestructuringBind.as_pattern", Contract,
                quireMethod, null, quireArgs, originalMethod, null, originalArgs,
                value => Describe(value), value => Describe(value));
        }
    }
}
