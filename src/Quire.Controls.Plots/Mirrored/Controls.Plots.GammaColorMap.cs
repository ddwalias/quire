#nullable disable
#pragma warning disable

using System.Windows.Media;
using Quire.Controls.Plots.Contour;

namespace Quire.Controls.Plots;

internal partial class GammaColorMap
{
    public Brush GetBrush(double value)
    {
        HsvColor hsvColor = new HsvColor
        {
            Gamma = (float)value,
        };
        return new SolidColorBrush(hsvColor.Rgb);
    }
}
