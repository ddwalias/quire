#nullable disable
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D.Wires;

public abstract partial class WirePointBase : WireBase
{
    protected static SphereMesh Mesh;

    static WirePointBase()
    {
        Mesh = new SphereMesh
        {
            Center = new Point3D(0.0, 0.0, 0.0),
            Radius = 1.0
        };
        Mesh.Freeze();
    }

    protected double GetRadiusScale(Point3D center)
    {
        double num = Math.Sqrt(Thickness);
        return (Widen(center, new Vector(num, num)) - center).Length / Mesh.Radius;
    }

    protected WirePointBase()
    {
    }
}
