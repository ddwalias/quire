#nullable disable
#pragma warning disable

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Quire.Undo;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.ViewModel;

internal partial class MarkersViewModel
{
    private static readonly global::System.Type _typeOfThis;

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

    internal global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel AddVerticalMarker()
    {
        VerticalMarkerViewModel viewModel;
        AxisMarker marker = CreateDefaultAxisMarker(_horizontalRange);
        viewModel = CreateVerticalMarkerViewModel(marker);
        CommandBuffer.Execute(new UndoableCommand("Add Vertical Marker", () =>
        {
            AddVerticalMarker(viewModel);
            return true;
        }, () => DeleteMarker(viewModel)), null);
        return viewModel;
    }

    private VerticalMarkerViewModel CreateVerticalMarkerViewModel(AxisMarker marker)
    {
        return new VerticalMarkerViewModel(marker)
        {
            DisplayNumberFormat = _horizontalRange.DisplayNumberFormat,
        };
    }

    internal global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel AddHorizontalMarker()
    {
        HorizontalMarkerViewModel viewModel;
        AxisMarker marker = CreateDefaultAxisMarker(_verticalRange);
        viewModel = CreateHorizontalMarkerViewModel(marker);
        CommandBuffer.Execute(new UndoableCommand("Add Horizontal Marker", () =>
        {
            AddHorizontalMarker(viewModel);
            return true;
        }, () => DeleteMarker(viewModel)), null);
        return viewModel;
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

    private void HandleRangeChanged(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> markers, global::Quire.Controls.Plots.Presenter.IMarkersContainer markersContainer)
    {
        if (!markersContainer.CanHaveMarkers)
        {
            foreach (AxisMarkerViewModel marker in markers.ToList())
            {
                DeleteMarker(marker);
            }
        }
        foreach (AxisMarkerViewModel marker in markers)
        {
            SetCustomErrorIfMarkerOutOfRange(marker, markersContainer);
        }
    }

    private static void HandleRangePropertyChanged(global::System.ComponentModel.PropertyChangedEventArgs e, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> markers, global::Quire.Controls.Plots.Presenter.IMarkersContainer markersContainer)
    {
        if (e.PropertyName == "DisplayNumberFormat")
        {
            foreach (AxisMarkerViewModel marker in markers)
            {
                marker.DisplayNumberFormat = markersContainer.DisplayNumberFormat;
            }
        }
    }

    private void HorizontalMarkersCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        HandleMarkersCollectionChanged(e, HorizontalMarkerPropertyChanged);
    }

    private void HorizontalMarkerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        MarkerPropertyChangedInCollection(sender, e, HorizontalMarkers, _verticalRange);
    }

    private void VerticalMarkersCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        HandleMarkersCollectionChanged(e, VerticalMarkerPropertyChanged);
    }

    private void VerticalMarkerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        MarkerPropertyChangedInCollection(sender, e, VerticalMarkers, _horizontalRange);
    }

    private void MarkerPropertyChangedInCollection(object sender, global::System.ComponentModel.PropertyChangedEventArgs e, global::System.Collections.ObjectModel.ObservableCollection<global::Quire.Controls.Plots.ViewModel.AxisMarkerViewModel> markersCollection, global::Quire.Controls.Plots.Presenter.IMarkersContainer container)
    {
        AxisMarkerViewModel marker = sender as AxisMarkerViewModel;
        if (marker == null)
        {
            return;
        }
        switch (e.PropertyName)
        {
            case "Value":
                SetCustomErrorIfMarkerOutOfRange(marker, container);
                MoveToNewPlace(sender as AxisMarkerViewModel, markersCollection);
                break;
            case "Color":
            case "LineStyle":
            case "LineWeight":
                if (ActiveMarker == marker)
                {
                    OnActiveMarkerStyleChanged();
                }
                break;
        }
        MarkerPropertyChanged(sender, e);
    }

    private void MarkerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        // The IL compares the literal on the left of ==.
        if ("IsActive" == e.PropertyName)
        {
            BaseMarkerViewModel marker = (BaseMarkerViewModel)sender;
            if (marker.IsActive || marker == ActiveMarker)
            {
                ActiveMarker = marker.IsActive ? marker : null;
            }
        }
    }

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

    public void UpdateFromModel()
    {
        UpdateHorizontalMarkersFromModel();
        UpdateVerticalMarkersFromModel();
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

    private void HandleMarkersCollectionChanged(global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e, global::System.ComponentModel.PropertyChangedEventHandler handler)
    {
        if (e.Action == NotifyCollectionChangedAction.Move)
        {
            return;
        }
        if (e.OldItems != null)
        {
            foreach (AxisMarkerViewModel marker in e.OldItems)
            {
                marker.PropertyChanged -= handler;
                AllMarkers.Remove(marker);
                FireCalculatableRemoved(marker.Calculatable);
            }
        }
        if (e.NewItems != null)
        {
            foreach (AxisMarkerViewModel marker in e.NewItems)
            {
                marker.PropertyChanged += handler;
                AllMarkers.Add(marker);
                FireCalculatableAdded(marker.Calculatable);
            }
        }
        global::Quire.DebugTools.DevAssert(
            e.Action == NotifyCollectionChangedAction.Add
            || e.Action == NotifyCollectionChangedAction.Move
            || e.Action == NotifyCollectionChangedAction.Remove
            || e.Action == NotifyCollectionChangedAction.Replace,
            "Unsupported action: {0}",
            e.Action);
    }

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

}
