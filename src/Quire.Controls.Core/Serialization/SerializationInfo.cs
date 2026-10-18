#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class SerializationInfo
{
    [MirrorVerified("PtcControlsCoreSerializationSerializationInfoMechanicalDifferentialTests")]
    public SerializationInfo() { }

    [MirrorVerified("PtcControlsCoreSerializationSerializationInfoMechanicalDifferentialTests")]
    public SerializationFlags Flags { get; set; }

    [MirrorVerified("PtcControlsCoreSerializationSerializationInfoMechanicalDifferentialTests")]
    public object DefaultValue { get; set; }
}
