using System.Windows;

namespace Quire.Controls.Plots.Contour.Model;

public partial struct ContourLogicalData
{
    public double ZLevel { get; set; }

    public Point[] Points { get; set; }

    public double GradientSign { get; set; }

    public override string ToString()
    {
        return string.Format("{0},{1}", ZLevel, Points);
    }
}
