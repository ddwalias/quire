#nullable disable
using System.Xml;
using System.Xml.Schema;
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Serialization;
using Quire.Xml;
using Quire.Xml;

namespace Quire.Core.Serialization;

public partial class ApplicationPropertiesXmlSerializable
{
    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    private readonly ISerializationHelper _helper;

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public ApplicationPropertiesXmlSerializable(ISerializationHelper helper)
    {
        _helper = helper;
    }

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public static bool IsFormatVersionCompatible(Version fileFormatVersion) => XmlWorksheetData.CurrentFormatVersion >= fileFormatVersion;

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public bool ShouldValidateOnWrite => true;

    [MirrorVerified("PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests")]
    public bool ShouldValidateOnRead => true;

    [MirrorVerified("Release IL")]
    public XmlSchema GetSchema() => SchemasManager.App.Schema;

    [MirrorVerified("Release IL")]
    private static propertiesType DeserializeAppProperties(XmlReader reader) => (propertiesType)propertiesType.XmlSerializer.Deserialize(reader);
}
