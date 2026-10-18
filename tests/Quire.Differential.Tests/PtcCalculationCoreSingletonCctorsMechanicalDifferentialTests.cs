using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreSingletonCctorsMechanicalDifferentialTests
{
    private static void CompareCctor(Type quireType, Type mathcadType, string contract, string field)
    {
        string Field(Type type)
        {
            object? value = type.GetField(field,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null);
            if (value == null) return "null";
            var propertyName = value.GetType().GetProperty("PropertyName")?.GetValue(value);
            return $"{value.GetType().Name}:{propertyName ?? "<no PropertyName>"}";
        }

        MethodComparison.Compare($"{quireType.Name}..cctor()", contract,
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Field(quireType), _ => Field(mathcadType));
    }

    [SkippableFact]
    public void CalculatingStatusChangedEventArgs_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.CalculatingStatusChangedEventArgs);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.CalculatingStatusChangedEventArgs");
        CompareCctor(quireType, mathcadType, "Ptc.Calculation.Core.CalculatingStatusChangedEventArgs", "_singleton");
    }

    [SkippableFact]
    public void ErrorChangedEventArgs_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.ErrorChangedEventArgs);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ErrorChangedEventArgs");
        CompareCctor(quireType, mathcadType, "Ptc.Calculation.Core.ErrorChangedEventArgs", "_singleton");
    }

    [SkippableFact]
    public void SyncStatusChangedEventArgs_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.SyncStatusChangedEventArgs);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.SyncStatusChangedEventArgs");
        CompareCctor(quireType, mathcadType, "Ptc.Calculation.Core.SyncStatusChangedEventArgs", "_singleton");
    }
}
