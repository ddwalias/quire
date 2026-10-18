#nullable disable
using Quire.Mirror;

namespace Quire.ThirdParty;

public partial class DefaultArgumentAttribute
{
    [MirrorVerified("PtcThirdPartyDefaultArgumentAttributeMechanicalDifferentialTests")]
    public DefaultArgumentAttribute(ArgumentType type) : base(type) { }
}
