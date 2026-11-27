#nullable disable
#pragma warning disable

using System;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots;
using Quire.PersistentDataObjects;
using Quire.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;

namespace Quire.PlotDataObjects;

internal static partial class PlotDataObjectCreator
{
    private static BackgroundType ConvertBackgroundType(BackgroundTypeE backgroundTypeE)
    {
        switch (backgroundTypeE)
        {
            case BackgroundTypeE.Undefined:
                return BackgroundType.undefined;
            case BackgroundTypeE.Paper:
                return BackgroundType.paper;
            case BackgroundTypeE.White:
                return BackgroundType.white;
            case BackgroundTypeE.Transparent:
                return BackgroundType.transparent;
            default:
                throw new ArgumentOutOfRangeException("backgroundTypeE");
        }
    }

    private static scaleType ConvertScaling(AxisScaling axisScaling)
    {
        switch (axisScaling)
        {
            case AxisScaling.Linear:
                return scaleType.linear;
            case AxisScaling.Logarithmic:
                return scaleType.logarithmic;
            default:
                throw new ArgumentOutOfRangeException("axisScaling");
        }
    }

    private static axisBaseTypeAxisLinePosition ConvertAxisPosition(Positions position)
    {
        switch (position)
        {
            case Positions.Origin:
                return axisBaseTypeAxisLinePosition.origin;
            case Positions.Top:
                return axisBaseTypeAxisLinePosition.top;
            case Positions.Bottom:
                return axisBaseTypeAxisLinePosition.bottom;
            case Positions.Left:
                return axisBaseTypeAxisLinePosition.left;
            case Positions.Right:
                return axisBaseTypeAxisLinePosition.right;
            case Positions.TickNumberLock:
                return axisBaseTypeAxisLinePosition.ticknumberlock;
            default:
                throw new ArgumentOutOfRangeException("position");
        }
    }

    private static traceStyleEnumCommonType ConvertTraceType(Quire.Calculation.Core.Trace2DResults.TraceType traceType)
    {
        switch (traceType)
        {
            case Quire.Calculation.Core.Trace2DResults.TraceType.Lines:
                return traceStyleEnumCommonType.lines;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Columns:
                return traceStyleEnumCommonType.columns;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Effects:
                return traceStyleEnumCommonType.effects;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Boxes:
                return traceStyleEnumCommonType.boxes;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Bars:
                return traceStyleEnumCommonType.bars;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Errors:
                return traceStyleEnumCommonType.errors;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Stems:
                return traceStyleEnumCommonType.stems;
            case Quire.Calculation.Core.Trace2DResults.TraceType.Waterfall:
                return traceStyleEnumCommonType.waterfall;
            default:
                throw new ArgumentOutOfRangeException("traceType");
        }
    }

    private static traceSymbolType ConvertSymbol(TraceSymbol traceSymbol)
    {
        switch (traceSymbol)
        {
            case TraceSymbol.rectangle:
                return traceSymbolType.rectangle;
            case TraceSymbol.rhombus:
                return traceSymbolType.rhombus;
            case TraceSymbol.star:
                return traceSymbolType.star;
            case TraceSymbol.circle:
                return traceSymbolType.circle;
            case TraceSymbol.x:
                return traceSymbolType.x;
            case TraceSymbol.none:
                return traceSymbolType.none;
            case TraceSymbol.point:
                return traceSymbolType.point;
            case TraceSymbol.plus:
                return traceSymbolType.plus;
            case TraceSymbol.filledRectangle:
                return traceSymbolType.filledRectangle;
            case TraceSymbol.filledRhombus:
                return traceSymbolType.filledRhombus;
            case TraceSymbol.filledTriangle:
                return traceSymbolType.filledTriangle;
            case TraceSymbol.triangle:
                return traceSymbolType.triangle;
            case TraceSymbol.filledStar:
                return traceSymbolType.filledStar;
            default:
                throw new ArgumentOutOfRangeException("traceSymbol");
        }
    }

    public static traceLineStyle ConvertLineStyle(LineStyle lineStyle)
    {
        switch (lineStyle)
        {
            case LineStyle.None:
                return traceLineStyle.None;
            case LineStyle.Solid:
                return traceLineStyle.Solid;
            case LineStyle.Dot:
                return traceLineStyle.Dot;
            case LineStyle.Dash:
                return traceLineStyle.Dash;
            case LineStyle.DashDot:
                return traceLineStyle.DashDot;
            case LineStyle.DashDotDot:
                return traceLineStyle.DashDotDot;
            default:
                throw new ArgumentOutOfRangeException("lineStyle");
        }
    }

    public static LineStyle ConvertLineStyle(traceLineStyle lineStyle)
    {
        switch (lineStyle)
        {
            case traceLineStyle.None:
                return LineStyle.None;
            case traceLineStyle.Solid:
                return LineStyle.Solid;
            case traceLineStyle.Dot:
                return LineStyle.Dot;
            case traceLineStyle.Dash:
                return LineStyle.Dash;
            case traceLineStyle.DashDot:
                return LineStyle.DashDot;
            case traceLineStyle.DashDotDot:
                return LineStyle.DashDotDot;
            default:
                throw new ArgumentOutOfRangeException("lineStyle");
        }
    }

