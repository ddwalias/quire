#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial struct ImagePackagingInformation
{
    [MirrorVerified("PtcWpfImagePackagingInformationMechanicalDifferentialTests")]
    public string ImageNameFormat { get; set; }

    [MirrorVerified("PtcWpfImagePackagingInformationMechanicalDifferentialTests")]
    public string ImageId { get; set; }
}
