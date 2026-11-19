#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Plots.Presenter;

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
}
