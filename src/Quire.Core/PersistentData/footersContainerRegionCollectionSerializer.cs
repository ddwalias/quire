#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class footersContainerRegionCollectionSerializer
{
    [MirrorVerified("PtcPersistentDatafootersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public footersContainerRegionCollectionSerializer() { }

    [MirrorVerified("PtcPersistentDatafootersContainerRegionCollectionSerializerMechanicalDifferentialTests")]
    public string RootNodeName => "footers";

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => ContainerWrapper.Regions; set => ContainerWrapper.Regions = value; }
}
