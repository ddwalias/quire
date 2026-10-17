using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class DynamicTypeNameMangleDifferentialTests
{
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static void Compare(MethodInfo quireMethod, MethodInfo mathcadMethod, string caseId, string? prefix, System.Type[]? types)
    {
        object?[] quireArgs = new object?[] { prefix, types };
        object?[] mathcadArgs = new object?[] { prefix, types };
        MethodComparison.Compare(caseId, "Mpl.Run.DynamicType", quireMethod, null, quireArgs,
            mathcadMethod, null, mathcadArgs);
    }

    [SkippableFact]
    public void NameMangle_matches_prefix_and_type_name_mappings()
    {
        System.Type quireType = typeof(DynamicType);
        System.Type mathcadType = Mathcad.Type("mplrun", "Mpl.Run.DynamicType");
        MethodInfo quireMethod = quireType.GetMethod("NameMangle", PrivateStatic,
            new System.Type[] { typeof(string), typeof(System.Type[]) })!;
        MethodInfo mathcadMethod = mathcadType.GetMethod("NameMangle", PrivateStatic,
            new System.Type[] { typeof(string), typeof(System.Type[]) })!;

        Compare(quireMethod, mathcadMethod, "empty type list preserves prefix", "T_", new System.Type[0]);
        Compare(quireMethod, mathcadMethod, "known types", "T_", new System.Type[] { typeof(int), typeof(double), typeof(object) });
        Compare(quireMethod, mathcadMethod, "unknown type fallback", "T_", new System.Type[] { typeof(string) });
        Compare(quireMethod, mathcadMethod, "mixed mapped and unknown types", "T_", new System.Type[] { typeof(int), typeof(string), typeof(object) });
        Compare(quireMethod, mathcadMethod, "null prefix", null, new System.Type[] { typeof(int) });
        Compare(quireMethod, mathcadMethod, "null type array", "T_", null);
    }
}
