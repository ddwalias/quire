using System.Windows.Media;

namespace Quire.Controls.Plots;

public partial interface IColorMap
{
    Brush GetBrush(double value);
}
