#nullable disable
using System.Xml.Linq;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.Core.Settings;

public partial class SerializableAsElementContents : IXElementSerializable
{
    private XElementSerializable Serializable { get; set; }

    public SerializableAsElementContents(XElementSerializable serializable)
    {
        Serializable = serializable;
    }

    public void ReadXml(XElement xElement)
    {
        if (Serializable != null)
        {
            Serializable.ReadElementContents(xElement);
        }
    }

    public void WriteXml(XElement xElement)
    {
        if (Serializable != null)
        {
            Serializable.WriteElementContents(xElement);
        }
    }
}
