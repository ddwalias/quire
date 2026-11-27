#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;
using Quire.Wpf;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots;

public partial class PlotControl
{

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plotcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }

    protected bool IsResizing => ResizableElementAttachedProperties.GetIsResizing(this);

    public override Size PlotGraphSize
    {
        get
        {
            return _model.GraphSize;
        }
    }

    public override PlotType PlotType
    {
        get
        {
            return (PlotType)GetValue(PlotTypeProperty);
        }
        protected set
        {
            SetValue(PlotTypeProperty, value);
        }
    }

    private EquationList RightMostEquationList
    {
        get
        {
            switch (_resultLegendPositionIndex)
            {
            case 0:
                return _rangeEquations;
            case 1:
                return _resultEquations;
            case 2:
            case 4:
                switch (_rangeLegendPositionIndex)
                {
                case 0:
                case 1:
                case 2:
                case 3:
                    return _rangeEquations;
                case 4:
                case 5:
                    return _resultEquations;
                }
                break;
            case 3:
            case 5:
                switch (_rangeLegendPositionIndex)
                {
                case 2:
                case 3:
                    return _rangeEquations;
                case 0:
                case 1:
                case 4:
                case 5:
                    return _resultEquations;
                }
                break;
            }
            throw new Exception("not valid y legend position");
        }
    }

    private EquationList LeftMostEquationList
    {
        get
        {
            if (RightMostEquationList == _rangeEquations)
            {
                return _resultEquations;
            }
            return _rangeEquations;
        }
    }

    public bool IsInPrinting => (bool)GetValue(IsInPrintingProperty);

    public double PlotXMinGap
    {
        get
        {
            return (double)GetValue(PlotXMinGapProperty);
        }
        private set
        {
            SetValue(PlotXMinGapPropertyKey, value);
        }
    }

    public double PlotYMinGap
    {
        get
        {
            return (double)GetValue(PlotYMinGapProperty);
        }
        private set
        {
            SetValue(PlotYMinGapPropertyKey, value);
        }
    }

    public double XLegendWidthForXamlSerialization
    {
        get
        {
            return RangeAxis.LegendActualWidth;
        }
        set
        {
            RangeAxis.LegendActualWidth = value;
        }
    }

    public double YLegendWidthForXamlSerialization
    {
        get
        {
            return PrimaryResultAxis.LegendActualWidth;
        }
        set
        {
            PrimaryResultAxis.LegendActualWidth = value;
        }
    }

    public bool SnapToGrid
    {
        get
        {
            return (bool)GetValue(SnapToGridProperty);
        }
        set
        {
            SetValue(SnapToGridProperty, value);
        }
    }

    internal PlotItemPlaceholder[] XLegendsPositions => _rangeLegendsPositions;

    internal PlotItemPlaceholder[] YLegendsPositions => _resultLegendsPositions;

    internal PlotItemPlaceholder[] PlotTitlePositions => _plotTitlePositions;

    internal PlotItemPlaceholder[] PlotLegendPositions => _plotLegendPositions;

    internal PlotItemPlaceholder XAxisTitlePlaceholder { get; set; }

    internal PlotItemPlaceholder YAxisTitlePlaceholder { get; set; }

    public int TraceCount => Graph.Count;

    public string TraceLineStyle
    {
        get
        {
            return (string)GetValue(TraceLineStyleProperty);
        }
        set
        {
            SetValue(TraceLineStyleProperty, value);
        }
    }

    public double? TraceLineWeight
    {
        get
        {
            return (double?)GetValue(TraceLineWeightProperty);
        }
        set
        {
            SetValue(TraceLineWeightProperty, value);
        }
    }

    public string TraceSymbol
    {
        get
        {
            return (string)GetValue(TraceSymbolProperty);
        }
        set
        {
            SetValue(TraceSymbolProperty, value);
        }
    }

    public global::Quire.Calculation.Core.Trace2DResults.TraceType? SelectedTraceType
    {
        get
        {
            return (global::Quire.Calculation.Core.Trace2DResults.TraceType?)GetValue(SelectedTraceTypeProperty);
        }
        set
        {
            SetValue(SelectedTraceTypeProperty, value);
        }
    }

    private List<IPlotTrace> SelectedTracesClone => new List<IPlotTrace>(SelectedTraces);

    public int XLegendPositionIndex
    {
        get
        {
            return _rangeLegendPositionIndex;
        }
        set
        {
            if (XLegendsPositions != null)
            {
                PlotItemPlaceholder plotItemPlaceholderAtOldPosition = XLegendsPositions[_rangeLegendPositionIndex];
                PlotItemPlaceholder plotItemPlaceholderAtNewPosition = XLegendsPositions[value];
                _rangeEquations.Move(plotItemPlaceholderAtNewPosition, plotItemPlaceholderAtOldPosition);
            }
            _rangeLegendPositionIndex = value;
            FireXLegendPositionsIndexChanged();
        }
    }

    public int YLegendPositionIndex
    {
        get
        {
            return _resultLegendPositionIndex;
        }
        set
        {
            if (YLegendsPositions != null)
            {
                PlotItemPlaceholder plotItemPlaceholderAtOldPosition = YLegendsPositions[_resultLegendPositionIndex];
                PlotItemPlaceholder plotItemPlaceholderAtNewPosition = YLegendsPositions[value];
                _resultEquations.Move(plotItemPlaceholderAtNewPosition, plotItemPlaceholderAtOldPosition);
            }
            _resultLegendPositionIndex = value;
            FireYLegendPositionsIndexChanged();
        }
    }

    public int PlotTitlePositionIndex
    {
        get
        {
            return _plotTitlePositionIndex;
        }
        set
        {
            if (_plotTitle == null)
            {
                return;
            }
            if (PlotTitlePositions != null)
            {
                PlotItemPlaceholder plotItemPlaceholderAtOldPosition = PlotTitlePositions[_plotTitlePositionIndex];
                PlotItemPlaceholder plotItemPlaceholderAtNewPosition = PlotTitlePositions[value];
                _plotTitle.Move(plotItemPlaceholderAtNewPosition, plotItemPlaceholderAtOldPosition);
            }
            _plotTitlePositionIndex = value;
            FirePlotTitlePositionsIndexChanged();
        }
    }

    public bool IsCalculating
    {
        get
        {
            return (bool)GetValue(IsCalculatingProperty);
        }
        set
        {
            SetValue(IsCalculatingProperty, value);
        }
    }

    public IPlotAxisView RangeAxis => _rangeAxis;

    public IPlotAxisView PrimaryResultAxis => _primaryResultAxis;

    public IPlotLegend PlotLegend => _plotLegend;

    public List<IPlotTrace> SelectedTraces
    {
        get
        {
            return (List<IPlotTrace>)GetValue(SelectedTracesProperty);
        }
        set
        {
            SetValue(SelectedTracesProperty, value);
        }
    }

    public PlotViewType Type { get; set; }

    internal double GraphMinHeight
    {
        get
        {
            double num = GridCellSize.Height * 4.0;
            if (_plotLegend == null || !PlotLegend.ViewModel.ShowPlotLegend)
            {
                return num;
            }
            int plotLegendPositionIndex = _plotLegendPositionIndex;
            if (plotLegendPositionIndex < 1 || plotLegendPositionIndex > 4)
            {
                return num;
            }
            return Math.Max(num, _plotLegend.MinHeight + _plotLegend.Margin.Top + _plotLegend.Margin.Bottom);
        }
    }

    internal IPlotModel Model => _model;

    internal Quire.Controls.Plots.ViewModel.PlotViewModel ViewModel => _viewModel;

    public override int NumberOfPointsMin => 100;

    public override int NumberOfPointsMax => 3000;

    private static void HandleMinWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((PlotControl)obj).HandleMinWidthChanged();
    }

    private static void HandleMinHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((PlotControl)obj).HandleMinHeightChanged();
    }

    private void HandleMinHeightChanged()
    {
        if (Height < MinHeight)
        {
            Height = MinHeight;
        }
    }

    private void HandleMinWidthChanged()
    {
        if (Width < MinWidth)
        {
            Width = MinWidth;
        }
    }

    private void GraphTraceRemoved(PlotTrace trace)
    {
        trace.ErrorChanged -= TraceErrorChanged;
        trace.CalculatingStatusChangedEvent -= HandleTraceCalculatingStatusChanged;
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(PlotTrace.XMinGapProperty, trace, XMinGapPropertyChanged);
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(PlotTrace.YMinGapProperty, trace, YMinGapPropertyChanged);
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(PlotTrace.HasEquationErrorProperty, trace, HasEquationErrorPropertyChanged);
        trace.SetAxis(null, null);
        AttachedProperties.ClearContainedInCollapsedWhiteboard(trace);
    }

    private void GraphTraceAdded(PlotTrace newTrace)
    {
        AttachedProperties.SetContainedInCollapsedWhiteboard(newTrace, AttachedProperties.GetContainedInCollapsedWhiteboard(this));
    }

    private static void IsResizingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (!(sender is PlotControl plotControl))
        {
            DebugTools.DevAssert(false, null);
        }
        else
        {
            plotControl.Graph.SetIsResizing(plotControl.IsResizing);
        }
    }

    private static void HandleShowAxisExpressionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is PlotControl plotControl))
        {
            return;
        }
        if (!(bool)e.NewValue)
        {
            if (!plotControl.IsActiveRegion)
            {
                plotControl.SwitchExpressionsVisibility(HiddenExpressionsVisibility);
            }
        }
        else
        {
            plotControl.SwitchExpressionsVisibility(Visibility.Visible);
        }
    }

    private static bool TraceCanIncrease(IPlotTrace trace)
    {
        if (trace.IsNumOfPointsMutable)
        {
            return trace.NumberOfPoints < 3000;
        }
        return false;
    }

    private static bool TraceCanDecrease(IPlotTrace trace)
    {
        if (trace.IsNumOfPointsMutable)
        {
            return trace.NumberOfPoints > 100;
        }
        return false;
    }

    private bool AnySelectedTrace(Func<IPlotTrace, bool> predicate)
    {
        if (SelectedTraces != null)
        {
            return SelectedTraces.Any(predicate);
        }
        return false;
    }

    private void RemovePlotItemAdorner()
    {
        if (_plotItemAdorner != null)
        {
            AdornerLayer.GetAdornerLayer(this).Remove(_plotItemAdorner);
            _plotItemAdorner = null;
        }
    }

    public static void SetIsDropTarget(UIElement element, bool value)
    {
        ((DependencyObject)element).SetValue(IsDropTargetProperty, value);
    }

    public static bool GetIsDropTarget(UIElement element)
    {
        return (bool)((DependencyObject)element).GetValue(IsDropTargetProperty);
    }

    private static void CommitAllEquationListWithoutEvent(EquationList equationList)
    {
        for (int i = 0; i < equationList.EquationCount; i++)
        {
            equationList[i].Commit(fireEvent: false);
        }
    }

    private void TogglePlotTitleCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _plotTitle != null;
    }

    private void AddRowCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        EquationCommands.InsertRow.Execute(null, ActivePlotEquation);
    }

    private void DeleteTraceCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        EquationList equationList = ActivePlotEquation == _rangeEquations.SelectedItem.Equation ? _rangeEquations : _resultEquations;
        equationList.RequestRemoveItem(equationList.SelectedItem, Direction.Right);
    }

    private void ChangeTraceTypeCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(e.Parameter is TraceType, null))
        {
            return;
        }
        TraceType traceType = (TraceType)e.Parameter;
        SetTraceType(traceType);
    }

    private void ChangeTraceSymbolCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(e.Parameter is TraceSymbol, null))
        {
            return;
        }
        TraceSymbol traceSymbol = (TraceSymbol)e.Parameter;
        SetTraceSymbol(traceSymbol);
    }

    private void ChangeTraceLineCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(e.Parameter is LineStyle, null))
        {
            return;
        }
        LineStyle lineStyle = (LineStyle)e.Parameter;
        SetTraceLineStyle(lineStyle);
    }

    private void ChangeTraceWeightCommandExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if ((AssertionE)DebugTools.DevAssert(e.Parameter is double, null) == AssertionE.Passed)
        {
            SetSelectedTracesLineWeight((double)e.Parameter);
        }
    }

    private void CanNavigateToYEquationList(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ((object)_rangeEquations).Equals((object)WpfUtils.GetFocusedAncestorOfTypeElement<EquationList>(this));
    }

    private void CanNavigateToXEquationList(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ((object)_resultEquations).Equals((object)WpfUtils.GetFocusedAncestorOfTypeElement<EquationList>(this));
    }

    private void NavigateToYEquationListStart(object sender, ExecutedRoutedEventArgs e)
    {
        _resultEquations.MoveToEquationStart(_resultEquations.EquationCount - 1);
    }

    private void NavigateToYEquationListEnd(object sender, ExecutedRoutedEventArgs e)
    {
        _resultEquations.MoveToEquationEnd(_resultEquations.EquationCount - 1);
    }

    private void NavigateToXEquationList(object sender, ExecutedRoutedEventArgs e)
    {
        _rangeEquations.MoveToEquationStart(0);
    }

    private void XEquationsSizeChanged(object sender, SizeChangedEventArgs e)
    {
        EquationListOrPlotLegendSizeChanged();
    }

    private void YEquationsSizeChanged(object sender, SizeChangedEventArgs e)
    {
        EquationListOrPlotLegendSizeChanged();
    }

    private void PlotLegendSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!(sender is PlotLegend))
        {
            return;
        }
        EquationListOrPlotLegendSizeChanged();
    }

    protected override void IsActiveRegionChanged(bool newValue)
    {
        base.IsActiveRegionChanged(newValue);
        if (newValue)
        {
            _isActivationPending = false;
            return;
        }
        if (!ShowAxisExpressions)
        {
            SwitchExpressionsVisibility(HiddenExpressionsVisibility);
        }
        SelectAllTraces();
    }

    protected override void IsSelectedChanged(bool newValue)
    {
        base.IsSelectedChanged(newValue);
        if (newValue)
        {
            SelectAllTraces();
        }
    }

    private void EquationListOrPlotLegendSizeChanged()
    {
        MakeSureXEquationsNotOutsidePlot(changePlotSize: true);
        MakeSurePlotLegendNotOutsidePlot(changePlotSize: true);
        if (_resultLegendsPositions != null && (_resultLegendPositionIndex == 0 || _resultLegendPositionIndex == 1))
        {
            MakeSureYEquationsNotOutsidePlot(_resultLegendsPositions[_resultLegendPositionIndex], _resultEquations);
        }
    }
}
