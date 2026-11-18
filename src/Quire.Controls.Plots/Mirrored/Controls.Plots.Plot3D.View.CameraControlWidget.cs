#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Fluent;
using Quire.Controls.Core;
using Quire.Controls.Plots.Plot3D.ViewModel;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class CameraControlWidget : VisualAdorner, IComponentConnector
{
    internal StackPanel _buttonsPanel;
    internal global::Fluent.Button Spin;
    internal global::Fluent.Button Pan;
    internal global::Fluent.Button Zoom;
    internal global::Fluent.Button ResetView;
    private bool _contentLoaded;

    public CameraControlWidget(UIElement adornedElement, PlotViewModel3D viewModel)
        : base(adornedElement)
    {
        DataContext = viewModel;
        InitializeComponent();
        IsHitTestVisible = true;
        _buttonsPanel.DataContext = viewModel;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _buttonsPanel.Measure(constraint);
        return _buttonsPanel.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Size desiredSize = _buttonsPanel.DesiredSize;
        Point point = new Point(-desiredSize.Width * 2.0, 0.0);
        _buttonsPanel.Arrange(new Rect(point, desiredSize));
        return finalSize;
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
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plot3d/view/cameracontrolwidget.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }


}
