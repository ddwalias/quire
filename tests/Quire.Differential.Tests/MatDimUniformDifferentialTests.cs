using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x06000629;
    private const BindingFlags DeclaredStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side) =>
        side.T("Mcd.MatDim").GetMethod("Uniform", DeclaredStatic, null, [side.Dim.MakeArrayType()], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.Uniform(Dim[])");

    private static Array? Dims(Side side, string?[]? names)
    {
        if (names == null) return null;
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(names[i] == null ? null : side.D(names[i]!), i);
        return result;
    }

    private static string Describe(Side side, object? value, Array? input)
    {
        if (value == null) return "null";
        if (value is Array array)
        {
            var seen = new List<object>();
            return "[" + string.Join(",", array.Cast<object?>().Select(item => DescribeDim(item, seen))) + "]";
        }
        return DescribeDim(value, input?.Cast<object?>().Where(item => item != null).Cast<object>().ToList() ?? []);

        string DescribeDim(object? dimension, List<object> seen)
        {
            if (dimension == null) return "null";
            int index = seen.FindIndex(item => ReferenceEquals(item, dimension));
            if (index < 0)
            {
                index = seen.Count;
                seen.Add(dimension);
            }
            return $"{side.DimText(dimension)}@d{index}";
        }
    }

    private static void Compare(string caseId, string?[]? names)
    {
        var qMethod = Method(Side.Quire);
        var mMethod = Method(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        var qInput = Dims(Side.Quire, names);
        var mInput = Dims(Side.Mathcad, names);
        var qBefore = qInput?.Cast<object?>().ToArray();
        var mBefore = mInput?.Cast<object?>().ToArray();
        object?[] qArgs = [qInput], mArgs = [mInput];

        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => Describe(Side.Quire, value, qInput),
            originalShow: value => Describe(Side.Mathcad, value, mInput));

        Assert.Same(qInput, qArgs[0]);
        Assert.Same(mInput, mArgs[0]);
        if (qBefore != null) AssertArrayUnchanged(qBefore, qInput!);
        if (mBefore != null) AssertArrayUnchanged(mBefore, mInput!);
    }

    private static void AssertArrayUnchanged(object?[] before, Array current)
    {
        Assert.Equal(before.Length, current.Length);
        for (int i = 0; i < before.Length; i++) Assert.Same(before[i], current.GetValue(i));
    }

    [SkippableFact]
    public void Uniform_returns_only_a_reference_identical_dimension_and_preserves_input()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Method(Side.Mathcad).MetadataToken);

        Compare("Uniform(null array)", null);
        Compare("Uniform(empty array) throws before scanning", []);
        Compare("Uniform(singleton returns exact element", ["L"]);
        Compare("Uniform repeated identical references returns first", ["L", "L"]);
        Compare("Uniform equal-coordinate distinct markers are not uniform", ["identity", "mcd_zero"]);
        Compare("Uniform different dimensions returns null", ["L", "T"]);
        Compare("Uniform all-null entries", [null, null]);
        Compare("Uniform null first then dimension", [null, "L"]);
        Compare("Uniform dimension first then null", ["L", null]);
    }
}
