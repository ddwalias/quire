#nullable disable
#pragma warning disable

namespace Quire.Efi;

public partial class shpPoint3D
{
    private double x;

    private double y;

    private double z;

    public shpPoint3D()
    {
        x = 0.0;
        y = 0.0;
        z = 0.0;
    }

    public shpPoint3D(shpPoint3D p)
    {
        X(p.X());
        Y(p.Y());
        Z(p.Z());
    }

    public shpPoint3D(double x, double y, double z)
    {
        X(x);
        Y(y);
        Z(z);
    }

    public double X()
    {
        return x;
    }

    public double Y()
    {
        return y;
    }

    public double Z()
    {
        return z;
    }

    public void X(double x_)
    {
        x = x_;
    }

    public void Y(double y_)
    {
        y = y_;
    }

    public void Z(double z_)
    {
        z = z_;
    }
}
