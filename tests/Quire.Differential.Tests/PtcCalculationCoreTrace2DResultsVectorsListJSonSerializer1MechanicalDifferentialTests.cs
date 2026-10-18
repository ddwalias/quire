using System.IO;
using System.Reflection;

namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1</c> members and its static initializer.
/// </summary>
public class PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests
{
    [SkippableFact]
    public void cctor_0600023f()
    {
        var quireType = typeof(global::Quire.Calculation.Core.Trace2DResults.VectorsListJSonSerializer<>).MakeGenericType(typeof(double));
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1").MakeGenericType(typeof(double));

        string Field(Type type) => type.GetField("_jSonVectorsListSerializer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.GetValue(null)!.GetType().Name;

        MethodComparison.Compare("VectorsListJSonSerializer<double>..cctor()",
            "Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Field(quireType), _ => Field(mathcadType));
    }

    [SkippableFact]
    public void Write_0600023d()
    {
        var quireType = typeof(global::Quire.Calculation.Core.Trace2DResults.VectorsListJSonSerializer<>).MakeGenericType(typeof(double));
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1").MakeGenericType(typeof(double));
        var quireWrite = quireType.GetMethod("Write", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null, [typeof(Stream), typeof(double[])], null)!;
        var mathcadWrite = mathcadType.GetMethod("Write", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null, [typeof(Stream), typeof(double[])], null)!;

        static string Show(object? value) => value is MemoryStream stream
            ? Convert.ToHexString(stream.ToArray())
            : Reflect.Text(value);

        foreach (var values in new[] { Array.Empty<double>(), [0d], [1.5d, -2.25d, 3d] })
        {
            var quireStream = new MemoryStream();
            var mathcadStream = new MemoryStream();
            MethodComparison.Compare($"Write(double[{values.Length}])",
                "Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1",
                quireWrite, null, [quireStream, values], mathcadWrite, null, [mathcadStream, values], Show, Show);
        }
    }
}
