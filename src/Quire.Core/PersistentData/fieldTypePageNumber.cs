#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class fieldTypePageNumber
{
    [MirrorVerified("PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests")]
    public fieldTypePageNumber() { }

    [MirrorVerified("PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests")]
    private pageNumberTemplate templateField;

    [MirrorVerified("PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests")]
    public pageNumberTemplate template { get => templateField; set => templateField = value; }

    [MirrorVerified("PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests")]
    private bool templateFieldSpecified;

    [MirrorVerified("PtcPersistentDatafieldTypePageNumberMechanicalDifferentialTests")]
    public bool templateSpecified { get => templateFieldSpecified; set => templateFieldSpecified = value; }
}
