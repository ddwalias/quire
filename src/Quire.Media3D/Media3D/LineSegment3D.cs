#nullable disable
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class LineSegment3D : PathSegment3D
{
    public static readonly DependencyProperty PointProperty = DependencyProperty.Register(
        "Point",
        typeof(Point3D),
        typeof(LineSegment3D),
        new PropertyMetadata((object)new Point3D(0.0, 0.0, 0.0)));

    public LineSegment3D()
    {
    }

    public Point3D Point
    {
        get => (Point3D)GetValue(PointProperty);
        set => SetValue(PointProperty, value);
    }

    public override string ToString()
    {
        return "L" + Point.ToString();
    }

    protected override Freezable CreateInstanceCore()
    {
        return new LineSegment3D();
    }
}
