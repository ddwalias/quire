using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class DynamicTypeGetShortNameDifferentialTests
{
    private const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    private static string Show(object? value) => MplRunSides.Describe(value);

    private static void Compare(MethodInfo quireMethod, MethodInfo mathcadMethod, string caseId, string? value)
    {
        object[] quireArgs = new object[] { value! };
        object[] mathcadArgs = new object[] { value! };
        MethodComparison.Compare(caseId, "Mpl.Run.DynamicType", quireMethod, null, quireArgs,
            mathcadMethod, null, mathcadArgs, Show, Show);
    }

    [SkippableFact]
    public void GetShortName_preserves_colon_and_dot_rules()
    {
        System.Type quireType = typeof(DynamicType);
        System.Type mathcadType = Mathcad.Type("mplrun", "Mpl.Run.DynamicType");
        MethodInfo quireMethod = quireType.GetMethod("GetShortName", Static)!;
        MethodInfo mathcadMethod = mathcadType.GetMethod("GetShortName", Static)!;

        Compare(quireMethod, mathcadMethod, "colon after first character", "ns:Thing.Part");
        Compare(quireMethod, mathcadMethod, "colon after one character", "x:Thing");
        Compare(quireMethod, mathcadMethod, "colon at first character", ":Thing");
        Compare(quireMethod, mathcadMethod, "leading colon before a later dot", ":Thing.Part");
        Compare(quireMethod, mathcadMethod, "namespace suffix", "Mpl.Run.DynamicType");
        Compare(quireMethod, mathcadMethod, "dot after one character", "x.Y");
        Compare(quireMethod, mathcadMethod, "dot at first character", ".Thing");
        Compare(quireMethod, mathcadMethod, "trailing dot", "ns.Type.");
        Compare(quireMethod, mathcadMethod, "plain name", "plain");
        Compare(quireMethod, mathcadMethod, "null name", null);
    }
}
