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
        double stepSize = (xMax - xMin) / nPoints;
        points.Clear();
        for (double x = xMin; x <= xMax; x += stepSize)
        {
            double y = f(x);
            points.Add(new Point(x, y));
            if (stepSize == 0.0)
            {
                break;
            }
        }
    }
}
