#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCard
{
    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Version { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string FormattedName { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public Name Name { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Title { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Organization { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public Address DeliveryAddress { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string AddressLabel { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public Telephone[] TelephoneList { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Email { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Url { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public byte[] Photo { get; private set; }
}
