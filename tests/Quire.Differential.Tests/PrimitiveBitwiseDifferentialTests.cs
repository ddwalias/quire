using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveBitwiseDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly System.Type[] BinaryInt = [typeof(int), typeof(int)];
    private static readonly System.Type[] BinaryUIntInt = [typeof(uint), typeof(int)];
    private static readonly System.Type[] BinaryDouble = [typeof(double), typeof(double)];

    [SkippableFact]
    public void Integer_xor_matches_bit_patterns()
    {
        Compare("xor", BinaryInt, int.MinValue, 1);
        Compare("xor", BinaryInt, unchecked((int)0xA55AA55A), unchecked((int)0x5AA55AA5));
        Compare("xor", BinaryInt, -1, int.MinValue);
        Compare("xor", BinaryInt, 0, 0);
    }

    [SkippableFact]
    public void Signed_shifts_match_sign_extension_and_masked_counts()
    {
        int[] counts = [0, 1, 31, 32, 33, -1, int.MinValue];
        int[] values = [int.MinValue, -1, 0x40000001];
        foreach (int value in values)
            foreach (int count in counts)
            {
                Compare("shl", BinaryInt, value, count);
                Compare("shr", BinaryInt, value, count);
            }
    }

    [SkippableFact]
    public void Unsigned_shift_matches_zero_extension_and_masked_counts()
    {
        int[] counts = [0, 1, 31, 32, 33, -1, int.MinValue];
        uint[] values = [0x80000000, 0xffffffff, 0x40000001];
        foreach (uint value in values)
            foreach (int count in counts)
                Compare("shr_u4", BinaryUIntInt, value, count);
    }

    [SkippableFact]
    public void Nth_root_matches_pow_order_and_ieee_edges()
    {
        Compare("nrt_f8", BinaryDouble, 2.0, 9.0);
        Compare("nrt_f8", BinaryDouble, 3.0, -8.0);
        Compare("nrt_f8", BinaryDouble, 0.0, 2.0);
        Compare("nrt_f8", BinaryDouble, 2.0, 0.0);
        Compare("nrt_f8", BinaryDouble, 0.5, double.PositiveInfinity);
        Compare("nrt_f8", BinaryDouble, double.NaN, -1.0);
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
