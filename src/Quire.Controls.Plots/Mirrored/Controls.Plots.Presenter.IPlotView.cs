#nullable disable
#pragma warning disable

using System.Collections.Generic;
using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotView : IPlotViewBase, ICalculatableContainer, IHasChildArrangeInfoProvider
{
    IPlotAxisView RangeAxis { get; }

    IPlotAxisView PrimaryResultAxis { get; }

    IPlotLegend PlotLegend { get; }

    List<IPlotTrace> SelectedTraces { get; set; }

    PlotViewType Type { get; set; }

    int XLegendPositionIndex { get; set; }

    int YLegendPositionIndex { get; set; }

    int PlotTitlePositionIndex { get; set; }

    int PlotLegendPositionIndex { get; set; }

    IPlotEquation ActivePlotEquation { get; set; }

    event Delegate1<bool> HasOriginPositioningChanged;

    event Delegate1<int> YLegendPositionsIndexChanged;

    event Delegate1<int> XLegendPositionsIndexChanged;

    event Delegate1<LegendPositions.PlotTitlePosition> PlotTitlePositionsIndexChanged;

    event Delegate1<int> PlotLegendPositionsIndexChanged;

    IPlotTrace AddTrace();

    void RemoveTrace(IPlotTrace trace);

    void FireCalculatableAdded(ICalculatable calculatable);

    void FireCalculatableRemoved(ICalculatable calculatable);

    void UpdateAxisOriginPositions(bool hasOriginPositioning);
}
