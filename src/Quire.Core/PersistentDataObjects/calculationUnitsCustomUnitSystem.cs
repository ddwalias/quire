#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculationUnitsCustomUnitSystem
{
    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    public calculationUnitsCustomUnitSystem() { }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    private calculationUnitsCustomUnitSystemBaseUnits baseUnitsField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    public calculationUnitsCustomUnitSystemBaseUnits baseUnits { get => baseUnitsField; set => baseUnitsField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    private calculationUnitsCustomUnitSystemDerivedUnits derivedUnitsField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    public calculationUnitsCustomUnitSystemDerivedUnits derivedUnits { get => derivedUnitsField; set => derivedUnitsField = value; }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    private unitSystemType baseunitsystemField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemMechanicalDifferentialTests")]
    public unitSystemType baseunitsystem { get => baseunitsystemField; set => baseunitsystemField = value; }
}
