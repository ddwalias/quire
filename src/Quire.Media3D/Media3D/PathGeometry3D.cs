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
        double num2 = 0.0;
        foreach (PathFigure3D figure in Figures)
        {
            PathFigure3D flattenedPathFigure = figure.GetFlattenedPathFigure();
            Point3D point3D = flattenedPathFigure.StartPoint;
            foreach (PathSegment3D segment in flattenedPathFigure.Segments)
            {
                foreach (Point3D point2 in (segment as PolyLineSegment3D).Points)
                {
                    num += (point2 - point3D).Length;
                    point3D = point2;
                }
            }
        }
        foreach (PathFigure3D figure2 in Figures)
        {
            PathFigure3D flattenedPathFigure2 = figure2.GetFlattenedPathFigure();
            Point3D point3D2 = flattenedPathFigure2.StartPoint;
            foreach (PathSegment3D segment2 in flattenedPathFigure2.Segments)
            {
                foreach (Point3D point3 in (segment2 as PolyLineSegment3D).Points)
                {
                    tangent = point3 - point3D2;
                    double length = tangent.Length;
                    num2 += length;
                    if (num2 / num >= progress)
                    {
                        double num3 = (num2 / num - progress) / (length / num);
                        double num4 = 1.0 - num3;
                        point = (Point3D)(num3 * (Vector3D)point3D2 + num4 * (Vector3D)point3);
                        return;
                    }
                    point3D2 = point3;
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
