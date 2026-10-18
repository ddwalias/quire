using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreSimpleResultNodeFactoryCctorDifferentialTests
{
    [SkippableFact]
    public void SimpleResultNodeFactory_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.SimpleResultNodeFactory");
        string DefaultType(Type type) => type.GetField("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!.GetType().Name;

        MethodComparison.Compare("SimpleResultNodeFactory..cctor()", "Ptc.Calculation.Core.SimpleResultNodeFactory",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => DefaultType(quireType), _ => DefaultType(mathcadType));
    }
}
