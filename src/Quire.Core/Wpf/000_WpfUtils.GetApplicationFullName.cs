#nullable disable

namespace Quire.Wpf;

// This filename keeps the WpfUtils member ordinals aligned with Mathcad's generated helpers.
public static partial class WpfUtils
{
    public static string GetApplicationFullName(global::Quire.License.LicenseType licenseType, global::System.Windows.FrameworkElement resourcesElement)
    {
        if (resourcesElement == null)
        {
            return null;
        }

        string appVersionName = global::Quire.Xml.XmlWorksheetData.AppVersionName;
        if (global::Quire.License.LicenseManager.Instance != null)
        {
            if (global::Quire.Services.ApplicationVariant.IsEngNbPlus == true)
            {
                return "Creo+ Engineering Notebook";
            }
            if (global::Quire.Services.ApplicationVariant.IsEngNb == true)
            {
                return "Creo Engineering Notebook";
            }
        }

        string result;
        switch (licenseType)
        {
            case global::Quire.License.LicenseType.NoLicense:
            case global::Quire.License.LicenseType.Trial:
                string expressVersionName = resourcesElement.TryFindResource("Application.ExpressVersionName") as string;
                result = string.Join(" ", (System.ReadOnlySpan<string>)["PTC Mathcad", expressVersionName, appVersionName]);
                break;
            case global::Quire.License.LicenseType.EducationSub:
            case global::Quire.License.LicenseType.EducationTerm:
            case global::Quire.License.LicenseType.Education:
                string academicName = resourcesElement.TryFindResource("Application.AcademicNameAndVersion") as string;
                result = string.Join(" ", (System.ReadOnlySpan<string>)["PTC Mathcad", appVersionName, academicName]);
                break;
            case global::Quire.License.LicenseType.CommercialSub:
            case global::Quire.License.LicenseType.CommercialTerm:
            case global::Quire.License.LicenseType.Commercial:
                result = string.Join(" ", (System.ReadOnlySpan<string>)["PTC Mathcad", appVersionName]);
                break;
            default:
                result = string.Join(" ", (System.ReadOnlySpan<string>)["PTC Mathcad", appVersionName]);
                DebugTools.FailAssertion(licenseType.ToString());
                break;
        }
        return result;
    }
}
