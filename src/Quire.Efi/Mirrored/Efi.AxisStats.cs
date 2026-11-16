#nullable disable
#pragma warning disable

using System;

namespace Quire.Efi;

internal partial class AxisStats
{
    public string[] labels;

    public double lowerLimit;

    public double upperLimit;

    public double marker1;

    public double marker2;

    public double min = double.PositiveInfinity;

    public double max = double.NegativeInfinity;

    public double min1 = double.PositiveInfinity;

    public double max1 = double.NegativeInfinity;

    public double QPmin = Util.MARKER;

    public double QPmax = Util.MARKER;

    public virtual void Adjust(double x, double y, int nCurrentTrace, int nCurrentPoint)
    {
        min = Math.Min(x, min);
        max = Math.Max(x, max);
        if (nCurrentTrace == 0)
        {
            min1 = Math.Min(x, min1);
            max1 = Math.Max(x, max1);
        }
    }
}
