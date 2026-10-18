#nullable disable
using System.Xml;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

internal partial class XmlSerializableEndElement
{
    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableEndElementMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableEndElementMechanicalDifferentialTests")]
    public void WriteXml(XmlWriter writer) => writer.WriteEndElement();

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableEndElementMechanicalDifferentialTests")]
    public XmlSerializableEndElement() { }
}
