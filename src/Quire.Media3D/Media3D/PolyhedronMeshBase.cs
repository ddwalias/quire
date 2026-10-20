#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public abstract partial class PolyhedronMeshBase : FlatSurfaceMeshBase
{
    public static readonly DependencyProperty TextureCoordinatesProperty = MeshGeometry3D.TextureCoordinatesProperty.AddOwner(
        typeof(PolyhedronMeshBase),
        new PropertyMetadata((PropertyChangedCallback)MeshGeneratorBase.PropertyChanged));

    public PointCollection TextureCoordinates
    {
        get => (PointCollection)GetValue(TextureCoordinatesProperty);
        set => SetValue(TextureCoordinatesProperty, value);
    }

    protected abstract Point3D[,] Faces { get; }

    public PolyhedronMeshBase()
    {
        TextureCoordinates = TextureCoordinates.Clone();
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        Point3D[,] faces = Faces;
        PointCollection textureCoordinates = TextureCoordinates;
        int num = 0;
        for (int i = 0; i < faces.GetLength(0); i++)
        {
            Vector3D value = Vector3D.CrossProduct(faces[i, 1] - faces[i, 0], faces[i, 2] - faces[i, 0]);
            if (faces.GetLength(1) == 3)
            {
                int count = vertices.Count;
                for (int j = 0; j < 3; j++)
                {
                    vertices.Add(faces[i, j]);
                    normals.Add(value);
                    indices.Add(count + j);
                    if (textureCoordinates != null && textureCoordinates.Count > 0)
                    {
                        textures.Add(textureCoordinates[num]);
                        num = (num + 1) % textureCoordinates.Count;
                    }
                }
                if (Slices > 1)
                {
                    TriangleSubdivide(vertices, normals, indices, textures);
                }
                continue;
            }
            for (int k = 0; k < faces.GetLength(1) - 1; k++)
            {
                int count2 = vertices.Count;
                int num2 = faces.GetLength(1) - 1;
                vertices.Add(faces[i, 0]);
                vertices.Add(faces[i, k + 1]);
                vertices.Add(faces[i, (k + 1) % num2 + 1]);
                if (textureCoordinates != null && textureCoordinates.Count >= faces.GetLength(1))
                {
                    textures.Add(textureCoordinates[num]);
                    textures.Add(textureCoordinates[num + k + 1]);
                    textures.Add(textureCoordinates[num + (k + 1) % num2 + 1]);
                }
                normals.Add(value);
                normals.Add(value);
                normals.Add(value);
                indices.Add(count2);
                indices.Add(count2 + 1);
                indices.Add(count2 + 2);
                if (Slices > 1)
                {
                    TriangleSubdivide(vertices, normals, indices, textures);
                }
            }
            if (textureCoordinates != null && textureCoordinates.Count > 0)
            {
                num = (num + faces.GetLength(1)) % textureCoordinates.Count;
            }
        }
    }
}
