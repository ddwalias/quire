#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatTypeUnit
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests")]
    private bool formatunitsField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests")]
    public bool formatunits { get => formatunitsField; set => formatunitsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests")]
    private bool simplifyunitsField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests")]
    public bool simplifyunits { get => simplifyunitsField; set => simplifyunitsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests")]
    private bool fractionalunitexponentField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeUnitMechanicalDifferentialTests")]
    public bool fractionalunitexponent { get => fractionalunitexponentField; set => fractionalunitexponentField = value; }
}
