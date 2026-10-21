#nullable disable
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class XmlHelper
{
    [MirrorVerified("Release IL")]
    public static XmlAttribute AddAttribute(XmlElement node, string name, string value)
    {
        XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
        attribute.Value = value;
        node.Attributes.Append(attribute);
        return attribute;
    }

    [MirrorVerified("Release IL")]
    public static XmlElement GetXmlElementIfLocalNameIsSuitable(XmlNode element, string expectedLocalName)
    {
        if (element == null || !(element.LocalName == expectedLocalName))
        {
            return null;
        }
        return element as XmlElement;
    }

    [MirrorVerified("Release IL")]
    public static XElement XmlNodeToXElement(XmlNode source)
    {
        XDocument document = new XDocument();
        using (XmlWriter writer = document.CreateWriter())
        {
            source.WriteTo(writer);
        }
        return document.Root;
    }

    [MirrorVerified("Release IL")]
    public static XmlNode XElementToXmlNode(XElement source)
    {
        using XmlReader reader = source.CreateReader();
        XmlDocument document = new XmlDocument(reader.NameTable);
        document.Load(reader);
        return document;
    }

    [MirrorVerified("Release IL")]
    public static StreamWriter CreateStreamWriterForXamlCopy(Stream stream, bool leaveOpen) => new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen);
}
