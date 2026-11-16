using System;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotLegend
{
    PlotLegendViewModel ViewModel { get; }

    double LegendWidth { get; set; }

    event Action<double> LegendWidthChanged;

    IPlotLegendItem AddNewItemToPlotLegend(int index, IPlotTrace getViewTrace);

    IPlotLegendItem RemoveTraceFromPlotLegend(IPlotTrace getViewTrace);
}
