#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial struct LayoutMetrics
{
    public Size OverallSize;

    public double Baseline;

    public double ItegralTextWidth;

    public LayoutMetrics(Size overallSize, double baseline)
    {
        OverallSize = overallSize;
        Baseline = baseline;
        ItegralTextWidth = -1.0;
    }
}
