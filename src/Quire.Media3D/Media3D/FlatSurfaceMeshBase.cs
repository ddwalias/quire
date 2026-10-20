#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public abstract partial class FlatSurfaceMeshBase : MeshGeneratorBase
{
    private Point3D[] verticesBase = new Point3D[3];
    private Vector3D[] normalsBase = new Vector3D[3];
    private int[] indicesBase = new int[3];
    private Point[] texturesBase = new Point[3];

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register(
        "Slices",
        typeof(int),
        typeof(FlatSurfaceMeshBase),
        new PropertyMetadata((object)1, (PropertyChangedCallback)MeshGeneratorBase.PropertyChanged),
        (ValidateValueCallback)ValidateSlices);

    protected FlatSurfaceMeshBase()
    {
    }

    public int Slices
    {
        get => (int)GetValue(SlicesProperty);
        set => SetValue(SlicesProperty, value);
    }

    private static bool ValidateSlices(object obj)
    {
        return (int)obj > 0;
    }

    protected void TriangleSubdivide(Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        for (int i = 0; i < 3; i++)
        {
            verticesBase[2 - i] = vertices[vertices.Count - 1];
            normalsBase[2 - i] = normals[vertices.Count - 1];
            texturesBase[2 - i] = textures[vertices.Count - 1];
            vertices.RemoveAt(vertices.Count - 1);
            normals.RemoveAt(normals.Count - 1);
            indices.RemoveAt(indices.Count - 1);
            textures.RemoveAt(textures.Count - 1);
        }
        int count = vertices.Count;
        for (int j = 0; j <= Slices; j++)
        {
            double wt = (double)j / (double)Slices;
            Point3D one = Point3DWeight(verticesBase[0], verticesBase[1], wt);
            Point3D two = Point3DWeight(verticesBase[0], verticesBase[2], wt);
            Vector3D one2 = Vector3DWeight(normalsBase[0], normalsBase[1], wt);
            Vector3D two2 = Vector3DWeight(normalsBase[0], normalsBase[2], wt);
            Point one3 = PointWeight(texturesBase[0], texturesBase[1], wt);
            Point two3 = PointWeight(texturesBase[0], texturesBase[2], wt);
            for (int k = 0; k <= j; k++)
            {
                wt = (double)k / (double)j;
                if (double.IsNaN(wt))
                {
                    wt = 0.0;
                }
                vertices.Add(Point3DWeight(one, two, wt));
                normals.Add(Vector3DWeight(one2, two2, wt));
                textures.Add(PointWeight(one3, two3, wt));
            }
        }
        for (int l = 0; l < Slices; l++)
        {
            int num = (l + 1) * l / 2;
            int num2 = num + l + 1;
            for (int m = 0; m <= 2 * l; m++)
            {
                int num3 = m / 2;
                if ((m & 1) == 0)
                {
                    indices.Add(count + num + num3);
                    indices.Add(count + num2 + num3);
                    indices.Add(count + num2 + num3 + 1);
                }
                else
                {
                    indices.Add(count + num + num3);
                    indices.Add(count + num2 + num3 + 1);
                    indices.Add(count + num + num3 + 1);
                }
            }
        }
    }

    private Point3D Point3DWeight(Point3D one, Point3D two, double wt2)
    {
        double num = 1.0 - wt2;
        return new Point3D(num * one.X + wt2 * two.X, num * one.Y + wt2 * two.Y, num * one.Z + wt2 * two.Z);
    }

    private Vector3D Vector3DWeight(Vector3D one, Vector3D two, double wt2)
    {
        double num = 1.0 - wt2;
        return new Vector3D(num * one.X + wt2 * two.X, num * one.Y + wt2 * two.Y, num * one.Z + wt2 * two.Z);
    }

    private Point PointWeight(Point one, Point two, double wt2)
    {
        double num = 1.0 - wt2;
        return new Point(num * one.X + wt2 * two.X, num * one.Y + wt2 * two.Y);
    }
}
