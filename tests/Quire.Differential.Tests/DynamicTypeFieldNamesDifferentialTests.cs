using System.Collections;
using System.Reflection;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class DynamicTypeFieldNamesDifferentialTests
{
    private const string Contract = "Mpl.Run.DynamicType";

    [SkippableFact]
    public void type_initializer_and_field_name_cache_match()
    {
        Runtime.Sync();
        System.Type quireType = typeof(DynamicType);
        System.Type originalType = M(Contract);
        MethodComparison.Compare("DynamicType.cctor initializes name tables", Contract,
            quireType.TypeInitializer!, null, [], originalType.TypeInitializer!, null, [],
            _ => StaticState(quireType), _ => StaticState(originalType));
        MethodInfo quireMethod = quireType.GetMethod(nameof(DynamicType.FieldNames), [typeof(int)])!;
        MethodInfo originalMethod = originalType.GetMethod(nameof(DynamicType.FieldNames), [typeof(int)])!;

        foreach (int count in new[] { 0, 1, 4, -1 })
        {
            MethodComparison.Compare("DynamicType.FieldNames(" + count + ")", Contract,
                quireMethod, null, [count], originalMethod, null, [count], ShowNames, ShowNames);
        }

        string[] quireCached = DynamicType.FieldNames(4);
        string[] originalCached = (string[])originalMethod.Invoke(null, [4])!;
        MethodComparison.Compare("DynamicType.FieldNames returns cached array", Contract,
            quireMethod, null, [4], originalMethod, null, [4],
            value => ReferenceEquals(value, quireCached) ? "cached " + ShowNames(value) : "new " + ShowNames(value),
            value => ReferenceEquals(value, originalCached) ? "cached " + ShowNames(value) : "new " + ShowNames(value));
    }

    private static string StaticState(System.Type type)
    {
        string Table(string name)
        {
            var table = (Hashtable)Reflect.Get(type, name)!;
            return string.Join(",", table.Cast<DictionaryEntry>()
                .Select(entry => (MplRunSides.Describe(entry.Key), MplRunSides.Describe(entry.Value)))
                .OrderBy(entry => entry.Item1, StringComparer.Ordinal)
                .Select(entry => entry.Item1 + "=" + entry.Item2));
        }

        return "DelimiterString=" + MplRunSides.Describe(Reflect.Get(type, "DelimiterString"))
            + "\nTuplePrefix=" + MplRunSides.Describe(Reflect.Get(type, "TuplePrefix"))
            + "\nFieldNameTable=" + Table("FieldNameTable")
            + "\nNameToTypeTable=" + Table("NameToTypeTable")
            + "\nTypeToNameTable=" + Table("TypeToNameTable");
    }

    private static string ShowNames(object? value) => value is string[] names
        ? "string[" + names.Length + "]:[" + string.Join(",", names.Select(name => name ?? "<null>")) + "]"
        : MplRunSides.Describe(value);
}
