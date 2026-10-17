using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class DynamicTypeNameMapsDifferentialTests
{
    private const string Contract = "Mpl.Run.DynamicType";
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void name_to_type_matches_mappings_and_errors()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("NameToType", PrivateStatic, [typeof(string)])!;
        MethodInfo originalMethod = originalType.GetMethod("NameToType", PrivateStatic, [typeof(string)])!;

        foreach (string? name in new string?[] { "I", "F", "O", "unknown", null })
        {
            object?[] quireArgs = [name];
            object?[] originalArgs = [name];
            MethodComparison.Compare("DynamicType.NameToType(" + (name ?? "null") + ")", Contract,
                quireMethod, null, quireArgs, originalMethod, null, originalArgs,
                value => Describe(value), value => Describe(value));
        }
    }

    [SkippableFact]
    public void type_to_name_matches_mappings_and_fallback()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("TypeToName", PrivateStatic, [typeof(System.Type)])!;
        MethodInfo originalMethod = originalType.GetMethod("TypeToName", PrivateStatic, [typeof(System.Type)])!;

        foreach (System.Type? type in new System.Type?[] { typeof(int), typeof(double), typeof(object), typeof(string), null })
        {
            object?[] quireArgs = [type];
            object?[] originalArgs = [type];
            MethodComparison.Compare("DynamicType.TypeToName(" + (type?.FullName ?? "null") + ")", Contract,
                quireMethod, null, quireArgs, originalMethod, null, originalArgs);
        }
    }
}
