using Quire.Controls.ExcelComponent;

namespace Quire.Controls.Worksheet;

public partial class ExcelComponentPersistentData : ContainerRegionPersistentData
{
    private readonly ExcelComponentControl _excelComponent;

    public ExcelComponentPersistentData(ExcelComponentControl excelComponent)
        : base(excelComponent)
    {
        _excelComponent = excelComponent;
    }

    public override void DeserializeEpilog()
    {
        base.DeserializeEpilog();
        _excelComponent.UpdateAllErrors();
    }
}
