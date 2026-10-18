#nullable disable
using System.Xml.Linq;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class XElementSerializable
{
    [MirrorVerified("PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests")]
    public virtual XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests")]
    public object[] Children { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests")]
    private readonly XName _name;

    [MirrorVerified("PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests")]
    protected XName Name => _name;

    [MirrorVerified("PtcControlsCoreSerializationXElementSerializableMechanicalDifferentialTests")]
    protected XNamespace Namespace => _name.Namespace;
}
