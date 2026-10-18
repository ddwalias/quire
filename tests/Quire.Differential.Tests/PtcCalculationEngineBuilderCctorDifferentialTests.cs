using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationEngineBuilderCctorDifferentialTests
{
    [SkippableFact]
    public void EngineBuilder_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.EngineBuilder);
        var mathcadType = Mathcad.Type("Ptc.Calculation", "Ptc.Calculation.EngineBuilder");
        string Fields(Type type) => string.Join("|", new[]
        {
            "ImportRequestFormat", "unitRL", "unitSystemRL", "unitSystemDerivedRL", "unitSystemEmptyDerivedRL", "RL_NIL"
        }.Select(name => Reflect.Text(type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null))));

        MethodComparison.Compare("EngineBuilder..cctor()", "Ptc.Calculation.EngineBuilder",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Fields(quireType), _ => Fields(mathcadType));
    }
}
