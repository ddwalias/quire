using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class ImageRegionPersistentData : RegionPersistentData
{
    public ImageRegionPersistentData(ImageControl image)
        : base(image)
    {
    }

    protected override void Deserialize()
    {
        base.Deserialize();
        DeserializeDimensions();
    }

    protected override void DeserializeDimensions()
    {
        base.DeserializeDimensions();
        ImageControl imageControl = (ImageControl)Control;
        if (imageControl.Source != null)
        {
            double num = imageControl.Source.Width / imageControl.Source.Height;
            if (RegionData.widthSpecified && !RegionData.heightSpecified)
            {
                imageControl.Height = RegionData.width / num;
            }
            else if (RegionData.heightSpecified && !RegionData.widthSpecified)
            {
                imageControl.Width = RegionData.height * num;
            }
        }
    }
}
