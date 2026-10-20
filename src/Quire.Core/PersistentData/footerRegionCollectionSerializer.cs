#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class footerRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDatafooterRegionCollectionSerializerMechanicalDifferentialTests")]
    public footerRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDatafooterRegionCollectionSerializerMechanicalDifferentialTests")]
    public override string RootNodeName => "footer";

    [MirrorVerified("Release IL")]
    public bool ConvertedFromMC14 => RegionCollection.FooterConvertedFromMC14;
}
