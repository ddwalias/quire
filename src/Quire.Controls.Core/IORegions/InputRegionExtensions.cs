#nullable disable
namespace Quire.Controls.Core.IORegions;

public static partial class InputRegionExtensions
{
    public static bool IsSuitableForInputTagging(this IInputRegion region)
    {
        if (region.IsSuitableForTagging())
        {
            return region.IsSuitableForInputTaggingPrivate();
        }
        return false;
    }
}
