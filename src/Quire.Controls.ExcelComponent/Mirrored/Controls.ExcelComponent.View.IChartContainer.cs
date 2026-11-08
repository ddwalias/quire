#nullable disable
#pragma warning disable
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.ViewModel;

namespace Quire.Controls.ExcelComponent.View;

public partial interface IChartContainer
{
    IChartContainerViewModel ViewModel { get; }
    IPublicChartContainerModel Model { get; }
}
