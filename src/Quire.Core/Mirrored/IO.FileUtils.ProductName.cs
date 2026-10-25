#nullable disable

namespace Quire.IO;

public partial class FileUtils
{
    private static string GetProductNamePerVariant()
    {
        if (global::Quire.License.LicenseManager.Instance != null
            && (global::Quire.Services.ApplicationVariant.IsEngNb == true
                || global::Quire.Services.ApplicationVariant.IsEngNbPlus == true))
        {
            return "Engineering Notebook";
        }

        return "Mathcad Prime";
    }
}
