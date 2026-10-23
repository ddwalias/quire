#nullable disable
using Quire.Mirror;

namespace Quire.Serialization;

public partial class SpiritResolver
{
    public static string SimpleXpathCreator(string prefix, string ns, params string[] parameters)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(prefix);
        foreach (string text in parameters)
        {
            stringBuilder.Append(ns + ":" + text + "/");
        }
        stringBuilder.Remove(stringBuilder.ToString().LastIndexOf("/"), 1);
        return stringBuilder.ToString();
    }

    public static System.Xml.XmlNode WorksheetRootNode(System.Xml.XmlNode xmlContentDocument, System.Xml.XmlNamespaceManager nameSpaceManager, string rootNodeName)
    {
        string xpath = "//ws:" + rootNodeName;
        return xmlContentDocument.SelectSingleNode(xpath, nameSpaceManager);
    }

    [MirrorVerified("PtcSerializationSpiritResolverMechanicalDifferentialTests")]
    private SpiritResolver() : base("Ptc.Schemas.{0}") { }
}
