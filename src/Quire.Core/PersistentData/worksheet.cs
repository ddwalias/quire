#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class worksheet
{
    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    public worksheet() { }

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    private worksheetRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    public worksheetRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    private applicationVersionType convertedfromField;

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    public applicationVersionType convertedfrom { get => convertedfromField; set => convertedfromField = value; }

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    private worksheetMessageIdType msgIdField;

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    public worksheetMessageIdType msgId { get => msgIdField; set => msgIdField = value; }

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    private bool convertedfromFieldSpecified;

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    public bool convertedfromSpecified { get => convertedfromFieldSpecified; set => convertedfromFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataworksheetMechanicalDifferentialTests")]
    public worksheetMessageIdType MsgId => msgId;

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<worksheetRegionType>(Enumerable.Cast<worksheetRegionType>(value)); }

    [MirrorVerified("Release IL")]
    public bool WorksheetConvertedFromMC14
    {
        get
        {
            if (convertedfromFieldSpecified)
            {
                return convertedfrom == applicationVersionType.Mathcad14;
            }
            return false;
        }
    }
}
