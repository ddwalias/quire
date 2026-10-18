#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class cachedWorksheetType
{
    [MirrorVerified("PtcPersistentDatacachedWorksheetTypeMechanicalDifferentialTests")]
    public cachedWorksheetType() { }

    [MirrorVerified("PtcPersistentDatacachedWorksheetTypeMechanicalDifferentialTests")]
    private cachedWorksheetRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDatacachedWorksheetTypeMechanicalDifferentialTests")]
    public cachedWorksheetRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDatacachedWorksheetTypeMechanicalDifferentialTests")]
    private calculation calculationField;

    [MirrorVerified("PtcPersistentDatacachedWorksheetTypeMechanicalDifferentialTests")]
    public calculation calculation { get => calculationField; set => calculationField = value; }
}
