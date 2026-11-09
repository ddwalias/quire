#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class RhombusSymbol
{
    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        double weight = properties.SymbolWeight;
        point = new Point(point.X - weight, point.Y);
        return CreatePathGeometryPointCollection(new PointCollection
        {
            point,
            new Point(point.X + weight, point.Y + weight),
            new Point(point.X + weight * 2.0, point.Y),
            new Point(point.X + weight, point.Y - weight),
            point,
        });
    }

    public RhombusSymbol()
    {
    }
}
