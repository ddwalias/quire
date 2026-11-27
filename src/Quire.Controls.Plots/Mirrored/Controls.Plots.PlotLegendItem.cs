#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;
using Quire.Controls.Text;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Wpf;
using Quire.Xml;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;

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

    public PlotLegendItem(IPlotTrace trace)
    {
        InitializeComponent();
        InputBindings.Add(new MouseBinding(PlotControlBase.ActivateCommand, new MouseGesture(MouseAction.LeftClick)));
        CommandBindings.Add(new CommandBinding(PlotControlBase.ActivateCommand, OnActivateExecute));
        SourceTrace = trace;
        WpfUtils.ExecuteOnLoad(this, () =>
        {
            if (LegendLabel == null)
            {
                return;
            }
            LegendLabel.PreviewKeyDown += (object obj, KeyEventArgs e2) =>
            {
                if (e2.Key == Key.Return)
                {
                    e2.Handled = true;
                }
            };
            LegendLabel.LostFocus += OnLegendLabelLostFocus;
        });
    }

    private void OnActivateExecute(object sender, ExecutedRoutedEventArgs e)
    {
        if (LegendLabel == null)
        {
            return;
        }
        WpfUtils.ExecuteOnLoad(LegendLabel, () =>
        {
            LegendLabel.Focus();
        });
        e.Handled = true;
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

    public void AddTrace()
    {
        IPlotTrace sourceTrace = SourceTrace;
        PlotTrace plotTrace = new PlotTrace
        {
            PlottingPoints = MiniTraceDefaultPoints,
            Height = 10.0,
            Width = MiniTraceWidth,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = MiniTraceDefaultMargin
        };
        plotTrace.TraceClicked += (IPlotTrace plotTrace2) =>
        {
            if (LegendLabel != null && LegendLabel.IsEnabled)
            {
                LegendLabel.Focus();
            }
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.RenderColorProperty, new Binding
        {
            Path = new PropertyPath(PlotTrace.RenderColorProperty),
            Source = sourceTrace
        });
        BindingOperations.SetBinding(plotTrace, PlotTrace.SymbolProperty, new Binding
        {
            Path = new PropertyPath(PlotTrace.SymbolProperty),
            Source = sourceTrace
        });
        MultiBinding multiBinding = new MultiBinding
        {
            Converter = TraceLineWeightConverter.Instance
        };
        multiBinding.Bindings.Add(new Binding
        {
            Path = new PropertyPath(PlotTrace.ImageRendererProperty),
            Source = sourceTrace
        });
        multiBinding.Bindings.Add(new Binding
        {
            Path = new PropertyPath(PlotTrace.LineWeightProperty),
            Source = sourceTrace
        });
        BindingOperations.SetBinding(plotTrace, PlotTrace.LineWeightProperty, multiBinding);
        BindingOperations.SetBinding(plotTrace, PlotTrace.LineStyleProperty, new Binding
        {
            Path = new PropertyPath(PlotTrace.LineStyleProperty),
            Source = sourceTrace
        });
        BindingOperations.SetBinding(plotTrace, PlotTrace.SymbolWeightProperty, new Binding
        {
            Path = new PropertyPath(PlotTrace.SymbolWeightProperty),
            Source = sourceTrace
        });
        BindingOperations.SetBinding(plotTrace, PlotTrace.SymbolFrequencyProperty, new Binding
        {
            Path = new PropertyPath(PlotTrace.SymbolFrequencyProperty),
            Source = sourceTrace
        });
        BindingOperations.SetBinding(plotTrace, PlotTrace.IsSelectedProperty, new Binding
        {
            Path = new PropertyPath(PlotTrace.IsSelectedProperty),
            Source = sourceTrace
        });
        if (MiniTraceLegend != null)
        {
            MiniTraceLegend.Content = plotTrace;
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

    public XmlNode GetRegionXmlNode(IS11NProvider s11NProvider)
    {
        XmlDocument xmlDocument = new XmlDocument();
        string namespaceURI = XmlWorksheetData.McdxNameSpaceManager.LookupNamespace(string.Empty);
        XmlElement xmlElement = xmlDocument.CreateElement("legend-label-item", namespaceURI);
        if (LegendLabel == null || !LegendLabel.IsDirty)
        {
            return xmlElement;
        }
        if (s11NProvider is IRegionType regionType)
        {
            s11NProvider.Helper.AssignNextRegionId(LegendLabel);
            regionType.regionid = s11NProvider.Helper.GetRegionId(LegendLabel).ToString();
        }
        if (LegendLabel.GetRegionPersistentData(s11NProvider) is textType textType2)
        {
            if (textType2.Any != null)
            {
                XmlNode newChild = xmlDocument.ImportNode(textType2.Any, deep: true);
                xmlElement.AppendChild(newChild);
            }
            if (textType2.istextblock)
            {
                xmlElement.SetAttribute("is-text-block", XmlConvert.ToString(textType2.istextblock));
            }
            if (!string.IsNullOrEmpty(textType2.itemidref))
            {
                xmlElement.SetAttribute("item-idref", textType2.itemidref);
            }
            textType2.PreSerialize();
        }
        return xmlElement;
    }

    public void SetRegionXmlNode(IDeS11NProvider d11NProvider, XmlNode legendLabelItemNode)
    {
        if (LegendLabel == null || legendLabelItemNode == null)
        {
            return;
        }
        textType textType2 = new textType();
        if (legendLabelItemNode.FirstChild is XmlElement any)
        {
            textType2.Any = any;
        }
        XmlAttribute xmlAttribute = legendLabelItemNode.Attributes?["is-text-block"];
        if (xmlAttribute != null)
        {
            textType2.istextblock = XmlConvert.ToBoolean(xmlAttribute.Value);
        }
        XmlAttribute xmlAttribute2 = legendLabelItemNode.Attributes?["item-idref"];
        if (xmlAttribute2 != null)
        {
            textType2.itemidref = xmlAttribute2.Value;
        }
        if (textType2.Any != null)
        {
            LegendLabel.SetRegionPersistentData(d11NProvider, textType2);
        }
    }

    // XAML-generated members placed after methods to match Mathcad's member ordinals for closures
    internal StackPanel LegendItemStackPanel;
    internal ContentControl MiniTraceLegend;
    internal ContentPresenter LegendItemLabelPlaceholder;
    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plotlegenditem.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                LegendItemStackPanel = (StackPanel)target;
                break;
            case 2:
                MiniTraceLegend = (ContentControl)target;
                break;
            case 3:
                LegendItemLabelPlaceholder = (ContentPresenter)target;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
