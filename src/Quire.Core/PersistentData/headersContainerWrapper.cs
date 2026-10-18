#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class headersContainerWrapper
{
    [MirrorVerified("PtcPersistentDataheadersContainerWrapperMechanicalDifferentialTests")]
    private readonly cachedWorksheetRegionType.headersContainer _container;

    [MirrorVerified("PtcPersistentDataheadersContainerWrapperMechanicalDifferentialTests")]
    public cachedWorksheetRegionType.headersContainer Container => _container;
}
