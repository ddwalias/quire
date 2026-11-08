#nullable disable
#pragma warning disable
using Quire.Controls.ExcelComponent.ViewModel;

namespace Quire.Controls.ExcelComponent;

public partial interface IExcelContainer
{
    IExcelContainerViewModel ViewModel { get; }
}
