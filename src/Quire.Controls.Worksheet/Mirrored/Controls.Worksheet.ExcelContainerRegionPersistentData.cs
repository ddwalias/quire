using Quire.Controls.ExcelComponent;

namespace Quire.Controls.Worksheet;

public partial class ExcelContainerRegionPersistentData : RegionPersistentData
{
    public ExcelContainerRegionPersistentData(ExcelContainer excelContainer)
        : base(excelContainer)
    {
    }

    protected override void Deserialize()
    {
        base.Deserialize();
        DeserializeHeight();
    }
}
