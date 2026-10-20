#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class worksheetRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDataworksheetRegionCollectionSerializerMechanicalDifferentialTests")]
    public worksheetRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDataworksheetRegionCollectionSerializerMechanicalDifferentialTests")]
    public override string RootNodeName => "worksheet";

    [MirrorVerified("Release IL")]
    public bool ConvertedFromMC14 => RegionCollection.WorksheetConvertedFromMC14;

    [MirrorVerified("Release IL")]
    public worksheetMessageIdType MsgId => RegionCollection.MsgId;
}
