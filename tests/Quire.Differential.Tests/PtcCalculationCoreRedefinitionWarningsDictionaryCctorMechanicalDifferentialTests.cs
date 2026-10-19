using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreRedefinitionWarningsDictionaryCctorMechanicalDifferentialTests
{
    [SkippableFact]
    public void RedefinitionWarningsDictionary_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.RedefinitionWarningsDictionary);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.RedefinitionWarningsDictionary");

        string Types(Type type) => string.Join("|", ((Type[])type.GetField("KeysOwnerTypes",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!).Select(t => t.Name));

        MethodComparison.Compare("RedefinitionWarningsDictionary..cctor()", "Ptc.Calculation.Core.RedefinitionWarningsDictionary",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Types(quireType), _ => Types(mathcadType));
    }
}
