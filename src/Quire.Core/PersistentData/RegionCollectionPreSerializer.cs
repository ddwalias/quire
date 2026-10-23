#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class RegionCollectionPreSerializer
{
    public abstract IEnumerable<PersistentDataObjects.IRegionType> Regions { get; set; }

    [MirrorVerified("Release IL")]
    public void PreSerialize()
    {
        if (Regions != null)
        {
            goto HasRegions;
        }
        return;
    HasRegions:
        global::Quire.EnumerableExtensions.Each<global::Quire.PersistentDataObjects.IRegionType>(Regions, item => item.PreSerialize());
    }

    [MirrorVerified("PtcPersistentDataRegionCollectionPreSerializerMechanicalDifferentialTests")]
    protected RegionCollectionPreSerializer() { }
}
