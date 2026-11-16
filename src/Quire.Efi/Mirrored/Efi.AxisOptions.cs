#nullable disable
#pragma warning disable

namespace Quire.Efi;

internal partial class AxisOptions
{
    public double ll;

    public double ul;

    public AxisOptions(int bits)
        : base(bits)
    {
    }

    public bool isULAuto()
    {
        return GetBit(0);
    }

    public bool isLLAuto()
    {
        return GetBit(1);
    }

    public bool isM1Auto()
    {
        return GetBit(2);
    }

    public bool isM2Auto()
    {
        return GetBit(3);
    }

    public bool isAutoscale()
    {
        return GetBit(4);
    }

    public bool isLog()
    {
        return GetBit(5);
    }

    public bool isCategory()
    {
        return GetBit(6);
    }
}
