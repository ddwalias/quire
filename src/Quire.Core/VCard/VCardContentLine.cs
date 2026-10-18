#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCardContentLine
{
    [MirrorVerified("PtcVCardVCardContentLineMechanicalDifferentialTests")]
    public string Name { get; private set; }

    [MirrorVerified("PtcVCardVCardContentLineMechanicalDifferentialTests")]
    public Dictionary<string, string> Params { get; private set; }

    [MirrorVerified("PtcVCardVCardContentLineMechanicalDifferentialTests")]
    public string[] Values { get; private set; }
}
