namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotViewBase
{
    Core.BackgroundTypeE PlotBackgroundType { get; set; }
    event Delegate1<Core.BackgroundTypeE> BackgroundTypePropertyChanged;
}
