using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationMatrixValueProxyWrapperCctorMechanicalDifferentialTests
{
    [SkippableFact]
    public void MatrixValueProxyWrapper_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.MatrixValueProxyWrapper);
        var mathcadType = Mathcad.Type("Ptc.Calculation", "Ptc.Calculation.MatrixValueProxyWrapper");

        string Threshold(Type type)
        {
            object options = type.GetField("_conversionOptions", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
            return Reflect.Text(Reflect.Get(options, "ImaginaryThreshold"));
        }

        MethodComparison.Compare("MatrixValueProxyWrapper..cctor()", "Ptc.Calculation.MatrixValueProxyWrapper",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Threshold(quireType), _ => Threshold(mathcadType));
    }
}
