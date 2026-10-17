using System.Globalization;
using System.Reflection;
using Quire.Mcd;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveValueDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private static readonly System.Type[] IntParameters = [typeof(int)];
    private static readonly System.Type[] ObjectParameters = [typeof(object)];

    [SkippableFact]
    public void Integer_value_factory_matches_numval_construction()
    {
        CompareIntegerValue(int.MinValue);
        CompareIntegerValue(-1);
        CompareIntegerValue(0);
        CompareIntegerValue(42);
        CompareIntegerValue(int.MaxValue);
    }

    [SkippableFact]
    public void Numeric_truth_conversion_matches_scalar_and_error_cases()
    {
        CompareTruth(0.0, 0.0);
        CompareTruth(-0.0, 0.0);
        CompareTruth(1.0, 0.0);
        CompareTruth(0.0, -1.0);
        CompareTruth(double.NaN, 0.0);
        CompareTruth(double.PositiveInfinity, double.NegativeInfinity);
        CompareTruth(null, null);
        CompareTruth("not scalar", "not scalar");
        CompareTruth((object)17, (object)17);
    }

    private static void CompareIntegerValue(int value)
    {
        var qMethod = typeof(Primitive).GetMethod("iv", BindingFlags.Public | BindingFlags.Static,
            null, IntParameters, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod("iv", BindingFlags.Public | BindingFlags.Static,
            null, IntParameters, null)!;
        MethodComparison.Compare("iv(" + value + ")", Contract, qMethod, null, [value], mMethod, null, [value],
            result => DescribeNumVal(result, typeof(Dim)), result => DescribeNumVal(result, M("Mcd.Dim")));
    }

    private static void CompareTruth(double real, double imaginary)
    {
        var qValue = new NumVal(real, imaginary);
        var mValue = New(M("Mcd.NumVal"), ["Double", "Double"], real, imaginary);
        CompareTruthValues(qValue, mValue, "NumVal(" + real + ", " + imaginary + ")");
    }

    private static void CompareTruth(object? qValue, object? mValue) =>
        CompareTruthValues(qValue, mValue, "invalid scalar input");

    private static void CompareTruthValues(object? qValue, object? mValue, string scenario)
    {
        var qMethod = typeof(Primitive).GetMethod("with_bool", BindingFlags.Public | BindingFlags.Static,
            null, ObjectParameters, null)!;
        var mMethod = M("Mpl.Run.Primitive").GetMethod("with_bool", BindingFlags.Public | BindingFlags.Static,
            null, ObjectParameters, null)!;
        object qTrue = Get(typeof(True), "singleton")!;
        object mTrue = Get(M("Mpl.Run.True"), "singleton")!;
        static string Show(object? value, object expectedTrue) => value == null ? "null"
            : ReferenceEquals(value, expectedTrue) ? "True.singleton" : Describe(value);
        MethodComparison.Compare("with_bool " + scenario, Contract, qMethod, null, [qValue], mMethod, null, [mValue],
            value => Show(value, qTrue), value => Show(value, mTrue));
    }

    private static string DescribeNumVal(object? value, System.Type dimType)
    {
        if (value == null) return "null";
        if (value.GetType().Name != "NumVal") return Describe(value);
        object data = Get(value, "Data")!;
        object? dimension = Get(value, "m_dim");
        string[] markers = ["identity", "mcd_zero", "string_dim", "function_dim", "mixed_matrix_dim"];
        string dim = "null";
        foreach (string marker in markers)
            if (ReferenceEquals(dimension, Get(dimType, marker))) dim = marker;
        if (dim == "null" && dimension != null)
        {
            var vector = (int[])Get(dimension, "m_vec")!;
            dim = "[" + string.Join(",", vector) + "]";
        }
        return "NumVal[rp=" + Convert.ToString(Get(data, "rp"), CultureInfo.InvariantCulture)
            + ",ip=" + Convert.ToString(Get(data, "ip"), CultureInfo.InvariantCulture) + ",dim=" + dim + "]";
    }
}
