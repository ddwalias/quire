#nullable disable
using System;
using System.Windows.Media.Imaging;
using Quire.Mirror;

namespace Quire.Wpf;

public partial struct ImagePackagingInformation
{
    public Uri ImageSourceUri;
    public byte[] ImageOriginalStreamHash;
    public BitmapSource Image;
    public const string DefaultImageNameFormat = "Image{0}";
    public string ImageExtension;

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
