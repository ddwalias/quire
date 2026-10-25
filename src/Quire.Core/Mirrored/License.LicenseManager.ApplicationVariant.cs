#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public void UpdateApplicationVariant()
    {
        int applicationVariant = _provider.UpdateApplicationVariant();
        IsEngNbPlus = applicationVariant == 1;
        IsEngNb = applicationVariant == 2;
        global::Quire.Services.ApplicationVariant.CreateInstance();
    }

    public void LaunchLicenseWizardAtStartupIfNeeded()
    {
        if (Properties.Settings.Default.LicensePathForm == LicenseUriType.Unset)
        {
            LaunchLicenseWizard();
            if (Properties.Settings.Default.LicensePathForm == LicenseUriType.Unset)
            {
                Properties.Settings.Default.LicensePathForm = LicenseUriType.None;
                Properties.Settings.Default.Save();
            }
        }
    }
}
