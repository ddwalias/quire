#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Plots;

internal partial class PlotPointsGenerator
{
    private readonly List<Point> points = new List<Point>();

    public IEnumerable<Point> Points => points;

    public PlotPointsGenerator(RealFunction f, double xMin, double xMax, double nPoints)
    {
        double num = (xMax - xMin) / nPoints;
        points.Clear();
        for (double num2 = xMin; num2 <= xMax; num2 += num)
        {
            double num3 = f(num2);
            points.Add(new Point(num2, num3));
            if (num == 0.0)
            {
                break;
            }
        }
    }
}
