#nullable disable
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Quire;
using Quire.Mirror;
using Quire.Calculation.Core.Trace2DResults;
using Quire.MindTouch.IO;
using Quire.PersistentDataObjects;
using Quire.Xml;

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
        int result;
        XmlAttribute xmlAttribute = node.Attributes[attributeName];
        if (xmlAttribute == null || !int.TryParse(xmlAttribute.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
        {
            result = defaultValue;
        }

        return result;
    }

    private static bool TryParseUsingIntermediate<TIntermediate, TFinal>(string stringValue, out TFinal finalType)
        where TIntermediate : struct
        where TFinal : struct
    {
        finalType = default;
        if (EnumHelper.TryParse<TIntermediate>(stringValue, out var intermediate, EnumHelper.SettingsWithCache))
        {
            return EnumHelper.TryParse<TFinal>(intermediate, out finalType, EnumHelper.SettingsWithCache);
        }
        return false;
    }

    private static void ReadUnitMonomial(IUnitedValueResultNode uvResult, XmlNode unitMonomial)
    {
        foreach (XmlNode node in unitMonomial.ChildNodes)
        {
            string unit = node.Attributes["unit"].Value;
            uvResult.AddUnit(unit, GetUnitPowerNumerator(node), GetUnitPowerDenominator(node));
        }
    }

    private void AddElementsToMatrix(IEnumerable<XmlNode> elements, IEagerMatrixResultNode matrix)
    {
        int column = 0;
        int row = 0;
        foreach (XmlNode element in elements)
        {
            matrix.SetElementAt(row, column, MakeResultNode(element));
            row++;
            if (row == matrix.Rows)
            {
                column++;
                row = 0;
            }
        }
    }

    private static IList<IListResultNode> ReadJsonVectorsAsStream(XmlNode node, ResultValueType resultType, int singleVectorLength)
    {
        IList<IListResultNode> result;
        using (ChunkedMemoryStream jStream = new ChunkedMemoryStream())
        {
            using (StreamWriter writer = new StreamWriter(jStream, new UTF8Encoding(false)))
            {
                _resultVectorUnescapingXslt.Transform(node, new XsltArgumentList(), writer);
                result = ReadJson(jStream, resultType, singleVectorLength);
            }
        }
        return result;
    }

    private static IList<IListResultNode> ReadJson(Stream jStream, ResultValueType resultType, int singleVectorLength)
    {
        IList<IListResultNode> result;
        jStream.Position = 0L;
        switch (resultType)
        {
            case ResultValueType.String:
                result = VectorsListJSonSerializer<string>.Read(jStream, singleVectorLength);
                break;
            case ResultValueType.Numeric:
            case ResultValueType.NaNs:
                DebugTools.DevAssert(resultType == ResultValueType.Numeric);
                result = VectorsListJSonSerializer<double>.Read(jStream, singleVectorLength);
                break;
            default:
                DebugTools.FailAssertion(resultType.ToString());
                result = null;
                break;
        }
        return result;
    }

    private IResultNode BuildComplex(XmlNode node)
    {
        if (node.ChildNodes.Count == 1)
        {
            return MakeResultNode(node.ChildNodes[0]);
        }
        return _resultNodeFactory.CreateComplexResultNode(double.Parse(node.ChildNodes[0].InnerText, NumberFormatInfo.InvariantInfo), double.Parse(node.ChildNodes[1].InnerText, NumberFormatInfo.InvariantInfo));
    }

    private static IVectorListResultNode BuildDataVectors(XmlNode dataNode, string vectorsName)
    {
        string xpath = string.Format("{0}:{1}/{0}:{2}", "ml", vectorsName, "DataVectors");
        XmlNode node = dataNode.SelectSingleNode(xpath, XmlWorksheetData.McdxNameSpaceManager);
        if (node != null)
        {
            return BuildDataVectors(node);
        }
        return null;
    }

    private static IVectorListResultNode BuildDataVectors(XmlNode node)
    {
        int vectorCount = int.Parse(node.Attributes["Count"].Value, NumberFormatInfo.InvariantInfo);
        string vectorsTypeText = node.Attributes["VectorsType"].Value;
        if (!TryParseUsingIntermediate<VectorsTypeEnum, VectorsType>(vectorsTypeText, out VectorsType vectorsType))
        {
            throw new InvalidOperationException(string.Format(ErrorAttributeConversionToEnum, "VectorsType", vectorsTypeText, "DataVectors"));
        }
        int vectorLength = int.Parse(node.Attributes["VectorLength"].Value, NumberFormatInfo.InvariantInfo);
        string valuesTypeText = node.Attributes["ValuesType"].Value;
        if (!TryParseUsingIntermediate<ValuesTypeEnum, ResultValueType>(valuesTypeText, out ResultValueType valuesType))
        {
            throw new InvalidOperationException(string.Format(ErrorAttributeConversionToEnum, "ValuesType", valuesTypeText, node.Name));
        }
        IList<IListResultNode> vectors = ReadJsonVectorsAsStream(node, valuesType, vectorLength);
        if (vectors.Count != vectorCount)
        {
            throw new InvalidOperationException(string.Format("Logic error: result for {0} node expected to be generated with {1} vectors but has {2}", "DataVectors", vectorCount, vectors.Count));
        }
        return new VectorListResultNode(vectors, vectorLength, vectorsType);
    }

    public IResultNode Transform(string xmcdResult)
    {
        if (string.IsNullOrEmpty(xmcdResult))
        {
            return Transform((XmlNode)null);
        }
        XmlDocument document = new XmlDocument();
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
        namespaceManager.AddNamespace("ml", "foo");
        namespaceManager.AddNamespace("u", "bar");
        XmlParserContext context = new XmlParserContext(null, namespaceManager, "en", XmlSpace.None);
        XmlTextReader reader = new XmlTextReader(xmcdResult, XmlNodeType.Element, context);
        IResultNode result;
        try
        {
            document.Load(reader);
            result = Transform(document);
        }
        finally
        {
            if (reader != null)
            {
                ((IDisposable)reader).Dispose();
            }
        }
        return result;
    }

    public IResultNode Transform(XmlNode node)
    {
        if (node == null || node.FirstChild == null)
        {
            return _resultNodeFactory.CreateEmptyResultNode();
        }
        return MakeResultNode(node.FirstChild);
    }

    private IResultNode MakeResultNode(XmlNode node)
    {
        switch (node.LocalName)
        {
            case "real":
                return _resultNodeFactory.CreateRealResultNode(double.Parse(node.InnerText, NumberFormatInfo.InvariantInfo));
            case "imag":
                return _resultNodeFactory.CreateImaginaryResultNode(double.Parse(node.InnerText, NumberFormatInfo.InvariantInfo));
            case "complex":
                return BuildComplex(node);
            case "str":
                return _resultNodeFactory.CreateStringResultNode(node.InnerText);
            case "functionValue":
                return _resultNodeFactory.CreateFunctionResultNode(node.Attributes["description"].Value);
            case "unitedValue":
                return BuildUnitedValue(node);
            case "matrix":
                return BuildMatrix(node);
            case "DataVectors":
                return BuildDataVectors(node);
            case "Trace2dResult":
                return BuildPlotTraceResult(node);
            case "symResult":
                return BuildSymbolic(node);
            case "placeholder":
                return _resultNodeFactory.CreateEmptyResultNode();
            default:
                throw new InvalidOperationException(string.Format("Logic error: xmcd format result contained an unexpected element ('{0}')", node.LocalName));
        }
    }
}
