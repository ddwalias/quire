#nullable disable
using System.Xml;

namespace Quire.Controls.Core.Serialization;

internal partial interface IXmlElementSerializable
{
    void ReadXml(XmlElement xElement);
    void WriteXml(XmlElement xElement);
}
