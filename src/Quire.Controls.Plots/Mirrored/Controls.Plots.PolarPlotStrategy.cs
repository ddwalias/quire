#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Quire.Controls.Plots;

internal partial class PolarPlotStrategy
{
    public void InitializeAxes(PlotTitle xAxisTitle, PlotTitle yAxisTitle, PlotTitle title, EquationList rangeEquations, EquationList primaryResultEquations, out PlotGraphAxis rangeAxis, out PlotGraphAxis primaryResultAxis)
    {
        rangeEquations.EquationItemWOUnit = true;
        rangeAxis = new PlotGraphAngularAxis(rangeEquations);
        primaryResultAxis = new PlotGraphRadialAxis(primaryResultEquations);
        rangeEquations.Header = rangeEquations.Resources["AngularLegend.Header"] as string;
        primaryResultEquations.Header = primaryResultEquations.Resources["RadialLegend.Header"] as string;
        if (title != null)
        {
            title.Header = title.Resources["PlotTitle.Header"] as string;
        }
        rangeAxis.OtherAxis = primaryResultAxis;
        primaryResultAxis.OtherAxis = rangeAxis;
    }

    public void InitializeLegendPositions(PlotControl plot, out PlotItemPlaceholder[] rangeLegendsPositions, out PlotItemPlaceholder[] resultLegendsPositions, out PlotItemPlaceholder[] titlePositions, out PlotItemPlaceholder[] plotLegendPositions)
    {
        rangeLegendsPositions = new PlotItemPlaceholder[2];
        resultLegendsPositions = new PlotItemPlaceholder[2];
        titlePositions = new PlotItemPlaceholder[2];
        plotLegendPositions = new PlotItemPlaceholder[1];
        rangeLegendsPositions[0] = plot.Template.FindName("PART_X0", plot) as PlotItemPlaceholder;
        rangeLegendsPositions[1] = plot.Template.FindName("PART_X1", plot) as PlotItemPlaceholder;
        resultLegendsPositions[0] = plot.Template.FindName("PART_Y0", plot) as PlotItemPlaceholder;
        resultLegendsPositions[1] = plot.Template.FindName("PART_Y1", plot) as PlotItemPlaceholder;
        titlePositions[0] = plot.Template.FindName("PART_TitleBottom", plot) as PlotItemPlaceholder;
        titlePositions[1] = plot.Template.FindName("PART_TitleTop", plot) as PlotItemPlaceholder;
        plotLegendPositions[0] = plot.Template.FindName("PART_PlotLegendBelow", plot) as PlotItemPlaceholder;
    }

    public void ApplyPlotTemplate(PlotControl plot)
    {
        ContentControl contentControl = plot.Template.FindName("PART_PlotGraphRadialAxis", plot) as ContentControl;
        if (contentControl != null)
        {
            contentControl.Content = plot.PrimaryResultAxis;
        }
        ContentControl angularAxisControl = plot.Template.FindName("PART_PlotGraphAngularAxis", plot) as ContentControl;
        if (angularAxisControl != null)
        {
            angularAxisControl.Content = plot.RangeAxis;
        }
    }

    public List<EquationControl> BuildEditableAxisRelatedElements(PlotControl plot)
    {
        List<EquationControl> list = new List<EquationControl>();
        list.AddRange(plot.PrimaryResultAxis.CollectElementsForTraversal());
        list.AddRange(plot.RangeAxis.CollectElementsForTraversal());
        return list;
    }

    public void SetMinWidthHeight(PlotControl plot)
    {
        double minLegendPlaceholderWidth = plot.XLegendsPositions[0].GetMinLegendPlaceholderWidth();
        double secondXLegendWidth = plot.XLegendsPositions[1].GetMinLegendPlaceholderWidth();
        double plotTitleWidth = new double[2]
        {
            plot.PlotTitlePositions[0].GetMinLegendPlaceholderWidth(),
            plot.PlotTitlePositions[1].GetMinLegendPlaceholderWidth()
        }.Max();
        double legendPlaceholderWidth = 0.0;
        if (plot.PlotLegendPositions != null)
        {
            legendPlaceholderWidth = plot.PlotLegendPositions[0].GetMinLegendPlaceholderWidth();
        }
        double value = new double[5] { minLegendPlaceholderWidth, secondXLegendWidth, plot.GraphMinWidth, plotTitleWidth, legendPlaceholderWidth }.Max();
        double firstYLegendWidth = plot.YLegendsPositions[0].GetMinLegendPlaceholderWidth();
        double secondYLegendWidth = plot.YLegendsPositions[1].GetMinLegendPlaceholderWidth();
        plot.MinWidth = Safe(firstYLegendWidth) + Safe(value) + Safe(secondYLegendWidth);
        double titlePlaceholderHeight = plot.PlotTitlePositions[0].GetMinLegendPlaceholderHeight() + plot.PlotTitlePositions[1].GetMinLegendPlaceholderHeight();
        double minLegendPlaceholderHeight = plot.YLegendsPositions[0].GetMinLegendPlaceholderHeight();
        double xLegendHeight = plot.YLegendsPositions[1].GetMinLegendPlaceholderHeight();
        double plotBodyHeight = new double[3] { minLegendPlaceholderHeight, xLegendHeight, plot.GraphMinHeight }.Max();
        double firstYLegendHeight = plot.XLegendsPositions[0].GetMinLegendPlaceholderHeight();
        double secondYLegendHeight = plot.XLegendsPositions[1].GetMinLegendPlaceholderHeight();
        double legendPlaceholderHeight = ((plot.PlotLegendPositions != null) ? plot.PlotLegendPositions[0].GetMinLegendPlaceholderHeight() : 0.0);
        plot.MinHeight = Safe(firstYLegendHeight) + Safe(plotBodyHeight) + Safe(secondYLegendHeight) + Safe(titlePlaceholderHeight) + Safe(legendPlaceholderHeight);
        static double Safe(double value)
        {
            if (!double.IsNaN(value))
            {
                return value;
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

    public PolarPlotStrategy()
    {
    }
}
