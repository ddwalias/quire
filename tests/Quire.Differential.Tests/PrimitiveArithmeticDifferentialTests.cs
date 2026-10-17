using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveArithmeticDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly System.Type[] UnaryInt = [typeof(int)];
    private static readonly System.Type[] BinaryInt = [typeof(int), typeof(int)];
    private static readonly System.Type[] UnaryDouble = [typeof(double)];
    private static readonly System.Type[] BinaryDouble = [typeof(double), typeof(double)];

    [SkippableFact]
    public void Integer_arithmetic_matches_wraparound_signed_results_and_exceptions()
    {
        Compare("neg_i4", UnaryInt, int.MinValue);
        Compare("neg_i4", UnaryInt, -1);
        Compare("add_i4", BinaryInt, int.MaxValue, 1);
        Compare("add_i4", BinaryInt, int.MinValue, -1);
        Compare("sub_i4", BinaryInt, int.MinValue, 1);
        Compare("sub_i4", BinaryInt, int.MaxValue, -1);
        Compare("mul_i4", BinaryInt, int.MaxValue, 2);
        Compare("mul_i4", BinaryInt, int.MinValue, -1);
        Compare("div_i4", BinaryInt, -7, 3);
        Compare("div_i4", BinaryInt, int.MinValue, -1);
        Compare("div_i4", BinaryInt, 1, 0);
        Compare("rem_i4", BinaryInt, -7, 3);
        Compare("rem_i4", BinaryInt, int.MinValue, -1);
        Compare("rem_i4", BinaryInt, 1, 0);
    }

    [SkippableFact]
    public void Floating_arithmetic_matches_ieee_results()
    {
        Compare("neg_f8", UnaryDouble, -0.0);
        Compare("neg_f8", UnaryDouble, double.NaN);
        Compare("add_f8", BinaryDouble, double.MaxValue, double.MaxValue);
        Compare("add_f8", BinaryDouble, double.PositiveInfinity, double.NegativeInfinity);
        Compare("sub_f8", BinaryDouble, 0.0, -0.0);
        Compare("sub_f8", BinaryDouble, double.PositiveInfinity, double.PositiveInfinity);
        Compare("mul_f8", BinaryDouble, double.MaxValue, 2.0);
        Compare("mul_f8", BinaryDouble, double.PositiveInfinity, 0.0);
        Compare("div_f8", BinaryDouble, 1.0, 0.0);
        Compare("div_f8", BinaryDouble, 0.0, 0.0);
        Compare("div_f8", BinaryDouble, -1.0, 0.0);
    }

    private static void Compare(string name, System.Type[] parameterTypes, params object[] arguments)
    {
        var qMethod = typeof(Mpl.Run.Primitive).GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, parameterTypes, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, parameterTypes, null)!;
        MethodComparison.Compare(name + "(" + string.Join(", ", arguments) + ")", Contract,
            qMethod, null, arguments, mMethod, null, arguments,
            quireShow: qMethod.ReturnType == typeof(double) ? ShowDouble : null,
            originalShow: mMethod.ReturnType == typeof(double) ? ShowDouble : null);
    }

    private static string ShowDouble(object? value) => value is double number
        ? BitConverter.DoubleToInt64Bits(number).ToString("X16")
        : value?.ToString() ?? "<null>";
}
