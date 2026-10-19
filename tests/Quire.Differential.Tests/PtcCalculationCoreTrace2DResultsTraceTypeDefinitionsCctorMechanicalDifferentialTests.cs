using System.Collections;
using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsCctorMechanicalDifferentialTests
{
    [SkippableFact]
    public void TraceTypeDefinitions_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.Trace2DResults.TraceTypeDefinitions);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Trace2DResults.TraceTypeDefinitions");

        string Ranges(Type type)
        {
            var dictionary = (IEnumerable)type.GetField("_traceColumnsRanges",
                BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
            return string.Join("|", dictionary.Cast<object>().Select(entry =>
            {
                Type pairType = entry.GetType();
                int key = Convert.ToInt32(pairType.GetProperty("Key")!.GetValue(entry));
                object range = pairType.GetProperty("Value")!.GetValue(entry)!;
                int start = (int)Reflect.Get(range, "Start")!;
                int end = (int)Reflect.Get(range, "End")!;
                return (key, start, end);
            }).OrderBy(row => row.key).Select(row => $"{row.key}:{row.start}:{row.end}"));
        }

        MethodComparison.Compare("TraceTypeDefinitions..cctor()", "Ptc.Calculation.Core.Trace2DResults.TraceTypeDefinitions",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Ranges(quireType), _ => Ranges(mathcadType));
    }
}
