#nullable disable
#pragma warning disable

using System.Collections.ObjectModel;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Plots.Model;
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

    internal bool CanDeleteMarker()
    {
        return ActiveMarker != null;
    }

    private void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(calculatable);
        }
    }

    private void FireCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calculatable)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calculatable);
        }
    }

    private void OnActiveMarkerStyleChanged()
    {
        if (ActiveMarkerStyleChanged != null)
        {
            ActiveMarkerStyleChanged();
        }
    }

    private void UpdateVerticalMarkersFromModel()
    {
        VerticalMarkers.Clear();
        foreach (AxisMarker marker in _horizontalRange.Markers)
        {
            VerticalMarkers.Add(CreateVerticalMarkerViewModel(marker));
        }
    }

    private void UpdateHorizontalMarkersFromModel()
    {
        HorizontalMarkers.Clear();
        foreach (AxisMarker marker in _verticalRange.Markers)
        {
            HorizontalMarkers.Add(CreateHorizontalMarkerViewModel(marker));
        }
    }
}
