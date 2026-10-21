using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public static partial class Tools3D
{
    private static readonly Material _emptyMaterial;

    static Tools3D()
    {
        _emptyMaterial = new DiffuseMaterial();
        ((Freezable)_emptyMaterial).Freeze();
    }

    public static void GetLineGeometry(Point3D pt1, Point3D pt2, double thickness, Matrix3D matxVisualToScreen, Matrix3D matxScreenToVisual, Point3DCollection points, Int32Collection indices)
    {
        if (double.IsNaN(thickness) || double.IsInfinity(thickness) || thickness < 0.0)
        {
            DebugTools.FailAssertion("Thickness value is not valid : {0}", thickness);
            thickness = 0.0;
        }
        if (Point3D.Equals(pt1, pt2))
        {
            DebugTools.FailAssertionDoNotThrow("Invalid input 3D points : {0}, {1}", pt1, pt2);
            thickness = 0.0;
            pt2 = Correct3DPointCoordinates(pt1);
        }
        Point3D point3D = pt1 * matxVisualToScreen;
        Point3D point3D2 = pt2 * matxVisualToScreen;
        if ((point3D2 - point3D).X == 0.0 && (point3D2 - point3D).Y == 0.0)
        {
            pt2 = Correct3DPointCoordinates(pt2);
            point3D2 = pt2 * matxVisualToScreen;
        }
        Vector3D vector3D = point3D2 - point3D;
        vector3D.Z = 0.0;
        vector3D.Normalize();
        Vector val = thickness / 2.0 * new Vector(-vector3D.Y, vector3D.X);
        int count = points.Count;
        bool flag = false;
        try
        {
            points.Add(Widen(pt1, val, matxVisualToScreen, matxScreenToVisual));
            points.Add(Widen(pt1, -val, matxVisualToScreen, matxScreenToVisual));
            points.Add(Widen(pt2, val, matxVisualToScreen, matxScreenToVisual));
            points.Add(Widen(pt2, -val, matxVisualToScreen, matxScreenToVisual));
            flag = true;
        }
        catch (ArgumentException ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }
        if (flag)
        {
            indices.Add(count);
            indices.Add(count + 2);
            indices.Add(count + 1);
            indices.Add(count + 1);
            indices.Add(count + 2);
            indices.Add(count + 3);
        }
    }

    public static Point3D Widen(Point3D pointIn, Vector screenDelta, Matrix3D matxVisualToScreen, Matrix3D matxScreenToVisual)
    {
        Point4D point4D = (Point4D)pointIn * matxVisualToScreen;
        point4D.X += screenDelta.X * point4D.W;
        point4D.Y += screenDelta.Y * point4D.W;
        point4D *= matxScreenToVisual;
        if (point4D.W == 0.0 || double.IsNaN(point4D.W) || double.IsInfinity(point4D.W))
        {
            throw new ArgumentException("Either the visual to screen matrix or 3D point is invalid");
        }
        return new Point3D(point4D.X / point4D.W, point4D.Y / point4D.W, point4D.Z / point4D.W);
    }

    private static Point3D Correct3DPointCoordinates(Point3D point)
    {
        point.X += 1E-07;
        point.Y += 1E-07;
        point.Z += 1E-07;
        return point;
    }

    public static Vector3D InverseVector(Vector3D vector)
    {
        return new Vector3D(1.0 / vector.X, 1.0 / vector.Y, 1.0 / vector.Z);
    }

    public static Vector3D BoxCenter(Rect3D box)
    {
        return new Vector3D(box.X + box.SizeX * 0.5, box.Y + box.SizeY * 0.5, box.Z + box.SizeZ * 0.5);
    }

    public static Material MaterialForColor(Color? color)
    {
        if (!color.HasValue)
        {
            return _emptyMaterial;
        }
        DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(color.Value));
        ((Freezable)diffuseMaterial).Freeze();
        return diffuseMaterial;
    }
}
