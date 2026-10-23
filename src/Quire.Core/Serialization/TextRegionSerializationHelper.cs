#nullable disable
using System.IO;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows;
using System.Xml;
using System.Xml;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Serialization;

public static partial class TextRegionSerializationHelper
{
    public static XmlNamespaceManager CreateNamespaceManagerForTextRegions(XmlNode root)
    {
        XmlDocument xmlDocument = (root as XmlDocument) ?? root.OwnerDocument;
        XmlNamespaceManager xmlNamespaceManager;
        if (xmlDocument == null)
        {
            xmlNamespaceManager = null;
        }
        else
        {
            xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("p", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            xmlNamespaceManager.AddNamespace(PTC_CONTROLS_TEXT_NAMESPACE[0], PTC_CONTROLS_TEXT_NAMESPACE[1]);
        }
        return xmlNamespaceManager;
    }

    private static string GetAttributeValueAndRemoveIt(XmlNode xmlNode, string name)
    {
        string result;
        if (xmlNode.Attributes != null)
        {
            XmlAttribute xmlAttribute = xmlNode.Attributes[name];
            if (xmlAttribute != null)
            {
                result = xmlAttribute.Value;
                xmlNode.Attributes.Remove(xmlAttribute);
            }
            else
            {
                result = null;
            }
        }
        else
        {
            result = null;
        }
        return result;
    }

    [MirrorVerified("PtcSerializationTextRegionSerializationHelperMechanicalDifferentialTests")]
    public static void SaveTextRangeAsXamlPackage(TextRangeExtended src, Stream stream, bool isConversion = false) => src.Save(stream, DataFormats.XamlPackage, true, isConversion);

    [MirrorVerified("PtcSerializationTextRegionSerializationHelperMechanicalDifferentialTests")]
    public static XmlNodeList SelectNodes(XmlNode parentNode, string xPath, XmlNamespaceManager namespaceManager) => parentNode.SelectNodes(xPath, namespaceManager);

    [MirrorVerified("Release IL")]
    public static TextRangeExtended GetTextRangeFromStartToEnd(FlowDocument flowDocument) => new TextRangeExtended(flowDocument.ContentStart, flowDocument.ContentEnd);
}
