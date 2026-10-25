#nullable disable
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Quire.Controls.Core.Settings;

internal partial class XmlSerializableStartElement : IXmlSerializable
{
    private readonly string prefix;

    private readonly string localName;

    private readonly string ns;

    public XmlSerializableStartElement(string prefix, string localName, string ns)
    {
        this.prefix = prefix;
        this.ns = ns;
        this.localName = localName;
    }

    public XmlSerializableStartElement(string localName)
        : this(string.Empty, localName, string.Empty)
    {
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        if (reader.NodeType == XmlNodeType.None)
        {
            reader.Read();
        }
        reader.ReadToDescendant(localName, ns);
        if (!reader.IsStartElement(localName, ns))
        {
            throw new XmlSchemaValidationException(string.Format("{0} element not found", localName));
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(prefix, localName, ns);
    }
}
