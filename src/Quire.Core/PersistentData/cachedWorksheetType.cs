#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;
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

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<cachedWorksheetRegionType>(Enumerable.Cast<cachedWorksheetRegionType>(value)); }
}
