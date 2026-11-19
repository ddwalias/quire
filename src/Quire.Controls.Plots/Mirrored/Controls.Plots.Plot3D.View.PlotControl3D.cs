#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;

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
}
