#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Quire;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotTrace
{
    public void SelectTrace()
    {
        IsSelected = true;
    }

    public void DeselectTrace()
    {
        IsSelected = false;
    }

    public void fireTraceClicked()
    {
        if (TraceClicked != null)
        {
            TraceClicked(this);
        }
        IsSelected = true;
    }

    public void ResetIsTypeDefault()
    {
        if (RequestResetIsTypeDefault != null)
        {
            RequestResetIsTypeDefault();
        }
    }

    private void FireTypeChanged()
    {
        if (TypeChanged != null)
        {
            TypeChanged();
        }
        FireTraceChanged();
    }

    private void FireErrorChanged()
    {
        if (ErrorChanged != null)
        {
            ErrorChanged(this);
        }
    }

    private void FireCalculatingStatusChangedEvent()
    {
        if (CalculatingStatusChangedEvent != null)
        {
            CalculatingStatusChangedEvent();
        }
    }

    private void FireTraceChanged()
    {
        Action<IPlotTrace> handler = TraceChanged;
        if (handler != null)
        {
            handler(this);
        }
    }

    public bool ChangeSymbol(TraceSymbol value)
    {
        Symbol = value;
        if (RestrictedToDensityPointsOrSolidLine)
        {
            LineStyle = Symbol == Presenter.TraceSymbol.none ? LineStyle.Solid : LineStyle.None;
        }
        return Symbol == value;
    }

    public bool ChangeLineStyle(LineStyle value)
    {
        LineStyle = value;
        if (RestrictedToDensityPointsOrSolidLine)
        {
            Symbol = LineStyle == LineStyle.None ? Presenter.TraceSymbol.point : Presenter.TraceSymbol.none;
        }
        return LineStyle == value;
    }

    public void SetEquation(AxesType axis, IPlotEquation equation)
    {
        _plotEquations[(int)AxesTypeToIndex.Index(axis)] = equation;
    }

    public IPlotEquation GetEquation(AxesIndex axesIndex)
    {
        return _plotEquations[(int)axesIndex];
    }

    private static double GetMinGap(List<double> values)
    {
        values.Sort();
        double num = double.PositiveInfinity;
        for (int i = 1; i < values.Count; i++)
        {
            double num2 = values[i] - values[i - 1];
            if (num2 > 0.0)
            {
                num = Math.Min(num, num2);
            }
        }
        return num;
    }

    public TraceStyleState? DoStyleChangeCommand()
    {
        if (_previousStyleStateSaved)
        {
            _previousStyleStateSaved = false;
            return _savedStyleForSmallTrace;
        }
        return null;
    }

    public void UndoStyleChangeCommand(TraceStyleState? doData)
    {
        if (doData.HasValue)
        {
            _savedStyleForSmallTrace = doData.Value;
            _previousStyleStateSaved = true;
        }
    }

    private void ConformStyleToPointCount()
    {
        if (RestrictedToDensityPointsOrSolidLine || ForcedToDensityVisualization)
        {
            if (!_previousStyleStateSaved)
            {
                _savedStyleForSmallTrace = new TraceStyleState(Symbol, LineStyle, LineWeight);
                if (LineStyle == LineStyle.None)
                {
                    Symbol = Presenter.TraceSymbol.point;
                }
                else
                {
                    LineStyle = LineStyle.Solid;
                    Symbol = Presenter.TraceSymbol.none;
                }
                _previousStyleStateSaved = true;
            }
        }
        else if (_previousStyleStateSaved)
        {
            LineStyle = _savedStyleForSmallTrace.LineStyle;
            Symbol = _savedStyleForSmallTrace.TraceSymbol;
            _previousStyleStateSaved = false;
        }
    }

    public bool ResultValid
    {
        get
        {
            return (bool)GetValue(ResultValidProperty);
        }
        set
        {
            SetValue(ResultValidProperty, value);
        }
    }

    public BitmapSource ImageRenderer
    {
        get
        {
            return (BitmapSource)GetValue(ImageRendererProperty);
        }
        set
        {
            SetValue(ImageRendererProperty, value);
        }
    }

    public bool UseTraceInvalidStyle
    {
        get
        {
            return (bool)GetValue(UseTraceInvalidStyleProperty);
        }
        set
        {
            SetValue(UseTraceInvalidStyleProperty, value);
        }
    }

    public bool HasPoints
    {
        get
        {
            return (bool)GetValue(HasPointsProperty);
        }
        set
        {
            SetValue(HasPointsProperty, value);
        }
    }

    public int NumberOfPoints
    {
        get
        {
            return (int)GetValue(NumberOfPointsProperty);
        }
        set
        {
            SetValue(NumberOfPointsProperty, value);
        }
    }

    public double SymbolWeight
    {
        get
        {
            return (double)GetValue(SymbolWeightProperty);
        }
        set
        {
            SetValue(SymbolWeightProperty, value);
        }
    }

    public double SymbolFrequency
    {
        get
        {
            return (double)GetValue(SymbolFrequencyProperty);
        }
        set
        {
            SetValue(SymbolFrequencyProperty, value);
        }
    }

    public double LineWeight
    {
        get
        {
            return (double)GetValue(LineWeightProperty);
        }
        set
        {
            SetValue(LineWeightProperty, value);
        }
    }

    public string Label
    {
        get
        {
            return (string)GetValue(LabelProperty);
        }
        set
        {
            SetValue(LabelProperty, value);
        }
    }

    public bool IsNumOfPointsMutable
    {
        get
        {
            return (bool)GetValue(IsNumOfPointsMutableProperty);
        }
        set
        {
            SetValue(IsNumOfPointsMutableProperty, value);
        }
    }

    public bool IsCommitted
    {
        get
        {
            return (bool)GetValue(IsCommittedProperty);
        }
        set
        {
            SetValue(IsCommittedProperty, value);
        }
    }

    public bool IsSelected
    {
        get
        {
            return (bool)GetValue(IsSelectedProperty);
        }
        private set
        {
            SetValue(IsSelectedProperty, value);
        }
    }

    public bool HasEquationError
    {
        get
        {
            return (bool)GetValue(HasEquationErrorProperty);
        }
        set
        {
            SetValue(HasEquationErrorProperty, value);
        }
    }

    public Color RenderColor
    {
        get
        {
            return (Color)GetValue(RenderColorProperty);
        }
        set
        {
            SetColorStyle(value);
        }
    }

    public TraceType Type
    {
        get
        {
            return (TraceType)GetValue(TypeProperty);
        }
        set
        {
            SetValue(TypeProperty, value);
        }
    }

    public SynchronizationStatus SynchronizationStatus
    {
        get
        {
            return (SynchronizationStatus)GetValue(SynchronizationStatusProperty);
        }
        set
        {
            DispatchSetValue(SynchronizationStatusProperty, value);
        }
    }

    public CalculatingStatus CalculatingStatus
    {
        get
        {
            return (CalculatingStatus)GetValue(CalculatingStatusProperty);
        }
        set
        {
            DispatchSetValue(CalculatingStatusProperty, value);
        }
    }

    public IPlotAxisView XAxis { get; private set; }

    public IPlotAxisView YAxis { get; private set; }

    public bool IsResizing
    {
        get
        {
            return _isResizing;
        }
        set
        {
            if (value)
            {
                _sizeBeforeResizing = new Size(ActualWidth, ActualHeight);
            }

            _isResizing = value;
            if (!_isResizing)
            {
                InvalidateVisual();
                ViewPortRect = ComputeViewPortRect();
            }
        }
    }

    public string TraceLineStyle
    {
        get
        {
            return LineStyle.ToString();
        }
    }

    public string TraceSymbol
    {
        get
        {
            return Symbol.ToString();
        }
    }

    public bool RestrictedToDensityPointsOrSolidLine
    {
        get
        {
            if (_viewModel == null)
            {
                return false;
            }

            return _viewModel.RestrictedToDensityPointsOrSolidLine;
        }
    }

    public bool AreDensityPointsSupported
    {
        get
        {
            if (_viewModel == null)
            {
                return false;
            }

            return _viewModel.AreDensityPointsSupported;
        }
    }

    public bool ForcedToDensityVisualization
    {
        get
        {
            if (_viewModel == null)
            {
                return false;
            }

            return _viewModel.ForcedToDensityVisualization;
        }
    }

    private bool IsInPrinting
    {
        get
        {
            return (bool)GetValue(IsInPrintingProperty);
        }
    }

    public double XMinGap
    {
        get
        {
            return (double)GetValue(XMinGapProperty);
        }
        private set
        {
            SetValue(_xMinGapPropertyKey, value);
        }
    }

    public double YMinGap
    {
        get
        {
            return (double)GetValue(YMinGapProperty);
        }
        private set
        {
            SetValue(_yMinGapPropertyKey, value);
        }
    }

    private static void OnImageRendererChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.OldValue == null || eventArgs.NewValue == null)
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private static void HandleUseTraceInvalidStylechanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlotTrace plotTrace && e.OldValue != e.NewValue && plotTrace.ViewModel != null)
        {
            plotTrace.ViewModel.ImageRendererViewModel.UseTraceInvalidStyle = (bool)e.NewValue;
        }
    }

    private static object NumberOfPointsCoerceValue(DependencyObject d, object baseValue)
    {
        PlotTrace plotTrace = (PlotTrace)(object)d;
        if (!plotTrace.IsNumOfPointsMutable)
        {
            return plotTrace.NumberOfPoints;
        }
        int num = (int)baseValue;
        num = ((num < 100) ? 100 : num);
        num = ((num > 3000) ? 3000 : num);
        return num;
    }

    private static void NumberOfPointsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlotTrace plotTrace && plotTrace.NumberOfPointsChanged != null && (int)e.OldValue != (int)e.NewValue)
        {
            plotTrace.NumberOfPointsChanged();
        }
    }

    private static void IsNumOfPointsMutableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlotTrace plotTrace && plotTrace.IsNumOfPointsMutableChanged != null && (bool)e.OldValue != (bool)e.NewValue)
        {
            plotTrace.IsNumOfPointsMutableChanged();
        }
    }

    private static object CoerceSymbol(DependencyObject d, object baseValue)
    {
        PlotTrace plotTrace = (PlotTrace)(object)d;
        if (plotTrace.LineStyle == LineStyle.None && (global::Quire.Controls.Plots.Presenter.TraceSymbol)baseValue == global::Quire.Controls.Plots.Presenter.TraceSymbol.none)
        {
            return plotTrace.Symbol;
        }
        return baseValue;
    }

    private static object CoerceLineStyle(DependencyObject d, object baseValue)
    {
        PlotTrace plotTrace = (PlotTrace)(object)d;
        if ((LineStyle)baseValue == LineStyle.None && (plotTrace.Symbol == global::Quire.Controls.Plots.Presenter.TraceSymbol.none || plotTrace.Type == TraceType.Stems))
        {
            return plotTrace.LineStyle;
        }
        return baseValue;
    }

    private static void TraceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlotTrace plotTrace)
        {
            plotTrace.FireTraceChanged();
        }
    }

    private static void TraceLineStyleChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        PlotTrace plotTrace = sender as PlotTrace;
        if (DebugTools.AssertNotNull(plotTrace, null).Passed)
        {
            plotTrace.FireTraceChanged();
        }
    }

    private static void TraceSymbolChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        PlotTrace plotTrace = sender as PlotTrace;
        if (DebugTools.AssertNotNull(plotTrace, null).Passed)
        {
            plotTrace.FireTraceChanged();
        }
    }

    private static void TypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PlotTrace plotTrace)
        {
            Panel.SetZIndex(plotTrace, (plotTrace.Type == TraceType.Lines) ? 1 : 0);
            plotTrace.FireTypeChanged();
            plotTrace._renderer = plotTrace._rendererFactory.GetRenderer(plotTrace.Type);
        }
    }
}
