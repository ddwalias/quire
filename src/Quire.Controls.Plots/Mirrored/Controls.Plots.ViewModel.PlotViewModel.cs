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
    public static readonly global::System.Windows.Input.RoutedCommand ActivateCommand;

    public PlotViewModel(IPlotModel model, IValueToScreen horizontalValueToScreen, IValueToScreen verticalValueToScreen)
    {
        _model = model;
        _model.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);
        if (_model.Type == PlotType.XY)
        {
            Markers = new MarkersViewModel(_model.rangeAxis, _model.PrimaryResultAxis, horizontalValueToScreen, verticalValueToScreen);
            Markers.MarkerDeleted += (sender, e) => Activate();
            Markers.PropertyChanged += MarkersPropertyChanged;
            Markers.ActiveMarkerStyleChanged += SetValuesForActiveMarker;
            CommandBindings.AddRange(Markers.CommandBindings);
        }
        CreateCommandBinding(ShowAxisExpressionsCommand, ShowAxisExpressionsCommandExecute);
    }

    private readonly global::Quire.Controls.Plots.Presenter.IPlotModel _model;

    public static readonly global::System.Windows.Input.RoutedCommand ShowAxisExpressionsCommand;

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

    private List<IPlotTrace> _selectedTraces = new List<IPlotTrace>();

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

    private void SetValuesForSelectedTraceProperties()
    {
        SelectedColor = GetAggregatedTracesProperty(trace => trace.Color);
        SelectedLineStyle = GetAggregatedTracesProperty(trace => trace.LineStyle);
        SelectedLineWeight = GetAggregatedTracesProperty(trace => trace.LineWeight);
        SelectedTracesType = GetAggregatedTracesProperty(trace => trace.Type);
        SelectedTracesSymbol = GetAggregatedTracesProperty(trace => trace.Symbol);
    }

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

    private Nullable<T> GetAggregatedTracesProperty<T>(Func<IPlotTrace, T> propertySelector)
        where T : struct
    {
        return GetAggregatedProperty<IPlotTrace, T, T?>(SelectedTraces, propertySelector, value => value);
    }

    private static TResult GetAggregatedProperty<TSource, T, TResult>(IEnumerable<TSource> source, Func<TSource, T> propertySelector, Func<T, TResult> resultCorverter)
    {
        if (propertySelector == null)
        {
            throw new ArgumentNullException("propertySelector");
        }
        if (resultCorverter == null)
        {
            throw new ArgumentNullException("resultCorverter");
        }
        if (source == null)
        {
            return default(TResult);
        }
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return default(TResult);
            }
            T value = propertySelector(enumerator.Current);
            while (enumerator.MoveNext())
            {
                if (!value.Equals(propertySelector(enumerator.Current)))
                {
                    return default(TResult);
                }
            }
            return resultCorverter(value);
        }
    }

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
