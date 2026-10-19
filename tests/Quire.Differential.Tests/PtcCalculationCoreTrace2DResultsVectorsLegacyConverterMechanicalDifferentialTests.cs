using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.Trace2DResults.VectorsLegacyConverter";
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public class ResultNodeProxy : DispatchProxy
    {
        public object? ValueObject { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => targetMethod?.Name switch
        {
            "get_Value" => ValueObject,
            _ => throw new NotSupportedException(targetMethod?.Name),
        };
    }

    private static Type QType() => typeof(global::Quire.Calculation.Core.Trace2DResults.VectorsLegacyConverter);
    private static Type MType() => Mathcad.Type("Ptc.Calculation.Core", Contract);

    private static Type QInterface(string name) => typeof(global::Quire.Calculation.Core.IResultNode).Assembly
        .GetType("Quire.Calculation.Core." + name)!;
    private static Type MInterface(string name) => Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core." + name);

    private static object Proxy(Type interfaceType, object? value = null)
    {
        object proxy = DispatchProxy.Create(interfaceType, typeof(ResultNodeProxy));
        ((ResultNodeProxy)proxy).ValueObject = value;
        return proxy;
    }

    private static string Show(object? value)
    {
        if (value == null) return "null";
        if (value is double number) return double.IsNaN(number) ? "NaN" : number.ToString("R", CultureInfo.InvariantCulture);
        if (value is string text) return "String:" + text;
        Type[] interfaces = value.GetType().GetInterfaces();
        if (interfaces.Any(i => i.Name == "IComplexResultNode")) return "Complex";
        if (interfaces.Any(i => i.Name == "IImaginaryResultNode")) return "Imaginary";
        Type? real = interfaces.FirstOrDefault(i => i.Name == "IRealResultNode");
        if (real != null) return "Real:" + Show(real.GetProperty("Value")!.GetValue(value));
        Type? stringNode = interfaces.FirstOrDefault(i => i.Name == "IStringResultNode");
        if (stringNode != null) return "String:" + stringNode.GetProperty("Value")!.GetValue(value);
        return "ResultNode";
    }

    private static MethodInfo QMethod(string name) => QType().GetMethod(name, Static)!;

    private static void Compare(string name, int token, string caseId, string interfaceName, object? value = null)
    {
        object qNode = Proxy(QInterface(interfaceName), value);
        object mNode = Proxy(MInterface(interfaceName), value);
        object? initialOut = name == "ReadDoubleOrNone" ? double.NaN : null;
        MethodComparison.Compare(caseId, Contract, QMethod(name), null, [qNode, initialOut],
            MType().Module.ResolveMethod(token)!, null, [mNode, initialOut], Show, Show);
    }

    [SkippableFact]
    public void ReadAsColumns_get_06000265()
    {
        Type qType = QType();
        Type mType = MType();
        object qTarget = RuntimeHelpers.GetUninitializedObject(qType);
        object mTarget = RuntimeHelpers.GetUninitializedObject(mType);
        qType.GetField("_readAsColumns", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(qTarget, true);
        mType.GetField("_readAsColumns", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(mTarget, true);
        MethodInfo qGetter = qType.GetProperty("ReadAsColumns")!.GetMethod!;
        MethodInfo mGetter = mType.GetProperty("ReadAsColumns")!.GetMethod!;
        string State(object target) => target.GetType().GetField("_readAsColumns", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target)!.ToString()!;
        MethodComparison.Compare("ReadAsColumns true", Contract, qGetter, qTarget, [],
            mGetter, mTarget, [], Reflect.Text, Reflect.Text, State, State);
    }

    [SkippableFact]
    public void ReadDoubleOrNone_0600026c()
    {
        Compare("ReadDoubleOrNone", 0x0600026c, "null is not a double", "IResultNode");
        Compare("ReadDoubleOrNone", 0x0600026c, "real values are returned", "IRealResultNode", 12.5);
        Compare("ReadDoubleOrNone", 0x0600026c, "real NaN is retained", "IRealResultNode", double.NaN);
        Compare("ReadDoubleOrNone", 0x0600026c, "complex values map to NaN", "IComplexResultNode");
        Compare("ReadDoubleOrNone", 0x0600026c, "imaginary values map to NaN", "IImaginaryResultNode");
        Compare("ReadDoubleOrNone", 0x0600026c, "strings are rejected", "IStringResultNode", "x");
    }

    [SkippableFact]
    public void ReadString_0600026d()
    {
        Compare("ReadString", 0x0600026d, "null is not a string", "IResultNode");
        Compare("ReadString", 0x0600026d, "strings are returned", "IStringResultNode", "x");
        Compare("ReadString", 0x0600026d, "real values are rejected", "IRealResultNode", 12.5);
        Compare("ReadString", 0x0600026d, "real NaN is a missing string", "IRealResultNode", double.NaN);
        Compare("ReadString", 0x0600026d, "complex values are missing strings", "IComplexResultNode");
        Compare("ReadString", 0x0600026d, "imaginary values are missing strings", "IImaginaryResultNode");
    }
}
