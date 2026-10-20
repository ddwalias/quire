#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class CylinderMesh : CylindricalMeshBase
{
    public static readonly DependencyProperty EndStacksProperty = DependencyProperty.Register("EndStacks", typeof(int), typeof(CylinderMesh), new PropertyMetadata(1, PropertyChanged), ValidateEndStacks);

    public static readonly DependencyProperty FoldProperty = DependencyProperty.Register("Fold", typeof(double), typeof(CylinderMesh), new PropertyMetadata(1.0 / 3.0, PropertyChanged), ValidateFold);

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

    public CylinderMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
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
        for (int i = 0; i <= EndStacks; i++)
        {
            double length = Length;
            double num = (double)i * Radius / (double)EndStacks;
            int num2 = i * (Slices + 1);
            int num3 = (i + 1) * (Slices + 1);
            for (int j = 0; j <= Slices; j++)
            {
                double num4 = (double)(j * 2) * Math.PI / (double)Slices;
                double x = -num * Math.Sin(num4);
                double z = -num * Math.Cos(num4);
                vertices.Add(new Point3D(x, length, z));
                normals.Add(new Vector3D(0.0, 1.0, 0.0));
                textures.Add(new Point((double)j / (double)Slices, Fold * (double)i / (double)EndStacks));
                if (i < EndStacks && j < Slices)
                {
                    if (i != 0)
                    {
                        indices.Add(num2 + j);
                        indices.Add(num3 + j);
                        indices.Add(num2 + j + 1);
                    }
                    indices.Add(num2 + j + 1);
                    indices.Add(num3 + j);
                    indices.Add(num3 + j + 1);
                }
            }
        }
        int count = vertices.Count;
        for (int k = 0; k <= Stacks; k++)
        {
            double y = Length - (double)k * Length / (double)Stacks;
            int num5 = count + k * (Slices + 1);
            int num6 = count + (k + 1) * (Slices + 1);
            for (int l = 0; l <= Slices; l++)
            {
                double num7 = (double)(l * 2) * Math.PI / (double)Slices;
                double x2 = -Radius * Math.Sin(num7);
                double z2 = -Radius * Math.Cos(num7);
                vertices.Add(new Point3D(x2, y, z2));
                normals.Add(new Vector3D(x2, 0.0, z2));
                textures.Add(new Point((double)l / (double)Slices, Fold + (1.0 - 2.0 * Fold) * (double)k / (double)Stacks));
                if (k < Stacks && l < Slices)
                {
                    indices.Add(num5 + l);
                    indices.Add(num6 + l);
                    indices.Add(num5 + l + 1);
                    indices.Add(num5 + l + 1);
                    indices.Add(num6 + l);
                    indices.Add(num6 + l + 1);
                }
            }
        }
        count = vertices.Count;
        for (int m = 0; m <= EndStacks; m++)
        {
            double y2 = 0.0;
            double num8 = (double)(EndStacks - m) * Radius / (double)EndStacks;
            int num9 = count + m * (Slices + 1);
            int num10 = count + (m + 1) * (Slices + 1);
            for (int n = 0; n <= Slices; n++)
            {
                double num11 = (double)(n * 2) * Math.PI / (double)Slices;
                double x3 = -num8 * Math.Sin(num11);
                double z3 = -num8 * Math.Cos(num11);
                vertices.Add(new Point3D(x3, y2, z3));
                normals.Add(new Vector3D(0.0, -1.0, 0.0));
                textures.Add(new Point((double)n / (double)Slices, 1.0 - Fold + Fold * (double)m / (double)EndStacks));
                if (m < EndStacks && n < Slices)
                {
                    indices.Add(num9 + n);
                    indices.Add(num10 + n);
                    indices.Add(num9 + n + 1);
                    if (m != EndStacks - 1)
                    {
                        indices.Add(num9 + n + 1);
                        indices.Add(num10 + n);
                        indices.Add(num10 + n + 1);
                    }
                }
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new CylinderMesh();
    }
}
