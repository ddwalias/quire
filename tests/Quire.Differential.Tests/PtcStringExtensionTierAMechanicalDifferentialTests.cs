using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Quire;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for the Tier A Ptc.StringExtension members.</summary>
public class PtcStringExtensionTierAMechanicalDifferentialTests
{
    private const string Contract = "Ptc.StringExtension";

    [SkippableFact]
    public void TierA_methods_and_initializer_match()
    {
        Mathcad.Require();
        Assembly originalAssembly = Mathcad.Load("Ptc.Core");
        Assert.Equal("d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56",
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(originalAssembly.Location))).ToLowerInvariant());
        Assert.Equal(Guid.Parse("fc0806aa-db73-489f-8ae4-4729770faca6"), originalAssembly.ManifestModule.ModuleVersionId);

        Type quireType = typeof(StringExtension);
        Type originalType = originalAssembly.GetType("Ptc.StringExtension")!;
        ConstructorInfo quireInitializer = quireType.TypeInitializer!;
        ConstructorInfo originalInitializer = originalType.TypeInitializer!;
        Assert.Equal(0x0600027b, originalInitializer.MetadataToken);
        MethodComparison.Compare(".cctor initializes XML regexes and ASCII bound", Contract,
            quireInitializer, null, [], originalInitializer, null, [], Reflect.Text, Reflect.Text);
        Assert.Equal("[\\u0000-\\u0008\\u000B\\u000C\\u000E-\\u001F\\uD800-\\uDFFF\\uFFFE\\uFFFF]+",
            ((Regex)Reflect.Get(quireType, "_invalidXmlChars")!).ToString());
        Assert.Equal(((Regex)Reflect.Get(originalType, "_invalidXmlChars")!).ToString(),
            ((Regex)Reflect.Get(quireType, "_invalidXmlChars")!).ToString());
        Assert.Equal("^[\\u0009\\u000A\\u000D\\u0020-\\uD7FF\\uE000-\\uFFFD]*$",
            ((Regex)Reflect.Get(quireType, "_validXmlString")!).ToString());
        Assert.Equal(((Regex)Reflect.Get(originalType, "_validXmlString")!).ToString(),
            ((Regex)Reflect.Get(quireType, "_validXmlString")!).ToString());
        Assert.Equal(Reflect.Get(originalType, "AsciiEndCode"), Reflect.Get(quireType, "AsciiEndCode"));

        CompareChar(quireType, originalType, "straight double quote", "IsStraightQuoteChar", '"');
        CompareChar(quireType, originalType, "straight single quote", "IsStraightQuoteChar", '\'');
        CompareChar(quireType, originalType, "Unicode opening double quote", "IsOpenCloseUnicodeQuoteChar", '\u201c');
        CompareChar(quireType, originalType, "Unicode closing double quote", "IsOpenCloseUnicodeQuoteChar", '\u201d');
        CompareChar(quireType, originalType, "ordinary character is not a Unicode quote", "IsOpenCloseUnicodeQuoteChar", 'a');
        CompareChar(quireType, originalType, "straight double quote is accepted", "IsAnyQuoteChar", '"');
        CompareChar(quireType, originalType, "Unicode opening quote is accepted", "IsAnyQuoteChar", '\u2018');
        CompareChar(quireType, originalType, "ordinary character is not a quote", "IsAnyQuoteChar", 'a');

        Type quireQuotes = quireType.GetNestedType("QuotesType", BindingFlags.Public)!;
        Type originalQuotes = originalType.GetNestedType("QuotesType", BindingFlags.Public)!;
        MethodInfo quireWrapped = quireType.GetMethod("IsWrappedWithQuotes", BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo originalWrapped = originalType.GetMethod("IsWrappedWithQuotes", BindingFlags.Public | BindingFlags.Static)!;
        CompareWrapped(quireWrapped, originalWrapped, quireQuotes, originalQuotes, "straight pair", "\"value\"", 0);
        CompareWrapped(quireWrapped, originalWrapped, quireQuotes, originalQuotes, "Unicode pair", "\u201cvalue\u201d", 1);
        CompareWrapped(quireWrapped, originalWrapped, quireQuotes, originalQuotes, "any quote pair", "\u2018value\u2019", 2);
        CompareWrapped(quireWrapped, originalWrapped, quireQuotes, originalQuotes, "mixed pair", "\u201cvalue\"", 2);
        CompareWrapped(quireWrapped, originalWrapped, quireQuotes, originalQuotes, "single character", "\"", 2);
        CompareWrapped(quireWrapped, originalWrapped, quireQuotes, originalQuotes, "invalid quote enum", "\"value\"", 99);
    }

    private static void CompareChar(Type quireType, Type originalType, string caseId, string name, char value)
    {
        MethodInfo quire = quireType.GetMethod(name, BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo original = originalType.GetMethod(name, BindingFlags.Public | BindingFlags.Static)!;
        MethodComparison.Compare(caseId, Contract, quire, null, [value], original, null, [value], Reflect.Text, Reflect.Text);
    }

    private static void CompareWrapped(MethodInfo quire, MethodInfo original, Type quireQuotes, Type originalQuotes,
        string caseId, string value, int quotesType)
    {
        object quireKind = Enum.ToObject(quireQuotes, quotesType);
        object originalKind = Enum.ToObject(originalQuotes, quotesType);
        MethodComparison.Compare(caseId, Contract, quire, null, [value, quireKind],
            original, null, [value, originalKind], Reflect.Text, Reflect.Text);
    }
}
