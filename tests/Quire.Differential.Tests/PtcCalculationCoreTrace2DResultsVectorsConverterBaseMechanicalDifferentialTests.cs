using System;
using System.Reflection;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.Trace2DResults.VectorsConverterBase</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.Trace2DResults.VectorsConverterBase.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreTrace2DResultsVectorsConverterBaseMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000272() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.Trace2DResults.VectorsConverterBase), 0x06000272);

    [SkippableTheory]
    [InlineData(0u, 0u)]
    [InlineData(2u, 2u)]
    [InlineData(4u, 2u)]
    [InlineData(6u, 2u)]
    [InlineData(6u, 4u)]
    [InlineData(32u, 32u)]
    [InlineData(1u, 0u)]
    public void IsOfExactPlainType_06000271(uint vectorTypes, uint singleType)
    {
        Mathcad.Require();
        Type qType = typeof(global::Quire.Calculation.Core.Trace2DResults.VectorsConverterBase);
        Type mType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Trace2DResults.VectorsConverterBase");
        Type qPlainTypes = typeof(global::Quire.Calculation.Core.PlainDataTypes);
        Type mPlainTypes = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.PlainDataTypes");
        MethodInfo qMethod = qType.GetMethod("IsOfExactPlainType", BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodInfo mMethod = mType.GetMethod("IsOfExactPlainType", BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodComparison.Compare($"0x{vectorTypes:x} is exact 0x{singleType:x}", "Ptc.Calculation.Core.Trace2DResults.VectorsConverterBase",
            qMethod, null, [Enum.ToObject(qPlainTypes, vectorTypes), Enum.ToObject(qPlainTypes, singleType)],
            mMethod, null, [Enum.ToObject(mPlainTypes, vectorTypes), Enum.ToObject(mPlainTypes, singleType)], ShowEnum, ShowEnum);
    }

    private static string ShowEnum(object? value) => value is Enum enumValue
        ? Convert.ToUInt32(enumValue).ToString(System.Globalization.CultureInfo.InvariantCulture)
        : value?.ToString() ?? "null";
}
