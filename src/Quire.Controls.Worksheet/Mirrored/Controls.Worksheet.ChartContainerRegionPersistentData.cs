using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.Worksheet;

public partial class ChartContainerRegionPersistentData : RegionPersistentData
{
    public ChartContainerRegionPersistentData(ChartContainer chartContainer)
        : base(chartContainer)
    {
    }

    protected override void Deserialize()
    {
        base.Deserialize();
        DeserializeHeight();
    }
}
