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

    private void OnMarkerAdded(BaseMarkerViewModel marker)
    {
        ActivateMarker(marker);
        if (MarkerAdded != null)
        {
            MarkerAdded(this, new MarkerEventArgs(marker));
        }
    }

    private void OnMarkerDeleted(BaseMarkerViewModel marker)
    {
        if (marker == ActiveMarker)
        {
            ActivateMarker(null);
        }
        marker.Calculatable.OverrideHideFromCalculation = true;
        if (MarkerDeleted != null)
        {
            MarkerDeleted(this, new MarkerEventArgs(marker));
        }
    }

    private static void SetMarkerOutOfRangeError(IAxisMarker marker)
    {
        marker.RealNumberCalculatable.SetCustomError("Ptc.Plots.MARKER_OUT_OF_RANGE");
    }

    private static void ClearMarkerOutOfRangeError(IAxisMarker marker)
    {
        marker.RealNumberCalculatable.ClearCustomErrors("Ptc.Plots.MARKER_OUT_OF_RANGE");
    }

    private VerticalMarkerViewModel CreateVerticalMarkerViewModel(AxisMarker marker)
    {
        return new VerticalMarkerViewModel(marker)
        {
            DisplayNumberFormat = _horizontalRange.DisplayNumberFormat,
        };
    }

    private HorizontalMarkerViewModel CreateHorizontalMarkerViewModel(AxisMarker marker)
    {
        return new HorizontalMarkerViewModel(marker)
        {
            DisplayNumberFormat = _verticalRange.DisplayNumberFormat,
        };
    }

    internal void AddPointMarker()
    {
    }

    private void AddVerticalMarker(VerticalMarkerViewModel viewModel)
    {
        VerticalMarkers.Add(viewModel);
        _horizontalRange.Markers.Add(viewModel.Model as AxisMarker);
        OnMarkerAdded(viewModel);
    }

    private void AddHorizontalMarker(HorizontalMarkerViewModel viewModel)
    {
        HorizontalMarkers.Add(viewModel);
        _verticalRange.Markers.Add(viewModel.Model as AxisMarker);
        OnMarkerAdded(viewModel);
    }
}
