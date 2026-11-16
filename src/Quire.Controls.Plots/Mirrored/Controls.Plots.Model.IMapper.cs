using System.Windows.Media;

namespace Quire.Controls.Plots.Model;

public partial interface IMapper
{
    StyleType StyleType { get; }

    Color Color { get; set; }

    uint Alpha { get; set; }

    bool UseTraceInvalidStyle { get; set; }

    uint[] MapDensityToRGBA(double[] density, double maxDensityValue);
}
