#nullable disable
using System.Windows.Media.Media3D;
using Quire.Mirror;

namespace Quire.Media3D;

public partial struct LineRange
{
    private Point3D point1;
    private Point3D point2;

    public LineRange(Point3D point1, Point3D point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }

    public Point3D Point1
    {
        get => point1;
        set => point1 = value;
    }

    public Point3D Point2
    {
        get => point2;
        set => point2 = value;
    }

    public Point3D PointFromX(double x)
    {
        double num = (x - Point1.X) / (Point2.X - Point1.X);
        double y = Point1.Y + num * (Point2.Y - Point1.Y);
        double z = Point1.Z + num * (Point2.Z - Point1.Z);
        return new Point3D(x, y, z);
    }

    public Point3D PointFromY(double y)
    {
        double num = (y - Point1.Y) / (Point2.Y - Point1.Y);
        double x = Point1.X + num * (Point2.X - Point1.X);
        double z = Point1.Z + num * (Point2.Z - Point1.Z);
        return new Point3D(x, y, z);
    }

    public Point3D PointFromZ(double z)
    {
        double num = (z - Point1.Z) / (Point2.Z - Point1.Z);
        double x = Point1.X + num * (Point2.X - Point1.X);
        double y = Point1.Y + num * (Point2.Y - Point1.Y);
        return new Point3D(x, y, z);
    }
}
