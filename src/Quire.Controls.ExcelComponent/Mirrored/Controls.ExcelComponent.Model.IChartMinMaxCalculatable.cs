#nullable disable
#pragma warning disable
namespace Quire.Controls.ExcelComponent.Model;

internal partial interface IChartMinMaxCalculatable
{
    IChartComponentCalculatableWrapper GetLimit(ChartWrapperType type);
    bool SetLimit(ChartWrapperType type, IChartComponentCalculatableWrapper wrapper);
}
