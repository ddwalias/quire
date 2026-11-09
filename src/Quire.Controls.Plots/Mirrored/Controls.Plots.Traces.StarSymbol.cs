#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class StarSymbol
{
    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        return CreatePathGeometryPointCollection(StarPoints.generateStarPoints(5, properties.SymbolWeight, point));
    }

    public StarSymbol()
    {
    }
}
