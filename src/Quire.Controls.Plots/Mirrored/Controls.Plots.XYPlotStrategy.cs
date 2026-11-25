#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots;

internal partial class XYPlotStrategy
{
    public void InitializeAxes(PlotTitle xAxisTitle, PlotTitle yAxisTitle, PlotTitle title, EquationList rangeEquations, EquationList primaryResultEquations, out PlotGraphAxis rangeAxis, out PlotGraphAxis primaryResultAxis)
    {
        rangeAxis = new PlotGraphLinearAxis(rangeEquations, xAxisTitle);
        primaryResultAxis = new PlotGraphLinearAxis(primaryResultEquations, yAxisTitle);
        rangeAxis.OtherAxis = primaryResultAxis;
        primaryResultAxis.OtherAxis = rangeAxis;
        InitializeAxisTitle(rangeAxis, primaryResultAxis);
        rangeAxis.Orientation = Orientation.Horizontal;
        rangeAxis.Position = Positions.Origin;
        primaryResultAxis.Orientation = Orientation.Vertical;
        primaryResultAxis.Position = Positions.Origin;
        rangeEquations.Header = rangeEquations.Resources["XLegend.Header"] as string;
        primaryResultEquations.Header = primaryResultEquations.Resources["YLegend.Header"] as string;
        if (title != null)
        {
            title.Header = title.Resources["PlotTitle.Header"] as string;
        }
    }

    private static void InitializeAxisTitle(PlotGraphAxis rangeAxis, PlotGraphAxis primaryResultAxis)
    {
        if (rangeAxis.AxisTitle == null || primaryResultAxis.AxisTitle == null)
        {
            return;
        }

        rangeAxis.AxisTitle.Header = rangeAxis.AxisTitle.Resources["XAxisTitle.Header"] as string;
        primaryResultAxis.AxisTitle.Header = primaryResultAxis.AxisTitle.Resources["YAxisTitle.Header"] as string;
    }

    public void InitializeLegendPositions(PlotControl plot, out PlotItemPlaceholder[] rangeLegendsPositions, out PlotItemPlaceholder[] resultLegendsPositions, out PlotItemPlaceholder[] titlePositions, out PlotItemPlaceholder[] plotLegendPositions)
    {
        rangeLegendsPositions = new PlotItemPlaceholder[6];
        resultLegendsPositions = new PlotItemPlaceholder[6];
        titlePositions = new PlotItemPlaceholder[2];
        plotLegendPositions = new PlotItemPlaceholder[5];
        rangeLegendsPositions[0] = plot.Template.FindName("PART_X0", plot) as PlotItemPlaceholder;
        rangeLegendsPositions[1] = plot.Template.FindName("PART_X1", plot) as PlotItemPlaceholder;
        resultLegendsPositions[0] = plot.Template.FindName("PART_Y0", plot) as PlotItemPlaceholder;
        resultLegendsPositions[1] = plot.Template.FindName("PART_SharedY1", plot) as PlotItemPlaceholder;
        rangeLegendsPositions[2] = plot.Template.FindName("PART_X2", plot) as PlotItemPlaceholder;
        rangeLegendsPositions[3] = plot.Template.FindName("PART_X3", plot) as PlotItemPlaceholder;
        rangeLegendsPositions[4] = plot.Template.FindName("PART_X4", plot) as PlotItemPlaceholder;
        rangeLegendsPositions[5] = plot.Template.FindName("PART_X5", plot) as PlotItemPlaceholder;
        resultLegendsPositions[2] = plot.Template.FindName("PART_Y2", plot) as PlotItemPlaceholder;
        resultLegendsPositions[3] = plot.Template.FindName("PART_Y3", plot) as PlotItemPlaceholder;
        resultLegendsPositions[4] = plot.Template.FindName("PART_Y4", plot) as PlotItemPlaceholder;
        resultLegendsPositions[5] = plot.Template.FindName("PART_Y5", plot) as PlotItemPlaceholder;
        titlePositions[0] = plot.Template.FindName("PART_TitleBottom", plot) as PlotItemPlaceholder;
        titlePositions[1] = plot.Template.FindName("PART_TitleTop", plot) as PlotItemPlaceholder;
        plotLegendPositions[0] = plot.Template.FindName("PART_PlotLegendBelow", plot) as PlotItemPlaceholder;
        plotLegendPositions[1] = plot.Template.FindName("PART_PlotLegendTopLeft", plot) as PlotItemPlaceholder;
        plotLegendPositions[2] = plot.Template.FindName("PART_PlotLegendTopRight", plot) as PlotItemPlaceholder;
        plotLegendPositions[3] = plot.Template.FindName("PART_PlotLegendBottomLeft", plot) as PlotItemPlaceholder;
        plotLegendPositions[4] = plot.Template.FindName("PART_PlotLegendBottomRight", plot) as PlotItemPlaceholder;
    }

