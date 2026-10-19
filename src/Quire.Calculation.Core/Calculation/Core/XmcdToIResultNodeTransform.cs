#nullable disable
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class XmcdToIResultNodeTransform
{
    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformCctorMechanicalDifferentialTests")]
    static XmcdToIResultNodeTransform()
    {
        const string template = "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">\r\n                                     <xsl:output method=\"text\" omit-xml-declaration=\"yes\"/>\r\n                                     <xsl:template match=\"{0}\">\r\n                                       <xsl:value-of select=\"node()\"/>\r\n                                     </xsl:template>\r\n                                     </xsl:stylesheet>";
        string stylesheet = string.Format(template, "DataVectors");
        var transform = new XslCompiledTransform();
        using (var textReader = new StringReader(stylesheet))
        using (var xmlReader = XmlReader.Create(textReader))
        {
            transform.Load(xmlReader);
        }
        _resultVectorUnescapingXslt = transform;
    }

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private readonly IResultNodeFactory _resultNodeFactory;

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    public XmcdToIResultNodeTransform(IResultNodeFactory resultNodeFactory)
    {
        _resultNodeFactory = resultNodeFactory;
    }

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private static int GetUnitPowerDenominator(XmlNode node) => GetIntegerFromAttribute(node, "power-denominator", 1);

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private static int GetUnitPowerNumerator(XmlNode node) => GetIntegerFromAttribute(node, "power-numerator", 1);

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests")]
    private ISymbolicResultNode BuildSymbolic(XmlNode node) => _resultNodeFactory.CreateSymbolicResultNode(node.InnerXml);

    [MirrorVerified("PtcCalculationCoreXmcdToIResultNodeTransformIntegerAttribute_MechanicalDifferentialTests")]
    private static int GetIntegerFromAttribute(XmlNode node, string attributeName, int defaultValue)
    {
        XmlAttribute xmlAttribute = node.Attributes[attributeName];
        if (xmlAttribute == null || !int.TryParse(xmlAttribute.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int result))
        {
            return defaultValue;
        }

        return result;
    }
}
