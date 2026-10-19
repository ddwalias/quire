using System.Globalization;
using System.Reflection;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreResultNodeUtilsMechanicalDifferentialTests
{
    public class NodeProxy : DispatchProxy
    {
        public object? ValueObject { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => targetMethod?.Name switch
        {
            "get_Value" => ValueObject,
            _ => throw new NotSupportedException(targetMethod?.Name),
        };
    }

    private const string Contract = "Ptc.Calculation.Core.ResultNodeUtils";

    private static Type OriginalType() => Mathcad.Type("Ptc.Calculation.Core", Contract);

    private static MethodInfo QuireMethod() => typeof(global::Quire.Calculation.Core.ResultNodeUtils).GetMethod(
        "TryParsePlottableValue", BindingFlags.Static | BindingFlags.NonPublic, null,
        [typeof(global::Quire.Calculation.Core.IResultNode), typeof(double).MakeByRefType()], null)!;

    private static object Proxy(Type interfaceType, object? value = null)
    {
        object proxy = DispatchProxy.Create(interfaceType, typeof(NodeProxy));
        ((NodeProxy)proxy).ValueObject = value;
        return proxy;
    }

    private static string Number(double value) => double.IsNaN(value) ? "NaN" : value.ToString("R", CultureInfo.InvariantCulture);

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is double number) return Number(number);
        Type type = value.GetType();
        Type? unitedInterface = type.GetInterfaces().FirstOrDefault(i => i.Name == "IUnitedValueResultNode");
        if (unitedInterface != null)
            return "United:" + Show(unitedInterface.GetProperty("Value")!.GetValue(value));
        Type? realInterface = type.GetInterfaces().FirstOrDefault(i => i.Name == "IRealResultNode");
        if (realInterface != null)
            return "Real:" + Number((double)realInterface.GetProperty("Value")!.GetValue(value)!);
        if (type.GetInterfaces().Any(i => i.Name == "IComplexResultNode")) return "Complex";
        if (type.GetInterfaces().Any(i => i.Name == "IImaginaryResultNode")) return "Imaginary";
        return "UnknownResultNode";
    }

    private static void Compare(int token, string caseId, Type quireInterface, Type mathcadInterface,
        object? quireValue = null, object? mathcadValue = null)
    {
        Type originalType = OriginalType();
        MethodBase original = originalType.Module.ResolveMethod(token)!;
        object qNode = Proxy(quireInterface, quireValue);
        object mNode = Proxy(mathcadInterface, mathcadValue);
        MethodComparison.Compare(caseId, Contract, QuireMethod(), null, [qNode, 0.0], original, null, [mNode, 0.0], Show, Show);
    }

    [SkippableFact]
    public void TryParsePlottableValue_060001ff()
    {
        Type qReal = typeof(global::Quire.Calculation.Core.IRealResultNode);
        Type qComplex = typeof(global::Quire.Calculation.Core.IComplexResultNode);
        Type qImaginary = typeof(global::Quire.Calculation.Core.IImaginaryResultNode);
        Type qUnited = typeof(global::Quire.Calculation.Core.IUnitedValueResultNode);
        Type qUnknown = typeof(global::Quire.Calculation.Core.IResultNode);
        Type mReal = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IRealResultNode");
        Type mComplex = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IComplexResultNode");
        Type mImaginary = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IImaginaryResultNode");
        Type mUnited = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IUnitedValueResultNode");
        Type mUnknown = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.IResultNode");

        Compare(0x060001ff, "real result returns its scalar", qReal, mReal, 42.5, 42.5);
        Compare(0x060001ff, "complex result is plottable as NaN", qComplex, mComplex);
        Compare(0x060001ff, "imaginary result is plottable as NaN", qImaginary, mImaginary);
        object qNestedReal = Proxy(qReal, 17.25);
        object mNestedReal = Proxy(mReal, 17.25);
        Compare(0x060001ff, "united result delegates to its nested real value", qUnited, mUnited, qNestedReal, mNestedReal);
        Compare(0x060001ff, "unrecognized result returns false and NaN", qUnknown, mUnknown);
    }
}
