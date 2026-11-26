#nullable disable
#pragma warning disable

using Quire.Controls.Core;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotControl3D
{
    public CameraType3D PlotCameraType
    {
        get
        {
            return (CameraType3D)GetValue(PlotCameraTypeProperty);
        }
        set
        {
            SetValue(PlotCameraTypeProperty, value);
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

    public SolidColorBrush TraceColor
    {
        get
        {
            return (SolidColorBrush)GetValue(TraceColorProperty);
        }
        set
        {
            SetValue(TraceColorProperty, value);
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

    public SolidColorBrush TraceSurfaceFillColor
    {
        get
        {
            return (SolidColorBrush)GetValue(TraceSurfaceFillColorProperty);
        }
        set
        {
            SetValue(TraceSurfaceFillColorProperty, value);
        }
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
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plot3d/view/plotcontrol3d.xaml", UriKind.Relative);
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
        switch (connectionId)
        {
            case 1:
                _graphContainerFullSize = (Grid)target;
                break;
            case 2:
                _graphContainer = (Grid)target;
                break;
            case 3:
                _background = (Rectangle)target;
                break;
            case 4:
                _graph = (PlotGraph3D)target;
                break;
            case 5:
                _axesWidgetHost = (ContentControl)target;
                break;
            case 6:
                _graphContainerFullSizeComplementary = (Rectangle)target;
                _graphContainerFullSizeComplementary.SizeChanged += OnSizeChangedOfElementAffectingGraphWidth;
                break;
            case 7:
                _zEquationsList = (EquationList)target;
                break;
            case 8:
                _zEquationListComplementary = (Rectangle)target;
                _zEquationListComplementary.SizeChanged += OnSizeChangedOfElementAffectingGraphWidth;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
