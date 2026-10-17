using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class DynamicTypeConcat3MiDifferentialTests
{
    private const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    private static string Show(object? value) => value is MethodInfo method
        ? $"{MplRunSides.Describe(method.DeclaringType)}.{method.Name}({string.Join(",", method.GetParameters().Select(p => MplRunSides.Describe(p.ParameterType)))}):{MplRunSides.Describe(method.ReturnType)}:{method.IsStatic}"
        : MplRunSides.Describe(value);

    [SkippableFact]
    public void Concat3Mi_resolves_MplConcat3()
    {
        System.Type quireType = typeof(DynamicType);
        System.Type mathcadType = Mathcad.Type("mplrun", "Mpl.Run.DynamicType");
        MethodInfo quireMethod = quireType.GetMethod("Concat3Mi", Static)!;
        MethodInfo mathcadMethod = mathcadType.GetMethod("Concat3Mi", Static)!;

        MethodComparison.Compare("Concat3Mi", "Mpl.Run.DynamicType", quireMethod, null, new object[0], mathcadMethod, null, new object[0], Show, Show);
    }
}
