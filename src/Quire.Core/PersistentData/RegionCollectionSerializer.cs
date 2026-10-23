#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class RegionCollectionSerializer<TCollection>
{
    public abstract string RootNodeName { get; }
    public abstract TCollection RegionCollection { get; protected set; }

    [MirrorVerified("PtcPersistentDataRegionCollectionSerializer1MechanicalDifferentialTests")]
    protected RegionCollectionSerializer() { }

    [MirrorVerified("Release IL")]
    public override IEnumerable<IRegionType> Regions { get => RegionCollection.Regions; set => RegionCollection.Regions = value; }
}
