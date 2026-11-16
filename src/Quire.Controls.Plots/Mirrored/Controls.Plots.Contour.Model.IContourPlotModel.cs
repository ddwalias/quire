using System.ComponentModel;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Contour.Model;

public partial interface IContourPlotModel : IPlotModelBase, INotifyPropertyChanged
{
    ColorScheme ColorScheme { get; set; }

    LineStyle LineStyle { get; set; }

    Color ContoursColor { get; set; }

    bool ShowLabels { get; set; }

    double LineWeight { get; set; }

    bool HasOriginPositioning { get; set; }

    BackgroundTypeE PlotBackgroundType { get; set; }

    ICalculatable ContourGraphCalculatable { get; }

    ContourLogicalDataCollection LogicalContoursDataCollection { get; }

    void InitializeZEquation(IPlotEquation zEquation);
}
