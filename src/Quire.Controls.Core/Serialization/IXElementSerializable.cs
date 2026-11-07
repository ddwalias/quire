#nullable disable
using System.Xml.Linq;

namespace Quire.Controls.Core.Serialization;

public partial interface IXElementSerializable
{
    void ReadXml(XElement xElement);
    void WriteXml(XElement xElement);
}
