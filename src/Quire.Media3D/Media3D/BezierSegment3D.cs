#nullable disable
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class BezierSegment3D : PathSegment3D
{
    public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
        "Point1",
        typeof(Point3D),
        typeof(BezierSegment3D),
        new PropertyMetadata((object)default(Point3D)));

    public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
        "Point2",
        typeof(Point3D),
        typeof(BezierSegment3D),
        new PropertyMetadata((object)default(Point3D)));

    public static readonly DependencyProperty Point3Property = DependencyProperty.Register(
        "Point3",
        typeof(Point3D),
        typeof(BezierSegment3D),
        new PropertyMetadata((object)default(Point3D)));

    public BezierSegment3D()
    {
    }

    public Point3D Point1
    {
        get => (Point3D)GetValue(Point1Property);
        set => SetValue(Point1Property, value);
    }

    public Point3D Point2
    {
        get => (Point3D)GetValue(Point2Property);
        set => SetValue(Point2Property, value);
    }

    public Point3D Point3
    {
        get => (Point3D)GetValue(Point3Property);
        set => SetValue(Point3Property, value);
    }

    public override string ToString()
    {
        return string.Format("C{0} {1} {2}", Point1, Point2, Point3);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BezierSegment3D();
    }
}
