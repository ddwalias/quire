using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimAxisArrayFactoriesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int RowToken = 0x06000624;
    private const int ColToken = 0x06000626;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatDim").GetMethod(name, StaticDeclared, null,
            [side.T("Mcd.Dim"), side.T("Mcd.Dim[]"), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{name}(Dim, Dim[], int)");

    private static Array? MakeDims(Side side, string[]? names)
    {
        if (names == null) return null;
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) values.SetValue(side.D(names[i]), i);
        return values;
    }

    private static string Dims(Side side, object? values) => values is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : values == null ? "null" : side.Show(values);

    private static string Result(Side side, object? value, object prefix, object?[] expectedFactors,
        object? expectedUniform, Array? suffix, string suffixBefore)
    {
        if (value == null) return "null";
        object? uniform = Get(value, "UniformDim");
        string output = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" uniform={side.DimText(uniform)} uniform-ref-prefix={(ReferenceEquals(uniform, expectedUniform) ? "yes" : "no")}";
        string member = value.GetType().Name == "r_dim" ? "RowDimensions" : value.GetType().Name == "c_dim" ? "ColDimensions" : "";
        if (member.Length > 0)
        {
            var actual = (Array)Get(value, member)!;
            output += $" {member}-count={actual.Length} axis-array-suffix-alias={(ReferenceEquals(actual, suffix) ? "yes" : "no")}" +
                " factors=" + string.Join(",", actual.Cast<object?>().Select((factor, i) =>
                $"{side.DimText(factor)}:{(i < expectedFactors.Length && ReferenceEquals(factor, expectedFactors[i]) ? "expected-ref" : "other-ref")}"));
        }
        return output + $" suffix-before={suffixBefore}";
    }

    private static string Show(Side side, object? value, object prefix, object?[] expectedFactors,
        object? expectedUniform, Array? suffix, string suffixBefore)
    {
        if (value != null && side.T("Mcd.MatDim").IsInstanceOfType(value))
            return Result(side, value, prefix, expectedFactors, expectedUniform, suffix, suffixBefore);
        if (value != null && side.T("Mcd.Dim[]").IsInstanceOfType(value)) return Dims(side, value);
        if (value != null && side.T("Mcd.Dim").IsInstanceOfType(value)) return side.DimText(value);
        if (value is IFormattable scalar) return scalar.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
        return side.Show(value);
    }

    private static void Compare(string caseId, string methodName, int token, string prefixName,
        string[]? suffixNames, int n)
    {
        object qPrefix = Side.Quire.D(prefixName)!, mPrefix = Side.Mathcad.D(prefixName)!;
        Array? qSuffix = MakeDims(Side.Quire, suffixNames);
        Array? mSuffix = MakeDims(Side.Mathcad, suffixNames);
        object?[] qFactors = [qPrefix, .. (qSuffix?.Cast<object?>() ?? [])];
        object?[] mFactors = [mPrefix, .. (mSuffix?.Cast<object?>() ?? [])];
        bool uniform = qFactors.Length > 0 && suffixNames != null && suffixNames.All(name => name == prefixName);
        object? qUniform = uniform ? qPrefix : null, mUniform = uniform ? mPrefix : null;
        string suffixBeforeQ = Dims(Side.Quire, qSuffix), suffixBeforeM = Dims(Side.Mathcad, mSuffix);
        var qMethod = Method(Side.Quire, methodName); var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(token, mMethod.MetadataToken);
        object?[] qArgs = [qPrefix, qSuffix, n], mArgs = [mPrefix, mSuffix, n];
        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => Show(Side.Quire, value, qPrefix, qFactors, qUniform, qSuffix, suffixBeforeQ),
            originalShow: value => Show(Side.Mathcad, value, mPrefix, mFactors, mUniform, mSuffix, suffixBeforeM));

        Assert.Same(qSuffix, qArgs[1]);
        Assert.Same(mSuffix, mArgs[1]);
        if (qSuffix != null)
            for (int i = 0; i < qSuffix.Length; i++) Assert.Same(qFactors[i + 1], qSuffix.GetValue(i));
        if (mSuffix != null)
            for (int i = 0; i < mSuffix.Length; i++) Assert.Same(mFactors[i + 1], mSuffix.GetValue(i));
    }

    [SkippableFact]
    public void Array_axis_factories_push_prefix_and_preserve_original_suffix_behavior()
    {
        Runtime.Sync();
        Assert.Equal(RowToken, Method(Side.Mathcad, "makeRowDimension").MetadataToken);
        Assert.Equal(ColToken, Method(Side.Mathcad, "makeColDimension").MetadataToken);

        foreach (var (name, token) in new[] { ("makeRowDimension", RowToken), ("makeColDimension", ColToken) })
        {
            Compare($"{name} with empty suffix produces a 1x1 uniform dimension", name, token, "L", [], 7);
            Compare($"{name} repeated uniform factors ignore n and use pushed length", name, token, "L", ["L", "L"], 7);
            Compare($"{name} nonuniform suffix preserves pushed order and n", name, token, "L", ["T", "L"], 4);
            Compare($"{name} preserves a negative requested axis size", name, token, "L", ["T", "L"], -1);
            Compare($"{name} null suffix throws in Push before the factory", name, token, "L", null, 2);
        }
    }
}
