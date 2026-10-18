using System.Reflection;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class RngValConversionsDifferentialTests
{
    private const string Contract = "Mcd.RngVal";
    private const BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void Range_to_vector_and_matrix_conversions_match()
    {
        Runtime.Sync();
        var methods = new[] { "asVector", "asMatrix", "asRealMatrix", "asRealVector" };
        var ranges = new[]
        {
            ("ordinary", MakeRange(0.25, 0.5, 4, "L")),
            ("interpolated", MakeInterpolated(-1.0, 2.0, 4, "T")),
            ("empty", MakeRange(0.0, 1.0, 0, "identity"))
        };

        foreach (var (rangeKind, rangesForSide) in ranges)
        foreach (string name in methods)
        {
            var (qRange, mRange) = rangesForSide;
            MethodInfo qMethod = typeof(global::Quire.Mcd.RngVal).GetMethod(name, InstancePublic, [typeof(string), typeof(uint)])!;
            MethodInfo mMethod = Mathcad.Type("mplrun", "Mcd.RngVal").GetMethod(name, InstancePublic, [typeof(string), typeof(uint)])!;
            MethodComparison.Compare($"RngVal.{name}({rangeKind})", Contract,
                qMethod, qRange, ["ignored", 0xA5u], mMethod, mRange, ["ignored", 0xA5u],
                value => Show(Side.Quire, value), value => Show(Side.Mathcad, value),
                target => Side.Quire.Show(target), target => Side.Mathcad.Show(target));
        }
    }

    private static (object Quire, object Mathcad) MakeRange(double first, double increment, int count, string dimension)
    {
        return (
            New(typeof(global::Quire.Mcd.RngVal), first, increment, count, Side.Quire.D(dimension)!),
            New(Mathcad.Type("mplrun", "Mcd.RngVal"), first, increment, count, Side.Mathcad.D(dimension)!));
    }

    private static (object Quire, object Mathcad) MakeInterpolated(double first, double last, int intervals, string dimension)
    {
        return (
            New(typeof(global::Quire.Mcd.RngValInterpolate), first, last, intervals, Side.Quire.D(dimension)!),
            New(Mathcad.Type("mplrun", "Mcd.RngValInterpolate"), first, last, intervals, Side.Mathcad.D(dimension)!));
    }

    private static string ShowMatrix(Side side, object? value)
    {
        if (value == null) return "null";
        int rows = (int)Get(value, "Rows")!;
        int cols = (int)Get(value, "Cols")!;
        int stride = (int)Get(Get(value, "Data")!, "m_anr")!;
        int offset = (int)Get(value, "m_offset")!;
        var real = (double[]?)Get(value, "m_rarr");
        string values = real == null ? "null" : string.Join(",", Enumerable.Range(0, rows * cols).Select(i =>
        {
            int row = i % rows;
            int col = i / rows;
            return Side.Bits(real[offset + row + col * stride]);
        }));
        return $"MatVal[{rows}x{cols}; values={values}; dim={side.DimText(Get(value, "m_dim"))}]";
    }

    private static string Show(Side side, object? value) =>
        value?.GetType().Name == "MatVal" ? ShowMatrix(side, value) : side.Show(value);
}
