using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.Worksheet;

public partial class ChartComponentPersistentData : ContainerRegionPersistentData
{
    private readonly ChartComponentControl _chartComponent;

    public ChartComponentPersistentData(ChartComponentControl chartComponent)
        : base(chartComponent)
    {
        _chartComponent = chartComponent;
    }

    public override void DeserializeEpilog()
    {
        base.DeserializeEpilog();
        _chartComponent.UpdateAllErrors();
    }
}