    public void ApplyPlotTemplate(PlotControl plot)
    {
        if (plot.Template.FindName("PART_xSplitter", plot) is PlotGridSplitter xSplitter)
        {
            xSplitter.Axis = plot.RangeAxis;
            xSplitter.MouseDownAuxiliary += plot.HandlePlotGridSplitterMouseDown;
        }
        if (plot.Template.FindName("PART_ySplitter", plot) is PlotGridSplitter ySplitter)
        {
            ySplitter.Axis = plot.PrimaryResultAxis;
            ySplitter.MouseDownAuxiliary += plot.HandlePlotGridSplitterMouseDown;
        }
        plot.XAxisTitlePlaceholder = plot.Template.FindName("PART_XAxisTitle", plot) as PlotItemPlaceholder;
        plot.YAxisTitlePlaceholder = plot.Template.FindName("PART_YAxisTitle", plot) as PlotItemPlaceholder;
        if (DebugTools.AssertNotNull(plot.XAxisTitlePlaceholder, null).Passed && DebugTools.AssertNotNull(plot.YAxisTitlePlaceholder, null).Passed)
        {
            plot.XAxisTitlePlaceholder.Content = plot.RangeAxis.AxisTitle;
            plot.YAxisTitlePlaceholder.Content = plot.PrimaryResultAxis.AxisTitle;
            plot.XAxisTitlePlaceholder.Visibility = plot.RangeAxis.AxisTitle.ViewModel.ShowTitle ? Visibility.Visible : Visibility.Collapsed;
            plot.YAxisTitlePlaceholder.Visibility = plot.PrimaryResultAxis.AxisTitle.ViewModel.ShowTitle ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public List<EquationControl> BuildEditableAxisRelatedElements(PlotControl plot)
    {
        List<EquationControl> editableAxisRelatedElements = new List<EquationControl>();
        Action<AxisMarkerViewModel> action = (AxisMarkerViewModel marker) =>
        {
            editableAxisRelatedElements.Add(plot.GetMarkerEquation(marker));
        };
        editableAxisRelatedElements.AddRange(plot.PrimaryResultAxis.CollectElementsForTraversal());
        plot.ViewModel.Markers.HorizontalMarkers.Each(action);
        editableAxisRelatedElements.AddRange(plot.RangeAxis.CollectElementsForTraversal());
        plot.ViewModel.Markers.VerticalMarkers.Each(action);
        return editableAxisRelatedElements;
    }

    public void SetMinWidthHeight(PlotControl plot)
    {
        double minLegendPlaceholderWidth = plot.YAxisTitlePlaceholder.GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderHeight = plot.YAxisTitlePlaceholder.GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderWidth2 = plot.XAxisTitlePlaceholder.GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderHeight2 = plot.XAxisTitlePlaceholder.GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderWidth3 = plot.XLegendsPositions[0].GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderWidth4 = plot.XLegendsPositions[1].GetMinLegendPlaceholderWidth();
        double titleWidth = new double[2]
        {
            plot.PlotTitlePositions[0].GetMinLegendPlaceholderWidth(),
            plot.PlotTitlePositions[1].GetMinLegendPlaceholderWidth()
        }.Max();
        double plotLegendWidth = 0.0;
        double plotLegendColumnsWidth = 0.0;
        if (plot.PlotLegendPositions != null)
        {
            plotLegendWidth = plot.PlotLegendPositions[0].GetMinLegendPlaceholderWidth();
            double minLegendPlaceholderWidth5 = plot.PlotLegendPositions[1].GetMinLegendPlaceholderWidth();
            double minLegendPlaceholderWidth6 = plot.PlotLegendPositions[2].GetMinLegendPlaceholderWidth();
            double minLegendPlaceholderWidth7 = plot.PlotLegendPositions[3].GetMinLegendPlaceholderWidth();
            double minLegendPlaceholderWidth8 = plot.PlotLegendPositions[4].GetMinLegendPlaceholderWidth();
            plotLegendColumnsWidth = new double[4] { minLegendPlaceholderWidth5, minLegendPlaceholderWidth6, minLegendPlaceholderWidth7, minLegendPlaceholderWidth8 }.Max();
        }
        double plotLegendMaxWidth = Math.Max(plotLegendWidth, plotLegendColumnsWidth);
        double rangeLegendWidth = new double[6] { minLegendPlaceholderWidth3, minLegendPlaceholderWidth4, plot.GraphMinWidth, titleWidth, minLegendPlaceholderWidth2, plotLegendMaxWidth }.Max();
        double minLegendPlaceholderWidth9 = plot.XLegendsPositions[2].GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderWidth10 = plot.XLegendsPositions[3].GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderWidth11 = plot.XLegendsPositions[4].GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderWidth12 = plot.XLegendsPositions[5].GetMinLegendPlaceholderWidth();
        double resultLegendWidth = new double[4] { minLegendPlaceholderWidth9, minLegendPlaceholderWidth10, minLegendPlaceholderWidth11, minLegendPlaceholderWidth12 }.Max();
        double minLegendPlaceholderWidth13 = plot.YLegendsPositions[0].GetMinLegendPlaceholderWidth();
        double minLegendPlaceholderWidth14 = plot.YLegendsPositions[1].GetMinLegendPlaceholderWidth();
        double perpendicularAxisPositionRatio = plot.RangeAxis.GetPerpendicularAxisPositionRatio();
        double num3 = plot.YLegendsPositions[2].GetMinLegendPlaceholderWidth() - plot.GraphMinWidth * perpendicularAxisPositionRatio;
        double num4 = plot.YLegendsPositions[3].GetMinLegendPlaceholderWidth() - plot.GraphMinWidth * (1.0 - perpendicularAxisPositionRatio);
        double num5 = plot.YLegendsPositions[4].GetMinLegendPlaceholderWidth() - plot.GraphMinWidth * perpendicularAxisPositionRatio;
        double num6 = plot.YLegendsPositions[5].GetMinLegendPlaceholderWidth() - plot.GraphMinWidth * (1.0 - perpendicularAxisPositionRatio);
        double yLegendOverflowWidth = new double[5] { num3, num4, num5, num6, 0.0 }.Max();
        plot.MinWidth = Safe(minLegendPlaceholderWidth) + Safe(minLegendPlaceholderWidth13) + Safe(rangeLegendWidth) + Safe(minLegendPlaceholderWidth14) + Safe(resultLegendWidth) + Safe(yLegendOverflowWidth);
        double minLegendPlaceholderHeight3 = plot.YLegendsPositions[0].GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderHeight4 = plot.YLegendsPositions[1].GetMinLegendPlaceholderHeight();
        double yLegendHeight = new double[4] { minLegendPlaceholderHeight3, minLegendPlaceholderHeight4, plot.GraphMinHeight, minLegendPlaceholderHeight }.Max();
        double minLegendPlaceholderHeight5 = plot.YLegendsPositions[2].GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderHeight6 = plot.YLegendsPositions[3].GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderHeight7 = plot.YLegendsPositions[4].GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderHeight8 = plot.YLegendsPositions[5].GetMinLegendPlaceholderHeight();
        double yLegendsColumnHeight = new double[4] { minLegendPlaceholderHeight5, minLegendPlaceholderHeight6, minLegendPlaceholderHeight7, minLegendPlaceholderHeight8 }.Max();
        double perpendicularAxisPositionRatio2 = plot.PrimaryResultAxis.GetPerpendicularAxisPositionRatio();
        double num8 = plot.XLegendsPositions[2].GetMinLegendPlaceholderHeight() - plot.GraphMinHeight * (1.0 - perpendicularAxisPositionRatio2);
        double num9 = plot.XLegendsPositions[3].GetMinLegendPlaceholderHeight() - plot.GraphMinHeight * (1.0 - perpendicularAxisPositionRatio2);
        double num10 = plot.XLegendsPositions[4].GetMinLegendPlaceholderHeight() - plot.GraphMinHeight * perpendicularAxisPositionRatio2;
        double num11 = plot.XLegendsPositions[5].GetMinLegendPlaceholderHeight() - plot.GraphMinHeight * perpendicularAxisPositionRatio2;
        double xLegendOverflowHeight = new double[5] { num8, num9, num10, num11, 0.0 }.Max();
        double value4 = new double[2]
        {
            plot.PlotTitlePositions[0].GetMinLegendPlaceholderHeight(),
            plot.PlotTitlePositions[1].GetMinLegendPlaceholderHeight()
        }.Max();
        double value5 = plot.PlotLegendPositions != null ? plot.PlotLegendPositions[0].GetMinLegendPlaceholderHeight() : 0.0;
        plot.MinHeight = Safe(plot.GridCellSize.Height) + Safe(value4) + Safe(minLegendPlaceholderHeight2) + Safe(value5) + Safe(Math.Max(yLegendHeight, yLegendsColumnHeight + xLegendOverflowHeight));
        static double Safe(double result)
        {
            if (!double.IsNaN(result))
            {
                return result;
            }
            return 0.0;
        }
    }

    public void GetInitialPlaceholderIndices(PlotControl plot, out int xLegendIndex, out int yLegendIndex, out int plotTitleIndex, out int plotLegendIndex)
    {
        xLegendIndex = 0;
        yLegendIndex = 1;
        plotTitleIndex = 1;
        plotLegendIndex = 0;
    }
    public XYPlotStrategy() { }
}
