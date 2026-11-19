#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Controls.Core;
using Quire.Controls.Plots.Plot3D.ViewModel;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotGraph3D
{
    public double BaseLineThickness
    {
        get
        {
            return (double)GetValue(BaseLineThicknessProperty);
        }
        set
        {
            SetValue(BaseLineThicknessProperty, value);
        }
    }

    public bool IsInPrinting => (bool)GetValue(IsInPrintingProperty);

    public bool CanPrint => !AttachedProperties.GetContainedInCollapsedWhiteboard(this);

    public int TraceViewsCount => _traceViews.Count;

    internal bool BoxVisibility
    {
        get
        {
            return _boundingBoxVisual.Color.ScA > 0f;
        }
        set
        {
            Color color = _boundingBoxVisual.Color;
            color.ScA = (value ? 1 : 0);
            _boundingBoxVisual.Color = color;
        }
    }

    internal IPlotAxis3D XAxis => _xAxis;

    internal IPlotAxis3D YAxis => _yAxis;

    internal IPlotAxis3D ZAxis => _zAxis;

    internal Viewport3D Viewport => _viewport;

    internal ProjectionCamera Camera => Viewport.Camera as ProjectionCamera;
}
