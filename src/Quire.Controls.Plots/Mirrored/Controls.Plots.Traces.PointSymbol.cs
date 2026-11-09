#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Traces;

public partial class PointSymbol
{
    public override bool IsFilled => true;

    public override Geometry Create(Point point, ISymbolProperties properties)
    {
        double weight = properties.LineWeight;
        return new EllipseGeometry(point, weight, weight);
    }

    public PointSymbol()
    {
    }
}
