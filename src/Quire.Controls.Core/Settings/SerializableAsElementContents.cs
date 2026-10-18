#nullable disable
using Quire.Controls.Core.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core.Settings;

public partial class SerializableAsElementContents
{
    [MirrorVerified("PtcControlsCoreSettingsSerializableAsElementContentsMechanicalDifferentialTests")]
    private XElementSerializable Serializable { get; set; }
}
