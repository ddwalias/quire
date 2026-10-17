using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class DynamicTypeConcat2MiDifferentialTests
{
    private const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    private static string Show(object? value) => value is MethodInfo method
        ? $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName))}):{method.ReturnType.FullName}:{method.IsStatic}"
        : MplRunSides.Describe(value);

    [SkippableFact]
    public void Concat2Mi_resolves_string_concat_object_overload()
    {
        System.Type quireType = typeof(DynamicType);
        System.Type mathcadType = Mathcad.Type("mplrun", "Mpl.Run.DynamicType");
        MethodInfo quireMethod = quireType.GetMethod("Concat2Mi", Static)!;
        MethodInfo mathcadMethod = mathcadType.GetMethod("Concat2Mi", Static)!;

        MethodComparison.Compare("Concat2Mi", "Mpl.Run.DynamicType", quireMethod, null, new object[0], mathcadMethod, null, new object[0], Show, Show);
    }
}
