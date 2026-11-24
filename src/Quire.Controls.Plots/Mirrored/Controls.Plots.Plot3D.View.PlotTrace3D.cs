#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Controls.Plots.Plot3D.Model;
using Quire.Controls.Plots.Plot3D.ViewModel;
using Quire.Controls.Plots.Presenter;
using Quire.Media3D;
using Quire.Media3D.Wires;

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

    private void FireVisualChanged(Visual3D oldVisual, Visual3D newVisual)
    {
        if (VisualChanged != null)
        {
            VisualChanged(this, oldVisual, newVisual);
        }
    }

    private void UpdateLineStyle()
    {
        if (Visual == null || _linesTraceVisual == null)
        {
            return;
        }
        DebugTools.DevAssert(LineStyle == LineStyle.Solid || LineStyle == LineStyle.None, "Only LineStyle.Solid is supported in PlotTrace3D");
        AddOrRemoveFromVisual(_linesTraceVisual, ShouldShowLines);
    }

    private void UpdateSymbol()
    {
        if (Visual == null || _symbolTraceVisual == null)
        {
            return;
        }
        DebugTools.DevAssert(Symbol == TraceSymbol.point || Symbol == TraceSymbol.none, "Only TraceSymbol.point is supported in PlotTrace3D");
        AddOrRemoveFromVisual(_symbolTraceVisual, ShouldShowSymbols);
    }

    private ModelVisual3D CreateSolidMesh()
    {
        Point3DCollection vertices = ViewModel.VisualData.Vertices;
        Int32Collection indices = ViewModel.VisualData.Indices;
        int num = indices.Count();
        DebugTools.DevAssert(num % 4 == 0, null);
        Int32Collection int32Collection = new Int32Collection(num * 6 / 4);
        for (int i = 0; i < indices.Count; i += 4)
        {
            int32Collection.Add(indices[i + 2]);
            int32Collection.Add(indices[i + 1]);
            int32Collection.Add(indices[i]);
            int32Collection.Add(indices[i]);
            int32Collection.Add(indices[i + 3]);
            int32Collection.Add(indices[i + 2]);
        }
        MeshGeometry3D geometry = new MeshGeometry3D
        {
            Positions = vertices,
            TriangleIndices = int32Collection
        };
        Material material = Tools3D.MaterialForColor(ViewModel.SurfaceFill);
        return new ModelVisual3D
        {
            Content = new GeometryModel3D(geometry, material)
            {
                BackMaterial = material
            }
        };
    }

    private void UpdateVisualData()
    {
        PlotTraceVisualData3D visualData = ViewModel.VisualData;
        Point3DCollection vertices = visualData.Vertices;
        WireBase wireBase;
        ModelVisual3D surfaceFillVisual = null;
        switch (visualData.TraceType)
        {
            case TraceType3D.Curve:
                wireBase = new WireLines
                {
                    Lines = vertices
                };
                break;
            case TraceType3D.Surface:
                wireBase = new WireRectMesh
                {
                    TriangleIndices = visualData.Indices,
                    Positions = vertices
                };
                surfaceFillVisual = CreateSolidMesh();
                break;
            default:
                wireBase = null;
                break;
        }
        if (wireBase != null)
        {
            wireBase.Thickness = RenderThickness;
            wireBase.Color = RenderColor;
        }
        _symbolTraceVisual = new WirePoints
        {
            Positions = visualData.FilteredVertices,
            Color = RenderColor,
            Thickness = RenderThickness
        };
        _linesTraceVisual = wireBase;
        _surfaceFillVisual = surfaceFillVisual;
        Visual = new ModelVisual3D();
        UpdateLineStyle();
        UpdateSymbol();
        UpdateSurfaceFill();
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

    private void AddOrRemoveFromVisual(Visual3D element, bool add)
    {
        if (add)
        {
            if (!Visual.Children.Contains(element))
            {
                Visual.Children.Add(element);
            }
        }
        else
        {
            Visual.Children.Remove(element);
        }
    }

}
