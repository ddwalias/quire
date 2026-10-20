#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class TubeMesh : CylindricalMeshBase
{
    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(TubeMesh), new PropertyMetadata(0.25, PropertyChanged), ValidateThickness);

    public static readonly DependencyProperty EndStacksProperty = DependencyProperty.Register("EndStacks", typeof(int), typeof(TubeMesh), new PropertyMetadata(1, PropertyChanged), ValidateEndStacks);

    public static readonly DependencyProperty FoldProperty = DependencyProperty.Register("Fold", typeof(double), typeof(TubeMesh), new PropertyMetadata(0.125, PropertyChanged), ValidateFold);

    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    public int EndStacks
    {
        get => (int)GetValue(EndStacksProperty);
        set => SetValue(EndStacksProperty, value);
    }

    public double Fold
    {
        get => (double)GetValue(FoldProperty);
        set => SetValue(FoldProperty, value);
    }

    public TubeMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    private static bool ValidateThickness(object obj)
    {
        return (double)obj >= 0.0;
    }

    private static bool ValidateEndStacks(object obj)
    {
        return (int)obj > 0;
    }

    private static bool ValidateFold(object obj)
    {
        return (double)obj < 0.5;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        for (int num = 1; num >= -1; num -= 2)
        {
            int count = vertices.Count;
            for (int i = 0; i <= EndStacks; i++)
            {
                double length = Length;
                double num2 = Radius + (double)(num * i) * Thickness / 2.0 / (double)EndStacks;
                int num3 = count + i * (Slices + 1);
                int num4 = count + (i + 1) * (Slices + 1);
                for (int j = 0; j <= Slices; j++)
                {
                    double num5 = (double)(j * 2) * Math.PI / (double)Slices;
                    double x = -num2 * Math.Sin(num5);
                    double z = -num2 * Math.Cos(num5);
                    vertices.Add(new Point3D(x, length, z));
                    normals.Add(new Vector3D(0.0, num, 0.0));
                    textures.Add(new Point((double)j / (double)Slices, Fold * (double)i / (double)EndStacks));
                    if (i < EndStacks && j < Slices)
                    {
                        indices.Add(num3 + j);
                        indices.Add(num4 + j);
                        indices.Add(num3 + j + 1);
                        indices.Add(num3 + j + 1);
                        indices.Add(num4 + j);
                        indices.Add(num4 + j + 1);
                    }
                }
            }
            count = vertices.Count;
            for (int k = 0; k <= Stacks; k++)
            {
                double y = Length - (double)k * Length / (double)Stacks;
                int num6 = count + k * (Slices + 1);
                int num7 = count + (k + 1) * (Slices + 1);
                for (int l = 0; l <= Slices; l++)
                {
                    double num8 = (double)(l * 2) * Math.PI / (double)Slices;
                    double num9 = -(Radius + (double)num * Thickness / 2.0) * Math.Sin(num8);
                    double num10 = -(Radius + (double)num * Thickness / 2.0) * Math.Cos(num8);
                    vertices.Add(new Point3D(num9, y, num10));
                    normals.Add(new Vector3D((double)num * num9, 0.0, (double)num * num10));
                    textures.Add(new Point((double)l / (double)Slices, Fold + (1.0 - 2.0 * Fold) * (double)k / (double)Stacks));
                    if (k < Stacks && l < Slices)
                    {
                        indices.Add(num6 + l);
                        indices.Add(num7 + l);
                        indices.Add(num6 + l + 1);
                        indices.Add(num6 + l + 1);
                        indices.Add(num7 + l);
                        indices.Add(num7 + l + 1);
                    }
                }
            }
            count = vertices.Count;
            for (int m = 0; m <= EndStacks; m++)
            {
                double y2 = 0.0;
                double num11 = Radius + (double)num * Thickness / 2.0 * (1.0 - (double)m / (double)EndStacks);
                int num12 = count + m * (Slices + 1);
                int num13 = count + (m + 1) * (Slices + 1);
                for (int n = 0; n <= Slices; n++)
                {
                    double num14 = (double)(n * 2) * Math.PI / (double)Slices;
                    double x2 = -num11 * Math.Sin(num14);
                    double z2 = -num11 * Math.Cos(num14);
                    vertices.Add(new Point3D(x2, y2, z2));
                    normals.Add(new Vector3D(0.0, -num, 0.0));
                    textures.Add(new Point((double)n / (double)Slices, 1.0 - Fold + Fold * (double)m / (double)EndStacks));
                    if (m < EndStacks && n < Slices)
                    {
                        indices.Add(num12 + n);
                        indices.Add(num13 + n);
                        indices.Add(num12 + n + 1);
                        indices.Add(num12 + n + 1);
                        indices.Add(num13 + n);
                        indices.Add(num13 + n + 1);
                    }
                }
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new TubeMesh();
    }
}
