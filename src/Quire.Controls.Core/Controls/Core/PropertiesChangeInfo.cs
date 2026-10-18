#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

internal partial class PropertiesChangeInfo<TSource>
{
    [MirrorVerified("PtcControlsCorePropertiesChangeInfo1MechanicalDifferentialTests")]
    public string[] PropertyNames { get; protected set; }

    [MirrorVerified("PtcControlsCorePropertiesChangeInfo1MechanicalDifferentialTests")]
    public TSource PropertiesSource { get; protected set; }
}
