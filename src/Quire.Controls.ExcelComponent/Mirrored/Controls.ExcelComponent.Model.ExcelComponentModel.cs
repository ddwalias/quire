using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ExcelComponentModel
{
    public EngineError GetError(ICalculatable calculatable)
    {
        return GetWrapper(calculatable).Error;
    }

    public ExcelComponentEquationType GetEquationType(ICalculatable calculatable)
    {
        return GetExcelComponentCalculatableWrapper(calculatable).ExcelComponentEquationType;
    }

    private IExcelComponentCalculatableWrapper GetExcelComponentCalculatableWrapper(ICalculatable calc)
    {
        return (IExcelComponentCalculatableWrapper)GetWrapper(calc);
    }
}
