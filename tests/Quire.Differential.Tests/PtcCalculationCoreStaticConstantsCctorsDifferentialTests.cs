using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreStaticConstantsCctorsDifferentialTests
{
    private static void CompareCctor(Type quireType, Type mathcadType, string contract, params string[] fields)
    {
        string Values(Type type) => string.Join("|", fields.Select(name =>
            Reflect.Text(type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null))));

        MethodComparison.Compare($"{quireType.Name}..cctor()", contract,
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Values(quireType), _ => Values(mathcadType));
    }

    [SkippableFact]
    public void CalcResultsProcessingPriorities_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.CalcResultsProcessingPriorities);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.CalcResultsProcessingPriorities");
        CompareCctor(quireType, mathcadType, "Ptc.Calculation.Core.CalcResultsProcessingPriorities",
            "DefaultResultDispatchPriority_Background", "ShadowResultsProcessingPriority_Send");
    }

    [SkippableFact]
    public void CalculationOptionDefaults_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.CalculationOptionDefaults);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.CalculationOptionDefaults");
        CompareCctor(quireType, mathcadType, "Ptc.Calculation.Core.CalculationOptionDefaults", "SymbolicResultMaximumSize");
    }
}
