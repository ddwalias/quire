using System.Reflection;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveConversionDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";

    [SkippableFact]
    public void Conv_i4_matches_double_to_int_conversion()
    {
        Compare("conv_i4", [typeof(double)], -3.9);
        Compare("conv_i4", [typeof(double)], 3.9);
        Compare("conv_i4", [typeof(double)], (double)int.MinValue);
        Compare("conv_i4", [typeof(double)], (double)int.MaxValue);
        Compare("conv_i4", [typeof(double)], 2147483648.0);
        Compare("conv_i4", [typeof(double)], double.NaN);
        Compare("conv_i4", [typeof(double)], double.PositiveInfinity);
        Compare("conv_i4", [typeof(double)], double.NegativeInfinity);
    }

    [SkippableFact]
    public void Conv_f8_matches_int_to_double_conversion()
    {
        Compare("conv_f8", [typeof(int)], int.MinValue);
        Compare("conv_f8", [typeof(int)], -1);
        Compare("conv_f8", [typeof(int)], 0);
        Compare("conv_f8", [typeof(int)], 1);
        Compare("conv_f8", [typeof(int)], int.MaxValue);
    }

    private static void Compare(string name, System.Type[] parameterTypes, object argument)
    {
        var qMethod = typeof(Mpl.Run.Primitive).GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, parameterTypes, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod(name, BindingFlags.Public | BindingFlags.Static,
            null, parameterTypes, null)!;
        MethodComparison.Compare(name + "(" + argument + ")", Contract,
            qMethod, null, [argument], mMethod, null, [argument]);
    }
}
