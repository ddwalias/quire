#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class TriangleSymbol
{
    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        double weight = properties.SymbolWeight;
        point = new Point(point.X + weight, point.Y + weight);
        return CreatePathGeometryPointCollection(new PointCollection
        {
            point,
            new Point(point.X - 2.0 * weight, point.Y),
            new Point(point.X - weight, point.Y - 2.0 * weight),
            point,
        });
    }

    public TriangleSymbol()
    {
    }
}
