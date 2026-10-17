using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class DynamicTypeMplConcat3DifferentialTests
{
    [SkippableFact]
    public void null_arguments_are_replaced_with_the_literal_null()
    {
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M("Mpl.Run.DynamicType");
        MethodInfo quireMethod = quireType.GetMethod(nameof(DynamicType.MplConcat3))!;
        MethodInfo originalMethod = originalType.GetMethod(nameof(DynamicType.MplConcat3))!;

        foreach (object?[] args in new[]
        {
            new object?[] { null, null, null },
            new object?[] { null, "b", "c" },
            new object?[] { "a", null, "c" },
            new object?[] { "a", "b", null },
            new object?[] { "null", 17, 2.5 },
            new object?[] { 1, false, "x" },
        })
        {
            MethodComparison.Compare("DynamicType.MplConcat3", "Mpl.Run.DynamicType",
                quireMethod, null, args, originalMethod, null, args);
        }
    }
}
