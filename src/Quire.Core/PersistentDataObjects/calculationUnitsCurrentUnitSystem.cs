#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculationUnitsCurrentUnitSystem
{
    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCurrentUnitSystemMechanicalDifferentialTests")]
    public calculationUnitsCurrentUnitSystem() { }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCurrentUnitSystemMechanicalDifferentialTests")]
    private unitSystemType nameField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCurrentUnitSystemMechanicalDifferentialTests")]
    public unitSystemType name { get => nameField; set => nameField = value; }
}
