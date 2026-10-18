#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class XmlWorksheetData
{
    [MirrorVerified("PtcXmlXmlWorksheetDataMechanicalDifferentialTests")]
    public static XmlElement CreateMathcad14MLElement(XmlDocument document, string localName) => CreateMLElement(document, localName, "http://schemas.mathsoft.com/math30");

    [MirrorVerified("PtcXmlXmlWorksheetDataMechanicalDifferentialTests")]
    public static XmlElement CreateMLElement(XmlDocument document, string localName, string namespaceURI) => document.CreateElement("ml", localName, namespaceURI);

    [MirrorVerified("PtcXmlXmlWorksheetDataMechanicalDifferentialTests")]
    public static XmlElement CreateMcdxMLElement(XmlDocument document, string localName) => CreateMLElement(document, localName, "http://schemas.mathsoft.com/math50");
}
