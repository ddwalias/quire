#nullable disable
using System.Security.Principal;
using System.Xml;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class MetadataModel : IMetadataModel
{
    private string _creator;

    private string _lastModified;

    private int _version = 1;

    public string Creator { get => _creator; set => _creator = value; }

    public string LastModified { get => _lastModified; set => _lastModified = value; }

    public int Version { get => _version; set => _version = value; }

    public MetadataModel()
    {
        _creator = WindowsIdentity.GetCurrent().Name;
        _lastModified = _creator;
    }

    [MirrorVerified("PtcControlsCoreSerializationMetadataModelMechanicalDifferentialTests")]
    public MetadataModel(XmlDocument xmlMetadata) : this() { }
}
