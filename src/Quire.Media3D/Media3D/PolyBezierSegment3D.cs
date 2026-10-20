#nullable disable
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class PolyBezierSegment3D : PathSegment3D
{
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        "Points",
        typeof(Point3DCollection),
        typeof(PolyBezierSegment3D),
        new PropertyMetadata((PropertyChangedCallback)null));

    public PolyBezierSegment3D()
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
        return "C" + Points.ToString();
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PolyBezierSegment3D();
    }
}
