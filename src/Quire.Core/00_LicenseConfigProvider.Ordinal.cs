#nullable disable

namespace Quire.License;

public partial class LicenseConfigProvider
{
    // Keep the constructor before HashCodesEqual so its lambda emits Mathcad's <HashCodesEqual>b__8_0 ordinal.
    public static string LicenseConfigFilePath =>
        global::System.IO.Path.Combine(UserSettingsProvider.CommonConfigFolderName, "License.config");

    public static void SetProductCode(string hashCode, string productCode) =>
        throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static void SetProductCode(string hashCode, string productCode, ref global::System.Xml.Linq.XDocument configDoc) =>
        throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static string GetProductCode(global::System.Collections.Generic.IList<string> hashCodes) =>
        throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static string GetProductCode(global::System.Collections.Generic.IList<string> hashCodes, global::System.Xml.Linq.XDocument configDoc) =>
        throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static global::System.Xml.Linq.XElement GetProductCodeElement(
        string hashCode,
        global::System.Xml.Linq.XDocument configDoc,
        out global::System.Xml.Linq.XElement matchingCodesElem) =>
        throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public LicenseConfigProvider()
    {
    }

    public static bool HashCodesEqual(string first, string second)
    {
        if (DebugTools.AssertNotNull(first, "first is null").Failed
            || DebugTools.AssertNotNull(second, "second is null").Failed)
        {
            return false;
        }

        global::System.Func<string, string> normalize = input =>
            global::System.Text.RegularExpressions.Regex.Replace(input, @"\s", string.Empty);
        return normalize(first).Equals(normalize(second), global::System.StringComparison.InvariantCultureIgnoreCase);
    }
}
