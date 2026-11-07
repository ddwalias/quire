#nullable disable
using System.Xml.Linq;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

internal static partial class Integration10Repairer
{
    private const string TargetNamespaceUri = "http://schemas.ptc.com/integration10";

    public static void RepairXDocument(XDocument xDocument)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(xDocument != null && xDocument.Root != null && xDocument.Root.Name.LocalName == "regions", null))
        {
            return;
        }

        if (string.IsNullOrEmpty(xDocument.Root.Name.Namespace.NamespaceName))
        {
            UpdateNameSpace(xDocument);
        }
    }

    private static void UpdateNameSpace(XDocument document)
    {
        XmlHelper.UpdateNameSpace(document.Elements(), TargetNamespaceUri);
    }
}
