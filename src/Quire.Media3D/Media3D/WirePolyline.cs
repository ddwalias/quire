using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class WirePolyline : WireBase
{
    public static readonly DependencyProperty PointsProperty;

    public Point3DCollection Points
    {
        get => (Point3DCollection)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        Point3DCollection points = Points;
        lines.Clear();
        for (int i = 0; i < points.Count - 1; i++)
        {
            lines.Add(points[i]);
            lines.Add(points[i + 1]);
        }
    }

    public WirePolyline()
    {
    }

    static WirePolyline()
    {
        PointsProperty = DependencyProperty.Register("Points", typeof(Point3DCollection), typeof(WirePolyline), new PropertyMetadata(null, new PropertyChangedCallback(WireBase.PropertyChanged)));
    }
}
