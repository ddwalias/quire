#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class footerRegionCollectionSerializer
{
    private footer _footer;

    public override global::Quire.PersistentData.footer RegionCollection { get => _footer ?? (_footer = new global::Quire.PersistentData.footer()); protected set => _footer = value; }
}
