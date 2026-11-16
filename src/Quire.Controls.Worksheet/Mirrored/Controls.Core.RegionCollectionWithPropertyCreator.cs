using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial class RegionCollectionWithPropertyCreator
{
    public bool IsConnected { get; set; }

    public IRegionCollectionWithProperty CreateRegionCollection(IEnumerable<IWhiteboardItem> regions)
    {
        return new RegionCollectionWithProperty(regions);
    }
}
