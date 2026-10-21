using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class WirePath : WireBase
{
    public static readonly DependencyProperty DataProperty;

    public PathGeometry3D Data
    {
        get => (PathGeometry3D)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public WirePath()
    {
        Data = (PathGeometry3D)Data.Clone();
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        lines.Clear();
        if (Data == null)
        {
            return;
        }
        Transform3D transform = Data.Transform;
        foreach (PathFigure3D figure in Data.Figures)
        {
            PathFigure3D flattenedPathFigure = figure.GetFlattenedPathFigure();
            Point3D value = transform.Transform(flattenedPathFigure.StartPoint);
            foreach (PathSegment3D segment in flattenedPathFigure.Segments)
            {
                PolyLineSegment3D polyLineSegment3D = segment as PolyLineSegment3D;
                for (int i = 0; i < polyLineSegment3D.Points.Count; i++)
                {
                    lines.Add(value);
                    Point3D point3D = transform.Transform(polyLineSegment3D.Points[i]);
                    lines.Add(point3D);
                    value = point3D;
                }
            }
        }
    }

    static WirePath()
    {
        DataProperty = DependencyProperty.Register("Data", typeof(PathGeometry3D), typeof(WirePath), new PropertyMetadata(new PathGeometry3D(), new PropertyChangedCallback(WireBase.PropertyChanged)));
    }
}
