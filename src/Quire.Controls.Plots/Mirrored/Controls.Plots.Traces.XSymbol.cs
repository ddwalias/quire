#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class XSymbol
{
    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        double size = properties.SymbolWeight;
        return new PathGeometry(new PathFigureCollection())
        {
            Figures = { CreateXSymbolFirstAxe(point, size), CreateXSymbolSecondAxe(point, size) },
        };
    }

    private static PathFigure CreateXSymbolFirstAxe(Point point, double size)
    {
        PointCollection pointCollection = new PointCollection
        {
            new Point(point.X - size, point.Y - size),
            new Point(point.X + size, point.Y + size),
        };
        PolyLineSegment polyLineSegment = new PolyLineSegment { Points = pointCollection };
        PathSegmentCollection segments = new PathSegmentCollection { polyLineSegment };
        return new PathFigure(polyLineSegment.Points[0], segments, false);
    }

    private static PathFigure CreateXSymbolSecondAxe(Point point, double size)
    {
        PointCollection pointCollection = new PointCollection
        {
            new Point(point.X - size, point.Y + size),
            new Point(point.X + size, point.Y - size),
        };
        PolyLineSegment polyLineSegment = new PolyLineSegment { Points = pointCollection };
        PathSegmentCollection segments = new PathSegmentCollection { polyLineSegment };
        return new PathFigure(polyLineSegment.Points[0], segments, false);
    }

    public XSymbol()
    {
    }
}
