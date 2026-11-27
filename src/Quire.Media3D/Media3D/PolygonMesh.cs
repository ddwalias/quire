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
        double angle = 4.71238898038469 + num / 2.0;
        double minimumX = 0.0;
        double maximumX = 0.0;
        double minimumY = 0.0;
        double maximumY = 0.0;
        for (int i = 0; i < Sides; i++)
        {
            double val = Math.Cos(angle);
            double sine = Math.Sin(angle);
            minimumX = Math.Min(minimumX, val);
            maximumX = Math.Max(maximumX, val);
            minimumY = Math.Min(minimumY, sine);
            maximumY = Math.Max(maximumY, sine);
            angle += num;
        }
        angle = 4.71238898038469 + num / 2.0;
        for (int j = 0; j < Sides; j++)
        {
            vertices.Add(new Point3D(0.0, 0.0, 0.0));
            textures.Add(new Point(-minimumX / (maximumX - minimumX), maximumY / (maximumY - minimumY)));
            normals.Add(value);
            double x = Math.Cos(angle);
            double y = Math.Sin(angle);
            vertices.Add(new Point3D(x, y, 0.0));
            textures.Add(new Point((x - minimumX) / (maximumX - minimumX), (maximumY - y) / (maximumY - minimumY)));
            normals.Add(value);
            angle += num;
            x = Math.Cos(angle);
            y = Math.Sin(angle);
            vertices.Add(new Point3D(x, y, 0.0));
            textures.Add(new Point((x - minimumX) / (maximumX - minimumX), (maximumY - y) / (maximumY - minimumY)));
            normals.Add(value);
            int firstTriangleIndex = vertices.Count - 3;
            indices.Add(firstTriangleIndex);
            indices.Add(firstTriangleIndex + 1);
            indices.Add(firstTriangleIndex + 2);
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
