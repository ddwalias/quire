#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots;

internal partial class StarPoints
{
    private Point[] _primaryPoints;
    private Point[] _secondaryPoints;
    private Point[] _mergedPoints;
    private int _rays = 5;
    private double _radius = 15.0;
    private Point _center = new Point(0.0, 0.0);

    private StarPoints(int rays, double radius, Point center)
    {
        if (rays < 5)
        {
            throw new ArgumentOutOfRangeException("rays", "cannot define star for less then 5 rays");
        }
        if (radius < 0.0)
        {
            throw new ArgumentOutOfRangeException("radius", "radius must be positive");
        }
        _rays = rays;
        _radius = radius;
        _center = center;
        CalculatePoints();
    }

    public static PointCollection generateStarPoints(int rays, double radius, Point center)
    {
        StarPoints starPoints = new StarPoints(rays, radius, center);
        Array.Reverse(starPoints._mergedPoints);
        return new PointCollection(starPoints._mergedPoints);
    }

    private static double getX(double radius, double startAngle, double stepAngle, int iPoint)
    {
        return radius * Math.Cos(startAngle + stepAngle * (double)iPoint);
    }

    private static double getY(double radius, double startAngle, double stepAngle, int iPoint)
    {
        // Mathcad negates the radius with neg before the multiply; a 0.0 - radius subtraction differs.
        return -radius * Math.Sin(startAngle + stepAngle * (double)iPoint);
    }

    private void CalculatePoints()
    {
        _primaryPoints = BuildPrimaryPoints();
        _secondaryPoints = BuildSecondaryPoints();
        _mergedPoints = MergePoints();
        AdjustToCenter(_mergedPoints);
    }

    private void AdjustToCenter(Point[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].Offset(_center.X, _center.Y);
        }
    }

    private Point[] MergePoints()
    {
        Point[] array = new Point[_rays * 2];
        for (int i = 0; i < _rays; i++)
        {
            array[2 * i] = _primaryPoints[i];
            array[2 * i + 1] = _secondaryPoints[i];
        }
        return array;
    }

    private double FindSecondaryRadius()
    {
        Point[] primaryPoints = _primaryPoints;
        if (primaryPoints == null || primaryPoints.Length < 5)
        {
            throw new ArgumentException("minimal number of points should be 5");
        }
        findLineCoefficients(out var ac, out var b, primaryPoints[0], primaryPoints[2]);
        findLineCoefficients(out var ac2, out var b2, primaryPoints[primaryPoints.Length - 1], primaryPoints[1]);
        Point? val = findIntersectionPoint(ac, b, ac2, b2);
        if (!val.HasValue)
        {
            throw new ArgumentException("points declare parallel lines");
        }
        Vector val2 = new Vector(val.Value.X, val.Value.Y);
        return val2.Length;
    }

    private static void findLineCoefficients(out double ac, out double b, Point point1, Point point2)
    {
        ac = (point1.Y - point2.Y) / (point1.X - point2.X);
        b = point1.Y - ac * point1.X;
    }

    private static Point? findIntersectionPoint(double a1, double b1, double a2, double b2)
    {
        if (a1 == a2)
        {
            return null;
        }
        double num = -(b1 - b2) / (a1 - a2);
        double num2 = a1 * num + b1;
        return new Point(num, num2);
    }

    private Point[] BuildPrimaryPoints()
    {
        double startAngle = Math.PI / 2.0;
        return BuildPoints(startAngle, _radius);
    }

    private Point[] BuildSecondaryPoints()
    {
        double radius = FindSecondaryRadius();
        double startAngle = Math.PI / 2.0 + Math.PI / (double)_rays;
        return BuildPoints(startAngle, radius);
    }

    private Point[] BuildPoints(double startAngle, double radius)
    {
        Point[] array = new Point[_rays];
        double stepAngle = Math.PI * 2.0 / (double)_rays;
        for (int i = 0; i < _rays; i++)
        {
            double x = getX(radius, startAngle, stepAngle, i);
            double y = getY(radius, startAngle, stepAngle, i);
            array[i] = new Point(x, y);
        }
        return array;
    }
}
