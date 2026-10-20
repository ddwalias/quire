#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class SphereMesh : MeshGeneratorBase
{
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point3D), typeof(SphereMesh), new PropertyMetadata(default(Point3D), PropertyChanged));

    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(SphereMesh), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register("Slices", typeof(int), typeof(SphereMesh), new PropertyMetadata(36, PropertyChanged), ValidateSlices);

    public static readonly DependencyProperty StacksProperty = DependencyProperty.Register("Stacks", typeof(int), typeof(SphereMesh), new PropertyMetadata(18, PropertyChanged), ValidateStacks);

    public Point3D Center
    {
        get => (Point3D)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public int Slices
    {
        get => (int)GetValue(SlicesProperty);
        set => SetValue(SlicesProperty, value);
    }

    public int Stacks
    {
        get => (int)GetValue(StacksProperty);
        set => SetValue(StacksProperty, value);
    }

    public SphereMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    private static bool ValidateSlices(object obj)
    {
        return (int)obj > 2;
    }

    private static bool ValidateStacks(object obj)
    {
        return (int)obj > 1;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        for (int i = 0; i <= Stacks; i++)
        {
            double num = Math.PI / 2.0 - (double)i * Math.PI / (double)Stacks;
            double y = Radius * Math.Sin(num);
            double num2 = -Radius * Math.Cos(num);
            for (int j = 0; j <= Slices; j++)
            {
                double num3 = (double)(j * 2) * Math.PI / (double)Slices;
                double x = num2 * Math.Sin(num3);
                double z = num2 * Math.Cos(num3);
                Vector3D vector3D = new Vector3D(x, y, z);
                normals.Add(vector3D);
                vertices.Add(vector3D + Center);
                textures.Add(new Point((double)j / (double)Slices, (double)i / (double)Stacks));
            }
        }
        for (int k = 0; k < Stacks; k++)
        {
            for (int l = 0; l < Slices; l++)
            {
                if (k != 0)
                {
                    indices.Add(k * (Slices + 1) + l);
                    indices.Add((k + 1) * (Slices + 1) + l);
                    indices.Add(k * (Slices + 1) + l + 1);
                }
                if (k != Stacks - 1)
                {
                    indices.Add(k * (Slices + 1) + l + 1);
                    indices.Add((k + 1) * (Slices + 1) + l);
                    indices.Add((k + 1) * (Slices + 1) + l + 1);
                }
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new SphereMesh();
    }
}
