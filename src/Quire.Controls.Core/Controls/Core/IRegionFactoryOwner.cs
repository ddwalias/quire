#nullable disable
namespace Quire.Controls.Core;

public partial interface IRegionFactoryOwner
{
    IRegionFactory RegionFactory { get; set; }
}
