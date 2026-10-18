using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class DynamicTypeDemangleDifferentialTests
{
    private const string Contract = "Mpl.Run.DynamicType";
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void remove_prefix_matches_index_and_failure_behavior()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("RemovePrefix", PrivateStatic,
            new System.Type[] { typeof(string), typeof(string) })!;
        MethodInfo originalMethod = originalType.GetMethod("RemovePrefix", PrivateStatic,
            new System.Type[] { typeof(string), typeof(string) })!;

        CompareStrings(quireMethod, originalMethod, "matching prefix", "Mpl.Tuple_I_F", "Mpl.Tuple");
        CompareStrings(quireMethod, originalMethod, "prefix occurs later", "xMpl.Tuple_I", "Mpl.Tuple");
        CompareStrings(quireMethod, originalMethod, "empty prefix", "abc", "");
        CompareStrings(quireMethod, originalMethod, "missing prefix", "Mpl.Record_I", "Mpl.Tuple");
        CompareStrings(quireMethod, originalMethod, "prefix only", "Mpl.Tuple", "Mpl.Tuple");
        CompareStrings(quireMethod, originalMethod, "null name", null, "Mpl.Tuple");
        CompareStrings(quireMethod, originalMethod, "null prefix", "Mpl.Tuple_I", null);
    }

    [SkippableFact]
    public void name_demangle_matches_mappings_and_malformed_suffixes()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("NameDemangle", PrivateStatic,
            new System.Type[] { typeof(string), typeof(string) })!;
        MethodInfo originalMethod = originalType.GetMethod("NameDemangle", PrivateStatic,
            new System.Type[] { typeof(string), typeof(string) })!;

        CompareTypes(quireMethod, originalMethod, "mapped type names", "Mpl.Tuple_I_F", "Mpl.Tuple");
        CompareTypes(quireMethod, originalMethod, "object type name", "Mpl.Tuple_O", "Mpl.Tuple");
        CompareTypes(quireMethod, originalMethod, "unknown type name", "Mpl.Tuple_X", "Mpl.Tuple");
        CompareTypes(quireMethod, originalMethod, "repeated delimiter", "Mpl.Tuple_I__F", "Mpl.Tuple");
        CompareTypes(quireMethod, originalMethod, "prefix only", "Mpl.Tuple", "Mpl.Tuple");
        CompareTypes(quireMethod, originalMethod, "null name", null, "Mpl.Tuple");
    }

    [SkippableFact]
    public void tuple_name_demangle_matches_public_wrapper()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodInfo quireMethod = quireType.GetMethod("TupleNameDemangle", PublicStatic,
            new System.Type[] { typeof(string) })!;
        MethodInfo originalMethod = originalType.GetMethod("TupleNameDemangle", PublicStatic,
            new System.Type[] { typeof(string) })!;

        CompareTypes(quireMethod, originalMethod, "mapped tuple", "Mpl.Tuple_I_F");
        CompareTypes(quireMethod, originalMethod, "object tuple", "Mpl.Tuple_O");
        CompareTypes(quireMethod, originalMethod, "unknown tuple type", "Mpl.Tuple_X");
        CompareTypes(quireMethod, originalMethod, "malformed tuple suffix", "Mpl.Tuple_I__F");
        CompareTypes(quireMethod, originalMethod, "prefix only", "Mpl.Tuple");
        CompareTypes(quireMethod, originalMethod, "null name", null);
    }

    private static void CompareStrings(MethodInfo quireMethod, MethodInfo originalMethod,
        string caseId, string? value, string? prefix)
    {
        object?[] quireArgs = new object?[] { value, prefix };
        object?[] originalArgs = new object?[] { value, prefix };
        MethodComparison.Compare(caseId, Contract, quireMethod, null, quireArgs,
            originalMethod, null, originalArgs);
    }

    private static void CompareTypes(MethodInfo quireMethod, MethodInfo originalMethod,
        string caseId, string? value, string? prefix = null)
    {
        object?[] quireArgs = prefix == null ? new object?[] { value } : new object?[] { value, prefix };
        object?[] originalArgs = prefix == null ? new object?[] { value } : new object?[] { value, prefix };
        MethodComparison.Compare(caseId, Contract, quireMethod, null, quireArgs,
            originalMethod, null, originalArgs, ShowTypes, ShowTypes);
    }

    private static string ShowTypes(object? value)
    {
        if (value is not System.Type[] types)
        {
            return Describe(value);
        }
        return "[" + string.Join(",", types.Select(type => Describe(type))) + "]";
    }
}
