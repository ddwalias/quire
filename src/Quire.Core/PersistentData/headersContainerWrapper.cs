#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class headersContainerWrapper
{
    public headersContainerWrapper()
    {
        _container = new cachedWorksheetRegionType.headersContainer
        {
            header = new headerFooterRegions(),
            firstPageHeader = new headerFooterRegions()
        };
    }

    [MirrorVerified("PtcPersistentDataheadersContainerWrapperMechanicalDifferentialTests")]
    private readonly cachedWorksheetRegionType.headersContainer _container;

    [MirrorVerified("PtcPersistentDataheadersContainerWrapperMechanicalDifferentialTests")]
    public cachedWorksheetRegionType.headersContainer Container => _container;

    [MirrorVerified("Release IL")]
    public IEnumerable<IRegionType> Regions { get => Header.Regions; set => Header.Regions = value; }
}