    internal static plotType CreatePlotDataObject(PlotControl plotControl, IPlotModel model)
    {
        plotType plotData = new plotType
        {
            originpositioning = plotControl.HasOriginPositioning,
            backgroundtype = ConvertBackgroundType(plotControl.PlotBackgroundType),
            showaxisexpressions = plotControl.ShowAxisExpressions,
            disablecalc = plotControl.HideFromCalculationContext
        };
        plotBaseType item = ((plotControl.PlotType == PlotType.XY) ? CreateXyPlotType((XYPlotControl)plotControl, model) : CreatePolarPlotType(plotControl, model));
        FillPlotBaseType(item, plotControl);
        plotData.Item = item;
        return plotData;
    }

    private static plotBaseType CreatePolarPlotType(IPlotView plotControl, IPlotModel model)
    {
        polarPlotType polarPlotData = new polarPlotType();
        polarPlotTypeAxes polarAxesData = new polarPlotTypeAxes();
        polarPlotTypeAxesAngularAxis angularAxisData = new polarPlotTypeAxesAngularAxis();
        FillAxisBaseType(angularAxisData, plotControl.RangeAxis, null, model.rangeAxis);
        angularAxisData.angularDomain = CreateXyDomainType(plotControl.RangeAxis);
        angularAxisData.legendposition = GetXLegendPosition(plotControl);
        polarAxesData.angularAxis = angularAxisData;
        polarPlotTypeAxesRadialAxis radialAxisData = new polarPlotTypeAxesRadialAxis();
        FillAxisBaseType(radialAxisData, plotControl.PrimaryResultAxis, null, model.PrimaryResultAxis);
        radialAxisData.radialDomain = CreateXyDomainType(plotControl.PrimaryResultAxis);
        radialAxisData.legendposition = GetYLegendPosition(plotControl);
        polarAxesData.radialAxis = radialAxisData;
        polarPlotData.axes = polarAxesData;
        return polarPlotData;
    }

    private static string GetXLegendPosition(IPlotView plotControl)
    {
        return Enum.GetName(typeof(LegendPositions.XLegendPosition), plotControl.XLegendPositionIndex);
    }

    private static string GetYLegendPosition(IPlotView plotControl)
    {
        return Enum.GetName(typeof(LegendPositions.YLegendPosition), plotControl.YLegendPositionIndex);
    }

    private static string GetPlotTitlePosition(IPlotView plotControl)
    {
        return Enum.GetName(typeof(LegendPositions.PlotTitlePosition), plotControl.PlotTitlePositionIndex);
    }

    private static plotBaseType CreateXyPlotType(XYPlotControl plotControl, IPlotModel model)
    {
        xyPlotType xyPlotData = new xyPlotType();
        xyPlotTypeAxes xyAxesData = new xyPlotTypeAxes();
        xAxis xAxisData = new xAxis();
        FillAxisBaseType(xAxisData, plotControl.RangeAxis, plotControl.VerticalMarkersForSerialization, model.rangeAxis);
        xAxisData.xyDomain = CreateXyDomainType(plotControl.RangeAxis);
        xAxisData.legendposition = GetXLegendPosition(plotControl);
        xyAxesData.xAxis = xAxisData;
        yAxis[] array = new yAxis[1]
        {
            new yAxis()
        };
        FillAxisBaseType(array[0], plotControl.PrimaryResultAxis, plotControl.HorizontalMarkersForSerialization, model.PrimaryResultAxis);
        array[0].xyDomain = CreateXyDomainType(plotControl.PrimaryResultAxis);
        array[0].legendposition = GetYLegendPosition(plotControl);
        xyAxesData.yAxis = array;
        xyPlotData.axes = xyAxesData;
        return xyPlotData;
    }

    private static xyDomainType CreateXyDomainType(IPlotAxisView axis)
    {
        xyDomainType domainData = new xyDomainType();
        if (axis.StartLimitEditor.IsDefaultValue)
        {
            domainData.startValue = CreateValueType(XmlWorksheetData.CreateMLElement(new XmlDocument(), "placeholder", "http://schemas.mathsoft.com/math50"));
        }
        else
        {
            domainData.startValue = CreateValueType(axis.StartExpressionEqn.GetRegionXmlNode().FirstChild);
        }
        if (!axis.SecondTickEditor.IsDefaultValue)
        {
            domainData.secondTickValue = CreateValueType(axis.SecondTickExpressionEqn.GetRegionXmlNode().FirstChild);
        }
        if (axis.EndLimitEditor.IsDefaultValue)
        {
            domainData.endValue = CreateValueType(XmlWorksheetData.CreateMLElement(new XmlDocument(), "placeholder", "http://schemas.mathsoft.com/math50"));
        }
        else
        {
            domainData.endValue = CreateValueType(axis.EndExpressionEqn.GetRegionXmlNode().FirstChild);
        }
        domainData.scaletype = ConvertScaling(axis.Scaling);
        domainData.autoscale = true;
        return domainData;
    }

