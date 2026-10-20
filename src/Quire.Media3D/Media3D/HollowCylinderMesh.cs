#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class HollowCylinderMesh : CylindricalMeshBase
{
    public HollowCylinderMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        for (int i = 0; i <= Stacks; i++)
        {
            double y = Length - (double)i * Length / (double)Stacks;
            for (int j = 0; j <= Slices; j++)
            {
                double num = (double)(j * 2) * Math.PI / (double)Slices;
                double x = -Radius * Math.Sin(num);
                double z = -Radius * Math.Cos(num);
                normals.Add(new Vector3D(x, 0.0, z));
                vertices.Add(new Point3D(x, y, z));
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
        return new HollowCylinderMesh();
    }
}
