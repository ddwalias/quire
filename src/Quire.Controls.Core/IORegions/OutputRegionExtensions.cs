#nullable disable
namespace Quire.Controls.Core.IORegions;

public static partial class OutputRegionExtensions
{
    public static bool IsSuitableForOutputTagging(this IOutputRegion region)
    {
        if (region.IsSuitableForTagging())
        {
            return region.IsSuitableForOutputTaggingPrivate();
        }
        return false;
    }
}
