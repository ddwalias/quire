#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Quire.Calculation.Core.Trace2DResults;
using Quire.MindTouch.IO;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Calculation.Core;

public partial class ResultNodeToXmcdTransform
{
    private const string RealElementName = "real";
    private const string ImaginaryElementName = "imag";
    private const string ComplexElementName = "complex";
    private const string StringElementName = "str";
    private const string FunctionElementName = "functionValue";
    private const string MatrixElementName = "matrix";
    private const string SymbolicResultElementName = "symResult";
    private const string UnitedValueElementName = "unitedValue";
    private const string UnitMonomialElementName = "unitMonomial";
    private const string UnitReferenceElementName = "unitReference";
    private const string PlaceholderElementName = "placeholder";
    private const string MatrixViewportElementName = "display";
    private const string UnitAttributeName = "unit";
    private const string PowerNumeratorAttributeName = "power-numerator";
    private const string PowerDenominatorAttributeName = "power-denominator";
    private const string RowsAttributeName = "rows";
    private const string ColsAttributeName = "cols";
    private const string DescriptionAttributeName = "description";
    private const string OffsetAttributeName = "offset";
    private const string SizeAttributeName = "size";
    private const string DataVectorsElementName = "DataVectors";
    private const string ResultValueTypeAttributeName = "ValuesType";
    private const string VectorsTypeAttributeName = "VectorsType";
    private const string CountAttributeName = "Count";
    private const string MathPrefix = "ml";
    public const string MathNamespaceUri = "http://schemas.mathsoft.com/math50";
    private const string UnitPrefix = "u";
    public const string UnitNamespaceUri = "http://schemas.mathsoft.com/units10";

    public XmlWriter Writer { get; private set; }

    static ResultNodeToXmcdTransform()
    {
    }

