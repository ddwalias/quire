#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial struct ImagePackagingInformation
{
    [MirrorVerified("PtcWpfImagePackagingInformationMechanicalDifferentialTests")]
    public string ImageNameFormat { get; set; }

    [MirrorVerified("PtcWpfImagePackagingInformationMechanicalDifferentialTests")]
    public string ImageId { get; set; }

    [MirrorVerified("Release IL")]
    public string ImageName
    {
        get
        {
            if (string.IsNullOrEmpty(ImageNameFormat))
            {
                ImageNameFormat = "Image{0}";
            }
            return string.Format(ImageNameFormat, ImageId);
        }
    }
}
