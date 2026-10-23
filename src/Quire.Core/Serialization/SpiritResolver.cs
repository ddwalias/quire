#nullable disable
using Quire.Mirror;

namespace Quire.Serialization;

public partial class SpiritResolver
{
    public static System.Xml.XmlNode WorksheetRootNode(System.Xml.XmlNode xmlContentDocument, System.Xml.XmlNamespaceManager nameSpaceManager, string rootNodeName)
    {
        string xpath = "//ws:" + rootNodeName;
        return xmlContentDocument.SelectSingleNode(xpath, nameSpaceManager);
    }

    [MirrorVerified("PtcSerializationSpiritResolverMechanicalDifferentialTests")]
    private SpiritResolver() : base("Ptc.Schemas.{0}") { }
}
