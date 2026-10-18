#nullable disable
using Quire.Mirror;

namespace Quire.Services;

public partial class ApplicationVariant
{
    [MirrorVerified("PtcServicesApplicationVariantMechanicalDifferentialTests")]
    public ApplicationVariant() { }

    [MirrorVerified("PtcServicesApplicationVariantMechanicalDifferentialTests")]
    public static ApplicationType CurrentApplicationType { get; set; }
}
