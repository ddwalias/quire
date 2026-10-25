#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    public static global::System.Collections.Generic.IList<string> GetLicenseHashCodesFromFile(string filePath)
    {
        try
        {
            return global::System.IO.File.Exists(filePath)
                ? GetLicenseHashCodesFromText(global::System.IO.File.ReadAllText(filePath))
                : null;
        }
        catch (global::System.Exception exception)
        {
            global::Quire.DebugTools.FailAssertion(exception.Message);
            return null;
        }
    }
}
