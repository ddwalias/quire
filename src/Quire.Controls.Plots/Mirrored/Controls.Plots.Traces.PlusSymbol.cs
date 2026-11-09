#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class PlusSymbol
{
    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        double size = properties.SymbolWeight;
        return new PathGeometry(new PathFigureCollection())
        {
            Figures = { CreateVerticalLine(point, size), CreateGorizontalLine(point, size) },
        };
    }

    private static PathFigure CreateVerticalLine(Point point, double size)
    {
        PointCollection pointCollection = new PointCollection
        {
            new Point(point.X - size, point.Y),
            new Point(point.X + size, point.Y),
        };
        PolyLineSegment polyLineSegment = new PolyLineSegment { Points = pointCollection };
        PathSegmentCollection segments = new PathSegmentCollection { polyLineSegment };
        return new PathFigure(polyLineSegment.Points[0], segments, false);
    }

    private static PathFigure CreateGorizontalLine(Point point, double size)
    {
        PointCollection pointCollection = new PointCollection
        {
            new Point(point.X, point.Y + size),
            new Point(point.X, point.Y - size),
        };
        PolyLineSegment polyLineSegment = new PolyLineSegment { Points = pointCollection };
        PathSegmentCollection segments = new PathSegmentCollection { polyLineSegment };
        return new PathFigure(polyLineSegment.Points[0], segments, false);
    }

    public PlusSymbol()
    {
    }
}
