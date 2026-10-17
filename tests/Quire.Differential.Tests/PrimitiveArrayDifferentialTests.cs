using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveArrayDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly System.Type[] Parameters = [typeof(int)];

    [SkippableFact]
    public void Arr_i4_creates_zero_initialized_integer_arrays()
    {
        Compare("arr_i4", 0);
        Compare("arr_i4", 1);
        Compare("arr_i4", 3);
        Compare("arr_i4", -1);
    }

    [SkippableFact]
    public void Arr_f8_creates_zero_initialized_double_arrays()
    {
        Compare("arr_f8", 0);
        Compare("arr_f8", 1);
        Compare("arr_f8", 3);
        Compare("arr_f8", -1);
    }

    [SkippableFact]
    public void Arr_ref_creates_null_initialized_object_arrays()
    {
        Compare("arr_ref", 0);
        Compare("arr_ref", 1);
        Compare("arr_ref", 3);
        Compare("arr_ref", -1);
    }

    private static void Compare(string name, int size)
    {
        var qMethod = typeof(Mpl.Run.Primitive).GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, Parameters, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, Parameters, null)!;
        MethodComparison.Compare(name + "(" + size + ")", Contract,
            qMethod, null, [size], mMethod, null, [size], Describe, Describe);
    }

    private static string Describe(object? value)
    {
        if (value is not Array array) return value?.ToString() ?? "<null>";
        var elements = new string[array.Length];
        for (int i = 0; i < array.Length; i++) elements[i] = array.GetValue(i)?.ToString() ?? "null";
        return array.GetType().FullName + "[" + string.Join(",", elements) + "]";
    }
}
