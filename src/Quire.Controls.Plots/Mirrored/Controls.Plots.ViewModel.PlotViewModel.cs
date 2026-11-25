#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Undo;

namespace Quire.Controls.Plots.ViewModel;

internal partial class PlotViewModel
{
    private readonly global::Quire.Controls.Plots.Presenter.IPlotModel _model;

    public static readonly global::System.Windows.Input.RoutedCommand ActivateCommand;

    public static readonly global::System.Windows.Input.RoutedCommand ShowAxisExpressionsCommand;

    public PlotViewModel(global::Quire.Controls.Plots.Presenter.IPlotModel model, global::Quire.Controls.Plots.Presenter.IValueToScreen horizontalValueToScreen, global::Quire.Controls.Plots.Presenter.IValueToScreen verticalValueToScreen) { }

    private void MarkersPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ActiveMarker")
        {
            if (Markers.ActiveMarker != null)
            {
                SelectedTraces = null;
            }
            else
            {
                SetValuesForStyleProperties();
            }
        }
    }

    private void SetValuesForStyleProperties()
    {
        if (Markers != null && Markers.ActiveMarker != null)
        {
            SetValuesForActiveMarker();
        }
        else if (SelectedTraces.IsNullOrEmpty())
        {
            SetNullValuesForSelectedTraceProperties();
        }
        else
        {
            SetValuesForSelectedTraceProperties();
        }
    }

    private void ShowAxisExpressionsCommandExecute(object arg)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(arg is bool, null, Array.Empty<object>()))
        {
            return;
        }
        bool oldState = ShowAxisExpressions;
        CommandBuffer.Execute(new UndoableCommand(
            "Set ShowAxisExpressions",
            () =>
            {
                ShowAxisExpressions = GetNewValue(IsActive, ShowAxisExpressions, arg);
                return true;
            },
            () =>
            {
                ShowAxisExpressions = oldState;
            }));
    }

    public void UpdateFromModel()
    {
        if (Markers != null)
        {
            Markers.UpdateFromModel();
        }
    }

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

    private void Activate()
    {
        IsActive = true;
        EventHandler<EventArgs> activated = Activated;
        if (activated != null)
        {
            activated(this, new EventArgs());
        }
    }

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

    private void SelectTrace(IPlotTrace trace)
    {
        trace.SelectTrace();
        trace.TraceChanged += TraceChanged;
    }

    private void TraceChanged(IPlotTrace obj)
    {
        SetValuesForSelectedTraceProperties();
    }

    private void DeselectTrace(IPlotTrace trace)
    {
        trace.DeselectTrace();
        trace.TraceChanged -= TraceChanged;
    }

    private void SetValuesForSelectedTraceProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetValuesForActiveMarker()
    {
        SelectedColor = Markers.ActiveMarker.Color;
        SelectedLineStyle = ((global::Quire.Controls.Plots.Model.IAxisMarker)Markers.ActiveMarker).LineStyle;
        SelectedLineWeight = ((global::Quire.Controls.Plots.Model.IAxisMarker)Markers.ActiveMarker).LineWeight;
        SelectedTracesType = null;
        SelectedTracesSymbol = null;
    }

    private void SetNullValuesForSelectedTraceProperties()
    {
        SelectedColor = null;
        SelectedLineStyle = null;
        SelectedLineWeight = null;
        SelectedTracesType = null;
        SelectedTracesSymbol = null;
    }

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

    public global::System.Nullable<global::Quire.Controls.Plots.Presenter.LineStyle> SelectedLineStyle
    {
        get
        {
            return _selectedLineStyle;
        }
        set
        {
            if (_selectedLineStyle != value)
            {
                _selectedLineStyle = value;
                OnPropertyChanged("SelectedLineStyle");
            }
        }
    }

    private global::System.Nullable<double> _selectedLineWeight;

    public global::System.Nullable<double> SelectedLineWeight
    {
        get
        {
            return _selectedLineWeight;
        }
        set
        {
            if (_selectedLineWeight != value)
            {
                _selectedLineWeight = value;
                OnPropertyChanged("SelectedLineWeight");
            }
        }
    }

    private global::System.Nullable<global::Quire.Calculation.Core.Trace2DResults.TraceType> _selectedTracesType;

    public global::System.Nullable<global::Quire.Calculation.Core.Trace2DResults.TraceType> SelectedTracesType
    {
        get
        {
            return _selectedTracesType;
        }
        set
        {
            if (_selectedTracesType != value)
            {
                _selectedTracesType = value;
                OnPropertyChanged("SelectedTracesType");
            }
        }
    }

    private global::System.Nullable<global::Quire.Controls.Plots.Presenter.TraceSymbol> _selectedTracesSymbol;

    public global::System.Nullable<global::Quire.Controls.Plots.Presenter.TraceSymbol> SelectedTracesSymbol
    {
        get
        {
            return _selectedTracesSymbol;
        }
        set
        {
            if (_selectedTracesSymbol != value)
            {
                _selectedTracesSymbol = value;
                OnPropertyChanged("SelectedTracesSymbol");
            }
        }
    }
}