    private static valueType CreateValueType(XmlNode regionXmlNode)
    {
        MathTransformer mathTransformer = new MathTransformer(regionXmlNode, XmlWorksheetData.McdxNameSpaceManager, null, null);
        valueType valueData = new valueType();
        valueData.InnerXml = new XmlNode[1] { mathTransformer.Math50Xml };
        valueData.EquationSExpression = new string[1] { mathTransformer.SExpression };
        return valueData;
    }

    private static void FillAxisBaseType(axisBaseType axisBaseType, IPlotAxisView axis, IEnumerable<AxisMarker> markers, IAxisModel axisModel)
    {
        axisGridType axisGridData = new axisGridType();
        axisGridData.gridFrequency = new axisGridTypeGridFrequency
        {
            Value = axis.TickMarks.ToString(CultureInfo.InvariantCulture)
        };
        axisGridData.gridLabels = new axisGridTypeGridLabels
        {
            display = axis.ShowNumbers
        };
        axisGridData.gridLines = new axisGridTypeGridLines();
        axisGridData.tickMarks = new axisGridTypeTickMarks
        {
            display = axis.ShowTicks
        };
        axisBaseType.axisGrid = axisGridData;
        axisBaseType.axisLabel = new plotTitleType();
        axisBaseType.axisLine = new axisBaseTypeAxisLine
        {
            position = ConvertAxisPosition(axis.Position),
            positionticmark = axis.PositionTickMark.ToString(CultureInfo.InvariantCulture),
            legendWidth = axis.LegendActualWidth,
            legendWidthSpecified = !double.IsNaN(axis.LegendActualWidth)
        };
        if (markers != null)
        {
            List<axisBaseTypeMarker> list = new List<axisBaseTypeMarker>();
            foreach (AxisMarker marker in markers)
            {
                list.Add(marker.GetDataObject(null));
            }
            axisBaseType.markers = list.ToArray();
        }
        else
        {
            axisBaseType.markers = new axisBaseTypeMarker[0];
        }
        if (axis.IsDisplayNumberFormatLocal)
        {
            axisBaseType.numberFormat = CreateNumberFormat(axis.DisplayNumberFormat);
        }
        axisBaseTypePlotEquations plotEquations = new axisBaseTypePlotEquations();
        FillEquations(plotEquations, axis);
        axisBaseType.plotEquations = plotEquations;
        axisBaseType.rank = "1";
        axisBaseType.endSpecified = true;
        axisBaseType.end = axisModel.End;
        axisBaseType.startSpecified = true;
        axisBaseType.start = axisModel.Start;
    }

    private static void FillEquations(axisBaseTypePlotEquations axisBaseTypePlotEquations, IPlotAxisView axis)
    {
        axisBaseTypePlotEquations.plotEquation = new plotEquation[axis.EquationCount];
        for (int i = 0; i < axis.EquationCount; i++)
        {
            axisBaseTypePlotEquations.plotEquation[i] = axis[i].GetDataObject();
        }
    }

    private static resultFormatType CreateNumberFormat(NumberFormat displayNumberFormat)
    {
        return new resultFormatType();
    }

    private static void FillPlotBaseType(plotBaseType plotBaseType, PlotControl plotControl)
    {
        plotBaseType.legend = new plotBaseTypeLegend();
        plotBaseType.graphsize = new SizeType
        {
            height = plotControl.Model.GraphSize.Height,
            width = plotControl.Model.GraphSize.Width
        };
        plotBaseType.traces = new trace2DType[plotControl.Traces.Length];
        for (int i = 0; i < plotControl.Traces.Length; i++)
        {
            IPlotTrace plotTrace = plotControl.Traces[i];
            plotBaseType.traces[i] = CreateTrace(plotTrace);
        }
    }

    private static trace2DType CreateTrace(IPlotTrace plotTrace)
    {
        trace2DType traceData = new trace2DType();
        if (plotTrace.IsNumOfPointsMutable)
        {
            traceData.numofpoints = plotTrace.NumberOfPoints.ToString(CultureInfo.InvariantCulture);
        }
        traceData.resultRefSpecified = false;
        traceData.traceLabel = plotTrace.Label;
        traceData.traceStyle = CreateTraceStyle(plotTrace);
        return traceData;
    }

    private static traceStyleType CreateTraceStyle(IPlotTrace plotTrace)
    {
        return new traceStyleType
        {
            color = plotTrace.Color.ToString(CultureInfo.InvariantCulture),
            linestyle = ConvertLineStyle(plotTrace.LineStyle),
            lineweight = plotTrace.LineWeight,
            symbol = ConvertSymbol(plotTrace.Symbol),
            Value = ConvertTraceType(plotTrace.Type)
        };
    }

}
