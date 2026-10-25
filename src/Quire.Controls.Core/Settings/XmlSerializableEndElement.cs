#nullable disable
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Quire.Controls.Core.Settings;

internal partial class XmlSerializableEndElement : IXmlSerializable
{
    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        if (reader.NodeType == XmlNodeType.EndElement)
        {
            reader.ReadEndElement();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteEndElement();
    }

    public XmlSerializableEndElement() { }
}
