#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class PolygonMesh : FlatSurfaceMeshBase
{
    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register("Length", typeof(double), typeof(PolygonMesh), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty SidesProperty = DependencyProperty.Register("Sides", typeof(int), typeof(PolygonMesh), new PropertyMetadata(5, PropertyChanged));

    public double Length
    {
        get => (double)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    public int Sides
    {
        get => (int)GetValue(SidesProperty);
        set => SetValue(SidesProperty, value);
    }

    public PolygonMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        Vector3D value = new Vector3D(0.0, 0.0, 1.0);
        double num = Math.PI * 2.0 / (double)Sides;
        _ = Length / 2.0 / Math.Sin(num / 2.0);
        double num2 = 4.71238898038469 + num / 2.0;
        double num3 = 0.0;
        double num4 = 0.0;
        double num5 = 0.0;
        double num6 = 0.0;
        for (int i = 0; i < Sides; i++)
        {
            double val = Math.Cos(num2);
            double val2 = Math.Sin(num2);
            num3 = Math.Min(num3, val);
            num4 = Math.Max(num4, val);
            num5 = Math.Min(num5, val2);
            num6 = Math.Max(num6, val2);
            num2 += num;
        }
        num2 = 4.71238898038469 + num / 2.0;
        for (int j = 0; j < Sides; j++)
        {
            vertices.Add(new Point3D(0.0, 0.0, 0.0));
            textures.Add(new Point(-num3 / (num4 - num3), num6 / (num6 - num5)));
            normals.Add(value);
            double num7 = Math.Cos(num2);
            double num8 = Math.Sin(num2);
            vertices.Add(new Point3D(num7, num8, 0.0));
            textures.Add(new Point((num7 - num3) / (num4 - num3), (num6 - num8) / (num6 - num5)));
            normals.Add(value);
            num2 += num;
            num7 = Math.Cos(num2);
            num8 = Math.Sin(num2);
            vertices.Add(new Point3D(num7, num8, 0.0));
            textures.Add(new Point((num7 - num3) / (num4 - num3), (num6 - num8) / (num6 - num5)));
            normals.Add(value);
            int num9 = vertices.Count - 3;
            indices.Add(num9);
            indices.Add(num9 + 1);
            indices.Add(num9 + 2);
            if (Slices > 1)
            {
                TriangleSubdivide(vertices, normals, indices, textures);
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PolygonMesh();
    }
}
