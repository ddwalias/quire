#nullable disable
using Quire.FunctionalitiesLimitation;
using Quire.Mirror;

namespace Quire.Services;

public partial class FunctionalityService
{
    [MirrorVerified("Release IL")]
    private readonly FunctionalitiesLimitationDictionary _functionalitiesLimitationDictionary;

    [MirrorVerified("Release IL")]
    private FunctionalityService()
    {
        _functionalitiesLimitationDictionary = new FunctionalitiesLimitationDictionary();
    }

    public bool IsFunctionalityEnabled(EnumFunctionalityKey functionality)
    {
        FunctionalityItem functionalityItem = _functionalitiesLimitationDictionary[functionality] as FunctionalityItem;
        if (global::Quire.AssertionE.Failed
            == (global::Quire.AssertionE)global::Quire.DebugTools.AssertNotNull(
                functionalityItem,
                global::System.String.Format(
                    "FunctionalityItem with the {0} key was not found in the Functionality Dictionary.",
                    functionality),
                global::System.Array.Empty<object>()))
        {
            return true;
        }

        if (global::Quire.License.LicenseManager.Instance == null)
        {
            return true;
        }

        global::Quire.EnumHelper.TryParse<global::Quire.License.LicenseTypeBitFlags>(
            global::Quire.License.LicenseManager.Instance.LicenseType,
            out global::Quire.License.LicenseTypeBitFlags licenseType);
        return (licenseType & functionalityItem.UnsupportedLicenseTypes) == 0;
    }

    public bool IsFunctionalityHasUnsupportedLicenseTypes(EnumFunctionalityKey functionality)
    {
        FunctionalityItem functionalityItem = _functionalitiesLimitationDictionary[functionality] as FunctionalityItem;
        if (global::Quire.AssertionE.Failed
            == (global::Quire.AssertionE)global::Quire.DebugTools.AssertNotNull(
                functionalityItem,
                global::System.String.Format(
                    "FunctionalityItem with the {0} key was not found in the Functionality Dictionary.",
                    functionality),
                global::System.Array.Empty<object>()))
        {
            return true;
        }

        return functionalityItem.UnsupportedLicenseTypes != 0;
    }
}
