#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class customUnitType
{
    [MirrorVerified("PtcPersistentDataObjectscustomUnitTypeMechanicalDifferentialTests")]
    public customUnitType() { }

    [MirrorVerified("PtcPersistentDataObjectscustomUnitTypeMechanicalDifferentialTests")]
    private string unitnameField;

    [MirrorVerified("PtcPersistentDataObjectscustomUnitTypeMechanicalDifferentialTests")]
    public string unitname { get => unitnameField; set => unitnameField = value; }

    [MirrorVerified("PtcPersistentDataObjectscustomUnitTypeMechanicalDifferentialTests")]
    private string quantitynameField;

    [MirrorVerified("PtcPersistentDataObjectscustomUnitTypeMechanicalDifferentialTests")]
    public string quantityname { get => quantitynameField; set => quantitynameField = value; }
}
