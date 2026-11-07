using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Quire.Calculation.Core.Trace2DResults;
using Quire.PersistentDataObjects;
using Quire.Xml;

namespace Quire.Calculation.Core;

public partial class XmcdToIResultNodeTransform
{
    private ITrace2DResultNode BuildPlotTraceResult(XmlNode node)
    {
        IEnumerable<XmlNode> children = node.ChildNodes.Cast<XmlNode>();
        string traceType = node.Attributes["TraceType"].Value;
        if (!TryParseUsingIntermediate<TraceResultTypeEnum, Trace2DResultType>(traceType, out var resultType))
        {
            throw new InvalidOperationException(string.Format(ErrorAttributeConversionToEnum, "TraceType", traceType, node.Name));
        }

        ITraceAxisInfo rangeAxisInfo = null;
        ITraceAxisInfo resultAxisInfo = null;
        ITraceDataPoints dataPoints = null;
        foreach (XmlNode child in children)
        {
            string childName = child.LocalName;
            switch (childName)
            {
                case "RangeInfo":
                    rangeAxisInfo = BuildAxisInfo(child);
                    break;
                case "ResultInfo":
                    resultAxisInfo = BuildAxisInfo(child);
                    break;
                case "Data":
                    IVectorListResultNode rangePoints = BuildDataVectors(child, "RangePoints");
                    IVectorListResultNode resultPoints = BuildDataVectors(child, "ResultPoints");
                if (rangePoints == null && resultPoints == null)
                {
                    dataPoints = new TraceDataPoints();
                }
                else
                {
                    dataPoints = new TraceDataPoints(rangePoints, resultPoints);
                }
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Format error: {0} node has unsupported child node{1}", "Trace2dResult", child.LocalName));
            }
        }

        if (rangeAxisInfo != null && resultAxisInfo != null && dataPoints != null)
        {
            return new Quire.Calculation.Core.Trace2DResults.Trace2DResultNode(rangeAxisInfo, resultAxisInfo, dataPoints, resultType);
        }
        throw new InvalidOperationException(string.Format("Failed to build result from node {0}", "Trace2dResult"));
    }

    private static ITraceAxisInfo BuildAxisInfo(XmlNode node)
    {
        double max = double.Parse(node.Attributes["Max"].Value, NumberFormatInfo.InvariantInfo);
        double min = double.Parse(node.Attributes["Min"].Value, NumberFormatInfo.InvariantInfo);
        var axis = new UnitedValueSimpleResultNode();
        if (node.ChildNodes.Count > 0)
        {
            string xpath = string.Format("{0}:{1}", "u", "unitMonomial");
            XmlNode unitMonomial = node.SelectSingleNode(xpath, XmlWorksheetData.McdxNameSpaceManager);
            if (unitMonomial != null)
            {
                ReadUnitMonomial(axis, unitMonomial);
            }
        }
        return new TraceAxisInfo(min, max, axis.Units);
    }

    private IResultNode BuildMatrix(XmlNode node)
    {
        int rows = int.Parse(node.Attributes["rows"].Value, NumberFormatInfo.InvariantInfo);
        int columns = int.Parse(node.Attributes["cols"].Value, NumberFormatInfo.InvariantInfo);
        int elementCount = node.ChildNodes.Count;
        IEnumerable<XmlNode> elements = node.ChildNodes.Cast<XmlNode>();
        if (node.FirstChild != null && node.FirstChild.LocalName == "display")
        {
            XmlElement display = (XmlElement)node.FirstChild;
            TableSize? viewportSize = (TableSize?)new TableSizeConverter().ConvertFromString(display.GetAttribute("size"));
            if (viewportSize.HasValue && (viewportSize.Value.Rows != rows || viewportSize.Value.Columns != columns))
            {
                return BuildDisplayMatrix(node, rows, columns, viewportSize.Value);
            }
            elementCount--;
            elements = elements.Skip(1);
        }

        IEagerMatrixResultNode matrix = _resultNodeFactory.CreateEagerMatrixResultNode(rows, columns);
        if (matrix.Count != elementCount)
        {
            throw new InvalidOperationException(string.Format("Logic error: xmcd format result {0}x{1} matrix is expected to have {2} child(ren) but it has {3}", rows, columns, matrix.Count, elementCount));
        }
        AddElementsToMatrix(elements, matrix);
        return matrix;
    }

    private IResultNode BuildDisplayMatrix(XmlNode node, int rows, int cols, TableSize viewportSize)
    {
        XmlElement display = (XmlElement)node.FirstChild;
        TableCoordinate? viewportOffset = (TableCoordinate?)new TableCoordinateConverter().ConvertFromString(display.GetAttribute("offset"));
        if (!viewportOffset.HasValue)
        {
            viewportOffset = new TableCoordinate(0, 0);
        }
        IDisplayMatrixResultNode matrix = _resultNodeFactory.CreateDisplayMatrixResultNode(rows, cols, viewportOffset.Value, viewportSize);
        if (matrix.Viewport.Count != node.ChildNodes.Count - 1)
        {
            throw new InvalidOperationException(string.Format("Logic error: xmcd format result {0}x{1} display matrix is expected to have {2} child(ren) but it has {3}", rows, cols, matrix.Viewport.Count, node.ChildNodes.Count - 1));
        }
        IEnumerable<XmlNode> elements = node.ChildNodes.Cast<XmlNode>().Skip(1);
        AddElementsToMatrix(elements, matrix.Viewport);
        return matrix;
    }

    private IUnitedValueResultNode BuildUnitedValue(XmlNode node)
    {
        IUnitedValueResultNode unitedValue = _resultNodeFactory.CreateUnitedValueResultNode();
        unitedValue.Value = MakeResultNode(node.ChildNodes[0]);
        XmlNode unitMonomial = node.ChildNodes[1];
        ReadUnitMonomial(unitedValue, unitMonomial);
        return unitedValue;
    }
}
