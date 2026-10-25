#nullable disable

namespace Quire.Xml;

public static partial class XmlWorksheetData
{
    public static string VersionName
    {
        get
        {
            if (global::Quire.License.LicenseManager.Instance != null
                && (global::Quire.Services.ApplicationVariant.IsEngNb == true
                    || global::Quire.Services.ApplicationVariant.IsEngNbPlus == true))
            {
                return CurrentEngNbVersion.ToString();
            }

            return AppVersionName;
        }
    }
}
