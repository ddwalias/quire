#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotGraphControl
{
    private readonly PlotType _plotType;

    private static readonly Type typeOfThis;

    public static readonly DependencyProperty selectedTraceProperty;

    private static readonly DependencyPropertyKey TraceCountByTypePropertyKey;

    public static readonly DependencyProperty TraceCountByTypeProperty;

    private static readonly DependencyPropertyKey TracePlaceByTypePropertyKey;

    public static readonly DependencyProperty TracePlaceByTypeProperty;

    private PlotGraphLinearAxisBase _xAxis;

    private PlotGraphLinearAxisBase _yAxis;

    private PlotGraphAxis _angularAxis;

    private PlotGraphAxis _radialAxis;

    public static readonly RoutedEvent ChangedEvent;

    public PlotTrace selectedTrace
    {
        get
        {
            return (PlotTrace)GetValue(selectedTraceProperty);
        }
        set
        {
            SetValue(selectedTraceProperty, value);
        }
    }

    internal PlotGraphLinearAxisBase XAxis
    {
        get
        {
            return _xAxis;
        }
        set
        {
            if (_xAxis != null)
            {
                _xAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _xAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _xAxis = value;
            if (_xAxis != null)
            {
                _xAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _xAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    internal PlotGraphLinearAxisBase YAxis
    {
        get
        {
            return _yAxis;
        }
        set
        {
            if (_yAxis != null)
            {
                _yAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _yAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _yAxis = value;
            if (_yAxis != null)
            {
                _yAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _yAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    internal PlotGraphAxis AngularAxis
    {
        get
        {
            return _angularAxis;
        }
        set
        {
            if (_angularAxis != null)
            {
                _angularAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _angularAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _angularAxis = value;
            if (_angularAxis != null)
            {
                _angularAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _angularAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    internal PlotGraphAxis RadialAxis
    {
        get
        {
            return _radialAxis;
        }
        set
        {
            if (_radialAxis != null)
            {
                _radialAxis.ShowGridLinesChanged -= OnAxisGridLinesChanged;
                _radialAxis.TickMarksChangedEvent -= OnAxisGridLinesChanged;
            }
            _radialAxis = value;
            if (_radialAxis != null)
            {
                _radialAxis.ShowGridLinesChanged += OnAxisGridLinesChanged;
                _radialAxis.TickMarksChangedEvent += OnAxisGridLinesChanged;
            }
            InvalidateVisual();
        }
    }

    public IEnumerable<PlotTrace> TracesUnsorted => Children.OfType<PlotTrace>();

    internal Action<PlotTrace> OnTraceRemoved { private get; set; }

    internal Action<PlotTrace> OnTraceAdded { private get; set; }

    public int Count => TracesUnsorted.Count();

    private static void SelectedTraceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }
        if (e.OldValue != null)
        {
            ((PlotTrace)e.OldValue).DeselectTrace();
        }
        if (e.NewValue != null)
        {
            ((PlotTrace)e.NewValue).SelectTrace();
        }
    }

    private static void SetTraceCountByType(UIElement element, int value)
    {
        element.SetValue(TraceCountByTypePropertyKey, value);
    }

    public static int GetTraceCountByType(UIElement element)
    {
        return (int)element.GetValue(TraceCountByTypeProperty);
    }

    private static void SetTracePlaceByType(UIElement element, int value)
    {
        element.SetValue(TracePlaceByTypePropertyKey, value);
    }

    public static int GetTracePlaceByType(UIElement element)
    {
        return (int)element.GetValue(TracePlaceByTypeProperty);
    }

    public PlotGraphControl(PlotType type)
    {
        _plotType = type;
    }

    private void OnAxisGridLinesChanged()
    {
        InvalidateVisual();
    }

    static PlotGraphControl()
    {
        typeOfThis = typeof(PlotGraphControl);
        selectedTraceProperty = DependencyProperty.Register("selectedTrace", typeof(PlotTrace), typeOfThis, new PropertyMetadata(SelectedTraceChanged));
        TraceCountByTypePropertyKey = DependencyProperty.RegisterAttachedReadOnly("TraceCountByType", typeof(int), typeOfThis, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        TraceCountByTypeProperty = TraceCountByTypePropertyKey.DependencyProperty;
        TracePlaceByTypePropertyKey = DependencyProperty.RegisterAttachedReadOnly("TracePlaceByType", typeof(int), typeOfThis, new FrameworkPropertyMetadata(0));
        TracePlaceByTypeProperty = TracePlaceByTypePropertyKey.DependencyProperty;
        ChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(typeOfThis);
    }

    internal PlotTrace AddTrace()
    {
        PlotTrace plotTrace = new PlotTrace();
        Children.Insert(TracesUnsorted.Count(), plotTrace);
        return plotTrace;
    }

    protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
    {
        ObservableUIElementCollection observableUIElementCollection = new ObservableUIElementCollection(this, logicalParent);
        observableUIElementCollection.RangeChanged += UIElementCollectionChanged;
        return observableUIElementCollection;
    }

    private void UIElementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                ElementsAdded(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                ElementsRemoved(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                ElementsRemoved(e.OldItems);
                ElementsAdded(e.NewItems);
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                ElementsRemoved(e.OldItems);
                ElementsAdded(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
        }
        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    private void ElementsAdded(IList uiElements)
    {
        if (uiElements != null)
        {
            foreach (UIElement uiElement in uiElements)
            {
                ElementAdded(uiElement);
            }
        }
    }

    private void ElementAdded(UIElement element)
    {
        if (element is PlotTrace)
        {
            TraceAdded(element as PlotTrace);
        }
    }

    private void ElementsRemoved(IList uiElements)
    {
        if (uiElements != null)
        {
            foreach (UIElement uiElement in uiElements)
            {
                ElementRemoved(uiElement);
            }
        }
    }

    private void ElementRemoved(UIElement element)
    {
        if (element is PlotTrace)
        {
            TraceRemoved(element as PlotTrace);
        }
    }

    internal void SetIsResizing(bool value)
    {
        foreach (PlotTrace item in TracesUnsorted)
        {
            item.IsResizing = value;
        }
    }

    private void TraceRemoved(PlotTrace plotTrace)
    {
        plotTrace.GotFocus -= PlotTraceMouseDown;
        plotTrace.LostFocus -= LostKeyboard;
        plotTrace.TypeChanged -= UpdateTraceCountAndPlace;
        UpdateTraceCountAndPlace();
        OnTraceRemoved(plotTrace);
    }

    private void TraceAdded(PlotTrace newTrace)
    {
        newTrace.GotFocus += PlotTraceMouseDown;
        newTrace.LostFocus += LostKeyboard;
        newTrace.TypeChanged += UpdateTraceCountAndPlace;
        UpdateTraceCountAndPlace();
        OnTraceAdded(newTrace);
    }

    private void LostKeyboard(object sender, RoutedEventArgs e)
    {
        selectedTrace = null;
    }

    private void PlotTraceMouseDown(object sender, RoutedEventArgs e)
    {
        if (e.Source is PlotTrace plotTrace)
        {
            selectedTrace = plotTrace;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_plotType == PlotType.Polar)
        {
            availableSize = new Size(Math.Min(availableSize.Height, availableSize.Width), Math.Min(availableSize.Height, availableSize.Width));
        }
        foreach (PlotTrace item in TracesUnsorted)
        {
            item.Measure(availableSize);
        }
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (PlotTrace item in TracesUnsorted)
        {
            item.Arrange(new Rect(finalSize));
        }
        return finalSize;
    }

    public void RemoveTrace(IPlotTrace trace)
    {
        Children.Remove(trace as UIElement);
    }

    private void UpdateTraceCountAndPlace()
    {
        foreach (PlotTrace item in TracesUnsorted)
        {
            PlotTrace trace = item;
            List<PlotTrace> list = TracesUnsorted.Where((PlotTrace t) => t.Type == trace.Type).ToList();
            SetTracePlaceByType(item, list.IndexOf(item));
            SetTraceCountByType(item, list.Count);
        }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        switch (_plotType)
        {
            case PlotType.Polar:
                DrawPolarGridlines(drawingContext);
                break;
            case PlotType.XY:
                DrawXYGridlines(drawingContext);
                break;
        }
    }

    private void DrawXYGridlines(DrawingContext drawingContext)
    {
        if (XAxis == null || YAxis == null)
        {
            return;
        }
        GuidelineSet guidelines = new GuidelineSet();
        CollectAxisTickPositions(XAxis, guidelines, isXAxis: true, out var xMajorTickPositions, out var xMinorTickPositions);
        CollectAxisTickPositions(YAxis, guidelines, isXAxis: false, out var yMajorTickPositions, out var yMinorTickPositions);
        drawingContext.PushGuidelineSet(guidelines);
        DrawGridlinesIfPresent(drawingContext, xMajorTickPositions, XAxis, isVertical: true);
        DrawGridlinesIfPresent(drawingContext, yMajorTickPositions, YAxis, isVertical: false);
        DrawGridlinesIfPresent(drawingContext, xMinorTickPositions, XAxis, isVertical: true);
        DrawGridlinesIfPresent(drawingContext, yMinorTickPositions, YAxis, isVertical: false);
        drawingContext.Pop();
    }

    private void CollectAxisTickPositions(PlotGraphLinearAxisBase axis, GuidelineSet guidelines, bool isXAxis, out List<double> majorTickPositions, out List<double> minorTickPositions)
    {
        majorTickPositions = null;
        minorTickPositions = null;
        if (axis == null || !axis.ShowGridLines)
        {
            return;
        }
        DoubleCollection guidelinesCollection = (isXAxis ? guidelines.GuidelinesX : guidelines.GuidelinesY);
        TickScale majorTickmarks = axis.GetMajorTickmaks();
        majorTickPositions = new List<double>(majorTickmarks.Count);
        for (int i = 0; i < majorTickmarks.Count; i++)
        {
            if (majorTickmarks.ShouldDrawTick(i))
            {
                double tickPosition = axis.GetTickPositionByTickNumber(i);
                majorTickPositions.Add(tickPosition);
                guidelinesCollection.Add(tickPosition + 0.25);
            }
        }
        TickScale minorTickmarks = axis.GetMinorTickmarks();
        if (minorTickmarks != null)
        {
            minorTickPositions = new List<double>(minorTickmarks.Count);
            for (int i = 0; i < minorTickmarks.Count; i++)
            {
                if (minorTickmarks.ShouldDrawTick(i))
                {
                    double? minorTickPosition = axis.GetMinorTickPositionByTickNumber(i);
                    if (minorTickPosition.HasValue)
                    {
                        minorTickPositions.Add(minorTickPosition.Value);
                        guidelinesCollection.Add(minorTickPosition.Value + 0.25);
                    }
                }
            }
        }
    }

    private void DrawGridlinesIfPresent(DrawingContext drawingContext, List<double> tickPositions, PlotGraphLinearAxisBase axis, bool isVertical)
    {
        if (tickPositions != null && tickPositions.Count > 0)
        {
            Pen gridPen = new Pen(axis.GridLineBrush, 0.5);
            DrawLinearAxisGridlines(drawingContext, tickPositions, gridPen, isVertical);
        }
    }

    private void DrawLinearAxisGridlines(DrawingContext drawingContext, List<double> tickPositions, Pen gridPen, bool isVertical)
    {
        foreach (double tickPosition in tickPositions)
        {
            if (isVertical)
            {
                drawingContext.DrawLine(gridPen, new Point(tickPosition, 0.0), new Point(tickPosition, ActualHeight));
            }
            else
            {
                drawingContext.DrawLine(gridPen, new Point(0.0, tickPosition), new Point(ActualWidth, tickPosition));
            }
        }
    }

    private void DrawPolarGridlines(DrawingContext drawingContext)
    {
        if (AngularAxis == null || RadialAxis == null)
        {
            return;
        }
        GuidelineSet guidelines = new GuidelineSet();
        double halfSize = Math.Min(ActualHeight, ActualWidth) / 2.0;
        double outerRadius = halfSize * 0.8;
        Point center = new Point(halfSize, halfSize);
        guidelines.GuidelinesX.Add(halfSize + 0.25);
        guidelines.GuidelinesY.Add(halfSize + 0.25);
        List<Point> endpoints = null;
        if (AngularAxis.ShowGridLines && AngularAxis.TickMarksValues != null)
        {
            endpoints = new List<Point>(AngularAxis.TickMarks - 1);
            for (int i = 0; i < AngularAxis.TickMarks - 1; i++)
            {
                double tickMarkValue = AngularAxis.TickMarksValues[i];
                double angle = tickMarkValue / 180.0 * Math.PI;
                double x = halfSize + outerRadius * Math.Cos(angle);
                double y = halfSize - outerRadius * Math.Sin(angle);
                endpoints.Add(new Point(x, y));
                if (tickMarkValue % 90.0 == 0.0)
                {
                    guidelines.GuidelinesX.Add(x + 0.25);
                    guidelines.GuidelinesY.Add(y + 0.25);
                }
            }
        }
        List<double> radii = null;
        if (RadialAxis.ShowGridLines && RadialAxis.TickMarks > 1)
        {
            TickScale majorTickmarks = RadialAxis.GetMajorTickmaks();
            double maxTickPosition = majorTickmarks.TickMarkPosition(RadialAxis.TickMarks) - 1.0;
            radii = new List<double>(majorTickmarks.Count);
            for (int i = 0; i < majorTickmarks.Count; i++)
            {
                if (majorTickmarks.ShouldDrawTick(i))
                {
                    double normalizedPosition = majorTickmarks.TickMarkPosition(i) / maxTickPosition;
                    double radius = outerRadius * normalizedPosition;
                    if (radius > 0.0)
                    {
                        radii.Add(radius);
                    }
                }
            }
        }
        drawingContext.PushGuidelineSet(guidelines);
        if (endpoints != null)
        {
            Pen gridPen = new Pen(AngularAxis.GridLineBrush, 0.5);
            DrawRadialGridlines(drawingContext, gridPen, center, endpoints);
        }
        if (radii != null)
        {
            Pen gridPen = new Pen(RadialAxis.GridLineBrush, 0.5);
            DrawConcentricGridlines(drawingContext, gridPen, center, radii);
        }
        drawingContext.Pop();
    }

    private void DrawRadialGridlines(DrawingContext drawingContext, Pen gridPen, Point center, List<Point> endpoints)
    {
        foreach (Point endpoint in endpoints)
        {
            drawingContext.DrawLine(gridPen, center, endpoint);
        }
    }

    private void DrawConcentricGridlines(DrawingContext drawingContext, Pen gridPen, Point center, List<double> radii)
    {
        foreach (double radius in radii)
        {
            drawingContext.DrawEllipse(null, gridPen, center, radius, radius);
        }
    }
}
