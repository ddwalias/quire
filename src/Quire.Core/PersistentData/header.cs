#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class header
{
    [MirrorVerified("PtcPersistentDataheaderMechanicalDifferentialTests")]
    public header() { }

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<headerFooterRegionType>(Enumerable.Cast<headerFooterRegionType>(value)); }
}
