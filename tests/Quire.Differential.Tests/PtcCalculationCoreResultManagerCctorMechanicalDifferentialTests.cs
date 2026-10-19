using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreResultManagerCctorMechanicalDifferentialTests
{
    private static string State(Type type)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
        object? instance = type.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        if (instance == null) return "null";
        Type instanceType = instance.GetType();
        int oldResultKey = (int)instanceType.GetField("_oldResultKey", flags)!.GetValue(instance)!;
        int resultKey = (int)instanceType.GetField("_resultKey", flags)!.GetValue(instance)!;
        object? results = instanceType.GetField("_results", flags)!.GetValue(instance);
        int count = results is System.Collections.IDictionary dictionary ? dictionary.Count : -1;
        return $"{instanceType.Name}:{oldResultKey}:{resultKey}:{count}";
    }

    [SkippableFact]
    public void cctor()
    {
        Type quireType = typeof(global::Quire.Calculation.Core.ResultManager);
        Type mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.ResultManager");
        MethodComparison.Compare("ResultManager..cctor()", "Ptc.Calculation.Core.ResultManager",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => State(quireType), _ => State(mathcadType));
    }
}
