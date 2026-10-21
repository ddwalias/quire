using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

[ContentProperty("Segments")]
public partial class PathFigure3D : Animatable
{
    public static int Approximation;

    private PathFigure3D figFlattened;

    public static readonly DependencyProperty StartPointProperty;

    public static readonly DependencyProperty SegmentsProperty;

    public Point3D StartPoint
    {
        get => (Point3D)GetValue(StartPointProperty);
        set => SetValue(StartPointProperty, value);
    }

    public PathSegment3DCollection Segments
    {
        get => (PathSegment3DCollection)GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public PathFigure3D()
    {
        figFlattened = new PathFigure3D(0);
        Segments = new PathSegment3DCollection();
    }

    private PathFigure3D(int unused)
    {
        Segments = new PathSegment3DCollection();
        Segments.Add(new PolyLineSegment3D());
    }

    private static void StartPointPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        PathFigure3D pathFigure3D = obj as PathFigure3D;
        if (pathFigure3D.figFlattened != null)
        {
            pathFigure3D.figFlattened.StartPoint = (Point3D)args.NewValue;
            SegmentsPropertyChanged(obj, args);
        }
    }

    private static void SegmentsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        PathFigure3D pathFigure3D = obj as PathFigure3D;
        if (pathFigure3D.figFlattened != null)
        {
            pathFigure3D.SegmentsPropertyChanged();
        }
    }

    private void SegmentsPropertyChanged()
    {
        PolyLineSegment3D polyLineSegment3D = figFlattened.Segments[0] as PolyLineSegment3D;
        Point3DCollection points = polyLineSegment3D.Points;
        polyLineSegment3D.Points = null;
        points.Clear();
        Point3D point = StartPoint;
        foreach (PathSegment3D segment in Segments)
        {
            if (segment is LineSegment3D)
            {
                LineSegment3D lineSegment3D = segment as LineSegment3D;
                points.Add(lineSegment3D.Point);
                point = lineSegment3D.Point;
            }
            else if (segment is PolyLineSegment3D)
            {
                foreach (Point3D point2 in (segment as PolyLineSegment3D).Points)
                {
                    points.Add(point2);
                    point = point2;
                }
            }
            else if (segment is BezierSegment3D)
            {
                BezierSegment3D bezierSegment3D = segment as BezierSegment3D;
                ConvertBezier(points, point, bezierSegment3D.Point1, bezierSegment3D.Point2, bezierSegment3D.Point3);
                point = bezierSegment3D.Point3;
            }
            else if (segment is PolyBezierSegment3D)
            {
                PolyBezierSegment3D polyBezierSegment3D = segment as PolyBezierSegment3D;
                for (int i = 0; i < polyBezierSegment3D.Points.Count; i += 3)
                {
                    ConvertBezier(points, point, polyBezierSegment3D.Points[i], polyBezierSegment3D.Points[i + 1], polyBezierSegment3D.Points[i + 2]);
                    point = polyBezierSegment3D.Points[i + 2];
                }
            }
        }
        polyLineSegment3D.Points = points;
    }

    public PathFigure3D GetFlattenedPathFigure()
    {
        return figFlattened;
    }

    private void ConvertBezier(Point3DCollection points, Point3D point0, Point3D point1, Point3D point2, Point3D point3)
    {
        for (int i = 0; i < Approximation; i++)
        {
            double num = (double)(i + 1) / (double)Approximation;
            double x = (1.0 - num) * (1.0 - num) * (1.0 - num) * point0.X + 3.0 * num * (1.0 - num) * (1.0 - num) * point1.X + 3.0 * num * num * (1.0 - num) * point2.X + num * num * num * point3.X;
            double y = (1.0 - num) * (1.0 - num) * (1.0 - num) * point0.Y + 3.0 * num * (1.0 - num) * (1.0 - num) * point1.Y + 3.0 * num * num * (1.0 - num) * point2.Y + num * num * num * point3.Y;
            double z = (1.0 - num) * (1.0 - num) * (1.0 - num) * point0.Z + 3.0 * num * (1.0 - num) * (1.0 - num) * point1.Z + 3.0 * num * num * (1.0 - num) * point2.Z + num * num * num * point3.Z;
            points.Add(new Point3D(x, y, z));
        }
    }

    public override string ToString()
    {
        return "M" + StartPoint.ToString() + (object)Segments;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PathFigure3D();
    }

    static PathFigure3D()
    {
        Approximation = 100;
        StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point3D), typeof(PathFigure3D), new PropertyMetadata(default(Point3D), new PropertyChangedCallback(StartPointPropertyChanged)));
        SegmentsProperty = DependencyProperty.Register("Segments", typeof(PathSegment3DCollection), typeof(PathFigure3D), new PropertyMetadata(null, new PropertyChangedCallback(SegmentsPropertyChanged)));
    }
}
