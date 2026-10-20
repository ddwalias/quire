#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class TorusMesh : MeshGeneratorBase
{
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(TorusMesh), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty TubeRadiusProperty = DependencyProperty.Register("TubeRadius", typeof(double), typeof(TorusMesh), new PropertyMetadata(0.25, PropertyChanged));

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register("Slices", typeof(int), typeof(TorusMesh), new PropertyMetadata(18, PropertyChanged), ValidateSlices);

    public static readonly DependencyProperty StacksProperty = DependencyProperty.Register("Stacks", typeof(int), typeof(TorusMesh), new PropertyMetadata(36, PropertyChanged), ValidateStacks);

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public double TubeRadius
    {
        get => (double)GetValue(TubeRadiusProperty);
        set => SetValue(TubeRadiusProperty, value);
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

    public TorusMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    private static bool ValidateSlices(object obj)
    {
        return (int)obj > 2;
    }

    private static bool ValidateStacks(object obj)
    {
        return (int)obj > 2;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        for (int i = 0; i <= Stacks; i++)
        {
            double num = (double)(i * 2) * Math.PI / (double)Stacks;
            double x = Radius * Math.Sin(num);
            double y = Radius * Math.Cos(num);
            Point3D point3D = new Point3D(x, y, 0.0);
            for (int j = 0; j <= Slices; j++)
            {
                double num2 = (double)(j * 2) * Math.PI / (double)Slices + Math.PI;
                double x2 = (Radius + TubeRadius * Math.Cos(num2)) * Math.Sin(num);
                double y2 = (Radius + TubeRadius * Math.Cos(num2)) * Math.Cos(num);
                double z = -TubeRadius * Math.Sin(num2);
                Point3D point3D2 = new Point3D(x2, y2, z);
                vertices.Add(point3D2);
                normals.Add(point3D2 - point3D);
                textures.Add(new Point((double)j / (double)Slices, (double)i / (double)Stacks));
            }
        }
        for (int k = 0; k < Stacks; k++)
        {
            for (int l = 0; l < Slices; l++)
            {
                indices.Add(k * (Slices + 1) + l);
                indices.Add((k + 1) * (Slices + 1) + l);
                indices.Add(k * (Slices + 1) + l + 1);
                indices.Add(k * (Slices + 1) + l + 1);
                indices.Add((k + 1) * (Slices + 1) + l);
                indices.Add((k + 1) * (Slices + 1) + l + 1);
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new TorusMesh();
    }
}
