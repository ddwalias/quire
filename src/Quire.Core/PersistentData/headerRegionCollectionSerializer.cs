#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class headerRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDataheaderRegionCollectionSerializerMechanicalDifferentialTests")]
    public headerRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDataheaderRegionCollectionSerializerMechanicalDifferentialTests")]
    public override string RootNodeName => "header";

    [MirrorVerified("Release IL")]
    public bool ConvertedFromMC14 => RegionCollection.HeaderConvertedFromMC14;
}
