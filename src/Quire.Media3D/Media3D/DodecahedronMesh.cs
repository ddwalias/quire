#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class DodecahedronMesh : PolyhedronMeshBase
{
    private static readonly double G = (1.0 + Math.Sqrt(5.0)) / 2.0;
    private static readonly double H = 1.0 / G;
    private static readonly double A = (3.0 * H + 4.0) / 5.0;
    private static readonly double B = (2.0 + G) / 5.0;

    private static readonly Point3D[,] faces = new Point3D[12, 6]
    {
        {
            new Point3D(A, -B, 0.0),
            new Point3D(1.0, -1.0, -1.0),
            new Point3D(G, 0.0, -H),
            new Point3D(G, 0.0, H),
            new Point3D(1.0, -1.0, 1.0),
            new Point3D(H, -G, 0.0)
        },
        {
            new Point3D(-A, -B, 0.0),
            new Point3D(-1.0, -1.0, 1.0),
            new Point3D(-G, 0.0, H),
            new Point3D(-G, 0.0, -H),
            new Point3D(-1.0, -1.0, -1.0),
            new Point3D(-H, -G, 0.0)
        },
        {
            new Point3D(-A, B, 0.0),
            new Point3D(-1.0, 1.0, -1.0),
            new Point3D(-G, 0.0, -H),
            new Point3D(-G, 0.0, H),
            new Point3D(-1.0, 1.0, 1.0),
            new Point3D(-H, G, 0.0)
        },
        {
            new Point3D(A, B, 0.0),
            new Point3D(1.0, 1.0, 1.0),
            new Point3D(G, 0.0, H),
            new Point3D(G, 0.0, -H),
            new Point3D(1.0, 1.0, -1.0),
            new Point3D(H, G, 0.0)
        },
        {
            new Point3D(-B, 0.0, -A),
            new Point3D(-1.0, 1.0, -1.0),
            new Point3D(0.0, H, -G),
            new Point3D(0.0, -H, -G),
            new Point3D(-1.0, -1.0, -1.0),
            new Point3D(-G, 0.0, -H)
        },
        {
            new Point3D(-B, 0.0, A),
            new Point3D(-1.0, -1.0, 1.0),
            new Point3D(0.0, -H, G),
            new Point3D(0.0, H, G),
            new Point3D(-1.0, 1.0, 1.0),
            new Point3D(-G, 0.0, H)
        },
        {
            new Point3D(B, 0.0, -A),
            new Point3D(1.0, -1.0, -1.0),
            new Point3D(0.0, -H, -G),
            new Point3D(0.0, H, -G),
            new Point3D(1.0, 1.0, -1.0),
            new Point3D(G, 0.0, -H)
        },
        {
            new Point3D(B, 0.0, A),
            new Point3D(1.0, 1.0, 1.0),
            new Point3D(0.0, H, G),
            new Point3D(0.0, -H, G),
            new Point3D(1.0, -1.0, 1.0),
            new Point3D(G, 0.0, H)
        },
        {
            new Point3D(0.0, -A, -B),
            new Point3D(1.0, -1.0, -1.0),
            new Point3D(H, -G, 0.0),
            new Point3D(-H, -G, 0.0),
            new Point3D(-1.0, -1.0, -1.0),
            new Point3D(0.0, -H, -G)
        },
        {
            new Point3D(0.0, A, -B),
            new Point3D(-1.0, 1.0, -1.0),
            new Point3D(-H, G, 0.0),
            new Point3D(H, G, 0.0),
            new Point3D(1.0, 1.0, -1.0),
            new Point3D(0.0, H, -G)
        },
        {
            new Point3D(0.0, -A, B),
            new Point3D(-1.0, -1.0, 1.0),
            new Point3D(-H, -G, 0.0),
            new Point3D(H, -G, 0.0),
            new Point3D(1.0, -1.0, 1.0),
            new Point3D(0.0, -H, G)
        },
        {
            new Point3D(0.0, A, B),
            new Point3D(1.0, 1.0, 1.0),
            new Point3D(H, G, 0.0),
            new Point3D(-H, G, 0.0),
            new Point3D(-1.0, 1.0, 1.0),
            new Point3D(0.0, H, G)
        }
    };

    protected override Point3D[,] Faces => faces;

    public DodecahedronMesh()
    {
        PointCollection textureCoordinates = TextureCoordinates;
        TextureCoordinates = null;
        textureCoordinates.Add(new Point(0.5, 0.5));
        textureCoordinates.Add(new Point(0.5, 0.0));
        textureCoordinates.Add(new Point(1.0, 0.4));
        textureCoordinates.Add(new Point(0.85, 1.0));
        textureCoordinates.Add(new Point(0.15, 1.0));
        textureCoordinates.Add(new Point(0.0, 0.4));
        TextureCoordinates = textureCoordinates;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new OctahedronMesh();
    }
}
