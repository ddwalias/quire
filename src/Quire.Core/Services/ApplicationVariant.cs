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

    public static void ApplyIsEngNbVariants()
    {
        IsEngNbPlus = global::Quire.License.LicenseManager.Instance != null
            && global::Quire.License.LicenseManager.Instance.IsEngNbPlus;
        IsEngNb = global::Quire.License.LicenseManager.Instance != null
            && global::Quire.License.LicenseManager.Instance.IsEngNb;
        CurrentApplicationType = IsEngNbPlus == true
            ? ApplicationType.EngNBPlus
            : IsEngNb == true ? ApplicationType.EngNB : ApplicationType.MathcadOnPrem;
    }
}
