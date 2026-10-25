#nullable disable

namespace Quire.License;

public partial class LicenseConfigProvider
{
    // Keep the constructor before both lambdas so their compiler-generated ordinals match Mathcad.
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

    public static string GetProductCode(global::System.Collections.Generic.IList<string> hashCodes)
    {
        if (!global::System.IO.File.Exists(LicenseConfigFilePath))
        {
            return null;
        }

        try
        {
            return GetProductCode(
                hashCodes,
                global::System.Xml.Linq.XDocument.Load(LicenseConfigFilePath, global::System.Xml.Linq.LoadOptions.None));
        }
        catch (global::System.Exception exception)
        {
            DebugTools.FailAssertion(exception.Message);
            return null;
        }
    }

    public LicenseConfigProvider()
    {
    }

    public static string GetProductCode(global::System.Collections.Generic.IList<string> hashCodes, global::System.Xml.Linq.XDocument configDoc)
    {
        if (DebugTools.DevAssert(
                global::Quire.EnumerableExtensions.IsNotEmpty(hashCodes),
                "hashCode is null or empty").Failed
            || DebugTools.AssertNotNull(configDoc, "configDoc is null").Failed)
        {
            return null;
        }

        global::System.Collections.Generic.Dictionary<string, string> productCodes = new();
        foreach (string hashCode in hashCodes)
        {
            global::System.Xml.Linq.XElement matchingCodesElement;
            global::System.Xml.Linq.XElement productCodeElement =
                GetProductCodeElement(hashCode, configDoc, out matchingCodesElement);
            if (productCodeElement == null)
            {
                continue;
            }

            string productCode = productCodeElement.Value;
            if (!DebugTools.AssertNotNull(
                    productCode,
                    "productCodeElem.Value is null for '{0}'",
                    hashCode).Passed)
            {
                continue;
            }

            if (!productCodes.ContainsKey(productCode))
            {
                productCodes.Add(productCodeElement.Value, hashCode);
            }
        }

        DebugTools.DevAssert(productCodes.Count < 2);
        if (!global::Quire.EnumerableExtensions.IsNullOrEmpty(productCodes))
        {
            return productCodes.Keys.First();
        }

        return null;
    }

    private static global::System.Xml.Linq.XElement GetProductCodeElement(
        string hashCode,
        global::System.Xml.Linq.XDocument configDoc,
        out global::System.Xml.Linq.XElement matchingCodesElem)
    {
        matchingCodesElem = null;
        if (DebugTools.AssertNotNull(configDoc, "configDoc is null").Failed
            || DebugTools.AssertNotNull(configDoc.Root, "root is null").Failed)
        {
            return null;
        }

        global::System.Func<global::System.Xml.Linq.XElement, bool> matches = codeElement =>
        {
            global::System.Xml.Linq.XElement hashCodeElement = codeElement.Element("HashCode");
            if (hashCodeElement == null)
            {
                return false;
            }

            return HashCodesEqual(hashCode, hashCodeElement.Value);
        };

        matchingCodesElem = configDoc.Root.Elements("LicenseCodes").FirstOrDefault(matches);
        if (matchingCodesElem != null)
        {
            return matchingCodesElem.Element("ProductCode");
        }

        return null;
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
