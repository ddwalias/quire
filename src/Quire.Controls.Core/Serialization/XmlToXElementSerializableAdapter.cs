#nullable disable
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class XmlToXElementSerializableAdapter : IXmlAndXElementSerializable, IXmlSerializable, IXElementSerializable
{
    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    private readonly IXmlSerializable _xmlSerializable;

    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    public XmlToXElementSerializableAdapter(IXmlSerializable xmlSerializable)
    {
        _xmlSerializable = xmlSerializable;
    }

    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => _xmlSerializable.GetSchema();

    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    public void ReadXml(XmlReader reader) => _xmlSerializable.ReadXml(reader);

    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    public void WriteXml(XmlWriter writer) => _xmlSerializable.WriteXml(writer);

    public void ReadXml(XElement xElement)
    {
        using XmlReader reader = xElement.CreateReader();
        ReadXml(reader);
    }

    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    public void WriteXml(XElement xElement) => WriteXmlToXContainer(xElement);

    [MirrorVerified("PtcControlsCoreSerializationXmlToXElementSerializableAdapterMechanicalDifferentialTests")]
    public void WriteXml(XDocument xDocument) => WriteXmlToXContainer(xDocument);

    private void WriteXmlToXContainer(XContainer xContainer)
    {
        using XmlWriter writer = xContainer.CreateWriter();
        WriteXml(writer);
    }
}
