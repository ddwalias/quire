using System.Globalization;
using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class DynamicTypeExplodeDifferentialTests
{
    private const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    private static string Show(object? value)
    {
        if (value is not string[] parts)
        {
            return MplRunSides.Describe(value);
        }
        return parts.Length.ToString(CultureInfo.InvariantCulture) + ":" + string.Concat(parts.Select(part =>
            part == null ? "N;" : "S" + part.Length.ToString(CultureInfo.InvariantCulture) + ":" + part));
    }

    private static void Compare(MethodInfo quireMethod, MethodInfo mathcadMethod, string caseId, string? value)
    {
        object[] quireArgs = new object[] { value! };
        object[] mathcadArgs = new object[] { value! };
        MethodComparison.Compare(caseId, "Mpl.Run.DynamicType", quireMethod, null, quireArgs,
            mathcadMethod, null, mathcadArgs, Show, Show);
    }

    [SkippableFact]
    public void Explode_splits_on_underscore_and_preserves_empty_entries()
    {
        System.Type quireType = typeof(DynamicType);
        System.Type mathcadType = Mathcad.Type("mplrun", "Mpl.Run.DynamicType");
        MethodInfo quireMethod = quireType.GetMethod("Explode", Static)!;
        MethodInfo mathcadMethod = mathcadType.GetMethod("Explode", Static)!;

        Compare(quireMethod, mathcadMethod, "middle delimiter", "I_F");
        Compare(quireMethod, mathcadMethod, "comma inside an element", "I,_F");
        Compare(quireMethod, mathcadMethod, "repeated and trailing delimiters", "I__O_");
        Compare(quireMethod, mathcadMethod, "single trailing delimiter", "F_");
        Compare(quireMethod, mathcadMethod, "empty input", "");
        Compare(quireMethod, mathcadMethod, "no delimiter", "O");
        Compare(quireMethod, mathcadMethod, "null input", null);
    }
}
