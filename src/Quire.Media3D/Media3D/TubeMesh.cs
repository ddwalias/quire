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
                double ringRadius = Radius + (double)(num * i) * Thickness / 2.0 / (double)EndStacks;
                int currentRingIndex = count + i * (Slices + 1);
                int nextRingIndex = count + (i + 1) * (Slices + 1);
                for (int j = 0; j <= Slices; j++)
                {
                    double angle = (double)(j * 2) * Math.PI / (double)Slices;
                    double x = -ringRadius * Math.Sin(angle);
                    double z = -ringRadius * Math.Cos(angle);
                    vertices.Add(new Point3D(x, length, z));
                    normals.Add(new Vector3D(0.0, num, 0.0));
                    textures.Add(new Point((double)j / (double)Slices, Fold * (double)i / (double)EndStacks));
                    if (i < EndStacks && j < Slices)
                    {
                        indices.Add(currentRingIndex + j);
                        indices.Add(nextRingIndex + j);
                        indices.Add(currentRingIndex + j + 1);
                        indices.Add(currentRingIndex + j + 1);
                        indices.Add(nextRingIndex + j);
                        indices.Add(nextRingIndex + j + 1);
                    }
                }
            }
            count = vertices.Count;
            for (int k = 0; k <= Stacks; k++)
            {
                double y = Length - (double)k * Length / (double)Stacks;
                int currentRingIndex = count + k * (Slices + 1);
                int nextRingIndex = count + (k + 1) * (Slices + 1);
                for (int l = 0; l <= Slices; l++)
                {
                    double angle = (double)(l * 2) * Math.PI / (double)Slices;
                    double x = -(Radius + (double)num * Thickness / 2.0) * Math.Sin(angle);
                    double z = -(Radius + (double)num * Thickness / 2.0) * Math.Cos(angle);
                    vertices.Add(new Point3D(x, y, z));
                    normals.Add(new Vector3D((double)num * x, 0.0, (double)num * z));
                    textures.Add(new Point((double)l / (double)Slices, Fold + (1.0 - 2.0 * Fold) * (double)k / (double)Stacks));
                    if (k < Stacks && l < Slices)
                    {
                        indices.Add(currentRingIndex + l);
                        indices.Add(nextRingIndex + l);
                        indices.Add(currentRingIndex + l + 1);
                        indices.Add(currentRingIndex + l + 1);
                        indices.Add(nextRingIndex + l);
                        indices.Add(nextRingIndex + l + 1);
                    }
                }
            }
            count = vertices.Count;
            for (int m = 0; m <= EndStacks; m++)
            {
                double y = 0.0;
                double ringRadius = Radius + (double)num * Thickness / 2.0 * (1.0 - (double)m / (double)EndStacks);
                int currentRingIndex = count + m * (Slices + 1);
                int nextRingIndex = count + (m + 1) * (Slices + 1);
                for (int n = 0; n <= Slices; n++)
                {
                    double angle = (double)(n * 2) * Math.PI / (double)Slices;
                    double x = -ringRadius * Math.Sin(angle);
                    double z = -ringRadius * Math.Cos(angle);
                    vertices.Add(new Point3D(x, y, z));
                    normals.Add(new Vector3D(0.0, -num, 0.0));
                    textures.Add(new Point((double)n / (double)Slices, 1.0 - Fold + Fold * (double)m / (double)EndStacks));
                    if (m < EndStacks && n < Slices)
                    {
                        indices.Add(currentRingIndex + n);
                        indices.Add(nextRingIndex + n);
                        indices.Add(currentRingIndex + n + 1);
                        indices.Add(currentRingIndex + n + 1);
                        indices.Add(nextRingIndex + n);
                        indices.Add(nextRingIndex + n + 1);
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
