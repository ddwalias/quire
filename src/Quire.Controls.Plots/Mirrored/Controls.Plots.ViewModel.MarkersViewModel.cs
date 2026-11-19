#nullable disable
#pragma warning disable

using System.Collections.ObjectModel;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.ViewModel;

internal partial class MarkersViewModel
{
    public IValueToScreen VerticalValueToScreen { get; private set; }

    public IValueToScreen HorizontalValueToScreen { get; private set; }

    public ObservableCollection<AxisMarkerViewModel> VerticalMarkers => _verticalMarkers;

    public ObservableCollection<AxisMarkerViewModel> HorizontalMarkers => _horizontalMarkers;

    public IObservableRangeCollection<BaseMarkerViewModel> AllMarkers => _allMarkers;

    public BaseMarkerViewModel ActiveMarker
    {
        get
        {
            return _activeMarker;
        }
        private set
        {
            if (_activeMarker != value)
            {
                _activeMarker = value;
                OnPropertyChanged("ActiveMarker");
            }
        }
    }

    public RoutedEventBindingCollection RoutedEventBindings => _routedEventBindings;
}
