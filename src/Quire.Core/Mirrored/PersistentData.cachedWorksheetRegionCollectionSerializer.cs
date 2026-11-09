#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class cachedWorksheetRegionCollectionSerializer
{
    private cachedWorksheetType _cachedWorksheet;

    public override global::Quire.PersistentData.cachedWorksheetType RegionCollection { get => _cachedWorksheet ?? (_cachedWorksheet = new global::Quire.PersistentData.cachedWorksheetType()); protected set => _cachedWorksheet = value; }
}
