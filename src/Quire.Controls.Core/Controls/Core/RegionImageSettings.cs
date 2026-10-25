#nullable disable
namespace Quire.Controls.Core;

public partial struct RegionImageSettings
{
    public bool ApplyZoom;

    public bool ApplyTransparency;

    public bool ShrinkToFrame;

    public static RegionImageSettings DefaultForRtfCopy;

    public static RegionImageSettings DefaultForImageCopy;

    static RegionImageSettings()
    {
        DefaultForRtfCopy = new RegionImageSettings
        {
            ApplyZoom = false,
            ApplyTransparency = true,
            ShrinkToFrame = false
        };
        DefaultForImageCopy = new RegionImageSettings
        {
            ApplyZoom = true,
            ApplyTransparency = false,
            ShrinkToFrame = true
        };
    }
}
