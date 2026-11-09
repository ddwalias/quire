#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class headerRegionCollectionSerializer
{
    private header _header;

    public override global::Quire.PersistentData.header RegionCollection { get => _header ?? (_header = new global::Quire.PersistentData.header()); protected set => _header = value; }
}
