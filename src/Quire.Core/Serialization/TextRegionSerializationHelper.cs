#nullable disable
using System.IO;
using System.Windows;
using System.Xml;
using Quire.Mirror;

namespace Quire.Serialization;

public static partial class TextRegionSerializationHelper
{
    [MirrorVerified("PtcSerializationTextRegionSerializationHelperMechanicalDifferentialTests")]
    public static void SaveTextRangeAsXamlPackage(TextRangeExtended src, Stream stream, bool isConversion = false) => src.Save(stream, DataFormats.XamlPackage, true, isConversion);

    [MirrorVerified("PtcSerializationTextRegionSerializationHelperMechanicalDifferentialTests")]
    public static XmlNodeList SelectNodes(XmlNode parentNode, string xPath, XmlNamespaceManager namespaceManager) => parentNode.SelectNodes(xPath, namespaceManager);
}
