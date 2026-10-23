#nullable disable
using Quire.Mirror;

namespace Quire.Xml;

public static partial class XmlWorksheetData
{
    [MirrorVerified("Release IL")]
    public static System.Xml.XmlNode XmlElementFromString(string xml)
    {
        System.Xml.XmlDocument document = new System.Xml.XmlDocument();
        document.LoadXml(xml);
        return document.DocumentElement;
    }
}
