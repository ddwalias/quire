#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculationUnitsCustomUnitSystemDerivedUnits
{
    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemDerivedUnitsMechanicalDifferentialTests")]
    public calculationUnitsCustomUnitSystemDerivedUnits() { }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemDerivedUnitsMechanicalDifferentialTests")]
    private customUnitType[] unitField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemDerivedUnitsMechanicalDifferentialTests")]
    public customUnitType[] unit { get => unitField; set => unitField = value; }
}
