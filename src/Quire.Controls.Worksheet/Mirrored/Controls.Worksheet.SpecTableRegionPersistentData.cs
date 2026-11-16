using Quire.Controls;

namespace Quire.Controls.Worksheet;

public partial class SpecTableRegionPersistentData : RegionPersistentData
{
    public SpecTableRegionPersistentData(SpecTableControl tbl)
        : base(tbl)
    {
    }

    protected override void Deserialize()
    {
        HideFromCalculationContext = true;
        base.Deserialize();
    }
}
