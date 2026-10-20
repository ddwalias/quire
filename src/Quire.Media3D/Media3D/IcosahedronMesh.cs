#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class IcosahedronMesh : PolyhedronMeshBase
{
    private static readonly double G = (1.0 + Math.Sqrt(5.0)) / 2.0;

    private static readonly Point3D[,] faces = new Point3D[20, 3]
    {
        {
            new Point3D(0.0, G, 1.0),
            new Point3D(-1.0, 0.0, G),
            new Point3D(1.0, 0.0, G)
        },
        {
            new Point3D(-1.0, 0.0, G),
            new Point3D(0.0, -G, 1.0),
            new Point3D(1.0, 0.0, G)
        },
        {
            new Point3D(0.0, G, 1.0),
            new Point3D(1.0, 0.0, G),
            new Point3D(G, 1.0, 0.0)
        },
        {
            new Point3D(1.0, 0.0, G),
            new Point3D(G, -1.0, 0.0),
            new Point3D(G, 1.0, 0.0)
        },
        {
            new Point3D(0.0, G, 1.0),
            new Point3D(G, 1.0, 0.0),
            new Point3D(0.0, G, -1.0)
        },
        {
            new Point3D(G, 1.0, 0.0),
            new Point3D(1.0, 0.0, -G),
            new Point3D(0.0, G, -1.0)
        },
        {
            new Point3D(0.0, G, 1.0),
            new Point3D(0.0, G, -1.0),
            new Point3D(-G, 1.0, 0.0)
        },
        {
            new Point3D(0.0, G, -1.0),
            new Point3D(-1.0, 0.0, -G),
            new Point3D(-G, 1.0, 0.0)
        },
        {
            new Point3D(0.0, G, 1.0),
            new Point3D(-G, 1.0, 0.0),
            new Point3D(-1.0, 0.0, G)
        },
        {
            new Point3D(-G, 1.0, 0.0),
            new Point3D(-G, -1.0, 0.0),
            new Point3D(-1.0, 0.0, G)
        },
        {
            new Point3D(1.0, 0.0, G),
            new Point3D(0.0, -G, 1.0),
            new Point3D(G, -1.0, 0.0)
        },
        {
            new Point3D(0.0, -G, 1.0),
            new Point3D(0.0, -G, -1.0),
            new Point3D(G, -1.0, 0.0)
        },
        {
            new Point3D(G, 1.0, 0.0),
            new Point3D(G, -1.0, 0.0),
            new Point3D(1.0, 0.0, -G)
        },
        {
            new Point3D(G, -1.0, 0.0),
            new Point3D(0.0, -G, -1.0),
            new Point3D(1.0, 0.0, -G)
        },
        {
            new Point3D(0.0, G, -1.0),
            new Point3D(1.0, 0.0, -G),
            new Point3D(-1.0, 0.0, -G)
        },
        {
            new Point3D(1.0, 0.0, -G),
            new Point3D(0.0, -G, -1.0),
            new Point3D(-1.0, 0.0, -G)
        },
        {
            new Point3D(-G, 1.0, 0.0),
            new Point3D(-1.0, 0.0, -G),
            new Point3D(-G, -1.0, 0.0)
        },
        {
            new Point3D(-1.0, 0.0, -G),
            new Point3D(0.0, -G, -1.0),
            new Point3D(-G, -1.0, 0.0)
        },
        {
            new Point3D(-1.0, 0.0, G),
            new Point3D(-G, -1.0, 0.0),
            new Point3D(0.0, -G, 1.0)
        },
        {
            new Point3D(-G, -1.0, 0.0),
            new Point3D(0.0, -G, -1.0),
            new Point3D(0.0, -G, 1.0)
        }
    };

    protected override Point3D[,] Faces => faces;

    public IcosahedronMesh()
    {
        PointCollection textureCoordinates = TextureCoordinates;
        TextureCoordinates = null;
        textureCoordinates.Add(new Point(1.0, 0.0));
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        textureCoordinates.Add(new Point(0.0, 0.0));
        textureCoordinates.Add(new Point(0.0, 1.0));
        textureCoordinates.Add(new Point(1.0, 1.0));
        TextureCoordinates = textureCoordinates;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new OctahedronMesh();
    }
}
