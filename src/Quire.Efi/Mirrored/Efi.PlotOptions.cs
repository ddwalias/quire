#nullable disable
#pragma warning disable

namespace Quire.Efi;

internal partial class PlotOptions
{
    public PlotOptions(int bits)
        : base(bits)
    {
    }

    public bool isPolar()
    {
        return GetBit(0);
    }

    public bool equalScales()
    {
        return GetBit(1);
    }

    public bool hasY2()
    {
        return GetBit(2);
    }
}
