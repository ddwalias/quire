#nullable disable
using System.Xml.Linq;
using Quire.Xml;

namespace Quire.Controls.Core.Settings;

internal static partial class Calculation10Repairer
{
    private const string _sourceNamespaceURI = "http://schemas.ptc.com/mathcad/settings/calculation10";

    private const string _targetNamespaceURI = "http://schemas.ptc.com/mathcad/settings/calculation10";

    public static void RepairXDocument(XDocument xDocument)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(xDocument != null && xDocument.Root != null && xDocument.Root.Name.LocalName == "calculation" && xDocument.Root.Name.Namespace.NamespaceName == _sourceNamespaceURI, null))
        {
            return;
        }

        UpdateNodeValue(xDocument);
    }

    private static void UpdateNodeValue(XDocument document)
    {
        XmlHelper.UpdateToLowerCase(document.Descendants(XName.Get("calculationBehavior", _targetNamespaceURI)), new string[]
        {
            "automatic-recalculation",
            "strings-use-origin",
            "matrix-strict-singularity-check",
            "optimize-expressions",
            "exact-boolean",
            "units-in-symbolics",
            "constants-in-symbolics",
            "bin-oct-hex-mode"
        });
        XmlHelper.UpdateToLowerCase(document.Descendants(XName.Get("currentUnitSystem", _targetNamespaceURI)), new string[] { "name" });
        XmlHelper.UpdateToLowerCase(new XElement[] { document.Root }, new string[] { "cache-worksheet" });
    }
}
