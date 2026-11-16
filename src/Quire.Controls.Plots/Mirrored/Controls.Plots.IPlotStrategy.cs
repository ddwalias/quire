using System.Collections.Generic;

namespace Quire.Controls.Plots;

internal partial interface IPlotStrategy
{
    void InitializeAxes(PlotTitle xAxisTitle, PlotTitle yAxisTitle, PlotTitle title, EquationList rangeEquations, EquationList primaryResultEquations, out PlotGraphAxis rangeAxis, out PlotGraphAxis primaryResultAxis);

    void InitializeLegendPositions(PlotControl plot, out PlotItemPlaceholder[] rangeLegendsPositions, out PlotItemPlaceholder[] resultLegendsPositions, out PlotItemPlaceholder[] titlePositions, out PlotItemPlaceholder[] plotLegendPositions);

    void ApplyPlotTemplate(PlotControl plot);

    List<global::Quire.Controls.EquationControl> BuildEditableAxisRelatedElements(PlotControl plot);

    void SetMinWidthHeight(PlotControl plot);

    void GetInitialPlaceholderIndices(PlotControl plot, out int xLegendIndex, out int yLegendIndex, out int plotTitleIndex, out int plotLegendIndex);
}
