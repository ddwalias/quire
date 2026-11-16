#nullable disable
#pragma warning disable

namespace Quire.Efi;

internal partial class YAxisStats
{
    public int traces;

    public double SQmin = Util.MARKER;

    public double SQmax = Util.MARKER;

    public double median;

    public double dataQPmin = Util.MARKER;

    public double dataQPmax = Util.MARKER;

    public int nmin1;

    public double nmax1;

    public YAxisStats()
    {
    }

    public override void Adjust(double x, double y, int nCurrentTrace, int nCurrentPoint)
    {
        if (nCurrentTrace == 0)
        {
            if (y < min1)
            {
                nmin1 = nCurrentPoint;
            }
            if (y > max1)
            {
                nmax1 = nCurrentPoint;
            }
        }
        base.Adjust(x, y, nCurrentTrace, nCurrentPoint);
    }
}
