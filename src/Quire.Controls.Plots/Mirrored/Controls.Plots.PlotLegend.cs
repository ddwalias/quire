#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Controls.Text;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Plots;

public partial class PlotLegend
{
    private const string DefaultTraceLabelFormat = "Trace {0}";
    public static readonly DependencyProperty PlotTypeProperty;
    private Action<double> _legendWidthChanged;
    private double? _pendingLegendWidth;
    internal PlotLegendContents PlotLegendGrid;

    public PlotLegendViewModel ViewModel { get; }

    public ITextStyleChangedHandler TextStyleChangedHandler { get; set; }

    public TextFormat InitialTextFormat { get; set; }

    public PlotType PlotType
    {
        get
        {
            return (PlotType)GetValue(PlotTypeProperty);
        }
        set
        {
            SetValue(PlotTypeProperty, value);
        }
    }

    internal double MaxItemWidth { get; set; }

    public IEnumerable<IPlotLegendItem> PlotLegendListEnumerable => PlotLegendGrid.Children.Cast<IPlotLegendItem>();

    public int PlotLegendItemsCount => PlotLegendGrid.Children.Count;

    event Action<double> IPlotLegend.LegendWidthChanged
    {
        add
        {
            _legendWidthChanged = (Action<double>)Delegate.Combine(_legendWidthChanged, value);
        }
        remove
        {
            _legendWidthChanged = (Action<double>)Delegate.Remove(_legendWidthChanged, value);
        }
    }

    public PlotLegend(PlotType plotType, PlotLegendViewModel plotLegendViewModel)
    {
        InitializeComponent();
        PlotType = plotType;
        ViewModel = plotLegendViewModel;
        DataContext = ViewModel;
        SizeChanged += PlotLegendSizeChanged;
    }

    static PlotLegend()
    {
        PlotTypeProperty = DependencyProperty.Register("PlotType", typeof(PlotType), typeof(PlotLegend), new FrameworkPropertyMetadata(PlotType.XY));
        FrameworkElement.WidthProperty.OverrideMetadata(typeof(PlotLegend), new FrameworkPropertyMetadata(double.NaN, PlotLegendWidthChanged));
    }

    private void SetMinMaxWidth()
    {
        SetMinMaxWidth(MaxItemWidth, 0.0);
    }

    public IPlotLegendItem RemoveTraceFromPlotLegend(IPlotTrace trace)
    {
        DebugTools.AssertNotNullDoNotThrow(trace, "Can not remove PlotLegendItem due to null value");
        PlotLegendItem plotLegendItem = FindByTrace(trace) as PlotLegendItem;
        DebugTools.AssertNotNullDoNotThrow(plotLegendItem, "Trace does not exist in PlotLegend");
        int num = PlotLegendGrid.Children.IndexOf(plotLegendItem);
        FrameworkElement frameworkElement = plotLegendItem;
        if (frameworkElement != null)
        {
            frameworkElement.SizeChanged -= PlotLegendItemSizeChanged;
        }
        if (plotLegendItem.LegendLabel != null)
        {
            plotLegendItem.LegendLabel.TextStyleChangedHandler = null;
        }
        PlotLegendGrid.Children.Remove(plotLegendItem);
        PlotLegendGrid.UpdateLayout();
        UpdateDefaultLegendLabelsAfterIndex(num - 1);
        SetMinMaxWidth();
        return plotLegendItem;
    }
}
