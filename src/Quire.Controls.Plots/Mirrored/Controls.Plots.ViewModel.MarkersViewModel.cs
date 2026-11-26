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
    private readonly global::Quire.Controls.Plots.Presenter.IMarkersContainer _verticalRange;

    private readonly global::Quire.Controls.Plots.Presenter.IMarkersContainer _horizontalRange;

    private readonly global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> _verticalMarkers;

    private readonly global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> _horizontalMarkers;

    private readonly global::Quire.Wpf.IObservableRangeCollection<global::Quire.Controls.Plots.ViewModel.BaseMarkerViewModel> _allMarkers;

    private readonly global::Quire.Controls.Core.AttachedBehavior.RoutedEventBindingCollection _routedEventBindings;

    public static readonly global::System.Windows.Input.RoutedUICommand AddVerticalMarkerCommand;

    public static readonly global::System.Windows.Input.RoutedUICommand AddHorizontalMarkerCommand;

    public static readonly global::System.Windows.Input.RoutedCommand AddPointMarkerCommand;

    public static readonly global::System.Windows.Input.RoutedUICommand DeleteMarkerCommand;

    public MarkersViewModel(global::Quire.Controls.Plots.Presenter.IMarkersContainer horizontalRange, global::Quire.Controls.Plots.Presenter.IMarkersContainer verticalRange, global::Quire.Controls.Plots.Presenter.IValueToScreen horizontalValueToScreen, global::Quire.Controls.Plots.Presenter.IValueToScreen verticalValueToScreen) { }

    public event global::System.EventHandler<global::Quire.Controls.Plots.ViewModel.MarkerEventArgs> MarkerAdded;

    private void OnMarkerAdded(BaseMarkerViewModel marker)
    {
        ActivateMarker(marker);
        if (MarkerAdded != null)
        {
            MarkerAdded(this, new MarkerEventArgs(marker));
        }
    }

    public event global::System.EventHandler<global::Quire.Controls.Plots.ViewModel.MarkerEventArgs> MarkerDeleted;

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

    public event global::System.Action ActiveMarkerStyleChanged;

    private void OnActiveMarkerStyleChanged()
    {
        if (ActiveMarkerStyleChanged != null)
        {
            ActiveMarkerStyleChanged();
        }
    }

    public IValueToScreen VerticalValueToScreen { get; private set; }

    public IValueToScreen HorizontalValueToScreen { get; private set; }

    public ObservableCollection<AxisMarkerViewModel> VerticalMarkers => _verticalMarkers;

    public ObservableCollection<AxisMarkerViewModel> HorizontalMarkers => _horizontalMarkers;

    public IObservableRangeCollection<BaseMarkerViewModel> AllMarkers => _allMarkers;

    private global::Quire.Controls.Plots.ViewModel.BaseMarkerViewModel _activeMarker;

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

    internal global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel AddVerticalMarker() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private VerticalMarkerViewModel CreateVerticalMarkerViewModel(AxisMarker marker)
    {
        return new VerticalMarkerViewModel(marker)
        {
            DisplayNumberFormat = _horizontalRange.DisplayNumberFormat,
        };
    }

    internal global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel AddHorizontalMarker() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    internal bool CanDeleteMarker()
    {
        return ActiveMarker != null;
    }

    internal void DeleteMarker() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void AddMarker(BaseMarkerViewModel marker)
    {
        if (marker is VerticalMarkerViewModel)
        {
            AddVerticalMarker(marker as VerticalMarkerViewModel);
            MoveToNewPlace(marker as AxisMarkerViewModel, VerticalMarkers);
        }
        else if (marker is HorizontalMarkerViewModel)
        {
            AddHorizontalMarker(marker as HorizontalMarkerViewModel);
            MoveToNewPlace(marker as AxisMarkerViewModel, HorizontalMarkers);
        }
        else
        {
            global::Quire.DebugTools.DevAssert(false, "Not supported marker type");
        }
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

    private void ActivateMarker(BaseMarkerViewModel marker)
    {
        if (ActiveMarker != null)
        {
            ActiveMarker.Deactivate();
        }
        if (marker != null)
        {
            marker.Activate();
        }
        ActiveMarker = marker;
    }

    private void DeleteMarker(BaseMarkerViewModel marker)
    {
        if (marker is VerticalMarkerViewModel)
        {
            VerticalMarkers.Remove(marker as VerticalMarkerViewModel);
            _horizontalRange.Markers.Remove(marker.Model as AxisMarker);
        }
        else if (marker is HorizontalMarkerViewModel)
        {
            HorizontalMarkers.Remove(marker as HorizontalMarkerViewModel);
            _verticalRange.Markers.Remove(marker.Model as AxisMarker);
        }
        else
        {
            global::Quire.DebugTools.DevAssert(false, "Not supported marker type");
        }
        OnMarkerDeleted(marker);
    }

    private static AxisMarker CreateDefaultAxisMarker(IMarkersContainer markersContainer)
    {
        return new AxisMarker
        {
            Value = markersContainer.End,
            Color = global::System.Windows.Media.Colors.Black,
            LineStyle = LineStyle.Solid,
            LineWeight = 1.0,
        };
    }

    private static void SetMarkerOutOfRangeError(IAxisMarker marker)
    {
        marker.RealNumberCalculatable.SetCustomError("Ptc.Plots.MARKER_OUT_OF_RANGE");
    }

    private static void ClearMarkerOutOfRangeError(IAxisMarker marker)
    {
        marker.RealNumberCalculatable.ClearCustomErrors("Ptc.Plots.MARKER_OUT_OF_RANGE");
    }

    private static void SetCustomErrorIfMarkerOutOfRange(IAxisMarker marker, IMarkersContainer markersContainer)
    {
        ClearMarkerOutOfRangeError(marker);
        if (markersContainer.Start > marker.Value)
        {
            SetMarkerOutOfRangeError(marker);
        }
        if (markersContainer.End < marker.Value)
        {
            SetMarkerOutOfRangeError(marker);
        }
    }

    private void HandleRangeChanged(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> markers, global::Quire.Controls.Plots.Presenter.IMarkersContainer markersContainer) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void HandleRangePropertyChanged(global::System.ComponentModel.PropertyChangedEventArgs e, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> markers, global::Quire.Controls.Plots.Presenter.IMarkersContainer markersContainer) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HorizontalMarkersCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HorizontalMarkerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void VerticalMarkersCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void VerticalMarkerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void MarkerPropertyChangedInCollection(object sender, global::System.ComponentModel.PropertyChangedEventArgs e, global::System.Collections.ObjectModel.ObservableCollection<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> markersCollection, global::Quire.Controls.Plots.Presenter.IMarkersContainer container) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void MarkerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void MoveToNewPlace(AxisMarkerViewModel marker, ObservableCollection<AxisMarkerViewModel> markers)
    {
        int oldIndex = markers.IndexOf(marker);
        for (int i = 0; i < markers.Count; i++)
        {
            if (marker.Value < markers[i].Value)
            {
                int adjust = i < oldIndex ? 0 : -1;
                markers.Move(oldIndex, i + adjust);
                return;
            }
        }
    }

    public void UpdateFromModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    private void HandleMarkersCollectionChanged(global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e, global::System.ComponentModel.PropertyChangedEventHandler handler) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> InternalCalculatables { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;

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

    private static readonly global::System.Type _typeOfThis;
}
