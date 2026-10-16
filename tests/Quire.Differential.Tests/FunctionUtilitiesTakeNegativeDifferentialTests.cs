using System.Reflection;
using Quire.Mpl.Run;

namespace Quire.Differential.Tests;

public class FunctionUtilitiesTakeNegativeDifferentialTests
{
    private const string Contract = "Mpl.Run.FunctionUtilities";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static System.Type QUtilities => typeof(FunctionUtilities);
    private static System.Type MUtilities => Mathcad.Type("mplrun", "Mpl.Run.FunctionUtilities");

    private static MethodInfo Take(System.Type type) =>
        type.GetMethod("take", Static)!.MakeGenericMethod(typeof(int));

    private static string Show(object? value, int[]? input)
    {
        if (value is not int[] array)
            return MplRunSides.Describe(value);

        var contents = string.Join(",", array.Select(item => item.ToString()));
        return $"int[{array.Length}] same-input={ReferenceEquals(array, input)} contents=[{contents}]";
    }

    private static void Compare(string caseId, int count, int[]? input)
    {
        int[]? qInput = input == null ? null : (int[])input.Clone();
        int[]? mInput = input == null ? null : (int[])input.Clone();
        int[]? qBefore = qInput == null ? null : (int[])qInput.Clone();
        int[]? mBefore = mInput == null ? null : (int[])mInput.Clone();
        var qTake = Take(QUtilities);
        var mTake = Take(MUtilities);
        Assert.Equal(0x0600027b, mTake.MetadataToken);
        Assert.Equal(new Guid("17a0933b-4988-4c18-963d-563998321cc8"), mTake.Module.ModuleVersionId);

        MethodComparison.Compare(caseId, Contract, qTake, null, [count, qInput], mTake, null, [count, mInput],
            value => Show(value, qInput), value => Show(value, mInput));

        Assert.Equal(qBefore, qInput);
        Assert.Equal(mBefore, mInput);
    }

    [SkippableFact]
    public void FunctionUtilities_take_negative_counts_and_array_identity()
    {
        Compare("take(-1, empty array)", -1, []);
        Compare("take(-1, nonempty array)", -1, [10, 20, 30]);
        Compare("take(-1, null array reads Length first)", -1, null);

        Compare("take(0, nonempty array returns new empty prefix)", 0, [10, 20, 30]);
        Compare("take(2, nonempty array copies prefix)", 2, [10, 20, 30]);
        Compare("take(length, returns input array)", 3, [10, 20, 30]);
        Compare("take(count above length, returns input array)", 4, [10, 20, 30]);
    }
}