    public ResultNodeToXmcdTransform(XmlWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException();
        }
        Writer = writer;
    }

    public static void Transform(IResultNode resultNode, XmlWriter writer)
    {
        if (resultNode == null)
        {
            return;
        }
        ResultNodeToXmcdTransform visitor = new ResultNodeToXmcdTransform(writer);
        resultNode.Accept(visitor);
    }

    public static string Transform(IResultNode resultNode)
    {
        if (resultNode == null)
        {
            return string.Empty;
        }
        StringBuilder stringBuilder = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings
        {
            CloseOutput = true,
            ConformanceLevel = ConformanceLevel.Fragment
        };
        using (XmlWriter writer = XmlWriter.Create(stringBuilder, settings))
        {
            ResultNodeToXmcdTransform visitor = new ResultNodeToXmcdTransform(writer);
            resultNode.Accept(visitor);
        }
        return stringBuilder.ToString();
    }

    private void WritePlaceholderXml()
    {
        Writer.WriteElementString("ml", "placeholder", "http://schemas.mathsoft.com/math50", string.Empty);
    }

    private void WriteRealXml(double value)
    {
        Writer.WriteStartElement("ml", "real", "http://schemas.mathsoft.com/math50");
        Writer.WriteValue(value.ToString("R", NumberFormatInfo.InvariantInfo));
        Writer.WriteEndElement();
    }

    private void WriteImaginaryXml(double value)
    {
        Writer.WriteStartElement("ml", "imag", "http://schemas.mathsoft.com/math50");
        Writer.WriteValue(value.ToString("R", NumberFormatInfo.InvariantInfo));
        Writer.WriteEndElement();
    }

    private void WriteMatrixXml(IMatrixResultNode matrix)
    {
        Writer.WriteStartElement("ml", "matrix", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("rows");
        Writer.WriteValue(matrix.Rows);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("cols");
        Writer.WriteValue(matrix.Columns);
        Writer.WriteEndAttribute();
        for (int i = 0; i < matrix.Columns; i++)
        {
            for (int j = 0; j < matrix.Rows; j++)
            {
                matrix[j, i].Accept(this);
            }
        }
        Writer.WriteEndElement();
    }

    private void WriteDisplayMatrixXml(IDisplayMatrixResultNode displayMatrix)
    {
        if (displayMatrix.Viewport.Size.Equals(displayMatrix.Size))
        {
            WriteMatrixXml(displayMatrix);
            return;
        }
        Writer.WriteStartElement("ml", "matrix", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("rows");
        Writer.WriteValue(displayMatrix.Rows);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("cols");
        Writer.WriteValue(displayMatrix.Columns);
        Writer.WriteEndAttribute();
        Writer.WriteStartElement("ml", "display", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("offset");
        Writer.WriteValue(displayMatrix.ViewportOffset.ToString());
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("size");
        Writer.WriteValue(displayMatrix.Viewport.Size.ToString());
        Writer.WriteEndAttribute();
        Writer.WriteEndElement();
        for (int i = 0; i < displayMatrix.Viewport.Columns; i++)
        {
            for (int j = 0; j < displayMatrix.Viewport.Rows; j++)
            {
                displayMatrix.Viewport[j, i].Accept(this);
            }
        }
        Writer.WriteEndElement();
    }

    public void Visit(IEmptyResultNode empty)
    {
        WritePlaceholderXml();
    }

    public void Visit(IRealResultNode real)
    {
        WriteRealXml(real.Value);
    }

    public void Visit(IImaginaryResultNode imag)
    {
        WriteImaginaryXml(imag.Value);
    }

    public void Visit(IComplexResultNode complex)
    {
        Writer.WriteStartElement("ml", "complex", "http://schemas.mathsoft.com/math50");
        WriteRealXml(complex.RValue);
        WriteImaginaryXml(complex.IValue);
        Writer.WriteEndElement();
    }

    public void Visit(IStringResultNode str)
    {
        Writer.WriteStartElement("ml", "str", "http://schemas.mathsoft.com/math50");
        Writer.WriteString(str.Value);
        Writer.WriteEndElement();
    }

    public void Visit(IFunctionResultNode function)
    {
        Writer.WriteStartElement("ml", "functionValue", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("description");
        Writer.WriteString(function.Description);
        Writer.WriteEndAttribute();
        Writer.WriteEndElement();
    }

    public void Visit(IUnitedValueResultNode unitedValue)
    {
        if (unitedValue.Value is ILazyMatrixResultNode)
        {
            unitedValue.Value.Accept(this);
            return;
        }
        Writer.WriteStartElement("ml", "unitedValue", "http://schemas.mathsoft.com/math50");
        unitedValue.Value.Accept(this);
        Visit(unitedValue.Units);
        Writer.WriteEndElement();
    }

    private void Visit(IList<Unit> units)
    {
        Writer.WriteStartElement("u", "unitMonomial", "http://schemas.mathsoft.com/units10");
        foreach (Unit unit in units)
        {
            Visit(unit);
        }
        Writer.WriteEndElement();
    }

    public void Visit(IDisplayMatrixResultNode displayMatrix)
    {
        WriteDisplayMatrixXml(displayMatrix);
    }

    public void Visit(IEagerMatrixResultNode eagerMatrix)
    {
        WriteMatrixXml(eagerMatrix);
    }

    public void Visit(ILazyMatrixResultNode lazyMatrix)
    {
        lazyMatrix.MatrixProxy.WriteXml(Writer);
    }

    public void Visit(ISymbolicResultNode symbolic)
    {
        Writer.WriteStartElement("ml", "symResult", "http://schemas.mathsoft.com/math50");
        NameTable nameTable = new NameTable();
        XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(nameTable);
        xmlNamespaceManager.AddNamespace("ml", "http://schemas.mathsoft.com/math50");
        xmlNamespaceManager.AddNamespace("u", "http://schemas.mathsoft.com/units10");
        XmlParserContext inputContext = new XmlParserContext(nameTable, xmlNamespaceManager, null, XmlSpace.Default);
        XmlReaderSettings settings = new XmlReaderSettings
        {
            NameTable = nameTable,
            ConformanceLevel = ConformanceLevel.Fragment
        };
        using (StringReader input = new StringReader(symbolic.Xml))
        {
            using XmlReader reader = XmlReader.Create(input, settings, inputContext);
            Writer.WriteNode(reader, defattr: false);
        }
        Writer.WriteEndElement();
    }

    public void Visit(ITrace2DResultNode traceResult)
    {
        Writer.WriteStartElement("ml", "Trace2dResult", "http://schemas.mathsoft.com/math50");
        Trace2DResultType? trace2DResultType = ConvertAndGetNullableValue<Trace2DResultType>(traceResult.TraceResultType);
        Writer.WriteAttributeString("TraceType", trace2DResultType.ToString());
        WriteAxisInfoNode("RangeInfo", traceResult.RangeAxisInfo);
        WriteAxisInfoNode("ResultInfo", traceResult.ResultAxisInfo);
        WritePlotDataPoints(traceResult.DataPoints);
        Writer.WriteEndElement();
    }

    private void WriteAxisInfoNode(string xmlNodeName, ITraceAxisInfo axisInfo)
    {
        Writer.WriteStartElement("ml", xmlNodeName, "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("Min");
        Writer.WriteValue(axisInfo.Min);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("Max");
        Writer.WriteValue(axisInfo.Max);
        Writer.WriteEndAttribute();
        if (!axisInfo.Units.IsNullOrEmpty())
        {
            Visit(axisInfo.Units);
        }
        Writer.WriteEndElement();
    }

    private void WritePlotDataPoints(ITraceDataPoints dataDataPoints)
    {
        Writer.WriteStartElement("ml", "Data", "http://schemas.mathsoft.com/math50");
        if (!dataDataPoints.IsEmpty)
        {
            Writer.WriteStartElement("ml", "RangePoints", "http://schemas.mathsoft.com/math50");
            dataDataPoints.RangePoints.Accept(this);
            Writer.WriteEndElement();
            Writer.WriteStartElement("ml", "ResultPoints", "http://schemas.mathsoft.com/math50");
            dataDataPoints.ResultPoints.Accept(this);
            Writer.WriteEndElement();
        }
        Writer.WriteEndElement();
    }

    public void Visit(IVectorListResultNode vectorList)
    {
        Writer.WriteStartElement("ml", "DataVectors", "http://schemas.mathsoft.com/math50");
        VectorsTypeEnum? vectorsTypeEnum = ConvertAndGetNullableValue<VectorsTypeEnum>(vectorList.VectorsType);
        Writer.WriteAttributeString("VectorsType", vectorsTypeEnum.ToString());
        Writer.WriteStartAttribute("Count");
        Writer.WriteValue(vectorList.Count);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("VectorLength");
        Writer.WriteValue(vectorList.SingleVectorLength);
        Writer.WriteEndAttribute();
        Action<IVectorListResultNode> action = null;
        ValuesTypeEnum? valuesTypeEnum = null;
        switch (vectorList.CommonValuesType)
        {
        case ResultValueType.String:
            action = WriteVectorsAsJson<string>;
            valuesTypeEnum = ValuesTypeEnum.String;
            break;
        case ResultValueType.Numeric:
        case ResultValueType.NaNs:
            action = WriteVectorsAsJson<double>;
            valuesTypeEnum = ValuesTypeEnum.Numeric;
            break;
        default:
            DebugTools.FailAssertion(vectorList.CommonValuesType.ToString());
            break;
        }
        Writer.WriteAttributeString("ValuesType", valuesTypeEnum.ToString());
        action?.Invoke(vectorList);
        Writer.WriteEndElement();
    }

    private static TTarget? ConvertAndGetNullableValue<TTarget>(Enum sourceEnumValue) where TTarget : struct
    {
        if (!EnumHelper.TryParse<TTarget>(sourceEnumValue, out var enumValue, EnumHelper.SettingsWithCacheAndAssert))
        {
            return null;
        }
        return enumValue;
    }

    private void WriteVectorsAsJson<T>(IVectorListResultNode vectorsList)
    {
        XmlWriter writer = Writer;
        using ChunkedMemoryStream chunkedMemoryStream = new ChunkedMemoryStream();
        StreamWriter streamWriter = new StreamWriter(chunkedMemoryStream);
        VectorsListJSonSerializer<T>.Write(chunkedMemoryStream, vectorsList.Vectors, vectorsList.SingleVectorLength);
        streamWriter.Flush();
        StreamReader streamReader = new StreamReader(chunkedMemoryStream);
        chunkedMemoryStream.Position = 0L;
        if (chunkedMemoryStream.Length < 2097152)
        {
            writer.WriteValue(streamReader.ReadToEnd());
        }
        else
        {
            char[] buffer = new char[2097152];
            while (chunkedMemoryStream.Position < chunkedMemoryStream.Length)
            {
                int count = streamReader.ReadBlock(buffer, 0, 2097152);
                writer.WriteChars(buffer, 0, count);
            }
        }
        streamWriter.Close();
        streamReader.Close();
    }

    private void Visit(Unit unit)
    {
        Writer.WriteStartElement("u", "unitReference", "http://schemas.mathsoft.com/units10");
        Writer.WriteAttributeString("unit", unit.CanonicalName);
        Writer.WriteStartAttribute("power-numerator");
        Writer.WriteValue(unit.PowerNumerator);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("power-denominator");
        Writer.WriteValue(unit.PowerDenominator);
        Writer.WriteEndAttribute();
        Writer.WriteEndElement();
    }
}
