#nullable disable
using Quire.Mirror;
using Quire.Serialization;
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
}
