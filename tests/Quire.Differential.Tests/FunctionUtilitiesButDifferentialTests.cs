using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class FunctionUtilitiesButDifferentialTests
{
    private const string Contract = "Mpl.Run.FunctionUtilities";
    private const int ButToken = 0x0600027a;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static Type QUtilities => typeof(FunctionUtilities);
    private static Type MUtilities => Mathcad.Type("mplrun", "Mpl.Run.FunctionUtilities");

    private static MethodInfo But(Type type) => type.GetMethod("but", Static)!.MakeGenericMethod(typeof(int));

    private static string Show(object? value, int[]? input)
    {
        if (value is not int[] array) return MplRunSides.Describe(value);
        return $"int[{array.Length}]:same-input={ReferenceEquals(array, input)}:" +
            $"contents=[{string.Join(",", array)}]";
    }

    private static void Compare(string caseId, int count, int[]? values)
    {
        int[]? qInput = values == null ? null : (int[])values.Clone();
        int[]? mInput = values == null ? null : (int[])values.Clone();
        int[]? qBefore = qInput == null ? null : (int[])qInput.Clone();
        int[]? mBefore = mInput == null ? null : (int[])mInput.Clone();
        MethodInfo qBut = But(QUtilities), mBut = But(MUtilities);
        Assert.Equal(ButToken, mBut.MetadataToken);
        Assert.Equal(new Guid("17a0933b-4988-4c18-963d-563998321cc8"), mBut.Module.ModuleVersionId);

        MethodComparison.Compare(caseId, Contract, qBut, null, [count, qInput], mBut, null, [count, mInput],
            value => Show(value, qInput), value => Show(value, mInput));

        Assert.Equal(qBefore, qInput);
        Assert.Equal(mBefore, mInput);
    }

    [SkippableFact]
    public void FunctionUtilities_but_negative_counts_null_inputs_boundaries_and_identity()
    {
        Runtime.Sync();
        Compare("but(-1, empty array) allocates before Array.Copy rejects the negative source index", -1, []);
        Compare("but(-1, nonempty array) preserves Array.Copy negative-index exception", -1, [10, 20, 30]);
        Compare("but(int.MinValue, empty array) overflows the suffix length before Array.Copy", int.MinValue, []);
        Compare("but(int.MinValue, nonempty array) overflows the suffix length before Array.Copy", int.MinValue, [10, 20, 30]);
        Compare("but(-1, null array) reads Length before count handling", -1, null);
        Compare("but(0, null array) reads Length before count handling", 0, null);
        Compare("but(0, nonempty array) returns a fresh full-length copy", 0, [10, 20, 30]);
        Compare("but(length-1, nonempty array) returns the final one-element suffix", 2, [10, 20, 30]);
        Compare("but(length, nonempty array) returns an empty array", 3, [10, 20, 30]);
        Compare("but(count above length, nonempty array) returns an empty array", 4, [10, 20, 30]);
    }
}
