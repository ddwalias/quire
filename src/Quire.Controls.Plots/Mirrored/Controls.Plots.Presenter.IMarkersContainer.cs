using System;
using System.ComponentModel;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;

namespace Quire.Controls.Plots.Presenter;

public partial interface IMarkersContainer : INotifyPropertyChanged
{
    double Start { get; }

    double End { get; }

    AxisMarkersCollection Markers { get; }

    NumberFormat DisplayNumberFormat { get; }

    bool CanHaveMarkers { get; }

    event Action RangeChanged;
}
