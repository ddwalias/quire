#nullable disable
using System.Xml;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

internal partial class XmlSerializableStartElement
{
    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableStartElementMechanicalDifferentialTests")]
    public XmlSerializableStartElement(string localName) : this(string.Empty, localName, string.Empty) { }

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableStartElementMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableStartElementMechanicalDifferentialTests")]
    private readonly string prefix;

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableStartElementMechanicalDifferentialTests")]
    private readonly string localName;

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableStartElementMechanicalDifferentialTests")]
    private readonly string ns;

    [MirrorVerified("PtcControlsCoreSettingsXmlSerializableStartElementMechanicalDifferentialTests")]
    public void WriteXml(XmlWriter writer) => writer.WriteStartElement(prefix, localName, ns);
}
