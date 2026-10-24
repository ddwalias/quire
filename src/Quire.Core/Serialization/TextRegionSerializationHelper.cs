#nullable disable
using System.IO;
using System.IO;
using System.IO.Packaging;
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
    public static readonly string[] PTC_CONTROLS_TEXT_NAMESPACE = new string[2]
    {
        "pct",
        "clr-namespace:Ptc.Controls.Text;assembly=Ptc.Controls.Text"
    };

    public static readonly string PAGE_GAP_CONTAINER_NODE_NAME = "PageGapContainer";

    private static readonly System.Version _firstVerToNotPackFlowDocumentImages = new System.Version(3, 0, 0, 0);

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

    public static void CopyImagesFromPackageIntoFlowDocument(XmlNode flowDocumentXaml, IDeS11NProvider d11NProvider, XmlNamespaceManager nsManager)
    {
        if (d11NProvider.Helper.FormatVersion == null || d11NProvider.Helper.FormatVersion < _firstVerToNotPackFlowDocumentImages)
        {
            string xPath = string.Format("//{0}:{1}", "p", "Image");
            foreach (XmlNode item in SelectNodes(flowDocumentXaml, xPath, nsManager))
            {
                string attributeValueAndRemoveIt = GetAttributeValueAndRemoveIt(item, "item-idref");
                if (DebugTools.DevAssert(!string.IsNullOrEmpty(attributeValueAndRemoveIt), null).Passed)
                {
                    PackageRelationship relationship = d11NProvider.PackagePart.GetRelationship(attributeValueAndRemoveIt);
                    string value = Quire.Wpf.ImageUtils.CopyImageToTempBitmap(PackUriHelper.Create(new Uri(Path.GetFullPath(d11NProvider.FilePath), UriKind.Absolute), relationship.TargetUri)).ToString();
                    if (DebugTools.DevAssert(!string.IsNullOrEmpty(value), null).Passed)
                    {
                        ((XmlElement)item).SetAttribute("Source", value);
                    }
                }
            }
        }
    }

    [MirrorVerified("PtcSerializationTextRegionSerializationHelperMechanicalDifferentialTests")]
    public static XmlNodeList SelectNodes(XmlNode parentNode, string xPath, XmlNamespaceManager namespaceManager) => parentNode.SelectNodes(xPath, namespaceManager);

    [MirrorVerified("Release IL")]
    public static TextRangeExtended GetTextRangeFromStartToEnd(FlowDocument flowDocument) => new TextRangeExtended(flowDocument.ContentStart, flowDocument.ContentEnd);
}
