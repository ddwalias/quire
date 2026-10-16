using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimMakeRcDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int OriginalToken = 0x06000622;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Make(Side side) =>
        side.T("Mcd.MatDim").GetMethod("make", StaticDeclared, null,
            [Nested(side.T("Mcd.MatDim"), "RC"), side.T("Mcd.Dim[]"), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.make(RC, Dim[], int)");

    private static string Dimensions(Side side, object? dimensions) => dimensions is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : dimensions == null ? "null" : side.Show(dimensions);

    private static string Result(Side side, object? value, object? inputDimensions,
        object? expectedUniform, string inputText)
    {
        if (value == null) return "null";
        var type = value.GetType();
        object? uniform = Get(value, "UniformDim");
        string result = $"{type.Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" uniform={side.DimText(uniform)} uniform-ref-input={(ReferenceEquals(uniform, expectedUniform) ? "yes" : "no")}";
        if (type.Name == "r_dim")
        {
            object? rows = Get(value, "RowDimensions");
            result += $" row-array-input={(ReferenceEquals(rows, inputDimensions) ? "yes" : "no")}" +
                $" row-dims={Dimensions(side, rows)} col-dims={Dimensions(side, Get(value, "ColDimensions"))}";
        }
        else if (type.Name == "c_dim")
        {
            object? cols = Get(value, "ColDimensions");
            result += $" col-array-input={(ReferenceEquals(cols, inputDimensions) ? "yes" : "no")}" +
                $" row-dims={Dimensions(side, Get(value, "RowDimensions"))} col-dims={Dimensions(side, cols)}";
        }
        return result + $" input={inputText}";
    }

    private static string Show(Side side, object? value, object? inputDimensions,
        object? expectedUniform, string inputText)
    {
        if (value != null && side.T("Mcd.MatDim").IsInstanceOfType(value))
            return Result(side, value, inputDimensions, expectedUniform, inputText);
        if (value is Array array && array.GetType().GetElementType() == side.T("Mcd.Dim"))
            return Dimensions(side, array);
        if (value != null && value.GetType().IsEnum) return Convert.ToInt32(value).ToString();
        if (value is IFormattable scalar && value is not string) return scalar.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
        return side.DimText(value);
    }

    private static object?[] MakeArgs(Side side, string rc, object? dimensions, int n) =>
        [Enum.Parse(Nested(side.T("Mcd.MatDim"), "RC"), rc), dimensions, n];

    private static void Compare(string caseId, string rc, string[]? names, int n,
        string? inputText = null, int? enumValue = null)
    {
        var qDims = names == null ? null : Array.CreateInstance(Side.Quire.Dim, names.Length);
        var mDims = names == null ? null : Array.CreateInstance(Side.Mathcad.Dim, names.Length);
        if (names != null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                qDims!.SetValue(Side.Quire.D(names[i]), i);
                mDims!.SetValue(Side.Mathcad.D(names[i]), i);
            }
        }
        object? qExpectedUniform = names is { Length: > 0 } && names.All(name => name == names[0]) ? qDims!.GetValue(0) : null;
        object? mExpectedUniform = names is { Length: > 0 } && names.All(name => name == names[0]) ? mDims!.GetValue(0) : null;
        string text = inputText ?? (names == null ? "null" : "[" + string.Join(",", names) + "]");
        object?[] qArgs = enumValue is int qValue
            ? [Enum.ToObject(Nested(Side.Quire.T("Mcd.MatDim"), "RC"), qValue), qDims, n]
            : MakeArgs(Side.Quire, rc, qDims, n);
        object?[] mArgs = enumValue is int mValue
            ? [Enum.ToObject(Nested(Side.Mathcad.T("Mcd.MatDim"), "RC"), mValue), mDims, n]
            : MakeArgs(Side.Mathcad, rc, mDims, n);
        var qMethod = Make(Side.Quire);
        var mMethod = Make(Side.Mathcad);
        Assert.Equal(OriginalToken, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, qArgs, mMethod, null, mArgs,
            quireShow: value => Show(Side.Quire, value, qDims, qExpectedUniform, text),
            originalShow: value => Show(Side.Mathcad, value, mDims, mExpectedUniform, text));

        if (names != null)
        {
            Assert.Equal(names.Select(name => Side.Quire.DimText(Side.Quire.D(name))),
                qDims!.Cast<object>().Select(Side.Quire.DimText));
            Assert.Equal(names.Select(name => Side.Mathcad.DimText(Side.Mathcad.D(name))),
                mDims!.Cast<object>().Select(Side.Mathcad.DimText));
        }
    }

    [SkippableFact]
    public void Make_from_RC_preserves_axis_arrays_and_original_uniform_shape_rule()
    {
        Runtime.Sync();
        Assert.Equal(OriginalToken, Make(Side.Mathcad).MetadataToken);

        Compare("uniform dimensions ignore COL and n and use square array length", "COL", ["L", "L"], 7);
        Compare("nonuniform ROW retains the supplied array and requested columns", "ROW", ["L", "T"], 3);
        Compare("nonuniform COL retains the supplied array and requested rows", "COL", ["L", "T"], 4);
        Compare("unknown nonzero enum follows the original COL branch", "COL", ["L", "T"], 2, enumValue: 77);
        Compare("null dimension array throws from the selected row factory", "ROW", null, 2);
        Compare("empty dimensions throw while Uniform reads element zero", "ROW", [], 2);
    }
}
