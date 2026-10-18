#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculationUnits
{
    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests")]
    public calculationUnits() { }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests")]
    private calculationUnitsCustomUnitSystem customUnitSystemField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests")]
    public calculationUnitsCustomUnitSystem customUnitSystem { get => customUnitSystemField; set => customUnitSystemField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests")]
    private calculationUnitsCurrentUnitSystem currentUnitSystemField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsMechanicalDifferentialTests")]
    public calculationUnitsCurrentUnitSystem currentUnitSystem { get => currentUnitSystemField; set => currentUnitSystemField = value; }
}
