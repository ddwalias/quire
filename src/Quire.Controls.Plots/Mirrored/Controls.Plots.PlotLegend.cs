#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
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

    internal int Columns
    {
        get
        {
            return PlotLegendGrid.Columns;
        }
        set
        {
            if (PlotLegendGrid.Columns == value)
            {
                return;
            }
            PlotLegendGrid.Columns = value;
            PlotLegendGrid.InvalidateMeasure();
        }
    }

    internal double MaxItemWidth { get; set; }

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

    double IPlotLegend.LegendWidth
    {
        get
        {
            return ActualWidth;
        }
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    internal void SetLegendWidthWhenLayoutReady()
    {
        if (!_pendingLegendWidth.HasValue)
        {
            return;
        }
        double value = _pendingLegendWidth.Value;
        _pendingLegendWidth = null;
        if (value != 0.0)
        {
            Width = value;
        }
    }

    private void PlotLegendSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.PreviousSize.Width.Equals(e.NewSize.Width))
        {
            return;
        }
        _legendWidthChanged?.Invoke(e.NewSize.Width);
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

    private static void PlotLegendWidthChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        PlotLegend plotLegend = dObj as PlotLegend;
        double newWidth = (double)eArgs.NewValue;
        if (plotLegend == null || !(plotLegend.MaxItemWidth > 0.0))
        {
            return;
        }
        plotLegend.Columns = plotLegend.PlotLegendGrid.AdjustNumberOfColumnsToWidth(newWidth);
        plotLegend.SetMinMaxWidth();
    }

    private void SetMinMaxWidth()
    {
        SetMinMaxWidth(MaxItemWidth, 0.0);
    }

    private void SetMinMaxWidth(double previousWidth, double newWidth)
    {
        if (PlotLegendListEnumerable.IsNullOrEmpty())
        {
            return;
        }
        if ((previousWidth == MaxItemWidth && newWidth < MaxItemWidth) || MaxItemWidth == 0.0)
        {
            MaxItemWidth = PlotLegendGrid.Children.OfType<PlotLegendItem>().Max((PlotLegendItem item) => item.ActualWidth);
            MaxWidth = (double)Math.Min(PlotLegendItemsCount, 3) * (MaxItemWidth + 10.0);
            PlotLegendGrid.MaxWidth = MaxWidth;
            if (double.IsNaN(Width))
            {
                Width = (double)Columns * (MaxItemWidth + 10.0);
            }
            MinWidth = MaxItemWidth + 10.0;
        }
        if (newWidth > MaxItemWidth)
        {
            MaxItemWidth = newWidth;
            if (double.IsNaN(Width))
            {
                Width = (double)Columns * (MaxItemWidth + 10.0);
            }
            MaxWidth = (double)Math.Min(PlotLegendItemsCount, 3) * (MaxItemWidth + 10.0);
            PlotLegendGrid.MaxWidth = MaxWidth;
            MinWidth = MaxItemWidth + 10.0;
        }
        MinHeight = PlotLegendGrid.ActualHeight;
    }

    public IPlotLegendItem AddNewItemToPlotLegend(int index, IPlotTrace trace)
    {
        RichTextTitle richTextTitle = new RichTextTitle(string.Format(DefaultTraceLabelFormat, index + 1))
        {
            BorderThickness = new Thickness(0.0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        if (TextStyleChangedHandler != null)
        {
            richTextTitle.TextStyleChangedHandler = TextStyleChangedHandler;
        }
        if (InitialTextFormat != null)
        {
            richTextTitle.ApplyInitialTextFormat(InitialTextFormat);
        }
        PlotLegendItem plotLegendItem = new PlotLegendItem(trace)
        {
            LegendLabel = richTextTitle
        };
        FrameworkElement frameworkElement = plotLegendItem;
        if (frameworkElement != null)
        {
            frameworkElement.SizeChanged += PlotLegendItemSizeChanged;
        }
        plotLegendItem.AddTrace();
        PlotLegendGrid.Children.Insert(index, plotLegendItem);
        UpdateDefaultLegendLabelsAfterIndex(index);
        SetMinMaxWidth();
        return plotLegendItem;
    }

    private void PlotLegendItemSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetMinMaxWidth(e.PreviousSize.Width, e.NewSize.Width);
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

    private IPlotLegendItem FindByTrace(IPlotTrace trace)
    {
        return PlotLegendListEnumerable.FirstOrDefault((IPlotLegendItem legendItem) => legendItem.SourceTrace == trace);
    }

    private void UpdateDefaultLegendLabelsAfterIndex(int startIndex)
    {
        List<IPlotLegendItem> list = PlotLegendListEnumerable.ToList();
        for (int num = startIndex + 1; num < PlotLegendItemsCount; num++)
        {
            if (list[num] is PlotLegendItem { LegendLabel: not null } plotLegendItem && !(plotLegendItem.LegendLabel.GetText() != plotLegendItem.LegendLabel.InitialTitleText))
            {
                string text = string.Format(DefaultTraceLabelFormat, num + 1);
                if (plotLegendItem.LegendLabel.GetText() != text)
                {
                    plotLegendItem.LegendLabel.SetText(text);
                    plotLegendItem.LegendLabel.InitialTitleText = text;
                }
            }
        }
    }

    public IEnumerable<IPlotLegendItem> PlotLegendListEnumerable => PlotLegendGrid.Children.Cast<IPlotLegendItem>();

    public int PlotLegendItemsCount => PlotLegendGrid.Children.Count;

    internal PlotLegendContents PlotLegendGrid;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plotlegend.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            PlotLegendGrid = (PlotLegendContents)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
