#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class SerializableXmlPackagePart
{
    [MirrorVerified("PtcControlsCoreSerializationSerializableXmlPackagePartMechanicalDifferentialTests")]
    public SerializableXmlPackagePart(IXmlAndXElementSerializable xmlSerializable) : this(xmlSerializable, null) { }
}
