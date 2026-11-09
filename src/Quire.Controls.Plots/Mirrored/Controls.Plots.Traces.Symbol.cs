#nullable disable
#pragma warning disable

using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public abstract partial class Symbol
{
    public virtual bool IsFilled => false;

    protected static Geometry CreatePathGeometryPointCollection(PointCollection pointCollection)
    {
        PolyLineSegment polyLineSegment = new PolyLineSegment { Points = pointCollection };
        PathSegmentCollection segments = new PathSegmentCollection { polyLineSegment };
        PathFigure figure = new PathFigure(polyLineSegment.Points[0], segments, true);
        // The figure is added through the Figures initializer, so the geometry stays on the stack for the return.
        return new PathGeometry(new PathFigureCollection()) { Figures = { figure } };
    }

    protected Symbol()
    {
    }
}
