#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class CubeMesh : PolyhedronMeshBase
{
    private static readonly Point3D[,] faces = new Point3D[6, 5]
    {
        {
            new Point3D(0.0, 0.0, 1.0),
            new Point3D(-1.0, 1.0, 1.0),
            new Point3D(-1.0, -1.0, 1.0),
            new Point3D(1.0, -1.0, 1.0),
            new Point3D(1.0, 1.0, 1.0)
        },
        {
            new Point3D(0.0, 0.0, -1.0),
            new Point3D(1.0, 1.0, -1.0),
            new Point3D(1.0, -1.0, -1.0),
            new Point3D(-1.0, -1.0, -1.0),
            new Point3D(-1.0, 1.0, -1.0)
        },
        {
            new Point3D(-1.0, 0.0, 0.0),
            new Point3D(-1.0, 1.0, -1.0),
            new Point3D(-1.0, -1.0, -1.0),
            new Point3D(-1.0, -1.0, 1.0),
            new Point3D(-1.0, 1.0, 1.0)
        },
        {
            new Point3D(1.0, 0.0, 0.0),
            new Point3D(1.0, 1.0, 1.0),
            new Point3D(1.0, -1.0, 1.0),
            new Point3D(1.0, -1.0, -1.0),
            new Point3D(1.0, 1.0, -1.0)
        },
        {
            new Point3D(0.0, 1.0, 0.0),
            new Point3D(-1.0, 1.0, -1.0),
            new Point3D(-1.0, 1.0, 1.0),
            new Point3D(1.0, 1.0, 1.0),
            new Point3D(1.0, 1.0, -1.0)
        },
        {
            new Point3D(0.0, -1.0, 0.0),
            new Point3D(-1.0, -1.0, 1.0),
            new Point3D(-1.0, -1.0, -1.0),
            new Point3D(1.0, -1.0, -1.0),
            new Point3D(1.0, -1.0, 1.0)
        }
    };

    protected override Point3D[,] Faces => faces;

    public CubeMesh()
    {
        PointCollection textureCoordinates = TextureCoordinates;
        TextureCoordinates = null;
        textureCoordinates.Add(new Point(0.5, 0.5));
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(0.0, 1.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        textureCoordinates.Add(new Point(1.0, 0.0));
        TextureCoordinates = textureCoordinates;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new CubeMesh();
    }
}
