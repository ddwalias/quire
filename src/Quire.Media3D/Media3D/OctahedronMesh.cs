#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class OctahedronMesh : PolyhedronMeshBase
{
    private static readonly Point3D[,] faces = new Point3D[8, 3]
    {
        {
            new Point3D(0.0, 1.0, 0.0),
            new Point3D(0.0, 0.0, 1.0),
            new Point3D(1.0, 0.0, 0.0)
        },
        {
            new Point3D(0.0, 1.0, 0.0),
            new Point3D(-1.0, 0.0, 0.0),
            new Point3D(0.0, 0.0, 1.0)
        },
        {
            new Point3D(0.0, -1.0, 0.0),
            new Point3D(0.0, 0.0, 1.0),
            new Point3D(-1.0, 0.0, 0.0)
        },
        {
            new Point3D(0.0, -1.0, 0.0),
            new Point3D(1.0, 0.0, 0.0),
            new Point3D(0.0, 0.0, 1.0)
        },
        {
            new Point3D(0.0, -1.0, 0.0),
            new Point3D(0.0, 0.0, -1.0),
            new Point3D(1.0, 0.0, 0.0)
        },
        {
            new Point3D(0.0, -1.0, 0.0),
            new Point3D(-1.0, 0.0, 0.0),
            new Point3D(0.0, 0.0, -1.0)
        },
        {
            new Point3D(0.0, 1.0, 0.0),
            new Point3D(0.0, 0.0, -1.0),
            new Point3D(-1.0, 0.0, 0.0)
        },
        {
            new Point3D(0.0, 1.0, 0.0),
            new Point3D(1.0, 0.0, 0.0),
            new Point3D(0.0, 0.0, -1.0)
        }
    };

    protected override Point3D[,] Faces => faces;

    public OctahedronMesh()
    {
        PointCollection textureCoordinates = TextureCoordinates;
        TextureCoordinates = null;
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        textureCoordinates.Add(new Point(1.0, 0.0));
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(0.0, 1.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        textureCoordinates.Add(new Point(0.0, 1.0));
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(1.0, 0.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        TextureCoordinates = textureCoordinates;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new OctahedronMesh();
    }
}
