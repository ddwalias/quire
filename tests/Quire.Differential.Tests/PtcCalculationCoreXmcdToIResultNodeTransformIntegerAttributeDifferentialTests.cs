using System.Reflection;
using System.Xml;
using Quire.Calculation.Core;

namespace Quire.Differential.Tests;

/// <summary>Direct Mathcad comparisons for the private integer XML-attribute parser.</summary>
public class PtcCalculationCoreXmcdToIResultNodeTransformIntegerAttributeDifferentialTests
{
    private const string Contract = "Ptc.Calculation.Core.XmcdToIResultNodeTransform";
    private static Type Q => typeof(XmcdToIResultNodeTransform);
    private static Type M => Mathcad.Type("Ptc.Calculation.Core", Contract);

    [SkippableFact]
    public void GetIntegerFromAttribute_06000239()
    {
        Mathcad.Require();
        MethodInfo qMethod = Q.GetMethod("GetIntegerFromAttribute", BindingFlags.NonPublic | BindingFlags.Static)!;
        MethodInfo mMethod = M.GetMethod("GetIntegerFromAttribute", BindingFlags.NonPublic | BindingFlags.Static)!;
        Compare(qMethod, mMethod, "<unit power-numerator=\"12\" />", "power-numerator", 4, "valid integer");
        Compare(qMethod, mMethod, "<unit power-numerator=\" -17 \" />", "power-numerator", 4, "signed integer with whitespace");
        Compare(qMethod, mMethod, "<unit />", "power-numerator", -3, "missing attribute");
        Compare(qMethod, mMethod, "<unit power-numerator=\"12x\" />", "power-numerator", 4, "invalid integer");
        Compare(qMethod, mMethod, "<unit power-numerator=\"2147483648\" />", "power-numerator", 4, "overflow");
        MethodComparison.Compare("null XmlNode", Contract, qMethod, null, [null, "power-numerator", 4],
            mMethod, null, [null, "power-numerator", 4], Show, Show);
    }

    private static void Compare(MethodInfo qMethod, MethodInfo mMethod, string xml,
        string name, int fallback, string label)
    {
        MethodComparison.Compare("GetIntegerFromAttribute " + label, Contract, qMethod, null,
            [Node(xml), name, fallback], mMethod, null, [Node(xml), name, fallback], Show, Show);
    }

    private static XmlNode Node(string xml)
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return document.DocumentElement!;
    }

    private static string Show(object? value) => value is XmlNode node ? node.OuterXml : Reflect.Text(value);
}
