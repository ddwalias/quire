#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class footersContainerWrapper
{
    [MirrorVerified("PtcPersistentDatafootersContainerWrapperMechanicalDifferentialTests")]
    private readonly cachedWorksheetRegionType.footersContainer _container;

    [MirrorVerified("PtcPersistentDatafootersContainerWrapperMechanicalDifferentialTests")]
    public cachedWorksheetRegionType.footersContainer Container => _container;
}
