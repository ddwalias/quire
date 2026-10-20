#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Services;

public partial class ApplicationVariant
{
    [MirrorVerified("PtcServicesApplicationVariantMechanicalDifferentialTests")]
    public ApplicationVariant() { }

    [MirrorVerified("PtcServicesApplicationVariantMechanicalDifferentialTests")]
    public static ApplicationType CurrentApplicationType { get; set; }

    [MirrorVerified("Release IL")]
    public static ApplicationVariant Instance;

    [MirrorVerified("Release IL")]
    public static void CreateInstance()
    {
        Instance = new ApplicationVariant();
        ApplyIsEngNbVariants();
    }
}
