#nullable disable
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class PolyLineSegment3D : PathSegment3D
{
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        "Points",
        typeof(Point3DCollection),
        typeof(PolyLineSegment3D));

    public PolyLineSegment3D()
    {
        Points = new Point3DCollection();
    }

    public Point3DCollection Points
    {
        get => (Point3DCollection)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public override string ToString()
    {
        return "L" + Points;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PolyLineSegment3D();
    }
}
