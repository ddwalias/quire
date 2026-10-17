using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveArrayAccessDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly System.Type[] IntReadParameters = [typeof(int[]), typeof(int)];
    private static readonly System.Type[] DoubleReadParameters = [typeof(double[]), typeof(int)];
    private static readonly System.Type[] RefReadParameters = [typeof(object[]), typeof(int)];
    private static readonly System.Type[] IntWriteParameters = [typeof(int[]), typeof(int), typeof(int)];
    private static readonly System.Type[] DoubleWriteParameters = [typeof(double[]), typeof(int), typeof(double)];
    private static readonly System.Type[] RefWriteParameters = [typeof(object[]), typeof(int), typeof(object)];
    private static readonly System.Type[] VecWriteParameters = [typeof(object), typeof(object[]), typeof(int)];
    private static readonly System.Type[] LengthParameters = [typeof(object[])];

    [SkippableFact]
    public void Integer_array_reads_and_stores_match_indexing_and_mutation()
    {
        Compare("ldelem_i4", IntReadParameters, [new[] { 10, 20, 30 }, 0], [new[] { 10, 20, 30 }, 0]);
        Compare("ldelem_i4", IntReadParameters, [new[] { 10, 20, 30 }, 2], [new[] { 10, 20, 30 }, 2]);
        Compare("ldelem_i4", IntReadParameters, [new[] { 10, 20 }, -1], [new[] { 10, 20 }, -1]);
        Compare("stelem_i4_void", IntWriteParameters, [new[] { 1, 2, 3 }, 1, 42], [new[] { 1, 2, 3 }, 1, 42]);
        Compare("stelem_i4_void", IntWriteParameters, [new[] { 1, 2 }, 2, 42], [new[] { 1, 2 }, 2, 42]);
        Compare("stelem_i4", IntWriteParameters, [new[] { 1, 2, 3 }, 1, 42], [new[] { 1, 2, 3 }, 1, 42]);
    }

    [SkippableFact]
    public void Double_array_reads_and_stores_preserve_exact_double_bits()
    {
        Compare("ldelem_f8", DoubleReadParameters, [new[] { -0.0, double.NaN, Math.PI }, 0], [new[] { -0.0, double.NaN, Math.PI }, 0]);
        Compare("ldelem_f8", DoubleReadParameters, [new[] { -0.0, double.NaN, Math.PI }, 1], [new[] { -0.0, double.NaN, Math.PI }, 1]);
        Compare("stelem_f8_void", DoubleWriteParameters, [new[] { 1.0, 2.0 }, 1, -0.0], [new[] { 1.0, 2.0 }, 1, -0.0]);
        Compare("stelem_f8", DoubleWriteParameters, [new[] { 1.0, 2.0 }, 0, double.NaN], [new[] { 1.0, 2.0 }, 0, double.NaN]);
    }

    [SkippableFact]
    public void Object_array_reads_and_stores_preserve_values_and_identity()
    {
        var qFirst = new object();
        var qSecond = new object();
        var mFirst = new object();
        var mSecond = new object();
        Compare("ldelem_ref", RefReadParameters, [new object?[] { qFirst, qSecond, null }, 0],
            [new object?[] { mFirst, mSecond, null }, 0]);
        Compare("ldelem_ref", RefReadParameters, [new object?[] { qFirst, qSecond, null }, 2],
            [new object?[] { mFirst, mSecond, null }, 2]);
        Compare("stelem_ref_void", RefWriteParameters, [new object?[] { qFirst, qSecond }, 1, qFirst],
            [new object?[] { mFirst, mSecond }, 1, mFirst]);
        Compare("stelem_ref", RefWriteParameters, [new object?[] { qFirst, qSecond }, 0, qSecond],
            [new object?[] { mFirst, mSecond }, 0, mSecond]);
        Compare("stelem_vec", VecWriteParameters, [qSecond, new object?[] { qFirst, null }, 1],
            [mSecond, new object?[] { mFirst, null }, 1]);
        Compare("stelem_ref_void", RefWriteParameters, [new object?[] { qFirst }, 2, qSecond],
            [new object?[] { mFirst }, 2, mSecond]);
    }

    [SkippableFact]
    public void Object_array_length_matches_length_and_null_behavior()
    {
        Compare("ldlen", LengthParameters, [Array.Empty<object>()], [Array.Empty<object>()]);
        Compare("ldlen", LengthParameters, [new object[] { new object(), null! }], [new object[] { new object(), null! }]);
        Compare("ldlen", LengthParameters, [null], [null]);
    }

    private static void Compare(string name, System.Type[] parameterTypes, object?[] qArguments, object?[] mArguments)
    {
        var qMethod = typeof(Mpl.Run.Primitive).GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, parameterTypes, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, parameterTypes, null)!;
        var qArray = qArguments.OfType<Array>().FirstOrDefault();
        var mArray = mArguments.OfType<Array>().FirstOrDefault();
        var qElements = qArray is object[] qo ? (object?[])qo.Clone() : new object?[0];
        var mElements = mArray is object[] mo ? (object?[])mo.Clone() : new object?[0];
        object? qAssigned = name == "stelem_vec" || name.Contains("ref", StringComparison.Ordinal)
            ? qArguments.FirstOrDefault(a => a is not Array and not int and not null)
            : null;
        object? mAssigned = name == "stelem_vec" || name.Contains("ref", StringComparison.Ordinal)
            ? mArguments.FirstOrDefault(a => a is not Array and not int and not null)
            : null;
        Func<object?, string> qShow = value => Describe(value, qArray, qAssigned, qElements);
        Func<object?, string> mShow = value => Describe(value, mArray, mAssigned, mElements);
        MethodComparison.Compare(name + " direct invocation", Contract, qMethod, null, qArguments,
            mMethod, null, mArguments, qShow, mShow);
    }

    private static string Describe(object? value, Array? array, object? assigned, object?[] originalElements)
    {
        if (value == null) return "null";
        if (value is Array resultArray)
        {
            string identity = ReferenceEquals(resultArray, array) ? "same" : "other";
            var elements = new string[resultArray.Length];
            for (int i = 0; i < resultArray.Length; i++)
            {
                var element = resultArray.GetValue(i);
                elements[i] = element == null ? "null" : Label(element, assigned, originalElements) ?? Scalar(element);
            }
            return identity + ":" + resultArray.GetType().FullName + ":[" + string.Join(",", elements) + "]";
        }
        string? label = Label(value, assigned, originalElements);
        if (label != null) return label;
        return Scalar(value);
    }

    private static string? Label(object value, object? assigned, object?[] originalElements)
    {
        if (ReferenceEquals(value, assigned)) return "assigned";
        for (int i = 0; i < originalElements.Length; i++)
            if (ReferenceEquals(value, originalElements[i])) return "element:" + i;
        return null;
    }

    private static string Scalar(object value) => value is double number
        ? BitConverter.DoubleToInt64Bits(number).ToString("X16")
        : value.ToString() ?? "<null>";
}
