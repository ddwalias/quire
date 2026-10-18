#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial struct Telephone
{
    [MirrorVerified("PtcVCardTelephoneMechanicalDifferentialTests")]
    public string Number { get; set; }

    [MirrorVerified("PtcVCardTelephoneMechanicalDifferentialTests")]
    public Telephone.Types Type { get; set; }

    [MirrorVerified("PtcVCardTelephoneMechanicalDifferentialTests")]
    public bool IsFax { get; set; }

    [MirrorVerified("PtcVCardTelephoneMechanicalDifferentialTests")]
    public bool IsVoice { get; set; }
}
