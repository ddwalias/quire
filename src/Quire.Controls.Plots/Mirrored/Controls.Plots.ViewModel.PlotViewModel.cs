#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.ViewModel;

internal partial class PlotViewModel
{
    public PlotViewModel(global::Quire.Controls.Plots.Presenter.IPlotModel model, global::Quire.Controls.Plots.Presenter.IValueToScreen horizontalValueToScreen, global::Quire.Controls.Plots.Presenter.IValueToScreen verticalValueToScreen) { }

    private void MarkersPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetValuesForStyleProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ShowAxisExpressionsCommandExecute(object arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void UpdateFromModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private readonly global::Quire.Controls.Plots.Presenter.IPlotModel _model;

    public static readonly global::System.Windows.Input.RoutedCommand ActivateCommand;

    public static readonly global::System.Windows.Input.RoutedCommand ShowAxisExpressionsCommand;

    public global::Quire.Controls.Plots.ViewModel.MarkersViewModel Markers { get; private set; }

    public bool ShowAxisExpressions
    {
        get
        {
            return _model.ShowAxisExpressions;
        }
        set
        {
            _model.ShowAxisExpressions = value;
        }
    }

    public event global::System.EventHandler<global::System.EventArgs> Activated;

    private void Activate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool _isActive;

    public bool IsActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            if (value != _isActive)
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }

    private global::System.Collections.Generic.List<global::Quire.Controls.Plots.Presenter.IPlotTrace> _selectedTraces;

    public List<IPlotTrace> SelectedTraces
    {
        get
        {
            return _selectedTraces;
        }
        set
        {
            if (!_selectedTraces.IsNullOrEmpty())
            {
                _selectedTraces.Each(DeselectTrace);
            }
            _selectedTraces = value;
            if (!_selectedTraces.IsNullOrEmpty())
            {
                _selectedTraces.Each(SelectTrace);
            }
            SetValuesForStyleProperties();
            OnPropertyChanged("SelectedTraces");
        }
    }

    private void SelectTrace(global::Quire.Controls.Plots.Presenter.IPlotTrace trace) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void TraceChanged(global::Quire.Controls.Plots.Presenter.IPlotTrace obj) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DeselectTrace(global::Quire.Controls.Plots.Presenter.IPlotTrace trace) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetValuesForSelectedTraceProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetValuesForActiveMarker() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetNullValuesForSelectedTraceProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Nullable<T> GetAggregatedTracesProperty<T>(global::System.Func<global::Quire.Controls.Plots.Presenter.IPlotTrace, T> propertySelector) where T : struct => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static TResult GetAggregatedProperty<TSource, T, TResult>(global::System.Collections.Generic.IEnumerable<TSource> source, global::System.Func<TSource, T> propertySelector, global::System.Func<T, TResult> resultCorverter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Nullable<global::System.Windows.Media.Color> _selectedColor;

    public Color? SelectedColor
    {
        get
        {
            return _selectedColor;
        }
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;
                OnPropertyChanged("SelectedColor");
            }
        }
    }

    private global::System.Nullable<global::Quire.Controls.Plots.Presenter.LineStyle> _selectedLineStyle;

    public global::System.Nullable<global::Quire.Controls.Plots.Presenter.LineStyle> SelectedLineStyle { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::System.Nullable<double> _selectedLineWeight;

    public global::System.Nullable<double> SelectedLineWeight { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::System.Nullable<global::Quire.Calculation.Core.Trace2DResults.TraceType> _selectedTracesType;

    public global::System.Nullable<global::Quire.Calculation.Core.Trace2DResults.TraceType> SelectedTracesType { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private global::System.Nullable<global::Quire.Controls.Plots.Presenter.TraceSymbol> _selectedTracesSymbol;

    public global::System.Nullable<global::Quire.Controls.Plots.Presenter.TraceSymbol> SelectedTracesSymbol { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
}
