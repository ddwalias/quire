#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class worksheetRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDataWorksheetSerializerTierAMechanicalDifferentialTests")]
    private worksheet _worksheet;

    [MirrorVerified("PtcPersistentDataWorksheetSerializerTierAMechanicalDifferentialTests")]
    public override worksheet RegionCollection
    {
        get
        {
            return _worksheet ?? (_worksheet = new worksheet());
        }
        protected set
        {
            _worksheet = value;
        }
    }

    [MirrorVerified("PtcPersistentDataWorksheetSerializerTierAMechanicalDifferentialTests")]
    public bool ConvertedFromMC14 => RegionCollection.WorksheetConvertedFromMC14;

    [MirrorVerified("PtcPersistentDataWorksheetSerializerTierAMechanicalDifferentialTests")]
    public worksheetMessageIdType MsgId => RegionCollection.MsgId;

    [MirrorVerified("PtcPersistentDataworksheetRegionCollectionSerializerMechanicalDifferentialTests")]
    public worksheetRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDataworksheetRegionCollectionSerializerMechanicalDifferentialTests")]
    public override string RootNodeName => "worksheet";
}
