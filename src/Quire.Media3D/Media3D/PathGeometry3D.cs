using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

[ContentProperty("Figures")]
public partial class PathGeometry3D : Animatable
{
    public static readonly DependencyProperty FiguresProperty;

    public static readonly DependencyProperty TransformProperty;

    public PathFigure3DCollection Figures
    {
        get => (PathFigure3DCollection)GetValue(FiguresProperty);
        set => SetValue(FiguresProperty, value);
    }

    public Transform3D Transform
    {
        get => (Transform3D)GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }

    public PathGeometry3D()
    {
        Figures = new PathFigure3DCollection();
    }

    public void GetPointAtFractionLength(double progress, out Point3D point, out Vector3D tangent)
    {
        progress = Math.Max(0.0, Math.Min(1.0, progress));
        point = default;
        tangent = default;
        double num = 0.0;
        double distanceAlongPath = 0.0;
        foreach (PathFigure3D figure in Figures)
        {
            PathFigure3D flattenedPathFigure = figure.GetFlattenedPathFigure();
            Point3D point3D = flattenedPathFigure.StartPoint;
            foreach (PathSegment3D segment in flattenedPathFigure.Segments)
            {
                foreach (Point3D polylinePoint in (segment as PolyLineSegment3D).Points)
                {
                    num += (polylinePoint - point3D).Length;
                    point3D = polylinePoint;
                }
            }
        }
        foreach (PathFigure3D figure in Figures)
        {
            PathFigure3D flattenedFigure = figure.GetFlattenedPathFigure();
            Point3D previousPoint = flattenedFigure.StartPoint;
            foreach (PathSegment3D segment in flattenedFigure.Segments)
            {
                foreach (Point3D pathPoint in (segment as PolyLineSegment3D).Points)
                {
                    tangent = pathPoint - previousPoint;
                    double length = tangent.Length;
                    distanceAlongPath += length;
                    if (distanceAlongPath / num >= progress)
                    {
                        double segmentProgress = (distanceAlongPath / num - progress) / (length / num);
                        double remainingProgress = 1.0 - segmentProgress;
                        point = (Point3D)(segmentProgress * (Vector3D)previousPoint + remainingProgress * (Vector3D)pathPoint);
                        return;
                    }
                    previousPoint = pathPoint;
                }
            }
        }
    }

    public override string ToString()
    {
        return Figures.ToString();
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PathGeometry3D();
    }

    static PathGeometry3D()
    {
        FiguresProperty = DependencyProperty.Register("Figures", typeof(PathFigure3DCollection), typeof(PathGeometry3D));
        TransformProperty = DependencyProperty.Register("Transform", typeof(Transform3D), typeof(PathGeometry3D), new PropertyMetadata(Transform3D.Identity));
    }
}
