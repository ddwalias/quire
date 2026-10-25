#nullable disable
using System.Windows;

namespace Quire.Controls.Core.IORegions;

public static partial class IORegionExtensions
{
    public static bool IsSuitableForTagging(this IIORegion region)
    {
        if (region == null)
        {
            return false;
        }
        if (region is UIElement elem)
        {
            return AttachedProperties.GetSupportsInputsOutputs(elem);
        }
        return true;
    }
}
