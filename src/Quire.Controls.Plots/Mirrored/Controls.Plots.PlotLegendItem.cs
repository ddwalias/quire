#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Text;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Plots;

public partial class PlotLegendItem
{
    private RichTextTitle _legendLabel;
    private const double DefaultMiniTraceWidth = 36.7;
    private const double MiniTraceDefaultHeight = 10.0;
    private static readonly Thickness MiniTraceDefaultMargin = new Thickness(0.7, -2.3, 0.7, -2.3);
    private static readonly PointCollection MiniTraceDefaultPoints = new PointCollection
    {
        new Point(-10.0, 0.0),
        new Point(0.0, 0.0),
        new Point(10.0, 0.0)
    };
    public static readonly DependencyProperty MiniTraceWidthProperty = DependencyProperty.Register("MiniTraceWidth", typeof(double), typeof(PlotLegendItem), new PropertyMetadata(36.7));
    internal StackPanel LegendItemStackPanel;
    internal ContentControl MiniTraceLegend;
    internal ContentPresenter LegendItemLabelPlaceholder;
    private bool _contentLoaded;

    public IPlotTrace SourceTrace { get; set; }

    public RichTextTitle LegendLabel
    {
        get
        {
            return _legendLabel;
        }
        set
        {
            _legendLabel = value;
            LegendItemLabelPlaceholder.Content = _legendLabel;
        }
    }

    public double MiniTraceWidth
    {
        get
        {
            return (double)GetValue(MiniTraceWidthProperty);
        }
        set
        {
            SetValue(MiniTraceWidthProperty, value);
        }
    }

    private void OnLegendLabelLostFocus(object sender, RoutedEventArgs e)
    {
        if (LegendLabel == null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(LegendLabel.GetText()))
        {
            LegendLabel.ResetText();
        }
    }

    public textType GetDataObject(IS11NProvider s11NProvider)
    {
        return (textType)LegendLabel?.GetRegionPersistentData(s11NProvider);
    }

    public void SetDataObject(IDeS11NProvider d11NProvider, textType label)
    {
        LegendLabel.SetRegionPersistentData(d11NProvider, label);
    }
}
