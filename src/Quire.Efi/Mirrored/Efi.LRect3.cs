#nullable disable
#pragma warning disable

namespace Quire.Efi;

public partial class LRect3
{
    public double xmin;

    public double xmax;

    public double ymin;

    public double ymax;

    public double zmin;

    public double zmax;

    public double Dx;

    public double Dy;

    public double Dz;

    public LRect3()
    {
        xmin = (xmax = (ymin = (ymax = (zmin = (zmax = 0.0)))));
    }

    public LRect3(double xmin_, double xmax_, double ymin_, double ymax_, double zmin_, double zmax_)
    {
        xmin = xmin_;
        xmax = xmax_;
        ymin = ymin_;
        ymax = ymax_;
        zmin = zmin_;
        zmax = zmax_;
        Dx = xmax - xmin;
        Dy = ymax - ymin;
        Dz = zmax - zmin;
    }
}
