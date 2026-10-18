using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class DynamicTypeTupleNameMangleDifferentialTests
{
    private const string Contract = "Mpl.Run.DynamicType";
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void tuple_name_mangle_matches_type_arrays()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("TupleNameMangle", PublicStatic,
            new System.Type[] { typeof(System.Type[]) })!;
        MethodInfo originalMethod = originalType.GetMethod("TupleNameMangle", PublicStatic,
            new System.Type[] { typeof(System.Type[]) })!;

        Compare(quireMethod, originalMethod, "empty tuple", new System.Type[0]);
        Compare(quireMethod, originalMethod, "mapped tuple types",
            new System.Type[] { typeof(int), typeof(double), typeof(object) });
        Compare(quireMethod, originalMethod, "fallback tuple type", new System.Type[] { typeof(string) });
        Compare(quireMethod, originalMethod, "mixed tuple types",
            new System.Type[] { typeof(int), typeof(string), typeof(object) });
        Compare(quireMethod, originalMethod, "null type array", null);
    }

    [SkippableFact]
    public void tuple_name_mangle_matches_repeated_object_types()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("TupleNameMangle", PublicStatic,
            new System.Type[] { typeof(int) })!;
        MethodInfo originalMethod = originalType.GetMethod("TupleNameMangle", PublicStatic,
            new System.Type[] { typeof(int) })!;

        foreach (int count in new int[] { 0, 1, 3, -1 })
        {
            MethodComparison.Compare("TupleNameMangle(" + count + ")", Contract,
                quireMethod, null, new object?[] { count },
                originalMethod, null, new object?[] { count });
        }
    }

    private static void Compare(MethodInfo quireMethod, MethodInfo originalMethod, string caseId,
        System.Type[]? types)
    {
        MethodComparison.Compare(caseId, Contract, quireMethod, null, new object?[] { types },
            originalMethod, null, new object?[] { types });
    }
}
