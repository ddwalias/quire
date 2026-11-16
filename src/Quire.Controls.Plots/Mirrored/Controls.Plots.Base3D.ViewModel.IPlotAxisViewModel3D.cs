using System;
using System.ComponentModel;
using System.Windows.Media;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots.Base3D.ViewModel;

public partial interface IPlotAxisViewModel3D : IAxisViewModel, INotifyPropertyChanged
{
    string[] Labels { get; }

    bool IsActive { get; set; }

    bool IsHighlighted { get; set; }

    Color LineColor { get; }

    string[] FilteredLabels { get; }

    event Action RangeChanged;

    event Action TickMarkLabelsUpdated;
}
