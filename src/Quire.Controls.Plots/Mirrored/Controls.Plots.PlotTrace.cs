#nullable disable
#pragma warning disable

using System;
using System.Windows.Markup;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Quire;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
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

    public Quire.Controls.Plots.ViewModel.TraceViewModel ViewModel
    {
        get
        {
            return _viewModel;
        }
        set
        {
            if (_viewModel == value)
            {
                return;
            }
            _viewModel = value;
            DataContext = value;
            Rect val = default;
            if (_copiedImageRenderer.Bitmap != null)
            {
                ViewModel.ImageRendererViewModel.SetCachedBitmap(_copiedImageRenderer.Bitmap);
                val = _copiedImageRenderer.ViewPortRect;
                _copiedImageRenderer = default;
                DebugTools.DevAssert(val != default(Rect), null);
            }
            ViewPortRect = ((val != default(Rect)) ? val : ComputeViewPortRect());
        }
    }

    public Rect ViewPortRect
    {
        get
        {
            if (ViewModel != null)
            {
                return ViewModel.ImageRendererViewModel.ViewPortRect;
            }
            return _copiedImageRenderer.ViewPortRect;
        }
        set
        {
            if (value == default(Rect))
            {
                return;
            }
            if (ViewModel == null)
            {
                _copiedImageRenderer.ViewPortRect = value;
            }
            else if (DoubleComparer.DefaultDoubleComparer.Compare(value, ViewModel.ImageRendererViewModel.ViewPortRect) != 0)
            {
                ViewModel.ImageRendererViewModel.ViewPortRect = value;
                if (ViewModel.IsUsingDensityVisualization)
                {
                    InvalidateVisual();
                }
            }
        }
    }

    public Rect ClippingRect
    {
        get
        {
            if (DebugTools.DevAssert(XAxis != null && YAxis != null, null).Failed)
            {
                return Rect.Empty;
            }
            if (IsResizing)
            {
                return _clippingRect;
            }
            _clippingRect = ComputeViewPortRect();
            if (XAxis is PlotGraphLinearAxis && YAxis is PlotGraphLinearAxis)
            {
                Vector val = new Vector(2.0 * SymbolWeight, 2.0 * SymbolWeight);
                _clippingRect.Union(_clippingRect.TopLeft - val);
                _clippingRect.Union(_clippingRect.BottomRight + val);
            }
            return _clippingRect;
        }
    }

    public Geometry ClippingGeometry
    {
        get
        {
            if (DebugTools.DevAssert(XAxis != null && YAxis != null, null).Failed)
            {
                return null;
            }
            if (ViewModel.PlotType != PlotType.Polar)
            {
                return new RectangleGeometry(ClippingRect);
            }
            return new EllipseGeometry(ClippingRect);
        }
    }

    public TraceSymbol Symbol
    {
        get
        {
            return (TraceSymbol)((DependencyObject)this).GetValue(SymbolProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(SymbolProperty, (object)value);
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            return (LineStyle)((DependencyObject)this).GetValue(LineStyleProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(LineStyleProperty, (object)value);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<Point> PlottingPoints
    {
        get
        {
            return (IList<Point>)((DependencyObject)this).GetValue(PlottingPointsProperty);
        }
        set
        {
            DispatchSetValue(PlottingPointsProperty, value);
            ConformStyleToPointCount();
        }
    }

    public PlotGraphAxis.Point2ScreenHelper XPoint2ScreenHelper
    {
        get
        {
            if (XAxis == null)
            {
                return new PlotGraphAxis.Point2ScreenHelper(ActualWidth);
            }
            return XAxis.Helper;
        }
    }

    public PlotGraphAxis.Point2ScreenHelper YPoint2ScreenHelper
    {
        get
        {
            if (YAxis == null)
            {
                return new PlotGraphAxis.Point2ScreenHelper(ActualHeight);
            }
            return YAxis.Helper;
        }
    }

    EngineError IPlotTrace.PlotCalculatableError
    {
        get
        {
            return _plotCalculatableError;
        }
        set
        {
            _plotCalculatableError = value;
            IPlotEquation[] plotEquations = _plotEquations;
            foreach (IPlotEquation plotEquation in plotEquations)
            {
                if (plotEquation != null)
                {
                    plotEquation.PlotCalculatableError = _plotCalculatableError;
                }
            }
            FireErrorChanged();
        }
    }

    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            if (_color != value)
            {
                _color = value;
                RenderColor = value;
                FireTraceChanged();
            }
        }
    }

    private bool IsCopyAsImage => DocumentAttachedProperties.GetCopyAsImageSettings(this).HasValue;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BitmapSource CopiedImageBitmap
    {
        get
        {
            if (ViewModel == null || !ViewModel.ImageRendererViewModel.HasCachedBitmap)
            {
                return _copiedImageRenderer.Bitmap;
            }
            return ViewModel.ImageRendererViewModel.RenderingBitmap;
        }
        set
        {
            if (ViewModel != null)
            {
                ViewModel.ImageRendererViewModel.SetCachedBitmap(value);
            }
            else
            {
                _copiedImageRenderer.Bitmap = value;
            }
        }
    }

    private Rect ComputeViewPortRect()
    {
        Rect result = default;
        try
        {
            result = new Rect(new Point(XAxis.Point2Screen(XAxis.ClipBounds.First), YAxis.Point2Screen(YAxis.ClipBounds.First)), new Point(XAxis.Point2Screen(XAxis.ClipBounds.Second), YAxis.Point2Screen(YAxis.ClipBounds.Second)));
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message, null);
        }
        return result;
    }

    public void MoveFocusToEquation(AxesIndex id)
    {
        ((UIElement)_plotEquations[(int)id]).Focus();
    }

    public void SetAxis(IPlotAxisView xAxis, IPlotAxisView yAxis)
    {
        if (XAxis != null)
        {
            XAxis.PropertyChanged -= InvalidateVisualOnAxisPropertiesChanged;
            XAxis.SizeChanged -= InitializeViewPortBoxOnAxisSizeChanged;
        }
        XAxis = xAxis;
        if (XAxis != null)
        {
            XAxis.PropertyChanged += InvalidateVisualOnAxisPropertiesChanged;
            XAxis.SizeChanged += InitializeViewPortBoxOnAxisSizeChanged;
        }
        if (YAxis != null)
        {
            YAxis.PropertyChanged -= InvalidateVisualOnAxisPropertiesChanged;
            YAxis.SizeChanged -= InitializeViewPortBoxOnAxisSizeChanged;
        }
        YAxis = yAxis;
        if (YAxis != null)
        {
            YAxis.PropertyChanged += InvalidateVisualOnAxisPropertiesChanged;
            YAxis.SizeChanged += InitializeViewPortBoxOnAxisSizeChanged;
        }
    }

    private void InitializeViewPortBoxOnAxisSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateViewPortRectOnAxisPropertyChanged();
    }

    private void InvalidateVisualOnAxisPropertiesChanged(object sender, PropertyChangedEventArgs e)
    {
        bool flag = e.PropertyName == "EndValue" || e.PropertyName == "StartValue";
        if (flag)
        {
            UpdateViewPortRectOnAxisPropertyChanged();
        }
        if (!ViewModel.IsUsingDensityVisualization && ((e.PropertyName == "Offset" || e.PropertyName == "PositionTickmrak") | flag))
        {
            InvalidateVisual();
        }
    }

    private void UpdateViewPortRectOnAxisPropertyChanged()
    {
        if (!IsResizing)
        {
            ViewPortRect = ComputeViewPortRect();
        }
    }

    private static void CalculatingStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        PlotTrace plotTrace = d as PlotTrace;
        if (plotTrace == null)
        {
            return;
        }
        plotTrace.FireCalculatingStatusChangedEvent();
    }

    private void DispatchSetValue(DependencyProperty dp, object value)
    {
        if (((DispatcherObject)this).Dispatcher.CheckAccess())
        {
            ((DependencyObject)this).SetValue(dp, value);
            return;
        }
        Action<DependencyProperty, object> action = ((DependencyObject)this).SetValue;
        ((DispatcherObject)this).Dispatcher.Invoke((DispatcherPriority)9, (Delegate)action, (object)dp, new object[1] { value });
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (IsResizing)
        {
            if (RenderTransform is ScaleTransform scaleTransform)
            {
                scaleTransform.ScaleX = ActualWidth / _sizeBeforeResizing.Width;
                scaleTransform.ScaleY = ActualHeight / _sizeBeforeResizing.Height;
            }
            else
            {
                RenderTransform = new ScaleTransform(ActualWidth / _sizeBeforeResizing.Width, ActualHeight / _sizeBeforeResizing.Height);
            }
            ClipToBounds = false;
        }
        else
        {
            RenderTransform = Transform.Identity;
            ClipToBounds = true;
        }
    }

    private void SetColorStyle(Color color)
    {
        Style style = new Style(_typeOfThis, (Style)Resources["BasicPlotTraceStyle"]);
        style.Setters.Add(new Setter(RenderColorProperty, color));
        Style = style;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plottrace.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
