#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class zAxis
{
    [MirrorVerified("PtcPersistentDataObjectszAxisMechanicalDifferentialTests")]
    public zAxis() { }

    [MirrorVerified("PtcPersistentDataObjectszAxisMechanicalDifferentialTests")]
    private xyDomainType zDomainField;

    [MirrorVerified("PtcPersistentDataObjectszAxisMechanicalDifferentialTests")]
    public xyDomainType zDomain { get => zDomainField; set => zDomainField = value; }
}
