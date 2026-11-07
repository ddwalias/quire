#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial interface IRegionCollectionWithPropertyCreator
{
    IRegionCollectionWithProperty CreateRegionCollection(IEnumerable<IWhiteboardItem> regions);
}
