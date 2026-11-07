#nullable disable
using System.Xml.Linq;
using Quire.Xml;

namespace Quire.Controls.Core.Serialization;

internal static partial class Result10Repairer
{
    private const string TargetNamespaceUri = "http://schemas.mathsoft.com/result10";

    public static void RepairXDocument(XDocument xDocument)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(xDocument != null && xDocument.Root != null && xDocument.Root.Name.LocalName == "resultsList", null))
        {
            return;
        }

        if (string.IsNullOrEmpty(xDocument.Root.Name.Namespace.NamespaceName))
        {
            UpdateNameSpace(xDocument);
            UpdateResultNode(xDocument);
        }
    }

    private static void UpdateNameSpace(XDocument document)
    {
        XmlHelper.UpdateNameSpace(document.Elements(), TargetNamespaceUri);
    }

    private static void UpdateResultNode(XDocument document)
    {
        foreach (XElement item in document.Descendants(ResultDataSerializer.GetXNameWithResultNameSpace("result")))
        {
            item.Name = XName.Get(item.Name.LocalName, "http://schemas.mathsoft.com/math50");
        }
    }
}
