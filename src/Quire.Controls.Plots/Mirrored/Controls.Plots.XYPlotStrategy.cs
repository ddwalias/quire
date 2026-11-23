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

    public void InitializeLegendPositions(PlotControl plot, out PlotItemPlaceholder[] rangeLegendsPositions, out PlotItemPlaceholder[] resultLegendsPositions, out PlotItemPlaceholder[] titlePositions, out PlotItemPlaceholder[] plotLegendPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ApplyPlotTemplate(PlotControl plot) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    public void SetMinWidthHeight(PlotControl plot) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void GetInitialPlaceholderIndices(PlotControl plot, out int xLegendIndex, out int yLegendIndex, out int plotTitleIndex, out int plotLegendIndex)
    {
        xLegendIndex = 0;
        yLegendIndex = 1;
        plotTitleIndex = 1;
        plotLegendIndex = 0;
    }
    public XYPlotStrategy() { }
}
