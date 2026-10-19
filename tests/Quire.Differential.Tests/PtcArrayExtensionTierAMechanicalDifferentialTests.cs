using System.Reflection;
using System.Security.Cryptography;
using Quire;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for the Tier A Ptc.ArrayExtension.Fill member.</summary>
public class PtcArrayExtensionTierAMechanicalDifferentialTests
{
    private const string Contract = "Ptc.ArrayExtension";

    [SkippableFact]
    public void Fill_clamps_ranges_and_mutates_only_selected_elements()
    {
        Mathcad.Require();
        Assembly originalAssembly = Mathcad.Load("Ptc.Core");
        Assert.Equal("d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56",
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(originalAssembly.Location))).ToLowerInvariant());
        Assert.Equal(Guid.Parse("fc0806aa-db73-489f-8ae4-4729770faca6"), originalAssembly.ManifestModule.ModuleVersionId);
        Type originalType = originalAssembly.GetType("Ptc.ArrayExtension")!;
        MethodInfo quireFill = typeof(ArrayExtension).GetMethod(nameof(ArrayExtension.Fill))!.MakeGenericMethod(typeof(int));
        MethodInfo originalDefinition = originalType.GetMethod("Fill", BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo originalFill = originalDefinition.MakeGenericMethod(typeof(int));
        Assert.Equal(0x06000284, originalDefinition.MetadataToken);
        Assert.Equal(typeof(void), originalDefinition.ReturnType);
        Assert.Equal([typeof(int[]), typeof(int), typeof(int), typeof(int)], originalFill.GetParameters().Select(parameter => parameter.ParameterType));

        CompareInt(quireFill, originalFill, "in-range half-open range", [1, 2, 3, 4], 9, 1, 3, [1, 9, 9, 4]);
        CompareInt(quireFill, originalFill, "negative start and negative end", [1, 2, 3], 8, -4, -1, [8, 8, 8]);
        CompareInt(quireFill, originalFill, "end beyond length", [1, 2, 3], 7, 1, 99, [1, 7, 7]);
        CompareInt(quireFill, originalFill, "start beyond length resets to zero", [1, 2, 3], 6, 9, 2, [6, 6, 3]);
        CompareInt(quireFill, originalFill, "start equals end", [1, 2, 3], 5, 2, 2, [1, 2, 3]);
        CompareInt(quireFill, originalFill, "start exceeds end", [1, 2, 3, 4], 4, 3, 1, [1, 2, 3, 4]);
        CompareInt(quireFill, originalFill, "empty array", [], 3, 0, -1, []);

        MethodInfo quireStringFill = typeof(ArrayExtension).GetMethod(nameof(ArrayExtension.Fill))!.MakeGenericMethod(typeof(string));
        MethodInfo originalStringFill = originalDefinition.MakeGenericMethod(typeof(string));
        Compare<string>(quireStringFill, originalStringFill, "generic reference elements", ["a", "b", "c"], "x", 1, 2, ["a", "x", "c"]);
    }

    private static void CompareInt(MethodInfo quire, MethodInfo original, string caseId,
        int[] initial, int value, int start, int end, int[] expected)
    {
        int[] behavior = (int[])initial.Clone();
        ArrayExtension.Fill(behavior, value, start, end);
        Assert.Equal(expected, behavior);
        Compare(quire, original, caseId, (int[])initial.Clone(), value, start, end);
    }

    private static void Compare<T>(MethodInfo quire, MethodInfo original, string caseId,
        T[] initial, T value, int start, int end, T[] expected)
    {
        T[] behavior = (T[])initial.Clone();
        ArrayExtension.Fill(behavior, value, start, end);
        Assert.Equal(expected, behavior);
        Compare(quire, original, caseId, (T[])initial.Clone(), value, start, end);
    }

    private static void Compare<T>(MethodInfo quire, MethodInfo original, string caseId,
        T[] quireArray, T value, int start, int end) =>
        MethodComparison.Compare(caseId, Contract, quire, null, [quireArray, value, start, end],
            original, null, [(T[])quireArray.Clone(), value, start, end], Reflect.Text, Reflect.Text);
}
