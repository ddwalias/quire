#nullable disable
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Quire.Controls.Core.Serialization;

public partial interface IXmlAndXElementSerializable : IXmlSerializable, IXElementSerializable
{
    void WriteXml(XDocument xDocument);
}
