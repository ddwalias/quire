#nullable disable

namespace Quire.License;

public partial class LicenseConfigProvider
{
    public static string LicenseConfigFilePath =>
        global::System.IO.Path.Combine(UserSettingsProvider.CommonConfigFolderName, "License.config");
}
