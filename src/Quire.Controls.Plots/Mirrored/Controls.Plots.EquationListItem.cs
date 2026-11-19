using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class EquationListItem : UserControl, IEquationListItem, IComponentConnector
{
    private IPlotEquation _equation;
    internal StackPanel mainStackPanel;
    internal ContentControl EquationPlaceHolder;
    internal StackPanel TraceStackPanel;
    private bool _contentLoaded;

    public bool ContainsLabels => false;

    public bool IsItemActive => IsKeyboardFocusWithin;

    public IPlotEquation Equation
    {
        get => _equation;
        set
        {
            _equation = value;
            EquationPlaceHolder.Content = _equation;
        }
    }

    public event Action<EquationListItem> ItemChange;

    public EquationListItem()
    {
        InitializeComponent();
    }

    private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            FireItemChange();
        }
    }

    protected void FireItemChange()
    {
        if (ItemChange != null)
        {
            ItemChange(this);
        }
    }

    public void AddTrace(IPlotTrace trace)
    {
        AddTrace(trace, TraceStackPanel.Children.Count);
    }

    public void AddTrace(IPlotTrace trace, int index)
    {
        PlotTrace plotTrace = new PlotTrace
        {
            PlottingPoints = new PointCollection
            {
                new Point(-10.0, 0.0),
                new Point(0.0, 0.0),
                new Point(10.0, 0.0)
            },
            Height = 10.0,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0.7, -2.3, 0.7, -2.3)
        };
        Binding binding = new Binding
        {
            Path = new PropertyPath(PlotTrace.RenderColorProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.RenderColorProperty, binding);
        Binding binding2 = new Binding
        {
            Path = new PropertyPath(PlotTrace.SymbolProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.SymbolProperty, binding2);
        MultiBinding multiBinding = new MultiBinding
        {
            Converter = TraceLineWeightConverter.Instance
        };
        multiBinding.Bindings.Add(new Binding
        {
            Path = new PropertyPath(PlotTrace.ImageRendererProperty),
            Source = trace
        });
        multiBinding.Bindings.Add(new Binding
        {
            Path = new PropertyPath(PlotTrace.LineWeightProperty),
            Source = trace
        });
        BindingOperations.SetBinding(plotTrace, PlotTrace.LineWeightProperty, multiBinding);
        Binding binding3 = new Binding
        {
            Path = new PropertyPath(PlotTrace.LineStyleProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.LineStyleProperty, binding3);
        Binding binding4 = new Binding
        {
            Path = new PropertyPath(PlotTrace.SymbolWeightProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.SymbolWeightProperty, binding4);
        Binding binding5 = new Binding
        {
            Path = new PropertyPath(PlotTrace.SymbolFrequencyProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.SymbolFrequencyProperty, binding5);
        Binding binding6 = new Binding
        {
            Path = new PropertyPath(PlotTrace.IsSelectedProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, PlotTrace.IsSelectedProperty, binding6);
        Binding binding7 = new Binding
        {
            Path = new PropertyPath(UIElement.VisibilityProperty),
            Source = trace
        };
        BindingOperations.SetBinding(plotTrace, UIElement.VisibilityProperty, binding7);
        TraceStackPanel.Children.Insert(index, plotTrace);
    }

    public void RemoveTrace(int index)
    {
        TraceStackPanel.Children.RemoveAt(index);
    }

    public void ClearTraces()
    {
        TraceStackPanel.Children.Clear();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/equationlistitem.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                ((EquationListItem)target).IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
                break;
            case 2:
                mainStackPanel = (StackPanel)target;
                break;
            case 3:
                EquationPlaceHolder = (ContentControl)target;
                break;
            case 4:
                TraceStackPanel = (StackPanel)target;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
