using System.ComponentModel;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

public partial interface IAxisMarker : IBaseMarker, INotifyPropertyChanged
{
    double Value { get; set; }

    LineStyle LineStyle { get; set; }

    double LineWeight { get; set; }

    RealNumberCalculatable RealNumberCalculatable { get; }
}
