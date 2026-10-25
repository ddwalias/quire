#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public void LaunchLicenseWizard()
    {
        string args = global::System.String.Format(
            "/culture:{0} /hostId:{1} {2}",
            global::System.Globalization.CultureInfo.CurrentUICulture.Name,
            HostId,
            LicenseType == LicenseType.Trial ? " /t-" : " /l:trial");
        LaunchExternalLicenseProcess("LicenseWizard.exe", args);
    }
}
