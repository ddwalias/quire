#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class MetadataModel
{
    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    public MetadataModel(XmlDocument xmlMetadata) : this() { }

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    private string _creator;

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    public string Creator { get => _creator; set => _creator = value; }

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    private string _lastModified;

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    public string LastModified { get => _lastModified; set => _lastModified = value; }

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    private int _version;

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    public int Version { get => _version; set => _version = value; }
}
