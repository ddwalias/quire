#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Controls.Plots.Plot3D.ViewModel;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotTrace3D
{
    static PlotTrace3D()
    {
        _emptyPoint3DCollection = new Point3DCollection();
    }

    public double BaseLineThickness
    {
        get
        {
            return _baseLineThickness;
        }
        set
        {
            _baseLineThickness = value;
            UpdateLineWeight();
        }
    }

    private bool ShouldShowSymbols => Symbol != TraceSymbol.none;

    private bool ShouldShowLines => LineStyle != LineStyle.None;

    internal IPlotTraceViewModel3D ViewModel { get; private set; }

    private double RenderThickness => Thickness * BaseLineThickness;

    public Color RenderColor => ViewModel.RenderColor;

    internal double Thickness
    {
        get
        {
            return ViewModel.LineWeight;
        }
        set
        {
            ViewModel.LineWeight = value;
            UpdateLineWeight();
        }
    }

    internal Color? SurfaceFill
    {
        get
        {
            return ViewModel.SurfaceFill;
        }
        set
        {
            ViewModel.SurfaceFill = value;
        }
    }

    internal LineStyle LineStyle
    {
        get
        {
            return ViewModel.LineStyle;
        }
        set
        {
            DebugTools.DevAssert(value == LineStyle.None || value == LineStyle.Solid, null);
            ViewModel.LineStyle = value;
            UpdateLineStyle();
        }
    }

    internal TraceSymbol Symbol
    {
        get
        {
            return ViewModel.Symbol;
        }
        set
        {
            DebugTools.DevAssert(value == TraceSymbol.none || value == TraceSymbol.point, null);
            ViewModel.Symbol = value;
            UpdateSymbol();
        }
    }

    internal ModelVisual3D Visual
    {
        get
        {
            return _visual;
        }
        private set
        {
            if (value != _visual)
            {
                Visual3D visual = _visual;
                _visual = value;
                FireVisualChanged(visual, _visual);
            }
        }
    }

    internal event Action<PlotTrace3D, Visual3D, Visual3D> VisualChanged;

    private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case "Symbol":
                UpdateSymbol();
                break;
            case "LineStyle":
                UpdateLineStyle();
                break;
            case "LineWeight":
                UpdateLineWeight();
                break;
            case "VisualData":
                UpdateVisualData();
                break;
            case "RenderColor":
                UpdateColor();
                break;
            case "SurfaceFill":
                UpdateSurfaceFill();
                break;
        }
    }

    private void UpdateLineWeight()
    {
        if (_linesTraceVisual != null)
        {
            _linesTraceVisual.Thickness = RenderThickness;
        }
        if (_symbolTraceVisual != null)
        {
            _symbolTraceVisual.Thickness = RenderThickness;
        }
    }

    private void UpdateColor()
    {
        if (_linesTraceVisual != null)
        {
            _linesTraceVisual.Color = RenderColor;
        }
        if (_symbolTraceVisual != null)
        {
            _symbolTraceVisual.Color = RenderColor;
        }
    }
}
