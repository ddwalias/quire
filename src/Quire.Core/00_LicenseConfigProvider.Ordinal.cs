#nullable disable

namespace Quire.License;

public partial class LicenseConfigProvider
{
    // Keep the constructor before HashCodesEqual so its lambda emits Mathcad's <HashCodesEqual>b__8_0 ordinal.
    public static string LicenseConfigFilePath =>
        global::System.IO.Path.Combine(UserSettingsProvider.CommonConfigFolderName, "License.config");

    public static void SetProductCode(string hashCode, string productCode)
    {
        try
        {
            global::System.Xml.Linq.XDocument configDocument = global::System.IO.File.Exists(LicenseConfigFilePath)
                ? global::System.Xml.Linq.XDocument.Load(LicenseConfigFilePath, global::System.Xml.Linq.LoadOptions.None)
                : null;
            SetProductCode(hashCode, productCode, ref configDocument);
            configDocument.Save(LicenseConfigFilePath);
        }
        catch (global::System.Exception exception)
        {
            DebugTools.FailAssertion(exception.Message);
        }
    }

    public static void SetProductCode(string hashCode, string productCode, ref global::System.Xml.Linq.XDocument configDoc)
    {
        if (configDoc == null)
        {
            configDoc = new global::System.Xml.Linq.XDocument(new global::System.Xml.Linq.XElement("LicenseConfig"));
        }

        global::System.Xml.Linq.XElement matchingCodesElement;
        global::System.Xml.Linq.XElement productCodeElement =
            GetProductCodeElement(hashCode, configDoc, out matchingCodesElement);
        if (productCodeElement != null)
        {
            string previousProductCode = productCodeElement.Value;
            productCodeElement.Value = productCode;
            DebugTools.DevAssert(previousProductCode == productCode,
                "ProductCode doesn't match: was {0}, now {1}", previousProductCode, productCode);
            return;
        }

        if (matchingCodesElement != null)
        {
            matchingCodesElement.SetElementValue("ProductCode", productCode);
            DebugTools.FailAssertion("No ProductCode element");
            return;
        }

        if (DebugTools.AssertNotNull(configDoc.Root).Passed)
        {
            matchingCodesElement = new global::System.Xml.Linq.XElement("LicenseCodes");
            matchingCodesElement.SetElementValue("HashCode", hashCode);
            matchingCodesElement.SetElementValue("ProductCode", productCode);
            configDoc.Root.Add(matchingCodesElement);
        }
    }

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
