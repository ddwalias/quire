#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class calculationUnitsCustomUnitSystemBaseUnits
{
    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemBaseUnitsMechanicalDifferentialTests")]
    public calculationUnitsCustomUnitSystemBaseUnits() { }

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemBaseUnitsMechanicalDifferentialTests")]
    private customUnitType[] unitField;

    [MirrorVerified("PtcPersistentDataObjectscalculationUnitsCustomUnitSystemBaseUnitsMechanicalDifferentialTests")]
    public customUnitType[] unit { get => unitField; set => unitField = value; }
}
