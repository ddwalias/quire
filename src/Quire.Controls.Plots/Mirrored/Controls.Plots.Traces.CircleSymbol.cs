#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class CircleSymbol
{
    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        return new EllipseGeometry(point, properties.SymbolWeight, properties.SymbolWeight);
    }

    public CircleSymbol()
    {
    }
}
