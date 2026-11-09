#nullable disable

namespace Quire.Properties;

public static partial class SettingsHelper
{
    // Compile order places fields in Generated/ ahead of Properties/SettingsHelper.cs to preserve Mathcad closure ordinals.
    public static readonly string LicenseSettingsNamespace = typeof(Settings).FullName;
    public const string LicenseType = "LicenseType";
    public const string LicensePathForm = "LicensePathForm";
    public const string LicensePath = "LicensePath";
}
