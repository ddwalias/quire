using Quire.Controls.Plots.View;

namespace Quire.Controls.Worksheet;

public partial class PlotRegionPersistentData : RegionPersistentData
{
    public PlotRegionPersistentData(PlotControlBase element)
        : base(element)
    {
    }

    protected override void Deserialize()
    {
        HideFromCalculationContext = true;
        DeserializeDimensions();
        base.Deserialize();
    }

    public override void DeserializeEpilog()
    {
        base.DeserializeEpilog();
        ((PlotControlBase)Control).DeserializeEpilog();
    }
}
