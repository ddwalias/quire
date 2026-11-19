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
        plotType plotType2 = new plotType
        {
            originpositioning = plotControl.HasOriginPositioning,
            backgroundtype = ConvertBackgroundType(plotControl.PlotBackgroundType),
            showaxisexpressions = plotControl.ShowAxisExpressions,
            disablecalc = plotControl.HideFromCalculationContext
        };
        plotBaseType item = ((plotControl.PlotType == PlotType.XY) ? CreateXyPlotType((XYPlotControl)plotControl, model) : CreatePolarPlotType(plotControl, model));
        FillPlotBaseType(item, plotControl);
        plotType2.Item = item;
        return plotType2;
    }

    private static plotBaseType CreatePolarPlotType(IPlotView plotControl, IPlotModel model)
    {
        polarPlotType polarPlotType2 = new polarPlotType();
        polarPlotTypeAxes polarPlotTypeAxes2 = new polarPlotTypeAxes();
        polarPlotTypeAxesAngularAxis polarPlotTypeAxesAngularAxis2 = new polarPlotTypeAxesAngularAxis();
        FillAxisBaseType(polarPlotTypeAxesAngularAxis2, plotControl.RangeAxis, null, model.rangeAxis);
        polarPlotTypeAxesAngularAxis2.angularDomain = CreateXyDomainType(plotControl.RangeAxis);
        polarPlotTypeAxesAngularAxis2.legendposition = GetXLegendPosition(plotControl);
        polarPlotTypeAxes2.angularAxis = polarPlotTypeAxesAngularAxis2;
        polarPlotTypeAxesRadialAxis polarPlotTypeAxesRadialAxis2 = new polarPlotTypeAxesRadialAxis();
        FillAxisBaseType(polarPlotTypeAxesRadialAxis2, plotControl.PrimaryResultAxis, null, model.PrimaryResultAxis);
        polarPlotTypeAxesRadialAxis2.radialDomain = CreateXyDomainType(plotControl.PrimaryResultAxis);
        polarPlotTypeAxesRadialAxis2.legendposition = GetYLegendPosition(plotControl);
        polarPlotTypeAxes2.radialAxis = polarPlotTypeAxesRadialAxis2;
        polarPlotType2.axes = polarPlotTypeAxes2;
        return polarPlotType2;
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
        xyPlotType xyPlotType2 = new xyPlotType();
        xyPlotTypeAxes xyPlotTypeAxes2 = new xyPlotTypeAxes();
        xAxis xAxis2 = new xAxis();
        FillAxisBaseType(xAxis2, plotControl.RangeAxis, plotControl.VerticalMarkersForSerialization, model.rangeAxis);
        xAxis2.xyDomain = CreateXyDomainType(plotControl.RangeAxis);
        xAxis2.legendposition = GetXLegendPosition(plotControl);
        xyPlotTypeAxes2.xAxis = xAxis2;
        yAxis[] array = new yAxis[1]
        {
            new yAxis()
        };
        FillAxisBaseType(array[0], plotControl.PrimaryResultAxis, plotControl.HorizontalMarkersForSerialization, model.PrimaryResultAxis);
        array[0].xyDomain = CreateXyDomainType(plotControl.PrimaryResultAxis);
        array[0].legendposition = GetYLegendPosition(plotControl);
        xyPlotTypeAxes2.yAxis = array;
        xyPlotType2.axes = xyPlotTypeAxes2;
        return xyPlotType2;
    }

    private static xyDomainType CreateXyDomainType(IPlotAxisView axis)
    {
        xyDomainType xyDomainType2 = new xyDomainType();
        if (axis.StartLimitEditor.IsDefaultValue)
        {
            xyDomainType2.startValue = CreateValueType(XmlWorksheetData.CreateMLElement(new XmlDocument(), "placeholder", "http://schemas.mathsoft.com/math50"));
        }
        else
        {
            xyDomainType2.startValue = CreateValueType(axis.StartExpressionEqn.GetRegionXmlNode().FirstChild);
        }
        if (!axis.SecondTickEditor.IsDefaultValue)
        {
            xyDomainType2.secondTickValue = CreateValueType(axis.SecondTickExpressionEqn.GetRegionXmlNode().FirstChild);
        }
        if (axis.EndLimitEditor.IsDefaultValue)
        {
            xyDomainType2.endValue = CreateValueType(XmlWorksheetData.CreateMLElement(new XmlDocument(), "placeholder", "http://schemas.mathsoft.com/math50"));
        }
        else
        {
            xyDomainType2.endValue = CreateValueType(axis.EndExpressionEqn.GetRegionXmlNode().FirstChild);
        }
        xyDomainType2.scaletype = ConvertScaling(axis.Scaling);
        xyDomainType2.autoscale = true;
        return xyDomainType2;
    }

    private static valueType CreateValueType(XmlNode regionXmlNode)
    {
        MathTransformer mathTransformer = new MathTransformer(regionXmlNode, XmlWorksheetData.McdxNameSpaceManager, null, null);
        valueType valueType2 = new valueType();
        valueType2.InnerXml = new XmlNode[1] { mathTransformer.Math50Xml };
        valueType2.EquationSExpression = new string[1] { mathTransformer.SExpression };
        return valueType2;
    }

    private static void FillAxisBaseType(axisBaseType axisBaseType, IPlotAxisView axis, IEnumerable<AxisMarker> markers, IAxisModel axisModel)
    {
        axisGridType axisGridType2 = new axisGridType();
        axisGridType2.gridFrequency = new axisGridTypeGridFrequency
        {
            Value = axis.TickMarks.ToString(CultureInfo.InvariantCulture)
        };
        axisGridType2.gridLabels = new axisGridTypeGridLabels
        {
            display = axis.ShowNumbers
        };
        axisGridType2.gridLines = new axisGridTypeGridLines();
        axisGridType2.tickMarks = new axisGridTypeTickMarks
        {
            display = axis.ShowTicks
        };
        axisBaseType.axisGrid = axisGridType2;
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

    private static trace2DType CreateTrace(IPlotTrace plotTrace)
    {
        trace2DType trace2DType2 = new trace2DType();
        if (plotTrace.IsNumOfPointsMutable)
        {
            trace2DType2.numofpoints = plotTrace.NumberOfPoints.ToString(CultureInfo.InvariantCulture);
        }
        trace2DType2.resultRefSpecified = false;
        trace2DType2.traceLabel = plotTrace.Label;
        trace2DType2.traceStyle = CreateTraceStyle(plotTrace);
        return trace2DType2;
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
