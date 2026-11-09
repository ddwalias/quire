#nullable disable
#pragma warning disable

using Quire.Core;

namespace Quire.Controls.Plots.Presenter;

public partial class AxesTypeToIndex
{
    public static AxesIndex Index(AxesType type)
    {
        switch (type)
        {
            case AxesType.X:
            case AxesType.Angular:
                return AxesIndex.Range;
            case AxesType.Y:
            case AxesType.Radial:
                return AxesIndex.Result;
            default:
                DebugTools.DevAssert(false, null);
                return AxesIndex.None;
        }
    }

    public AxesTypeToIndex()
    {
    }
}
