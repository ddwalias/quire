#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class footersContainerWrapper
{
    [MirrorVerified("PtcPersistentDatafootersContainerWrapperMechanicalDifferentialTests")]
    private readonly cachedWorksheetRegionType.footersContainer _container;

    [MirrorVerified("PtcPersistentDatafootersContainerWrapperMechanicalDifferentialTests")]
    public cachedWorksheetRegionType.footersContainer Container => _container;

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => Footer.Regions; set => Footer.Regions = value; }
}
